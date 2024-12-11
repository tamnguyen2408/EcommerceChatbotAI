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
using Microsoft.AspNetCore.Authorization;

namespace EcommerceChatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ECommerceAiDbContext _context;
        private readonly ILogger<ChatbotController> _logger;
        private readonly string _baseUrl;

        public ChatbotController(ECommerceAiDbContext context, ILogger<ChatbotController> logger)
        {
            _context = context;
            _logger = logger;
            _baseUrl = "https://5b01-118-70-118-224.ngrok-free.app"; // Ensure this URL is active and correct
        }

        private string GetProductImageUrl(string imageName)
        {
            // Ensure imageName does not start with a '/'
            if (string.IsNullOrWhiteSpace(imageName))
                return $"{_baseUrl}/images/products/default-image.jpg"; // Fallback image

            return $"{_baseUrl}{imageName}"; // Use the imageName directly
        }


        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] DialogflowRequest dialogflowRequest)
        {
            try
            {
                _logger.LogInformation("Received webhook request: {Request}", dialogflowRequest.ToString());

                if (dialogflowRequest?.QueryResult == null)
                    return BadRequest("Invalid request: dialogflowRequest is required.");

                var intentName = dialogflowRequest.QueryResult.Intent?.DisplayName;
                var parameters = dialogflowRequest.QueryResult.Parameters;
                object responsePayload;

                switch (intentName)
                {
                    case "SuggestProduct":
                        var products = await GetProductSuggestions();
                        responsePayload = FormatProductListWithImages(products);
                        break;

                    case "FilterProductByGender":
                        var gender = parameters["gender"]?.ToString();
                        var filteredProductsByGender = await GetProductsByGender(gender);
                        responsePayload = filteredProductsByGender;
                        break;

                    case "SearchProductByCategoryAndGender":
                        var genderParam = parameters["gender"]?.ToString();
                        var categoryParam = parameters["category"]?.ToString();
                        var filteredProductsByCategoryAndGender = await GetProductsByCategoryAndGender(categoryParam, genderParam);
                        responsePayload = filteredProductsByCategoryAndGender;
                        break;

                    case "FilterProductByCategory":
                        var category = parameters["category"]?.ToString();
                        var filteredProducts = await GetProductsByCategory(category);
                        responsePayload = filteredProducts;
                        break;

                    case "GetProductDetails":
                        var detailProductName = parameters["productName"]?.ToString();
                        responsePayload = await GetProductDetails(detailProductName);
                        break;

                    case "CheckStock":
                        var stockProductName = parameters["productName"]?.ToString();
                        responsePayload = await CheckStock(stockProductName);
                        break;

                    case "SearchProduct":
                        var searchProductName = parameters["productName"]?.ToString();
                        responsePayload = await SearchProduct(searchProductName);
                        break;
                   
                  

                    default:
                        responsePayload = new { fulfillmentText = "Xin lỗi, tôi không hiểu điều đó." };
                        break;
                }

                return Ok(responsePayload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while processing the webhook.");
            }
        }

        private async Task<List<Product>> GetProductSuggestions()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        private object FormatProductListWithImages(List<Product> products)
        {
            if (products == null || !products.Any())
                return new { fulfillmentText = "Hiện tại chưa có sản phẩm nào có sẵn." };

            var groupedProducts = products.GroupBy(p => p.Category?.CategoryName)
                                          .Select(g => $"**{g.Key ?? "Uncategorized"}**\n" +
                                                       string.Join("\n", g.Select(p =>
                                                       {
                                                           string productImageUrl = GetProductImageUrl(p.ImageUrl); // Get image URL
                                                           return $"🌟 **{p.ProductName}**\n" +
                                                                  $"💰 **Giá**: {p.Price:C}\n" +
                                                                  $"📅 **Mô tả**: {p.Description ?? "Không có mô tả"}\n" +
                                                                  $"📂 **Danh mục**: {p.Category?.CategoryName ?? "Không có"}\n" +
                                                                  $"🔗 **Xem chi tiết**: [Tại đây]({_baseUrl}/home/details/{p.ProductId})\n" +
                                                                  $"------------------------------------------------------------";
                                                       })));

            var productListText = string.Join("\n\n", groupedProducts);

            return new { fulfillmentText = productListText };
        }



        private object FormatCategoryList(List<ProductCategory> categories)
        {
            if (categories == null || !categories.Any())
                return new { fulfillmentText = "Hiện tại, không có danh mục nào có sẵn." };

            return new { fulfillmentText = string.Join("\n", categories.Select(c => $"- {c.CategoryName}")) };
        }




        private async Task<object> GetProductDetails(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để cung cấp thông tin chi tiết. Vui lòng cung cấp tên sản phẩm!" };

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Sản phẩm **'{productName}'** không tìm thấy. Vui lòng kiểm tra lại tên sản phẩm." };

            var productDetailsText = $"🔍 **Chi tiết sản phẩm**: **{product.ProductName}**\n" +
                                     $"💼 **Danh mục**: {product.Category?.CategoryName ?? "Không có"}\n" +
                                     $"💰 **Giá**: {product.Price:C}\n" +
                                     $"📝 **Mô tả**: {product.Description ?? "Không có mô tả"}\n\n";

            return new { fulfillmentText = productDetailsText };
        }

        private async Task<object> CheckStock(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để kiểm tra hàng tồn kho. Vui lòng cung cấp tên sản phẩm!" };

            var product = await _context.Products
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Sản phẩm **'{productName}'** không tìm thấy." };

            return new { fulfillmentText = $"**{product.ProductName}** hiện có **{product.StockQuantity}** sản phẩm trong kho." };
        }

        private async Task<object> SearchProduct(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để tìm kiếm. Vui lòng cung cấp tên sản phẩm!" };

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Sản phẩm **'{productName}'** không tìm thấy." };

            var productText = $"🔎 **Tìm thấy sản phẩm**: **{product.ProductName}**\n" +
                              $"💰 **Giá**: {product.Price:C}\n" +
                              $"🔗 **Xem chi tiết**: [Tại đây]({_baseUrl}/home/details/{product.ProductId})";

            return new { fulfillmentText = productText };
        }

        private async Task<object> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new { fulfillmentText = "Không có danh mục nào được cung cấp." };

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => EF.Functions.Like(p.Category.CategoryName, $"%{category}%"))
                .ToListAsync();

            if (products.Any())
            {
                var responseText = $"🛍️ **Sản phẩm trong danh mục '{category}'**:\n";
                foreach (var product in products)
                {
                    string productImageUrl = GetProductImageUrl(product.ImageUrl);
                    responseText += $"\n🌟 **{product.ProductName}** - {product.Price:C}\n" +
                                    $"📂 **Danh mục**: {product.Category?.CategoryName ?? "Không có"}\n" +
                                    $"📝 **Mô tả**: {product.Description ?? "Không có mô tả"}\n" +
                                    $"🔗 **Xem chi tiết**: [Tại đây]({_baseUrl}/products/{product.ProductId})\n";
                }
                return new { fulfillmentText = responseText };
            }

            return new { fulfillmentText = $"Không có sản phẩm nào trong danh mục **{category}**." };
        }

        private async Task<object> GetProductsByGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                return new { fulfillmentText = "Không có giới tính nào được cung cấp." };

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => EF.Functions.Like(p.Gender, $"%{gender}%"))
                .ToListAsync();

            if (products.Any())
            {
                var responseText = $"🛒 **Sản phẩm cho {gender}**:\n";
                foreach (var product in products)
                {
                    string productImageUrl = GetProductImageUrl(product.ImageUrl);
                    responseText += $"\n🌟 **{product.ProductName}** - {product.Price:C}\n" +
                                    $"📂 **Danh mục**: {product.Category?.CategoryName ?? "Không có"}\n" +
                                    $"📝 **Mô tả**: {product.Description ?? "Không có mô tả"}\n" +
                                    $"🔗 **Xem chi tiết**: [Tại đây]({_baseUrl}/products/{product.ProductId})\n";
                }
                return new { fulfillmentText = responseText };
            }

            return new { fulfillmentText = $"Không có sản phẩm nào cho giới tính **{gender}**." };
        }

        private async Task<object> GetProductsByCategoryAndGender(string category, string gender)
        {
            if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(gender))
                return new { fulfillmentText = "Vui lòng cung cấp danh mục và giới tính." };

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => EF.Functions.Like(p.Category.CategoryName, $"%{category}%") &&
                            EF.Functions.Like(p.Gender, $"%{gender}%"))
                .ToListAsync();

            if (products.Any())
            {
                var responseText = $"🛒 **Sản phẩm cho {gender} trong danh mục '{category}'**:\n";
                foreach (var product in products)
                {
                    string productImageUrl = GetProductImageUrl(product.ImageUrl);
                    responseText += $"🌟 **{product.ProductName}** - {product.Price:C}\n" +
                                    $"📂 **Danh mục**: {product.Category?.CategoryName ?? "Không có"}\n" +
                                    $"📝 **Mô tả**: {product.Description ?? "Không có mô tả"}\n\n" +
                                    $"🔗 **Xem chi tiết**: [Tại đây]({_baseUrl}/products/{product.ProductId})\n";
                }

                return new { fulfillmentText = responseText };
            }

            return new { fulfillmentText = $"Không có sản phẩm nào cho {gender} trong danh mục **{category}**." };
        }








    }
}