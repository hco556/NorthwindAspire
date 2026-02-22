# OData API Testing Guide - NUnit + Playwright

Complete guide to creating a comprehensive C# .NET 10 test project using NUnit with Playwright for headless testing of the NorthwindAspire backend OData API.

---

## Overview

This guide covers:
- **NUnit Framework**: Testing framework for .NET
- **Playwright**: Browser automation for API testing
- **Headless Testing**: API testing without UI dependencies
- **OData API Coverage**: Full CRUD operation testing for all entities
- **CI/CD Integration**: Automated testing in Azure DevOps pipelines

---

## Prerequisites

### .NET 10 SDK
```bash
dotnet --version  # Should be 10.0.0 or higher
```

### Required NuGet Packages
```
NUnit v4.1.0+
NUnit3TestAdapter v4.5.0+
Microsoft.Playwright v1.40.0+
Microsoft.Playwright.NUnit v1.40.0+
HttpClientFactory Abstractions v10.0.3+
```

---

## Step 1: Create Test Project

### Create New Test Project
```bash
# Navigate to test directory
cd tests

# Create new test project (if not exists)
dotnet new nunit -n NorthwindAspire.Backend.Tests --framework net10.0

# Or add NUnit to existing project
dotnet add package NUnit --version 4.1.0
dotnet add package NUnit3TestAdapter --version 4.5.0
dotnet add package Microsoft.Playwright --version 1.40.0
dotnet add package Microsoft.Playwright.NUnit --version 1.40.0
```

### Update Project File

**File**: `NorthwindAspire.Backend.Tests/NorthwindAspire.Backend.Tests.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NUnit" Version="4.1.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.Playwright" Version="1.40.0" />
    <PackageReference Include="Microsoft.Playwright.NUnit" Version="1.40.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="10.0.3" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.3" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\src\NorthwindAspire.Backend\NorthwindAspire.Backend.csproj" />
  </ItemGroup>

</Project>
```

---

## Step 2: Create Test Base Classes

### OData API Test Base Class

**File**: `NorthwindAspire.Backend.Tests/ODataApiTestBase.cs`

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NorthwindAspire.Backend;
using NorthwindAspire.Backend.Data;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests;

[TestFixture]
public abstract class ODataApiTestBase
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected NorthwindContext DbContext { get; private set; } = null!;

    private const string BaseODataUrl = "/odata";
    private const string DefaultMediaType = "application/json";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Create factory with custom configuration
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Use in-memory database for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<NorthwindContext>));
                    
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<NorthwindContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        HttpClient = Factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(DefaultMediaType));

        // Initialize database
        using var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
        await DbContext.Database.EnsureCreatedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            DbContext.Dispose();
        }
        HttpClient?.Dispose();
        Factory?.Dispose();
    }

    [SetUp]
    public virtual void SetUp()
    {
        // Clear database before each test
        CleanDatabase();
    }

    protected void CleanDatabase()
    {
        DbContext.Categories.RemoveRange(DbContext.Categories);
        DbContext.Customers.RemoveRange(DbContext.Customers);
        DbContext.Products.RemoveRange(DbContext.Products);
        DbContext.Orders.RemoveRange(DbContext.Orders);
        DbContext.OrderDetails.RemoveRange(DbContext.OrderDetails);
        DbContext.Employees.RemoveRange(DbContext.Employees);
        DbContext.Suppliers.RemoveRange(DbContext.Suppliers);
        DbContext.Shippers.RemoveRange(DbContext.Shippers);
        DbContext.Regions.RemoveRange(DbContext.Regions);
        DbContext.Territories.RemoveRange(DbContext.Territories);
        DbContext.EmployeeTerritories.RemoveRange(DbContext.EmployeeTerritories);
        DbContext.SaveChanges();
    }

    protected string BuildODataUrl(string entitySet, string? filter = null, string? select = null, 
        string? expand = null, string? orderby = null, int? top = null, int? skip = null)
    {
        var url = $"{BaseODataUrl}/{entitySet}";
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filter))
            queryParams.Add($"$filter={Uri.EscapeDataString(filter)}");
        if (!string.IsNullOrEmpty(select))
            queryParams.Add($"$select={select}");
        if (!string.IsNullOrEmpty(expand))
            queryParams.Add($"$expand={expand}");
        if (!string.IsNullOrEmpty(orderby))
            queryParams.Add($"$orderby={orderby}");
        if (top.HasValue)
            queryParams.Add($"$top={top}");
        if (skip.HasValue)
            queryParams.Add($"$skip={skip}");

        if (queryParams.Any())
        {
            url += "?" + string.Join("&", queryParams);
        }

        return url;
    }

    protected async Task<T?> GetAsync<T>(string url) where T : class
    {
        var response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsAsync<T>();
        }
        return null;
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T content) where T : class
    {
        var json = JsonSerializer.Serialize(content);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, DefaultMediaType);
        return await HttpClient.PostAsync(url, httpContent);
    }

    protected async Task<HttpResponseMessage> PatchAsync<T>(string url, T content) where T : class
    {
        var json = JsonSerializer.Serialize(content);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, DefaultMediaType);
        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = httpContent
        };
        return await HttpClient.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await HttpClient.DeleteAsync(url);
    }

    protected async Task<ODataResponse<T>> GetODataCollectionAsync<T>(string url) where T : class
    {
        var response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ODataResponse<T>>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? 
                new ODataResponse<T> { Value = new List<T>() };
        }
        return new ODataResponse<T> { Value = new List<T>() };
    }
}

public class ODataResponse<T> where T : class
{
    [System.Text.Json.Serialization.JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();

    [System.Text.Json.Serialization.JsonPropertyName("@odata.count")]
    public long? Count { get; set; }
}
```

---

## Step 3: Create Category Controller Tests

**File**: `NorthwindAspire.Backend.Tests/Controllers/CategoriesControllerTests.cs`

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class CategoriesControllerTests : ODataApiTestBase
{
    private const string EntitySet = "categories";

    [Test]
    public async Task GetAllCategories_ReturnsOkStatus()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test", Description = "Test Desc" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetAllCategories_ReturnsCollectionWithItems()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await GetODataCollectionAsync<Category>($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.Value, Is.Not.Empty);
        Assert.That(response.Value[0].CategoryName, Is.EqualTo("Beverages"));
    }

    [Test]
    public async Task GetCategoryById_WithValidId_ReturnsCategory()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test", Description = "Test" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var result = await httpResponse.Content.ReadAsAsync<Category>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.CategoryName, Is.EqualTo("Test"));
    }

    [Test]
    public async Task GetCategoryById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}(999)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newCategory = new { categoryName = "Electronics", description = "Electronic devices" };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newCategory);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);
    }

    [Test]
    public async Task CreateCategory_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidCategory = new { };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", invalidCategory);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateCategory_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Old Name", Description = "Test" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        var updateData = new { categoryName = "New Name" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var updated = await DbContext.Categories.FindAsync(1);
        Assert.That(updated?.CategoryName, Is.EqualTo("New Name"));
    }

    [Test]
    public async Task UpdateCategory_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateData = new { categoryName = "New Name" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(999)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteCategory_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test", Description = "Test" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var deleted = await DbContext.Categories.FindAsync(1);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteCategory_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(999)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetCategories_WithFilter_ReturnsFilteredResults()
    {
        // Arrange
        var category1 = new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Test" };
        var category2 = new Category { CategoryId = 2, CategoryName = "Electronics", Description = "Test" };
        DbContext.Categories.AddRange(category1, category2);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "categoryName eq 'Beverages'");

        // Act
        var response = await GetODataCollectionAsync<Category>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
        Assert.That(response.Value[0].CategoryName, Is.EqualTo("Beverages"));
    }

    [Test]
    public async Task GetCategories_WithSelect_ReturnsOnlySelectedFields()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test", Description = "Test Desc" };
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, select: "categoryId,categoryName");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCategories_WithTop_LimitResults()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            DbContext.Categories.Add(new Category 
            { 
                CategoryId = i, 
                CategoryName = $"Category{i}", 
                Description = "Test" 
            });
        }
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, top: 3);

        // Act
        var response = await GetODataCollectionAsync<Category>(url);

        // Assert
        Assert.That(response.Value.Count, Is.LessThanOrEqualTo(3));
    }

    [Test]
    public async Task GetCategories_WithOrderBy_ReturnsSortedResults()
    {
        // Arrange
        DbContext.Categories.AddRange(
            new Category { CategoryId = 1, CategoryName = "Zebra", Description = "Test" },
            new Category { CategoryId = 2, CategoryName = "Apple", Description = "Test" },
            new Category { CategoryId = 3, CategoryName = "Mango", Description = "Test" }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, orderby: "categoryName");

        // Act
        var response = await GetODataCollectionAsync<Category>(url);

        // Assert
        Assert.That(response.Value[0].CategoryName, Is.EqualTo("Apple"));
    }
}
```

---

## Step 4: Create Customers Controller Tests

**File**: `NorthwindAspire.Backend.Tests/Controllers/CustomersControllerTests.cs`

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class CustomersControllerTests : ODataApiTestBase
{
    private const string EntitySet = "customers";

    [Test]
    public async Task GetAllCustomers_ReturnsOkStatus()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "CUST1",
            CompanyName = "Test Company",
            ContactName = "John Doe",
            City = "New York"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCustomerById_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            City = "Berlin"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}('ALFKI')");
        var result = await httpResponse.Content.ReadAsAsync<Customer>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.CompanyName, Is.EqualTo("Alfreds Futterkiste"));
    }

    [Test]
    public async Task CreateCustomer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newCustomer = new 
        { 
            customerId = "NEW01",
            companyName = "New Company",
            contactName = "Jane Doe",
            city = "London"
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newCustomer);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateCustomer_WithValidId_UpdatesData()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "TEST1",
            CompanyName = "Old Company",
            ContactName = "John",
            City = "Old City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        var updateData = new { companyName = "Updated Company", city = "New City" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}('TEST1')", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var updated = await DbContext.Customers.FindAsync("TEST1");
        Assert.That(updated?.CompanyName, Is.EqualTo("Updated Company"));
    }

    [Test]
    public async Task DeleteCustomer_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "DEL01",
            CompanyName = "Company",
            ContactName = "Test",
            City = "City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}('DEL01')");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var deleted = await DbContext.Customers.FindAsync("DEL01");
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task GetCustomers_WithContainsFilter_ReturnsMatches()
    {
        // Arrange
        DbContext.Customers.AddRange(
            new Customer { CustomerId = "C1", CompanyName = "Alfreds Ltd", ContactName = "Maria", City = "Berlin" },
            new Customer { CustomerId = "C2", CompanyName = "Bob's Shop", ContactName = "Bob", City = "London" }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "contains(companyName,'Ltd')");

        // Act
        var response = await GetODataCollectionAsync<Customer>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
        Assert.That(response.Value[0].CompanyName, Is.EqualTo("Alfreds Ltd"));
    }

    [Test]
    public async Task GetCustomers_WithExpandOrders_IncludesRelatedData()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "CUST1",
            CompanyName = "Test",
            ContactName = "Test",
            City = "City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "orders");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCustomers_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            DbContext.Customers.Add(new Customer 
            { 
                CustomerId = $"CUST{i:D2}",
                CompanyName = $"Company {i}",
                ContactName = $"Contact {i}",
                City = $"City {i}"
            });
        }
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, top: 5, skip: 5);

        // Act
        var response = await GetODataCollectionAsync<Customer>(url);

        // Assert
        Assert.That(response.Value.Count, Is.LessThanOrEqualTo(5));
    }
}
```

---

## Step 5: Create Products Controller Tests

**File**: `NorthwindAspire.Backend.Tests/Controllers/ProductsControllerTests.cs`

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class ProductsControllerTests : ODataApiTestBase
{
    private const string EntitySet = "products";

    [Test]
    public async Task GetAllProducts_ReturnsOkStatus()
    {
        // Arrange
        var product = new Product 
        { 
            ProductId = 1, 
            ProductName = "Test Product", 
            UnitPrice = 10.00m 
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetProductById_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product 
        { 
            ProductId = 1, 
            ProductName = "Chai", 
            UnitPrice = 18.00m 
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var result = await httpResponse.Content.ReadAsAsync<Product>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.ProductName, Is.EqualTo("Chai"));
    }

    [Test]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new 
        { 
            productName = "New Product",
            unitPrice = 25.99,
            unitsInStock = 100
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newProduct);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateProduct_WithValidId_UpdatesPrice()
    {
        // Arrange
        var product = new Product 
        { 
            ProductId = 1, 
            ProductName = "Product", 
            UnitPrice = 10.00m 
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var updateData = new { unitPrice = 15.50 };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var updated = await DbContext.Products.FindAsync(1);
        Assert.That(updated?.UnitPrice, Is.EqualTo(15.50m));
    }

    [Test]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var product = new Product 
        { 
            ProductId = 1, 
            ProductName = "Product", 
            UnitPrice = 10.00m 
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetProducts_WithPriceFilter_ReturnsFilteredResults()
    {
        // Arrange
        DbContext.Products.AddRange(
            new Product { ProductId = 1, ProductName = "Expensive", UnitPrice = 50.00m },
            new Product { ProductId = 2, ProductName = "Cheap", UnitPrice = 5.00m }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "unitPrice gt 20");

        // Act
        var response = await GetODataCollectionAsync<Product>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
        Assert.That(response.Value[0].ProductName, Is.EqualTo("Expensive"));
    }

    [Test]
    public async Task GetProducts_OrderByPrice_ReturnsSortedByPrice()
    {
        // Arrange
        DbContext.Products.AddRange(
            new Product { ProductId = 1, ProductName = "Product1", UnitPrice = 50.00m },
            new Product { ProductId = 2, ProductName = "Product2", UnitPrice = 10.00m },
            new Product { ProductId = 3, ProductName = "Product3", UnitPrice = 30.00m }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, orderby: "unitPrice");

        // Act
        var response = await GetODataCollectionAsync<Product>(url);

        // Assert
        Assert.That(response.Value[0].UnitPrice, Is.LessThanOrEqualTo(response.Value[1].UnitPrice));
    }

    [Test]
    public async Task GetProducts_WithCategoryExpand_IncludesCategory()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Test" };
        var product = new Product 
        { 
            ProductId = 1, 
            ProductName = "Chai", 
            CategoryId = 1,
            UnitPrice = 18.00m,
            Category = category
        };
        DbContext.Categories.Add(category);
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "category");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

---

## Step 6: Create Orders Controller Tests

**File**: `NorthwindAspire.Backend.Tests/Controllers/OrdersControllerTests.cs`

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class OrdersControllerTests : ODataApiTestBase
{
    private const string EntitySet = "orders";

    private Customer SetupTestCustomer()
    {
        var customer = new Customer 
        { 
            CustomerId = "TESTCUST",
            CompanyName = "Test Company",
            ContactName = "Test Contact",
            City = "Test City"
        };
        DbContext.Customers.Add(customer);
        DbContext.SaveChanges();
        return customer;
    }

    [Test]
    public async Task GetAllOrders_ReturnsOkStatus()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            OrderDate = DateTime.Now,
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetOrderById_WithValidId_ReturnsOrder()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            OrderDate = new DateTime(2024, 1, 15),
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var result = await httpResponse.Content.ReadAsAsync<Order>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.CustomerId, Is.EqualTo("TESTCUST"));
    }

    [Test]
    public async Task CreateOrder_WithValidData_ReturnsCreated()
    {
        // Arrange
        SetupTestCustomer();
        var newOrder = new 
        { 
            customerId = "TESTCUST",
            orderDate = DateTime.Now,
            freight = 10.00
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newOrder);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateOrder_WithValidId_UpdatesFreight()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Freight = 10.00m,
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        var updateData = new { freight = 25.50 };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetOrders_WithDateFilter_ReturnsFilteredResults()
    {
        // Arrange
        var customer = SetupTestCustomer();
        DbContext.Orders.AddRange(
            new Order 
            { 
                OrderId = 1,
                CustomerId = "TESTCUST",
                OrderDate = new DateTime(2024, 1, 1),
                Customer = customer
            },
            new Order 
            { 
                OrderId = 2,
                CustomerId = "TESTCUST",
                OrderDate = new DateTime(2024, 6, 1),
                Customer = customer
            }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "orderDate ge 2024-06-01");

        // Act
        var response = await GetODataCollectionAsync<Order>(url);

        // Assert
        Assert.That(response.Value.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetOrders_WithExpandCustomer_IncludesCustomerData()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "customer");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

---

## Step 7: Create Employees Controller Tests

**File**: `NorthwindAspire.Backend.Tests/Controllers/EmployeesControllerTests.cs`

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class EmployeesControllerTests : ODataApiTestBase
{
    private const string EntitySet = "employees";

    [Test]
    public async Task GetAllEmployees_ReturnsOkStatus()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Sales Manager"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetEmployeeById_WithValidId_ReturnsEmployee()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "Nancy",
            LastName = "Davolio",
            Title = "Sales Representative"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var result = await httpResponse.Content.ReadAsAsync<Employee>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.LastName, Is.EqualTo("Davolio"));
    }

    [Test]
    public async Task CreateEmployee_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newEmployee = new 
        { 
            firstName = "Michael",
            lastName = "Scott",
            title = "Manager",
            hireDate = DateTime.Now
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newEmployee);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateEmployee_WithValidId_UpdatesTitle()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Salesman"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        var updateData = new { title = "Senior Sales Manager" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var updated = await DbContext.Employees.FindAsync(1);
        Assert.That(updated?.Title, Is.EqualTo("Senior Sales Manager"));
    }

    [Test]
    public async Task DeleteEmployee_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "Test",
            LastName = "Employee",
            Title = "Tester"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetEmployees_FilterByTitle_ReturnsMatchingEmployees()
    {
        // Arrange
        DbContext.Employees.AddRange(
            new Employee { EmployeeId = 1, FirstName = "John", LastName = "Manager", Title = "Sales Manager" },
            new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Rep", Title = "Sales Representative" }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "title eq 'Sales Manager'");

        // Act
        var response = await GetODataCollectionAsync<Employee>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmployees_ExpandOrders_IncludesRelatedOrders()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Sales Manager"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "orders");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

---

## Step 8: Create Base Test Fixtures and Utilities

**File**: `NorthwindAspire.Backend.Tests/Fixtures/ODataTestData.cs`

```csharp
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Tests.Fixtures;

public static class ODataTestData
{
    public static void SeedTestCategories(NorthwindContext context)
    {
        var categories = new List<Category>
        {
            new() { CategoryId = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas" },
            new() { CategoryId = 2, CategoryName = "Condiments", Description = "Sweet and savory sauces" },
            new() { CategoryId = 3, CategoryName = "Confections", Description = "Desserts and candy" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    public static void SeedTestCustomers(NorthwindContext context)
    {
        var customers = new List<Customer>
        {
            new() 
            { 
                CustomerId = "ALFKI", 
                CompanyName = "Alfreds Futterkiste", 
                ContactName = "Maria Anders",
                City = "Berlin"
            },
            new() 
            { 
                CustomerId = "BLONP", 
                CompanyName = "Blondesddsl", 
                ContactName = "Frédérique Citeaux",
                City = "Strasbourg"
            },
            new() 
            { 
                CustomerId = "BOLID", 
                CompanyName = "Bólido Comidas preparadas", 
                ContactName = "Martín Sommer",
                City = "Madrid"
            }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();
    }

    public static void SeedTestProducts(NorthwindContext context)
    {
        var products = new List<Product>
        {
            new() 
            { 
                ProductId = 1, 
                ProductName = "Chai", 
                UnitPrice = 18.00m,
                UnitsInStock = 39
            },
            new() 
            { 
                ProductId = 2, 
                ProductName = "Chang", 
                UnitPrice = 19.00m,
                UnitsInStock = 17
            },
            new() 
            { 
                ProductId = 3, 
                ProductName = "Aniseed Syrup", 
                UnitPrice = 10.00m,
                UnitsInStock = 13
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }

    public static void SeedTestData(NorthwindContext context)
    {
        SeedTestCategories(context);
        SeedTestCustomers(context);
        SeedTestProducts(context);
    }
}
```

---

## Step 9: Configure Test Runner Settings

**File**: `NorthwindAspire.Backend.Tests/nunit.runsettings`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <TestRunParameters>
    <Parameter name="ApiBaseUrl" value="http://localhost:5000" />
    <Parameter name="ODataBaseUrl" value="/odata" />
  </TestRunParameters>
  
  <NUnit>
    <NumberOfTestWorkers>1</NumberOfTestWorkers>
    <TimeoutInSeconds>30</TimeoutInSeconds>
  </NUnit>

  <LoggerRunSettings>
    <Verbosity>Normal</Verbosity>
  </LoggerRunSettings>
</RunSettings>
```

---

## Step 10: Add CI/CD Pipeline Configuration

**File**: `.github/workflows/test-odata-api.yml`

```yaml
name: OData API Tests

on:
  push:
    branches: [ master, develop ]
  pull_request:
    branches: [ master, develop ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    strategy:
      matrix:
        dotnet-version: [ '10.0.x' ]

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ matrix.dotnet-version }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run OData API Tests
      run: dotnet test NorthwindAspire.Backend.Tests/NorthwindAspire.Backend.Tests.csproj --no-build --logger="console;verbosity=detailed" --filter "Category!=Integration"
    
    - name: Run Integration Tests
      run: dotnet test NorthwindAspire.Backend.Tests/NorthwindAspire.Backend.Tests.csproj --no-build --logger="console;verbosity=detailed" --filter "Category=Integration"
    
    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: test-results
        path: '**/TestResults/*.trx'
```

---

## Step 11: Add Azure DevOps Pipeline

**File**: `azure-pipelines-tests.yml`

```yaml
trigger:
  branches:
    include:
    - master
    - develop
  paths:
    include:
    - src/NorthwindAspire.Backend/**
    - NorthwindAspire.Backend.Tests/**
    - azure-pipelines-tests.yml

pr:
  branches:
    include:
    - master
    - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'

stages:
- stage: Build
  displayName: Build and Test
  jobs:
  - job: BuildAndTest
    displayName: Build and Run Tests
    steps:
    - task: UseDotNet@2
      displayName: 'Setup .NET SDK'
      inputs:
        version: $(dotnetVersion)

    - task: DotNetCoreCLI@2
      displayName: 'Restore NuGet packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'

    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        arguments: '--configuration $(buildConfiguration) --no-restore'

    - task: DotNetCoreCLI@2
      displayName: 'Run Unit Tests'
      inputs:
        command: 'test'
        projects: 'NorthwindAspire.Backend.Tests/NorthwindAspire.Backend.Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build --logger trx --logger console --collect:"XPlat Code Coverage"'
        publishTestResults: true

    - task: PublishCodeCoverageResults@1
      displayName: 'Publish Code Coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/*coverage.cobertura.xml'

    - task: PublishBuildArtifacts@1
      displayName: 'Publish Test Results'
      condition: always()
      inputs:
        PathtoPublish: '$(Build.ArtifactStagingDirectory)'
        ArtifactName: 'test-results'

- stage: Report
  displayName: Generate Reports
  dependsOn: Build
  condition: succeeded()
  jobs:
  - job: ReportResults
    displayName: Test Report
    steps:
    - task: PublishTestResults@2
      displayName: 'Publish Test Results'
      inputs:
        testResultsFormat: 'NUnit'
        testResultsFiles: '**/*.trx'
```

---

## Step 12: Run Tests Locally

### Execute All Tests
```bash
cd NorthwindAspire.Backend.Tests
dotnet test --verbosity normal
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~NorthwindAspire.Backend.Tests.Controllers.CategoriesControllerTests"
```

### Run Tests with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### Run Tests in Watch Mode
```bash
dotnet watch test
```

---

## Test Controller Template

Use this template for creating additional controller tests:

```csharp
using System.Net;
using NorthwindAspire.Backend.Models;
using NUnit.Framework;

namespace NorthwindAspire.Backend.Tests.Controllers;

[TestFixture]
public class [EntityName]ControllerTests : ODataApiTestBase
{
    private const string EntitySet = "[entitysetlowercase]";

    [Test]
    public async Task GetAll[EntityName]_ReturnsOkStatus()
    {
        // Arrange
        var entity = new [EntityName] { /* Initialize properties */ };
        DbContext.[EntitySet].Add(entity);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get[EntityName]ById_WithValidId_Returns[EntityName]()
    {
        // Arrange
        var entity = new [EntityName] { /* Initialize properties */ };
        DbContext.[EntitySet].Add(entity);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(key)");
        var result = await httpResponse.Content.ReadAsAsync<[EntityName]>();

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Create[EntityName]_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newEntity = new { /* Initialize properties */ };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newEntity);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task Update[EntityName]_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var entity = new [EntityName] { /* Initialize properties */ };
        DbContext.[EntitySet].Add(entity);
        await DbContext.SaveChangesAsync();

        var updateData = new { /* Update properties */ };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(key)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Delete[EntityName]_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var entity = new [EntityName] { /* Initialize properties */ };
        DbContext.[EntitySet].Add(entity);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(key)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Get[EntityName]_WithFilter_ReturnsFilteredResults()
    {
        // Arrange
        var url = BuildODataUrl(EntitySet, filter: "property eq 'value'");

        // Act
        var response = await GetODataCollectionAsync<[EntityName]>(url);

        // Assert
        Assert.That(response.Value, Is.Not.Empty);
    }
}
```

---

## Test Coverage Goals

### Target Coverage Metrics
- **Overall Code Coverage**: 80%+
- **Controller Coverage**: 95%+
- **Data Access Coverage**: 90%+
- **Business Logic Coverage**: 85%+

### Coverage Report Generation
```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura /p:Exclude="[NorthwindAspire.Backend.Tests]*"

# View HTML report
# Report generated in coverage/index.html
```

---

## Best Practices

### Test Naming Conventions
```csharp
[MethodName]_[Scenario]_[ExpectedResult]

Examples:
- CreateCategory_WithValidData_ReturnsCreated()
- GetCategoryById_WithInvalidId_ReturnsNotFound()
- UpdateCategory_WithValidId_UpdatesSuccessfully()
```

### Arrange-Act-Assert Pattern
```csharp
[Test]
public async Task TestMethod()
{
    // Arrange - Setup test data
    var data = new TestData();

    // Act - Execute the code
    var result = await ExecuteAction(data);

    // Assert - Verify results
    Assert.That(result, Is.EqualTo(expected));
}
```

### Test Data Management
- Use `SetUp` for test-specific data
- Use `OneTimeSetUp` for shared test resources
- Clean database between tests
- Use helper methods for common data setup

### Async Test Best Practices
```csharp
// All API tests should be async
[Test]
public async Task GetData_ReturnsSuccessfully()
{
    var result = await HttpClient.GetAsync(url);
    // Assert
}
```

---

## Common Test Scenarios

### Entity CRUD Operations
- ✓ Create with valid data
- ✓ Create with invalid data
- ✓ Read single entity by ID
- ✓ Read all entities
- ✓ Update entity
- ✓ Delete entity

### OData Query Operations
- ✓ Filter with equals operator
- ✓ Filter with comparison operators (gt, lt, ge, le)
- ✓ Filter with contains function
- ✓ Select specific fields
- ✓ Expand related entities
- ✓ Order by single/multiple fields
- ✓ Pagination with $top/$skip
- ✓ Count total records

### Error Scenarios
- ✓ Not Found (404)
- ✓ Bad Request (400)
- ✓ Unauthorized (401)
- ✓ Server Error (500)
- ✓ Validation errors

---

## Troubleshooting

### Issue: Tests timeout
**Solution**: Increase timeout in nunit.runsettings or use `[Timeout(60000)]` attribute

### Issue: Database locking
**Solution**: Ensure `SaveChangesAsync()` is awaited properly

### Issue: Null reference exceptions
**Solution**: Verify test data is properly seeded before assertions

### Issue: Flaky tests
**Solution**: Ensure test isolation by clearing database in `SetUp`

---

## Running Tests in CI/CD

### GitHub Actions
```bash
dotnet test --verbosity normal --logger=trx
```

### Azure DevOps
```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: '**/NorthwindAspire.Backend.Tests.csproj'
```

### Local with Azure CLI
```bash
az pipelines run --name "OData API Tests" --branch master
```

---

## Performance Testing

### Load Testing Example
```csharp
[Test]
public async Task GetCategories_Under100ms()
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var response = await HttpClient.GetAsync("/odata/categories");
    stopwatch.Stop();

    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100));
}
```

---

## Documentation

- Test Results: Published to Azure DevOps test runs
- Code Coverage: Generated as Cobertura XML
- Test Reports: Available in CI/CD pipelines
- Test Logs: Available in build artifacts

---

## Next Steps

1. Create test project using guide above
2. Implement base test classes
3. Create controller-specific test classes
4. Configure CI/CD pipelines
5. Run tests locally and in pipeline
6. Generate coverage reports
7. Achieve target coverage goals
8. Integrate with code quality tools

---

## Additional Resources

- [NUnit Documentation](https://docs.nunit.org/)
- [Playwright Documentation](https://playwright.dev/dotnet/)
- [Microsoft Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/)
- [OData Testing Patterns](https://odata.github.io/)
- [Azure DevOps Testing](https://learn.microsoft.com/en-us/azure/devops/test/index)
