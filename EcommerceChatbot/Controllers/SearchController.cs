using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Controllers
{
    public class SearchController : Controller
    {
        private readonly ECommerceAiDbContext _context;
        public SearchController(ECommerceAiDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .Where(p => EF.Functions.Like(p.ProductName, $"%{searchTerm}%"))
                                         .OrderBy(p => p.ProductName)
                                         .ToListAsync();

            if (!products.Any())
            {
                ViewBag.Message = $"No products found with keyword'{searchTerm}'.";
            }

            return View(products); // Sử dụng view 'Index' để hiển thị kết quả
        }
    }
}
