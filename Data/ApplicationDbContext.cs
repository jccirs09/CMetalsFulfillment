using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranchMembership> BranchMemberships { get; set; }
        public DbSet<UserBranchRole> BranchRoles { get; set; }

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
        }
    }
}
