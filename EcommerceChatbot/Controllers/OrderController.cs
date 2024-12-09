using System;
using System.Linq;
using System.Threading.Tasks;
using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceChatbot.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Stripe.Checkout;
using Stripe;

namespace EcommerceChatbot.Controllers
{
    [Authorize(AuthenticationSchemes = "UserCookie", Roles = "user")]

    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly ECommerceAiDbContext _context;
        private readonly ShoppingCart _shoppingCart;
        private readonly IConfiguration _configuration;


        public OrderController(ECommerceAiDbContext context, ShoppingCart shoppingCart, ILogger<OrderController> logger, IConfiguration configuration)
        {
            _context = context;
            _shoppingCart = shoppingCart;
            _logger = logger;
            _configuration = configuration;

        }

        [HttpGet]
        public IActionResult Checkout(string selectedProductIds) // Accept selected product IDs as a parameter
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

            // Kiểm tra xem có sản phẩm nào được chọn không
            if (string.IsNullOrEmpty(selectedProductIds))
            {
                TempData["ErrorMessage"] = "Please select at least one product to proceed with checkout.";
                return RedirectToAction("Index", "Cart");
            }

            // Chuyển đổi chuỗi selectedProductIds thành một danh sách ID sản phẩm
            var productIds = selectedProductIds.Split(',').Select(int.Parse).ToList();

            // Lọc ra các sản phẩm được chọn trong giỏ hàng
            var selectedItems = cartItems.Where(item => productIds.Contains(item.ProductId)).ToList();
            if (!selectedItems.Any())
            {
                TempData["ErrorMessage"] = "No valid products selected for checkout.";
                return RedirectToAction("Index", "Cart");
            }

            // Tính tổng tiền của các sản phẩm đã chọn
            var totalAmount = selectedItems.Sum(c => c.Price * c.Quantity);

            // Khởi tạo model CheckoutViewModel
            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = selectedItems.Select(item => new EcommerceChatbot.Models.CartItem
                {
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl // Include the image URL
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
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to proceed with checkout.";
                return RedirectToAction("Index", "User");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
                return RedirectToAction("Index", "User");
            }

            var cartItems = _shoppingCart.Items;
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Create an order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                OrderStatus = "Pending",
                UpdatedAt = DateTime.Now,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes
            };

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

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Setup Stripe API
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            // Create a PaymentIntent with Automatic Payment Methods enabled
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100), // Convert to the smallest currency unit (e.g., cents)
                Currency = "usd", // Set the currency
                PaymentMethod = model.PaymentMethodId, // This should be passed from your frontend (Stripe Element)
                Confirm = true, // Automatically confirm the payment
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never" // Disable redirect-based payment methods
                }
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = await service.CreateAsync(options);

            if (paymentIntent.Status == "succeeded")
            {
                // Create a payment record
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    Amount = order.TotalAmount,
                    PaymentStatus = "Paid",
                    PaymentDate = DateTime.Now,
                    PaymentMethod = model.PaymentMethod,
                    StripeSessionId = paymentIntent.Id // Store the Stripe Session ID
                };

                _context.Payments.Add(payment);
                order.OrderStatus = "Paid"; // Update the order status to Paid
                await _context.SaveChangesAsync();

                // Clear the shopping cart and redirect to Order History
                _shoppingCart.Clear();
                TempData["SuccessMessage"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderHistory");
            }
            else
            {
                // If payment is not successful
                TempData["ErrorMessage"] = "Payment failed. Please try again.";
                return RedirectToAction("Checkout");
            }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceipt(int id)
        {
            // Check if the user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please log in to confirm receipt of your order.";
                return RedirectToAction("Index", "User");
            }

            // Get UserId from Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
                return RedirectToAction("Index", "User");
            }

            // Find the order by ID and ensure it belongs to the logged-in user
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found or access denied.";
                return RedirectToAction("OrderHistory");
            }

            // Check if the order status is "Shipping"
            if (order.OrderStatus != "Shipping")
            {
                TempData["ErrorMessage"] = "You can only confirm receipt for orders that are currently shipping.";
                return RedirectToAction("OrderHistory");
            }

            // Update the order status
            order.OrderStatus = "Delivered";
            order.UpdatedAt = DateTime.Now;

            // Save changes
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you for confirming receipt of your order!";
            return RedirectToAction("OrderHistory");
        }

       // Hành động hủy đơn hàng
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CancelOrder(int id, string cancelReason)
{
    // Kiểm tra nếu người dùng đã đăng nhập
    if (!User.Identity.IsAuthenticated)
    {
        TempData["ErrorMessage"] = "Please log in to cancel your order.";
        return RedirectToAction("Index", "User");
    }

    // Lấy UserId từ Claims
    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
    {
        TempData["ErrorMessage"] = "Unable to retrieve user information. Please log in again.";
        return RedirectToAction("Index", "User");
    }

    // Tìm đơn hàng của người dùng với id tương ứng
    var order = await _context.Orders
        .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

    if (order == null)
    {
        TempData["ErrorMessage"] = "Order not found or you do not have permission to cancel it.";
        return RedirectToAction("OrderHistory");
    }

    // Kiểm tra trạng thái đơn hàng, chỉ cho phép hủy nếu trạng thái là Pending hoặc Confirmed
    if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
    {
        TempData["ErrorMessage"] = "Only orders with status 'Pending' or 'Confirmed' can be canceled.";
        return RedirectToAction("OrderHistory");
    }

    // Cập nhật trạng thái đơn hàng và lý do hủy
    order.OrderStatus = "Canceled";
    order.CancelReason = cancelReason; // Giả sử `CancelReason` là một cột trong bảng Orders
    order.CancelDate = DateTime.UtcNow; // Tùy chọn: thêm thời gian hủy (nếu cần)

    // Lưu thay đổi vào cơ sở dữ liệu
    _context.Orders.Update(order);
    await _context.SaveChangesAsync();

    // Thông báo hủy thành công
    TempData["SuccessMessage"] = "Order canceled successfully!";
    return RedirectToAction("OrderHistory");
}

        [HttpGet]
        public IActionResult Details(int id)
        {
            // Lấy thông tin đơn hàng từ database
            var order = _context.Orders
                .Include(o => o.User)          // Lấy thông tin người dùng
                .Include(o => o.OrderItems)    // Lấy thông tin sản phẩm trong đơn hàng
                .Where(o => o.OrderId == id)
                .FirstOrDefault();

            // Kiểm tra nếu không tìm thấy đơn hàng
            if (order == null)
            {
                return NotFound();
            }

            // Tạo ViewModel để chuyển dữ liệu sang view
            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                Notes = order.Notes,
                OrderDate = order.OrderDate ?? DateTime.MinValue,
                OrderStatus = order.OrderStatus,
                UserName = order.User?.UserName ?? "Unknown",
                Email = order.User?.Email ?? "Unknown",
                OrderItems = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    ProductName = item.ProductName,
                    ImageUrl = item.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Subtotal
                }).ToList()
            };

            return View(orderDetailsViewModel);
        }
       

    }
}
