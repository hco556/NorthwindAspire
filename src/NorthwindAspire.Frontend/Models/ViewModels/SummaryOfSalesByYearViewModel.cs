using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

/// <summary>
/// ViewModel for the Summary of Sales by Year view.
/// Provides total sales amounts aggregated by year.
/// </summary>
public class SummaryOfSalesByYearViewModel
{
    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal SaleAmount { get; set; }
}
