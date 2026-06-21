using BuyMore.Models;

namespace BuyMore.Managers.Interfaces
{
    public interface ICartManager
    {
        Cart CreateCart(int userId, string userEmail);
        Cart GetOrCreateCart(int userId, string userEmail);
        Cart? GetCart(int cartId);
        Cart? GetCartByUser(int userId);
        void AddItem(int cartId, Item item, int quantity);
        void RemoveItem(int cartId, int itemId, int quantity = 0);
        void ClearCart(int cartId);
        void PrintCart(int cartId, IItemManager itemManager);
    }
}
