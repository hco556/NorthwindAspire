using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

/// <summary>
/// ViewModel for the Order Details Extended view.
/// Extends order detail information with product names and calculated extended prices.
/// </summary>
public class OrderDetailsExtendedViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [StringLength(40)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, 1)]
    public decimal Discount { get; set; }

    [Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal ExtendedPrice { get; set; }
}
