using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult RedirectToVNPay(decimal totalAmount)
        {
            // Tạo link thanh toán VNPay dựa trên các thông số yêu cầu của VNPay
            var vnp_Url = "http://sandbox.vnpayment.vn/tryitnow/Home/CreateOrder";

            // Các tham số cần thiết cho VNPay
            var paymentUrl = $"{vnp_Url}?amount={totalAmount * 100}"; // số tiền tính theo đồng, chuyển đổi sang đơn vị nhỏ nhất nếu cần

            // Chuyển hướng người dùng đến URL thanh toán của VNPay
            return Redirect(paymentUrl);
        }
    }
}
