using System.Collections.Generic;

namespace EcommerceChatbot.Models
{
    public class CheckoutViewModel
    {
        // Tổng tiền thanh toán
        public decimal TotalPrice { get; set; }

        // Thông tin khách hàng
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        // Danh sách CartItems
        public List<CartItem> CartItems { get; set; } = new List<CartItem>(); // Khởi tạo để tránh null
    }
}
