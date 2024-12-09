using EcommerceChatbot.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using ECommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceChatbot.Areas.Admin.Service;
using Microsoft.Extensions.Logging;
using EcommerceChatbot.Models;
using Google.Api;
using System.Globalization;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
// Configure Stripe
var stripeSettings = builder.Configuration.GetSection("Stripe");
StripeConfiguration.ApiKey = stripeSettings["SecretKey"];

// Add services to the container.
builder.Services.Configure<StripeSettings>(stripeSettings); // Register Stripe settings
builder.Services.AddScoped<StripePaymentService>();

// Configure Kestrel to use HTTPS on localhost:5167.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5167, listenOptions =>
    {
        listenOptions.UseHttps(); // Enable HTTPS on port 5167.
    });

    // Optionally, listen on port 80 and redirect HTTP to HTTPS.
    options.ListenLocalhost(80);
});

// Add logging services.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ShoppingCart>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Đăng ký IHttpContextAccessor

// Configure Session (important for saving ShoppingCart state)
builder.Services.AddDistributedMemoryCache(); // Use in-memory cache for sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure DbContext to use SQL Server.
builder.Services.AddDbContext<ECommerceAiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EcommerceDb")));

// Register scoped services.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddAuthentication()
    .AddCookie("UserCookie", options =>
    {
        options.Cookie.Name = "EcommerceAuth_User";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
    })
    .AddCookie("AdminCookie", options =>
    {
        options.Cookie.Name = "EcommerceAuth_Admin";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
    });

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});


// Configure CORS to allow specific origins, including ngrok URL.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://3db5-118-70-118-224.ngrok-free.app") // Replace with your ngrok URL.
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure middleware based on environment.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enable HSTS for production.
}

// Force HTTPS redirection.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigins"); // Apply CORS policy

// Use Session middleware
app.UseSession(); // Bật sử dụng Session

app.UseRouting();

// Ensure authentication runs before authorization.
app.UseAuthentication();
app.UseAuthorization();

// Log every request.
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Received {context.Request.Method} request for {context.Request.Path}");
    await next.Invoke();
    logger.LogInformation($"Response {context.Response.StatusCode} for {context.Request.Path}");
});

// Configure routing for controllers.
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Run the application.
app.Run();