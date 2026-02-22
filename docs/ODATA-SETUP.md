# OData Setup Guide - NorthwindAspire Backend

Complete guide to adding OData API endpoints with EntitySets, Swagger/OpenAPI documentation, and CRUD operations for the Northwind database in the backend project.

---

## Overview

This guide covers:
- **OData Protocol**: Exposing Northwind tables as queryable OData endpoints
- **EntitySets**: Creating OData endpoints for each Northwind table
- **Swagger/OpenAPI**: Auto-generated API documentation with OData metadata
- **Query Support**: $filter, $select, $expand, $orderby, $top, $skip
- **CRUD Operations**: Full Create, Read, Update, Delete support

---

## Prerequisites

### NuGet Packages (Already Installed)
```
Microsoft.AspNetCore.OData v9.4.1+
Microsoft.EntityFrameworkCore v10.0.3+
Microsoft.EntityFrameworkCore.Sqlite v10.0.3+
Microsoft.AspNetCore.OpenApi v10.0.3+
```

Verify in `NorthwindAspire.Backend.csproj`:
```xml
<PackageReference Include="Microsoft.AspNetCore.OData" Version="9.4.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.3" />
```

---

## Step 1: Create Entity Models

Create a file: `src/NorthwindAspire.Backend/Models/NorthwindModels.cs`

```csharp
namespace NorthwindAspire.Backend.Models;

// Categories
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

// Customers
public class Customer
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Employees
public class Employee
{
    public int EmployeeId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleOfCourtesy { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string PhotoPath { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int? ReportsTo { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<EmployeeTerritory> Territories { get; set; } = new List<EmployeeTerritory>();
}

// Order Details
public class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

// Orders
public class Order
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal Freight { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipAddress { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipRegion { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;
    public Customer Customer { get; set; } = null!;
    public Employee? Employee { get; set; }
    public Shipper? Shipper { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

// Products
public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public int QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public int ReorderLevel { get; set; }
    public string Discontinued { get; set; } = string.Empty;
    public Supplier? Supplier { get; set; }
    public Category? Category { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

// Shippers
public class Shipper
{
    public int ShipperId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Suppliers
public class Supplier
{
    public int SupplierId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string HomePage { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

// Territories
public class Territory
{
    public string TerritoryId { get; set; } = string.Empty;
    public string TerritoryDescription { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public ICollection<EmployeeTerritory> Employees { get; set; } = new List<EmployeeTerritory>();
}

// Regions
public class Region
{
    public int RegionId { get; set; }
    public string RegionDescription { get; set; } = string.Empty;
    public ICollection<Territory> Territories { get; set; } = new List<Territory>();
}

// Employee Territories (Junction Table)
public class EmployeeTerritory
{
    public int EmployeeId { get; set; }
    public string TerritoryId { get; set; } = string.Empty;
    public Employee Employee { get; set; } = null!;
    public Territory Territory { get; set; } = null!;
}
```

---

## Step 2: Create Entity Framework DbContext

Create a file: `src/NorthwindAspire.Backend/Data/NorthwindContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Data;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Shipper> Shippers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Territory> Territories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary keys
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Employee>().HasKey(e => e.EmployeeId);
        modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
        modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderId, od.ProductId });
        modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
        modelBuilder.Entity<Region>().HasKey(r => r.RegionId);
        modelBuilder.Entity<Shipper>().HasKey(s => s.ShipperId);
        modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);
        modelBuilder.Entity<Territory>().HasKey(t => t.TerritoryId);
        modelBuilder.Entity<EmployeeTerritory>().HasKey(et => new { et.EmployeeId, et.TerritoryId });

        // Configure foreign keys and relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Shipper)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShipVia)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ReportsTo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Employee)
            .WithMany(e => e.Territories)
            .HasForeignKey(et => et.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Territory)
            .WithMany(t => t.Employees)
            .HasForeignKey(et => et.TerritoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Territory>()
            .HasOne(t => t.Region)
            .WithMany(r => r.Territories)
            .HasForeignKey(t => t.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## Step 3: Update Program.cs with OData and OpenAPI

Replace the contents of `src/NorthwindAspire.Backend/Program.cs`:

```csharp
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add Entity Framework with SQLite
var sqliteConnection = "Data Source=northwind.db";
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlite(sqliteConnection));

// Add OpenAPI (Swagger)
builder.Services.AddOpenApi("v1", options =>
{
    options.Title = "Northwind OData API";
    options.Description = "OData API for Northwind database with full CRUD operations";
    options.Version = "v1.0.0";
});

// Build OData Model
var odataModelBuilder = new ODataConventionModelBuilder();
odataModelBuilder.EntitySet<Category>("Categories");
odataModelBuilder.EntitySet<Customer>("Customers");
odataModelBuilder.EntitySet<Employee>("Employees");
odataModelBuilder.EntitySet<EmployeeTerritory>("EmployeeTerritories");
odataModelBuilder.EntitySet<Order>("Orders");
odataModelBuilder.EntitySet<OrderDetail>("OrderDetails");
odataModelBuilder.EntitySet<Product>("Products");
odataModelBuilder.EntitySet<Region>("Regions");
odataModelBuilder.EntitySet<Shipper>("Shippers");
odataModelBuilder.EntitySet<Supplier>("Suppliers");
odataModelBuilder.EntitySet<Territory>("Territories");

var odataModel = odataModelBuilder.GetEdmModel();

// Add OData services
builder.Services.AddControllers().AddOData(options =>
    options
        .Select()
        .Expand()
        .Filter()
        .OrderBy()
        .SetMaxTop(1000)
        .Count()
        .SkipToken()
        .AddRouteComponents("odata", odataModel));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
        options.WithTitle("Northwind OData API Documentation")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

// Display available endpoints
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("OData endpoints available at: http://localhost:port/odata");
    app.Logger.LogInformation("OData metadata available at: http://localhost:port/odata/$metadata");
    app.Logger.LogInformation("API documentation available at: http://localhost:port/openapi/v1.json");
}

app.Run();
```

---

## Step 4: Create OData Controllers

Create a file: `src/NorthwindAspire.Backend/Controllers/CategoriesController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;

[ApiController]
[Route("odata/[controller]")]
public class CategoriesController : ODataController
{
    private readonly NorthwindContext _context;

    public CategoriesController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all categories with OData query support
    /// </summary>
    /// <remarks>
    /// OData Query Examples:
    /// - /odata/categories?$filter=categoryName eq 'Beverages'
    /// - /odata/categories?$select=categoryId,categoryName
    /// - /odata/categories?$expand=products
    /// - /odata/categories?$orderby=categoryName&$top=10&$skip=5
    /// </remarks>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Category> Get()
    {
        return _context.Categories;
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Category>> GetById(int key)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Category>> Post([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Created($"odata/categories({category.CategoryId})", category);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Category> delta)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }

        delta.Patch(category);
        await _context.SaveChangesAsync();
        return Updated(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

Create a file: `src/NorthwindAspire.Backend/Controllers/CustomersController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;

[ApiController]
[Route("odata/[controller]")]
public class CustomersController : ODataController
{
    private readonly NorthwindContext _context;

    public CustomersController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Customer> Get()
    {
        return _context.Customers;
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Customer>> GetById(string key)
    {
        var customer = await _context.Customers.FindAsync(key);
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

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return Created($"odata/customers('{customer.CustomerId}')", customer);
    }

    /// <summary>
    /// Update a customer
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(string key, [FromBody] Delta<Customer> delta)
    {
        var customer = await _context.Customers.FindAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        delta.Patch(customer);
        await _context.SaveChangesAsync();
        return Updated(customer);
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string key)
    {
        var customer = await _context.Customers.FindAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

Create remaining controllers similarly for:
- `ProductsController.cs`
- `OrdersController.cs`
- `EmployeesController.cs`
- `SuppliersController.cs`
- `ShippersController.cs`

(Template above can be reused for each entity)

---

## Step 5: Add Entity Configuration to appsettings.json

Update `src/NorthwindAspire.Backend/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "NorthwindDb": "Data Source=northwind.db"
  },
  "OData": {
    "MaxTop": 1000,
    "PageSize": 100,
    "AllowedArithmeticOperators": "All",
    "AllowedLogicalOperators": "All",
    "AllowedFunctions": ["contains", "substringof", "startswith", "endswith"]
  }
}
```

---

## OData Query Examples

### Basic Queries

```
# Get all categories
GET /odata/categories

# Get category by ID
GET /odata/categories(1)

# Get specific fields
GET /odata/categories?$select=categoryId,categoryName

# Expand related entities
GET /odata/categories?$expand=products

# Filter with conditions
GET /odata/categories?$filter=categoryName eq 'Beverages'

# Order and pagination
GET /odata/categories?$orderby=categoryName&$top=10&$skip=5

# Count total records
GET /odata/categories/$count

# Complex query
GET /odata/products?$filter=unitPrice gt 10 and discontinued eq 'false'&$select=productId,productName,unitPrice&$orderby=unitPrice desc&$top=20
```

### Advanced Queries

```
# Search with contains
GET /odata/customers?$filter=contains(companyName,'Ltd')

# Date range filter
GET /odata/orders?$filter=orderDate ge 2023-01-01 and orderDate le 2023-12-31

# Multiple expand
GET /odata/orders?$expand=customer,employee,orderDetails($expand=product)

# Skip and take pagination
GET /odata/customers?$skip=20&$top=10

# Multiple filters
GET /odata/products?$filter=categoryId eq 1 and unitsInStock gt 0

# Sort by multiple fields
GET /odata/employees?$orderby=lastName,firstName
```

### CRUD Operations

```
# Create
POST /odata/categories
Content-Type: application/json
{
  "categoryName": "Electronics",
  "description": "Electronic devices and accessories"
}

# Update (PATCH)
PATCH /odata/categories(5)
Content-Type: application/json
{
  "categoryName": "Updated Name"
}

# Delete
DELETE /odata/categories(5)
```

---

## Swagger/OpenAPI Documentation

### Access Documentation

```
Development Mode:
- OpenAPI JSON: http://localhost:5000/openapi/v1.json
- Swagger UI: http://localhost:5000/scalar (if Scalar is installed)
- OData Metadata: http://localhost:5000/odata/$metadata
```

### Install Scalar for Beautiful UI (Optional)

```bash
dotnet add package Scalar.AspNetCore
```

Then in Program.cs (already included in Step 3):

```csharp
app.MapScalarApiReference(options =>
    options.WithTitle("Northwind OData API Documentation")
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
```

---

## Testing OData Endpoints

### Using PowerShell

```powershell
# Get all categories
$response = Invoke-RestMethod -Uri "http://localhost:5000/odata/categories" -Method Get
$response.value | Format-Table

# Get with filter
$response = Invoke-RestMethod -Uri "http://localhost:5000/odata/customers?`$filter=startswith(city,'New')" -Method Get
$response.value | Format-Table

# Get with expand
$response = Invoke-RestMethod -Uri "http://localhost:5000/odata/categories?`$expand=products&`$top=5" -Method Get
$response.value | ConvertTo-Json
```

### Using curl

```bash
# Get all customers
curl "http://localhost:5000/odata/customers"

# Get with filter
curl "http://localhost:5000/odata/customers?\$filter=city eq 'London'"

# Get with expand and select
curl "http://localhost:5000/odata/orders?\$expand=customer,orderDetails&\$select=orderId,orderDate,customer"

# Create new category
curl -X POST "http://localhost:5000/odata/categories" \
  -H "Content-Type: application/json" \
  -d '{"categoryName":"New Category","description":"Description"}'
```

### Using C# HttpClient

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000");

// Get all products with filter
var response = await client.GetAsync("/odata/products?$filter=unitPrice gt 10&$orderby=productName");
var json = await response.Content.ReadAsStringAsync();
Console.WriteLine(json);

// Create new order
var order = new { customerId = "ALFKI", employeeId = 1, orderDate = DateTime.Now };
var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
var createResponse = await client.PostAsync("/odata/orders", content);
var createdOrder = await createResponse.Content.ReadAsAsync<Order>();
```

---

## OData Query Operators

| Operator | Example | Description |
|----------|---------|-------------|
| `$filter` | `?$filter=price gt 100` | Filter records |
| `$select` | `?$select=id,name` | Select specific columns |
| `$expand` | `?$expand=orders` | Include related entities |
| `$orderby` | `?$orderby=name desc` | Sort results |
| `$top` | `?$top=10` | Limit records |
| `$skip` | `?$skip=20` | Skip records (pagination) |
| `$count` | `/$count` | Get total count |
| `$search` | `?$search="keyword"` | Full-text search |

---

## Filter Functions

| Function | Example |
|----------|---------|
| `eq` | `name eq 'John'` |
| `ne` | `status ne 'Inactive'` |
| `gt` | `price gt 100` |
| `ge` | `price ge 100` |
| `lt` | `price lt 100` |
| `le` | `price le 100` |
| `and` | `price gt 100 and stock lt 50` |
| `or` | `city eq 'London' or city eq 'Paris'` |
| `not` | `not(discontinued)` |
| `contains` | `contains(name,'Pro')` |
| `startswith` | `startswith(email,'admin')` |
| `endswith` | `endswith(email,'@example.com')` |
| `length` | `length(productName) gt 10` |
| `substring` | `substring(productName,1,5) eq 'North'` |
| `tolower` | `tolower(city) eq 'london'` |
| `toupper` | `toupper(country) eq 'USA'` |

---

## Controller Template

Use this template for creating additional OData controllers:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;

[ApiController]
[Route("odata/[controller]")]
public class [EntityName]sController : ODataController
{
    private readonly NorthwindContext _context;

    public [EntityName]sController(NorthwindContext context)
    {
        _context = context;
    }

    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<[EntityName]> Get()
    {
        return _context.[EntitySet];
    }

    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<[EntityName]>> GetById([KeyType] key)
    {
        var entity = await _context.[EntitySet].FindAsync(key);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<[EntityName]>> Post([FromBody] [EntityName] entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.[EntitySet].Add(entity);
        await _context.SaveChangesAsync();
        return Created($"odata/[controllername]({entity.[KeyProperty]})", entity);
    }

    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch([KeyType] key, [FromBody] Delta<[EntityName]> delta)
    {
        var entity = await _context.[EntitySet].FindAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        delta.Patch(entity);
        await _context.SaveChangesAsync();
        return Updated(entity);
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete([KeyType] key)
    {
        var entity = await _context.[EntitySet].FindAsync(key);
        if (entity == null)
        {
            return NotFound();
        }

        _context.[EntitySet].Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
```

---

## Docker Configuration

Your Dockerfiles should include health checks for OData endpoints:

### Update `src/NorthwindAspire.Backend/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080 8443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj", "src/NorthwindAspire.Backend/"]
RUN dotnet restore "src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj"
COPY . .
WORKDIR "/src/src/NorthwindAspire.Backend"
RUN dotnet build "NorthwindAspire.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NorthwindAspire.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthwindAspire.Backend.dll"]

# Health check for OData endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/odata/$metadata || exit 1
```

---

## Verifying OData Setup

### 1. Run the Backend
```bash
cd src/NorthwindAspire.Backend
dotnet run
```

### 2. Check OData Metadata
```
GET http://localhost:5000/odata/$metadata
```

Should return XML with all EntitySets and their properties.

### 3. Test a Simple Query
```
GET http://localhost:5000/odata/categories
```

Should return all categories in JSON format.

### 4. View OpenAPI Documentation
```
http://localhost:5000/openapi/v1.json
```

---

## Common Issues & Troubleshooting

### Issue: OData endpoints return 404
**Solution**: Ensure controllers inherit from `ODataController` and routes are correct

### Issue: Metadata endpoint empty
**Solution**: Verify all EntitySets are added to `ODataConventionModelBuilder`

### Issue: $expand not working
**Solution**: Ensure navigation properties are properly configured in DbContext relationships

### Issue: Filter operations fail
**Solution**: Check that EnableQuery attribute is applied to action methods

### Issue: Update/Patch returns 405 Method Not Allowed
**Solution**: Ensure `[HttpPatch]` attribute is present on Patch method

---

## Next Steps

1. Create `Models/NorthwindModels.cs` with all entity classes
2. Create `Data/NorthwindContext.cs` with DbContext configuration
3. Update `Program.cs` with OData configuration
4. Create controllers for each entity
5. Run migrations: `dotnet ef database update`
6. Test OData endpoints
7. Access OpenAPI documentation at `/openapi/v1.json`
8. Deploy with Docker using updated Dockerfile

---

## Additional Resources

- [OData Documentation](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html)
- [ASP.NET OData](https://github.com/OData/AspNetCoreOData)
- [Entity Framework Documentation](https://learn.microsoft.com/en-us/ef/)
- [Swagger/OpenAPI Specification](https://spec.openapis.org/oas/v3.0.3)
