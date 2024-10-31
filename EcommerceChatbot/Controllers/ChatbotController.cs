using Microsoft.AspNetCore.Mvc;
using EcommerceChatbot.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EcommerceChatbot.Models.DTOs;

namespace EcommerceChatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ECommerceAiDbContext _context;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(ECommerceAiDbContext context, ILogger<ChatbotController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] DialogflowRequest dialogflowRequest)
        {
            try
            {
                _logger.LogInformation("Received webhook request: {Request}", dialogflowRequest.ToString());

                if (dialogflowRequest?.QueryResult == null)
                    return BadRequest("Yêu cầu không hợp lệ: dialogflowRequest là bắt buộc.");

                var intentName = dialogflowRequest.QueryResult.Intent?.DisplayName;
                var parameters = dialogflowRequest.QueryResult.Parameters;
                string responseText;

                switch (intentName)
                {
                    case "SuggestProduct":
                        var products = await GetProductSuggestions();
                        responseText = FormatProductList(products);
                        break;

                    case "GetCategories":
                        var categories = await GetProductCategories();
                        responseText = FormatCategoryList(categories);
                        break;

                    case "FilterProductByCategory":
                        var category = parameters["category"]?.ToString();
                        var filteredProducts = await GetProductsByCategory(category);
                        responseText = FormatProductList(filteredProducts);
                        break;

                    case "GetProductDetails":
                        var detailProductName = parameters["productName"]?.ToString();
                        responseText = await GetProductDetails(detailProductName);
                        break;

                    case "CheckStock":
                        var stockProductName = parameters["productName"]?.ToString();
                        responseText = await CheckStock(stockProductName);
                        break;

                    case "SearchProduct":
                        var searchProductName = parameters["productName"]?.ToString();
                        responseText = await SearchProduct(searchProductName);
                        break;

                    default:
                        responseText = "Xin lỗi, tôi chưa hiểu ý bạn.";
                        break;
                }

                return Ok(new { fulfillmentText = responseText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook: {Message}", ex.Message);
                return StatusCode(500, "Lỗi trong quá trình xử lý webhook.");
            }
        }


        // Retrieve and format product suggestions
        private async Task<List<Product>> GetProductSuggestions()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        private string FormatProductList(List<Product> products)
        {
            if (products == null || !products.Any())
                return "Hiện tại không có sản phẩm nào.";

            return string.Join("\n", products.Select(p =>
                $"- {p.ProductName} (Loại: {p.Category?.CategoryName}, Giá: {p.Price})"));
        }

        // Retrieve and format product categories
        private async Task<List<ProductCategory>> GetProductCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        private string FormatCategoryList(List<ProductCategory> categories)
        {
            if (categories == null || !categories.Any())
                return "Hiện tại không có thể loại nào.";

            return string.Join("\n", categories.Select(c => $"- {c.CategoryName}"));
        }

        // Retrieve products by category
        private async Task<List<Product>> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Product>();

            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName == category)
                .ToListAsync();
        }

        // Retrieve product details by product name
        private async Task<string> GetProductDetails(string productName)
        {
            _logger.LogInformation("Getting details for product: {ProductName}", productName);

            if (string.IsNullOrWhiteSpace(productName))
                return "Xin lỗi, tôi cần biết tên sản phẩm để cung cấp thông tin chi tiết.";

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return $"Không tìm thấy sản phẩm '{productName}'.";

            return $"Sản phẩm {product.ProductName} có giá {product.Price} và mô tả: {product.Description}.";
        }

        // Check stock availability for a given product
        private async Task<string> CheckStock(string productName)
        {
            _logger.LogInformation("Checking stock for product: {ProductName}", productName);

            if (string.IsNullOrWhiteSpace(productName))
                return "Xin lỗi, tôi cần biết tên sản phẩm để kiểm tra tồn kho.";

            var product = await _context.Products
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return $"Không tìm thấy sản phẩm '{productName}'.";

            return $"Sản phẩm {product.ProductName} hiện còn {product.StockQuantity} trong kho.";
        }

        // Search for a product by name
        private async Task<string> SearchProduct(string productName)
        {
            _logger.LogInformation("Searching for product: {ProductName}", productName);

            if (string.IsNullOrWhiteSpace(productName))
                return "Xin lỗi, tôi cần biết tên sản phẩm để tìm kiếm.";

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return $"Không tìm thấy sản phẩm '{productName}'.";

            return $"Chúng tôi có sản phẩm {product.ProductName} với giá {product.Price}.";
        }
    }
}
