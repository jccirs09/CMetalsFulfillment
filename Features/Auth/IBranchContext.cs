using System.Threading.Tasks;

namespace CMetalsFulfillment.Features.Auth
{
    public interface IBranchContext
    {
        Task<int> GetBranchIdAsync();
    }
}
