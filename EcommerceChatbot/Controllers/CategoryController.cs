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

        // Display products by category and optionally filter by gender
        public async Task<IActionResult> Index(string category, string? gender)
        {
            IQueryable<Product> productsQuery = _context.Products.Include(p => p.Category);

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category.CategoryName == category);
            }

            // Filter by gender
            if (!string.IsNullOrEmpty(gender))
            {
                productsQuery = productsQuery.Where(p => p.Gender == gender);
            }

            var products = await productsQuery.ToListAsync();

            // Check if no products were found
            if (!products.Any())
            {
                ViewBag.Message = $"No products found for category '{category}'" +
                                  (string.IsNullOrEmpty(gender) ? "" : $" and gender '{gender}'.");
            }

            return View(products);
        }
    }
}
