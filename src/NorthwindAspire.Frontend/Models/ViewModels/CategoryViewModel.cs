using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class CategoryViewModel
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 15 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(int.MaxValue)]
    public string Description { get; set; } = string.Empty;

    public List<int> ProductIds { get; set; } = new();

    public int ProductCount => ProductIds?.Count ?? 0;
}
