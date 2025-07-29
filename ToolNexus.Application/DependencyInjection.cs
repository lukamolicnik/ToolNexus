using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Audit;
using ToolNexus.Application.Tools;
using ToolNexus.Application.Users;
using ToolNexus.Application.Suppliers;
using ToolNexus.Application.DeliveryNotes;
using ToolNexus.Application.ToolCategories;
using ToolNexus.Application.StockAdjustments;

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

            // Supplier services
            services.AddScoped<ISupplierService, SupplierService>();

            // Delivery Note services
            services.AddScoped<IDeliveryNoteService, DeliveryNoteService>();

            // Stock Adjustment services
            services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

            return services;
        }
    }
}