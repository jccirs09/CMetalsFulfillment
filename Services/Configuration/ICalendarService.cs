using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services.Configuration;

public interface ICalendarService
{
    Task<List<NonWorkingDay>> GetNonWorkingDaysAsync(int branchId);
    Task<List<NonWorkingDayOverride>> GetOverridesAsync(int branchId);

    Task AddNonWorkingDayAsync(NonWorkingDay day);
    Task AddOverrideAsync(NonWorkingDayOverride ovr);

    Task DeleteNonWorkingDayAsync(int id, int branchId);
    Task DeleteOverrideAsync(int id, int branchId);

    Task<bool> IsWorkingDayAsync(DateOnly date, int branchId);
}
