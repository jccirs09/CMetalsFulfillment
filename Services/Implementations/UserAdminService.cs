using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain.Entities;
using CMetalsFulfillment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Implementations;

public class UserAdminService : IUserAdminService
{
    private readonly ApplicationDbContext _dbContext;

    public UserAdminService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ApplicationUser>> SearchUsersAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
             return await _dbContext.Users.Take(20).ToListAsync();
        }

        return await _dbContext.Users
            .Where(u => u.UserName!.Contains(query) || u.Email!.Contains(query))
            .Take(50)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetUsersByBranchAsync(int branchId)
    {
        return await _dbContext.Users
            .Where(u => u.BranchMemberships.Any(m => m.BranchId == branchId && m.IsActive))
            .ToListAsync();
    }

    public async Task SetBranchMembershipAsync(string userId, int branchId, bool isActive, bool isDefault)
    {
        var membership = await _dbContext.UserBranchMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == branchId);

        if (membership == null)
        {
            membership = new UserBranchMembership
            {
                UserId = userId,
                BranchId = branchId
            };
            _dbContext.UserBranchMemberships.Add(membership);
        }

        membership.IsActive = isActive;
        membership.DefaultForUser = isDefault;

        if (isDefault)
        {
            // Unset other defaults for this user
            var otherDefaults = await _dbContext.UserBranchMemberships
                .Where(m => m.UserId == userId && m.BranchId != branchId && m.DefaultForUser)
                .ToListAsync();
            foreach (var od in otherDefaults)
            {
                od.DefaultForUser = false;
            }

            // Also update User.DefaultBranchId
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.DefaultBranchId = branchId;
            }
        }
        else
        {
             // If unsetting default, we might leave user with no default or handle it.
             // For now just update.
             var user = await _dbContext.Users.FindAsync(userId);
             if (user != null && user.DefaultBranchId == branchId)
             {
                 user.DefaultBranchId = null;
             }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task SetBranchRoleAsync(string userId, int branchId, string roleName, bool isActive)
    {
        var role = await _dbContext.UserBranchRoles
            .FirstOrDefaultAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName);

        if (role == null)
        {
            if (!isActive) return; // Nothing to do

            role = new UserBranchRole
            {
                UserId = userId,
                BranchId = branchId,
                RoleName = roleName,
                IsActive = true
            };
            _dbContext.UserBranchRoles.Add(role);
        }
        else
        {
            role.IsActive = isActive;
        }

        await _dbContext.SaveChangesAsync();

        // Audit
        _dbContext.AuditEvents.Add(new AuditEvent
        {
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId, // Whom the action was performed ON
            BranchId = branchId,
            EventType = "RoleChange",
            Details = $"Role {roleName} set to Active={isActive}",
            EntityName = "UserBranchRole",
            EntityId = role.Id.ToString()
        });
        await _dbContext.SaveChangesAsync();
    }
}
