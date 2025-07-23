namespace ToolNexus.Application.DeliveryNotes.DTOs
{
    public class CreateDeliveryNoteDto
    {
        public string DeliveryNoteNumber { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public string? Notes { get; set; }
        public List<CreateDeliveryNoteItemDto> Items { get; set; } = new();
    }

    public class CreateDeliveryNoteItemDto
    {
        public int ToolId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}