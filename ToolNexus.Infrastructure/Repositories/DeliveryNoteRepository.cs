using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.DeliveryNotes;

namespace ToolNexus.Infrastructure.Repositories
{
    public class DeliveryNoteRepository : IDeliveryNoteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DeliveryNoteRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DeliveryNote?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DeliveryNote?> GetByIdWithItemsAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .Include(d => d.DeliveryNoteItems)
                    .ThenInclude(i => i.Tool)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DeliveryNote>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .Include(d => d.DeliveryNoteItems)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNote>> GetAllWithDetailsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .Include(d => d.DeliveryNoteItems)
                    .ThenInclude(i => i.Tool)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }

        public async Task<DeliveryNote> AddAsync(DeliveryNote deliveryNote)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNotes.Add(deliveryNote);
            await context.SaveChangesAsync();
            return deliveryNote;
        }

        public async Task UpdateAsync(DeliveryNote deliveryNote)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNotes.Update(deliveryNote);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeliveryNote deliveryNote)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNotes.Remove(deliveryNote);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes.AnyAsync(d => d.Id == id);
        }

        public async Task<bool> DeliveryNoteNumberExistsAsync(string deliveryNoteNumber)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes.AnyAsync(d => d.DeliveryNoteNumber == deliveryNoteNumber);
        }

        public async Task<List<DeliveryNote>> GetBySupplierIdAsync(int supplierId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .Where(d => d.SupplierId == supplierId)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNote>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNotes
                .Include(d => d.Supplier)
                .Where(d => d.DeliveryDate >= startDate && d.DeliveryDate <= endDate)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }
    }
}