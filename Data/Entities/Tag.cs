using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data.Enums;

namespace CMetalsFulfillment.Data.Entities;

public class Tag : IConcurrencyAware
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TagNumber { get; set; } = string.Empty;

    [Required]
    public int ItemMasterId { get; set; }

    [ForeignKey(nameof(ItemMasterId))]
    public ItemMaster Item { get; set; } = default!;

    [Required]
    public TagStatus Status { get; set; } = TagStatus.Received;

    [Range(0, double.MaxValue)]
    public decimal WeightNet { get; set; }

    [Range(0, double.MaxValue)]
    public decimal WeightGross { get; set; }

    [MaxLength(50)]
    public string? Location { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    [ConcurrencyCheck]
    public long Version { get; set; }
}

// Interface for Optimistic Concurrency
public interface IConcurrencyAware
{
    long Version { get; set; }
}
