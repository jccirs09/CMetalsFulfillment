using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Data.Interfaces;
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

            // Branch
            builder.Entity<Branch>()
                .HasIndex(b => b.Code)
                .IsUnique();

            // UserBranchMembership
            builder.Entity<UserBranchMembership>()
                .HasIndex(m => new { m.UserId, m.BranchId })
                .IsUnique();

            // UserBranchRole
            builder.Entity<UserBranchRole>()
                .HasIndex(r => new { r.UserId, r.BranchId, r.RoleName })
                .IsUnique();

            // PickingListHeader
            builder.Entity<PickingListHeader>()
                .HasIndex(h => new { h.BranchId, h.PickingListNumber })
                .IsUnique();

            // PickingListLine
            builder.Entity<PickingListLine>()
                .HasIndex(l => new { l.PickingListHeaderId, l.LineNumber })
                .IsUnique();

            // ErpShipmentRef
            builder.Entity<ErpShipmentRef>()
                .HasIndex(e => new { e.BranchId, e.ErpPackingListNumber })
                .IsUnique();

            // Tag
            builder.Entity<Tag>()
                .HasIndex(t => new { t.BranchId, t.TagNumber })
                .IsUnique();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IConcurrencyAware>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Version++;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
