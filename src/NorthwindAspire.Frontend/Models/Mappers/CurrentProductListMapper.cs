using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Current Product List view to CurrentProductListViewModel.
/// This view displays all active (non-discontinued) products with their IDs and names.
/// Maps from: Product model (filtered for active products only)
/// </summary>
public class CurrentProductListMapper : IMapper<Product, CurrentProductListViewModel>
{
    public CurrentProductListViewModel MapToViewModel(Product model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new CurrentProductListViewModel
        {
            ProductId = model.ProductId,
            ProductName = model.ProductName
        };
    }

    public Product MapToModel(CurrentProductListViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Use Product model directly for create/update operations.");
    }

    public IEnumerable<CurrentProductListViewModel> MapToViewModelList(IEnumerable<Product> models)
    {
        if (models == null)
            throw new ArgumentNullException(nameof(models));

        return models.Select(MapToViewModel);
    }

    public IEnumerable<Product> MapToModelList(IEnumerable<CurrentProductListViewModel> viewModels)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model list.");
    }
}
