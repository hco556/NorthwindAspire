using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class EmployeeViewModel
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(20)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string FirstName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    [StringLength(30)]
    public string Title { get; set; } = string.Empty;

    [StringLength(25)]
    public string TitleOfCourtesy { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? HireDate { get; set; }

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
    public string HomePhone { get; set; } = string.Empty;

    [StringLength(4)]
    public string Extension { get; set; } = string.Empty;

    [Url]
    public string PhotoPath { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public int? ReportsTo { get; set; }

    public string ManagerName { get; set; } = string.Empty;

    public List<int> SubordinateIds { get; set; } = new();

    public List<int> OrderIds { get; set; } = new();

    public List<string> TerritoryIds { get; set; } = new();

    public int SubordinateCount => SubordinateIds?.Count ?? 0;

    public int OrderCount => OrderIds?.Count ?? 0;

    public int TerritoryCount => TerritoryIds?.Count ?? 0;
}
