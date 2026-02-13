using CMetalsFulfillment.Features.Auth;

namespace CMetalsFulfillment.Features.Admin
{
    public class BranchSetupStatus
    {
        public bool HasMachines { get; set; }
        public bool HasPickPackStations { get; set; }
        public bool HasShiftTemplates { get; set; }
        public bool HasShippingRules { get; set; }
        public bool HasItemMaster { get; set; }

        public bool IsComplete => HasMachines && HasPickPackStations && HasShiftTemplates && HasShippingRules && HasItemMaster;
    }

    public class SetupGateService
    {
        private readonly IBranchContext _branchContext;

        public SetupGateService(IBranchContext branchContext)
        {
            _branchContext = branchContext;
        }

        public Task<BranchSetupStatus> GetStatusAsync()
        {
             // Placeholder: Always return incomplete until features are implemented.
             return Task.FromResult(new BranchSetupStatus());
        }
    }
}
