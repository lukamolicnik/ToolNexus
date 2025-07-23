using ToolNexus.Application.DeliveryNotes.DTOs;
using ToolNexus.Domain.DeliveryNotes;
using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Suppliers;

namespace ToolNexus.Application.DeliveryNotes
{
    public class DeliveryNoteService : IDeliveryNoteService
    {
        private readonly IDeliveryNoteRepository _deliveryNoteRepository;
        private readonly IDeliveryNoteItemRepository _deliveryNoteItemRepository;
        private readonly IToolRepository _toolRepository;
        private readonly ISupplierRepository _supplierRepository;

        public DeliveryNoteService(
            IDeliveryNoteRepository deliveryNoteRepository,
            IDeliveryNoteItemRepository deliveryNoteItemRepository,
            IToolRepository toolRepository,
            ISupplierRepository supplierRepository)
        {
            _deliveryNoteRepository = deliveryNoteRepository;
            _deliveryNoteItemRepository = deliveryNoteItemRepository;
            _toolRepository = toolRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<List<DeliveryNoteDto>> GetAllDeliveryNotesAsync()
        {
            var deliveryNotes = await _deliveryNoteRepository.GetAllAsync();
            return deliveryNotes.Select(MapToDto).ToList();
        }

        public async Task<DeliveryNoteDto?> GetDeliveryNoteByIdAsync(int id)
        {
            var deliveryNote = await _deliveryNoteRepository.GetByIdWithItemsAsync(id);
            return deliveryNote == null ? null : MapToDtoWithItems(deliveryNote);
        }

        public async Task<List<DeliveryNoteDto>> GetDeliveryNotesBySupplierAsync(int supplierId)
        {
            var deliveryNotes = await _deliveryNoteRepository.GetBySupplierIdAsync(supplierId);
            return deliveryNotes.Select(MapToDto).ToList();
        }

        public async Task<List<DeliveryNoteDto>> GetDeliveryNotesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var deliveryNotes = await _deliveryNoteRepository.GetByDateRangeAsync(startDate, endDate);
            return deliveryNotes.Select(MapToDto).ToList();
        }

        public async Task<DeliveryNoteDto> CreateDeliveryNoteAsync(CreateDeliveryNoteDto createDto)
        {
            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(createDto.SupplierId);
            if (supplier == null)
                throw new InvalidOperationException($"Dobavitelj z ID {createDto.SupplierId} ne obstaja.");

            // Validate delivery note number is unique
            if (await _deliveryNoteRepository.DeliveryNoteNumberExistsAsync(createDto.DeliveryNoteNumber))
                throw new InvalidOperationException($"Dobavnica s številko {createDto.DeliveryNoteNumber} že obstaja.");

            // Create delivery note
            var deliveryNote = new DeliveryNote
            {
                DeliveryNoteNumber = createDto.DeliveryNoteNumber,
                DeliveryDate = createDto.DeliveryDate,
                SupplierId = createDto.SupplierId,
                Notes = createDto.Notes
            };

            // Add items and update tool stock
            foreach (var itemDto in createDto.Items)
            {
                var tool = await _toolRepository.GetByIdAsync(itemDto.ToolId);
                if (tool == null)
                    throw new InvalidOperationException($"Orodje z ID {itemDto.ToolId} ne obstaja.");

                var item = new DeliveryNoteItem
                {
                    ToolId = itemDto.ToolId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                };

                deliveryNote.DeliveryNoteItems.Add(item);

                // Update tool stock
                tool.CurrentStock += itemDto.Quantity;
                await _toolRepository.UpdateAsync(tool);
            }

            var created = await _deliveryNoteRepository.AddAsync(deliveryNote);
            return await GetDeliveryNoteByIdAsync(created.Id) ?? throw new InvalidOperationException("Napaka pri ustvarjanju dobavnice.");
        }

        public async Task<bool> DeleteDeliveryNoteAsync(int id)
        {
            var deliveryNote = await _deliveryNoteRepository.GetByIdWithItemsAsync(id);
            if (deliveryNote == null)
                return false;

            // Reverse stock changes
            foreach (var item in deliveryNote.DeliveryNoteItems)
            {
                var tool = await _toolRepository.GetByIdAsync(item.ToolId);
                if (tool != null)
                {
                    tool.CurrentStock -= item.Quantity;
                    if (tool.CurrentStock < 0) tool.CurrentStock = 0;
                    await _toolRepository.UpdateAsync(tool);
                }
            }

            await _deliveryNoteRepository.DeleteAsync(deliveryNote);
            return true;
        }

        public async Task<bool> DeliveryNoteNumberExistsAsync(string deliveryNoteNumber)
        {
            return await _deliveryNoteRepository.DeliveryNoteNumberExistsAsync(deliveryNoteNumber);
        }

        private static DeliveryNoteDto MapToDto(DeliveryNote deliveryNote)
        {
            return new DeliveryNoteDto
            {
                Id = deliveryNote.Id,
                DeliveryNoteNumber = deliveryNote.DeliveryNoteNumber,
                DeliveryDate = deliveryNote.DeliveryDate,
                SupplierId = deliveryNote.SupplierId,
                SupplierName = deliveryNote.Supplier?.Name ?? string.Empty,
                Notes = deliveryNote.Notes,
                TotalAmount = deliveryNote.TotalAmount,
                CreatedAt = deliveryNote.CreatedAt,
                CreatedBy = deliveryNote.CreatedBy
            };
        }

        private static DeliveryNoteDto MapToDtoWithItems(DeliveryNote deliveryNote)
        {
            var dto = MapToDto(deliveryNote);
            dto.Items = deliveryNote.DeliveryNoteItems.Select(item => new DeliveryNoteItemDto
            {
                Id = item.Id,
                ToolId = item.ToolId,
                ToolCode = item.Tool?.Code ?? string.Empty,
                ToolName = item.Tool?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList();
            return dto;
        }
    }
}