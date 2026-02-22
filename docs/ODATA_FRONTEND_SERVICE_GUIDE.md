# Frontend OData Service Implementation Guide

## Overview

This guide provides instructions for creating a generic frontend service that makes OData API calls to the backend OData API and integrates with the existing `MapperRegistry` for automatic ViewModel/DataModel mapping. The service will support full CRUD operations with generics support.

## Architecture

The service will:
- Accept `HttpClient` configured with the backend API base URL
- Use `MapperRegistry` to automatically map between ViewModels and Data Models
- Provide generic methods for Create, Read (single), Read (list), Update, and Delete operations
- Use OData `$filter=eq` for single record retrieval by key
- Return properly typed ViewModels

## Service Implementation

### Step 1: Create the OData Frontend Service Interface

**File:** `src/NorthwindAspire.Frontend/Services/IODataFrontendService.cs`

```csharp
namespace NorthwindAspire.Frontend.Services;

public interface IODataFrontendService
{
    /// <summary>
    /// Retrieves a single object by its key property and returns it as a ViewModel.
    /// Uses OData $filter=eq filter on the key property.
    /// </summary>
    Task<TViewModel?> GetByKeyAsync<TModel, TViewModel>(object keyValue)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Retrieves all objects from a table/view and returns them as a List of ViewModels.
    /// </summary>
    Task<List<TViewModel>> GetAllAsync<TModel, TViewModel>()
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Creates a new object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API, and returns the created ViewModel.
    /// </summary>
    Task<TViewModel> CreateAsync<TModel, TViewModel>(TViewModel viewModel)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Updates an existing object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API.
    /// </summary>
    Task UpdateAsync<TModel, TViewModel>(object keyValue, TViewModel viewModel)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Deletes an object from the backend by its key property.
    /// </summary>
    Task DeleteAsync<TModel>(object keyValue)
        where TModel : class;
}
```

### Step 2: Create the OData Frontend Service Implementation

**File:** `src/NorthwindAspire.Frontend/Services/ODataFrontendService.cs`

```csharp
using System.Net.Http.Json;
using NorthwindAspire.Frontend.Models.Mappers;

namespace NorthwindAspire.Frontend.Services;

public class ODataFrontendService : IODataFrontendService
{
    private readonly HttpClient _httpClient;
    private readonly MapperRegistry _mapperRegistry;
    private readonly ILogger<ODataFrontendService> _logger;

    public ODataFrontendService(
        HttpClient httpClient,
        MapperRegistry mapperRegistry,
        ILogger<ODataFrontendService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _mapperRegistry = mapperRegistry ?? throw new ArgumentNullException(nameof(mapperRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a single object by its key property and returns it as a ViewModel.
    /// Uses OData $filter=eq filter on the key property.
    /// Example: /api/Categories?$filter=CategoryId eq 1
    /// </summary>
    public async Task<TViewModel?> GetByKeyAsync<TModel, TViewModel>(object keyValue)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var keyProperty = GetKeyPropertyName<TModel>();
            
            // Build OData filter: $filter=KeyProperty eq keyValue
            var filterValue = keyValue is string ? $"'{keyValue}'" : keyValue;
            var filter = $"$filter={keyProperty} eq {filterValue}";
            
            var url = $"/api/{entityName}?{filter}";
            _logger.LogInformation("Requesting: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsAsync<ODataCollectionResponse<TModel>>();
            
            if (result?.Value == null || result.Value.Count == 0)
            {
                _logger.LogWarning("No {EntityName} found with {KeyProperty} = {KeyValue}", 
                    entityName, keyProperty, keyValue);
                return null;
            }
            
            var model = result.Value[0];
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            return mapper.Map(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all objects from a table/view and returns them as a List of ViewModels.
    /// Example: /api/Categories
    /// </summary>
    public async Task<List<TViewModel>> GetAllAsync<TModel, TViewModel>()
        where TModel : class
        where TViewModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var url = $"/api/{entityName}";
            
            _logger.LogInformation("Requesting: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsAsync<ODataCollectionResponse<TModel>>();
            
            if (result?.Value == null || result.Value.Count == 0)
            {
                _logger.LogInformation("No {EntityName} records found", entityName);
                return new List<TViewModel>();
            }
            
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            return result.Value
                .Select(model => mapper.Map(model))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all {Entity} records", typeof(TModel).Name);
            throw;
        }
    }

    /// <summary>
    /// Creates a new object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API, 
    /// and returns the created ViewModel with any server-generated values.
    /// </summary>
    public async Task<TViewModel> CreateAsync<TModel, TViewModel>(TViewModel viewModel)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            
            var entityName = GetEntityName<TModel>();
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            
            // Map ViewModel to Data Model
            var model = MapViewModelToModel<TModel, TViewModel>(viewModel, mapper);
            
            var url = $"/api/{entityName}";
            _logger.LogInformation("Creating new {Entity}", entityName);
            
            var response = await _httpClient.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            
            var createdModel = await response.Content.ReadAsAsync<TModel>();
            
            // Map the created Data Model back to ViewModel
            return mapper.Map(createdModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Entity}", typeof(TModel).Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API.
    /// </summary>
    public async Task UpdateAsync<TModel, TViewModel>(object keyValue, TViewModel viewModel)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            
            var entityName = GetEntityName<TModel>();
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            
            // Map ViewModel to Data Model
            var model = MapViewModelToModel<TModel, TViewModel>(viewModel, mapper);
            
            var url = $"/api/{entityName}/{keyValue}";
            _logger.LogInformation("Updating {Entity} with key {KeyValue}", entityName, keyValue);
            
            var response = await _httpClient.PutAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Deletes an object from the backend by its key property.
    /// </summary>
    public async Task DeleteAsync<TModel>(object keyValue)
        where TModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var url = $"/api/{entityName}/{keyValue}";
            
            _logger.LogInformation("Deleting {Entity} with key {KeyValue}", entityName, keyValue);
            
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Gets the entity name from the model class name.
    /// Strips 'Model' suffix if present.
    /// Examples: Category -> Categories, Product -> Products, OrdersQry -> OrdersQries
    /// </summary>
    private string GetEntityName<TModel>() where TModel : class
    {
        var className = typeof(TModel).Name;
        
        // Remove 'Model' suffix if present
        if (className.EndsWith("Model"))
            className = className[..^5]; // Remove last 5 characters
        
        // Pluralize: simple approach, add 's' or 'es' based on ending
        return className.EndsWith("y") 
            ? $"{className[..^1]}ies" 
            : $"{className}s";
    }

    /// <summary>
    /// Gets the key property name for a model.
    /// Conventional: TypeName + "Id" (e.g., CategoryId, ProductId)
    /// </summary>
    private string GetKeyPropertyName<TModel>() where TModel : class
    {
        var className = typeof(TModel).Name;
        return $"{className}Id";
    }

    /// <summary>
    /// Maps a ViewModel to a Data Model using the mapper.
    /// This is a helper method that encapsulates the mapping logic.
    /// </summary>
    private TModel MapViewModelToModel<TModel, TViewModel>(TViewModel viewModel, IMapper<TModel, TViewModel> mapper)
        where TModel : class
        where TViewModel : class
    {
        // Note: Mappers in this architecture are typically one-way (Model -> ViewModel).
        // For two-way mapping, you may need to implement a reverse mapping interface
        // or use a mapping library like AutoMapper with bidirectional mapping.
        
        // Current approach: Create a new instance and manually copy properties
        // OR implement a MapReverse method in your mapper interfaces
        
        // For now, throw NotImplementedException to encourage proper mapper implementation
        throw new NotImplementedException(
            $"Reverse mapping from {typeof(TViewModel).Name} to {typeof(TModel).Name} not implemented. " +
            $"Consider implementing an IReverseMapper<TViewModel, TModel> interface or using bidirectional mapping.");
    }
}

/// <summary>
/// Represents the OData response collection format.
/// The OData API returns data in a collection with a 'value' property.
/// </summary>
public class ODataCollectionResponse<T>
{
    public List<T>? Value { get; set; }
}
```

### Step 3: Register the Service in Dependency Injection

**File:** `src/NorthwindAspire.Frontend/Program.cs`

Add the following to your service registration:

```csharp
// Register OData Frontend Service
builder.Services.AddHttpClient<ODataFrontendService>((sp, client) =>
{
    var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7001";
    client.BaseAddress = new Uri(backendUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IODataFrontendService>(sp => sp.GetRequiredService<ODataFrontendService>());
```

## Enhanced Mapper Architecture for Two-Way Mapping

To support Create and Update operations, enhance your mapper interfaces to support reverse mapping:

### Step 4: Create Reverse Mapper Interface (Optional)

**File:** `src/NorthwindAspire.Frontend/Models/Mappers/IReverseMapper.cs`

```csharp
namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Represents a bidirectional mapper between a ViewModel and a Data Model.
/// </summary>
public interface IReverseMapper<TViewModel, TModel> : IMapper<TModel, TViewModel>
    where TModel : class
    where TViewModel : class
{
    /// <summary>
    /// Maps a ViewModel to a Data Model.
    /// </summary>
    TModel MapReverse(TViewModel viewModel);
}
```

### Step 5: Update Existing Mappers

Modify your existing mappers to implement `IReverseMapper<TViewModel, TModel>`:

**Example:** `src/NorthwindAspire.Frontend/Models/Mappers/CategoryMapper.cs`

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class CategoryMapper : IReverseMapper<CategoryViewModel, Category>
{
    public CategoryViewModel Map(Category model)
    {
        if (model == null)
            return null!;

        return new CategoryViewModel
        {
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            Description = model.Description,
            Picture = model.Picture
        };
    }

    public Category MapReverse(CategoryViewModel viewModel)
    {
        if (viewModel == null)
            return null!;

        return new Category
        {
            CategoryId = viewModel.CategoryId,
            CategoryName = viewModel.CategoryName,
            Description = viewModel.Description,
            Picture = viewModel.Picture
        };
    }
}
```

### Step 6: Update ODataFrontendService to Use Reverse Mapping

Update the `MapViewModelToModel` method in `ODataFrontendService`:

```csharp
private TModel MapViewModelToModel<TModel, TViewModel>(TViewModel viewModel, IMapper<TModel, TViewModel> mapper)
    where TModel : class
    where TViewModel : class
{
    // Check if mapper implements IReverseMapper
    if (mapper is IReverseMapper<TViewModel, TModel> reverseMapper)
    {
        return reverseMapper.MapReverse(viewModel);
    }

    throw new NotImplementedException(
        $"Mapper for {typeof(TViewModel).Name} does not implement IReverseMapper<{typeof(TViewModel).Name}, {typeof(TModel).Name}>. " +
        $"Reverse mapping (ViewModel -> Model) is required for Create and Update operations.");
}
```

## Usage Examples

### Example 1: Get a Single Category

```csharp
public class CategoryService
{
    private readonly IODataFrontendService _oDataService;

    public CategoryService(IODataFrontendService oDataService)
    {
        _oDataService = oDataService;
    }

    public async Task<CategoryViewModel?> GetCategoryAsync(int categoryId)
    {
        // Uses OData filter: /api/Categories?$filter=CategoryId eq 1
        return await _oDataService.GetByKeyAsync<Category, CategoryViewModel>(categoryId);
    }
}
```

### Example 2: Get All Products

```csharp
public class ProductService
{
    private readonly IODataFrontendService _oDataService;

    public ProductService(IODataFrontendService oDataService)
    {
        _oDataService = oDataService;
    }

    public async Task<List<ProductViewModel>> GetAllProductsAsync()
    {
        // Calls: /api/Products
        return await _oDataService.GetAllAsync<Product, ProductViewModel>();
    }
}
```

### Example 3: Create a New Customer

```csharp
public class CustomerService
{
    private readonly IODataFrontendService _oDataService;

    public CustomerService(IODataFrontendService oDataService)
    {
        _oDataService = oDataService;
    }

    public async Task<CustomerViewModel> CreateCustomerAsync(CustomerViewModel customerViewModel)
    {
        // Maps CustomerViewModel -> Customer (data model)
        // Sends POST to: /api/Customers
        // Returns created CustomerViewModel with server-generated values
        return await _oDataService.CreateAsync<Customer, CustomerViewModel>(customerViewModel);
    }
}
```

### Example 4: Update an Order

```csharp
public class OrderService
{
    private readonly IODataFrontendService _oDataService;

    public OrderService(IODataFrontendService oDataService)
    {
        _oDataService = oDataService;
    }

    public async Task UpdateOrderAsync(int orderId, OrderViewModel orderViewModel)
    {
        // Maps OrderViewModel -> Order (data model)
        // Sends PUT to: /api/Orders/{orderId}
        await _oDataService.UpdateAsync<Order, OrderViewModel>(orderId, orderViewModel);
    }
}
```

### Example 5: Delete a Supplier

```csharp
public class SupplierService
{
    private readonly IODataFrontendService _oDataService;

    public SupplierService(IODataFrontendService oDataService)
    {
        _oDataService = oDataService;
    }

    public async Task DeleteSupplierAsync(int supplierId)
    {
        // Sends DELETE to: /api/Suppliers/{supplierId}
        await _oDataService.DeleteAsync<Supplier>(supplierId);
    }
}
```

## Key Features

? **Generic CRUD Operations**: Work with any Model/ViewModel pair without code duplication

? **Automatic Mapping**: Uses `MapperRegistry` to automatically map between ViewModels and Data Models

? **OData Filtering**: Uses standard OData `$filter=eq` syntax for single record retrieval

? **Logging**: Integrated logging for debugging and monitoring

? **Error Handling**: Comprehensive error handling with detailed logging

? **Type Safety**: Full compile-time type safety with generics

? **Dependency Injection**: Fully integrated with ASP.NET Core DI container

## Configuration

Ensure your `appsettings.json` includes the backend API URL:

```json
{
  "BackendUrl": "https://localhost:7001"
}
```

## Future Enhancements

- Add support for OData `$expand` for related entities
- Add support for OData `$orderby` and `$select` in GetAllAsync
- Add paging support with `$top` and `$skip`
- Implement caching strategy
- Add support for batch operations
- Implement optimistic concurrency control with ETag support

## Notes

- The service assumes conventional naming: Model class names + "s" = API endpoint (e.g., Category -> Categories)
- Key properties follow convention: {ModelName}Id (e.g., CategoryId, ProductId)
- String key values are wrapped in quotes in OData filters, numeric values are not
- All methods are async and support cancellation tokens (can be added in future versions)
