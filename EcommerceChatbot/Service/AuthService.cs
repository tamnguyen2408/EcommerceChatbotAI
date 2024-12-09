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
        public async Task<User> GetUserByUsernameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
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
                Password = password, // No hashing applied (you should add hashing in production)
                Phone = phone,
                Role = "user", // Default role
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                // Add user to context and save changes
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return "An error occurred while registering the user.";
            }

            return "User registered successfully!";
        }

        public async Task<string> LoginAsync(string userName, string password, string role)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);

            if (user == null)
                return "Invalid username or password.";

            if (user.Role != role)
                return "Unauthorized role.";

            // Tạo Claims cho người dùng, bao gồm UserId
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("UserId", user.UserId.ToString()) // Thêm UserId vào Claims
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Xác định cookie scheme dựa trên vai trò
            string cookieScheme = role == "admin" ? "AdminCookie" : "UserCookie";

            // Đăng nhập người dùng với scheme phù hợp
            await _httpContextAccessor.HttpContext.SignInAsync(cookieScheme, principal);

            return "Login successful!";
        }

        // SignOut method
        public async Task<string> SignOutAsync()
        {
            // Đăng xuất người dùng
            await _httpContextAccessor.HttpContext.SignOutAsync("AuthCookie");
            return "Logout successful!";
        }
    }
}
