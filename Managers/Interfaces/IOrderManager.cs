using System.Collections.Generic;
using BuyMore.Enums;
using BuyMore.Models;

namespace BuyMore.Managers.Interfaces
{
    public interface IOrderManager
    {
        Order CreateOrder(User user, Cart cart, decimal totalAmount);
        Order? GetOrderByReference(string reference);
        IEnumerable<Order> GetOrdersByUser(int userId);
        IEnumerable<Order> GetAllOrders();
        void UpdateStatus(string reference, OrderStatus status);
        void PrintOrders(IEnumerable<Order> orders);
    }
}
