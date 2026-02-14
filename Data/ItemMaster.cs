using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class ItemMaster
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoilRelationship { get; set; } = "None"; // None, Parent, Child

    // Aggregates
    public decimal TotalWeightLbs { get; set; }
    public decimal TotalQuantity { get; set; }
    public string Uom { get; set; } = "PCS"; // PCS or LBS

    public virtual Branch Branch { get; set; } = null!;
}
