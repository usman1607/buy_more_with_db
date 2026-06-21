using System.Globalization;
using BuyMore.Enums;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;

namespace BuyMore.Menus
{
    public class CustomerMenu
    {
        private readonly IItemManager _itemManager;
        private readonly IUserManager _userManager;
        private readonly ICartManager _cartManager;
        private readonly IOrderManager _orderManager;
        private readonly IPaymentManager _paymentManager;

        public CustomerMenu(IItemManager itemManager, IUserManager userManager, ICartManager cartManager, IOrderManager orderManager, IPaymentManager paymentManager)
        {
            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            _itemManager = itemManager;
            _userManager = userManager;
            _cartManager = cartManager;
            _orderManager = orderManager;
            _paymentManager = paymentManager;   
        }

        public void Show(User user)
        {
            var exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("========== CUSTOMER MENU ==========");
                Console.WriteLine($"Wallet Balance: {user.WalletBalance:C}");
                Console.WriteLine("1. Browse All Items");
                Console.WriteLine("2. Search Items");
                Console.WriteLine("3. Add Item To Cart");
                Console.WriteLine("4. View Cart");
                Console.WriteLine("5. Remove Item From Cart");
                Console.WriteLine("6. Clear Cart");
                Console.WriteLine("7. Checkout");
                Console.WriteLine("8. View My Orders");
                Console.WriteLine("9. View My Payments");
                Console.WriteLine("10. Fund Wallet");
                Console.WriteLine("11. Update Profile");
                Console.WriteLine("0. Logout");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _itemManager.GetAll();
                        PromptAddToCart(user);
                        break;
                    case "2":
                        HandleSearch(user);
                        break;
                    case "3":
                        HandleAddToCart(user);
                        break;
                    case "4":
                        var cartForView = EnsureCart(user);
                        _cartManager.PrintCart(cartForView.Id, _itemManager);
                        break;
                    case "5":
                        HandleRemoveFromCart(user);
                        break;
                    case "6":
                        var cartToClear = EnsureCart(user);
                        _cartManager.ClearCart(cartToClear.Id);
                        break;
                    case "7":
                        Checkout(user);
                        break;
                    case "8":
                        _orderManager.PrintOrders(_orderManager.GetOrdersByUser(user.Id));
                        break;
                    case "9":
                        _paymentManager.PrintPayments(_paymentManager.GetPaymentsByUser(user.Id));
                        break;
                    case "10":
                        HandleFundWallet(user);
                        break;
                    case "11":
                        _userManager.UpdateUser(user.Email);
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

        private void HandleSearch(User user)
        {
            Console.Write("Enter category (press Enter to skip): ");
            var category = Console.ReadLine();
            Console.Write("Enter search keyword (press Enter to skip): ");
            var search = Console.ReadLine();
            var minPrice = ReadDouble("Enter min price (0 for none): ");
            var maxPrice = ReadDouble("Enter max price (0 for none): ");
            _itemManager.Search(category, search, minPrice, maxPrice);
            PromptAddToCart(user);
        }

        private void HandleAddToCart(User user)
        {
            var cart = EnsureCart(user);
            var itemId = ReadInt("Enter item Id: ");
            var quantity = ReadInt("Enter quantity: ");

            if (!TryResolveItem(itemId, out var item))
            {
                return;
            }

            _cartManager.AddItem(cart.Id, item!, quantity);
        }

        private void PromptAddToCart(User user)
        {
            if (!AskYesNo("Would you like to add an item to your cart? (y/n): "))
            {
                return;
            }

            HandleAddToCart(user);
        }

        private void HandleRemoveFromCart(User user)
        {
            var cart = EnsureCart(user);
            var itemId = ReadInt("Enter item Id to remove: ");
            Console.Write("Enter quantity to remove (0 removes all): ");
            var input = Console.ReadLine();
            int quantity = 0;
            int.TryParse(input, out quantity);
            _cartManager.RemoveItem(cart.Id, itemId, quantity);
        }

        private void HandleFundWallet(User user)
        {
            var amount = ReadDouble("Enter amount to add to your wallet: ");
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be greater than zero.");
                return;
            }

            try
            {
                user.CreditWallet(amount);
                _userManager.UpdateUserWallet(user.Id, user.WalletBalance);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine($"Wallet funded successfully. New balance: {user.WalletBalance:C}");
        }

        private void Checkout(User user)
        {
            var cart = EnsureCart(user);
            if (cart.Items.Count == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            if (!TryCalculateTotal(cart, out var total))
            {
                return;
            }

            if (!HasInventory(cart))
            {
                Console.WriteLine("One or more items no longer have enough stock. Please adjust your cart.");
                return;
            }

            var paymentMethod = PromptPaymentMethod();
            if (paymentMethod == PaymentMethod.Wallet && !HasSufficientWalletBalance(user, total))
            {
                var shortfall = total - user.WalletBalance;
                Console.WriteLine($"Insufficient wallet balance. You need an additional {shortfall:C} to complete this purchase.");
                return;
            }

            foreach (var entry in cart.Items)
            {
                if (!TryResolveItem(entry.Key, out var item))
                {
                    return;
                }
                item!.Quantity -= entry.Value;
            }

            Order order;
            try
            {
                order = _orderManager.CreateOrder(user, cart, total);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create order: {ex.Message}");
                return;
            }

            Payment payment;
            try
            {
                payment = _paymentManager.CreatePayment(order, total, paymentMethod);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not process payment: {ex.Message}");
                return;
            }

            if (paymentMethod == PaymentMethod.Wallet)
            {
                if (!user.TryDebitWallet(total))
                {
                    Console.WriteLine("Could not debit wallet. Payment canceled.");
                    _paymentManager.UpdateStatus(payment.Reference, PaymentStatus.Failed);
                    return;
                }
                payment.Status = PaymentStatus.Successful;
                Console.WriteLine($"Wallet charged {total:C}. Remaining balance: {user.WalletBalance:C}");
            }
            else
            {
                if (!ProcessCardPayment(total))
                {
                    Console.WriteLine("Card payment failed. Please try again later.");
                    _paymentManager.UpdateStatus(payment.Reference, PaymentStatus.Failed);
                    return;
                }
                payment.Status = PaymentStatus.Successful;
            }

            _cartManager.ClearCart(cart.Id);
            Console.WriteLine($"Checkout complete. Order {order.Reference} placed and payment {payment.Reference} successful via {paymentMethod}.");
        }

        private Cart EnsureCart(User user)
        {
            return _cartManager.GetOrCreateCart(user.Id, user.Email);
        }

        private bool TryCalculateTotal(Cart cart, out double total)
        {
            total = 0;
            foreach (var entry in cart.Items)
            {
                if (!TryResolveItem(entry.Key, out var item))
                {
                    total = 0;
                    return false;
                }
                total += entry.Value * item!.SellingPrice;
            }
            return true;
        }

        private bool HasInventory(Cart cart)
        {
            foreach (var entry in cart.Items)
            {
                if (!TryResolveItem(entry.Key, out var item))
                {
                    return false;
                }

                if (item!.Quantity < entry.Value)
                {
                    Console.WriteLine($"Insufficient stock for {item.Name}. Available: {item.Quantity}.");
                    return false;
                }
            }
            return true;
        }

        private PaymentMethod PromptPaymentMethod()
        {
            while (true)
            {
                Console.WriteLine("Select payment method:");
                Console.WriteLine("1. Wallet");
                Console.WriteLine("2. Card (mock)");
                Console.Write("Choice: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        return PaymentMethod.Wallet;
                    case "2":
                        return PaymentMethod.Card;
                    default:
                        Console.WriteLine("Invalid selection. Please choose 1 or 2.");
                        break;
                }
            }
        }

        private static bool HasSufficientWalletBalance(User user, double amount)
        {
            return user.WalletBalance >= amount;
        }

        private bool ProcessCardPayment(double amount)
        {
            Console.WriteLine("Processing card payment (mock)...");
            Console.Write("Enter mock card number (press Enter to skip): ");
            Console.ReadLine();
            Console.Write("Enter mock expiry (MM/YY): ");
            Console.ReadLine();
            Console.Write("Enter mock CVV: ");
            Console.ReadLine();
            Console.WriteLine($"Card payment authorized for {amount:C}.");
            return true;
        }

        private bool TryResolveItem(int itemId, out Item? item)
        {
            item = null;
            try
            {
                item = _itemManager.GetItem(itemId);
                return true;
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Item with id {itemId} not found.");
                return false;
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
