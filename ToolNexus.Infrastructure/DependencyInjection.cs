using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Tools;
using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Users;
using ToolNexus.Infrastructure.Repositories;

namespace ToolNexus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Use DbContextFactory for thread-safe DbContext creation
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository registrations
            services.AddScoped<IToolRepository, ToolRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}