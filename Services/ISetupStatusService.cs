namespace CMetalsFulfillment.Services;

public class SetupStatus
{
    public bool IsComplete => Gates.Values.All(v => v);
    public Dictionary<string, bool> Gates { get; set; } = new();
}

public interface ISetupStatusService
{
    Task<SetupStatus> GetStatusAsync(int branchId);
}
