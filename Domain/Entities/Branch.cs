using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Domain.Entities;

public class Branch
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
