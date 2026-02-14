using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services.Configuration;

public interface IShiftService
{
    Task<List<ShiftTemplate>> GetShiftsAsync(int branchId);
    Task<ShiftTemplate?> GetShiftAsync(int id, int branchId);
    Task CreateShiftAsync(ShiftTemplate shift, string userId);
    Task UpdateShiftAsync(ShiftTemplate shift, string userId, string? overtimeReason = null);
    Task DeleteShiftAsync(int id, int branchId);
}
