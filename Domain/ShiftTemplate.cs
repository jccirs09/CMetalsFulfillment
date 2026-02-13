using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class ShiftTemplate
    {
        public int ShiftTemplateId { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public TimeSpan StartTimeLocal { get; set; }
        public TimeSpan EndTimeLocal { get; set; }

        public string? BreakRulesJson { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }
}
