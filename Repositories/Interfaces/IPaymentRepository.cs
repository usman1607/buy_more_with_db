using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;
using BuyMore.Models;

namespace BuyMore.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        Payment? GetPaymentByReference(string reference);
        List<Payment> GetPaymentsByUser(int userId);
        List<Payment> GetAllPayments();
        bool UpdatePaymentStatus(string reference, PaymentStatus status);
    }
}