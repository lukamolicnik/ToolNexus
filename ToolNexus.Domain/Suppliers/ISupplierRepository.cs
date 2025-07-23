namespace ToolNexus.Domain.Suppliers
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(int id);
        Task<List<Supplier>> GetAllAsync();
        Task<List<Supplier>> GetActiveAsync();
        Task<Supplier> AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Supplier supplier);
        Task<bool> ExistsAsync(int id);
        Task<Supplier?> GetByNameAsync(string name);
    }
}