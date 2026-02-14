namespace CMetalsFulfillment.Services
{
    public interface IBranchContext
    {
        int? ActiveBranchId { get; }
        string? ActiveBranchCode { get; }
        string? ActiveTimeZoneId { get; }
        void SetBranch(int branchId, string branchCode, string timeZoneId);
        Task<int> GetBranchIdAsync();
    }

    public class BranchContext : IBranchContext
    {
        public int? ActiveBranchId { get; private set; }
        public string? ActiveBranchCode { get; private set; }
        public string? ActiveTimeZoneId { get; private set; }

        public void SetBranch(int branchId, string branchCode, string timeZoneId)
        {
            ActiveBranchId = branchId;
            ActiveBranchCode = branchCode;
            ActiveTimeZoneId = timeZoneId;
        }

        public Task<int> GetBranchIdAsync()
        {
             if (ActiveBranchId.HasValue)
                return Task.FromResult(ActiveBranchId.Value);

            throw new InvalidOperationException("No active branch set in BranchContext.");
        }
    }
}
