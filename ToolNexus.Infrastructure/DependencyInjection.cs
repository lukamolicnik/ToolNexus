using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Tools;
using ToolNexus.Domain.Audit;
using ToolNexus.Domain.ToolCategories;
using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Users;
using ToolNexus.Domain.Suppliers;
using ToolNexus.Domain.DeliveryNotes;
using ToolNexus.Infrastructure.Interceptors;
using ToolNexus.Infrastructure.Repositories;

namespace ToolNexus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register HttpContextAccessor for AuditInterceptor
            services.AddHttpContextAccessor();

            // Use DbContextFactory for thread-safe DbContext creation with audit interceptor
            services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                
                // Add audit interceptor to factory as well
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var auditInterceptor = new AuditInterceptor(httpContextAccessor);
                options.AddInterceptors(auditInterceptor);
            });

            // Register DbContext with interceptor for scoped usage
            services.AddScoped<ApplicationDbContext>(serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                
                // Add audit interceptor
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var auditInterceptor = new AuditInterceptor(httpContextAccessor);
                optionsBuilder.AddInterceptors(auditInterceptor);
                
                return new ApplicationDbContext(optionsBuilder.Options);
            });

            // Repository registrations
            services.AddScoped<IToolRepository, ToolRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IToolCategoryRepository, ToolCategoryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IDeliveryNoteRepository, DeliveryNoteRepository>();
            services.AddScoped<IDeliveryNoteItemRepository, DeliveryNoteItemRepository>();

            return services;
        }
    }
}