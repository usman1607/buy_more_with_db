using System.Collections.Generic;
using BuyMore.Enums;
using BuyMore.Models;

namespace BuyMore.Managers.Interfaces
{
    public interface IPaymentManager
    {
        Payment CreatePayment(Order order, double amount, PaymentMethod method);
        Payment? GetPaymentByReference(string reference);
        IEnumerable<Payment> GetPaymentsByUser(int userId);
        IEnumerable<Payment> GetAllPayments();
        void UpdateStatus(string reference, PaymentStatus status);
        void PrintPayments(IEnumerable<Payment> payments);
    }
}
