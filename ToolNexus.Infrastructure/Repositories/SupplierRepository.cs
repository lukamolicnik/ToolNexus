using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.Suppliers;

namespace ToolNexus.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SupplierRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Suppliers.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<Supplier>> GetActiveAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Suppliers.Update(supplier);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Supplier supplier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Suppliers.AnyAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Suppliers.FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}