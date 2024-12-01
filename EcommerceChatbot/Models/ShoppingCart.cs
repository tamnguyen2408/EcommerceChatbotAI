using System.Text.Json;

namespace ECommerceChatbot.Models
{
    public class ShoppingCart
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // Inject HttpContextAccessor
        private const string CartSessionKey = "ShoppingCart"; // Key để lưu trong session

        public ShoppingCart(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> Items
        {
            get
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartData = session.GetString(CartSessionKey);
                return string.IsNullOrEmpty(cartData)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartData);
            }
            private set
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var cartData = JsonSerializer.Serialize(value);
                session.SetString(CartSessionKey, cartData);
            }
        }

        public void AddItem(CartItem item)
        {
            var items = Items; // Lấy danh sách sản phẩm từ session
            var existingItem = items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity; // Cập nhật số lượng
            }
            else
            {
                items.Add(item); // Thêm sản phẩm mới
            }
            Items = items; // Lưu danh sách sản phẩm vào session
        }

        public bool RemoveItem(int productId)
        {
            var items = Items; // Lấy danh sách sản phẩm từ session
            var item = items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                items.Remove(item);
                Items = items; // Lưu lại session
                return true;
            }
            return false;
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var items = Items; // Lấy danh sách sản phẩm từ session
            var item = items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                Items = items; // Lưu lại session
            }
        }

        public void Clear()
        {
            Items = new List<CartItem>(); // Xóa toàn bộ sản phẩm
        }

        public decimal GetTotal()
        {
            return Items.Sum(i => i.Price * i.Quantity); // Tính tổng giá trị
        }
    }
}