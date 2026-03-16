# Repository Pattern Implementation Checklist

## Quick Reference for Controller Generation

Use this checklist when implementing a new OData controller with the repository pattern.

## Pre-Implementation

- [ ] Model class exists in `src/NorthwindAspire.Backend/Models/NorthwindModels.cs`
- [ ] DbSet is configured in `src/NorthwindAspire.Backend/Data/NorthwindContext.cs`
- [ ] Generic repository is already in place (`IGenericRepository<T>`, `GenericRepository<T>`)
- [ ] OData route is configured in `Program.cs`

## Controller Implementation Steps

### 1. Create the Controller File
- [ ] Create file: `src/NorthwindAspire.Backend/Controllers/[Entity]Controller.cs`
- [ ] Replace `[Entity]` with the actual entity name (e.g., `Customer`)

### 2. Add Required Imports
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Backend.Repositories;
```

### 3. Define Controller Class
- [ ] Inherit from `ODataController`
- [ ] Add `[Authorize]` attribute for security
- [ ] Use correct namespace: `namespace NorthwindAspire.Backend.Controllers;`

### 4. Implement Constructor
- [ ] Inject `IGenericRepository<[Entity]>` as parameter
- [ ] Store in private readonly field
- [ ] Example:
```csharp
private readonly IGenericRepository<Customer> _repository;

public CustomersController(IGenericRepository<Customer> repository)
{
    _repository = repository;
}
```

### 5. Implement GET (List All)
- [ ] Use `[HttpGet]` attribute
- [ ] Add `[EnableQuery(PageSize = 100)]` for OData support
- [ ] Return `IQueryable<[Entity]>`
- [ ] Call `_repository.GetQueryable()`
- [ ] Example:
```csharp
[HttpGet]
[EnableQuery(PageSize = 100)]
public IQueryable<Customer> Get()
{
    return _repository.GetQueryable();
}
```

### 6. Implement GET (Get by ID)
- [ ] Use `[HttpGet("{key}")]` attribute
- [ ] Return `Task<ActionResult<[Entity]>>`
- [ ] Call `_repository.GetByIdAsync(key)`
- [ ] Return `NotFound()` if null
- [ ] Example:
```csharp
[HttpGet("{key}")]
public async Task<ActionResult<Customer>> Get(string key)
{
    var customer = await _repository.GetByIdAsync(key);
    if (customer == null) return NotFound();
    return Ok(customer);
}
```

### 7. Implement POST (Create)
- [ ] Use `[HttpPost]` attribute
- [ ] Accept `[FromBody]` model
- [ ] Validate ModelState
- [ ] Call `_repository.AddAsync(entity)`
- [ ] Call `_repository.SaveChangesAsync()`
- [ ] Return `Created()` with location URI
- [ ] Example:
```csharp
[HttpPost]
public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    
    await _repository.AddAsync(customer);
    await _repository.SaveChangesAsync();
    return Created($"odata/customers('{customer.CustomerId}')", customer);
}
```

### 8. Implement PATCH (Update)
- [ ] Use `[HttpPatch("{key}")]` attribute
- [ ] Accept `Delta<[Entity]>` for partial updates
- [ ] Retrieve existing entity
- [ ] Return `NotFound()` if entity doesn't exist
- [ ] Apply delta using `delta.Patch(entity)`
- [ ] Call `_repository.UpdateAsync(entity)`
- [ ] Call `_repository.SaveChangesAsync()`
- [ ] Return `NoContent()`
- [ ] Example:
```csharp
[HttpPatch("{key}")]
public async Task<ActionResult> Patch(string key, [FromBody] Delta<Customer> delta)
{
    var customer = await _repository.GetByIdAsync(key);
    if (customer == null) return NotFound();
    
    delta.Patch(customer);
    await _repository.UpdateAsync(customer);
    await _repository.SaveChangesAsync();
    return NoContent();
}
```

### 9. Implement DELETE
- [ ] Use `[HttpDelete("{key}")]` attribute
- [ ] Retrieve existing entity
- [ ] Return `NotFound()` if entity doesn't exist
- [ ] Call `_repository.DeleteAsync(key)`
- [ ] Call `_repository.SaveChangesAsync()`
- [ ] Return `NoContent()`
- [ ] Example:
```csharp
[HttpDelete("{key}")]
public async Task<ActionResult> Delete(string key)
{
    var customer = await _repository.GetByIdAsync(key);
    if (customer == null) return NotFound();
    
    await _repository.DeleteAsync(key);
    await _repository.SaveChangesAsync();
    return NoContent();
}
```

### 10. Add XML Documentation
- [ ] Add summary comments for each public method
- [ ] Example:
```csharp
/// <summary>
/// Get all customers
/// </summary>
[HttpGet]
public IQueryable<Customer> Get() { ... }
```

### 11. Register in DI (if needed)
- [ ] If using entity-specific repository, add to `Program.cs`:
```csharp
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
```
- [ ] Generic repositories are already registered:
```csharp
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

## Optional: Create Entity-Specific Repository

### Only if you need specialized queries

- [ ] Create interface: `src/NorthwindAspire.Backend/Repositories/I[Entity]Repository.cs`
  - [ ] Extend `IGenericRepository<[Entity]>`
  - [ ] Define custom query methods

- [ ] Create implementation: `src/NorthwindAspire.Backend/Repositories/[Entity]Repository.cs`
  - [ ] Inherit from `GenericRepository<[Entity]>`
  - [ ] Implement custom query methods
  - [ ] Use `_context` or `DbSet` for LINQ queries

- [ ] Update controller constructor to inject specific repository
```csharp
public CustomersController(ICustomerRepository repository)
{
    _repository = repository;
}
```

- [ ] Register in `Program.cs`
```csharp
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
```

## Testing

- [ ] Create unit tests in `tests/NorthwindAspire.Tests/Controllers/[Entity]ControllerTests.cs`
- [ ] Mock `IGenericRepository<[Entity]>` using Moq
- [ ] Test each CRUD operation:
  - [ ] GET returns queryable
  - [ ] GET by ID returns entity or NotFound
  - [ ] POST creates and saves
  - [ ] PATCH updates and saves
  - [ ] DELETE deletes and saves

### Example Test:
```csharp
[Fact]
public async Task Post_WithValidEntity_CreatesAndReturns()
{
    var mock = new Mock<IGenericRepository<Customer>>();
    var controller = new CustomersController(mock.Object);
    var customer = new Customer { CustomerId = "NEW", CompanyName = "New Co" };

    var result = await controller.Post(customer);

    Assert.IsType<CreatedResult>(result.Result);
    mock.Verify(r => r.AddAsync(customer), Times.Once);
    mock.Verify(r => r.SaveChangesAsync(), Times.Once);
}
```

## Key Points to Remember

? **Always use async/await** for database operations
? **Always call SaveChangesAsync()** after Add, Update, or Delete
? **Always apply [Authorize]** for security
? **Always validate ModelState** on POST/PATCH
? **Use GetQueryable()** for OData support with $filter, $expand, etc.
? **Return correct HTTP status codes**: 200 OK, 201 Created, 204 No Content, 404 Not Found, 400 Bad Request
? **Handle null returns** from repository

## Common Mistakes to Avoid

? Injecting `NorthwindContext` directly instead of repository
? Forgetting `await` on async repository calls
? Forgetting to call `SaveChangesAsync()`
? Missing `[Authorize]` attribute
? Returning entity instead of location on POST
? Not validating ModelState
? Using `Put` instead of `Patch` for partial updates
? Forgetting to implement `GetQueryable()` for OData filtering

## File Locations Reference

```
src/NorthwindAspire.Backend/
??? Controllers/
?   ??? CustomersController.cs
?   ??? OrdersController.cs
?   ??? [Entity]Controller.cs (your new controller)
??? Models/
?   ??? NorthwindModels.cs (entity definitions)
??? Data/
?   ??? NorthwindContext.cs (DbContext)
??? Repositories/
?   ??? IGenericRepository.cs (interface)
?   ??? GenericRepository.cs (implementation)
?   ??? I[Entity]Repository.cs (optional, specialized)
?   ??? [Entity]Repository.cs (optional, specialized)
??? Program.cs (DI registration)
```

## Quick Command Reference

### Check controller compiles:
```bash
dotnet build src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj
```

### Run tests:
```bash
dotnet test tests/NorthwindAspire.Tests
```

### View API documentation:
```
https://localhost:port/swagger
https://localhost:port/odata/$metadata
```
