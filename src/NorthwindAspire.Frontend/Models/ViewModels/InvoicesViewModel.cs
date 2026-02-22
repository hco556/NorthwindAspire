using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

/// <summary>
/// ViewModel for the Invoices view.
/// Combines order, order details, product, customer, employee, and shipper information.
/// This is a denormalized view designed for invoice generation and reporting.
/// </summary>
public class InvoicesViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }

    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string CompanyName { get; set; } = string.Empty;

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

    [StringLength(10)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(20)]
    public string LastName { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? OrderDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? RequiredDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? ShippedDate { get; set; }

    [StringLength(40)]
    public string ShipperName { get; set; } = string.Empty;

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
