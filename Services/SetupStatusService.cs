using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class SetupStatusService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<bool> IsSetupCompleteAsync()
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Branches.AnyAsync();
    }
}
