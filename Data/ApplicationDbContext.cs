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

    // Phase 2 Entities
    public DbSet<Machine> Machines { get; set; }
    public DbSet<MachineOperatorAssignment> MachineOperatorAssignments { get; set; }
    public DbSet<PickPackStation> PickPackStations { get; set; }
    public DbSet<ShiftTemplate> ShiftTemplates { get; set; }
    public DbSet<Truck> Trucks { get; set; }
    public DbSet<ShippingRegion> ShippingRegions { get; set; }
    public DbSet<ShippingGroup> ShippingGroups { get; set; }
    public DbSet<ShippingFsaRule> ShippingFsaRules { get; set; }
    public DbSet<ShippingFobMapping> ShippingFobMappings { get; set; }
    public DbSet<NonWorkingDay> NonWorkingDays { get; set; }
    public DbSet<NonWorkingDayOverride> NonWorkingDayOverrides { get; set; }

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

        // Phase 2 Constraints

        // Machine
        builder.Entity<Machine>()
            .HasIndex(m => new { m.BranchId, m.MachineCode })
            .IsUnique();

        // MachineOperatorAssignment
        builder.Entity<MachineOperatorAssignment>()
            .HasIndex(m => new { m.BranchId, m.MachineId, m.UserId })
            .IsUnique();

        // PickPackStation
        builder.Entity<PickPackStation>()
            .HasIndex(s => new { s.BranchId, s.StationCode })
            .IsUnique();

        // ShiftTemplate
        builder.Entity<ShiftTemplate>()
            .HasIndex(s => new { s.BranchId, s.ShiftCode })
            .IsUnique();

        // Truck
        builder.Entity<Truck>()
            .HasIndex(t => new { t.BranchId, t.TruckCode })
            .IsUnique();

        // ShippingRegion
        builder.Entity<ShippingRegion>()
            .HasIndex(r => new { r.BranchId, r.RegionCode })
            .IsUnique();

        // ShippingGroup
        builder.Entity<ShippingGroup>()
            .HasIndex(g => new { g.BranchId, g.GroupCode })
            .IsUnique();

        // ShippingFsaRule
        builder.Entity<ShippingFsaRule>()
            .HasIndex(f => new { f.BranchId, f.FsaPrefix, f.Priority })
            .IsUnique();

        // ShippingFobMapping
        builder.Entity<ShippingFobMapping>()
            .HasIndex(f => new { f.BranchId, f.FobToken })
            .IsUnique();

        // NonWorkingDay
        builder.Entity<NonWorkingDay>()
            .HasIndex(n => new { n.BranchId, n.DateLocal })
            .IsUnique();

        // NonWorkingDayOverride
        builder.Entity<NonWorkingDayOverride>()
            .HasIndex(n => new { n.BranchId, n.DateLocal, n.OverrideType })
            .IsUnique();
    }
}
