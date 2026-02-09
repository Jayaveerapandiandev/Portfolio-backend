using Newtonsoft.Json;
using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using System.Data;

namespace Portfolio_Api.Bll
{
    public class EducationBLL
    {
        private readonly DatabaseHelper _db;

        public EducationBLL()
        {
            _db = new DatabaseHelper();
        }

        // 🔹 Add Education
        public async Task<EducationResponse> AddEducationAsync(EducationRequest request)
        {
            var response = new EducationResponse();

            try
            {
                string sql = @"
                INSERT INTO portfolio_education
                (
                    education_type,
                    institute_name,
                    department,
                    percentage,
                    start_year,
                    end_year,
                    location,
                    description,
                    highlights,
                    institute_logo_url,
                    institute_logo_public_id,
                    display_order,
                    is_visible,
                    created_at,
                    updated_at
                )
                VALUES
                (
                    @type,
                    @institute,
                    @dept,
                    @percentage,
                    @start,
                    @end,
                    @loc,
                    @desc,
                    @highlights::jsonb,
                    @logoUrl,
                    @logoPid,
                    @order,
                    @visible,
                    NOW(),
                    NOW()
                )
                RETURNING id;
            ";

                var parameters = new[]
                {
                new NpgsqlParameter("@type", request.EducationType),
                new NpgsqlParameter("@institute", request.InstituteName),
                new NpgsqlParameter("@dept", request.Department ?? ""),
                new NpgsqlParameter("@percentage", request.Percentage ?? ""),
                new NpgsqlParameter("@start", request.StartYear ?? ""),
                new NpgsqlParameter("@end", request.EndYear ?? ""),
                new NpgsqlParameter("@loc", request.Location ?? ""),
                new NpgsqlParameter("@desc", request.Description ?? ""),
                new NpgsqlParameter("@highlights", JsonConvert.SerializeObject(request.Highlights ?? new List<string>())),
                new NpgsqlParameter("@logoUrl", request.InstituteLogoUrl ?? ""),
                new NpgsqlParameter("@logoPid", request.InstituteLogoPublicId ?? ""),
                new NpgsqlParameter("@order", request.Order),
                new NpgsqlParameter("@visible", request.IsVisible)
            };

                var dt = await _db.ExecuteQueryAsync(sql, parameters);

                response.Success = true;
                response.Message = "Education added successfully";
                response.Id = dt.Rows[0]["id"].ToString();
                response.CreatedAt = DateTime.UtcNow;
                response.UpdatedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<GetEducationResponse> GetEducationAsync()
        {
            var response = new GetEducationResponse
            {
                Data = new List<EducationItemResponse>()
            };

            try
            {
                string sql = @"
                SELECT 
                    id,
                    education_type,
                    institute_name,
                    department,
                    percentage,
                    start_year,
                    end_year,
                    location,
                    description,
                    highlights,
                    institute_logo_url,
                    institute_logo_public_id,
                    display_order,
                    is_visible,
                    created_at,
                    updated_at
                FROM portfolio_education
                ORDER BY display_order ASC, start_year DESC;
            ";

                var dt = await _db.ExecuteQueryAsync(sql);

                foreach (DataRow row in dt.Rows)
                {
                    response.Data.Add(new EducationItemResponse
                    {
                        Id = row["id"].ToString(),
                        EducationType = row["education_type"].ToString(),
                        InstituteName = row["institute_name"].ToString(),
                        Department = row["department"].ToString(),
                        Percentage = row["percentage"].ToString(),
                        StartYear = row["start_year"].ToString(),
                        EndYear = row["end_year"].ToString(),
                        Location = row["location"].ToString(),
                        Description = row["description"].ToString(),

                        Highlights = JsonConvert.DeserializeObject<List<string>>(
                            row["highlights"]?.ToString() ?? "[]"
                        ),

                        InstituteLogo = new InstituteLogoResponse
                        {
                            Url = row["institute_logo_url"].ToString(),
                            PublicId = row["institute_logo_public_id"].ToString()
                        },

                        Order = Convert.ToInt32(row["display_order"]),
                        IsVisible = Convert.ToBoolean(row["is_visible"]),

                        CreatedAt = Convert.ToDateTime(row["created_at"]),
                        UpdatedAt = Convert.ToDateTime(row["updated_at"])
                    });
                }

                response.Success = true;
                response.Message = "Education fetched successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<DeleteEducationResponse> DeleteEducationAsync(string id)
        {
            var response = new DeleteEducationResponse();

            try
            {
                // ✅ Test case 1: Invalid UUID format
                if (!Guid.TryParse(id, out _))
                {
                    response.Success = false;
                    response.Message = "Invalid education id format";
                    return response;
                }

                // ✅ Test case 2: Check record exists
                string checkSql = "SELECT COUNT(*) FROM portfolio_education WHERE id = @id;";
                var checkParams = new[]
                {
                    new NpgsqlParameter("@id", Guid.Parse(id))
                };

                var dt = await _db.ExecuteQueryAsync(checkSql, checkParams);

                if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                {
                    response.Success = false;
                    response.Message = "Education record not found";
                    return response;
                }

                var deleteparams = new[]
                {
                new NpgsqlParameter("@id", Guid.Parse(id))
                 };

                // ✅ Test case 3: Delete record
                string deleteSql = "DELETE FROM portfolio_education WHERE id = @id;";
                int rows = await _db.ExecuteNonQueryAsync(deleteSql, deleteparams);

                // ✅ Test case 4: Deletion failed
                if (rows == 0)
                {
                    response.Success = false;
                    response.Message = "Failed to delete education";
                    return response;
                }

                response.Success = true;
                response.Message = "Education deleted successfully";
            }
            catch (Exception ex)
            {
                // ✅ Test case 5: Unexpected DB / server error
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<UpdateEducationResponse> UpdateEducationAsync(string id, EducationRequest request)
        {
            var response = new UpdateEducationResponse();

            try
            {
                // ✅ Test case 1: Invalid UUID
                if (!Guid.TryParse(id, out Guid educationId))
                {
                    response.Success = false;
                    response.Message = "Invalid education id format";
                    return response;
                }

                // ✅ Test case 2: Check record exists
                string checkSql = "SELECT COUNT(*) FROM portfolio_education WHERE id = @id;";
                var checkParams = new[]
                {
                new NpgsqlParameter("@id", educationId)
            };

                var dt = await _db.ExecuteQueryAsync(checkSql, checkParams);

                if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                {
                    response.Success = false;
                    response.Message = "Education record not found";
                    return response;
                }

                // ✅ Test case 3: Update record
                string updateSql = @"
                UPDATE portfolio_education
                SET
                    education_type = @type,
                    institute_name = @institute,
                    department = @dept,
                    percentage = @percentage,
                    start_year = @start,
                    end_year = @end,
                    location = @loc,
                    description = @desc,
                    highlights = @highlights::jsonb,
                    institute_logo_url = @logoUrl,
                    institute_logo_public_id = @logoPid,
                    display_order = @order,
                    is_visible = @visible,
                    updated_at = NOW()
                WHERE id = @id;
            ";

                var updateParams = new[]
                {
                new NpgsqlParameter("@type", request.EducationType),
                new NpgsqlParameter("@institute", request.InstituteName),
                new NpgsqlParameter("@dept", request.Department ?? ""),
                new NpgsqlParameter("@percentage", request.Percentage ?? ""),
                new NpgsqlParameter("@start", request.StartYear ?? ""),
                new NpgsqlParameter("@end", request.EndYear ?? ""),
                new NpgsqlParameter("@loc", request.Location ?? ""),
                new NpgsqlParameter("@desc", request.Description ?? ""),
                new NpgsqlParameter("@highlights", JsonConvert.SerializeObject(request.Highlights ?? new List<string>())),
                new NpgsqlParameter("@logoUrl", request.InstituteLogoUrl ?? ""),
                new NpgsqlParameter("@logoPid", request.InstituteLogoPublicId ?? ""),
                new NpgsqlParameter("@order", request.Order),
                new NpgsqlParameter("@visible", request.IsVisible),
                new NpgsqlParameter("@id", educationId)
            };

                int rows = await _db.ExecuteNonQueryAsync(updateSql, updateParams);

                // ✅ Test case 4: Update failed
                if (rows == 0)
                {
                    response.Success = false;
                    response.Message = "Failed to update education";
                    return response;
                }

                response.Success = true;
                response.Message = "Education updated successfully";
                response.UpdatedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                // ✅ Test case 5: Unexpected error
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }

}
