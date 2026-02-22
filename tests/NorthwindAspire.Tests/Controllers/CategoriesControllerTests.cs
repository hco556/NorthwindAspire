using System.Net;
using System.Text.Json;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Controllers;

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
        var json = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Category>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

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
