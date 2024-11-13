using EcommerceChatbot.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using ECommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceChatbot.Areas.Admin.Service;
using Microsoft.Extensions.Logging;
using EcommerceChatbot.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Configure DbContext to use SQL Server.
builder.Services.AddDbContext<ECommerceAiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EcommerceDb")));

// Register scoped services.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();

// Configure Cookie Authentication.
builder.Services.AddAuthentication("AuthCookie")
    .AddCookie("AuthCookie", options =>
    {
        options.Cookie.Name = "EcommerceAuth";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS.
        options.Cookie.HttpOnly = true; // Prevent JavaScript access to cookies.
        options.Cookie.SameSite = SameSiteMode.Strict; // Stronger protection against CSRF.
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true; // Extend expiration time on activity
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
    });

// Configure Authorization policies.
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

// Run the application.
app.Run();
