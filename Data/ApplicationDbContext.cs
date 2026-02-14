using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Branch> Branches { get; set; }
    public DbSet<UserBranchMembership> UserBranchMemberships { get; set; }
    public DbSet<UserBranchRole> UserBranchRoles { get; set; }
    public DbSet<AuditEvent> AuditEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Branch
        builder.Entity<Branch>().HasIndex(b => b.Code).IsUnique();

        // UserBranchMembership
        builder.Entity<UserBranchMembership>()
            .HasIndex(m => new { m.UserId, m.BranchId })
            .IsUnique();

        // UserBranchRole
        builder.Entity<UserBranchRole>()
            .HasIndex(r => new { r.UserId, r.BranchId, r.RoleName })
            .IsUnique();

        // AuditEvent
        builder.Entity<AuditEvent>()
            .HasIndex(a => a.BranchId);
    }
}
