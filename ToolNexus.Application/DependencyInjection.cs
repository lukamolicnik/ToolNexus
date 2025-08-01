﻿using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Audit;
using ToolNexus.Application.DeliveryNotes;
using ToolNexus.Application.Reports;
using ToolNexus.Application.StockAdjustments;
using ToolNexus.Application.Suppliers;
using ToolNexus.Application.ToolCategories;
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
            services.AddScoped<ILoginAttemptService, LoginAttemptService>();

            // Audit services
            services.AddScoped<IAuditService, AuditService>();

            // Supplier services
            services.AddScoped<ISupplierService, SupplierService>();

            // Delivery Note services
            services.AddScoped<IDeliveryNoteService, DeliveryNoteService>();

            // Stock Adjustment services
            services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();

            // Report services
            services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}