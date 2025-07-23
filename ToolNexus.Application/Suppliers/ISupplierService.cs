using ToolNexus.Application.Suppliers.DTOs;

namespace ToolNexus.Application.Suppliers
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllSuppliersAsync();
        Task<List<SupplierDto>> GetActiveSuppliersAsync();
        Task<SupplierDto?> GetSupplierByIdAsync(int id);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createDto);
        Task<SupplierDto> UpdateSupplierAsync(UpdateSupplierDto updateDto);
        Task<bool> DeleteSupplierAsync(int id);
        Task<bool> CanDeleteSupplierAsync(int id);
    }
}