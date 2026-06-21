using BuyMore.Enums;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Managers.Implementations
{
    public class PaymentManager: IPaymentManager
    {
        private static int _nextId = 1;
        private static List<Payment> _payments = new List<Payment>();

        public PaymentManager()
        {
            _payments = FileUtil.ReadFromFile<Payment>("payments.txt");
        }

        public Payment CreatePayment(Order order, double amount, PaymentMethod method)
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
            var payment = new Payment(reference, order.UserId, order.Id, order.UserEmail, amount, method)
            {
                Id = _nextId++
            };
            _payments.Add(payment);
            FileUtil.SaveToFile(_payments, "payments.txt");
            Console.WriteLine($"Payment {reference} recorded for order {order.Reference} via {method}.");
            return payment;
        }

        public Payment? GetPaymentByReference(string reference)
        {
            return _payments.FirstOrDefault(p => p.Reference.Equals(reference, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Payment> GetPaymentsByUser(int userId)
        {
            return _payments.Where(p => p.UserId == userId).ToList();
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            return _payments.ToList();
        }

        public void UpdateStatus(string reference, PaymentStatus status)
        {
            var payment = GetPaymentByReference(reference);
            if (payment == null)
            {
                Console.WriteLine($"Payment {reference} not found.");
                return;
            }

            payment.Status = status;
            FileUtil.SaveToFile(_payments, "payments.txt");
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
