using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface ITruckService
{
    Task<List<Truck>> GetTrucksAsync(int branchId);
    Task<Truck?> GetTruckAsync(int id, int branchId);
    Task<Truck> CreateTruckAsync(Truck truck);
    Task<Truck> UpdateTruckAsync(Truck truck);
}
