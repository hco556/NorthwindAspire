using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class TerritoryViewModel
{
    [Required]
    [StringLength(20)]
    public string TerritoryId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string TerritoryDescription { get; set; } = string.Empty;

    [Required]
    public int RegionId { get; set; }

    public string RegionDescription { get; set; } = string.Empty;

    public List<int> EmployeeIds { get; set; } = new();

    public int EmployeeCount => EmployeeIds?.Count ?? 0;
}
