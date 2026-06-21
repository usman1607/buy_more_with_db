using BuyMore.Enums;
using BuyMore.Helpers;
using BuyMore.Models;
using BuyMore.Repositories.Interfaces;
using Npgsql;

namespace BuyMore.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        public void InitializeDefaultAdmin()
        {
            var defaultAdminEmail = "admin@buymore.com";
            if (GetUserByEmail(defaultAdminEmail) is null)
            {
                var admin = new User("Admin", "User", "08012345678", defaultAdminEmail, UserHelper.EncryptPassword("Admin@123"), "Buy more store, Lagos", Role.Admin, "System");
                AddUser(admin);
            }
        }

        public void AddUser(User user)
        {
            using var connection = Database.GetConnection();

            var sql = "INSERT INTO users (first_name, last_name, phone_number, email, encrypted_password, address, role, wallet_balance, created_by, created_date) VALUES (@firstName, @lastName, @phoneNumber, @email, @encryptedPassword, @address, @role, @walletBalance, @createdBy, @createdDate)";

            using var cmd = new NpgsqlCommand(sql, connection);
            
            cmd.Parameters.AddWithValue("firstName", user.FirstName);
            cmd.Parameters.AddWithValue("lastName", user.LastName);
            cmd.Parameters.AddWithValue("phoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("email", user.Email.ToLower());
            cmd.Parameters.AddWithValue("encryptedPassword", user.EncryptedPassword);
            cmd.Parameters.AddWithValue("address", user.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("role", user.Role.ToString());
            cmd.Parameters.AddWithValue("walletBalance", user.WalletBalance);
            cmd.Parameters.AddWithValue("createdBy", user.CreatedBy);
            cmd.Parameters.AddWithValue("createdDate", user.CreatedDate);
            cmd.ExecuteNonQuery();
        }

        public bool DeleteUser(int id)
        {
            using var connection = Database.GetConnection();
            var sql = "DELETE FROM users WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("id", id);
            var rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<User> GetAllUsers()
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM users";
            using var cmd = new NpgsqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();

            var users = new List<User>();
            while (reader.Read())
            {
                users.Add(new User(
                    (int)reader["id"],
                    reader["first_name"].ToString()!,
                    reader["last_name"].ToString()!,
                    reader["phone_number"].ToString(),
                    reader["email"].ToString()!,
                    reader["encrypted_password"].ToString()!,
                    reader["address"].ToString(),
                    Enum.Parse<Role>(reader["role"].ToString()!),
                    reader["created_by"].ToString()!,
                    (DateTime)reader["created_date"]
                ));
            }
            return users;
        }

        public User? GetUserByEmail(string email)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM users WHERE email = @Email";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("Email", email.ToLower());
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User(
                    (int)reader["id"],
                    reader["first_name"].ToString()!,
                    reader["last_name"].ToString()!,
                    reader["phone_number"].ToString(),
                    reader["email"].ToString()!,
                    reader["encrypted_password"].ToString()!,
                    reader["address"].ToString(),
                    Enum.Parse<Role>(reader["role"].ToString()!),
                    reader["created_by"].ToString()!,
                    (DateTime)reader["created_date"]
                );
            }
            return null;
        }

        public User? GetUserById(int id)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM users WHERE id = @Id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("Id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User(
                    (int)reader["id"],
                    reader["first_name"].ToString()!,
                    reader["last_name"].ToString()!,
                    reader["phone_number"].ToString(),
                    reader["email"].ToString()!,
                    reader["encrypted_password"].ToString()!,
                    reader["address"].ToString(),
                    Enum.Parse<Role>(reader["role"].ToString()!),
                    reader["created_by"].ToString()!,
                    (DateTime)reader["created_date"]
                );
            }
            return null;
        }

        public bool UpdateUser(int id, User user)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE users SET first_name = @firstName, last_name = @lastName, phone_number = @phoneNumber, encrypted_password = @encryptedPassword, address = @address, role = @role, wallet_balance = @walletBalance WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            
            Database.AddParameter(cmd, new Dictionary<string, object>
            {
                { "firstName", user.FirstName },
                { "lastName", user.LastName },
                { "phoneNumber", user.PhoneNumber ?? (object)DBNull.Value },
                { "encryptedPassword", user.EncryptedPassword },
                { "address", user.Address ?? (object)DBNull.Value },
                { "role", user.Role.ToString() },
                { "walletBalance", user.WalletBalance },
                { "id", id }
            });
            /*cmd.Parameters.AddWithValue("firstName", user.FirstName);
            cmd.Parameters.AddWithValue("lastName", user.LastName);
            cmd.Parameters.AddWithValue("phoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("encryptedPassword", user.EncryptedPassword);
            cmd.Parameters.AddWithValue("address", user.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("role", user.Role.ToString());
            cmd.Parameters.AddWithValue("walletBalance", user.WalletBalance);
            cmd.Parameters.AddWithValue("id", id);*/

            var rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool UpdateWalletBalance(int id, double newBalance)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE users SET wallet_balance = @walletBalance WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("walletBalance", newBalance);
            cmd.Parameters.AddWithValue("id", id);

            var rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}