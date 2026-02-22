using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class OrderDetailMapper : IMapper<OrderDetail, OrderDetailViewModel>
{
    public OrderDetailViewModel MapToViewModel(OrderDetail model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new OrderDetailViewModel
        {
            OrderId = model.OrderId,
            ProductId = model.ProductId,
            ProductName = model.Product?.ProductName ?? string.Empty,
            UnitPrice = model.UnitPrice,
            Quantity = model.Quantity,
            Discount = model.Discount
        };
    }

    public OrderDetail MapToModel(OrderDetailViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new OrderDetail
        {
            OrderId = viewModel.OrderId,
            ProductId = viewModel.ProductId,
            UnitPrice = viewModel.UnitPrice,
            Quantity = viewModel.Quantity,
            Discount = viewModel.Discount
        };
    }

    public IEnumerable<OrderDetailViewModel> MapToViewModelList(IEnumerable<OrderDetail> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<OrderDetailViewModel>();
    }

    public IEnumerable<OrderDetail> MapToModelList(IEnumerable<OrderDetailViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<OrderDetail>();
    }
}
