using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;
using BuyMore.Models;
using BuyMore.Repositories.Interfaces;
using Npgsql;

namespace BuyMore.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        public void AddOrder(Order order)
        {
            using var connection = Database.GetConnection();
            var sql = "INSERT INTO Orders (reference, user_id, user_email, cart_id, total_amount, order_status, created_by, created_date) VALUES (@Reference, @UserId, @UserEmail, @CartId, @TotalAmount, @Status, @CreatedBy, @CreatedDate)";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Reference", order.Reference);
            command.Parameters.AddWithValue("@UserId", order.UserId);
            command.Parameters.AddWithValue("@UserEmail", order.UserEmail);
            command.Parameters.AddWithValue("@CartId", order.CartId);
            command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@CreatedBy", order.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
            command.ExecuteNonQuery();
        }

        public List<Order> GetAllOrders()
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Orders";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var orders = new List<Order>();
            while (reader.Read())
            {
                orders.Add(new Order
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    reader["user_email"].ToString()!,
                    (int)reader["cart_id"],
                    Convert.ToDecimal(reader["total_amount"]),
                    Enum.Parse<OrderStatus>(reader["order_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                ));
            }
            return orders;
        }

        public Order? GetOrderByReference(string reference)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Orders WHERE reference = @Reference";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Reference", reference);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Order
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    reader["user_email"].ToString()!,
                    (int)reader["cart_id"],
                    Convert.ToDecimal(reader["total_amount"]),
                    Enum.Parse<OrderStatus>(reader["order_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                );
            }
            return null;
        }

        public List<Order> GetOrdersByUser(int userId)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Orders WHERE user_id = @UserId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();
            var orders = new List<Order>();
            while (reader.Read())
            {
                orders.Add(new Order
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    reader["user_email"].ToString()!,
                    (int)reader["cart_id"],
                    Convert.ToDecimal(reader["total_amount"]),
                    Enum.Parse<OrderStatus>(reader["order_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                ));
            }
            return orders;
        }

        public bool UpdateOrderStatus(string reference, OrderStatus status)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE Orders SET order_status = @Status WHERE reference = @Reference";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Reference", reference);
            command.ExecuteNonQuery();
            return true;
        }        
    }
}