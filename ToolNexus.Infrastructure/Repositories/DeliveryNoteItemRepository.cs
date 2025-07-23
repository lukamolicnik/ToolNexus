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
    }
}