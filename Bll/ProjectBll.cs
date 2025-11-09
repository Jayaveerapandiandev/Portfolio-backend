using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using System.Data;
namespace Portfolio_Api.Bll
{
    public class ProjectBLL
    {
        private readonly DatabaseHelper _db;

        public ProjectBLL()
        {
            _db = new DatabaseHelper();
        }

        // 🔹 Get all projects
        public async Task<ProjectResponse> GetProjectsAsync()
        {
            var response = new ProjectResponse();
            try
            {
                string sql = "SELECT * FROM portfolio_projects ORDER BY created_at DESC;";
                var dt = await _db.ExecuteQueryAsync(sql);

                response.DataList = dt.AsEnumerable().Select(row => new ProjectData
                {
                    Id = Convert.ToInt32(row["id"]),
                    Title = row["title"]?.ToString(),
                    Subtitle = row["subtitle"]?.ToString(),
                    Description = row["description"]?.ToString(),
                    Type = row["type"]?.ToString(),
                    Technologies = ((string[])row["technologies"]).ToList(),
                    GithubUrl = row["github_url"]?.ToString(),
                    LiveUrl = row["live_url"]?.ToString(),
                    ImageUrl = row["image_url"]?.ToString(),
                    Featured = Convert.ToBoolean(row["featured"]),
                    DateFrom = row["date_from"]?.ToString(),
                    DateTo = row["date_to"]?.ToString(),
                    Status = row["status"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(row["created_at"]),
                    UpdatedAt = Convert.ToDateTime(row["updated_at"])
                }).ToList();

                response.Success = true;
                
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
                
            }

            return response;
        }

        // 🔹 Add a new project
        public async Task<ProjectResponse> AddProjectAsync(ProjectRequest req)
        {
            var response = new ProjectResponse();
            try
            {
                string sql = @"INSERT INTO portfolio_projects (title, subtitle, description, type, technologies, github_url, live_url, image_url, featured, date_from, date_to, status, created_at, updated_at) VALUES (@title, @subtitle, @desc, @type, @tech, @git, @live, @img, @feat, @from, @to, @status, NOW(), NOW()) RETURNING *;";

                var parameters = new[]
                {
                new NpgsqlParameter("@title", req.Title),
                new NpgsqlParameter("@subtitle", req.Subtitle ?? (object)DBNull.Value),
                new NpgsqlParameter("@desc", req.Description ?? (object)DBNull.Value),
                new NpgsqlParameter("@type", req.Type ?? (object)DBNull.Value),
                new NpgsqlParameter("@tech", req.Technologies?.ToArray() ?? new string[]{}),
                new NpgsqlParameter("@git", req.GithubUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@live", req.LiveUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@img", req.ImageUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@feat", req.Featured),
                new NpgsqlParameter("@from", req.DateFrom ?? (object)DBNull.Value),
                new NpgsqlParameter("@to", req.DateTo ?? (object)DBNull.Value),
                new NpgsqlParameter("@status", req.Status ?? (object)DBNull.Value)
            };

                var dt = await _db.ExecuteQueryAsync(sql, parameters);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    response.Data = new ProjectData
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Title = row["title"]?.ToString(),
                        Subtitle = row["subtitle"]?.ToString(),
                        Description = row["description"]?.ToString(),
                        Type = row["type"]?.ToString(),
                        Technologies = ((string[])row["technologies"]).ToList(),
                        GithubUrl = row["github_url"]?.ToString(),
                        LiveUrl = row["live_url"]?.ToString(),
                        ImageUrl = row["image_url"]?.ToString(),
                        Featured = Convert.ToBoolean(row["featured"]),
                        DateFrom = row["date_from"]?.ToString(),
                        DateTo = row["date_to"]?.ToString(),
                        Status = row["status"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(row["created_at"]),
                        UpdatedAt = Convert.ToDateTime(row["updated_at"])
                    };
                }

                response.Success = true;
                response.Message = "Project added successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        // 🔹 Update
        public async Task<ProjectResponse> UpdateProjectAsync(int id, ProjectRequest req)
        {
            var response = new ProjectResponse();
            try
            {
                string sql = @"
                UPDATE portfolio_projects 
                SET title=@title, subtitle=@subtitle, description=@desc, type=@type, technologies=@tech,
                    github_url=@git, live_url=@live, image_url=@img, featured=@feat,
                    date_from=@from, date_to=@to, status=@status, updated_at=NOW()
                WHERE id=@id RETURNING *;
            ";

                var parameters = new[]
                {
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@title", req.Title),
                new NpgsqlParameter("@subtitle", req.Subtitle ?? (object)DBNull.Value),
                new NpgsqlParameter("@desc", req.Description ?? (object)DBNull.Value),
                new NpgsqlParameter("@type", req.Type ?? (object)DBNull.Value),
                new NpgsqlParameter("@tech", req.Technologies?.ToArray() ?? new string[]{}),
                new NpgsqlParameter("@git", req.GithubUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@live", req.LiveUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@img", req.ImageUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@feat", req.Featured),
                new NpgsqlParameter("@from", req.DateFrom ?? (object)DBNull.Value),
                new NpgsqlParameter("@to", req.DateTo ?? (object)DBNull.Value),
                new NpgsqlParameter("@status", req.Status ?? (object)DBNull.Value)
            };

                var dt = await _db.ExecuteQueryAsync(sql, parameters);
                if (dt.Rows.Count > 0)
                {
                    response.Data = new ProjectData
                    {
                        Id = Convert.ToInt32(dt.Rows[0]["id"]),
                        Title = dt.Rows[0]["title"]?.ToString(),
                        Description = dt.Rows[0]["description"]?.ToString()
                    };
                }

                response.Success = true;
                response.Message = "Project updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        // 🔹 Delete
        public async Task<ProjectResponse> DeleteProjectAsync(int id)
        {
            var response = new ProjectResponse();
            try
            {
                string sql = "DELETE FROM portfolio_projects WHERE id=@id;";
                var parameters = new[] { new NpgsqlParameter("@id", id) };
                int rows = await _db.ExecuteNonQueryAsync(sql, parameters);

                response.Success = rows > 0;
                response.Message = rows > 0 ? "Project deleted successfully." : "Project not found.";
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
