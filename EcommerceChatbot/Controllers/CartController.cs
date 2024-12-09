using Microsoft.AspNetCore.Mvc;
using ECommerceChatbot.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
namespace EcommerceChatbot.Controllers
{
    [Authorize(AuthenticationSchemes = "UserCookie", Roles = "user")]

    public class CartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;

        public CartController(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        // Lấy danh sách sản phẩm trong giỏ hàng theo người dùng
        public IActionResult Index()
        {
            var items = _shoppingCart.Items; // Lấy giỏ hàng theo người dùng
            return View(items);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public JsonResult AddToCart(int productId, string productName, decimal price, string productImage, int quantity = 1)
        {
            // Kiểm tra nếu người dùng đã đăng nhập qua cookie UserCookie
            if (!User.Identity.IsAuthenticated || !User.HasClaim(c => c.Type == "UserId"))
            {
                return Json(new { success = false, redirectToLogin = true });
            }

            // Lấy UserId từ Claims
            var userIdString = User.FindFirst("UserId")?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Invalid User ID." });
            }

            // Lấy giỏ hàng hiện tại
            var items = _shoppingCart.Items;

            // Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng
            var existingItem = items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                // Cập nhật số lượng nếu sản phẩm đã tồn tại
                existingItem.Quantity += quantity;
            }
            else
            {
                // Thêm sản phẩm mới
                var item = new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity,
                    ImageUrl = productImage,
                    UserId = userId // Đảm bảo gắn UserId vào sản phẩm
                };
                items.Add(item);
            }

            // Cập nhật lại giỏ hàng thông qua phương thức UpdateCart
            _shoppingCart.UpdateCart(items);

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var totalItems = items.Sum(i => i.Quantity);

            return Json(new { success = true, totalItems });
        }


        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _shoppingCart.RemoveItem(productId); // Xóa sản phẩm khỏi giỏ hàng theo người dùng
            TempData["SuccessMessage"] = "Product removed successfully!";
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1)
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1.");
                return RedirectToAction("Index");
            }

            _shoppingCart.UpdateQuantity(productId, quantity); // Cập nhật số lượng sản phẩm trong giỏ hàng

            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Product quantity updated successfully!";

            return RedirectToAction("Index"); // Quay lại trang giỏ hàng
        }

        // Lấy tổng số lượng sản phẩm trong giỏ hàng
        [HttpGet]
        public JsonResult GetCartItemCount()
        {
            var totalItems = _shoppingCart.Items.Sum(item => item.Quantity); // Tổng số lượng sản phẩm
            return Json(totalItems);
        }
    }
}
