using BuyMore.Models;
using BuyMore.Repositories.Interfaces;
using Npgsql;

namespace BuyMore.Repositories.Implementations
{
    public class ItemRepository : IItemRepository
    {
        public void AddItem(Item item)
        {
            using var connection = Database.GetConnection();
            var sql = "INSERT INTO items (name, description, cost_price, selling_price, quantity, category, created_by, created_date) VALUES (@name, @description, @costPrice, @sellingPrice, @quantity, @category, @createdBy, @createdDate)";
            using var cmd = new NpgsqlCommand(sql, connection);
            Database.AddParameter(cmd, new Dictionary<string, object>
            {
                { "name", item.Name },
                { "description", item.Description ?? string.Empty },
                { "costPrice", item.CostPrice },
                { "sellingPrice", item.SellingPrice },
                { "quantity", item.Quantity },
                { "category", item.Category},
                { "createdBy", item.CreatedBy },
                { "createdDate", item.CreatedDate }
            });
            cmd.ExecuteNonQuery();
        }

        public bool DeleteItem(int id)
        {
            using var connection = Database.GetConnection();
            var sql = "DELETE FROM items WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("id", id);
            var rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Item> GetAllItems()
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM items";
            using var cmd = new NpgsqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            var items = new List<Item>();
            while (reader.Read())
            {
                items.Add(new Item(
                    (int)reader["id"],
                    reader["name"].ToString()!,
                    reader["description"].ToString()!,
                    (decimal)reader["cost_price"],
                    (decimal)reader["selling_price"],
                    (int)reader["quantity"],
                    reader["category"].ToString()!,
                    reader["created_by"].ToString()!,
                    (DateTime)reader["created_date"]
                ));
            }
            return items;
        }
        

        public Item? GetItemById(int id)
        {
            using var connection = Database.GetConnection();
            var sql = "SELECT * FROM items WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Item(
                    (int)reader["id"],
                    reader["name"].ToString()!,
                    reader["description"].ToString()!,
                    (decimal)reader["cost_price"],
                    (decimal)reader["selling_price"],
                    (int)reader["quantity"],
                    reader["category"].ToString()!,
                    reader["created_by"].ToString()!,
                    (DateTime)reader["created_date"]
                );
            }
            return null;
        }

        public bool UpdateItem(int id, Item item)
        {
            using var connection = Database.GetConnection();
            var sql = "UPDATE items SET name = @name, description = @description, cost_price = @costPrice, selling_price = @sellingPrice, quantity = @quantity, category = @category WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, connection);
            Database.AddParameter(cmd, new Dictionary<string, object>
            {
                { "id", id },
                { "name", item.Name },
                { "description", item.Description ?? string.Empty },
                { "costPrice", item.CostPrice },
                { "sellingPrice", item.SellingPrice },
                { "quantity", item.Quantity },
                { "category", item.Category }
            });
            var rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}