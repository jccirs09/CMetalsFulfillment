using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(20)]
        public string MachineType { get; set; } = "CTL"; // "CTL" or "Slitter"

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }

    public class MachineOperatorAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(MachineId))]
        public Machine? Machine { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
