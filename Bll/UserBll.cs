using Npgsql;
using NpgsqlTypes;
using Portfolio_Api.Data;
using Portfolio_Api.DTO.Request;
using Portfolio_Api.DTO.Response;
using Portfolio_Api.Utilities;
using System.Data;

namespace Portfolio_Api.Bll
{
    public class UserBLL
    {
        private readonly DatabaseHelper _db;
        private readonly AesCryptoService _crypto;
        private readonly HashingService _hashing;

        public UserBLL()
        {
            // DatabaseHelper automatically reads from AppConfig
            _db = new DatabaseHelper();
            _crypto = new AesCryptoService();
            _hashing = new HashingService();
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var response = new CreateUserResponse();

            try
            {
                // 1️⃣ Check if user already exists
                string checkSql = @"SELECT COUNT(*) FROM app_users WHERE user_id = @uid;";
                var countDt = await _db.ExecuteQueryAsync(checkSql, new NpgsqlParameter("@uid", request.UserId));

                int userExists = Convert.ToInt32(countDt.Rows[0][0]);
                if (userExists > 0)
                {
                    response.Success = false;
                    response.Message = "User ID already exists. Please choose a different one.";
                    return response;
                }

                // 2️⃣ Hash password (already decrypted if needed on backend)
                string hashedPassword = _hashing.HashPassword(request.Password);

                // 3️⃣ Insert new user
                string sql = @"INSERT INTO app_users (user_id, password_hash, name, created_at)
                       VALUES (@uid, @pwd, @name, NOW());";

                var parameters = new[]
                {
            new NpgsqlParameter("@uid", request.UserId),
            new NpgsqlParameter("@pwd", hashedPassword),
            new NpgsqlParameter("@name", request.Name)
        };

                int result = await _db.ExecuteNonQueryAsync(sql, parameters);

                // 4️⃣ Response setup
                response.Success = result > 0;
                response.UserId = request.UserId;
                response.CreatedAt = result > 0 ? DateTime.UtcNow : null;
                response.Message = result > 0
                    ? "User created successfully."
                    : "User creation failed.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }


        public async Task<LoginResponse> ValidateLoginAsync(LoginRequest request)
        {
            var response = new LoginResponse();
            try
            {
               // string decryptedPassword = _crypto.Decrypt(request.Password);

                // 1️⃣ Fetch user, password hash, and name
                string sql = @"SELECT user_id, password_hash, name FROM app_users WHERE user_id = @uid;";
                var dt = await _db.ExecuteQueryAsync(sql, new NpgsqlParameter("@uid", request.UserId));

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Invalid User ID or Password.";
                    return response;
                }

                var row = dt.Rows[0];
                string storedHash = row["password_hash"].ToString()!;
                string userName = row["name"].ToString()!; // 📌 Get the name
               // string reqHash = _hashing.HashPassword(decryptedPassword);

                // 2️⃣ Verify password
                bool isPasswordValid = _hashing.VerifyPassword(request.Password, storedHash);
                if (!isPasswordValid)
                {
                    response.Success = false;
                    response.Message = "Invalid User ID or Password.";
                    return response;
                }

                // 3️⃣ Check if session record already exists for user
                string checkSessionSql = @"SELECT session_id FROM user_sessions WHERE user_id = @uid LIMIT 1;";
                var checkResult = await _db.ExecuteQueryAsync(checkSessionSql, new NpgsqlParameter("@uid", request.UserId));
                DataTable sessionResult;

                if (checkResult.Rows.Count > 0)
                {
                    // ✅ Reuse the same record and mark as active again
                    string updateSql = @"UPDATE user_sessions SET session_started_at = NOW(), session_ended_at = NULL, ip_address = @ip, user_agent = @agent WHERE user_id = @uid RETURNING session_id;";
                    var updateParams = new[]
                    {
                        new NpgsqlParameter("@uid", request.UserId),
                        new NpgsqlParameter("@ip", request.IpAddress ?? "0.0.0.0"),
                        new NpgsqlParameter("@agent", request.UserAgent ?? "unknown")
                    };
                    sessionResult = await _db.ExecuteQueryAsync(updateSql, updateParams);
                }
                else
                {
                    // 🆕 No record found — insert fresh
                    string insertSql = @"INSERT INTO user_sessions (user_id, ip_address, user_agent, session_started_at) VALUES (@uid, @ip, @agent, NOW()) RETURNING session_id;";
                    var insertParams = new[]
                    {
                        new NpgsqlParameter("@uid", request.UserId),
                        new NpgsqlParameter("@ip", request.IpAddress ?? "0.0.0.0"),
                        new NpgsqlParameter("@agent", request.UserAgent ?? "unknown")
                    };
                    sessionResult = await _db.ExecuteQueryAsync(insertSql, insertParams);
                }

                // 4️⃣ Prepare success response
                response.Success = true;
                response.Message = "Login successful.";
                response.SessionId = sessionResult.Rows[0]["session_id"].ToString();
                response.UserId = request.UserId;
                response.username = userName; // 📌 Set the username
                response.SessionStartedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }
            return response;
        }


        public async Task<LogoutResponse> LogoutUserAsync(LogoutRequest request)
        {
            var response = new LogoutResponse();

            try
            {
                var param = new NpgsqlParameter("@sid", NpgsqlDbType.Uuid)
                {
                    Value = Guid.Parse(request.SessionId)
                };
                // 1️⃣ Check if session exists
                string checkSql = @"SELECT session_id FROM user_sessions WHERE session_id = @sid;";
                var dt = await _db.ExecuteQueryAsync(checkSql, param);

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Session not found or already ended.";
                    return response;
                }
                var updateParam = new NpgsqlParameter("@sid", NpgsqlDbType.Uuid)
                {
                    Value = Guid.Parse(request.SessionId)
                };
                // 2️⃣ Update session_ended_at timestamp
                string updateSql = @"
                    UPDATE user_sessions
                    SET session_ended_at = NOW()
                    WHERE session_id = @sid;
                ";

                int rows = await _db.ExecuteNonQueryAsync(updateSql, updateParam);

                response.Success = rows > 0;
                response.Message = rows > 0 ? "Logout successful." : "Logout failed.";
                response.SessionId = request.SessionId;
                response.SessionEndedAt = rows > 0 ? DateTime.UtcNow : null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        public async Task<DeleteResponse> DeleteUserAsync(DeleteUserRequest request)
        {
            var response = new DeleteResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    response.Success = false;
                    response.Message = "UserId is required.";
                    return response;
                }

                // === Step 1️⃣ Delete from dependent tables (if any) ===
                string deleteSessionsSql = @"DELETE FROM user_sessions WHERE user_id = @uid;";
                await _db.ExecuteNonQueryAsync(deleteSessionsSql, new NpgsqlParameter("@uid", request.UserId));

                // === Step 2️⃣ Delete from main user table ===
                string deleteUserSql = @"DELETE FROM app_users WHERE user_id = @uid;";
                int affected = await _db.ExecuteNonQueryAsync(deleteUserSql, new NpgsqlParameter("@uid", request.UserId));

                // === Step 3️⃣ Response ===
                if (affected > 0)
                {
                    response.Success = true;
                    response.Message = $"User '{request.UserId}' deleted successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "No matching user found to delete.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting user: {ex.Message}";
            }

            return response;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var response = new ChangePasswordResponse();

            try
            {
                // 1️⃣ Check if user exists
                string checkSql = @"SELECT password_hash FROM app_users WHERE user_id = @uid;";
                var dt = await _db.ExecuteQueryAsync(checkSql, new NpgsqlParameter("@uid", request.UserId));

                if (dt.Rows.Count == 0)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // 2️⃣ Validate old password
                string storedHash = dt.Rows[0]["password_hash"].ToString();
                bool isOldPasswordValid = _hashing.VerifyPassword(request.OldPassword, storedHash);

                if (!isOldPasswordValid)
                {
                    response.Success = false;
                    response.Message = "Old password is incorrect.";
                    return response;
                }

                // 3️⃣ Hash new password
                string newHashedPassword = _hashing.HashPassword(request.NewPassword);

                // 4️⃣ Update in DB
                string updateSql = @"UPDATE app_users 
                             SET password_hash = @newPwd, updated_at = NOW() 
                             WHERE user_id = @uid;";

                var parameters = new[]
                {
                    new NpgsqlParameter("@newPwd", newHashedPassword),
                    new NpgsqlParameter("@uid", request.UserId)
                };

                int rowsAffected = await _db.ExecuteNonQueryAsync(updateSql, parameters);

                // 5️⃣ Response
                response.Success = rowsAffected > 0;
                response.UpdatedAt = rowsAffected > 0 ? DateTime.UtcNow : null;
                response.Message = rowsAffected > 0
                    ? "Password changed successfully."
                    : "Password change failed.";
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
