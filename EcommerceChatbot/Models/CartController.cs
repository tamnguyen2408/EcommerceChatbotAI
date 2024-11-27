using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    // Hiển thị trang checkout
    [HttpGet("cart/checkout")]
    public IActionResult Checkout()
    {
        return View();  // Trả về trang checkout.cshtml
    }

    // Xử lý thanh toán qua VNPay
    [HttpPost("cart/checkout/vnpay")]
    public IActionResult RedirectToVNPay()
    {
        // Xử lý thanh toán VNPay
        return RedirectToAction("Index", "Home");  // Chuyển hướng về trang chính sau khi thanh toán
    }
}

