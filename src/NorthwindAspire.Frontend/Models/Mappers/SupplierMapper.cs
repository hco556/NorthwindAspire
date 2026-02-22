using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class SupplierMapper : IMapper<Supplier, SupplierViewModel>
{
    public SupplierViewModel MapToViewModel(Supplier model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new SupplierViewModel
        {
            SupplierId = model.SupplierId,
            CompanyName = model.CompanyName,
            ContactName = model.ContactName,
            ContactTitle = model.ContactTitle,
            Address = model.Address,
            City = model.City,
            Region = model.Region,
            PostalCode = model.PostalCode,
            Country = model.Country,
            Phone = model.Phone,
            Fax = model.Fax,
            HomePage = model.HomePage,
            ProductIds = model.Products?.Select(p => p.ProductId).ToList() ?? new()
        };
    }

    public Supplier MapToModel(SupplierViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Supplier
        {
            SupplierId = viewModel.SupplierId,
            CompanyName = viewModel.CompanyName,
            ContactName = viewModel.ContactName,
            ContactTitle = viewModel.ContactTitle,
            Address = viewModel.Address,
            City = viewModel.City,
            Region = viewModel.Region,
            PostalCode = viewModel.PostalCode,
            Country = viewModel.Country,
            Phone = viewModel.Phone,
            Fax = viewModel.Fax,
            HomePage = viewModel.HomePage
        };
    }

    public IEnumerable<SupplierViewModel> MapToViewModelList(IEnumerable<Supplier> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<SupplierViewModel>();
    }

    public IEnumerable<Supplier> MapToModelList(IEnumerable<SupplierViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Supplier>();
    }
}
