using ToolNexus.Application.Suppliers.DTOs;
using ToolNexus.Domain.Suppliers;
using ToolNexus.Domain.DeliveryNotes;

namespace ToolNexus.Application.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IDeliveryNoteRepository _deliveryNoteRepository;

        public SupplierService(
            ISupplierRepository supplierRepository,
            IDeliveryNoteRepository deliveryNoteRepository)
        {
            _supplierRepository = supplierRepository;
            _deliveryNoteRepository = deliveryNoteRepository;
        }

        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(MapToDto).ToList();
        }

        public async Task<List<SupplierDto>> GetActiveSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetActiveAsync();
            return suppliers.Select(MapToDto).ToList();
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier == null ? null : MapToDto(supplier);
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createDto)
        {
            var supplier = new Supplier
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Address = createDto.Address,
                IsActive = createDto.IsActive
            };

            var created = await _supplierRepository.AddAsync(supplier);
            return MapToDto(created);
        }

        public async Task<SupplierDto> UpdateSupplierAsync(UpdateSupplierDto updateDto)
        {
            var supplier = await _supplierRepository.GetByIdAsync(updateDto.Id);
            if (supplier == null)
                throw new InvalidOperationException($"Dobavitelj z ID {updateDto.Id} ne obstaja.");

            supplier.Name = updateDto.Name;
            supplier.Email = updateDto.Email;
            supplier.Phone = updateDto.Phone;
            supplier.Address = updateDto.Address;
            supplier.IsActive = updateDto.IsActive;

            await _supplierRepository.UpdateAsync(supplier);
            return MapToDto(supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return false;

            var hasDeliveryNotes = await _deliveryNoteRepository.GetBySupplierIdAsync(id);
            if (hasDeliveryNotes.Any())
                throw new InvalidOperationException("Dobavitelja ni mogoče izbrisati, ker ima povezane dobavnice.");

            await _supplierRepository.DeleteAsync(supplier);
            return true;
        }

        public async Task<bool> CanDeleteSupplierAsync(int id)
        {
            var deliveryNotes = await _deliveryNoteRepository.GetBySupplierIdAsync(id);
            return !deliveryNotes.Any();
        }

        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                CreatedBy = supplier.CreatedBy?.ToString(),
                CreatedByName = supplier.CreatedByUser != null ? $"{supplier.CreatedByUser.FirstName} {supplier.CreatedByUser.LastName}" : null,
                UpdatedAt = supplier.UpdatedAt,
                UpdatedBy = supplier.UpdatedBy?.ToString(),
                UpdatedByName = supplier.UpdatedByUser != null ? $"{supplier.UpdatedByUser.FirstName} {supplier.UpdatedByUser.LastName}" : null
            };
        }
    }
}