using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class PickPackStation
    {
        [Key]
        public int PickPackStationId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }

    public class PickPackStationAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PickPackStationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(PickPackStationId))]
        public PickPackStation? Station { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
