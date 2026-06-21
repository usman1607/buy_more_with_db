using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;
using BuyMore.Models;
using BuyMore.Repositories.Interfaces;
using Npgsql;

namespace BuyMore.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public void AddPayment(Payment payment)
        {
            using var connection = Database.GetConnection();
            var sql = "INSERT INTO Payments (reference, user_id, order_id, user_email, amount, payment_method, payment_status, created_by, created_date) VALUES (@Reference, @UserId, @OrderId, @UserEmail, @Amount, @Method, @Status, @CreatedBy, @CreatedDate )";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Reference", payment.Reference);
            command.Parameters.AddWithValue("@UserId", payment.UserId);
            command.Parameters.AddWithValue("@OrderId", payment.OrderId);
            command.Parameters.AddWithValue("@UserEmail", payment.UserEmail);
            command.Parameters.AddWithValue("@Amount", payment.Amount);
            command.Parameters.AddWithValue("@Method", payment.Method.ToString());
            command.Parameters.AddWithValue("@Status", payment.Status.ToString());
            command.Parameters.AddWithValue("@CreatedBy", payment.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDate", payment.CreatedDate);
            command.ExecuteNonQuery();
        }

        public List<Payment> GetAllPayments()
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Payments";
            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var payments = new List<Payment>();
            while (reader.Read())
            {
                payments.Add(new Payment
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    (int)reader["order_id"],
                    reader["user_email"].ToString()!,
                    Convert.ToDecimal(reader["amount"]),
                    Enum.Parse<PaymentMethod>(reader["payment_method"].ToString()!),
                    Enum.Parse<PaymentStatus>(reader["payment_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                ));
            }
            return payments;
        }

        public Payment? GetPaymentByReference(string reference)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Payments WHERE reference = @Reference";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Reference", reference);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Payment
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    (int)reader["order_id"],
                    reader["user_email"].ToString()!,
                    Convert.ToDecimal(reader["amount"]),
                    Enum.Parse<PaymentMethod>(reader["payment_method"].ToString()!),
                    Enum.Parse<PaymentStatus>(reader["payment_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                );
            }
            return null;
        }

        public List<Payment> GetPaymentsByUser(int userId)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM Payments WHERE user_id = @UserId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();
            var payments = new List<Payment>();
            while (reader.Read())
            {
                payments.Add(new Payment
                (
                    (int)reader["id"],
                    reader["reference"].ToString()!,
                    (int)reader["user_id"],
                    (int)reader["order_id"],
                    reader["user_email"].ToString()!,
                    Convert.ToDecimal(reader["amount"]),
                    Enum.Parse<PaymentMethod>(reader["payment_method"].ToString()!),
                    Enum.Parse<PaymentStatus>(reader["payment_status"].ToString()!),
                    reader["created_by"].ToString()!,
                    Convert.ToDateTime(reader["created_date"])
                ));
            }
            return payments;
        }

        public bool UpdatePaymentStatus(string reference, PaymentStatus status)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE Payments SET payment_status = @Status WHERE reference = @Reference";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", status.ToString());
            command.Parameters.AddWithValue("@Reference", reference);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}