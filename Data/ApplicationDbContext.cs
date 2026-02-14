using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Phase 1
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

        // Phase 2
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineOperatorAssignment> MachineOperatorAssignments { get; set; }
        public DbSet<PickPackStation> PickPackStations { get; set; }
        public DbSet<ShiftTemplate> ShiftTemplates { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<ShippingRegion> ShippingRegions { get; set; }
        public DbSet<ShippingGroup> ShippingGroups { get; set; }
        public DbSet<ShippingFsaRule> ShippingFsaRules { get; set; }
        public DbSet<NonWorkingDay> NonWorkingDays { get; set; }
        public DbSet<NonWorkingDayOverride> NonWorkingDayOverrides { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Phase 1 - Unique Indexes
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

            // Phase 1 - Relationships
            builder.Entity<UserBranchMembership>()
                .HasOne(x => x.User)
                .WithMany(u => u.BranchMemberships)
                .HasForeignKey(x => x.UserId);

            // Phase 2 - Unique Constraints (Name per Branch)
            builder.Entity<Machine>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<PickPackStation>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<ShiftTemplate>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<Truck>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<ShippingRegion>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<ShippingGroup>()
                .HasIndex(x => new { x.BranchId, x.Name })
                .IsUnique();

            builder.Entity<ShippingFsaRule>()
                .HasIndex(x => new { x.BranchId, x.FsaPrefix })
                .IsUnique();

            builder.Entity<NonWorkingDay>()
                .HasIndex(x => new { x.BranchId, x.Date })
                .IsUnique();

            builder.Entity<NonWorkingDayOverride>()
                .HasIndex(x => new { x.BranchId, x.Date })
                .IsUnique();

            // Phase 2 - Relationships
            builder.Entity<ShippingFsaRule>()
                .HasOne(x => x.ShippingRegion)
                .WithMany()
                .HasForeignKey(x => x.ShippingRegionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingFsaRule>()
                .HasOne(x => x.ShippingGroup)
                .WithMany()
                .HasForeignKey(x => x.ShippingGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MachineOperatorAssignment>()
                .HasOne(x => x.Machine)
                .WithMany()
                .HasForeignKey(x => x.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MachineOperatorAssignment>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
