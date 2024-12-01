using System;
using System.Linq;
using System.Threading.Tasks;
using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceChatbot.Models;
using System.Security.Claims;

namespace EcommerceChatbot.Controllers
{
    public class OrderController : Controller
    {
        private readonly ECommerceAiDbContext _context;
        private readonly ShoppingCart _shoppingCart;

        public OrderController(ECommerceAiDbContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to proceed with checkout.";
                return RedirectToAction("Index", "User");
            }

            // Lấy UserId từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
                return RedirectToAction("Index", "User");
            }

            // Lấy danh sách sản phẩm trong giỏ hàng
            var cartItems = _shoppingCart.Items; // Lấy các sản phẩm từ giỏ hàng
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Tính tổng tiền của giỏ hàng
            var totalAmount = cartItems.Sum(c => c.Price * c.Quantity);

            // Khởi tạo model CheckoutViewModel
            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems.Select(item => new EcommerceChatbot.Models.CartItem
                {
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl // Ensure you include the image URL here
                }).ToList(),
                TotalAmount = totalAmount
            };

            // Khi trả về View, sử dụng TempData.Keep để giữ lại thông báo lỗi nếu có
            TempData.Keep("ErrorMessage"); // Keep the error message for the next request

            return View(checkoutViewModel); // Trả về View với CheckoutViewModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to proceed with checkout.";
                return RedirectToAction("Index", "User");
            }

            // Lấy UserId từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
                return RedirectToAction("Index", "User");
            }

            // Lấy danh sách sản phẩm trong giỏ hàng
            var cartItems = _shoppingCart.Items; // Lấy các sản phẩm từ giỏ hàng
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                OrderStatus = "Pending",
                UpdatedAt = DateTime.Now,
                FullName = model.FullName, // Add FullName from model
                PhoneNumber = model.PhoneNumber, // Add PhoneNumber from model
                Address = model.Address, // Add Address from model
                PaymentMethod = model.PaymentMethod, // Add PaymentMethod from model
                Notes = model.Notes // Add Notes from model
            };

            // Thêm sản phẩm vào OrderItems
            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ImageUrl = item.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    Subtotal = item.Price * item.Quantity
                });
            }

            // Thêm OrderDetails vào Order
            order.OrderDetails.Add(new OrderDetail
            {
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                Notes = string.IsNullOrEmpty(model.Notes) ? null : model.Notes // Cho phép giá trị null nếu không có
            });

            // Lưu đơn hàng vào database
            _context.Orders.Add(order);

            // Xóa giỏ hàng của người dùng
            _shoppingCart.Clear(); // Xóa tất cả sản phẩm khỏi giỏ hàng

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your order has been placed successfully!";
            return RedirectToAction("OrderHistory");
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to view your order history.";
                return RedirectToAction("Index", "User");
            }

            // Lấy UserId từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
                return RedirectToAction("Index", "User");
            }

            // Lấy danh sách đơn hàng của user
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.OrderDetails) // Include OrderDetails
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }
    }
}
