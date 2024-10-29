using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public UserController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // GET: User/Index
         // Only allow regular users

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            var productById = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productById == null) return NotFound();

            return View(productById);
        }




    }
}
