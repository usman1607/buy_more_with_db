namespace BuyMore.Models
{
    public class Cart: BaseModel
    {
        public int UserId { get; }
        public string UserEmail { get; }
        public Dictionary<int, int> Items { get; } = new Dictionary<int, int>();

        public Cart(int userId, string userEmail)
        {
            UserId = userId;
            UserEmail = userEmail;
            CreatedBy = userEmail;
        }

        public Cart(int id, int userId, string userEmail, Dictionary<int, int> items, string createdBy, DateTime createdDate)
        {
            Id = id;
            UserId = userId;
            UserEmail = userEmail;
            Items = items;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        public void AddItem(int itemId, int quantity)
        {
            if (Items.ContainsKey(itemId))
            {
                Items[itemId] += quantity;
            }
            else
            {
                Items[itemId] = quantity;
            }
        }

        public void RemoveItem(int itemId, int quantity)
        {
            if (!Items.ContainsKey(itemId))
            {
                return;
            }

            if (quantity <= 0 || Items[itemId] <= quantity)
            {
                Items.Remove(itemId);
            }
            else
            {
                Items[itemId] -= quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
