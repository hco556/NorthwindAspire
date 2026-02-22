using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class ProductViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [StringLength(20)]
    public string QuantityPerUnit { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsInStock { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsOnOrder { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public bool IsAvailable => UnitsInStock > 0 && !Discontinued;

    public bool RequiresReorder => UnitsInStock <= ReorderLevel;
}
