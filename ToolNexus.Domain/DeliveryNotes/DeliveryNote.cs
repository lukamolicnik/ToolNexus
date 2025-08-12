namespace ToolNexus.Domain.DeliveryNotes
{
    using ToolNexus.Domain.Suppliers;
    using ToolNexus.Domain.Users;

    public class DeliveryNote
    {
        public int Id { get; set; }
        public required string DeliveryNoteNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public string? Notes { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<DeliveryNoteItem> DeliveryNoteItems { get; set; } = new List<DeliveryNoteItem>();
        public virtual User? CreatedByUser { get; set; }
        public virtual User? UpdatedByUser { get; set; }

        // Computed property
        public decimal TotalAmount => DeliveryNoteItems?.Sum(x => x.TotalPrice) ?? 0;
    }
}