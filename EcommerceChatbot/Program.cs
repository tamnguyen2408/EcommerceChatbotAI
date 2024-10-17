using EcommerceChatbot.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using ECommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceChatbot.Models;
using EcommerceChatbot.Areas.Admin.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ECommerceAiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
// Add authentication services
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
        name: "user",
        pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "shop",
    pattern: "Shop/{action=Index}/{id?}",
    defaults: new { controller = "Shop" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
