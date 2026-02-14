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
        public DbSet<AuditEvent> AuditEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Branch
            builder.Entity<Branch>()
                .OwnsOne(b => b.Settings);

            // UserBranchMembership
            builder.Entity<UserBranchMembership>()
                .HasKey(m => new { m.UserId, m.BranchId });

            builder.Entity<UserBranchMembership>()
                .HasOne(m => m.User)
                .WithMany(u => u.BranchMemberships)
                .HasForeignKey(m => m.UserId);

            builder.Entity<UserBranchMembership>()
                .HasOne(m => m.Branch)
                .WithMany(b => b.Memberships)
                .HasForeignKey(m => m.BranchId);

            // UserBranchRole
            builder.Entity<UserBranchRole>()
                .HasOne(r => r.Membership)
                .WithMany(m => m.Roles)
                .HasForeignKey(r => new { r.UserId, r.BranchId });
        }
    }
}
