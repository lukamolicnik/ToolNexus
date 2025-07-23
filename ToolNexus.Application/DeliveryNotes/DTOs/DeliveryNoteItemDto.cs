namespace ToolNexus.Application.DeliveryNotes.DTOs
{
    public class DeliveryNoteItemDto
    {
        public int Id { get; set; }
        public int ToolId { get; set; }
        public string ToolCode { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}