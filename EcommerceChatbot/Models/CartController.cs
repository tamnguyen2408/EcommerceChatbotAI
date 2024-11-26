using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    // Action để hiển thị trang checkout
    public IActionResult Checkout()
    {
        return View();  // Trả về trang checkout.cshtml
    }

    // Action để xử lý thanh toán qua VNPay
    [HttpPost]
    public IActionResult RedirectToVNPay()
    {
        // Xử lý thanh toán VNPay ở đây
        return RedirectToAction("Index", "Home");  // Chuyển hướng về trang chính sau khi thanh toán
    }
}
