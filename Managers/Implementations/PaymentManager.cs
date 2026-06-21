using BuyMore.Enums;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;
using BuyMore.Repositories;
using BuyMore.Repositories.Interfaces;

namespace BuyMore.Managers.Implementations
{
    public class PaymentManager: IPaymentManager
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentManager()
        {
            _paymentRepository = new PaymentRepository();
        }

        public Payment CreatePayment(Order order, decimal amount, PaymentMethod method)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }

            var reference = Util.GenerateReference("PAY");
            var payment = new Payment(reference, order.UserId, order.Id, order.UserEmail, amount, method);
           
            _paymentRepository.AddPayment(payment);
            Console.WriteLine($"Payment {reference} recorded for order {order.Reference} via {method}.");
            return payment;
        }

        public Payment? GetPaymentByReference(string reference)
        {
            return _paymentRepository.GetPaymentByReference(reference);
        }

        public IEnumerable<Payment> GetPaymentsByUser(int userId)
        {
            return _paymentRepository.GetPaymentsByUser(userId);
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAllPayments();
        }

        public void UpdateStatus(string reference, PaymentStatus status)
        {
            var payment = GetPaymentByReference(reference);
            if (payment == null)
            {
                Console.WriteLine($"Payment {reference} not found.");
                return;
            }

            _paymentRepository.UpdatePaymentStatus(reference, status);
            Console.WriteLine($"Payment {reference} is now {status}.");
        }

        public void PrintPayments(IEnumerable<Payment> payments)
        {
            var paymentList = payments.ToList();
            if (paymentList.Count == 0)
            {
                Console.WriteLine("No payments to display.");
                return;
            }

            foreach (var payment in paymentList)
            {
                Console.WriteLine(payment);
            }
        }

    }
}
