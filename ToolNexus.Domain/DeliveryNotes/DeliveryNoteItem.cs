namespace ToolNexus.Domain.DeliveryNotes
{
    using ToolNexus.Domain.Tools;

    public class DeliveryNoteItem
    {
        public int Id { get; set; }
        public int DeliveryNoteId { get; set; }
        public int ToolId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        // Navigation properties
        public virtual DeliveryNote DeliveryNote { get; set; } = null!;
        public virtual Tool Tool { get; set; } = null!;
    }
}