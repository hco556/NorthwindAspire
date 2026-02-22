using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Summary of Sales by Year view to SummaryOfSalesByYearViewModel.
/// This aggregate view provides total sales amounts grouped by year.
/// Maps from: SalesAggregate intermediate DTO model
/// </summary>
public class SummaryOfSalesByYearMapper : IMapper<SalesAggregate, SummaryOfSalesByYearViewModel>
{
    public SummaryOfSalesByYearViewModel MapToViewModel(SalesAggregate model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new SummaryOfSalesByYearViewModel
        {
            Year = model.Year ?? 0,
            SaleAmount = model.SaleAmount
        };
    }

    public SalesAggregate MapToModel(SummaryOfSalesByYearViewModel viewModel)
    {
        throw new NotSupportedException(
            "Aggregate view data is read-only and cannot be mapped to a data model. " +
            "Aggregate data is calculated from order details.");
    }

    public IEnumerable<SummaryOfSalesByYearViewModel> MapToViewModelList(IEnumerable<SalesAggregate> models)
    {
        if (models == null)
            throw new ArgumentNullException(nameof(models));

        return models.Select(MapToViewModel);
    }

    public IEnumerable<SalesAggregate> MapToModelList(IEnumerable<SummaryOfSalesByYearViewModel> viewModels)
    {
        throw new NotSupportedException(
            "Aggregate view data is read-only and cannot be mapped to a data model list.");
    }
}
