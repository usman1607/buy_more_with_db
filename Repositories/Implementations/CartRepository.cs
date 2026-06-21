using System.Text.Json;
using BuyMore.Models;
using BuyMore.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace BuyMore.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        public void AddCart(Cart cart)
        {
            using var connection = Database.GetConnection();
            var sql = "INSERT INTO carts (user_id, user_email, items, created_by, created_date) VALUES (@UserId, @UserEmail, @Items, @Createdby, @CreatedDate)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", cart.UserId);
            command.Parameters.AddWithValue("@UserEmail", cart.UserEmail);
            command.Parameters.Add(new NpgsqlParameter("items", NpgsqlDbType.Jsonb) { Value = JsonSerializer.Serialize(cart.Items) });
            command.Parameters.AddWithValue("@Createdby", cart.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDate", cart.CreatedDate);
            command.ExecuteNonQuery();
        }

        public bool DeleteCart(int userId)
        {
            using var connection = Database.GetConnection();
            var sql = "DELETE FROM carts WHERE user_id = @UserId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public Cart? GetCart(int cartId)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM carts WHERE id = @CartId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CartId", cartId);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var itemsJson = reader["items"].ToString();
                var items = JsonSerializer.Deserialize<Dictionary<int, int>>(itemsJson ?? "[]") ?? new Dictionary<int, int>();

                return new Cart(
                    (int)reader["id"],
                    (int)reader["user_id"],
                    reader["user_email"].ToString() ?? string.Empty,
                    items,
                    reader["created_by"].ToString() ?? string.Empty,
                    (DateTime)reader["created_date"]
                );
            }

            return null;
        }

        public Cart? GetCartByUserId(int userId)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM carts WHERE user_id = @UserId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var itemsJson = reader["items"].ToString();
                var items = JsonSerializer.Deserialize<Dictionary<int, int>>(itemsJson ?? "[]") ?? new Dictionary<int, int>();

                return new Cart(
                    (int)reader["id"],
                    (int)reader["user_id"],
                    reader["user_email"].ToString() ?? string.Empty,
                    items,
                    reader["created_by"].ToString() ?? string.Empty,
                    (DateTime)reader["created_date"]
                );
            }

            return null;
        }

        public bool UpdateCart(int userId, Cart cart)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE carts SET items = @Items WHERE user_id = @UserId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add(new NpgsqlParameter("items", NpgsqlDbType.Jsonb) { Value = JsonSerializer.Serialize(cart.Items) });
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}