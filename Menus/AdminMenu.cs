using System;
using BuyMore.Enums;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Menus
{
    public class AdminMenu
    {
        private readonly IItemManager _itemManager;
        private readonly IUserManager _userManager;
        private readonly IOrderManager _orderManager;
        private readonly IPaymentManager _paymentManager;

        public AdminMenu(IItemManager itemManager, IUserManager userManager, IOrderManager orderManager, IPaymentManager paymentManager)
        {
            _itemManager = itemManager;
            _userManager = userManager;
            _orderManager = orderManager;
            _paymentManager = paymentManager;
        }

        public void Show(User admin)
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("========== ADMIN MENU ==========");
                Console.WriteLine("1. Create Item");
                Console.WriteLine("2. Update Item");
                Console.WriteLine("3. View Item By Id");
                Console.WriteLine("4. View Item By Name");
                Console.WriteLine("5. View All Items");
                Console.WriteLine("6. Search Items");
                Console.WriteLine("7. Create Another Admin");
                Console.WriteLine("8. View All Users");
                Console.WriteLine("9. View All Orders");
                Console.WriteLine("10. Update Order Status");
                Console.WriteLine("11. View All Payments");
                Console.WriteLine("0. Logout");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _itemManager.CreateItem(admin.Email);
                        break;
                    case "2":
                        var updateId = ReadInt("Enter item Id: ");
                        _itemManager.UpdateItem(admin.Email, updateId);
                        break;
                    case "3":
                        var viewId = ReadInt("Enter item Id: ");
                        _itemManager.GetItemById(viewId);
                        break;
                    case "4":
                        Console.Write("Enter item name: ");
                        var name = Console.ReadLine() ?? string.Empty;
                        _itemManager.GetItemByName(name);
                        break;
                    case "5":
                        _itemManager.GetAll();
                        break;
                    case "6":
                        HandleSearch();
                        break;
                    case "7":
                        _userManager.CreateAdmin(admin.Email);
                        break;
                    case "8":
                        _userManager.GetAll();
                        break;
                    case "9":
                        _orderManager.PrintOrders(_orderManager.GetAllOrders());
                        break;
                    case "10":
                        HandleOrderStatusUpdate();
                        break;
                    case "11":
                        _paymentManager.PrintPayments(_paymentManager.GetAllPayments());
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }

        private void HandleSearch()
        {
            Console.Write("Enter category (press Enter to skip): ");
            var category = Console.ReadLine();
            Console.Write("Enter search keyword (press Enter to skip): ");
            var search = Console.ReadLine();
            var minPrice = ReadDouble("Enter min price (0 for none): ");
            var maxPrice = ReadDouble("Enter max price (0 for none): ");
            _itemManager.Search(category, search, minPrice, maxPrice);
        }

        private void HandleOrderStatusUpdate()
        {
            Console.Write("Enter order reference: ");
            var reference = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(reference))
            {
                Console.WriteLine("Reference cannot be empty.");
                return;
            }

            Console.WriteLine("Select new status:");
            foreach (OrderStatus value in Enum.GetValues(typeof(OrderStatus)))
            {
                Console.WriteLine($" {(int)value} - {value}");
            }

            var statusNumber = ReadInt("Enter status number: ");
            if (Enum.IsDefined(typeof(OrderStatus), statusNumber))
            {
                var status = (OrderStatus)statusNumber;
                _orderManager.UpdateStatus(reference, status);
            }
            else
            {
                Console.WriteLine("Invalid status selected.");
            }
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

        private static double ReadDouble(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            double value;
            while (!double.TryParse(input, out value) || value < 0)
            {
                Console.Write("Please enter a valid amount: ");
                input = Console.ReadLine();
            }
            return value;
        }
    }
}
