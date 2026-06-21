using System;
using System.Collections.Generic;
using BuyMore.Enums;
using BuyMore.Managers.Implementations;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Menus
{
    public class MainMenu
    {
        private readonly IUserManager _userManager;
        private readonly IItemManager _itemManager;
        private readonly ICartManager _cartManager;
        private readonly IOrderManager _orderManager;
        private readonly IPaymentManager _paymentManager;
        private readonly AdminMenu _adminMenu;
        private readonly CustomerMenu _customerMenu;

        public MainMenu()
        {
            _userManager = new UserManager();
            _itemManager = new ItemManager();
            _cartManager = new CartManager();
            _orderManager = new OrderManager();
            _paymentManager = new PaymentManager();
            _adminMenu = new AdminMenu(_itemManager, _userManager, _orderManager, _paymentManager);
            _customerMenu = new CustomerMenu(_itemManager, _userManager, _cartManager, _orderManager, _paymentManager);
        }

        public void Run()
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("========== BUYMORE ==========");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. View Available Items");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleLogin();
                        break;
                    case "2":
                        var newUser = _userManager.Register();
                        if (newUser != null && newUser.Role == Role.Customer)
                        {
                            _customerMenu.Show(newUser);
                        }
                        break;
                    case "3":
                        BrowseItemsFromLanding();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }

            Console.WriteLine("Thank you for using BuyMore!");
        }

        private void HandleLogin()
        {
            var user = AuthenticateUser();
            if (user == null)
            {
                return;
            }

            if (user.Role == Role.Admin)
            {
                _adminMenu.Show(user);
            }
            else
            {
                _customerMenu.Show(user);
            }
        }

        private void BrowseItemsFromLanding()
        {
            _itemManager.GetAll();
            if (!AskYesNo("Would you like to add an item to your cart? (y/n): "))
            {
                return;
            }

            var customer = EnsureCustomerForCart();
            if (customer == null)
            {
                return;
            }

            AddItemToCart(customer);
        }

        private User? AuthenticateUser()
        {
            return _userManager.Login();
        }

        private User? EnsureCustomerForCart()
        {
            Console.WriteLine("Please login to continue.");
            var user = AuthenticateUser();
            if (user == null)
            {
                return null;
            }

            if (user.Role != Role.Customer)
            {
                Console.WriteLine("Only customer accounts can add items to a cart.");
                return null;
            }

            return user;
        }

        private void AddItemToCart(User user)
        {
            var cart = _cartManager.GetOrCreateCart(user.Id, user.Email);
            var itemId = ReadInt("Enter item Id: ");
            var quantity = ReadInt("Enter quantity: ");

            Item item;
            try
            {
                item = _itemManager.GetItem(itemId);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Item not found. Please try again.");
                return;
            }

            _cartManager.AddItem(cart.Id, item, quantity);
        }

        private static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            int value;
            while (!int.TryParse(input, out value) || value <= 0)
            {
                Console.Write("Please enter a valid positive number: ");
                input = Console.ReadLine();
            }

            return value;
        }

        private static bool AskYesNo(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Trim().ToLowerInvariant();
            return normalized == "y" || normalized == "yes";
        }
    }
}
