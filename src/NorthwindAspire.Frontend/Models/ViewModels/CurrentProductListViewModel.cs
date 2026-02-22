using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

/// <summary>
/// ViewModel for the Current Product List view.
/// Displays all active (non-discontinued) products with their IDs and names.
/// </summary>
public class CurrentProductListViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;
}
