using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using System.Data;

namespace Portfolio_Api.Bll
{
    public class ExperienceBLL
    {
        private readonly DatabaseHelper _db;

        public ExperienceBLL()
        {
            _db = new DatabaseHelper();
        }

        public async Task<CompanyWithExperiencesResponse> GetAllCompaniesWithExperiencesAsync()
        {
            var response = new CompanyWithExperiencesResponse();

            try
            {
                string query = @"
                    SELECT 
                        c.id AS company_id, 
                        c.name AS company_name,
                        c.location AS company_location,
                        c.website AS company_website,
                        c.logo_url AS company_logo_url,
                        e.id AS experience_id,
                        e.designation,
                        e.position_title,
                        e.start_date,
                        e.end_date,
                        e.is_current_company,
                        e.description,
                        e.technologies_used
                    FROM companies c
                    LEFT JOIN experiences e ON e.company_id = c.id
                    ORDER BY c.name ASC, e.start_date DESC;";

                var dt = await _db.ExecuteQueryAsync(query);

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No companies found.";
                    response.DataList = new List<CompanyExperienceData>();
                    return response;
                }

                var companyMap = new Dictionary<int, CompanyExperienceData>();

                foreach (DataRow row in dt.Rows)
                {
                    int companyId = Convert.ToInt32(row["company_id"]);

                    if (!companyMap.ContainsKey(companyId))
                    {
                        companyMap[companyId] = new CompanyExperienceData
                        {
                            Id = companyId,
                            Name = row["company_name"].ToString() ?? "",
                            Location = row["company_location"]?.ToString(),
                            Website = row["company_website"]?.ToString(),
                            LogoUrl = row["company_logo_url"]?.ToString(),
                            Experiences = new List<ExperienceDetailData>()
                        };
                    }

                    if (row["experience_id"] != DBNull.Value)
                    {
                        string? startDateStr = row["start_date"] != DBNull.Value
                           ? Convert.ToDateTime(row["start_date"]).ToString("dd-MM-yyyy")
                           : null;

                        string? endDateStr = row["end_date"] != DBNull.Value
                            ? Convert.ToDateTime(row["end_date"]).ToString("dd-MM-yyyy")
                            : null;
                        var experience = new ExperienceDetailData
                        {
                            Id = Convert.ToInt32(row["experience_id"]),
                            Designation = row["designation"]?.ToString() ?? "",
                            PositionTitle = row["position_title"]?.ToString(),
                            StartDate = startDateStr,
                            EndDate = endDateStr,
                            IsCurrentCompany = Convert.ToBoolean(row["is_current_company"]),
                            Description = row["description"]?.ToString(),
                            TechnologiesUsed = row["technologies_used"]?.ToString()
                        };

                        companyMap[companyId].Experiences.Add(experience);
                    }
                }

                response.Success = true;
                response.Message = "Companies with experiences fetched successfully.";
                response.DataList = companyMap.Values.ToList();
            }
            catch (NpgsqlException npgEx)
            {
                response.Success = false;
                response.Message = $"Database error while fetching companies: {npgEx.Message}";
                response.DataList = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.DataList = null;
            }

            return response;
        }



        public async Task<AddExperienceResponse> AddExperienceAsync(AddExperienceRequest request)
        {
            var response = new AddExperienceResponse();

            if (string.IsNullOrWhiteSpace(request.Company.Name))
            {
                response.Success = false;
                response.Message = "Failed to add experience. Company name is required.";
                response.Data = null;
                return response;
            }

            try
            {
                using var db = new DatabaseHelper();

                // 🔹 Check if the company already exists
                string checkCompanyQuery = "SELECT id FROM companies WHERE LOWER(name) = LOWER(@name);";
                var existingCompany = await db.ExecuteQueryAsync(checkCompanyQuery, new NpgsqlParameter("@name", request.Company.Name));

                int companyId;
                if (existingCompany.Rows.Count > 0)
                {
                    // Use existing company
                    companyId = Convert.ToInt32(existingCompany.Rows[0]["id"]);
                }
                else
                {
                    // Insert new company
                    string insertCompanyQuery = @"
                        INSERT INTO companies (name, location, website, logo_url, created_at)
                        VALUES (@name, @location, @website, @logo_url, NOW())
                        RETURNING id;";

                    var companyResult = await db.ExecuteQueryAsync(insertCompanyQuery,
                        new NpgsqlParameter("@name", request.Company.Name),
                        new NpgsqlParameter("@location", (object?)request.Company.Location ?? DBNull.Value),
                        new NpgsqlParameter("@website", (object?)request.Company.Website ?? DBNull.Value),
                        new NpgsqlParameter("@logo_url", (object?)request.Company.LogoUrl ?? DBNull.Value));

                    companyId = Convert.ToInt32(companyResult.Rows[0]["id"]);
                }

                // 🔹 Insert experience
                string insertExperienceQuery = @"
                    INSERT INTO experiences
                    (company_id, designation, position_title, start_date, end_date, is_current_company, description, technologies_used, parent_experience_id)
                    VALUES
                    (@company_id, @designation, @position_title, @start_date, @end_date, @is_current_company, @description, @technologies_used, @parent_experience_id)
                    RETURNING id;";

                var experienceResult = await db.ExecuteQueryAsync(insertExperienceQuery,
                    new NpgsqlParameter("@company_id", companyId),
                    new NpgsqlParameter("@designation", request.Experience.Designation),
                    new NpgsqlParameter("@position_title", (object?)request.Experience.PositionTitle ?? DBNull.Value),
                    new NpgsqlParameter("@start_date", request.Experience.StartDate),
                    new NpgsqlParameter("@end_date", (object?)request.Experience.EndDate ?? DBNull.Value),
                    new NpgsqlParameter("@is_current_company", request.Experience.IsCurrentCompany),
                    new NpgsqlParameter("@description", (object?)request.Experience.Description ?? DBNull.Value),
                    new NpgsqlParameter("@technologies_used", (object?)request.Experience.TechnologiesUsed ?? DBNull.Value),
                    new NpgsqlParameter("@parent_experience_id", (object?)request.Experience.ParentExperienceId ?? DBNull.Value)
                );

                int experienceId = Convert.ToInt32(experienceResult.Rows[0]["id"]);

                // ✅ Success response
                response.Success = true;
                response.Message = "Experience added successfully";
                response.Data = new ExperienceCreatedData
                {
                    CompanyId = companyId,
                    ExperienceId = experienceId,
                    Company = new CompanySummary
                    {
                        Name = request.Company.Name,
                        Location = request.Company.Location,
                        Website = request.Company.Website,
                        LogoUrl = request.Company.LogoUrl
                    },
                    Experience = new ExperienceSummary
                    {
                        Designation = request.Experience.Designation,
                        PositionTitle = request.Experience.PositionTitle,
                        StartDate = request.Experience.StartDate,
                        EndDate = request.Experience.EndDate,
                        IsCurrentCompany = request.Experience.IsCurrentCompany,
                        Description = request.Experience.Description,
                        TechnologiesUsed = request.Experience.TechnologiesUsed
                    }
                };
            }
            catch (NpgsqlException ex)
            {
                response.Success = false;
                response.Message = $"Database error while adding experience: {ex.Message}";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.Data = null;
            }

            return response;
        }

        public async Task<AddExperienceForExistingCompanyResponse> AddExperienceForExistingCompanyAsync(AddExperienceForExistingCompanyRequest req)
        {
            var response = new AddExperienceForExistingCompanyResponse();

            try
            {
                using var db = new DatabaseHelper();

                // 🔹 1. Check if company exists
                string checkCompany = "SELECT id FROM companies WHERE id = @company_id;";
                var dt = await db.ExecuteQueryAsync(checkCompany, new NpgsqlParameter("@company_id", req.CompanyId));

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Company not found";
                    response.Data = null;
                    return response;
                }

                // 🔹 2. Insert experience record for that company
                string insertExperience = @"
                    INSERT INTO experiences
                    (company_id, designation, position_title, start_date, end_date, 
                     is_current_company, description, technologies_used, parent_experience_id)
                    VALUES
                    (@company_id, @designation, @position_title, @start_date, @end_date,
                     @is_current_company, @description, @technologies_used, @parent_experience_id)
                    RETURNING id;";

                var experienceIdObj = await db.ExecuteScalarAsync(insertExperience,
                    new NpgsqlParameter("@company_id", req.CompanyId),
                    new NpgsqlParameter("@designation", req.Experience.Designation),
                    new NpgsqlParameter("@position_title", (object?)req.Experience.PositionTitle ?? DBNull.Value),
                    new NpgsqlParameter("@start_date", req.Experience.StartDate),
                    new NpgsqlParameter("@end_date", (object?)req.Experience.EndDate ?? DBNull.Value),
                    new NpgsqlParameter("@is_current_company", req.Experience.IsCurrentCompany),
                    new NpgsqlParameter("@description", (object?)req.Experience.Description ?? DBNull.Value),
                    new NpgsqlParameter("@technologies_used", (object?)req.Experience.TechnologiesUsed ?? DBNull.Value),
                    new NpgsqlParameter("@parent_experience_id", (object?)req.Experience.ParentExperienceId ?? DBNull.Value)
                );

                int experienceId = Convert.ToInt32(experienceIdObj);

                // 🔹 3. Build successful response
                response.Success = true;
                response.Message = "Experience added successfully";
                response.Data = new ExperienceAddedData
                {
                    ExperienceId = experienceId,
                    CompanyId = req.CompanyId,
                    Designation = req.Experience.Designation,
                    PositionTitle = req.Experience.PositionTitle,
                    StartDate = req.Experience.StartDate,
                    EndDate = req.Experience.EndDate,
                    IsCurrentCompany = req.Experience.IsCurrentCompany
                };
            }
            catch (NpgsqlException npgEx)
            {
                response.Success = false;
                response.Message = $"Database error while adding experience: {npgEx.Message}";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.Data = null;
            }

            return response;
        }

        public async Task<DeleteCompanyResponse> DeleteCompanyAsync(int companyId)
        {
            var response = new DeleteCompanyResponse();

            try
            {
                using var db = new DatabaseHelper();

                // 🔹 Check if the company exists
                string checkCompany = "SELECT id FROM companies WHERE id = @id;";
                var dt = await db.ExecuteQueryAsync(checkCompany, new NpgsqlParameter("@id", companyId));

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Company not found";
                    response.DeletedCompanyId = null;
                    return response;
                }

                // 🔹 Delete experiences linked to the company first
                string deleteExperiences = "DELETE FROM experiences WHERE company_id = @company_id;";
                await db.ExecuteNonQueryAsync(deleteExperiences, new NpgsqlParameter("@company_id", companyId));

                // 🔹 Delete the company itself
                string deleteCompany = "DELETE FROM companies WHERE id = @id;";
                await db.ExecuteNonQueryAsync(deleteCompany, new NpgsqlParameter("@id", companyId));

                // ✅ Success response
                response.Success = true;
                response.Message = "Company deleted successfully";
                response.DeletedCompanyId = companyId;
            }
            catch (NpgsqlException ex)
            {
                response.Success = false;
                response.Message = $"Database error while deleting company: {ex.Message}";
                response.DeletedCompanyId = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.DeletedCompanyId = null;
            }

            return response;
        }

        public async Task<DeleteExperienceResponse> DeleteExperienceByIdAsync(int experienceId)
        {
            var response = new DeleteExperienceResponse();

            if (experienceId <= 0)
            {
                response.Success = false;
                response.Message = "Invalid experience ID.";
                response.Data = null;
                return response;
            }

            try
            {
                using var db = new DatabaseHelper();

                // 🔹 Check if experience exists
                string checkQuery = "SELECT COUNT(*) FROM experiences WHERE id = @id;";
                var countResult = await db.ExecuteScalarAsync(checkQuery, new NpgsqlParameter("@id", experienceId));
                int count = Convert.ToInt32(countResult);

                if (count == 0)
                {
                    response.Success = false;
                    response.Message = "Experience not found.";
                    response.Data = null;
                    return response;
                }

                // 🔹 Delete the experience record
                string deleteQuery = "DELETE FROM experiences WHERE id = @id;";
                await db.ExecuteNonQueryAsync(deleteQuery, new NpgsqlParameter("@id", experienceId));

                response.Success = true;
                response.Message = "Experience position deleted successfully";
                response.Data = new DeletedExperienceData
                {
                    ExperienceId = experienceId
                };
            }
            catch (NpgsqlException ex)
            {
                response.Success = false;
                response.Message = $"Database error while deleting experience: {ex.Message}";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Unexpected error: {ex.Message}";
                response.Data = null;
            }

            return response;
        }

    }
}
