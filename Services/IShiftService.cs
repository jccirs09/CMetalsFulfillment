using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface IShiftService
{
    Task<List<ShiftTemplate>> GetShiftsAsync(int branchId);
    Task<ShiftTemplate?> GetShiftAsync(int id, int branchId);
    Task<ShiftTemplate> CreateShiftAsync(ShiftTemplate shift);
    Task<ShiftTemplate> UpdateShiftAsync(ShiftTemplate shift);
}
