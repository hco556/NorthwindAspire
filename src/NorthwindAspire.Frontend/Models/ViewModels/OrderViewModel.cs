using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class OrderViewModel
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    public string CustomerCompanyName { get; set; } = string.Empty;

    public int? EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime? OrderDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? RequiredDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    public string ShipperName { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    public decimal Freight { get; set; }

    [StringLength(40)]
    public string ShipName { get; set; } = string.Empty;

    [StringLength(60)]
    public string ShipAddress { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipCity { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipRegion { get; set; } = string.Empty;

    [StringLength(10)]
    public string ShipPostalCode { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipCountry { get; set; } = string.Empty;

    public List<OrderDetailViewModel> OrderDetails { get; set; } = new();

    public decimal OrderTotal => OrderDetails?.Sum(od => od.LineTotal) ?? 0;

    public bool IsShipped => ShippedDate.HasValue;
}
