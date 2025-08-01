// ToolNexus.Application/Reports/ReportService.cs
using System.Globalization;
using ToolNexus.Application.Reports.DTOs;
using ToolNexus.Domain.DeliveryNotes;
using ToolNexus.Domain.StockAdjustments;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Reports
{
    public class ReportService : IReportService
    {
        private readonly IStockAdjustmentRepository _stockAdjustmentRepository;
        private readonly IDeliveryNoteRepository _deliveryNoteRepository;
        private readonly IDeliveryNoteItemRepository _deliveryNoteItemRepository;
        private readonly IToolRepository _toolRepository;

        public ReportService(
            IStockAdjustmentRepository stockAdjustmentRepository,
            IDeliveryNoteRepository deliveryNoteRepository,
            IDeliveryNoteItemRepository deliveryNoteItemRepository,
            IToolRepository toolRepository)
        {
            _stockAdjustmentRepository = stockAdjustmentRepository;
            _deliveryNoteRepository = deliveryNoteRepository;
            _deliveryNoteItemRepository = deliveryNoteItemRepository;
            _toolRepository = toolRepository;
        }

        public async Task<MonthlyLossReportDto> GetMonthlyLossReportAsync(MonthlyLossReportRequestDto request)
        {
            var startDate = new DateTime(request.Year, request.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all decrease adjustments for the month
            var adjustments = await _stockAdjustmentRepository.GetAllAsync();
            var monthlyAdjustments = adjustments
                .Where(a => a.AdjustmentType == StockAdjustmentType.Decrease &&
                           a.AdjustedAt >= startDate &&
                           a.AdjustedAt <= endDate)
                .ToList();

            if (request.CategoryId.HasValue)
            {
                monthlyAdjustments = monthlyAdjustments
                    .Where(a => a.Tool?.ToolCategoryId == request.CategoryId.Value)
                    .ToList();
            }

            // Group by tool
            var toolGroups = monthlyAdjustments
                .GroupBy(a => new { a.ToolId, ToolCode = a.Tool?.Code, ToolName = a.Tool?.Name, CategoryName = a.Tool?.ToolCategory?.Name })
                .ToList();

            var lossItems = new List<LossItemDto>();

            foreach (var toolGroup in toolGroups)
            {
                var toolAdjustments = toolGroup.ToList();

                var lossItem = new LossItemDto
                {
                    ToolId = toolGroup.Key.ToolId,
                    ToolCode = toolGroup.Key.ToolCode ?? "",
                    ToolName = toolGroup.Key.ToolName ?? "",
                    CategoryName = toolGroup.Key.CategoryName ?? "-",
                    LostQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("izgub") == true).Sum(a => a.Quantity),
                    DamagedQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("poškod") == true).Sum(a => a.Quantity),
                    DiscardedQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("odpis") == true).Sum(a => a.Quantity),
                };

                lossItem.TotalQuantity = lossItem.LostQuantity + lossItem.DamagedQuantity + lossItem.DiscardedQuantity;

                // Estimate value based on recent purchases
                lossItem.EstimatedValue = await EstimateToolValueAsync(toolGroup.Key.ToolId, lossItem.TotalQuantity);

                if (request.IncludeDetails)
                {
                    lossItem.Incidents = toolAdjustments.Select(a => new LossIncidentDto
                    {
                        Date = a.AdjustedAt,
                        Reason = a.Reason ?? "",
                        Quantity = a.Quantity,
                        AdjustedBy = a.AdjustedBy,
                        Notes = a.Notes
                    }).OrderBy(i => i.Date).ToList();
                }

                lossItems.Add(lossItem);
            }

            var summary = new LossSummaryDto
            {
                TotalLostQuantity = lossItems.Sum(i => i.LostQuantity),
                TotalDamagedQuantity = lossItems.Sum(i => i.DamagedQuantity),
                TotalDiscardedQuantity = lossItems.Sum(i => i.DiscardedQuantity),
                TotalQuantity = lossItems.Sum(i => i.TotalQuantity),
                TotalEstimatedValue = lossItems.Sum(i => i.EstimatedValue),
                UniqueToolsAffected = lossItems.Count,
                TotalIncidents = lossItems.Sum(i => i.Incidents.Count)
            };

            return new MonthlyLossReportDto
            {
                Year = request.Year,
                Month = request.Month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(request.Month),
                LossItems = lossItems.OrderByDescending(i => i.EstimatedValue).ToList(),
                Summary = summary
            };
        }

        public async Task<LossCostByCategoryReportDto> GetLossCostByCategoryReportAsync(LossCostByCategoryReportRequestDto request)
        {
            // Get all decrease adjustments in the date range
            var adjustments = await _stockAdjustmentRepository.GetAllAsync();
            var filteredAdjustments = adjustments
                .Where(a => a.AdjustmentType == StockAdjustmentType.Decrease &&
                           a.AdjustedAt >= request.StartDate &&
                           a.AdjustedAt <= request.EndDate)
                .ToList();

            // Group by category
            var categoryGroups = filteredAdjustments
                .GroupBy(a => new {
                    CategoryId = a.Tool?.ToolCategoryId ?? 0,
                    CategoryName = a.Tool?.ToolCategory?.Name ?? "Nekategorizirano"
                })
                .ToList();

            var categories = new List<CategoryLossCostDto>();
            decimal totalValue = 0;

            foreach (var categoryGroup in categoryGroups)
            {
                var categoryAdjustments = categoryGroup.ToList();

                // Group by tool within category
                var toolGroups = categoryAdjustments
                    .GroupBy(a => new { a.ToolId, ToolCode = a.Tool?.Code, ToolName = a.Tool?.Name })
                    .ToList();

                var categoryTools = new List<CategoryToolLossDto>();
                decimal categoryValue = 0;

                foreach (var toolGroup in toolGroups)
                {
                    var toolAdjustments = toolGroup.ToList();

                    var toolLoss = new CategoryToolLossDto
                    {
                        ToolId = toolGroup.Key.ToolId,
                        ToolCode = toolGroup.Key.ToolCode ?? "",
                        ToolName = toolGroup.Key.ToolName ?? "",
                        LostQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("izgub") == true).Sum(a => a.Quantity),
                        DamagedQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("poškod") == true).Sum(a => a.Quantity),
                        DiscardedQuantity = toolAdjustments.Where(a => a.Reason?.ToLower().Contains("odpis") == true).Sum(a => a.Quantity),
                    };

                    toolLoss.TotalQuantity = toolLoss.LostQuantity + toolLoss.DamagedQuantity + toolLoss.DiscardedQuantity;
                    toolLoss.EstimatedValue = await EstimateToolValueAsync(toolGroup.Key.ToolId, toolLoss.TotalQuantity);

                    categoryTools.Add(toolLoss);
                    categoryValue += toolLoss.EstimatedValue;
                }

                var categoryLoss = new CategoryLossCostDto
                {
                    CategoryId = categoryGroup.Key.CategoryId,
                    CategoryName = categoryGroup.Key.CategoryName,
                    TotalLostQuantity = categoryTools.Sum(t => t.LostQuantity),
                    TotalDamagedQuantity = categoryTools.Sum(t => t.DamagedQuantity),
                    TotalDiscardedQuantity = categoryTools.Sum(t => t.DiscardedQuantity),
                    TotalQuantity = categoryTools.Sum(t => t.TotalQuantity),
                    EstimatedValue = categoryValue,
                    Tools = request.IncludeToolDetails ? categoryTools.OrderByDescending(t => t.EstimatedValue).ToList() : new()
                };

                if (request.MinimumLossValue == null || categoryLoss.EstimatedValue >= request.MinimumLossValue)
                {
                    categories.Add(categoryLoss);
                    totalValue += categoryValue;
                }
            }

            // Calculate percentages
            foreach (var category in categories)
            {
                category.PercentageOfTotalLoss = totalValue > 0 ? (category.EstimatedValue / totalValue) * 100 : 0;
            }

            var summary = new LossCostSummaryDto
            {
                TotalEstimatedValue = totalValue,
                TotalQuantityLost = categories.Sum(c => c.TotalQuantity),
                TotalCategories = categories.Count,
                TotalToolsAffected = categories.Sum(c => c.Tools.Count),
                MostAffectedCategory = categories.OrderByDescending(c => c.EstimatedValue).FirstOrDefault(),
                LeastAffectedCategory = categories.OrderBy(c => c.EstimatedValue).FirstOrDefault()
            };

            return new LossCostByCategoryReportDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Categories = categories.OrderByDescending(c => c.EstimatedValue).ToList(),
                Summary = summary
            };
        }

        private async Task<decimal> EstimateToolValueAsync(int toolId, int quantity)
        {
            // Get recent delivery note items for this tool to estimate current price
            var recentItems = await _deliveryNoteItemRepository.GetByToolIdAsync(toolId);

            if (!recentItems.Any())
                return 0;

            // Take average of last 3 orders or all if less than 3
            var recentPrices = recentItems
                .OrderByDescending(i => i.DeliveryNote?.DeliveryDate)
                .Take(3)
                .Select(i => i.UnitPrice)
                .ToList();

            var averagePrice = recentPrices.Any() ? recentPrices.Average() : 0;
            return averagePrice * quantity;
        }
    }
}