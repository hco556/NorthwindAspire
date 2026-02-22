using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class CustomerViewModel
{
    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(30)]
    public string ContactName { get; set; } = string.Empty;

    [StringLength(30)]
    public string ContactTitle { get; set; } = string.Empty;

    [StringLength(60)]
    public string Address { get; set; } = string.Empty;

    [StringLength(15)]
    public string City { get; set; } = string.Empty;

    [StringLength(15)]
    public string Region { get; set; } = string.Empty;

    [StringLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(15)]
    public string Country { get; set; } = string.Empty;

    [Phone]
    [StringLength(24)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(24)]
    public string Fax { get; set; } = string.Empty;

    public List<int> OrderIds { get; set; } = new();

    public int OrderCount => OrderIds?.Count ?? 0;

    public string DisplayName => $"{CompanyName} ({ContactName})";
}
