using System.ComponentModel.DataAnnotations;

namespace ToolNexus.Application.Tools.DTOs;

public class IncreaseStockDto
{
    [Required]
    public int ToolId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti vsaj 1")]
    public int Quantity { get; set; }

    public string? Notes { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
}