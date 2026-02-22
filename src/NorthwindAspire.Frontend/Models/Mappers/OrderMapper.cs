using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class OrderMapper : IMapper<Order, OrderViewModel>
{
    private readonly OrderDetailMapper _orderDetailMapper;

    public OrderMapper(OrderDetailMapper? orderDetailMapper = null)
    {
        _orderDetailMapper = orderDetailMapper ?? new OrderDetailMapper();
    }

    public OrderViewModel MapToViewModel(Order model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new OrderViewModel
        {
            OrderId = model.OrderId,
            CustomerId = model.CustomerId,
            CustomerCompanyName = model.Customer?.CompanyName ?? string.Empty,
            EmployeeId = model.EmployeeId,
            EmployeeName = GetEmployeeName(model.Employee),
            OrderDate = model.OrderDate,
            RequiredDate = model.RequiredDate,
            ShippedDate = model.ShippedDate,
            ShipVia = model.ShipVia,
            ShipperName = model.Shipper?.CompanyName ?? string.Empty,
            Freight = model.Freight,
            ShipName = model.ShipName,
            ShipAddress = model.ShipAddress,
            ShipCity = model.ShipCity,
            ShipRegion = model.ShipRegion,
            ShipPostalCode = model.ShipPostalCode,
            ShipCountry = model.ShipCountry,
            OrderDetails = model.OrderDetails?
                .Select(od => _orderDetailMapper.MapToViewModel(od))
                .ToList() ?? new()
        };
    }

    public Order MapToModel(OrderViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Order
        {
            OrderId = viewModel.OrderId,
            CustomerId = viewModel.CustomerId,
            EmployeeId = viewModel.EmployeeId,
            OrderDate = viewModel.OrderDate,
            RequiredDate = viewModel.RequiredDate,
            ShippedDate = viewModel.ShippedDate,
            ShipVia = viewModel.ShipVia,
            Freight = viewModel.Freight,
            ShipName = viewModel.ShipName,
            ShipAddress = viewModel.ShipAddress,
            ShipCity = viewModel.ShipCity,
            ShipRegion = viewModel.ShipRegion,
            ShipPostalCode = viewModel.ShipPostalCode,
            ShipCountry = viewModel.ShipCountry
        };
    }

    public IEnumerable<OrderViewModel> MapToViewModelList(IEnumerable<Order> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<OrderViewModel>();
    }

    public IEnumerable<Order> MapToModelList(IEnumerable<OrderViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Order>();
    }

    private static string GetEmployeeName(Employee? employee)
    {
        if (employee == null)
            return string.Empty;
        return $"{employee.FirstName} {employee.LastName}".Trim();
    }
}
