using EcommerceChatbot.Areas.Admin.Models;
using EcommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Areas.Admin.Service
{
    public class CategoryService
    {
        private readonly ECommerceAiDbContext _context;

        public CategoryService(ECommerceAiDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            var newCategory = new ProductCategory
            {
                CategoryName = categoryDto.CategoryName
            };

            await _context.ProductCategories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }
    }
}
