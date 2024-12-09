using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace ECommerceChatbot.Models
{
    public class ShoppingCart
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // Inject HttpContextAccessor
        private const string CartSessionKeyPrefix = "ShoppingCart_"; // Prefix cho key session theo UserId

        public ShoppingCart(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy UserId từ Claims hoặc trả về "guest" nếu chưa đăng nhập
        private string GetUserId()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;

            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                // Lấy UserId từ Claim
                var userIdClaim = claimsPrincipal.FindFirst("UserId");
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }

            return "guest"; // Trả về "guest" nếu người dùng chưa đăng nhập
        }

        // Lấy giỏ hàng từ session dựa trên UserId
        public List<CartItem> Items
        {
            get
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var userId = GetUserId(); // Lấy UserId
                var cartSessionKey = CartSessionKeyPrefix + userId; // Tạo key riêng cho từng người dùng
                var cartData = session.GetString(cartSessionKey);

                return string.IsNullOrEmpty(cartData)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartData);
            }
            private set
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var userId = GetUserId(); // Lấy UserId
                var cartSessionKey = CartSessionKeyPrefix + userId; // Tạo key riêng cho từng người dùng
                var cartData = JsonSerializer.Serialize(value);
                session.SetString(cartSessionKey, cartData);
            }
        }

        // Thêm sản phẩm vào giỏ hàng
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
            Items = items; // Lưu lại giỏ hàng vào session
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public bool RemoveItem(int productId)
        {
            var items = Items; // Lấy danh sách sản phẩm từ session
            var item = items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                items.Remove(item);
                Items = items; // Lưu lại giỏ hàng vào session
                return true;
            }
            return false;
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public void UpdateQuantity(int productId, int quantity)
        {
            var items = Items; // Lấy danh sách sản phẩm từ session
            var item = items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                Items = items; // Lưu lại giỏ hàng vào session
            }
        }

        // Xóa tất cả sản phẩm trong giỏ hàng
        public void Clear()
        {
            Items = new List<CartItem>(); // Xóa toàn bộ sản phẩm
        }

        // Tính tổng giá trị của giỏ hàng
        public decimal GetTotal()
        {
            return Items.Sum(i => i.Price * i.Quantity); // Tính tổng giá trị
        }
        // Cập nhật toàn bộ giỏ hàng
        public void UpdateCart(List<CartItem> items)
        {
            Items = items; // Lưu giỏ hàng mới vào session
        }

    }
}
