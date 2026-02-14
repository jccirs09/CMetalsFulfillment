using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain.Constants;
using CMetalsFulfillment.Domain.Entities;
using CMetalsFulfillment.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Implementations;

public class BranchService : IBranchService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly UserManager<ApplicationUser> _userManager;

    public BranchService(
        ApplicationDbContext dbContext,
        AuthenticationStateProvider authStateProvider,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _authStateProvider = authStateProvider;
        _userManager = userManager;
    }

    private async Task CheckSystemAdminAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated != true)
             throw new UnauthorizedAccessException("User not authenticated");

        var userId = _userManager.GetUserId(user);
        var isSystemAdmin = await _dbContext.UserBranchRoles
            .AnyAsync(r => r.UserId == userId && r.RoleName == BranchRole.SystemAdmin && r.IsActive);

        if (!isSystemAdmin)
            throw new UnauthorizedAccessException("User is not SystemAdmin");
    }

    public async Task<List<Branch>> GetAllBranchesAsync()
    {
        return await _dbContext.Branches.ToListAsync();
    }

    public async Task<Branch?> GetBranchByIdAsync(int id)
    {
        return await _dbContext.Branches.FindAsync(id);
    }

    public async Task<Branch> CreateBranchAsync(Branch branch)
    {
        await CheckSystemAdminAsync();
        _dbContext.Branches.Add(branch);
        await _dbContext.SaveChangesAsync();
        return branch;
    }

    public async Task UpdateBranchAsync(Branch branch)
    {
        await CheckSystemAdminAsync();
        _dbContext.Branches.Update(branch);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteBranchAsync(int id)
    {
        await CheckSystemAdminAsync();
        var branch = await _dbContext.Branches.FindAsync(id);
        if (branch != null)
        {
            // Soft delete by setting IsActive to false
            branch.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }
    }
}
