using System;
using System.Collections.Generic;
using System.Linq;
using BuyMore.Enums;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;
using BuyMore.Repositories.Implementations;
using BuyMore.Repositories.Interfaces;

namespace BuyMore.Managers.Implementations
{
    public class OrderManager: IOrderManager
    {
        private readonly IOrderRepository _orderRepository;
        public OrderManager()
        {
            _orderRepository = new OrderRepository();
        }

        public Order CreateOrder(User user, Cart cart, decimal totalAmount)
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
            var order = new Order(reference, user.Id, user.Email, cart.Id, totalAmount);
            _orderRepository.AddOrder(order);
            Console.WriteLine($"Order {reference} created successfully.");
            return order;
        }

        public Order? GetOrderByReference(string reference)
        {
            return _orderRepository.GetOrderByReference(reference);
        }

        public IEnumerable<Order> GetOrdersByUser(int userId)
        {
            return _orderRepository.GetOrdersByUser(userId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public void UpdateStatus(string reference, OrderStatus status)
        {
            var order = GetOrderByReference(reference);
            if (order == null)
            {
                Console.WriteLine($"Order {reference} not found.");
                return;
            }

            _orderRepository.UpdateOrderStatus(reference, status);
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
