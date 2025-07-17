using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Audit;
using ToolNexus.Application.Interfaces;
using ToolNexus.Application.Services;
using ToolNexus.Application.Tools;
using ToolNexus.Application.Users;

namespace ToolNexus.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Tool services
            services.AddScoped<IToolService, ToolService>();
            services.AddScoped<IToolCategoryService, ToolCategoryService>();

            // User services
            services.AddScoped<IUserService, UserService>();

            // Audit services
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }
    }
}