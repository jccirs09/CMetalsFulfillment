using Microsoft.AspNetCore.Authorization;

namespace CMetalsFulfillment.Services.Authorization;

public class BranchRoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public BranchRoleRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
