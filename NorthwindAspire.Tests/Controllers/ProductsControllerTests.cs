using System.Net;
using System.Text.Json;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Controllers;

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
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Request failed with {response.StatusCode}: {content}");
        }
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
        var json = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

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
        await RefreshEntityAsync(updated!);
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
