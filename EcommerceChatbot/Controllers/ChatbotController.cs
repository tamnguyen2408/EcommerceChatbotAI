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
        private readonly string _baseUrl;

        public ChatbotController(ECommerceAiDbContext context, ILogger<ChatbotController> logger)
        {
            _context = context;
            _logger = logger;
            _baseUrl = "https://3db5-118-70-118-224.ngrok-free.app"; // Ensure this URL is active and correct
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

                    case "GetCategories":
                        var categories = await GetProductCategories();
                        responsePayload = FormatCategoryList(categories);
                        break;

                    case "FilterProductByCategory":
                        var category = parameters["category"]?.ToString();
                        var filteredProducts = await GetProductsByCategory(category);
                        responsePayload = FormatProductListWithImages(filteredProducts);
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
                        responsePayload = new { fulfillmentText = "Sorry, I didn't understand that." };
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
                return new { fulfillmentText = "Currently, no products are available." };

            var cards = products.Select(p => new
            {
                card = new
                {
                    title = p.ProductName,
                    subtitle = $"Category: {p.Category?.CategoryName}, Price: {p.Price}",
                    imageUri = GetProductImageUrl(p.ImageUrl), // Use the correct URL for the image
                    buttons = new[]
                    {
                new { text = "View Product", postback = $"{_baseUrl}/home/details/{p.ProductId}" }
            }
                }
            }).ToArray();

            return new { fulfillmentMessages = cards }; // Adjusted to match Dialogflow's expected structure
        }

        private async Task<List<ProductCategory>> GetProductCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        private object FormatCategoryList(List<ProductCategory> categories)
        {
            if (categories == null || !categories.Any())
                return new { fulfillmentText = "Currently, no categories are available." };

            return new { fulfillmentText = string.Join("\n", categories.Select(c => $"- {c.CategoryName}")) };
        }

        private async Task<List<Product>> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Product>();

            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName == category)
                .ToListAsync();
        }

        private async Task<object> GetProductDetails(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để cung cấp thông tin chi tiết." };

            // Using the product name for search
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Product '{productName}' not found." };

            // If the product is found, return detailed information
            return new
            {
                fulfillmentMessages = new[]
                {
            new
            {
                card = new
                {
                    title = product.ProductName,
                    subtitle = $"Category: {product.Category?.CategoryName}, Price: {product.Price}\nDescription: {product.Description}",
                    imageUri = GetProductImageUrl(product.ImageUrl),  // Generate full URL for image
                    buttons = new[]
                    {
                        new { text = "View Product", postback = $"{_baseUrl}/products/{product.ProductId}" } // Ensure this URL is valid
                    }
                }
            }
        }
            };
        }

        private async Task<object> CheckStock(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để kiểm tra hàng tồn kho." };

            var product = await _context.Products
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Product '{productName}' not found." };

            return new { fulfillmentText = $"{product.ProductName} có {product.StockQuantity} sản phẩm trong kho." };
        }

        private async Task<object> SearchProduct(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new { fulfillmentText = "Tôi cần tên sản phẩm để tìm kiếm." };

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));

            if (product == null)
                return new { fulfillmentText = $"Product '{productName}' not found." };

            return new
            {
                fulfillmentMessages = new[]
                {
                    new
                    {
                        card = new
                        {
                            title = product.ProductName,
                            subtitle = $"Price: {product.Price}",
                            imageUri = GetProductImageUrl(product.ImageUrl), // Generate full URL for image
                            buttons = new[]
                            {
                                new { text = "View Product", postback = $"{_baseUrl}/home/details/{product.ProductId}" }
                            }
                        }
                    }
                }
            };
        }
    }
}