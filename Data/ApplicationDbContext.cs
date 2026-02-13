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
        public DbSet<PickPackStation> PickPackStations { get; set; }
        public DbSet<ShiftTemplate> ShiftTemplates { get; set; }
        public DbSet<ShippingRegion> ShippingRegions { get; set; }
        public DbSet<ShippingGroup> ShippingGroups { get; set; }
        public DbSet<ShippingFsaRule> ShippingFsaRules { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser Navigation Properties
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.BranchMemberships)
                .WithOne()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.BranchClaims)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Item unique index
            builder.Entity<Item>()
                .HasIndex(i => new { i.BranchId, i.ItemCode })
                .IsUnique();

            builder.Entity<Item>()
                .Property(i => i.PoundsPerSquareFoot)
                .HasPrecision(18, 4);
        }
    }
}
