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
using ToolNexus.Domain.StockAdjustments;
using ToolNexus.Infrastructure.Interceptors;
using ToolNexus.Infrastructure.Repositories;
using ToolNexus.Infrastructure.Services;

namespace ToolNexus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register HttpContextAccessor for AuditInterceptor
            services.AddHttpContextAccessor();

            // Register UserContextService
            services.AddScoped<IUserContextService, UserContextService>();

            // Register AuditInterceptor as scoped to maintain HttpContext
            services.AddScoped<AuditInterceptor>();

            // Use DbContextFactory for thread-safe DbContext creation with audit interceptor
            services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                
                // Always create a new audit interceptor with the current service provider
                // This ensures we get the correct HttpContext
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var userContextService = new UserContextService(httpContextAccessor);
                var auditInterceptor = new AuditInterceptor(userContextService);
                options.AddInterceptors(auditInterceptor);
            });

            // Register DbContext with interceptor for scoped usage
            services.AddScoped<ApplicationDbContext>(serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                
                // Get the scoped audit interceptor
                var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
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
            services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();

            return services;
        }
    }
}