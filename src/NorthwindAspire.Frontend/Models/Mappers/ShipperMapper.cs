using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class ShipperMapper : IMapper<Shipper, ShipperViewModel>
{
    public ShipperViewModel MapToViewModel(Shipper model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new ShipperViewModel
        {
            ShipperId = model.ShipperId,
            CompanyName = model.CompanyName,
            Phone = model.Phone,
            OrderIds = model.Orders?.Select(o => o.OrderId).ToList() ?? new()
        };
    }

    public Shipper MapToModel(ShipperViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Shipper
        {
            ShipperId = viewModel.ShipperId,
            CompanyName = viewModel.CompanyName,
            Phone = viewModel.Phone
        };
    }

    public IEnumerable<ShipperViewModel> MapToViewModelList(IEnumerable<Shipper> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<ShipperViewModel>();
    }

    public IEnumerable<Shipper> MapToModelList(IEnumerable<ShipperViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Shipper>();
    }
}
