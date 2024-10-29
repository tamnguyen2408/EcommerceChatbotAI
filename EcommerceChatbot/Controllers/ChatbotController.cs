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
                // Log the incoming request
                _logger.LogInformation("Received a webhook request: {Request}", dialogflowRequest.ToString());

                // Check for null dialogflowRequest
                if (dialogflowRequest == null || dialogflowRequest.QueryResult == null)
                {
                    return BadRequest("Yêu cầu không hợp lệ: dialogflowRequest là bắt buộc.");
                }

                // Extract the intent name from the Dialogflow request
                var intentName = dialogflowRequest.QueryResult.Intent?.DisplayName;
                string responseText;

                // Switch case to handle different intents
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
                        var category = dialogflowRequest.QueryResult.Parameters["category"]?.ToString();
                        var filteredProducts = await GetProductsByCategory(category);
                        responseText = FormatProductList(filteredProducts);
                        break;

                    default:
                        responseText = "Xin lỗi, tôi chưa hiểu ý bạn.";
                        break;
                }

                // Return the response back to Dialogflow
                return Ok(new { fulfillmentText = responseText });
            }
            catch (Exception ex)
            {
                // Log any errors that occur during processing
                _logger.LogError(ex, "Error processing webhook: {Message}", ex.Message);
                return StatusCode(500, "Lỗi trong quá trình xử lý webhook.");
            }
        }


        private async Task<List<Product>> GetProductSuggestions()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        private string FormatProductList(List<Product> products)
        {
            if (products == null || !products.Any())
                return "Hiện tại không có sản phẩm nào.";

            return string.Join("\n", products.Select(p =>
                $"- {p.ProductName} (Category: {p.Category?.CategoryName}, Price: {p.Price})"
            ));
        }

        private async Task<List<ProductCategory>> GetProductCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        private string FormatCategoryList(List<ProductCategory> categories)
        {
            if (categories == null || !categories.Any())
                return "Hiện tại không có thể loại nào.";

            return string.Join("\n", categories.Select(c =>
                $"- {c.CategoryName}"
            ));
        }


        private async Task<List<Product>> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Product>();

            // Fetch products that match the given category name (case-sensitive)
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.CategoryName == category)
                .ToListAsync();
        }


    }
}

