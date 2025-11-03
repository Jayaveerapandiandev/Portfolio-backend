using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;

namespace Portfolio_Api.Bll
{
    public class AdminHomeBLL
    {
        private readonly DatabaseHelper _db;

        public AdminHomeBLL( )
        {
            _db = new DatabaseHelper();
        }

        public async Task<SaveAdminHomeResponse> SaveAdminHomeAsync(SaveAdminHomeRequest request)
        {
            var response = new SaveAdminHomeResponse();

            try
            {
                // Check if a record already exists
                string checkSql = @"SELECT COUNT(*) FROM admin_home;";
                var countDt = await _db.ExecuteQueryAsync(checkSql);
                int recordExists = Convert.ToInt32(countDt.Rows[0][0]);

                string sql;
                NpgsqlParameter[] parameters = new[]
                {
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@role", request.Role),
                    new NpgsqlParameter("@tagline", request.Tagline),
                    new NpgsqlParameter("@intro", request.Intro),
                    new NpgsqlParameter("@cta1text", request.Cta1Text),
                    new NpgsqlParameter("@cta1link", request.Cta1Link),
                    new NpgsqlParameter("@cta2text", request.Cta2Text),
                    new NpgsqlParameter("@cta2link", request.Cta2Link),
                    new NpgsqlParameter("@linkedin", request.Linkedin),
                    new NpgsqlParameter("@github", request.Github),
                    new NpgsqlParameter("@twitter", request.Twitter)
                };

                // If exists → update; else → insert
                if (recordExists > 0)
                {
                    sql = @"UPDATE admin_home SET 
                                name = @name,
                                role = @role,
                                tagline = @tagline,
                                intro = @intro,
                                cta1text = @cta1text,
                                cta1link = @cta1link,
                                cta2text = @cta2text,
                                cta2link = @cta2link,
                                linkedin = @linkedin,
                                github = @github,
                                twitter = @twitter,
                                updated_at = NOW();";
                }
                else
                {
                    sql = @"INSERT INTO admin_home 
                            (name, role, tagline, intro, cta1text, cta1link, cta2text, cta2link, linkedin, github, twitter, created_at)
                            VALUES (@name, @role, @tagline, @intro, @cta1text, @cta1link, @cta2text, @cta2link, @linkedin, @github, @twitter, NOW());";
                }

                int result = await _db.ExecuteNonQueryAsync(sql, parameters);

                response.Success = result > 0;
                response.Message = result > 0
                    ? (recordExists > 0 ? "Home content updated successfully." : "Home content created successfully.")
                    : "No changes applied.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<AdminHomeResponse> GetAdminHomeAsync()
        {
            var response = new AdminHomeResponse();

            try
            {
                // ✅ Use correct column names from the actual table
                string sql = @"SELECT name, role, tagline, intro, cta1text, cta1link, cta2text, cta2link, linkedin, github, twitter FROM admin_home LIMIT 1;";

                var dt = await _db.ExecuteQueryAsync(sql);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    response.Success = true;
                    response.Data = new AdminHomeData
                    {
                        Name = row["name"]?.ToString() ?? string.Empty,
                        Role = row["role"]?.ToString() ?? string.Empty,
                        Tagline = row["tagline"]?.ToString() ?? string.Empty,
                        Intro = row["intro"]?.ToString() ?? string.Empty,
                        Cta1Text = row["cta1text"]?.ToString() ?? string.Empty,
                        Cta1Link = row["cta1link"]?.ToString() ?? string.Empty,
                        Cta2Text = row["cta2text"]?.ToString() ?? string.Empty,
                        Cta2Link = row["cta2link"]?.ToString() ?? string.Empty,
                        Linkedin = row["linkedin"]?.ToString() ?? string.Empty,
                        Github = row["github"]?.ToString() ?? string.Empty,
                        Twitter = row["twitter"]?.ToString() ?? string.Empty,
                    };
                }
                else
                {
                    response.Success = false;
                    response.Message = "No home content found.";
                }
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
