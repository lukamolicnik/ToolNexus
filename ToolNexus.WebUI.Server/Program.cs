using ToolNexus.WebUI.Server.Components;
using ToolNexus.Application;
using ToolNexus.Infrastructure;
using ToolNexus.Application.Users;
using ToolNexus.WebUI.Server.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add application and infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add memory cache for user sessions
builder.Services.AddMemoryCache();

// Register CurrentUserService as Scoped
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    await ToolNexus.Infrastructure.DataSeeder.SeedDataAsync(scope.ServiceProvider);
}

app.Run();