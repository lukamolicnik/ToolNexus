using System.Globalization;
using ToolNexus.Application.Reports.DTOs;
using ToolNexus.Domain.DeliveryNotes;
using ToolNexus.Domain.StockAdjustments;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Reports
{
    public interface IReportService
    {
        Task<MonthlyLossReportDto> GetMonthlyLossReportAsync(MonthlyLossReportRequestDto request);
        Task<LossCostByCategoryReportDto> GetLossCostByCategoryReportAsync(LossCostByCategoryReportRequestDto request);
    }
}