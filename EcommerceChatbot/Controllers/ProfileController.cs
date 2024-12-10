using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    [Authorize(AuthenticationSchemes = "UserCookie", Roles = "user")]
    public class ProfileController : Controller
    {
        private readonly ECommerceAiDbContext _context;

        public ProfileController(ECommerceAiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

                if (user != null)
                {
                    return View(user);
                }
            }

            return RedirectToAction("Login", "Auth");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Lưu chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            Console.WriteLine("Edit action started.");

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid.");

                var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (existingUser != null)
                {
                    Console.WriteLine("User found in database.");

                    existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Password = user.Password;
                    existingUser.UpdatedAt = DateTime.Now;

                    try
                    {
                        _context.SaveChanges();
                        Console.WriteLine("User updated successfully.");
                        return RedirectToAction("Index", "Profile");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SaveChanges failed: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid.");
            }

            return View(user);
        }


    }
}