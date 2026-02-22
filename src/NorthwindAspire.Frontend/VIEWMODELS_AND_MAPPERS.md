# ViewModel and Mapper Implementation Guide for Blazor Server Frontend

## Overview

This guide provides comprehensive instructions for creating ViewModel classes and mapper classes in the NorthwindAspire Frontend Blazor Server application. These mappers will handle the conversion between backend OData API models and frontend ViewModels.

## Table of Contents

1. [Directory Structure](#directory-structure)
2. [ViewModel Classes](#viewmodel-classes)
3. [Mapper Classes](#mapper-classes)
4. [Implementation Examples](#implementation-examples)
5. [Integration with Blazor Components](#integration-with-blazor-components)

---

## Directory Structure

Create the following directory structure in the frontend project:

```
src/NorthwindAspire.Frontend/
??? Models/
?   ??? ViewModels/
?   ?   ??? CategoryViewModel.cs
?   ?   ??? CustomerViewModel.cs
?   ?   ??? EmployeeViewModel.cs
?   ?   ??? OrderViewModel.cs
?   ?   ??? OrderDetailViewModel.cs
?   ?   ??? ProductViewModel.cs
?   ?   ??? SupplierViewModel.cs
?   ?   ??? ShipperViewModel.cs
?   ?   ??? RegionViewModel.cs
?   ?   ??? TerritoryViewModel.cs
?   ?   ??? EmployeeTerritoryViewModel.cs
?   ??? Mappers/
?       ??? CategoryMapper.cs
?       ??? CustomerMapper.cs
?       ??? EmployeeMapper.cs
?       ??? OrderMapper.cs
?       ??? OrderDetailMapper.cs
?       ??? ProductMapper.cs
?       ??? SupplierMapper.cs
?       ??? ShipperMapper.cs
?       ??? RegionMapper.cs
?       ??? TerritoryMapper.cs
?       ??? EmployeeTerritoryMapper.cs
?       ??? MapperRegistry.cs
```

---

## ViewModel Classes

### Purpose

ViewModels serve as the data binding layer for Blazor components. They:
- Decouple UI from backend models
- Provide UI-friendly data representations
- Include validation attributes
- Support bi-directional data binding

### Key Principles

1. **Flatten Nested Objects**: Convert complex navigation properties to simple properties or IDs
2. **Use Simple Types**: Prefer primitive types and avoid circular references
3. **Add Validation**: Include `System.ComponentModel.DataAnnotations` attributes
4. **Make Collections Serializable**: Use `List<T>` instead of `ICollection<T>`
5. **Include Necessary Display Properties**: Add computed or display-friendly properties

### Example: CategoryViewModel

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class CategoryViewModel
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 1, ErrorMessage = "Category name must be between 1 and 15 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(int.MaxValue)]
    public string Description { get; set; } = string.Empty;

    public List<int> ProductIds { get; set; } = new();

    public int ProductCount => ProductIds?.Count ?? 0;
}
```

### Example: CustomerViewModel

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class CustomerViewModel
{
    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(30)]
    public string ContactName { get; set; } = string.Empty;

    [StringLength(30)]
    public string ContactTitle { get; set; } = string.Empty;

    [StringLength(60)]
    public string Address { get; set; } = string.Empty;

    [StringLength(15)]
    public string City { get; set; } = string.Empty;

    [StringLength(15)]
    public string Region { get; set; } = string.Empty;

    [StringLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(15)]
    public string Country { get; set; } = string.Empty;

    [Phone]
    [StringLength(24)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(24)]
    public string Fax { get; set; } = string.Empty;

    public List<int> OrderIds { get; set; } = new();

    public int OrderCount => OrderIds?.Count ?? 0;

    public string DisplayName => $"{CompanyName} ({ContactName})";
}
```

### Example: OrderViewModel

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class OrderViewModel
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    public string CustomerCompanyName { get; set; } = string.Empty;

    public int? EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime? OrderDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? RequiredDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    public string ShipperName { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    public decimal Freight { get; set; }

    [StringLength(40)]
    public string ShipName { get; set; } = string.Empty;

    [StringLength(60)]
    public string ShipAddress { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipCity { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipRegion { get; set; } = string.Empty;

    [StringLength(10)]
    public string ShipPostalCode { get; set; } = string.Empty;

    [StringLength(15)]
    public string ShipCountry { get; set; } = string.Empty;

    public List<OrderDetailViewModel> OrderDetails { get; set; } = new();

    public decimal OrderTotal => OrderDetails?.Sum(od => od.LineTotal) ?? 0;

    public bool IsShipped => ShippedDate.HasValue;
}
```

### Example: ProductViewModel

```csharp
using System.ComponentModel.DataAnnotations;

namespace NorthwindAspire.Frontend.Models.ViewModels;

public class ProductViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [StringLength(20)]
    public string QuantityPerUnit { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsInStock { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitsOnOrder { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public bool IsAvailable => UnitsInStock > 0 && !Discontinued;

    public bool RequiresReorder => UnitsInStock <= ReorderLevel;
}
```

---

## Mapper Classes

### Purpose

Mappers handle the bi-directional conversion between backend models and ViewModels.

### Design Pattern: Two-Way Mapper

Each mapper should provide:
- `MapToViewModel()` - Backend model ? ViewModel
- `MapToModel()` - ViewModel ? Backend model

### Base Mapper Interface

```csharp
namespace NorthwindAspire.Frontend.Models.Mappers;

public interface IMapper<TModel, TViewModel>
{
    TViewModel MapToViewModel(TModel model);
    TModel MapToModel(TViewModel viewModel);
    IEnumerable<TViewModel> MapToViewModelList(IEnumerable<TModel> models);
    IEnumerable<TModel> MapToModelList(IEnumerable<TViewModel> viewModels);
}
```

### Example: CategoryMapper

```csharp
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class CategoryMapper : IMapper<Category, CategoryViewModel>
{
    public CategoryViewModel MapToViewModel(Category model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new CategoryViewModel
        {
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            Description = model.Description,
            ProductIds = model.Products?.Select(p => p.ProductId).ToList() ?? new()
        };
    }

    public Category MapToModel(CategoryViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Category
        {
            CategoryId = viewModel.CategoryId,
            CategoryName = viewModel.CategoryName,
            Description = viewModel.Description
        };
    }

    public IEnumerable<CategoryViewModel> MapToViewModelList(IEnumerable<Category> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<CategoryViewModel>();
    }

    public IEnumerable<Category> MapToModelList(IEnumerable<CategoryViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Category>();
    }
}
```

### Example: CustomerMapper

```csharp
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
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<CustomerViewModel>();
    }
}
```

### Example: OrderMapper

```csharp
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
```

### Example: ProductMapper

```csharp
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class ProductMapper : IMapper<Product, ProductViewModel>
{
    public ProductViewModel MapToViewModel(Product model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new ProductViewModel
        {
            ProductId = model.ProductId,
            ProductName = model.ProductName,
            SupplierId = model.SupplierId,
            SupplierName = model.Supplier?.CompanyName ?? string.Empty,
            CategoryId = model.CategoryId,
            CategoryName = model.Category?.CategoryName ?? string.Empty,
            QuantityPerUnit = model.QuantityPerUnit,
            UnitPrice = model.UnitPrice,
            UnitsInStock = model.UnitsInStock,
            UnitsOnOrder = model.UnitsOnOrder,
            ReorderLevel = model.ReorderLevel,
            Discontinued = model.Discontinued
        };
    }

    public Product MapToModel(ProductViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Product
        {
            ProductId = viewModel.ProductId,
            ProductName = viewModel.ProductName,
            SupplierId = viewModel.SupplierId,
            CategoryId = viewModel.CategoryId,
            QuantityPerUnit = viewModel.QuantityPerUnit,
            UnitPrice = viewModel.UnitPrice,
            UnitsInStock = viewModel.UnitsInStock,
            UnitsOnOrder = viewModel.UnitsOnOrder,
            ReorderLevel = viewModel.ReorderLevel,
            Discontinued = viewModel.Discontinued
        };
    }

    public IEnumerable<ProductViewModel> MapToViewModelList(IEnumerable<Product> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<ProductViewModel>();
    }

    public IEnumerable<Product> MapToModelList(IEnumerable<ProductViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Product>();
    }
}
```

---

## Mapper Registry Pattern

Create a centralized registry to manage mapper instances:

```csharp
namespace NorthwindAspire.Frontend.Models.Mappers;

public class MapperRegistry
{
    private readonly Dictionary<Type, object> _mappers = new();

    public MapperRegistry()
    {
        RegisterDefaultMappers();
    }

    private void RegisterDefaultMappers()
    {
        Register<Category, CategoryViewModel>(new CategoryMapper());
        Register<Customer, CustomerViewModel>(new CustomerMapper());
        Register<Product, ProductViewModel>(new ProductMapper());
        Register<Order, OrderViewModel>(new OrderMapper());
        Register<OrderDetail, OrderDetailViewModel>(new OrderDetailMapper());
        Register<Employee, EmployeeViewModel>(new EmployeeMapper());
        Register<Supplier, SupplierViewModel>(new SupplierMapper());
        Register<Shipper, ShipperViewModel>(new ShipperMapper());
    }

    public void Register<TModel, TViewModel>(IMapper<TModel, TViewModel> mapper)
    {
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
        throw new InvalidOperationException($"Mapper for {typeof(TModel).Name} -> {typeof(TViewModel).Name} not registered");
    }
}
```

---

## Integration with Blazor Components

### Registering Mappers in Program.cs

Add the following to `src/NorthwindAspire.Frontend/Program.cs`:

```csharp
// Register mappers
builder.Services.AddSingleton<MapperRegistry>();
builder.Services.AddSingleton<CategoryMapper>();
builder.Services.AddSingleton<CustomerMapper>();
builder.Services.AddSingleton<ProductMapper>();
builder.Services.AddSingleton<OrderMapper>();
builder.Services.AddSingleton<OrderDetailMapper>();
builder.Services.AddSingleton<EmployeeMapper>();
builder.Services.AddSingleton<SupplierMapper>();
builder.Services.AddSingleton<ShipperMapper>();
```

### Using Mappers in Services

Create a service layer to handle API calls and mapping:

```csharp
using NorthwindAspire.Frontend.Models.Mappers;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Services;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
    Task<ProductViewModel?> GetProductAsync(int id);
    Task<ProductViewModel> CreateProductAsync(ProductViewModel viewModel);
    Task UpdateProductAsync(ProductViewModel viewModel);
    Task DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ProductMapper _mapper;

    public ProductService(HttpClient httpClient, ProductMapper mapper)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
    {
        var response = await _httpClient.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
        
        var products = await response.Content.ReadAsAsync<List<Product>>();
        return _mapper.MapToViewModelList(products);
    }

    public async Task<ProductViewModel?> GetProductAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/products/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        var product = await response.Content.ReadAsAsync<Product>();
        return _mapper.MapToViewModel(product);
    }

    public async Task<ProductViewModel> CreateProductAsync(ProductViewModel viewModel)
    {
        var model = _mapper.MapToModel(viewModel);
        var response = await _httpClient.PostAsJsonAsync("/api/products", model);
        response.EnsureSuccessStatusCode();

        var createdProduct = await response.Content.ReadAsAsync<Product>();
        return _mapper.MapToViewModel(createdProduct);
    }

    public async Task UpdateProductAsync(ProductViewModel viewModel)
    {
        var model = _mapper.MapToModel(viewModel);
        var response = await _httpClient.PutAsJsonAsync($"/api/products/{viewModel.ProductId}", model);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/products/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

### Using Mappers in Razor Components

```razor
@page "/products"
@inject IProductService ProductService

<div class="container">
    <h1>Products</h1>
    
    @if (products == null)
    {
        <p>Loading...</p>
    }
    else
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Product Name</th>
                    <th>Category</th>
                    <th>Unit Price</th>
                    <th>Stock</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var product in products)
                {
                    <tr>
                        <td>@product.ProductName</td>
                        <td>@product.CategoryName</td>
                        <td>@product.UnitPrice.ToString("C")</td>
                        <td>@product.UnitsInStock</td>
                        <td>
                            @if (product.IsAvailable)
                            {
                                <span class="badge bg-success">Available</span>
                            }
                            else
                            {
                                <span class="badge bg-danger">Unavailable</span>
                            }
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<ProductViewModel>? products;

    protected override async Task OnInitializedAsync()
    {
        products = (await ProductService.GetAllProductsAsync()).ToList();
    }
}
```

---

## Best Practices

### 1. **Null Safety**
- Always null-check inputs in mappers
- Use null-coalescing operators for default values
- Handle null navigation properties gracefully

### 2. **Immutability Considerations**
```csharp
// Use read-only properties for computed values
public int OrderCount => OrderIds?.Count ?? 0;
public decimal OrderTotal => OrderDetails?.Sum(od => od.LineTotal) ?? 0;
```

### 3. **Validation**
- Apply `DataAnnotations` for client-side validation
- Use `[Required]`, `[StringLength]`, `[Range]`, etc.
- Custom validation attributes for complex rules

### 4. **Separation of Concerns**
- Mappers handle model conversion only
- Services handle API communication
- Components handle UI logic
- ViewModels handle data binding

### 5. **Testing Mappers**

```csharp
[TestFixture]
public class CategoryMapperTests
{
    private CategoryMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new CategoryMapper();
    }

    [Test]
    public void MapToViewModel_WithValidModel_ReturnsCorrectViewModel()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "Beverages",
            Description = "Soft drinks, coffees, teas",
            Products = new List<Product>
            {
                new Product { ProductId = 1 },
                new Product { ProductId = 2 }
            }
        };

        // Act
        var result = _mapper.MapToViewModel(category);

        // Assert
        Assert.That(result.CategoryId, Is.EqualTo(1));
        Assert.That(result.CategoryName, Is.EqualTo("Beverages"));
        Assert.That(result.ProductIds.Count, Is.EqualTo(2));
    }

    [Test]
    public void MapToModel_WithValidViewModel_ReturnsCorrectModel()
    {
        // Arrange
        var viewModel = new CategoryViewModel
        {
            CategoryId = 1,
            CategoryName = "Beverages",
            Description = "Soft drinks, coffees, teas"
        };

        // Act
        var result = _mapper.MapToModel(viewModel);

        // Assert
        Assert.That(result.CategoryId, Is.EqualTo(1));
        Assert.That(result.CategoryName, Is.EqualTo("Beverages"));
    }
}
```

---

## Summary

This guide provides the foundation for implementing a clean, maintainable ViewModel and Mapper architecture in your Blazor Server frontend application. Key takeaways:

- **ViewModels** decouple the UI from backend models
- **Mappers** provide bi-directional conversion between models and ViewModels
- **Services** integrate mappers with API calls
- **Components** consume ViewModels through services
- **Testing** ensures mapper correctness and reliability

For more details on specific entity mappings, refer to the backend `NorthwindModels.cs` and model relationships defined in `NorthwindContext.cs`.
