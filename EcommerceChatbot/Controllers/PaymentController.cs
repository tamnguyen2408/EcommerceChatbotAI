using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EcommerceChatbot.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        // Action này sẽ nhận callback từ VNPay sau khi thanh toán
        [HttpGet]
        public IActionResult PaymentCallback()
        {
            // Lấy các tham số trả về từ VNPay
            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
            var vnp_TxnRef = Request.Query["vnp_TxnRef"];
            var vnp_SecureHash = Request.Query["vnp_SecureHash"];
            var vnp_Amount = Request.Query["vnp_Amount"];
            var vnp_OrderInfo = Request.Query["vnp_OrderInfo"];

            // Log các tham số để kiểm tra
            _logger.LogInformation($"vnp_ResponseCode: {vnp_ResponseCode}, vnp_TxnRef: {vnp_TxnRef}, vnp_SecureHash: {vnp_SecureHash}, vnp_Amount: {vnp_Amount}, vnp_OrderInfo: {vnp_OrderInfo}");

            // Kiểm tra tính hợp lệ của dữ liệu trả về từ VNPay
            if (vnp_ResponseCode == "00")
            {
                // Thanh toán thành công
                _logger.LogInformation($"Payment successful for Order {vnp_TxnRef}, Amount: {vnp_Amount}");
                return View("Success"); // Trả về view thành công
            }
            else
            {
                // Thanh toán thất bại
                _logger.LogInformation($"Payment failed for Order {vnp_TxnRef}");
                return View("Failure"); // Trả về view thất bại
            }
        }
    }
}
