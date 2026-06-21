using BuyMore.Models;

namespace BuyMore.Repositories.Interfaces
{
    public interface ICartRepository
    {
        void AddCart(Cart cart);
        Cart? GetCartByUserId(int userId);
        Cart? GetCart(int cartId);
        bool UpdateCart(int userId, Cart cart);
        bool DeleteCart(int userId);
    }
}