# View Mapper Implementation Quick Start

This guide provides step-by-step instructions for implementing ViewModels and Mappers for the Northwind database views.

## Prerequisites

- Review `VIEWS_VIEWMODELS_AND_MAPPERS.md` for complete patterns and examples
- Review `src/NorthwindAspire.Backend/VIEWS_SQL_DOCUMENTATION.md` for view purposes
- Review existing mapper implementations (CategoryMapper, ProductMapper, etc.)

## Step 1: Categorize Views by Type

All 16 views fall into one of these categories:

### Category A: Direct Product Mapping (3 views)
Maps directly from `Product` model:
- Alphabetical list of products
- Current Product List  
- Products Above Average Price

**Implementation:** Simple property mapping with null coalescing

### Category B: Filtered Product with Category (1 view)
Maps from `Product` with `Category` navigation:
- Products by Category

**Implementation:** Include category details, filter discontinued products

### Category C: Order-based Views (2 views)
Maps from `Order` with navigations:
- Orders Qry (Order + Customer + Employee)
- Quarterly Orders (Order + Customer)

**Implementation:** Denormalize related entities, concatenate names

### Category D: Order Detail Denormalized (2 views)
Maps from `OrderDetail` with multiple navigations:
- Invoices (OrderDetail + Order + Customer + Employee + Shipper + Product)
- Order Details Extended (OrderDetail + Product)

**Implementation:** Complex navigation chains with extensive null handling

### Category E: Aggregate Views (8 views)
Maps from intermediate DTOs:
- Order Subtotals
- Product Sales for 1997
- Summary of Sales by Quarter
- Summary of Sales by Year
- Sales Totals by Amount
- Category Sales for 1997
- Sales by Category
- Customer and Suppliers by City

**Implementation:** Use OrderSubtotal, SalesAggregate, or CategorySalesAggregate DTOs

## Step 2: Create ViewModels

For each view, create a ViewModel file in `src/NorthwindAspire.Frontend/Models/ViewModels/`

### Template for Category A (Simple Product)

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class AlphabeticalListOfProductsViewModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }

    [StringLength(20)]
    public string QuantityPerUnit { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsInStock { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsOnOrder { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public string Discontinued { get; set; } = "0";
}
```

### Template for Category C (Order with Relations)

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class OrdersQryViewModel
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    public int? EmployeeId { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? OrderDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? RequiredDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? ShippedDate { get; set; }

    // Order shipping details
    public int? ShipVia { get; set; }
    public decimal Freight { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipAddress { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipRegion { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;

    // Customer details
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // Employee (Salesman) details
    public string Salesman { get; set; } = string.Empty;
}
```

### Template for Category E (Aggregate)

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class SummaryOfSalesByYearViewModel
{
    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal SaleAmount { get; set; }
}
```

## Step 3: Create Mappers

For each ViewModel, create a corresponding Mapper in `src/NorthwindAspire.Frontend/Models/Mappers/`

### Template for Category A (Simple Product)

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Alphabetical list of products view to AlphabeticalListOfProductsViewModel.
/// This view displays all products sorted alphabetically by product name.
/// </summary>
public class AlphabeticalListOfProductsMapper : IMapper<Product, AlphabeticalListOfProductsViewModel>
{
    public AlphabeticalListOfProductsViewModel MapToViewModel(Product model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new AlphabeticalListOfProductsViewModel
        {
            ProductId = model.ProductId,
            ProductName = model.ProductName,
            SupplierId = model.SupplierId,
            CategoryId = model.CategoryId,
            QuantityPerUnit = model.QuantityPerUnit.ToString(),
            UnitPrice = model.UnitPrice,
            UnitsInStock = model.UnitsInStock,
            UnitsOnOrder = model.UnitsOnOrder,
            ReorderLevel = model.ReorderLevel,
            Discontinued = model.Discontinued
        };
    }

    public Product MapToModel(AlphabeticalListOfProductsViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Use Product model directly for create/update operations.");
    }
}
```

### Template for Category C (Order with Relations)

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Orders Qry view to OrdersQryViewModel.
/// This view provides detailed order information including customer and employee data.
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
            // Customer data
            CompanyName = model.Customer?.CompanyName ?? string.Empty,
            Address = model.Customer?.Address ?? string.Empty,
            City = model.Customer?.City ?? string.Empty,
            Region = model.Customer?.Region ?? string.Empty,
            PostalCode = model.Customer?.PostalCode ?? string.Empty,
            Country = model.Customer?.Country ?? string.Empty,
            // Employee data
            Salesman = $"{model.Employee?.FirstName} {model.Employee?.LastName}".Trim()
        };
    }

    public Order MapToModel(OrdersQryViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model. " +
            "Use Order model directly for create/update operations.");
    }
}
```

### Template for Category E (Aggregate)

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Summary of Sales by Year view to SummaryOfSalesByYearViewModel.
/// This aggregate view provides total sales amounts grouped by year.
/// </summary>
public class SummaryOfSalesByYearMapper : IMapper<SalesAggregate, SummaryOfSalesByYearViewModel>
{
    public SummaryOfSalesByYearViewModel MapToViewModel(SalesAggregate model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new SummaryOfSalesByYearViewModel
        {
            Year = model.Year ?? 0,
            SaleAmount = model.SaleAmount
        };
    }

    public SalesAggregate MapToModel(SummaryOfSalesByYearViewModel viewModel)
    {
        throw new NotSupportedException(
            "Aggregate view data is read-only and cannot be mapped to a data model. " +
            "Aggregate data is calculated from order details.");
    }
}
```

## Step 4: Register Mappers

Update `src/NorthwindAspire.Frontend/Models/Mappers/MapperRegistry.cs`:

```csharp
public static IServiceCollection AddMappers(this IServiceCollection services)
{
    // Existing mappers...
    services.AddScoped<IMapper<Category, CategoryViewModel>, CategoryMapper>();
    
    // Add view mappers - Category A (Direct Product)
    services.AddScoped<IMapper<Product, AlphabeticalListOfProductsViewModel>, AlphabeticalListOfProductsMapper>();
    services.AddScoped<IMapper<Product, CurrentProductListViewModel>, CurrentProductListMapper>();
    services.AddScoped<IMapper<Product, ProductsAboveAveragePriceViewModel>, ProductsAboveAveragePriceMapper>();
    
    // Add view mappers - Category B (Product with Category)
    services.AddScoped<IMapper<Product, ProductsByCategoryViewModel>, ProductsByCategoryMapper>();
    
    // Add view mappers - Category C (Order-based)
    services.AddScoped<IMapper<Order, OrdersQryViewModel>, OrdersQryMapper>();
    services.AddScoped<IMapper<Order, QuarterlyOrdersViewModel>, QuarterlyOrdersMapper>();
    
    // Add view mappers - Category D (OrderDetail Denormalized)
    services.AddScoped<IMapper<OrderDetail, InvoicesViewModel>, InvoicesMapper>();
    services.AddScoped<IMapper<OrderDetail, OrderDetailsExtendedViewModel>, OrderDetailsExtendedMapper>();
    
    // Add view mappers - Category E (Aggregate)
    services.AddScoped<IMapper<OrderSubtotal, OrderSubtotalViewModel>, OrderSubtotalMapper>();
    services.AddScoped<IMapper<SalesAggregate, SummaryOfSalesByYearViewModel>, SummaryOfSalesByYearMapper>();
    services.AddScoped<IMapper<SalesAggregate, SummaryOfSalesByQuarterViewModel>, SummaryOfSalesByQuarterMapper>();
    services.AddScoped<IMapper<SalesAggregate, SalesTotalsByAmountViewModel>, SalesTotalsByAmountMapper>();
    services.AddScoped<IMapper<CategorySalesAggregate, ProductSalesFor1997ViewModel>, ProductSalesFor1997Mapper>();
    services.AddScoped<IMapper<CategorySalesAggregate, CategorySalesFor1997ViewModel>, CategorySalesFor1997Mapper>();
    services.AddScoped<IMapper<CategorySalesAggregate, SalesByCategoryViewModel>, SalesByCategoryMapper>();
    services.AddScoped<IMapper<CustomerSupplierByCity, CustomerSupplierByCityViewModel>, CustomerSupplierByCityMapper>();
    
    return services;
}
```

## Step 5: Write Unit Tests

Create test file in `NorthwindAspire.Tests/Mappers/` for each mapper:

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.Mappers;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Tests.Mappers;

[TestFixture]
public class OrdersQryMapperTests
{
    private OrdersQryMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new OrdersQryMapper();
    }

    [Test]
    public void MapToViewModel_WithValidOrder_ReturnsCorrectViewModel()
    {
        // Arrange
        var order = new Order
        {
            OrderId = 1,
            CustomerId = "ALFKI",
            EmployeeId = 1,
            OrderDate = new DateTime(1996, 7, 4),
            Customer = new Customer { CompanyName = "Alfreds Futterkiste" },
            Employee = new Employee { FirstName = "Nancy", LastName = "Davolio" }
        };

        // Act
        var result = _mapper.MapToViewModel(order);

        // Assert
        Assert.That(result.OrderId, Is.EqualTo(1));
        Assert.That(result.CustomerId, Is.EqualTo("ALFKI"));
        Assert.That(result.CompanyName, Is.EqualTo("Alfreds Futterkiste"));
        Assert.That(result.Salesman, Is.EqualTo("Nancy Davolio"));
    }

    [Test]
    public void MapToViewModel_WithNullCustomer_HandlesGracefully()
    {
        // Arrange
        var order = new Order { OrderId = 1, Customer = null, Employee = null };

        // Act
        var result = _mapper.MapToViewModel(order);

        // Assert
        Assert.That(result.CompanyName, Is.EqualTo(string.Empty));
        Assert.That(result.Salesman, Is.EqualTo(string.Empty));
    }

    [Test]
    public void MapToViewModel_WithNullModel_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mapper.MapToViewModel(null));
    }

    [Test]
    public void MapToModel_ThrowsNotSupportedException()
    {
        // Arrange
        var viewModel = new OrdersQryViewModel();

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => _mapper.MapToModel(viewModel));
    }
}
```

## Implementation Checklist

For each view mapper, verify:

- [ ] ViewModel file created in `Models/ViewModels/`
- [ ] Mapper file created in `Models/Mappers/`
- [ ] Mapper implements `IMapper<TModel, TViewModel>`
- [ ] MapToViewModel handles null references with `?.` or `?? default`
- [ ] MapToModel throws `NotSupportedException`
- [ ] XML documentation added
- [ ] Mapper registered in MapperRegistry
- [ ] Unit tests created
- [ ] All tests pass
- [ ] Build successful

## Common Pitfalls to Avoid

1. ? Not handling null references ? Use `?.` and `?? ""`
2. ? Forgetting to register mapper ? Add to MapperRegistry
3. ? Wrong property names ? Check view SQL for exact column names
4. ? Missing validation attributes ? Add [Required], [StringLength], etc.
5. ? Not testing null cases ? Include null object and null property tests
6. ? Forgetting to cast quantities ? Use decimal for currency calculations

## Additional Resources

- `VIEWS_VIEWMODELS_AND_MAPPERS.md` - Complete mapping patterns
- `src/NorthwindAspire.Backend/VIEWS_SQL_DOCUMENTATION.md` - View documentation
- `src/NorthwindAspire.Frontend/VIEWMODELS_AND_MAPPERS.md` - ViewModel patterns
- Existing mappers - CategoryMapper, ProductMapper, etc.
