using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class RegionViewModel
{
    [Required]
    public int RegionId { get; set; }

    [Required]
    [StringLength(50)]
    public string RegionDescription { get; set; } = string.Empty;

    public List<string> TerritoryIds { get; set; } = new();

    public int TerritoryCount => TerritoryIds?.Count ?? 0;
}
