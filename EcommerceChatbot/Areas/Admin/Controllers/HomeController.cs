
﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
﻿using EcommerceChatbot.Models; // Thêm namespace cho mô hình Product
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceChatbot.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("admin")]
    [Authorize(Roles = "admin")] // Only allow admin users


    public class HomeController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public HomeController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            ViewBag.Username = username;

            // Lấy dữ liệu từ SQL Server: Đếm số sản phẩm theo danh mục
            var categoryData = _context.ProductCategories
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count()
                })
                .ToList();
            ViewBag.CategoryData = categoryData;

            // Lấy số lượng sản phẩm, danh mục, và người dùng từ SQL Server
            int productCount = _context.Products.Count();
            int categoryCount = _context.ProductCategories.Count();
            int userCount = _context.Users.Count();

            // Truyền dữ liệu vào View qua ViewBag
            ViewBag.UserCount = userCount;
            ViewBag.ProductCount = productCount;
            ViewBag.CategoryCount = categoryCount;

            // Lấy top 10 sản phẩm đắt nhất
            var topExpensiveProducts = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Price)
                .Take(10)
                .Select(p => new
                {
                    ProductName = p.ProductName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                    Price = p.Price
                })
                .ToListAsync();

            ViewBag.TopExpensiveProducts = topExpensiveProducts;

            return View();
        }

        //[HttpPost("auth/logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync("AuthCookie");
        //    return RedirectToAction("Index", "Home", new { area = "" }); // Điều hướng về trang login
        //}

    }
}
