using CMetalsFulfillment.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchSettings> BranchSettings { get; set; }
        public DbSet<UserBranchMembership> UserBranchMemberships { get; set; }
        public DbSet<UserBranchClaim> UserBranchClaims { get; set; }

        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineOperatorAssignment> MachineOperatorAssignments { get; set; }
        public DbSet<PickPackStation> PickPackStations { get; set; }
        public DbSet<PickPackStationAssignment> PickPackStationAssignments { get; set; }
        public DbSet<ShiftTemplate> ShiftTemplates { get; set; }

        public DbSet<ShippingRegion> ShippingRegions { get; set; }
        public DbSet<ShippingGroup> ShippingGroups { get; set; }
        public DbSet<ShippingFsaRule> ShippingFsaRules { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
        public DbSet<InventorySnapshotLine> InventorySnapshotLines { get; set; }
        public DbSet<InventoryStock> InventoryStocks { get; set; }

        public DbSet<PickingList> PickingLists { get; set; }
        public DbSet<PickingListLine> PickingListLines { get; set; }
        public DbSet<PickingListEvent> PickingListEvents { get; set; }

        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderInputCoil> WorkOrderInputCoils { get; set; }
        public DbSet<WorkOrderOutputLine> WorkOrderOutputLines { get; set; }
        public DbSet<WorkOrderProductionRecord> WorkOrderProductionRecords { get; set; }
        public DbSet<WorkOrderEvent> WorkOrderEvents { get; set; }
        public DbSet<LogisticsTag> LogisticsTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision
            builder.Entity<InventorySnapshotLine>()
                .Property(e => e.SnapshotValue)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<InventorySnapshotLine>()
                .Property(e => e.CountValue)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<InventorySnapshotLine>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<InventoryStock>()
                .Property(e => e.QuantityOnHand)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<InventoryStock>()
                .Property(e => e.WeightOnHand)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<Item>()
                .Property(e => e.PoundsPerSquareFoot)
                .HasColumnType("decimal(18, 4)");

            builder.Entity<PickingListLine>()
                .Property(e => e.QtyOrdered)
                .HasColumnType("decimal(18, 4)");

            // Fix SQL Server Multiple Cascade Paths
            builder.Entity<ShippingGroup>()
                .HasOne(g => g.Branch)
                .WithMany()
                .HasForeignKey(g => g.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingFsaRule>()
                .HasOne(r => r.Branch)
                .WithMany()
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingFsaRule>()
                .HasOne(r => r.Region)
                .WithMany()
                .HasForeignKey(r => r.ShippingRegionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingFsaRule>()
                .HasOne(r => r.Group)
                .WithMany()
                .HasForeignKey(r => r.ShippingGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Work Order Constraints
            builder.Entity<WorkOrder>()
                .HasOne(w => w.Branch)
                .WithMany()
                .HasForeignKey(w => w.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkOrderOutputLine>()
                .HasOne(l => l.PickingListLine)
                .WithMany()
                .HasForeignKey(l => l.PickingListLineId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<LogisticsTag>()
                .HasOne(t => t.Branch)
                .WithMany()
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkOrderOutputLine>()
                .Property(e => e.PlannedQty).HasColumnType("decimal(18, 4)");
            builder.Entity<WorkOrderOutputLine>()
                .Property(e => e.ProducedQty).HasColumnType("decimal(18, 4)");
            builder.Entity<WorkOrderInputCoil>()
                .Property(e => e.WeightLbs).HasColumnType("decimal(18, 4)");
            builder.Entity<WorkOrderProductionRecord>()
                .Property(e => e.ProducedWeightLbs).HasColumnType("decimal(18, 4)");
            builder.Entity<LogisticsTag>()
                .Property(e => e.WeightLbs).HasColumnType("decimal(18, 4)");
        }
    }
}
