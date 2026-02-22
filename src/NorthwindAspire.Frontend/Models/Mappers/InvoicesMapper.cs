using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Invoices view to InvoicesViewModel.
/// This complex denormalized view combines order, order details, product, customer, employee, and shipper information.
/// Maps from: OrderDetail model with Order, Customer, Employee, Shipper, and Product navigation properties
/// </summary>
public class InvoicesMapper : IMapper<OrderDetail, InvoicesViewModel>
{
    public InvoicesViewModel MapToViewModel(OrderDetail model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var order = model.Order;
        var customer = order?.Customer;
        var employee = order?.Employee;
        var shipper = order?.Shipper;
        var extendedPrice = model.UnitPrice * model.Quantity * (1 - model.Discount);

        return new InvoicesViewModel
        {
            OrderId = model.OrderId,
            CustomerId = customer?.CustomerId ?? string.Empty,
            CompanyName = customer?.CompanyName ?? string.Empty,
            Address = customer?.Address ?? string.Empty,
            City = customer?.City ?? string.Empty,
            Region = customer?.Region ?? string.Empty,
            PostalCode = customer?.PostalCode ?? string.Empty,
            Country = customer?.Country ?? string.Empty,
            FirstName = employee?.FirstName ?? string.Empty,
            LastName = employee?.LastName ?? string.Empty,
            OrderDate = order?.OrderDate,
            RequiredDate = order?.RequiredDate,
            ShippedDate = order?.ShippedDate,
            ShipperName = shipper?.CompanyName ?? string.Empty,
            ProductId = model.ProductId,
            ProductName = model.Product?.ProductName ?? string.Empty,
            UnitPrice = model.UnitPrice,
            Quantity = model.Quantity,
            Discount = model.Discount,
            ExtendedPrice = extendedPrice
        };
    }

    public OrderDetail MapToModel(InvoicesViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Invoices are generated from Order and OrderDetail models.");
    }

    public IEnumerable<InvoicesViewModel> MapToViewModelList(IEnumerable<OrderDetail> models)
    {
        if (models == null)
            throw new ArgumentNullException(nameof(models));

        return models.Select(MapToViewModel);
    }

    public IEnumerable<OrderDetail> MapToModelList(IEnumerable<InvoicesViewModel> viewModels)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model list.");
    }
}
