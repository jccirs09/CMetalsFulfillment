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

            // Branch: Code (unique)
            builder.Entity<Branch>()
                .HasIndex(b => b.Code)
                .IsUnique();

            // UserBranchMembership: Unique(UserId, BranchId)
            builder.Entity<UserBranchMembership>()
                .HasIndex(m => new { m.UserId, m.BranchId })
                .IsUnique();

            // UserBranchRole: Unique(UserId, BranchId, RoleName)
            builder.Entity<UserBranchRole>()
                .HasIndex(r => new { r.UserId, r.BranchId, r.RoleName })
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
