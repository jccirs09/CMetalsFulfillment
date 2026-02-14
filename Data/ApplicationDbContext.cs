using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranchMembership> BranchMemberships { get; set; }
        public DbSet<UserBranchRole> BranchRoles { get; set; }

        // Phase 2 Entities
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

            builder.Entity<Branch>(b =>
            {
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.TimezoneId).IsRequired().HasDefaultValue("UTC");
            });

            builder.Entity<UserBranchMembership>(m =>
            {
                m.HasOne(x => x.User)
                 .WithMany(u => u.BranchMemberships)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                m.HasOne(x => x.Branch)
                 .WithMany()
                 .HasForeignKey(x => x.BranchId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserBranchRole>(r =>
            {
                r.HasOne(x => x.Membership)
                 .WithMany(m => m.Roles)
                 .HasForeignKey(x => x.MembershipId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Phase 2 Configurations
            builder.Entity<Machine>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<MachineOperatorAssignment>(e =>
            {
                e.HasOne(x => x.Machine).WithMany().HasForeignKey(x => x.MachineId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PickPackStation>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShiftTemplate>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Truck>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShippingRegion>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShippingGroup>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShippingFsaRule>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<NonWorkingDay>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<NonWorkingDayOverride>(e =>
            {
                e.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.ApprovedByUser).WithMany().HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
