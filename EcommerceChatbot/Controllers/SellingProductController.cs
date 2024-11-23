using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceChatbot.Controllers
{
    public class SellingProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ECommerceAiDbContext _context;

        public SellingProductController(ILogger<HomeController> logger, ECommerceAiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            List<Product> products;

            if (category.ToLower() == "all")
            {
                products = await _context.Products.Include(p => p.Category).ToListAsync();
            }
            else
            {
                products = await _context.Products
                                         .Include(p => p.Category)
                                         .Where(p => p.Category.CategoryName.ToLower() == category.ToLower())
                                         .ToListAsync();
            }

            // Trả về Partial View
            return PartialView("_ProductListPartial", products);
        }


    }
}