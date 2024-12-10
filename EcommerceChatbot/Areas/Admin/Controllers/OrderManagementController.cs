using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = "admin")]
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public OrderManagementController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng chờ xác nhận
        public IActionResult Index(string orderStatus)
        {
            var orderStatuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "" },
                new SelectListItem { Text = "Pending", Value = "Pending" },
                new SelectListItem { Text = "Confirmed", Value = "Confirmed" },
                new SelectListItem { Text = "Shipping", Value = "Shipping" },
                new SelectListItem { Text = "Completed", Value = "Completed" },
                new SelectListItem { Text = "Rejected", Value = "Rejected" },
                new SelectListItem { Text = "Paid", Value = "Paid" }
            };

            ViewBag.OrderStatuses = orderStatuses;

            var ordersQuery = _context.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(orderStatus))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == orderStatus);
            }

            var orders = ordersQuery.OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        // Chi tiết đơn hàng
        [HttpGet]
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == id);

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
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            if (order.OrderStatus != "Pending")
            {
                TempData["ErrorMessage"] = "Only pending orders can be confirmed.";
                return RedirectToAction("Index");
            }

            order.OrderStatus = "Confirmed";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

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
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
            {
                TempData["ErrorMessage"] = "Only pending or confirmed orders can be rejected.";
                return RedirectToAction("Index");
            }

            order.OrderStatus = "Rejected";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            SendOrderStatusNotification((int)order.UserId, "rejected");
            TempData["SuccessMessage"] = "Order has been rejected.";
            return RedirectToAction("Index");
        }

        // Giao đơn hàng cho nhà vận chuyển
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HandOverToCarrier(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            if (order.OrderStatus != "Confirmed" && order.OrderStatus != "Paid")
            {
                TempData["ErrorMessage"] = "Only confirmed or paid orders can be handed over to the carrier.";
                return RedirectToAction("Index");
            }

            order.OrderStatus = "Shipping";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            SendOrderStatusNotification((int)order.UserId, "shipped");
            TempData["SuccessMessage"] = "Order has been handed over to the carrier.";
            return RedirectToAction("Index");
        }
        // Xóa đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            // Allow deletion only for rejected or canceled orders
            if (order.OrderStatus != "Rejected" && order.OrderStatus != "Canceled")
            {
                TempData["ErrorMessage"] = "Only rejected or canceled orders can be deleted.";
                return RedirectToAction("Index");
            }

            // Remove related payment records
            var payment = _context.Payments
                .FirstOrDefault(p => p.OrderId == id);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            // Remove associated order items
            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Order has been deleted successfully.";
            return RedirectToAction("Index");
        }



        // Đánh dấu đơn hàng đã thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsPaid(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            // Kiểm tra trạng thái đơn hàng
            if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
            {
                TempData["ErrorMessage"] = "Only pending or confirmed orders can be marked as paid.";
                return RedirectToAction("Index");
            }

            // Kiểm tra phương thức thanh toán
            if (order.PaymentMethod != "COD")
            {
                TempData["ErrorMessage"] = "Only orders with Cash on Delivery (COD) payment method can be marked as paid.";
                return RedirectToAction("Index");
            }

            // Đánh dấu đơn hàng là đã thanh toán
            order.OrderStatus = "Paid";
            order.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            // Gửi thông báo trạng thái đơn hàng
            SendOrderStatusNotification((int)order.UserId, "paid");
            TempData["SuccessMessage"] = "Order has been marked as paid.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancelRequest(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.OrderStatus == "Cancel Requested");
            if (order == null)
            {
                TempData["ErrorMessage"] = "Cancellation request not found.";
                return RedirectToAction("Index");
            }

            order.OrderStatus = "Canceled";
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cancellation request has been approved.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancelRequest(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.OrderStatus == "Cancel Requested");
            if (order == null)
            {
                TempData["ErrorMessage"] = "Cancellation request not found.";
                return RedirectToAction("Index");
            }

            order.OrderStatus = "Pending";
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cancellation request has been rejected.";
            return RedirectToAction("Index");
        }

        // Gửi thông báo cho người dùng
        private void SendOrderStatusNotification(int userId, string status)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                var emailSubject = $"Your order has been {status}";
                var emailBody = $"Dear {user.UserName},\n\nYour order status has been updated to '{status}'.\nThank you for shopping with us!";
                // EmailService.SendEmailAsync(user.Email, emailSubject, emailBody); // Implement email sending
            }
        }
    }
}
