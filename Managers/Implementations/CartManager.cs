using System;
using System.Collections.Generic;
using System.Linq;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Managers.Implementations
{
    public class CartManager: ICartManager
    {
        private static int _nextId = 1;
        private static List<Cart> _carts = new List<Cart>();

        public CartManager()
        {
            _carts = FileUtil.ReadFromFile<Cart>("carts.txt");
        }

        public Cart CreateCart(int userId, string userEmail)
        {
            var cart = new Cart(_nextId++, userId, userEmail);
            _carts.Add(cart);
            FileUtil.SaveToFile(_carts, "carts.txt");
            Console.WriteLine($"Cart {cart.Id} created for {userEmail}.");
            return cart;
        }

        public Cart GetOrCreateCart(int userId, string userEmail)
        {
            var existing = GetCartByUser(userId);
            return existing ?? CreateCart(userId, userEmail);
        }

        public Cart? GetCart(int cartId)
        {
            return _carts.FirstOrDefault(c => c.Id == cartId);
        }

        public Cart? GetCartByUser(int userId)
        {
            return _carts.FirstOrDefault(c => c.UserId == userId);
        }

        public void AddItem(int cartId, Item item, int quantity)
        {
            var cart = GetCart(cartId);
            if (cart == null)
            {
                Console.WriteLine($"Cart with Id {cartId} not found.");
                return;
            }

            if (item == null)
            {
                Console.WriteLine("Invalid item supplied.");
                return;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be greater than zero.");
                return;
            }

            if (item.Quantity < quantity)
            {
                Console.WriteLine($"Only {item.Quantity} unit(s) available for {item.Name}.");
                return;
            }

            cart.AddItem(item.Id, quantity);
            FileUtil.SaveToFile(_carts, "carts.txt");
            Console.WriteLine($"Added {quantity} x {item.Name} to cart {cart.Id}.");
        }

        public void RemoveItem(int cartId, int itemId, int quantity = 0)
        {
            var cart = GetCart(cartId);
            if (cart == null)
            {
                Console.WriteLine($"Cart with Id {cartId} not found.");
                return;
            }

            if (!cart.Items.ContainsKey(itemId))
            {
                Console.WriteLine("Item not found in cart.");
                return;
            }

            if (quantity <= 0)
            {
                quantity = cart.Items[itemId];
            }

            cart.RemoveItem(itemId, quantity);
            FileUtil.SaveToFile(_carts, "carts.txt");   
            Console.WriteLine("Item updated in cart.");
        }

        public void ClearCart(int cartId)
        {
            var cart = GetCart(cartId);
            if (cart == null)
            {
                Console.WriteLine($"Cart with Id {cartId} not found.");
                return;
            }

            cart.Clear();
            FileUtil.SaveToFile(_carts, "carts.txt");
            Console.WriteLine("Cart cleared.");
        }

        public void PrintCart(int cartId, IItemManager itemManager)
        {
            var cart = GetCart(cartId);
            if (cart == null)
            {
                Console.WriteLine($"Cart with Id {cartId} not found.");
                return;
            }

            if (cart.Items.Count == 0)
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            double total = 0;
            Console.WriteLine($"Cart #{cart.Id} for {cart.UserEmail}");
            foreach (var entry in cart.Items)
            {
                Item? item = null;
                try
                {
                    item = itemManager.GetItem(entry.Key);
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine($"Item with id {entry.Key} no longer exists.");
                    continue;
                }

                var lineTotal = entry.Value * item.SellingPrice;
                total += lineTotal;
                Console.WriteLine($" - {item.Name} x {entry.Value} @ {item.SellingPrice:C} = {lineTotal:C}");
            }

            Console.WriteLine($"Total: {total:C}");
        }
    }
}
