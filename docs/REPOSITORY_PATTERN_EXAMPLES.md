# Repository Pattern: Complete Examples

This document provides complete, copy-paste ready examples for implementing the repository pattern in your NorthwindAspire backend.

## Example 1: Generic Repository Interface

**File**: `src/NorthwindAspire.Backend/Repositories/IGenericRepository.cs`

```csharp
namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Generic repository interface for shared CRUD operations.
/// All entity repositories should extend this interface.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Get all entities asynchronously
    /// </summary>
    /// <returns>List of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Get a single entity by its primary key asynchronously
    /// </summary>
    /// <param name="id">The primary key value</param>
    /// <returns>The entity if found; null otherwise</returns>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Add a new entity asynchronously
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>A completed task</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Update an existing entity asynchronously
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <returns>A completed task</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete an entity by its primary key asynchronously
    /// </summary>
    /// <param name="id">The primary key value</param>
    /// <returns>A completed task</returns>
    Task DeleteAsync(object id);

    /// <summary>
    /// Save all pending changes to the database
    /// </summary>
    /// <returns>A completed task</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Get a queryable collection for advanced LINQ operations.
    /// Use this for complex queries with filtering, sorting, and projection.
    /// </summary>
    /// <returns>IQueryable collection</returns>
    IQueryable<T> GetQueryable();
}
```

## Example 2: Generic Repository Implementation

**File**: `src/NorthwindAspire.Backend/Repositories/GenericRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;

namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Generic repository implementation providing base CRUD operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly NorthwindContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(NorthwindContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(object id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(object id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public virtual IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }
}
```

## Example 3: Entity-Specific Repository Interface

**File**: `src/NorthwindAspire.Backend/Repositories/ICustomerRepository.cs`

```csharp
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Customer-specific repository for specialized customer queries
/// </summary>
public interface ICustomerRepository : IGenericRepository<Customer>
{
    /// <summary>
    /// Get all customers from a specific country
    /// </summary>
    /// <param name="country">The country name</param>
    /// <returns>Customers from the specified country</returns>
    Task<IEnumerable<Customer>> GetByCountryAsync(string country);

    /// <summary>
    /// Get customers with their related orders loaded
    /// </summary>
    /// <returns>Customers with expanded Orders collection</returns>
    Task<IEnumerable<Customer>> GetWithOrdersAsync();

    /// <summary>
    /// Search customers by company name (partial match)
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <returns>Matching customers</returns>
    Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string searchTerm);

    /// <summary>
    /// Get customers with high-value orders
    /// </summary>
    /// <param name="minimumOrderAmount">Minimum order amount filter</param>
    /// <returns>Customers with orders exceeding the amount</returns>
    Task<IEnumerable<Customer>> GetWithHighValueOrdersAsync(decimal minimumOrderAmount);
}
```

## Example 4: Entity-Specific Repository Implementation

**File**: `src/NorthwindAspire.Backend/Repositories/CustomerRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Customer repository with specialized queries
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(NorthwindContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetByCountryAsync(string country)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        return await _context.Customers
            .Where(c => c.Country == country)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetWithOrdersAsync()
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.Customers
            .Where(c => c.CompanyName.ToLower().Contains(lowerSearchTerm))
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetWithHighValueOrdersAsync(decimal minimumOrderAmount)
    {
        if (minimumOrderAmount < 0)
            throw new ArgumentException("Order amount cannot be negative", nameof(minimumOrderAmount));

        return await _context.Customers
            .Where(c => c.Orders != null && c.Orders.Any(o => o.OrderDetails!.Sum(od => od.UnitPrice * od.Quantity) >= minimumOrderAmount))
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }
}
```

## Example 5: OData Controller with Repository Pattern

**File**: `src/NorthwindAspire.Backend/Controllers/CustomersController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Backend.Repositories;

namespace NorthwindAspire.Backend.Controllers;

/// <summary>
/// OData controller for Customer resources
/// Provides CRUD operations for customers with full OData support
/// </summary>
[Authorize]
public class CustomersController : ODataController
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IGenericRepository<Customer> repository, ILogger<CustomersController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all customers with OData query support
    /// </summary>
    /// <remarks>
    /// Supports OData query options:
    /// - $filter: Filter by any field (e.g., ?$filter=country eq 'USA')
    /// - $orderby: Sort by fields (e.g., ?$orderby=companyName)
    /// - $top/$skip: Pagination (e.g., ?$top=20&$skip=10)
    /// - $select: Projection (e.g., ?$select=customerId,companyName)
    /// - $expand: Include related data (e.g., ?$expand=orders)
    /// </remarks>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Customer> Get()
    {
        _logger.LogInformation("GET: Retrieving all customers");
        return _repository.GetQueryable();
    }

    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    /// <param name="key">The customer ID (primary key)</param>
    /// <returns>The customer if found; 404 Not Found otherwise</returns>
    [HttpGet("{key}")]
    public async Task<ActionResult<Customer>> Get([FromRoute] string key)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Customer ID is required");

        _logger.LogInformation("GET: Retrieving customer {CustomerId}", key);

        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            _logger.LogWarning("GET: Customer {CustomerId} not found", key);
            return NotFound();
        }

        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="customer">The customer object to create</param>
    /// <returns>201 Created with location header and created customer</returns>
    /// <response code="201">Customer successfully created</response>
    /// <response code="400">Invalid customer data</response>
    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
    {
        if (customer == null)
            return BadRequest("Customer object cannot be null");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("POST: Creating new customer {CustomerId}", customer.CustomerId);

        try
        {
            await _repository.AddAsync(customer);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("POST: Customer {CustomerId} created successfully", customer.CustomerId);
            return Created($"odata/customers('{customer.CustomerId}')", customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST: Error creating customer {CustomerId}", customer.CustomerId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating customer");
        }
    }

    /// <summary>
    /// Update a customer (partial update)
    /// </summary>
    /// <param name="key">The customer ID (primary key)</param>
    /// <param name="delta">The partial update data</param>
    /// <returns>204 No Content on success; 404 Not Found if customer doesn't exist</returns>
    /// <response code="204">Customer successfully updated</response>
    /// <response code="404">Customer not found</response>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch([FromRoute] string key, [FromBody] Delta<Customer> delta)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Customer ID is required");

        if (delta == null)
            return BadRequest("Update data cannot be null");

        _logger.LogInformation("PATCH: Updating customer {CustomerId}", key);

        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            _logger.LogWarning("PATCH: Customer {CustomerId} not found", key);
            return NotFound();
        }

        try
        {
            delta.Patch(customer);
            await _repository.UpdateAsync(customer);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("PATCH: Customer {CustomerId} updated successfully", key);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PATCH: Error updating customer {CustomerId}", key);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating customer");
        }
    }

    /// <summary>
    /// Replace a customer completely
    /// </summary>
    /// <param name="key">The customer ID (primary key)</param>
    /// <param name="customer">The complete customer object</param>
    /// <returns>204 No Content on success; 404 Not Found if customer doesn't exist</returns>
    [HttpPut("{key}")]
    public async Task<ActionResult> Put([FromRoute] string key, [FromBody] Customer customer)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Customer ID is required");

        if (customer == null)
            return BadRequest("Customer object cannot be null");

        if (customer.CustomerId != key)
            return BadRequest("Customer ID in URL must match body");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("PUT: Replacing customer {CustomerId}", key);

        var existingCustomer = await _repository.GetByIdAsync(key);
        if (existingCustomer == null)
        {
            _logger.LogWarning("PUT: Customer {CustomerId} not found", key);
            return NotFound();
        }

        try
        {
            // Copy all properties from customer to existingCustomer
            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.ContactName = customer.ContactName;
            existingCustomer.ContactTitle = customer.ContactTitle;
            existingCustomer.Address = customer.Address;
            existingCustomer.City = customer.City;
            existingCustomer.Region = customer.Region;
            existingCustomer.PostalCode = customer.PostalCode;
            existingCustomer.Country = customer.Country;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Fax = customer.Fax;

            await _repository.UpdateAsync(existingCustomer);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("PUT: Customer {CustomerId} replaced successfully", key);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT: Error replacing customer {CustomerId}", key);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error replacing customer");
        }
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    /// <param name="key">The customer ID (primary key)</param>
    /// <returns>204 No Content on success; 404 Not Found if customer doesn't exist</returns>
    /// <response code="204">Customer successfully deleted</response>
    /// <response code="404">Customer not found</response>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete([FromRoute] string key)
    {
        if (string.IsNullOrEmpty(key))
            return BadRequest("Customer ID is required");

        _logger.LogInformation("DELETE: Deleting customer {CustomerId}", key);

        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            _logger.LogWarning("DELETE: Customer {CustomerId} not found", key);
            return NotFound();
        }

        try
        {
            await _repository.DeleteAsync(key);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("DELETE: Customer {CustomerId} deleted successfully", key);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE: Error deleting customer {CustomerId}", key);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting customer");
        }
    }
}
```

## Example 6: Dependency Injection Setup

**File**: `src/NorthwindAspire.Backend/Program.cs` (relevant section)

```csharp
// ... other configuration ...

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add database context
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Northwind")));

// Add repositories
// Generic repository for all entities
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Specific repositories (optional - only add if you have specialized implementations)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// Add more as needed

// Add OData
builder.Services.AddControllers().AddOData(opt =>
{
    var edmModel = GetEdmModel();
    opt.AddRouteComponents("odata", edmModel)
        .Filter()
        .Select()
        .Expand()
        .OrderBy()
        .Count();
});

// ... rest of configuration ...

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    
    builder.EntitySet<Category>("Categories");
    builder.EntitySet<Customer>("Customers");
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<EmployeeTerritory>("EmployeeTerritories");
    builder.EntitySet<Order>("Orders");
    builder.EntitySet<OrderDetail>("OrderDetails");
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Region>("Regions");
    builder.EntitySet<Shipper>("Shippers");
    builder.EntitySet<Supplier>("Suppliers");
    builder.EntitySet<Territory>("Territories");

    return builder.GetEdmModel();
}
```

## Example 7: Unit Tests with Mocked Repository

**File**: `tests/NorthwindAspire.Tests/Controllers/CustomersControllerTests.cs`

```csharp
using Moq;
using Xunit;
using NorthwindAspire.Backend.Controllers;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace NorthwindAspire.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<IGenericRepository<Customer>> _mockRepository;
    private readonly Mock<ILogger<CustomersController>> _mockLogger;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockRepository = new Mock<IGenericRepository<Customer>>();
        _mockLogger = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Get_WithValidId_ReturnsOkWithCustomer()
    {
        // Arrange
        var customerId = "ALFKI";
        var customer = new Customer { CustomerId = customerId, CompanyName = "Alfreds Futterkiste" };
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.Get(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCustomer = Assert.IsType<Customer>(okResult.Value);
        Assert.Equal(customerId, returnedCustomer.CustomerId);
        _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var customerId = "INVALID";
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.Get(customerId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task Post_WithValidCustomer_CreatesAndReturnsCreated()
    {
        // Arrange
        var customer = new Customer
        {
            CustomerId = "NEWCO",
            CompanyName = "New Company",
            ContactName = "John Doe"
        };

        // Act
        var result = await _controller.Post(customer);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal($"odata/customers('{customer.CustomerId}')", createdResult.Location);
        _mockRepository.Verify(r => r.AddAsync(customer), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Post_WithNullCustomer_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Post(null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WithValidId_DeletesAndReturnsNoContent()
    {
        // Arrange
        var customerId = "ALFKI";
        var customer = new Customer { CustomerId = customerId };
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.Delete(customerId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(customerId), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var customerId = "INVALID";
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.Delete(customerId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<object>()), Times.Never);
    }
}
```

## Summary

These examples provide:

1. ? Clean repository abstraction
2. ? Dependency injection setup
3. ? Full CRUD operations
4. ? OData support
5. ? Error handling with logging
6. ? Unit testing with mocks
7. ? Following ASP.NET Core best practices

Use these as templates for all your controllers and repositories!
