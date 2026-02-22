using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Orders Qry view to OrdersQryViewModel.
/// This view provides detailed order information including customer and employee data.
/// Maps from: Order model with Customer and Employee navigation properties
/// </summary>
public class OrdersQryMapper : IMapper<Order, OrdersQryViewModel>
{
    public OrdersQryViewModel MapToViewModel(Order model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new OrdersQryViewModel
        {
            OrderId = model.OrderId,
            CustomerId = model.CustomerId,
            EmployeeId = model.EmployeeId,
            OrderDate = model.OrderDate,
            RequiredDate = model.RequiredDate,
            ShippedDate = model.ShippedDate,
            ShipVia = model.ShipVia,
            Freight = model.Freight,
            ShipName = model.ShipName,
            ShipAddress = model.ShipAddress,
            ShipCity = model.ShipCity,
            ShipRegion = model.ShipRegion,
            ShipPostalCode = model.ShipPostalCode,
            ShipCountry = model.ShipCountry,
            // Customer details
            CompanyName = model.Customer?.CompanyName ?? string.Empty,
            Address = model.Customer?.Address ?? string.Empty,
            City = model.Customer?.City ?? string.Empty,
            Region = model.Customer?.Region ?? string.Empty,
            PostalCode = model.Customer?.PostalCode ?? string.Empty,
            Country = model.Customer?.Country ?? string.Empty,
            // Employee (Salesman) details
            Salesman = $"{model.Employee?.FirstName} {model.Employee?.LastName}".Trim()
        };
    }

    public Order MapToModel(OrdersQryViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Use Order model directly for create/update operations.");
    }

    public IEnumerable<OrdersQryViewModel> MapToViewModelList(IEnumerable<Order> models)
    {
        if (models == null)
            throw new ArgumentNullException(nameof(models));

        return models.Select(MapToViewModel);
    }

    public IEnumerable<Order> MapToModelList(IEnumerable<OrdersQryViewModel> viewModels)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model list.");
    }
}
