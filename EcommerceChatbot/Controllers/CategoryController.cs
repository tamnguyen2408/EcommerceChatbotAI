using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public CategoryController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // Display products by category
        public async Task<IActionResult> Index(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                // If no category is selected, display all products
                var allProducts = await _context.Products.Include(p => p.Category).ToListAsync();
                return View(allProducts);
            }

            // Filter products by category (name)
            var productsByCategory = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName == category)
                .ToListAsync();

            if (!productsByCategory.Any())
            {
                ViewBag.Message = $"No products found for category '{category}'.";
            }

            return View(productsByCategory);
        }
    }
}
