using EcommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace EcommerceChatbot.Service
{
    public class AuthService
    {
        private readonly ECommerceAiDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ECommerceAiDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Register method
        public async Task<string> RegisterAsync(string username, string email, string password, string confirmPassword, string phone)
        {
            if (password != confirmPassword)
                return "Passwords do not match.";

            var userExists = await _context.Users.AnyAsync(u => u.Email == email);
            if (userExists)
                return "User already exists with this email.";

            var newUser = new User
            {
                UserName = username,
                Email = email,
                Password = password, // No hashing applied
                Phone = phone,
                Role = "user", // Default role
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                // Add user to context and save changes
                await _context.Users.AddAsync(newUser); // Thay đổi từ Add sang AddAsync
                await _context.SaveChangesAsync();
            }
            catch
            {
                return "An error occurred while registering the user.";
            }

            return "User registered successfully!";
        }

        // Login method
        public async Task<string> LoginAsync(string userName, string password, string role)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

            if (user == null)
                return "Invalid username or password.";

            if (user.Role != role)
                return "Unauthorized role.";

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user
            await _httpContextAccessor.HttpContext.SignInAsync("AuthCookie", principal);

            return "Login successful!";
        }
    }
}
