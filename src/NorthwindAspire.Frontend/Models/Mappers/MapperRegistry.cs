using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class MapperRegistry
{
    private readonly Dictionary<Type, object> _mappers = new();

    public MapperRegistry(
        CategoryMapper? categoryMapper = null,
        CustomerMapper? customerMapper = null,
        ProductMapper? productMapper = null,
        OrderMapper? orderMapper = null,
        OrderDetailMapper? orderDetailMapper = null,
        EmployeeMapper? employeeMapper = null,
        SupplierMapper? supplierMapper = null,
        ShipperMapper? shipperMapper = null,
        RegionMapper? regionMapper = null,
        TerritoryMapper? territoryMapper = null,
        EmployeeTerritoryMapper? employeeTerritoryMapper = null,
        OrdersQryMapper? ordersQryMapper = null,
        InvoicesMapper? invoicesMapper = null,
        OrderDetailsExtendedMapper? orderDetailsExtendedMapper = null,
        SummaryOfSalesByYearMapper? summaryOfSalesByYearMapper = null,
        CurrentProductListMapper? currentProductListMapper = null)
    {
        RegisterDefaultMappers(
            categoryMapper,
            customerMapper,
            productMapper,
            orderMapper,
            orderDetailMapper,
            employeeMapper,
            supplierMapper,
            shipperMapper,
            regionMapper,
            territoryMapper,
            employeeTerritoryMapper,
            ordersQryMapper,
            invoicesMapper,
            orderDetailsExtendedMapper,
            summaryOfSalesByYearMapper,
            currentProductListMapper);
    }

    private void RegisterDefaultMappers(
        CategoryMapper? categoryMapper,
        CustomerMapper? customerMapper,
        ProductMapper? productMapper,
        OrderMapper? orderMapper,
        OrderDetailMapper? orderDetailMapper,
        EmployeeMapper? employeeMapper,
        SupplierMapper? supplierMapper,
        ShipperMapper? shipperMapper,
        RegionMapper? regionMapper,
        TerritoryMapper? territoryMapper,
        EmployeeTerritoryMapper? employeeTerritoryMapper,
        OrdersQryMapper? ordersQryMapper,
        InvoicesMapper? invoicesMapper,
        OrderDetailsExtendedMapper? orderDetailsExtendedMapper,
        SummaryOfSalesByYearMapper? summaryOfSalesByYearMapper,
        CurrentProductListMapper? currentProductListMapper)
    {
        Register<Category, CategoryViewModel>(categoryMapper ?? new CategoryMapper());
        Register<Customer, CustomerViewModel>(customerMapper ?? new CustomerMapper());
        Register<Product, ProductViewModel>(productMapper ?? new ProductMapper());
        Register<OrderDetail, OrderDetailViewModel>(orderDetailMapper ?? new OrderDetailMapper());
        Register<Order, OrderViewModel>(orderMapper ?? new OrderMapper(orderDetailMapper));
        Register<Employee, EmployeeViewModel>(employeeMapper ?? new EmployeeMapper());
        Register<Supplier, SupplierViewModel>(supplierMapper ?? new SupplierMapper());
        Register<Shipper, ShipperViewModel>(shipperMapper ?? new ShipperMapper());
        Register<Region, RegionViewModel>(regionMapper ?? new RegionMapper());
        Register<Territory, TerritoryViewModel>(territoryMapper ?? new TerritoryMapper());
        Register<EmployeeTerritory, EmployeeTerritoryViewModel>(employeeTerritoryMapper ?? new EmployeeTerritoryMapper());

        // View Mappers - High Priority
        Register<Order, OrdersQryViewModel>(ordersQryMapper ?? new OrdersQryMapper());
        Register<OrderDetail, InvoicesViewModel>(invoicesMapper ?? new InvoicesMapper());
        Register<OrderDetail, OrderDetailsExtendedViewModel>(orderDetailsExtendedMapper ?? new OrderDetailsExtendedMapper());
        Register<SalesAggregate, SummaryOfSalesByYearViewModel>(summaryOfSalesByYearMapper ?? new SummaryOfSalesByYearMapper());
        Register<Product, CurrentProductListViewModel>(currentProductListMapper ?? new CurrentProductListMapper());
    }

    public void Register<TModel, TViewModel>(IMapper<TModel, TViewModel> mapper)
    {
        if (mapper == null)
            throw new ArgumentNullException(nameof(mapper));

        var key = typeof((TModel, TViewModel));
        _mappers[key] = mapper;
    }

    public IMapper<TModel, TViewModel> GetMapper<TModel, TViewModel>()
    {
        var key = typeof((TModel, TViewModel));
        if (_mappers.TryGetValue(key, out var mapper))
        {
            return (IMapper<TModel, TViewModel>)mapper;
        }
        throw new InvalidOperationException(
            $"Mapper for {typeof(TModel).Name} -> {typeof(TViewModel).Name} not registered");
    }
}
