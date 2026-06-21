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
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } = "General";

        public Item(int id, string name, string description, decimal costPrice, decimal sellingPrice, int quantity, string category, string createdBy, DateTime createdDate)
        {
            Id = id;
            Name = name;
            Description = description;
            CostPrice = costPrice;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            Category = category;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        public Item(string name, string description, decimal costPrice, decimal sellingPrice, int quantity, string category, string createdBy)
        {
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