using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranchMembership> UserBranchMemberships { get; set; }
        public DbSet<UserBranchRole> UserBranchRoles { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<PickingListHeader> PickingListHeaders { get; set; }
        public DbSet<PickingListLine> PickingListLines { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<PickPackTask> PickPackTasks { get; set; }
        public DbSet<LoadPlan> LoadPlans { get; set; }
        public DbSet<ErpShipmentRef> ErpShipmentRefs { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unique Indexes
            builder.Entity<PickingListHeader>()
                .HasIndex(x => new { x.BranchId, x.PickingListNumber })
                .IsUnique();

            builder.Entity<PickingListLine>()
                .HasIndex(x => new { x.PickingListHeaderId, x.LineNumber })
                .IsUnique();

            builder.Entity<ErpShipmentRef>()
                .HasIndex(x => new { x.BranchId, x.ErpPackingListNumber })
                .IsUnique();

            builder.Entity<Tag>()
                .HasIndex(x => new { x.BranchId, x.TagNumber })
                .IsUnique();

            // Relationships
            builder.Entity<UserBranchMembership>()
                .HasOne(x => x.User)
                .WithMany(u => u.BranchMemberships)
                .HasForeignKey(x => x.UserId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified)
                {
                    var property = entry.Metadata.FindProperty("Version");
                    if (property != null && property.ClrType == typeof(long) && property.IsConcurrencyToken)
                    {
                        var currentVersion = (long?)entry.Property("Version").CurrentValue ?? 0;
                        entry.Property("Version").CurrentValue = currentVersion + 1;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
