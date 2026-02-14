using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface IStationService
{
    Task<List<PickPackStation>> GetStationsAsync(int branchId);
    Task<PickPackStation?> GetStationAsync(int id, int branchId);
    Task<PickPackStation> CreateStationAsync(PickPackStation station);
    Task<PickPackStation> UpdateStationAsync(PickPackStation station);
}
