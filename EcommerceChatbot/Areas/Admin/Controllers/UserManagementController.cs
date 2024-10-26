using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public UserManagementController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserManagement/Index
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

           

            return View(users);
        }


        // GET: Admin/UserManagement/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Admin/UserManagement/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = user.Role.ToLower();

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/UserManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Role = user.Role.ToLower();
                    user.UpdatedAt = DateTime.Now; // Update timestamp
                    _context.Update(user); // Update the user
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(user);
        }

        // Check if a user exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // POST: Admin/UserManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
