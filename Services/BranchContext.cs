using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

namespace CMetalsFulfillment.Services
{
    public interface IBranchContext
    {
        int? ActiveBranchId { get; }
        string? ActiveBranchCode { get; }
        string? ActiveTimeZoneId { get; }
        Task ResolveAsync(string userId);
        Task SetActiveBranchAsync(int branchId);
    }

    public class BranchContext(
        IBranchService branchService,
        IUserBranchService userBranchService,
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor,
        IJSRuntime jsRuntime) : IBranchContext
    {
        public int? ActiveBranchId { get; private set; }
        public string? ActiveBranchCode { get; private set; }
        public string? ActiveTimeZoneId { get; private set; }

        public async Task ResolveAsync(string userId)
        {
            if (ActiveBranchId.HasValue) return;

            int? targetBranchId = null;

            // 1. Query String ?branchId=
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal))
            {
                if (int.TryParse(branchIdVal, out int bid))
                {
                    if (await userBranchService.HasMembershipAsync(userId, bid))
                    {
                        targetBranchId = bid;
                    }
                }
            }

            // 2. Cookie mf.branch
            if (targetBranchId == null)
            {
                var cookie = httpContextAccessor.HttpContext?.Request.Cookies["mf.branch"];
                if (!string.IsNullOrEmpty(cookie) && int.TryParse(cookie, out int cookieBid))
                {
                    if (await userBranchService.HasMembershipAsync(userId, cookieBid))
                    {
                        targetBranchId = cookieBid;
                    }
                }
            }

            // 3. Default Membership
            if (targetBranchId == null)
            {
                var defaultMembership = await userBranchService.GetDefaultMembershipAsync(userId);
                if (defaultMembership != null)
                {
                    targetBranchId = defaultMembership.BranchId;
                }
            }

            if (targetBranchId.HasValue)
            {
                var branch = await branchService.GetBranchByIdAsync(targetBranchId.Value);
                if (branch != null && branch.IsActive)
                {
                    ActiveBranchId = branch.Id;
                    ActiveBranchCode = branch.Code;
                    ActiveTimeZoneId = branch.TimeZoneId;
                }
            }
        }

        public async Task SetActiveBranchAsync(int branchId)
        {
             var branch = await branchService.GetBranchByIdAsync(branchId);
             if (branch != null)
             {
                 ActiveBranchId = branch.Id;
                 ActiveBranchCode = branch.Code;
                 ActiveTimeZoneId = branch.TimeZoneId;

                 // Set cookie via JS Interop
                 try
                 {
                     await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'mf.branch={branchId}; path=/; max-age=31536000; samesite=strict'");
                 }
                 catch
                 {
                     // Ignored if prerendering or JS not available yet
                 }
             }
        }
    }
}
