using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;

namespace BuyMore.Models
{
    public class Order: BaseModel
    {
        public string Reference { get; }
        public int UserId { get; }
        public string UserEmail { get; } = default!;
        public int CartId { get; }
        public double TotalAmount { get; }
        public OrderStatus Status { get; set; }

        public Order(int id, string reference, int userId, string userEmail, int cartId, double totalAmount)
        {
            Id = id;
            Reference = reference;
            UserId = userId;
            UserEmail = userEmail;
            CartId = cartId;
            TotalAmount = totalAmount;
            Status = OrderStatus.Pending;
            CreatedBy = userEmail;
        }

        public override string ToString()
        {
            return $"OrderRef: {Reference}\tUser: {UserEmail}\tCartId: {CartId}\tAmount: {TotalAmount:C}\tStatus: {Status}";
        }
    }
}
