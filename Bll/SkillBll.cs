using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using System.Data;

namespace Portfolio_Api.Bll
{
    public class SkillBll
    {
        private readonly DatabaseHelper _db;

        public SkillBll()
        {
            _db = new DatabaseHelper();
        }

        public async Task<SkillResponse> GetAllSkillsAsync()
        {
            var response = new SkillResponse();

            try
            {
                string sql = @"SELECT id, name, category, proficiency, experience_years, logo_url, is_highlighted FROM admin_skills ORDER BY id ASC";

                var dt = await _db.ExecuteQueryAsync(sql);

                var skills = new List<SkillData>();

                foreach (DataRow row in dt.Rows)
                {
                    skills.Add(new SkillData
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["name"]?.ToString() ?? string.Empty,
                        Category = row["category"]?.ToString() ?? string.Empty,
                        Proficiency = Convert.ToInt32(row["proficiency"]),
                        ExperienceYears = Convert.ToInt32(row["experience_years"]),
                        LogoUrl = row["logo_url"] == DBNull.Value ? null : row["logo_url"].ToString(),
                        IsHighlighted = Convert.ToBoolean(row["is_highlighted"])
                    });
                }

                response.Success = true;
                response.Message = "Skills retrieved successfully";
                response.Data = skills;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }


        public async Task<CreateSkillResponse> CreateSkillAsync(CreateSkillRequest request)
        {
            var response = new CreateSkillResponse();

            try
            {
                // 1️⃣ Duplicate check (case-insensitive)
                string checkSql = @"SELECT id FROM admin_skills WHERE LOWER(name) = LOWER(@name) AND LOWER(category) = LOWER(@category) LIMIT 1;";

                var checkDt = await _db.ExecuteQueryAsync(
                    checkSql,
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@category", request.Category)
                );

                if (checkDt.Rows.Count > 0)
                {
                    response.Success = false;
                    response.Message = "A skill with the same name and category already exists.";
                    return response;
                }

                // 2️⃣ Insert new skill
                string insertSql = @"INSERT INTO admin_skills (name, category, proficiency, experience_years, logo_url, is_highlighted) VALUES (@name, @category, @proficiency, @experience_years, @logo_url, FALSE) RETURNING id;";

                var dt = await _db.ExecuteQueryAsync(
                    insertSql,
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@category", request.Category),
                    new NpgsqlParameter("@proficiency", request.Proficiency),
                    new NpgsqlParameter("@experience_years", request.ExperienceYears),
                    new NpgsqlParameter("@logo_url",
                        string.IsNullOrWhiteSpace(request.LogoUrl) ? (object)DBNull.Value : request.LogoUrl
                    )
                );

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Failed to create skill.";
                    return response;
                }

                int newId = Convert.ToInt32(dt.Rows[0]["id"]);

                // 3️⃣ Build response
                response.Success = true;
                response.Message = "Skill created successfully";
                response.Data = new SkillData
                {
                    Id = newId,
                    Name = request.Name,
                    Category = request.Category,
                    Proficiency = request.Proficiency,
                    ExperienceYears = request.ExperienceYears,
                    LogoUrl = request.LogoUrl,
                    IsHighlighted = false
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<UpdateSkillResponse> UpdateSkillAsync(int id, UpdateSkillRequest request)
        {
            var response = new UpdateSkillResponse();

            try
            {
                // 1️⃣ Check if skill exists
                string existsSql = "SELECT id FROM admin_skills WHERE id = @id;";
                var existsDt = await _db.ExecuteQueryAsync(
                    existsSql,
                    new NpgsqlParameter("@id", id)
                );

                if (existsDt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                // 2️⃣ Duplicate name/category check (exclude current id)
                string duplicateSql = @"SELECT id FROM admin_skills WHERE LOWER(name) = LOWER(@name) AND LOWER(category) = LOWER(@category) AND id != @id LIMIT 1;";

                var dupDt = await _db.ExecuteQueryAsync(
                    duplicateSql,
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@category", request.Category),
                    new NpgsqlParameter("@id", id)
                );

                if (dupDt.Rows.Count > 0)
                {
                    response.Success = false;
                    response.Message = "Another skill with the same name and category already exists.";
                    return response;
                }

                // 3️⃣ Update query
                string updateSql = @"UPDATE admin_skills SET name = @name, category = @category, proficiency = @proficiency, experience_years = @experience_years, logo_url = @logo_url, is_highlighted = @is_highlighted, updated_at = NOW() WHERE id = @id;";

                await _db.ExecuteQueryAsync(
                    updateSql,
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@category", request.Category),
                    new NpgsqlParameter("@proficiency", request.Proficiency),
                    new NpgsqlParameter("@experience_years", request.ExperienceYears),
                    new NpgsqlParameter("@logo_url",
                        string.IsNullOrWhiteSpace(request.LogoUrl) ? (object)DBNull.Value : request.LogoUrl),
                    new NpgsqlParameter("@is_highlighted", request.IsHighlighted)
                );

                // 4️⃣ Build final response
                response.Success = true;
                response.Message = "Skill updated successfully";
                response.Data = new SkillData
                {
                    Id = id,
                    Name = request.Name,
                    Category = request.Category,
                    Proficiency = request.Proficiency,
                    ExperienceYears = request.ExperienceYears,
                    LogoUrl = request.LogoUrl,
                    IsHighlighted = request.IsHighlighted
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<DeleteResponse> DeleteSkillAsync(int id)
        {
            var response = new DeleteResponse();

            try
            {
                // 1️⃣ Check if skill exists
                string checkSql = "SELECT id FROM admin_skills WHERE id = @id;";
                var checkDt = await _db.ExecuteQueryAsync(
                    checkSql,
                    new NpgsqlParameter("@id", id)
                );

                if (checkDt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                // 2️⃣ Perform delete
                string deleteSql = "DELETE FROM admin_skills WHERE id = @id;";

                await _db.ExecuteQueryAsync(
                    deleteSql,
                    new NpgsqlParameter("@id", id)
                );

                // 3️⃣ Success response
                response.Success = true;
                response.Message = "Skill deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }





    }
}
