using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data.Entities;

[Owned]
public class BranchSettings
{
    public string TimezoneId { get; set; } = "UTC";
}
