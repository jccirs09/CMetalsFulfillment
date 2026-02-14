namespace CMetalsFulfillment.Services;

public class SetupStatusService
{
    public bool IsSetupComplete(int branchId)
    {
        return branchId > 0;
    }
}
