# Database Views: ViewModel and Mapper Generation Guide

This document provides instructions for generating ViewModels and Mappers for the Northwind database views listed in `src/NorthwindAspire.Backend/Northwind.md`.

## Overview

The Northwind database includes 16 predefined views that provide curated data for common reporting and query scenarios. This guide walks through creating ViewModels and corresponding Mappers for each view, following the established patterns in the NorthwindAspire project.

## Database Views List

The following views are available in the Northwind database with their mapped model classes:

1. **Alphabetical list of products** - Products sorted alphabetically → `Product`
2. **Current Product List** - All active (not discontinued) products → `Product`
3. **Customer and Suppliers by City** - Customers and suppliers grouped by city → `Customer` / `Supplier`
4. **Invoices** - Order data with invoice details → `Order`, `OrderDetail`, `Product`
5. **Orders Qry** - Order query with related customer and employee information → `Order`, `Customer`, `Employee`
6. **Order Subtotals** - Order data with calculated subtotals → `OrderDetail` (aggregate)
7. **Product Sales for 1997** - Product sales aggregated for 1997 → `Product`, `Category` (aggregate)
8. **Products Above Average Price** - Products priced above the average → `Product`
9. **Products by Category** - Products grouped by category → `Product`, `Category`
10. **Quarterly Orders** - Orders grouped by quarter → `Order`, `Customer`
11. **Sales Totals by Amount** - Sales aggregated by total amount → `Order`, `OrderDetail` (aggregate)
12. **Summary of Sales by Quarter** - Sales summary data by quarter → `OrderDetail` (aggregate)
13. **Summary of Sales by Year** - Sales summary data by year → `OrderDetail` (aggregate)
14. **Category Sales for 1997** - Category sales aggregated for 1997 → `Category`, `OrderDetail` (aggregate)
15. **Order Details Extended** - Order details with extended pricing information → `OrderDetail`, `Product`
16. **Sales by Category** - Sales data organized by category → `Category`, `Product`, `OrderDetail`

## Architecture Pattern

The project follows this structure for ViewModels and Mappers:

```
Frontend/
├── Models/
│   ├── ViewModels/
│   │   ├── IViewModelBase.cs
│   │   ├── [Entity]ViewModel.cs
│   │   └── [View]ViewModel.cs
│   └── Mappers/
│       ├── IMapper.cs
│       ├── [Entity]Mapper.cs
│       ├── [View]Mapper.cs
│       └── MapperRegistry.cs
```

## Step-by-Step Instructions

### Step 1: Create ViewModel Classes

Create a new file for each view's ViewModel in `src/NorthwindAspire.Frontend/Models/ViewModels/`.

**Naming Convention:** `[ViewName]ViewModel.cs` (PascalCase, spaces removed, "View" suffix removed if already in name)

**Example Template:**

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class [ViewNameHere]ViewModel
{
    [Required]
    public [DataType] [PropertyName] { get; set; }
    
    public [DataType] [PropertyName] { get; set; } = [DefaultValue];
    
    // Add validation attributes as needed
    [StringLength(100)]
    [Range(0, int.MaxValue)]
    public [DataType] [PropertyName] { get; set; }
}
```

### Step 2: Determine View Schema

Before creating mappers, examine the view schema in `src/create.sql` or run a query against the database to determine:

1. Column names and data types
2. Which properties are required vs. optional
3. Any computed/derived properties

**Example:** For "Current Product List" view:
- ProductID (int) - Required, PK
- ProductName (string) - Required
- SupplierID (int) - Optional, FK
- CategoryID (int) - Optional, FK
- QuantityPerUnit (string) - Optional
- UnitPrice (decimal) - Optional
- UnitsInStock (int) - Optional
- Discontinued (bool) - Optional

### Step 3: Create Mapper Classes

Create a new file for each view's Mapper in `src/NorthwindAspire.Frontend/Models/Mappers/`.

**Naming Convention:** `[ViewName]Mapper.cs`

**Important Considerations:**

- View data typically maps FROM backend data models
- Views are often read-only, so MapToModel might throw NotSupportedException
- Use the `IMapper<TModel, TViewModel>` interface
- Handle null reference checks
- Map related entities appropriately

**Example Template for Read-Only View:**

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class [ViewName]Mapper : IMapper<[BackendModel], [ViewName]ViewModel>
{
    public [ViewName]ViewModel MapToViewModel([BackendModel] model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new [ViewName]ViewModel
        {
            PropertyOne = model.PropertyOne,
            PropertyTwo = model.PropertyTwo,
            PropertyThree = model.PropertyThree
        };
    }

    public [BackendModel] MapToModel([ViewName]ViewModel viewModel)
    {
        throw new NotSupportedException(
            $"View data is read-only and cannot be mapped to a data model");
    }
}
```

**Example Template for Aggregate View:**

For views that aggregate data from multiple tables, you may need to create a specialized data model or use an intermediate data structure:

```csharp
public class SalesAggregateMapper : IMapper<SalesAggregate, SalesAggregateViewModel>
{
    public SalesAggregateViewModel MapToViewModel(SalesAggregate model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new SalesAggregateViewModel
        {
            Year = model.Year,
            Quarter = model.Quarter,
            TotalSales = model.TotalSales,
            OrderCount = model.OrderCount
        };
    }

    public SalesAggregate MapToModel(SalesAggregateViewModel viewModel)
    {
        throw new NotSupportedException(
            $"Aggregate view data is read-only and cannot be mapped to a data model");
    }
}
```

### Step 4: Register Mappers in MapperRegistry

Update `src/NorthwindAspire.Frontend/Models/Mappers/MapperRegistry.cs`:

```csharp
public static class MapperRegistry
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        // Existing registrations...
        services.AddScoped<IMapper<Category, CategoryViewModel>, CategoryMapper>();
        
        // New view mappers
        services.AddScoped<IMapper<[DataModel], [ViewName]ViewModel>, [ViewName]Mapper>();
        
        return services;
    }
}
```

### Step 5: Add Validation Attributes

Apply appropriate validation attributes to ViewModel properties:

```csharp
[Required(ErrorMessage = "ProductId is required")]
[Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
public int ProductId { get; set; }

[Required(ErrorMessage = "ProductName is required")]
[StringLength(40, MinimumLength = 1, ErrorMessage = "ProductName must be between 1 and 40 characters")]
public string ProductName { get; set; } = string.Empty;

[Range(0, double.MaxValue, ErrorMessage = "UnitPrice must be non-negative")]
public decimal? UnitPrice { get; set; }
```

## View-Specific Implementation Notes

### View Mapper Implementation Patterns

#### Pattern 1: Single Entity Views (Product-based)

**Views:** Alphabetical list of products, Current Product List, Products Above Average Price, Products by Category

**Mapping:** These views map directly to the `Product` model class.

```csharp
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
            QuantityPerUnit = model.QuantityPerUnit,
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
            $"View data is read-only and cannot be mapped to a data model");
    }
}
```

#### Pattern 2: Multi-Entity Denormalized Views

**Views:** Orders Qry, Customer and Suppliers by City

**Mapping:** These views combine data from multiple entities (Order + Customer + Employee) or (Customer + Supplier). Create a denormalized ViewModel.

```csharp
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
            // Employee details
            Salesman = $"{model.Employee?.FirstName} {model.Employee?.LastName}".Trim()
        };
    }

    public Order MapToModel(OrdersQryViewModel viewModel)
    {
        throw new NotSupportedException(
            $"View data is read-only and cannot be mapped to a data model");
    }
}
```

#### Pattern 3: Invoice/Complex Detail Views

**Views:** Invoices, Order Details Extended

**Mapping:** These views denormalize Order with OrderDetails and related entities. Requires mapping collections or flattening for each line item.

```csharp
public class InvoicesMapper : IMapper<OrderDetail, InvoicesViewModel>
{
    // Maps from OrderDetail (which includes Order, Product, Customer, Employee, Shipper)
    public InvoicesViewModel MapToViewModel(OrderDetail model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var order = model.Order;
        var customer = order?.Customer;
        var employee = order?.Employee;
        var shipper = order?.Shipper;

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
            ExtendedPrice = model.UnitPrice * model.Quantity * (1 - model.Discount)
        };
    }

    public OrderDetail MapToModel(InvoicesViewModel viewModel)
    {
        throw new NotSupportedException(
            $"View data is read-only and cannot be mapped to a data model");
    }
}
```

#### Pattern 4: Aggregate/Summary Views

**Views:** Order Subtotals, Product Sales for 1997, Summary of Sales by Quarter/Year, Category Sales for 1997, Sales Totals by Amount

**Mapping:** These views require intermediate data models since they aggregate data from multiple OrderDetails. The view result doesn't directly map to a single entity.

**Solution:** Create intermediate DTO classes in the Backend.Models namespace:

```csharp
// In NorthwindAspire.Backend.Models:
public class OrderSubtotal
{
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
}

public class SalesAggregate
{
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public decimal SaleAmount { get; set; }
}

public class CategorySalesAggregate
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductSales { get; set; }
}
```

```csharp
public class OrderSubtotalMapper : IMapper<OrderSubtotal, OrderSubtotalViewModel>
{
    public OrderSubtotalViewModel MapToViewModel(OrderSubtotal model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new OrderSubtotalViewModel
        {
            OrderId = model.OrderId,
            Subtotal = model.Subtotal
        };
    }

    public OrderSubtotal MapToModel(OrderSubtotalViewModel viewModel)
    {
        throw new NotSupportedException(
            $"Aggregate view data is read-only and cannot be mapped to a data model");
    }
}
```

### Implementation Guidelines by View Type

| View Type | Example Views | Mapping Strategy | Challenges |
|-----------|--------------|-----------------|-----------|
| **Simple Entity** | Alphabetical list, Current Product List | Map directly from Product | None |
| **Filtered Entity** | Products Above Average Price | Map from Product with filter | Handle calculated average |
| **Related Entities** | Orders Qry, Products by Category | Denormalize to include related data | Null reference handling |
| **Complex Multi-Entity** | Invoices | Map from OrderDetail with navigation properties | Complex navigation chains |
| **Aggregate** | Summary of Sales by Quarter | Create intermediate DTO | Calculate aggregates separately |

### Null Reference Handling

Always use null-coalescing operators for foreign key relationships:

```csharp
// Instead of:
FirstName = model.Employee.FirstName,

// Use:
FirstName = model.Employee?.FirstName ?? string.Empty,

// Or for nullable types:
BirthDate = model.Employee?.BirthDate
```

### Read-Only View Notes

For read-only views:
- Set `MapToModel` to throw `NotSupportedException`
- Include comment explaining why the view is read-only
- Consider using `[DisplayFormat(NullDisplayText = "N/A")]` for optional fields

## Detailed View-to-Model Mapping Reference

### View Mapping Summary

| # | View Name | Source Model(s) | Mapper Class | View Model Class | Notes |
|---|-----------|-----------------|--------------|------------------|-------|
| 1 | Alphabetical list of products | `Product` | `AlphabeticalListOfProductsMapper` | `AlphabeticalListOfProductsViewModel` | Direct mapping, order by name |
| 2 | Current Product List | `Product` | `CurrentProductListMapper` | `CurrentProductListViewModel` | Filter: Discontinued = 0 |
| 3 | Customer and Suppliers by City | `Customer` / `Supplier` | `CustomerSupplierByCityMapper` | `CustomerSupplierByCityViewModel` | Union of two entities |
| 4 | Invoices | `OrderDetail` (with Order, Customer, Employee, Shipper, Product) | `InvoicesMapper` | `InvoicesViewModel` | Complex denormalized view |
| 5 | Orders Qry | `Order` (with Customer, Employee) | `OrdersQryMapper` | `OrdersQryViewModel` | Denormalized order view |
| 6 | Order Subtotals | `OrderSubtotal` (intermediate DTO) | `OrderSubtotalMapper` | `OrderSubtotalViewModel` | Aggregate from OrderDetail |
| 7 | Product Sales for 1997 | `CategorySalesAggregate` (intermediate DTO) | `ProductSalesFor1997Mapper` | `ProductSalesFor1997ViewModel` | Filter by year, grouped by category |
| 8 | Products Above Average Price | `Product` | `ProductsAboveAveragePriceMapper` | `ProductsAboveAveragePriceViewModel` | Filter: UnitPrice > AVG(UnitPrice) |
| 9 | Products by Category | `Product` (with Category) | `ProductsByCategoryMapper` | `ProductsByCategoryViewModel` | Joined with Category, active only |
| 10 | Quarterly Orders | `Order` (with Customer) | `QuarterlyOrdersMapper` | `QuarterlyOrdersViewModel` | Recent quarters, distinct customers |
| 11 | Sales Totals by Amount | `SalesAggregate` (intermediate DTO) | `SalesTotalsByAmountMapper` | `SalesTotalsByAmountViewModel` | Filter: Subtotal > 2500 |
| 12 | Summary of Sales by Quarter | `SalesAggregate` (intermediate DTO) | `SummaryOfSalesByQuarterMapper` | `SummaryOfSalesByQuarterViewModel` | Grouped by year and quarter |
| 13 | Summary of Sales by Year | `SalesAggregate` (intermediate DTO) | `SummaryOfSalesByYearMapper` | `SummaryOfSalesByYearViewModel` | Grouped by year |
| 14 | Category Sales for 1997 | `CategorySalesAggregate` (intermediate DTO) | `CategorySalesFor1997Mapper` | `CategorySalesFor1997ViewModel` | Filter by year, grouped by category |
| 15 | Order Details Extended | `OrderDetail` (with Product) | `OrderDetailsExtendedMapper` | `OrderDetailsExtendedViewModel` | Includes calculated ExtendedPrice |
| 16 | Sales by Category | `CategorySalesAggregate` (intermediate DTO) | `SalesByCategoryMapper` | `SalesByCategoryViewModel` | Grouped by category and product |

### Intermediate DTO Classes Required

For aggregate and summary views, create these intermediate DTO classes in `src/NorthwindAspire.Backend/Models/NorthwindModels.cs`:

```csharp
// ============================================================================
// Intermediate DTOs for View Mapping
// ============================================================================

/// <summary>
/// Intermediate DTO for Order Subtotals view
/// </summary>
public class OrderSubtotal
{
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
}

/// <summary>
/// Intermediate DTO for sales aggregates (by quarter, year, etc.)
/// </summary>
public class SalesAggregate
{
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public decimal SaleAmount { get; set; }
}

/// <summary>
/// Intermediate DTO for category-based sales aggregates
/// </summary>
public class CategorySalesAggregate
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductSales { get; set; }
}

/// <summary>
/// Intermediate DTO for customer/supplier by city view
/// </summary>
public class CustomerSupplierByCity
{
    public string City { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty; // "Customers" or "Suppliers"
}
```

### Mapper Implementation Checklist

For each view mapper, ensure you:

- ✅ **Map all view columns** to corresponding ViewModel properties
- ✅ **Handle null references** using `?.` null-coalescing operator or `?? default`
- ✅ **Calculate derived fields** (e.g., ExtendedPrice, Salesman full name)
- ✅ **Apply appropriate formatting** (e.g., concatenate FirstName + LastName for Salesman)
- ✅ **Filter by status** (e.g., Discontinued = 0 for product lists)
- ✅ **Handle date conversions** if needed (SQLite → C# DateTime)
- ✅ **Set MapToModel to throw NotSupportedException** for read-only views
- ✅ **Add XML documentation** describing the mapper's purpose and view dependencies

### Example: Complete View Mapper Implementation

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Maps the Order Details Extended view to OrderDetailsExtendedViewModel.
/// This view includes calculated ExtendedPrice (UnitPrice × Quantity × (1 - Discount)).
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
}
```

## Testing Considerations

For each ViewModel/Mapper pair:

1. **Unit Tests:** Test mapper null handling and property mapping
2. **Integration Tests:** Verify data flows correctly from view to ViewModel
3. **Validation Tests:** Confirm validation attributes work as expected

**Example Test:**

```csharp
[Test]
public void MapToViewModel_WithValidData_ReturnsCorrectViewModel()
{
    // Arrange
    var model = new Product { 
        ProductId = 1, 
        ProductName = "Test Product",
        UnitPrice = 9.99m
    };
    var mapper = new ProductMapper();

    // Act
    var result = mapper.MapToViewModel(model);

    // Assert
    Assert.AreEqual(1, result.ProductId);
    Assert.AreEqual("Test Product", result.ProductName);
    Assert.AreEqual(9.99m, result.UnitPrice);
}
```

## Additional Resources

- **IMapper Interface:** See `src/NorthwindAspire.Frontend/Models/Mappers/IMapper.cs`
- **Existing Implementations:** Review existing mappers (CategoryMapper, ProductMapper, etc.)
- **View Definitions:** Check `src/create.sql` for view SQL definitions
- **Database Schema:** Reference `src/NorthwindAspire.Backend/Northwind.md` for entity relationships

## Next Steps

1. Prioritize views based on UI requirements
2. Create ViewModels following the established patterns
3. Implement Mappers with proper null handling and validation
4. Register mappers in MapperRegistry
5. Add unit/integration tests
6. Create OData query endpoints if needed for UI consumption
7. Document any view-specific business logic or dependencies

## Quick Reference: Mapper Implementation Priority

### High Priority (Core Business Functionality)
1. **Orders Qry** - Essential for order management
2. **Invoices** - Critical for billing and reporting
3. **Order Details Extended** - Needed for order line items
4. **Summary of Sales by Year** - Annual reporting
5. **Current Product List** - Product catalog

### Medium Priority (Reporting & Analysis)
6. **Summary of Sales by Quarter** - Quarterly business review
7. **Product Sales for 1997** - Historical analysis
8. **Sales by Category** - Category performance
9. **Products by Category** - Product organization
10. **Sales Totals by Amount** - High-value order identification

### Lower Priority (Supporting/Specialized)
11. **Alphabetical list of products** - Alternative product browsing
12. **Products Above Average Price** - Pricing analysis
13. **Category Sales for 1997** - Historical category performance
14. **Quarterly Orders** - Recent customer activity
15. **Order Subtotals** - Component of other views
16. **Customer and Suppliers by City** - Location-based lookup

## Implementation Workflow

1. **Create Intermediate DTOs** (if needed)
   - Add to `src/NorthwindAspire.Backend/Models/NorthwindModels.cs`
   - Create only for aggregate/denormalized views

2. **Create ViewModels**
   - Create in `src/NorthwindAspire.Frontend/Models/ViewModels/`
   - Follow naming convention: `[ViewName]ViewModel.cs`
   - Add appropriate validation attributes

3. **Create Mappers**
   - Create in `src/NorthwindAspire.Frontend/Models/Mappers/`
   - Follow naming convention: `[ViewName]Mapper.cs`
   - Implement `IMapper<TModel, TViewModel>` interface

4. **Register Mappers**
   - Add to `src/NorthwindAspire.Frontend/Models/Mappers/MapperRegistry.cs`
   - Use `AddScoped<IMapper<TModel, TViewModel>, MapperClass>()`

5. **Create Tests**
   - Add unit tests in `NorthwindAspire.Tests/Mappers/`
   - Test null handling and property mapping

## Code Generation Tips

### Using Entity Framework with Views

To leverage EF Core with views:

```csharp
// In NorthwindContext.cs
public DbSet<OrderSubtotal> OrderSubtotals { get; set; }
public DbSet<SalesAggregate> SalesAggregates { get; set; }
public DbSet<CategorySalesAggregate> CategorySalesAggregates { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Map DTOs to views
    modelBuilder.Entity<OrderSubtotal>()
        .HasNoKey()
        .ToView("Order Subtotals");
    
    modelBuilder.Entity<SalesAggregate>()
        .HasNoKey()
        .ToView("Summary of Sales by Year");
    
    // ... etc
}
```

### Batch Mapper Generation

For multiple mappers with similar structure, create a template:

```powershell
# PowerShell script to generate mapper boilerplate
$views = @(
    @{ Name = "AlphabeticalListOfProducts"; Model = "Product" },
    @{ Name = "CurrentProductList"; Model = "Product" },
    # ... etc
)

foreach ($view in $views) {
    $content = @"
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class $($view.Name)Mapper : IMapper<$($view.Model), $($view.Name)ViewModel>
{
    public $($view.Name)ViewModel MapToViewModel($($view.Model) model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new $($view.Name)ViewModel
        {
            // TODO: Map properties
        };
    }

    public $($view.Model) MapToModel($($view.Name)ViewModel viewModel)
    {
        throw new NotSupportedException(
            "View data is read-only and cannot be mapped to a data model");
    }
}
"@
    
    $content | Out-File -FilePath "src/NorthwindAspire.Frontend/Models/Mappers/$($view.Name)Mapper.cs"
}
```

## Related Documentation

- `src/NorthwindAspire.Frontend/VIEWMODELS_AND_MAPPERS.md` - Detailed ViewModel/Mapper architecture
- `src/NorthwindAspire.Backend/IMPLEMENTATION_SUMMARY.md` - Backend implementation details
- `TESTING_IMPLEMENTATION_SUMMARY.md` - Testing patterns and practices
- `src/NorthwindAspire.Backend/create_views.sql` - SQL view definitions
- `src/NorthwindAspire.Backend/VIEWS_SQL_DOCUMENTATION.md` - View documentation and purposes
