namespace ToolNexus.Application.Audit.DTOs;

public class PagedAuditTrailDto
{
    public IEnumerable<AuditTrailDto> Items { get; set; } = new List<AuditTrailDto>();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public class AuditTrailPageRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
}