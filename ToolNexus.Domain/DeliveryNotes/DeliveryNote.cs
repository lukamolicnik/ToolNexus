namespace ToolNexus.Domain.DeliveryNotes
{
    using ToolNexus.Domain.Suppliers;

    public class DeliveryNote
    {
        public int Id { get; set; }
        public required string DeliveryNoteNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<DeliveryNoteItem> DeliveryNoteItems { get; set; } = new List<DeliveryNoteItem>();

        // Computed property
        public decimal TotalAmount => DeliveryNoteItems?.Sum(x => x.TotalPrice) ?? 0;
    }
}