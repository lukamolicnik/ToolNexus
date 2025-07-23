namespace ToolNexus.Domain.DeliveryNotes
{
    public interface IDeliveryNoteRepository
    {
        Task<DeliveryNote?> GetByIdAsync(int id);
        Task<DeliveryNote?> GetByIdWithItemsAsync(int id);
        Task<List<DeliveryNote>> GetAllAsync();
        Task<List<DeliveryNote>> GetAllWithDetailsAsync();
        Task<DeliveryNote> AddAsync(DeliveryNote deliveryNote);
        Task UpdateAsync(DeliveryNote deliveryNote);
        Task DeleteAsync(DeliveryNote deliveryNote);
        Task<bool> ExistsAsync(int id);
        Task<bool> DeliveryNoteNumberExistsAsync(string deliveryNoteNumber);
        Task<List<DeliveryNote>> GetBySupplierIdAsync(int supplierId);
        Task<List<DeliveryNote>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}