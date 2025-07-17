using ToolNexus.WebUI.Server.Components;
using ToolNexus.WebUI.Server.Services;
using ToolNexus.Application;
using ToolNexus.Infrastructure;
using ToolNexus.Application.Users;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controllers for API endpoints
builder.Services.AddControllers();

// Configure SignalR for better circuit handling
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = null;
    options.EnableDetailedErrors = true;
});

// Add MudBlazor services
builder.Services.AddMudServices();

// Add QR Code Service
builder.Services.AddScoped<IQRCodeService, QRCodeService>();

// Add application and infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add memory cache for user sessions
builder.Services.AddMemoryCache();

// Add Cookie Authentication Services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API controllers
app.MapControllers();

// Login redirect endpoint
app.MapPost("/login-redirect", async (HttpContext context) =>
{
    return Results.Redirect("/");
});

// Logout endpoint
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    
    // Clear all cookies
    foreach (var cookie in context.Request.Cookies.Keys)
    {
        context.Response.Cookies.Delete(cookie);
    }
    
    return Results.Redirect("/login");
});

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    await ToolNexus.Infrastructure.DataSeeder.SeedDataAsync(scope.ServiceProvider);
}

app.Run();