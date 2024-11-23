using Microsoft.AspNetCore.Mvc;
using ECommerceChatbot.Models;

public class CartController : Controller
{
    private readonly ShoppingCart _shoppingCart;

    public CartController(ShoppingCart shoppingCart)
    {
        _shoppingCart = shoppingCart;
    }

    public IActionResult Index()
    {
        var items = _shoppingCart.Items; // Lấy danh sách sản phẩm từ session
        return View(items);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, string productName, decimal price, int quantity = 1)
    {
        var item = new CartItem
        {
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };

        _shoppingCart.AddItem(item); // Thêm sản phẩm vào giỏ hàng
        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        _shoppingCart.RemoveItem(productId); // Xóa sản phẩm
        return RedirectToAction("Index");
    }
}
