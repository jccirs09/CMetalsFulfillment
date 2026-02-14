using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface ICalendarService
{
    Task<List<NonWorkingDay>> GetNonWorkingDaysAsync(int branchId, DateOnly start, DateOnly end);
    Task<NonWorkingDay> AddNonWorkingDayAsync(NonWorkingDay day);
    Task<NonWorkingDayOverride> AddOverrideAsync(int branchId, DateOnly date, string reason, string approvedByUserId);
    Task<bool> IsOvertimeEnabledAsync(int branchId, DateOnly date);
}
