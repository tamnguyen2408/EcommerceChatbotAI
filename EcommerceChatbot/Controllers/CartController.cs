﻿using Microsoft.AspNetCore.Mvc;
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
    public JsonResult AddToCart(int productId, string productName, decimal price, string productImage, int quantity = 1)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { success = false, redirectToLogin = true });
        }

        // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
        var existingItem = _shoppingCart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            return Json(new { success = false, message = "This product is already in your cart." });
        }

        // Thêm sản phẩm mới vào giỏ hàng
        var item = new CartItem
        {
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity,
            ImageUrl = productImage // Cập nhật hình ảnh sản phẩm
        };

        _shoppingCart.AddItem(item);

        var totalItems = _shoppingCart.Items.Sum(i => i.Quantity);

        return Json(new { success = true, totalItems });
    }




    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        _shoppingCart.RemoveItem(productId); // Xóa sản phẩm khỏi giỏ hàng
        TempData["SuccessMessage"] = "Product removed successfully!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 1)
        {
            ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
            return RedirectToAction("Index");
        }

        _shoppingCart.UpdateQuantity(productId, quantity); // Cập nhật số lượng sản phẩm

        // Thêm thông báo thành công vào TempData
        TempData["SuccessMessage"] = "Product quantity updated successfully!";

        return RedirectToAction("Index"); // Quay lại trang giỏ hàng
    }



    [HttpGet]
    public JsonResult GetCartItemCount()
    {
        var totalItems = _shoppingCart.Items.Sum(item => item.Quantity); // Tổng số lượng sản phẩm
        return Json(totalItems);
    }

}