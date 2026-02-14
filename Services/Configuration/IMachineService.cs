using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services.Configuration;

public interface IMachineService
{
    // Machine
    Task<List<Machine>> GetMachinesAsync(int branchId);
    Task CreateMachineAsync(Machine machine);
    Task UpdateMachineAsync(Machine machine);
    Task DeleteMachineAsync(int id, int branchId);

    // Operator
    Task AssignOperatorAsync(int machineId, string userId);
    Task UnassignOperatorAsync(int machineId, string userId);
    Task<List<MachineOperatorAssignment>> GetActiveAssignmentsAsync(int branchId);

    // Station
    Task<List<PickPackStation>> GetStationsAsync(int branchId);
    Task CreateStationAsync(PickPackStation station);
    Task UpdateStationAsync(PickPackStation station);
    Task DeleteStationAsync(int id, int branchId);
}
