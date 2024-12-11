using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ViewBag.Message = "Please enter a search term.";
                return View(new List<Product>());
            }

            // Chuẩn hóa từ khóa tìm kiếm (normalize the search term)
            searchTerm = searchTerm.Trim().ToLower();

            // Từ khóa tìm kiếm tiếng Việt và tiếng Anh cho giới tính
            var maleKeywords = new List<string> { "nam", "cho nam", "male" };
            var femaleKeywords = new List<string> { "nữ", "cho nữ", "female" };
            var unisexKeywords = new List<string> { "cả nam và nữ", "cho cả nam và nữ", "unisex" };

            // Danh sách các thể loại sản phẩm
            var categoryKeywords = new List<string> { "giày", "áo", "túi xách", "shoes", "clothes", "bags" };

            // Truy vấn ban đầu
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // Kiểm tra các từ khóa giới tính và thể loại
            string genderSearch = null;
            string categorySearch = null;

            // Kiểm tra giới tính
            if (maleKeywords.Any(k => searchTerm.Contains(k)))
            {
                genderSearch = "nam";
            }
            else if (femaleKeywords.Any(k => searchTerm.Contains(k)))
            {
                genderSearch = "nữ";
            }
            else if (unisexKeywords.Any(k => searchTerm.Contains(k)))
            {
                genderSearch = "cả nam và nữ";
            }

            // Kiểm tra thể loại sản phẩm
            foreach (var category in categoryKeywords)
            {
                if (searchTerm.Contains(category))
                {
                    categorySearch = category;
                    break;
                }
            }

            // Tìm kiếm theo tên chính xác
            var exactMatch = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.ToLower() == searchTerm)
                .ToListAsync();

            if (exactMatch.Any())
            {
                return View(exactMatch); // If exact match found, return these products
            }

            // Tìm kiếm theo giới tính và thể loại
            if (genderSearch != null && categorySearch != null)
            {
                // Tìm kiếm cả thể loại và giới tính
                productsQuery = productsQuery.Where(p =>
                    p.Gender != null && p.Gender.ToLower() == genderSearch &&
                    p.ProductName.ToLower().Contains(categorySearch.ToLower()));
            }
            else if (genderSearch != null)
            {
                // Tìm kiếm theo giới tính
                productsQuery = productsQuery.Where(p => p.Gender != null && p.Gender.ToLower() == genderSearch);
            }
            else if (categorySearch != null)
            {
                // Tìm kiếm theo thể loại
                productsQuery = productsQuery.Where(p => p.ProductName.ToLower().Contains(categorySearch.ToLower()));
            }
            else
            {
                // Nếu không có giới tính và thể loại, tìm kiếm theo tên sản phẩm và mô tả
                string pattern = $"%{searchTerm}%";
                productsQuery = productsQuery.Where(p =>
                    EF.Functions.Like(p.ProductName.ToLower(), pattern) ||
                    (p.Description != null && EF.Functions.Like(p.Description.ToLower(), pattern)) ||
                    (p.Category != null && EF.Functions.Like(p.Category.CategoryName.ToLower(), pattern))
                );
            }

            // Thực thi truy vấn
            var products = await productsQuery.OrderBy(p => p.ProductName).ToListAsync();

            if (!products.Any())
            {
                ViewBag.Message = $"No products found for keyword '{searchTerm}'.";
            }

            return View(products);
        }
    }
}
