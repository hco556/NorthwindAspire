using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class ShipperViewModel
{
    [Required]
    public int ShipperId { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string CompanyName { get; set; } = string.Empty;

    [Phone]
    [StringLength(24)]
    public string Phone { get; set; } = string.Empty;

    public List<int> OrderIds { get; set; } = new();

    public int OrderCount => OrderIds?.Count ?? 0;
}
