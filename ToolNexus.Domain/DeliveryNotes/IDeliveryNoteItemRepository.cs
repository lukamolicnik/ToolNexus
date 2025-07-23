namespace ToolNexus.Domain.DeliveryNotes
{
    public interface IDeliveryNoteItemRepository
    {
        Task<DeliveryNoteItem?> GetByIdAsync(int id);
        Task<List<DeliveryNoteItem>> GetByDeliveryNoteIdAsync(int deliveryNoteId);
        Task<DeliveryNoteItem> AddAsync(DeliveryNoteItem item);
        Task UpdateAsync(DeliveryNoteItem item);
        Task DeleteAsync(DeliveryNoteItem item);
        Task<List<DeliveryNoteItem>> GetByToolIdAsync(int toolId);
    }
}