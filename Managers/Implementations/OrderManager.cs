using System;
using System.Collections.Generic;
using System.Linq;
using BuyMore.Enums;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Managers.Implementations
{
    public class OrderManager: IOrderManager
    {
        private static int _nextId = 1;
        private static List<Order> _orders = new List<Order>();

        public OrderManager()
        {
            _orders = FileUtil.ReadFromFile<Order>("orders.txt");
        }

        public Order CreateOrder(User user, Cart cart, double totalAmount)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            if (cart.Items.Count == 0)
            {
                throw new InvalidOperationException("Cannot place an order with an empty cart.");
            }

            var reference = Util.GenerateReference("ORD");
            var order = new Order(_nextId++, reference, user.Id, user.Email, cart.Id, totalAmount);
            _orders.Add(order);
            FileUtil.SaveToFile(_orders, "orders.txt");
            Console.WriteLine($"Order {reference} created successfully.");
            return order;
        }

        public Order? GetOrderByReference(string reference)
        {
            return _orders.FirstOrDefault(o => o.Reference.Equals(reference, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Order> GetOrdersByUser(int userId)
        {
            return _orders.Where(o => o.UserId == userId).ToList();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orders.ToList();
        }

        public void UpdateStatus(string reference, OrderStatus status)
        {
            var order = GetOrderByReference(reference);
            if (order == null)
            {
                Console.WriteLine($"Order {reference} not found.");
                return;
            }

            order.Status = status;
            FileUtil.SaveToFile(_orders, "orders.txt");
            Console.WriteLine($"Order {reference} is now {status}.");
        }

        public void PrintOrders(IEnumerable<Order> orders)
        {
            var orderList = orders.ToList();
            if (orderList.Count == 0)
            {
                Console.WriteLine("No orders to display.");
                return;
            }

            foreach (var order in orderList)
            {
                Console.WriteLine(order);
            }
        }

    }
}
