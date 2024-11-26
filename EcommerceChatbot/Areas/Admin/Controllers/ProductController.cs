using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ECommerceAiDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ECommerceAiDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Product/Index
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Admin/Product/Add
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = await _context.ProductCategories.ToListAsync(); // Populate categories
            ViewBag.Genders = new List<string> { "nam", "nữ", "cả nam và nữ" }; // Thêm danh sách gender
            return View();
        }


        // POST: Admin/Product/Add
        // POST: Admin/Product/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product, IFormFile? productImage)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (productImage != null && productImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(productImage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImage.CopyToAsync(stream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName; // Save the path correctly
                }

                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _context.ProductCategories.ToListAsync(); // Repopulate categories on failure
            ViewBag.Genders = new List<string> { "nam", "nữ", "cả nam và nữ" }; // Repopulate genders
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.ProductCategories.ToListAsync(); // Populate categories
            ViewBag.Genders = new List<string> { "nam", "nữ", "cả nam và nữ" }; // Thêm danh sách gender
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? productImage)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload if a new image is provided
                    if (productImage != null && productImage.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                        Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(productImage.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productImage.CopyToAsync(stream);
                        }

                        // Set the new image URL
                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }
                    else
                    {
                        // Keep the existing image URL if no new image is uploaded
                        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                        product.ImageUrl = existingProduct?.ImageUrl;
                    }

                    product.UpdatedAt = DateTime.Now; // Update timestamp
                    _context.Update(product); // Update the product
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    throw; // Re-throw the exception
                }
            }

            ViewBag.Categories = await _context.ProductCategories.ToListAsync(); // Repopulate categories on failure
            return View(product);
        }


        // Check if a product exists
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        // POST: Admin/Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
