using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.DeliveryNotes;

namespace ToolNexus.Infrastructure.Repositories
{
    public class DeliveryNoteItemRepository : IDeliveryNoteItemRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DeliveryNoteItemRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DeliveryNoteItem?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(i => i.Tool)
                .Include(i => i.DeliveryNote)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<DeliveryNoteItem>> GetByDeliveryNoteIdAsync(int deliveryNoteId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(i => i.Tool)
                    .ThenInclude(t => t.ToolCategory)
                .Include(i => i.DeliveryNote)
                    .ThenInclude(d => d.Supplier)
                .Where(i => i.DeliveryNoteId == deliveryNoteId)
                .ToListAsync();
        }

        public async Task<DeliveryNoteItem> AddAsync(DeliveryNoteItem item)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNoteItems.Add(item);
            await context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(DeliveryNoteItem item)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNoteItems.Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeliveryNoteItem item)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.DeliveryNoteItems.Remove(item);
            await context.SaveChangesAsync();
        }

        public async Task<List<DeliveryNoteItem>> GetByToolIdAsync(int toolId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(i => i.DeliveryNote)
                    .ThenInclude(d => d.Supplier)
                .Where(i => i.ToolId == toolId)
                .OrderByDescending(i => i.DeliveryNote.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNoteItem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(dni => dni.DeliveryNote)
                    .ThenInclude(dn => dn.Supplier)
                .Include(dni => dni.Tool)
                    .ThenInclude(t => t.ToolCategory)
                .Where(dni => dni.DeliveryNote.DeliveryDate >= startDate &&
                            dni.DeliveryNote.DeliveryDate <= endDate)
                .OrderBy(dni => dni.DeliveryNote.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNoteItem>> GetByToolIdWithDeliveryNotesAsync(int toolId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(dni => dni.DeliveryNote)
                    .ThenInclude(dn => dn.Supplier)
                .Where(dni => dni.ToolId == toolId)
                .OrderByDescending(dni => dni.DeliveryNote.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNoteItem>> GetByCategoryAndDateRangeAsync(int categoryId, DateTime startDate, DateTime endDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(dni => dni.DeliveryNote)
                    .ThenInclude(dn => dn.Supplier)
                .Include(dni => dni.Tool)
                    .ThenInclude(t => t.ToolCategory)
                .Where(dni => dni.Tool.ToolCategoryId == categoryId &&
                            dni.DeliveryNote.DeliveryDate >= startDate &&
                            dni.DeliveryNote.DeliveryDate <= endDate)
                .OrderBy(dni => dni.DeliveryNote.DeliveryDate)
                .ToListAsync();
        }

        public async Task<List<DeliveryNoteItem>> GetBySupplierAndDateRangeAsync(int supplierId, DateTime startDate, DateTime endDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.DeliveryNoteItems
                .Include(dni => dni.DeliveryNote)
                    .ThenInclude(dn => dn.Supplier)
                .Include(dni => dni.Tool)
                    .ThenInclude(t => t.ToolCategory)
                .Where(dni => dni.DeliveryNote.SupplierId == supplierId &&
                            dni.DeliveryNote.DeliveryDate >= startDate &&
                            dni.DeliveryNote.DeliveryDate <= endDate)
                .OrderBy(dni => dni.DeliveryNote.DeliveryDate)
                .ToListAsync();
        }
    }
}