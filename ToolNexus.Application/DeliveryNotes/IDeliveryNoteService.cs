using ToolNexus.Application.DeliveryNotes.DTOs;

namespace ToolNexus.Application.DeliveryNotes
{
    public interface IDeliveryNoteService
    {
        Task<List<DeliveryNoteDto>> GetAllDeliveryNotesAsync();
        Task<DeliveryNoteDto?> GetDeliveryNoteByIdAsync(int id);
        Task<List<DeliveryNoteDto>> GetDeliveryNotesBySupplierAsync(int supplierId);
        Task<List<DeliveryNoteDto>> GetDeliveryNotesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<DeliveryNoteDto> CreateDeliveryNoteAsync(CreateDeliveryNoteDto createDto);
        Task<bool> DeleteDeliveryNoteAsync(int id);
        Task<bool> DeliveryNoteNumberExistsAsync(string deliveryNoteNumber);
    }
}