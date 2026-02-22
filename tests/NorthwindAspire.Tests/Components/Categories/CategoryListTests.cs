using Bunit;
using Moq;
using MudBlazor;
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Components.CRUD.Categories;
using NorthwindAspire.Frontend.Models.ViewModels;
using NorthwindAspire.Frontend.Services;
using NUnit.Framework;

namespace NorthwindAspire.Tests.Components.Categories;

/// <summary>
/// Unit tests for the CategoryList Blazor component
/// Tests rendering, data loading, user interactions, and error handling
/// </summary>
[TestFixture]
public class CategoryListTests_Corrected : ComponentTestBase
{
    private Mock<IODataFrontendService> _mockODataService = null!;
    private Mock<ISnackbar> _mockSnackbar = null!;
    private Mock<IDialogService> _mockDialogService = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _mockODataService = CreateMockODataService();
        _mockSnackbar = CreateMockSnackbar();
        _mockDialogService = CreateMockDialogService();
    }

    /// <summary>
    /// Test: Component renders data grid for displaying categories
    /// </summary>
    [Test]
    public void CategoryList_RendersDataGrid()
    {
        // Arrange
        var categories = new List<CategoryViewModel>
        {
            new() { CategoryId = 1, CategoryName = "Beverages" }
        };

        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(categories);

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        Assert.That(cut.Markup, Does.Contain("MudDataGrid"), "DataGrid should be rendered");
    }

    /// <summary>
    /// Test: New Category button is visible
    /// </summary>
    [Test]
    public void CategoryList_DisplaysNewCategoryButton()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        var button = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("New Category"));
        Assert.That(button, Is.Not.Null, "New Category button should be visible");
    }

    /// <summary>
    /// Test: Search input field is rendered
    /// </summary>
    [Test]
    public void CategoryList_SearchTextField_IsRendered()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        var searchInputs = cut.FindAll("input");
        Assert.That(searchInputs.Count, Is.GreaterThanOrEqualTo(1), "Should have at least one input field for search");
    }

    /// <summary>
    /// Test: Component handles empty data gracefully
    /// </summary>
    [Test]
    public void CategoryList_HandlesEmptyData()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        Assert.That(cut.Markup, Does.Contain("MudDataGrid"), "DataGrid should render even with no data");
    }

    /// <summary>
    /// Test: Component has container structure
    /// </summary>
    [Test]
    public void CategoryList_HasContainerStructure()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        Assert.That(cut.Markup, Does.Contain("MudContainer"), "Should have MudContainer");
    }

    /// <summary>
    /// Test: Component renders stack layout
    /// </summary>
    [Test]
    public void CategoryList_HasStackLayout()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        Assert.That(cut.Markup, Does.Contain("MudStack"), "Should have MudStack for layout");
    }

    /// <summary>
    /// Test: Multiple categories display in data grid
    /// </summary>
    [Test]
    public void CategoryList_DisplaysMultipleCategories()
    {
        // Arrange
        var categories = new List<CategoryViewModel>
        {
            new() { CategoryId = 1, CategoryName = "Beverages" },
            new() { CategoryId = 2, CategoryName = "Condiments" },
            new() { CategoryId = 3, CategoryName = "Confections" }
        };

        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(categories);

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        foreach (var category in categories)
        {
            Assert.That(cut.Markup, Does.Contain(category.CategoryName), 
                $"Category '{category.CategoryName}' should be displayed");
        }
    }

    /// <summary>
    /// Test: OData service is called to load categories
    /// </summary>
    [Test]
    public void CategoryList_CallsODataService()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        _mockODataService.Verify(
            x => x.GetAllAsync<Category, CategoryViewModel>(),
            Times.Once,
            "OData service should be called to load categories"
        );
    }

    /// <summary>
    /// Test: Component has required toolbar area for actions
    /// </summary>
    [Test]
    public void CategoryList_HasToolbar()
    {
        // Arrange
        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        var buttons = cut.FindAll("button");
        Assert.That(buttons.Count, Is.GreaterThanOrEqualTo(1), "Should have at least one button (New Category)");
    }

    /// <summary>
    /// Test: Search functionality is available
    /// </summary>
    [Test]
    public void CategoryList_HasSearchCapability()
    {
        // Arrange
        var categories = new List<CategoryViewModel>
        {
            new() { CategoryId = 1, CategoryName = "Beverages" }
        };

        _mockODataService
            .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
            .ReturnsAsync(categories);

        // Act
        var cut = Context.RenderComponent<CategoryList>();

        // Assert
        var textInputs = cut.FindAll("input[type='text']");
        Assert.That(textInputs.Count, Is.GreaterThanOrEqualTo(1), "Should have text input for search");
    }
}
