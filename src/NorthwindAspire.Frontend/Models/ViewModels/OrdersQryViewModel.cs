using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

/// <summary>
/// ViewModel for the Orders Qry view.
/// Provides detailed order information including customer and employee data.
/// </summary>
public class OrdersQryViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }

    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    public int? EmployeeId { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? OrderDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? RequiredDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", NullDisplayText = "N/A")]
    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    [Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
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

    // Customer details
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

    // Employee (Salesman) details
    [StringLength(81)]
    public string Salesman { get; set; } = string.Empty;
}
