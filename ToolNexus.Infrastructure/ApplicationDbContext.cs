using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.Audit;
using ToolNexus.Domain.ToolCategories;
using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Users;
using ToolNexus.Domain.Suppliers;
using ToolNexus.Domain.DeliveryNotes;
using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tool> Tools { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<ToolCategory> ToolCategories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<DeliveryNote> DeliveryNotes { get; set; }
        public DbSet<DeliveryNoteItem> DeliveryNoteItems { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UserRoleId)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                // Unique constraints
                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasOne(e => e.UserRole)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                // Unique constraint za Name
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            // Tool entity configuration (če potrebuješ dodatne konfiguracije)
            modelBuilder.Entity<Tool>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.CurrentStock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.MinimumStock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.CriticalStock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);
                
                entity.HasOne(e => e.ToolCategory)
                    .WithMany(tc => tc.Tools)
                    .HasForeignKey(e => e.ToolCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Unique constraint za Code
                entity.HasIndex(e => e.Code)
                    .IsUnique();
            });

            // AuditTrail entity configuration
            modelBuilder.Entity<AuditTrail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EntityType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.EntityId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.Property(e => e.Changes)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Index za hitrejše iskanje
                entity.HasIndex(e => e.EntityType);
                entity.HasIndex(e => e.EntityId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Timestamp);
            });

            // ToolCategory entity configuration
            modelBuilder.Entity<ToolCategory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Unique constraint za Code
                entity.HasIndex(e => e.Code)
                    .IsUnique();
            });

            // Supplier entity configuration
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasMaxLength(200);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50);

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Unique constraint
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            // DeliveryNote entity configuration
            modelBuilder.Entity<DeliveryNote>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DeliveryNoteNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DeliveryDate)
                    .IsRequired();

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100);

                // Unique constraint
                entity.HasIndex(e => e.DeliveryNoteNumber)
                    .IsUnique();

                // Relationships
                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.DeliveryNotes)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DeliveryNoteItem entity configuration
            modelBuilder.Entity<DeliveryNoteItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                // Relationships
                entity.HasOne(e => e.DeliveryNote)
                    .WithMany(d => d.DeliveryNoteItems)
                    .HasForeignKey(e => e.DeliveryNoteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tool)
                    .WithMany()
                    .HasForeignKey(e => e.ToolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockAdjustment entity configuration
            modelBuilder.Entity<StockAdjustment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ToolId)
                    .IsRequired();

                entity.Property(e => e.AdjustmentType)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.PreviousStock)
                    .IsRequired();

                entity.Property(e => e.NewStock)
                    .IsRequired();

                entity.Property(e => e.Reason)
                    .HasMaxLength(200);

                entity.Property(e => e.Notes)
                    .HasMaxLength(1000);

                entity.Property(e => e.AdjustedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AdjustedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(e => e.Tool)
                    .WithMany()
                    .HasForeignKey(e => e.ToolId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index for faster queries
                entity.HasIndex(e => e.ToolId);
                entity.HasIndex(e => e.AdjustedAt);
            });
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}