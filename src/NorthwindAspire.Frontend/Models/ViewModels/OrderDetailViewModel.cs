using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class OrderDetailViewModel
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, 1)]
    public decimal Discount { get; set; }

    public decimal LineTotal => UnitPrice * Quantity * (1 - Discount);

    public decimal DiscountAmount => UnitPrice * Quantity * Discount;
}
