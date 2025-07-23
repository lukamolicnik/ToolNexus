namespace ToolNexus.Application.DeliveryNotes.DTOs
{
    public class DeliveryNoteDto
    {
        public int Id { get; set; }
        public string DeliveryNoteNumber { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public List<DeliveryNoteItemDto> Items { get; set; } = new();
    }
}