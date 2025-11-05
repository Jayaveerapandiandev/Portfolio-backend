using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;

namespace Portfolio_Api.Bll
{
    public class AboutBLL
    {
        private readonly DatabaseHelper _db;

        public AboutBLL()
        {
            _db = new DatabaseHelper();
        }

        // 🔹 Add or Update About Info
        public async Task<AboutResponse> SaveAboutAsync(AboutRequest request)
        {
            var response = new AboutResponse();

            try
            {
                string checkSql = "SELECT COUNT(*) FROM portfolio_about;";
                var dt = await _db.ExecuteQueryAsync(checkSql);

                bool exists = Convert.ToInt32(dt.Rows[0][0]) > 0;

                string sql;

                if (exists)
                {
                    // 🔹 Update existing record (only 1 row expected)
                    sql = "UPDATE portfolio_about SET full_name = @name, role = @role, short_bio = @bio, description = @desc, profile_image_url = @img, email = @mail, location = @loc, resume_url = @resume, updated_at = NOW() WHERE id = (SELECT id FROM portfolio_about LIMIT 1);";
                }
                else
                {
                    sql = "INSERT INTO portfolio_about (full_name, role, short_bio, description, profile_image_url, email, location, resume_url, created_at, updated_at) VALUES (@name, @role, @bio, @desc, @img, @mail, @loc, @resume, NOW(), NOW());";
                }

                var parameters = new[]
                {
                    new NpgsqlParameter("@name", request.FullName),
                    new NpgsqlParameter("@role", request.Role),
                    new NpgsqlParameter("@bio", request.ShortBio),
                    new NpgsqlParameter("@desc", request.Description),
                    new NpgsqlParameter("@img", request.ProfileImageUrl),
                    new NpgsqlParameter("@mail", request.Email),
                    new NpgsqlParameter("@loc", request.Location),
                    new NpgsqlParameter("@resume", request.ResumeUrl)
                };

                int rows = await _db.ExecuteNonQueryAsync(sql, parameters);

                response.Success = rows > 0;
                response.Message = exists ? "About info updated successfully." : "About info added successfully.";
                response.CreatedAt = !exists ? DateTime.UtcNow : null;
                response.UpdatedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }


        // 🔹 Get About Info
        public async Task<AboutResponse> GetAboutAsync()
        {
            var response = new AboutResponse();

            try
            {
                string sql = "SELECT * FROM portfolio_about LIMIT 1;";
                var dt = await _db.ExecuteQueryAsync(sql);

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No data found.";
                    return response;
                }

                var row = dt.Rows[0];
                response.Success = true;
                response.Data = new
                {
                    Id = row["id"],
                    FullName = row["full_name"],
                    Role = row["role"],
                    ShortBio = row["short_bio"],
                    Description = row["description"],
                    ProfileImageUrl = row["profile_image_url"],
                    Email = row["email"],
                    Location = row["location"],
                    ResumeUrl = row["resume_url"],
                    CreatedAt = row["created_at"],
                    UpdatedAt = row["updated_at"]
                };
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
