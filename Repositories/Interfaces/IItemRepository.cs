using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Models;

namespace BuyMore.Repositories.Interfaces
{
    public interface IItemRepository
    {
        void AddItem(Item item);
        Item? GetItemById(int id);
        List<Item> GetAllItems();
        bool UpdateItem(int id, Item item);
        bool DeleteItem(int id);
    }
}