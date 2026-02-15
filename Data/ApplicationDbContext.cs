using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<UserBranchMembership> UserBranchMemberships { get; set; } = null!;
        public DbSet<UserBranchRole> UserBranchRoles { get; set; } = null!;
        public DbSet<AuditEvent> AuditEvents { get; set; } = null!;

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

            // At most one IsDefaultForUser=true per UserId.
            // Using a partial unique index.
            builder.Entity<UserBranchMembership>()
                .HasIndex(m => m.UserId)
                .IsUnique()
                .HasFilter("IsDefaultForUser = 1");

            // UserBranchRole
            builder.Entity<UserBranchRole>()
                .HasIndex(r => new { r.UserId, r.BranchId, r.RoleName })
                .IsUnique();

            // AuditEvent
            // No unique constraints specified, append-only.
            // But configure relationships.
            builder.Entity<AuditEvent>()
                .HasOne(a => a.Branch)
                .WithMany()
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // Never delete audit events on branch delete

             builder.Entity<AuditEvent>()
                .HasOne(a => a.ActorUser)
                .WithMany()
                .HasForeignKey(a => a.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Concurrency Configuration (Generic for future)
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IConcurrencyAware).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(IConcurrencyAware.Version))
                        .IsConcurrencyToken();
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<IConcurrencyAware>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.Version++;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
