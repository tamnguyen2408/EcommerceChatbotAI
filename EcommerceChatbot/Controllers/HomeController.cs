
using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceChatbot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ECommerceAiDbContext _context;

        public HomeController(ILogger<HomeController> logger, ECommerceAiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return RedirectToAction("Index");

            var productById = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productById == null) return NotFound();

            return View(productById);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost("auth/logout")]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie"); // Adjust the authentication scheme if necessary
            return RedirectToAction("Index", "Home", new { area = "" }); // Redirect to the admin login page
        }
    }
}
