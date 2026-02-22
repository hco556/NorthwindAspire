using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Order Details Extended view to OrderDetailsExtendedViewModel.
/// This view extends order detail information with product names and calculated extended prices.
/// Maps from: OrderDetail model with Product navigation property
/// </summary>
public class OrderDetailsExtendedMapper : IMapper<OrderDetail, OrderDetailsExtendedViewModel>
{
    public OrderDetailsExtendedViewModel MapToViewModel(OrderDetail model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var extendedPrice = model.UnitPrice * model.Quantity * (1 - model.Discount);

        return new OrderDetailsExtendedViewModel
        {
            OrderId = model.OrderId,
            ProductId = model.ProductId,
            ProductName = model.Product?.ProductName ?? string.Empty,
            UnitPrice = model.UnitPrice,
            Quantity = model.Quantity,
            Discount = model.Discount,
            ExtendedPrice = extendedPrice
        };
    }

    public OrderDetail MapToModel(OrderDetailsExtendedViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Use OrderDetail model directly for create/update operations.");
    }

    public IEnumerable<OrderDetailsExtendedViewModel> MapToViewModelList(IEnumerable<OrderDetail> models)
    {
        if (models == null)
            throw new ArgumentNullException(nameof(models));

        return models.Select(MapToViewModel);
    }

    public IEnumerable<OrderDetail> MapToModelList(IEnumerable<OrderDetailsExtendedViewModel> viewModels)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model list.");
    }
}
