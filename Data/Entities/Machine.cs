using System.ComponentModel.DataAnnotations;
using CMetalsFulfillment.Data.Enums;

namespace CMetalsFulfillment.Data.Entities;

public class Machine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(20)]
    public string MachineCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MachineName { get; set; } = string.Empty;

    [Required]
    public MachineType MachineType { get; set; }

    public bool IsActive { get; set; } = true;

    public int DefaultDurationMinutes { get; set; } = 60;

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
