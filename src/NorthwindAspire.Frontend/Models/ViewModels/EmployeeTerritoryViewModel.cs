using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class EmployeeTerritoryViewModel
{
    [Required]
    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string TerritoryId { get; set; } = string.Empty;

    public string TerritoryDescription { get; set; } = string.Empty;
}
