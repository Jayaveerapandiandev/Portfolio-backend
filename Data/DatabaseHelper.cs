using Npgsql;
using Portfolio_Api.Utilities;
using System.Data;

namespace Portfolio_Api.Data
{
    public class DatabaseHelper : IDisposable
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            _connectionString = AppConfig.GetConnectionString();
        }

        // Execute SELECT queries (return DataTable)
        public async Task<DataTable> ExecuteQueryAsync(string query, params NpgsqlParameter[] parameters)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        // Execute INSERT, UPDATE, DELETE
        public async Task<int> ExecuteNonQueryAsync(string query, params NpgsqlParameter[] parameters)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(query, connection);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            return await cmd.ExecuteNonQueryAsync();
        }

        public void Dispose()
        {
            // Nothing to dispose now - connections are disposed per-method
        }
    }
}