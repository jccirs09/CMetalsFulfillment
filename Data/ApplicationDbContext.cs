using CMetalsFulfillment.Domain.Entities;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Branch>().HasKey(b => b.Id);
            builder.Entity<Branch>().Property(b => b.Name).IsRequired();

            builder.Entity<UserBranchMembership>()
                .HasOne(ubm => ubm.User)
                .WithMany(u => u.BranchMemberships)
                .HasForeignKey(ubm => ubm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserBranchMembership>()
                .HasOne(ubm => ubm.Branch)
                .WithMany()
                .HasForeignKey(ubm => ubm.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserBranchRole>()
                .HasOne(ubr => ubr.User)
                .WithMany(u => u.BranchRoles)
                .HasForeignKey(ubr => ubr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserBranchRole>()
                .HasOne(ubr => ubr.Branch)
                .WithMany()
                .HasForeignKey(ubr => ubr.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
