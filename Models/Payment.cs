using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;

namespace BuyMore.Models
{
    public class Payment: BaseModel
    {
        public string Reference { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string UserEmail { get; set; } = default!;
        public double Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public Payment(string reference, int userId, int orderId, string userEmail, double amount, PaymentMethod method)
        {
            Reference = reference;
            UserId = userId;
            OrderId = orderId;
            UserEmail = userEmail;
            Amount = amount;
            Method = method;
            CreatedBy = userEmail;
        }

        public override string ToString()
        {
            return $"PaymentRef: {Reference}\tOrderId: {OrderId}\tAmount: {Amount:C}\tMethod: {Method}\tStatus: {Status}";
        }
    }
}
