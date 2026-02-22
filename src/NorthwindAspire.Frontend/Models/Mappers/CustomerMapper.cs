using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class CustomerMapper : IMapper<Customer, CustomerViewModel>
{
    public CustomerViewModel MapToViewModel(Customer model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new CustomerViewModel
        {
            CustomerId = model.CustomerId,
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
            OrderIds = model.Orders?.Select(o => o.OrderId).ToList() ?? new()
        };
    }

    public Customer MapToModel(CustomerViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Customer
        {
            CustomerId = viewModel.CustomerId,
            CompanyName = viewModel.CompanyName,
            ContactName = viewModel.ContactName,
            ContactTitle = viewModel.ContactTitle,
            Address = viewModel.Address,
            City = viewModel.City,
            Region = viewModel.Region,
            PostalCode = viewModel.PostalCode,
            Country = viewModel.Country,
            Phone = viewModel.Phone,
            Fax = viewModel.Fax
        };
    }

    public IEnumerable<CustomerViewModel> MapToViewModelList(IEnumerable<Customer> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<CustomerViewModel>();
    }

    public IEnumerable<Customer> MapToModelList(IEnumerable<CustomerViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Customer>();
    }
}
