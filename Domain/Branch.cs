using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Domain
{
    public class Branch
    {
        public int BranchId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
