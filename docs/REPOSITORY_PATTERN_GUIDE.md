# Repository Pattern Implementation Guide

## Overview

The repository pattern is a data access abstraction layer that encapsulates the logic for querying and persisting entities. This guide explains how to implement and use the repository pattern in the NorthwindAspire backend API.

## Architecture

```
Controllers
    ? (Dependency Injection)
Repositories (IGenericRepository<T>, IEntityRepository)
    ?
Entity Framework Core
    ?
SQLite Database
```

## Core Components

### 1. Generic Repository Interface

**File**: `src/NorthwindAspire.Backend/Repositories/IGenericRepository.cs`

The `IGenericRepository<T>` interface defines the contract for basic CRUD operations:

```csharp
namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Generic repository interface for shared CRUD operations
/// </summary>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Get all entities asynchronously
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Get entity by ID asynchronously
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Add a new entity asynchronously
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Update an existing entity asynchronously
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete entity by ID asynchronously
    /// </summary>
    Task DeleteAsync(object id);

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Get queryable collection for advanced LINQ queries
    /// </summary>
    IQueryable<T> GetQueryable();
}
```

### 2. Generic Repository Implementation

**File**: `src/NorthwindAspire.Backend/Repositories/GenericRepository.cs`

The `GenericRepository<T>` class implements the generic interface:

```csharp
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;

namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Generic repository implementation for CRUD operations
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly NorthwindContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(NorthwindContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }
}
```

### 3. Entity-Specific Repository (Optional)

For complex business logic specific to an entity, create a specialized repository:

**File**: `src/NorthwindAspire.Backend/Repositories/ICustomerRepository.cs`

```csharp
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Repositories;

/// <summary>
/// Customer-specific repository with specialized queries
/// </summary>
public interface ICustomerRepository : IGenericRepository<Customer>
{
    /// <summary>
    /// Get customers by country
    /// </summary>
    Task<IEnumerable<Customer>> GetByCountryAsync(string country);

    /// <summary>
    /// Get customers with their orders
    /// </summary>
    Task<IEnumerable<Customer>> GetWithOrdersAsync();

    /// <summary>
    /// Search customers by company name
    /// </summary>
    Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string searchTerm);
}
```

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
    private readonly NorthwindContext _context;

    public CustomerRepository(NorthwindContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetByCountryAsync(string country)
    {
        return await _context.Customers
            .Where(c => c.Country == country)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetWithOrdersAsync()
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> SearchByCompanyNameAsync(string searchTerm)
    {
        return await _context.Customers
            .Where(c => c.CompanyName.Contains(searchTerm))
            .ToListAsync();
    }
}
```

## Dependency Injection Setup

**File**: `src/NorthwindAspire.Backend/Program.cs`

Register repositories in the dependency injection container:

```csharp
// Add repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register specific repositories (optional)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// ... etc for other entities
```

## Using Repositories in Controllers

### Before (Without Repository Pattern)

```csharp
public class CustomersController : ODataController
{
    private readonly NorthwindContext _context;

    public CustomersController(NorthwindContext context)
    {
        _context = context;
    }

    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Customer> Get()
    {
        return _context.Customers;  // Direct DbContext access
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Customers.Add(customer);  // Direct DbContext access
        await _context.SaveChangesAsync();
        return Created($"odata/customers('{customer.CustomerId}')", customer);
    }
}
```

### After (With Repository Pattern)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Backend.Repositories;

namespace NorthwindAspire.Backend.Controllers;

public class CustomersController : ODataController
{
    private readonly IGenericRepository<Customer> _repository;

    public CustomersController(IGenericRepository<Customer> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Customer> Get()
    {
        return _repository.GetQueryable();  // Repository access
    }

    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<Customer>> Get(string key)
    {
        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();
        return Created($"odata/customers('{customer.CustomerId}')", customer);
    }

    /// <summary>
    /// Update a customer
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(string key, [FromBody] Delta<Customer> delta)
    {
        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        delta.Patch(customer);
        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string key)
    {
        var customer = await _repository.GetByIdAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(key);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
```

## Generating Controllers with Repository Pattern

### Template for New Controllers

When creating a new OData controller, follow this template:

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
/// OData controller for [Entity] resource
/// </summary>
[Authorize]
public class [Entity]Controller : ODataController
{
    private readonly IGenericRepository<[Entity]> _repository;

    public [Entity]Controller(IGenericRepository<[Entity]> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get all [entities]
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<[Entity]> Get()
    {
        return _repository.GetQueryable();
    }

    /// <summary>
    /// Get a specific [entity] by ID
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<[Entity]>> Get(object key)
    {
        var entity = await _repository.GetByIdAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    /// <summary>
    /// Create a new [entity]
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<[Entity]>> Post([FromBody] [Entity] entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return Created($"odata/[entities]('{entity.[Key]}')", entity);
    }

    /// <summary>
    /// Update a [entity]
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(object key, [FromBody] Delta<[Entity]> delta)
    {
        var entity = await _repository.GetByIdAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        delta.Patch(entity);
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete a [entity]
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(object key)
    {
        var entity = await _repository.GetByIdAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(key);
        await _repository.SaveChangesAsync();
        return NoContent();
    }
}
```

### Step-by-Step: Creating a New Controller

1. **Create the controller class** in `src/NorthwindAspire.Backend/Controllers/[Entity]Controller.cs`
2. **Inject the repository** via constructor
3. **Implement CRUD methods** using the repository
4. **Apply `[Authorize]` attribute** for security
5. **Register in DI** if using a specific repository in `Program.cs`

## Unit Testing with Repositories

The repository pattern simplifies testing by allowing you to mock data access:

```csharp
using Moq;
using NorthwindAspire.Backend.Controllers;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Backend.Repositories;

namespace NorthwindAspire.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<IGenericRepository<Customer>> _mockRepository;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockRepository = new Mock<IGenericRepository<Customer>>();
        _controller = new CustomersController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customerId = "ALFKI";
        var customer = new Customer { CustomerId = customerId, CompanyName = "Alfreds" };
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.Get(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCustomer = Assert.IsType<Customer>(okResult.Value);
        Assert.Equal(customerId, returnedCustomer.CustomerId);
    }

    [Fact]
    public async Task Post_WithValidCustomer_CreatesAndReturns()
    {
        // Arrange
        var customer = new Customer { CustomerId = "NEWCO", CompanyName = "New Company" };

        // Act
        var result = await _controller.Post(customer);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        _mockRepository.Verify(r => r.AddAsync(customer), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
```

## Best Practices

1. **Always use `async/await`** for database operations
2. **Inject specific repositories** when you need specialized queries
3. **Use `GetQueryable()`** for complex LINQ queries in controllers
4. **Call `SaveChangesAsync()`** after add/update/delete operations
5. **Return null for not found** instead of throwing exceptions
6. **Use dependency injection** for loose coupling
7. **Apply `[Authorize]`** to controllers for security
8. **Use `[EnableQuery]`** on GET methods for OData support

## Migration Path

To migrate an existing controller to use the repository pattern:

1. Add `IGenericRepository<T>` injection
2. Replace `_context.Entity` with `_repository` calls
3. Update `SaveChangesAsync()` calls
4. Test thoroughly
5. Remove direct `NorthwindContext` dependency
6. Register repository in DI if needed

## Summary

The repository pattern provides:

? Clean separation of data access logic
? Easy unit testing with mocks
? Flexibility to change database technology
? Consistent CRUD patterns across controllers
? Reduced code duplication
? Better maintainability

