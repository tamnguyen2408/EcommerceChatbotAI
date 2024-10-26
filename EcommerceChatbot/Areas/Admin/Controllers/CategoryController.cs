using Microsoft.AspNetCore.Mvc;
using EcommerceChatbot.Areas.Admin.Models;
using EcommerceChatbot.Areas.Admin.Service;
using EcommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ECommerceAiDbContext _context; // Injected DbContext

        public CategoryController(CategoryService categoryService, ECommerceAiDbContext context)
        {
            _categoryService = categoryService;
            _context = context; // Initialize the DbContext
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            // Map DTO to Entity
            var category = new ProductCategory
            {
                CategoryName = categoryDto.CategoryName
                // Add other properties as necessary
            };

            _context.ProductCategories.Add(category); // Use the injected _context
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the category from the database using the ID
            var category = await _context.ProductCategories.FindAsync(id);

            if (category == null)
            {
                return NotFound(); // Return a 404 if the category is not found
            }

            return View(category); // Return the view with the model
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory category)
        {
            if (id != category.CategoryId) // Ensure the IDs match
            {
                return NotFound(); // Return a 404 if the IDs do not match
            }

            if (!ModelState.IsValid)
            {
                return View(category); // Return the view with the model if the model state is not valid
            }

            // Retrieve the existing category from the database
            var existingCategory = await _context.ProductCategories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound(); // Return a 404 if the category is not found
            }

            // Update the properties
            existingCategory.CategoryName = category.CategoryName;
            // Update other properties as necessary

            _context.ProductCategories.Update(existingCategory); // Mark the entity as modified
            await _context.SaveChangesAsync(); // Save changes to the database

            return RedirectToAction(nameof(Index)); // Redirect to the index action after saving
        }
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra xem còn sản phẩm nào thuộc danh mục này không
            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                // Nếu còn sản phẩm, trả về thông báo không thể xóa
                TempData["Message"] = "Không thể xóa danh mục vì vẫn còn sản phẩm thuộc danh mục này.";
                return RedirectToAction("Index");
            }

            // Xóa danh mục nếu không còn sản phẩm nào
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Xóa danh mục thành công.";
            return RedirectToAction("Index");
        }


        // GET: Admin/Category/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
    }
}
