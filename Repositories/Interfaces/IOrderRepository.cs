using BuyMore.Enums;
using BuyMore.Models;

namespace BuyMore.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        void AddOrder(Order order);
        Order? GetOrderByReference(string reference);
        List<Order> GetOrdersByUser(int userId);
        List<Order> GetAllOrders();
        bool UpdateOrderStatus(string reference, OrderStatus status);
    }
}