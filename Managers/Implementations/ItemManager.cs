using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Helpers;
using BuyMore.Managers.Interfaces;
using BuyMore.Models;
using BuyMore.Repositories.Implementations;
using BuyMore.Repositories.Interfaces;

namespace BuyMore.Managers.Implementations
{
    public class ItemManager: IItemManager
    {
        private readonly IItemRepository _itemRepository;
        public ItemManager()
        {
            _itemRepository = new ItemRepository();
        }

        public void CreateItem(string loginUser)
        {
            var name = ReadRequiredString("Enter item name: ");
            var description = ReadOptionalString("Enter item description: ");
            var costPrice = ReadDouble("Enter cost price: ", minValue: 0);
            var sellingPrice = ReadDouble("Enter selling price: ", minValue: costPrice);
            var quantity = ReadInt("Enter quantity: ", minValue: 0);
            var category = ReadRequiredString("Enter category: ");

            var item = new Item(name, description, costPrice, sellingPrice, quantity, category, loginUser);
            _itemRepository.AddItem(item);
            Console.WriteLine("Item created successfully...");
            Console.WriteLine($"Id: {item.Id}    Name: {item.Name}    Category: {item.Category}");
        }

        public void UpdateItem(string loginUser, int id)
        {
            var item = _itemRepository.GetItemById(id);
            if (item == null)
            {
                Console.WriteLine($"Item with Id: {id} not found.");
                return;
            }

            Console.WriteLine(item);

            var name = ReadRequiredString("Enter item name: ");
            var description = ReadOptionalString("Enter item description: ");
            var costPrice = ReadDouble("Enter cost price: ", minValue: 0);
            var sellingPrice = ReadDouble("Enter selling price: ", minValue: costPrice);
            var quantity = ReadInt("Enter quantity: ", minValue: 0);
            var category = ReadRequiredString("Enter category: ");

            item.Name = name;
            item.Description = description;
            item.CostPrice = costPrice;
            item.SellingPrice = sellingPrice;
            item.Quantity = quantity;
            item.Category = category;
            item.CreatedBy = loginUser;

            _itemRepository.UpdateItem(id, item);
            Console.WriteLine("Item updated successfully.");
        }

        public void GetItemById(int id)
        {
            var item = _itemRepository.GetItemById(id);
            if (item == null)
            {
                Console.WriteLine($"Item with Id: {id} not found.");
                return;
            }
            Console.WriteLine(item);
        }

        public void GetItemByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Please provide a valid item name.");
                return;
            }

            var matchingItems = _itemRepository.GetAllItems()
                .Where(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (matchingItems.Count == 0)
            {
                Console.WriteLine($"Item with Name: {name} not found.");
                return;
            }

            foreach (var item in matchingItems)
            {
                Console.WriteLine(item);
            }
        }

        public void GetAll()
        {
            var items = _itemRepository.GetAllItems();
            if (items.Count == 0)
            {
                Console.WriteLine("No item found.");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public Item GetItem(int id)
        {
            var item = _itemRepository.GetItemById(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Item with Id: {id} not found.");
            }

            return item;
        }

        public void Search(string? category = null, string? searchKey = null, double minPriceRange = 0, double maxPriceRange = 0)
        {
            var items = _itemRepository.GetAllItems();
            if (items.Count == 0)
            {
                Console.WriteLine("No item found.");
                return;
            }

            if (minPriceRange < 0 || maxPriceRange < 0)
            {
                Console.WriteLine("Price range values cannot be negative.");
                return;
            }

            if (minPriceRange > 0 && maxPriceRange > 0 && minPriceRange > maxPriceRange)
            {
                (minPriceRange, maxPriceRange) = (maxPriceRange, minPriceRange);
            }

            var results = _itemRepository.GetAllItems().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                results = results.Where(i => i.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                var key = searchKey.Trim();
                results = results.Where(i =>
                    i.Name.Contains(key, StringComparison.InvariantCultureIgnoreCase) ||
                    (!string.IsNullOrEmpty(i.Description) && i.Description.Contains(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (minPriceRange > 0)
            {
                results = results.Where(i => i.SellingPrice >= minPriceRange);
            }

            if (maxPriceRange > 0)
            {
                results = results.Where(i => i.SellingPrice <= maxPriceRange);
            }

            var filteredItems = results.ToList();
            if (filteredItems.Count == 0)
            {
                Console.WriteLine("No matching items found.");
                return;
            }

            foreach (var item in filteredItems)
            {
                Console.WriteLine(item);
            }
        }

        private static string ReadRequiredString(string prompt)
        {
            string? value;
            do
            {
                Console.Write(prompt);
                value = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(value));
            return value.Trim();
        }

        private static string ReadOptionalString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        private static double ReadDouble(string prompt, double minValue = double.MinValue)
        {
            double value;
            Console.Write(prompt);
            var input = Console.ReadLine();
            while (!double.TryParse(input, out value) || value < minValue)
            {
                Console.WriteLine(minValue == double.MinValue
                    ? "Please enter a valid number."
                    : $"Please enter a number greater than or equal to {minValue}.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return value;
        }

        private static int ReadInt(string prompt, int minValue = int.MinValue)
        {
            int value;
            Console.Write(prompt);
            var input = Console.ReadLine();
            while (!int.TryParse(input, out value) || value < minValue)
            {
                Console.WriteLine(minValue == int.MinValue
                    ? "Please enter a valid whole number."
                    : $"Please enter a whole number greater than or equal to {minValue}.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return value;
        }
    }
}
