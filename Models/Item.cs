using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyMore.Models
{
    public class Item: BaseModel
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public double CostPrice { get; set; }
        public double SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } = "General";

        public Item(int id, string name, string description, double costPrice, double sellingPrice, int quantity, string category, string createdBy)
        {
            Id = id;
            Name = name;
            Description = description;
            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Category = category;
            CreatedBy = createdBy;
        }

        public override string ToString()
        {
            return $"ID: {Id}\tName: {Name}\tCategory: {Category}\tQty: {Quantity}\tCost: {CostPrice:C}\tPrice: {SellingPrice:C}\tCreatedBy: {CreatedBy}";
        }
    }
}