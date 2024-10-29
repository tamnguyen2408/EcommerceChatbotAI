using EcommerceChatbot.Models;
using Google;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    public class ProductController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public ProductController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            ViewData["Title"] = product.ProductName;  // Đặt Title cho layout

            return View(product);
        }

    }
}
