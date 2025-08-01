// ToolNexus.Application/Reports/DTOs/MonthlyLossReportDto.cs
using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Application.Reports.DTOs
{
    public class MonthlyLossReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public List<LossItemDto> LossItems { get; set; } = new();
        public LossSummaryDto Summary { get; set; } = new();
    }

    public class LossItemDto
    {
        public int ToolId { get; set; }
        public string ToolCode { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int LostQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public int DiscardedQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public decimal EstimatedValue { get; set; }
        public List<LossIncidentDto> Incidents { get; set; } = new();
    }

    public class LossIncidentDto
    {
        public DateTime Date { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string AdjustedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class LossSummaryDto
    {
        public int TotalLostQuantity { get; set; }
        public int TotalDamagedQuantity { get; set; }
        public int TotalDiscardedQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalEstimatedValue { get; set; }
        public int UniqueToolsAffected { get; set; }
        public int TotalIncidents { get; set; }
    }
}

// ToolNexus.Application/Reports/DTOs/LossCostByCategoryReportDto.cs
namespace ToolNexus.Application.Reports.DTOs
{
    public class LossCostByCategoryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CategoryLossCostDto> Categories { get; set; } = new();
        public LossCostSummaryDto Summary { get; set; } = new();
    }

    public class CategoryLossCostDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalLostQuantity { get; set; }
        public int TotalDamagedQuantity { get; set; }
        public int TotalDiscardedQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal PercentageOfTotalLoss { get; set; }
        public List<CategoryToolLossDto> Tools { get; set; } = new();
    }

    public class CategoryToolLossDto
    {
        public int ToolId { get; set; }
        public string ToolCode { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public int LostQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public int DiscardedQuantity { get; set; }
        public int TotalQuantity { get; set; }
        public decimal EstimatedValue { get; set; }
    }

    public class LossCostSummaryDto
    {
        public decimal TotalEstimatedValue { get; set; }
        public int TotalQuantityLost { get; set; }
        public int TotalCategories { get; set; }
        public int TotalToolsAffected { get; set; }
        public CategoryLossCostDto? MostAffectedCategory { get; set; }
        public CategoryLossCostDto? LeastAffectedCategory { get; set; }
    }
}

// ToolNexus.Application/Reports/DTOs/ReportRequestDto.cs
namespace ToolNexus.Application.Reports.DTOs
{
    public class MonthlyLossReportRequestDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int? CategoryId { get; set; }
        public bool IncludeDetails { get; set; } = true;
    }

    public class LossCostByCategoryReportRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeToolDetails { get; set; } = true;
        public int? MinimumLossValue { get; set; } // Filter categories with loss value above this threshold
    }
}