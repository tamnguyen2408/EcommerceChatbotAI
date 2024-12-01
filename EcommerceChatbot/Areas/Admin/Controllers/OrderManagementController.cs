using Microsoft.AspNetCore.Mvc;
using EcommerceChatbot.Models;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public OrderManagementController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng chờ xác nhận
        public IActionResult Index()
        {
            var pendingOrders = _context.Orders
                .Include(o => o.User)  // Nạp thông tin người dùng
                .Where(o => o.OrderStatus == "Pending")
                .OrderBy(o => o.OrderDate)
                .ToList();

            return View(pendingOrders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)          // Thông tin người dùng
                .Include(o => o.OrderItems)    // Sản phẩm trong đơn hàng
                .Where(o => o.OrderId == id)
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                Notes = order.Notes,
                OrderDate = (DateTime)order.OrderDate,
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



        // Xác nhận đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Confirmed"
            order.OrderStatus = "Confirmed";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            // Gửi thông báo cho người dùng (ví dụ, qua email hoặc thông báo trong ứng dụng)
            SendOrderStatusNotification((int)order.UserId, "confirmed");

            TempData["SuccessMessage"] = "Order has been confirmed successfully.";
            return RedirectToAction("Index");
        }

        // Từ chối đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Rejected"
            order.OrderStatus = "Rejected";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            // Gửi thông báo cho người dùng (ví dụ, qua email hoặc thông báo trong ứng dụng)
            SendOrderStatusNotification((int)order.UserId, "rejected");

            TempData["SuccessMessage"] = "Order has been rejected.";
            return RedirectToAction("Index");
        }

        // Gửi thông báo cho người dùng khi đơn hàng được xác nhận hoặc từ chối
        private void SendOrderStatusNotification(int userId, string status)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                var emailSubject = $"Your order has been {status}";
                var emailBody = $"Dear {user.UserName},\n\nYour order status has been updated to {status}.\nThank you for shopping with us!";

                // Gửi email thông báo (sử dụng thư viện gửi email hoặc API gửi email)
                // Ví dụ: EmailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
        }
    }
}
