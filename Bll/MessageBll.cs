using Npgsql;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using System.Data;

namespace Portfolio_Api.Bll
{
    public class MessageBLL
    {
        private readonly DatabaseHelper _db;

        public MessageBLL()
        {
            _db = new DatabaseHelper();
        }

        public async Task<CreateMessageResponse> CreateMessageAsync(CreateMessageRequest request, string? ipAddress)
        {
            var response = new CreateMessageResponse();

            try
            {
                // 1️⃣ CHECK IF SAME EMAIL SENT MESSAGE RECENTLY (12 hours limit)
                string checkSql = @"
                    SELECT createdAt 
                    FROM messages 
                    WHERE LOWER(email) = LOWER(@email)
                    ORDER BY createdAt DESC
                    LIMIT 1;
                ";

                var dt = await _db.ExecuteQueryAsync(
                    checkSql,
                    new NpgsqlParameter("@email", request.Email)
                );

                if (dt.Rows.Count > 0)
                {
                    DateTime lastSent = Convert.ToDateTime(dt.Rows[0]["createdAt"]);
                    DateTime now = DateTime.UtcNow;

                    double hoursDiff = (now - lastSent).TotalHours;

                    if (hoursDiff < 12)  // ⏳ 12 hours restriction
                    {
                        response.Success = false;
                        response.Message = "You have already sent a message. Try again after 12 hours.";
                        return response;
                    }
                }

                // 2️⃣ INSERT NEW MESSAGE
                string insertSql = @"
                        INSERT INTO messages 
                        (name, email, phone, purpose, message, ipAddress, isSeen) 
                        VALUES 
                        (@name, @email, @phone, @purpose, @message, @ipAddress, FALSE);
                    ";

                int rows = await _db.ExecuteNonQueryAsync(
                    insertSql,
                    new NpgsqlParameter("@name", request.Name),
                    new NpgsqlParameter("@email", request.Email),
                    new NpgsqlParameter("@phone",
                        string.IsNullOrWhiteSpace(request.Phone) ? (object)DBNull.Value : request.Phone),
                    new NpgsqlParameter("@purpose", request.Category),
                    new NpgsqlParameter("@message", request.Message),
                    new NpgsqlParameter("@ipAddress",
                        string.IsNullOrWhiteSpace(ipAddress) ? (object)DBNull.Value : ipAddress)
                );

                if (rows <= 0)
                {
                    response.Success = false;
                    response.Message = "Failed to send message";
                    return response;
                }

                // 3️⃣ SUCCESS
                response.Success = true;
                response.Message = "Message sent successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<GetAllMessagesResponse> GetAllMessagesAsync()
        {
            var response = new GetAllMessagesResponse();

            try
            {
                string sql = @"
            SELECT 
                id, 
                name, 
                email, 
                purpose AS category, 
                message, 
                ipAddress, 
                userAgent, 
                isSeen, 
                createdAt
            FROM messages
            ORDER BY createdAt DESC;
        ";

                var dt = await _db.ExecuteQueryAsync(sql);

                foreach (DataRow row in dt.Rows)
                {
                    response.Data.Add(new MessageData
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Name = row["name"].ToString(),
                        Email = row["email"].ToString(),
                        Category = row["category"].ToString(),
                        Message = row["message"].ToString(),
                        IpAddress = row["ipAddress"]?.ToString(),
                        UserAgent = row["userAgent"]?.ToString(),
                        IsSeen = Convert.ToBoolean(row["isSeen"]),
                        CreatedAt = Convert.ToDateTime(row["createdAt"])
                    });
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<CreateMessageResponse> MarkMessageAsSeenAsync(int id)
        {
            var response = new CreateMessageResponse();

            try
            {
                string sql = @"
                    UPDATE messages 
                    SET isSeen = TRUE 
                    WHERE id = @id;
                ";

                int rows = await _db.ExecuteNonQueryAsync(
                    sql,
                    new NpgsqlParameter("@id", id)
                );

                if (rows <= 0)
                {
                    response.Success = false;
                    response.Message = "Message not found";
                    return response;
                }

                response.Success = true;
                response.Message = "Message marked as seen";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<CreateMessageResponse> DeleteMessageAsync(int id)
        {
            var response = new CreateMessageResponse();

            try
            {
                string sql = @"
            DELETE FROM messages
            WHERE id = @id;
        ";

                int rows = await _db.ExecuteNonQueryAsync(
                    sql,
                    new NpgsqlParameter("@id", id)
                );

                if (rows <= 0)
                {
                    response.Success = false;
                    response.Message = "Message not found";
                    return response;
                }

                response.Success = true;
                response.Message = "Message deleted successfully";
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
