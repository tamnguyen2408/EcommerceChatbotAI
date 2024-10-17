using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Controllers
{
    public class UserController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public UserController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // GET: User/Index
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
    }
}
