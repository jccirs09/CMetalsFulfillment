using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranchMembership> BranchMemberships { get; set; }
        public DbSet<UserBranchRole> BranchRoles { get; set; }

        public DbSet<PickingListHeader> PickingListHeaders { get; set; }
        public DbSet<PickingListLine> PickingListLines { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<LoadPlan> LoadPlans { get; set; }
        public DbSet<LoadPlanReservation> LoadPlanReservations { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<PickPackTask> PickPackTasks { get; set; }
        public DbSet<ErpShipmentRef> ErpShipmentRefs { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IConcurrencyAware>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Version++;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.Version = 1;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Branch>()
                .Property(b => b.TimezoneId)
                .HasDefaultValue("UTC");

            builder.Entity<UserBranchMembership>()
                .HasIndex(m => new { m.UserId, m.BranchId })
                .IsUnique();

            builder.Entity<UserBranchRole>()
                .HasIndex(r => new { r.UserId, r.BranchId, r.RoleName })
                .IsUnique();

            builder.Entity<PickingListHeader>()
                .HasIndex(p => new { p.BranchId, p.PickingListNumber })
                .IsUnique();
            builder.Entity<PickingListHeader>()
                .HasIndex(p => p.ShipDate);

            builder.Entity<PickingListLine>()
                .HasIndex(l => new { l.PickingListHeaderId, l.LineNumber })
                .IsUnique();

            builder.Entity<Tag>()
                .HasIndex(t => new { t.BranchId, t.TagNumber })
                .IsUnique();
            builder.Entity<Tag>()
                .HasIndex(t => t.TagNumber);

            builder.Entity<WorkOrder>()
                .HasIndex(w => new { w.MachineId, w.PlannedStartUtc });

            builder.Entity<LoadPlan>()
                .HasIndex(l => l.PlannedDepartUtc);

            builder.Entity<LoadPlanReservation>()
                .HasIndex(l => new { l.LoadPlanId, l.PickingListHeaderId })
                .IsUnique();

            builder.Entity<ErpShipmentRef>()
                .HasIndex(e => new { e.BranchId, e.ErpPackingListNumber })
                .IsUnique();
            builder.Entity<ErpShipmentRef>()
                .HasIndex(e => e.ErpPackingListNumber);
        }
    }
}
