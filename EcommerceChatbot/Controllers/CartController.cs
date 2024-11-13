using Microsoft.AspNetCore.Mvc;
using EcommerceChatbot.Extensions; // Thêm dòng này để sử dụng extension method
using EcommerceChatbot.Models;
using System.Linq;

namespace EcommerceChatbot.Controllers
{
    public class CartController : Controller
    {
        // Action Checkout để hiển thị trang Checkout
        public IActionResult Checkout()
        {
            // Giả sử bạn lấy giỏ hàng từ session (hoặc từ cơ sở dữ liệu)
            var cart = GetCart(); // Phương thức này cần được cài đặt để lấy giỏ hàng từ session hoặc nơi lưu trữ khác

            // Tính toán tổng tiền của giỏ hàng
            var totalPrice = cart.Sum(item => item.Price * item.Quantity); // Cộng tổng tiền giỏ hàng

            // Tạo ViewModel và truyền vào View
            var model = new CheckoutViewModel
            {
                TotalPrice = totalPrice,
                // Truyền thêm các dữ liệu khác nếu cần
            };

            return View(model); // Trả về view Checkout.cshtml với ViewModel chứa thông tin cần thiết
        }

        // Phương thức để lấy giỏ hàng (ví dụ: từ session)
        private List<CartItem> GetCart()
        {
            // Ví dụ: Giả sử giỏ hàng lưu trong session
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return cart;
        }
    }

}
