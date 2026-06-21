using Npgsql;

namespace BuyMore.Repositories
{
    public static class Database
    {
        private static string _connectionString = "Host=localhost;Port=5432;Database=buymore_db;Username=postgres;Password=Oluwatobiloba007";

        public static NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static void AddParameter(this NpgsqlCommand command, Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }
        }
    }

    
}