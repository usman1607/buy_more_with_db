using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Models;

namespace BuyMore.Managers.Interfaces
{
    public interface IItemManager
    {
        void CreateItem(string loginUser);
        void UpdateItem(string loginUser, int id);
        void GetItemById(int id);
        void GetItemByName(string name);//Should print items because more than one Item can have the same name
        void GetAll();
        Item GetItem(int id);
        void Search(string? category = null, string? searchKey = null, double minPriceRange = 0, double maxPriceRange = 0);

    }
}