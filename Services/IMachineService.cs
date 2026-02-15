using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface IMachineService
{
    Task<List<Machine>> GetMachinesAsync(int branchId);
    Task<Machine?> GetMachineAsync(int id, int branchId);
    Task<Machine> CreateMachineAsync(Machine machine);
    Task<Machine> UpdateMachineAsync(Machine machine);
    Task AddOperatorAsync(int machineId, int branchId, string userId, string assignedBy);
    Task RemoveOperatorAsync(int machineId, int branchId, string userId);
    Task<List<MachineOperatorAssignment>> GetOperatorsAsync(int machineId, int branchId);
}
