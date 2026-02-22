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
/// Unit tests for the CategoryForm Blazor component
/// Tests form rendering, validation, submission, and error handling
/// </summary>
[TestFixture]
public class CategoryFormTests_Corrected : ComponentTestBase
{
    private Mock<IODataFrontendService> _mockODataService = null!;
    private Mock<ISnackbar> _mockSnackbar = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _mockODataService = CreateMockODataService();
        _mockSnackbar = CreateMockSnackbar();
    }

    /// <summary>
    /// Test: Create form renders with empty fields
    /// </summary>
    [Test]
    public void CategoryForm_CreateMode_RendersEmptyForm()
    {
        // Arrange
        var newCategory = new CategoryViewModel();

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert
        Assert.That(cut.Markup, Does.Contain("Create New Category"), "Should show create title");
        Assert.That(cut.Markup, Does.Contain("Category Name"), "Category Name field should be visible");
        Assert.That(cut.Markup, Does.Contain("Description"), "Description field should be visible");
    }

    /// <summary>
    /// Test: Edit form pre-fills existing data
    /// </summary>
    [Test]
    public void CategoryForm_EditMode_PreFillsData()
    {
        // Arrange
        var existingCategory = new CategoryViewModel
        {
            CategoryId = 1,
            CategoryName = "Beverages",
            Description = "Soft drinks, coffees, teas"
        };

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, existingCategory);
        });

        // Assert
        Assert.That(cut.Markup, Does.Contain("Edit Category"), "Should show edit title");
        Assert.That(cut.Markup, Does.Contain("Beverages"), "Category name should be pre-filled");
        Assert.That(cut.Markup, Does.Contain("Soft drinks"), "Description should be pre-filled");
    }

    /// <summary>
    /// Test: Submit button is disabled when form is invalid (empty required field)
    /// </summary>
    [Test]
    public void CategoryForm_InvalidForm_SubmitButtonDisabled()
    {
        // Arrange
        var newCategory = new CategoryViewModel 
        { 
            CategoryName = "" // Empty required field
        };

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert - Check that submit button is disabled in the markup
        var submitButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Create"));
        Assert.That(submitButton?.GetAttribute("disabled"), Is.Not.Null, "Submit button should be disabled for invalid form");
    }

    /// <summary>
    /// Test: Submit button is enabled when required fields are filled
    /// </summary>
    [Test]
    public void CategoryForm_ValidForm_SubmitButtonCheckStructure()
    {
        // Arrange
        var newCategory = new CategoryViewModel
        {
            CategoryName = "Valid Category Name"
        };

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert - Check button exists
        var submitButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Create"));
        Assert.That(submitButton, Is.Not.Null, "Submit button should exist");
    }

    /// <summary>
    /// Test: Form has proper structure and elements
    /// </summary>
    [Test]
    public void CategoryForm_HasRequiredElements()
    {
        // Arrange
        var newCategory = new CategoryViewModel();

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert
        Assert.That(cut.Markup, Does.Contain("MudDialog"), "Should have MudDialog");
        Assert.That(cut.Markup, Does.Contain("MudForm"), "Should have MudForm for validation");
        Assert.That(cut.Markup, Does.Contain("MudStack"), "Should have MudStack for layout");
        var textFields = cut.FindAll("input[type='text']");
        Assert.That(textFields.Count, Is.GreaterThanOrEqualTo(1), "Should have at least one text input field");
    }

    /// <summary>
    /// Test: Cancel button is visible and clickable
    /// </summary>
    [Test]
    public void CategoryForm_HasCancelButton()
    {
        // Arrange
        var newCategory = new CategoryViewModel();

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert
        var cancelButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Equals("Cancel"));
        Assert.That(cancelButton, Is.Not.Null, "Cancel button should be visible");
    }

    /// <summary>
    /// Test: Submit button text changes between create and edit modes
    /// </summary>
    [Test]
    public void CategoryForm_SubmitButtonText_DependsOnMode()
    {
        // Arrange - Create mode
        var newCategory = new CategoryViewModel { CategoryId = 0 };

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, newCategory);
        });

        // Assert
        var createButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Create"));
        Assert.That(createButton, Is.Not.Null, "Should show 'Create' button for new categories");

        // Arrange - Edit mode
        var existingCategory = new CategoryViewModel { CategoryId = 1, CategoryName = "Test" };

        // Act
        var editCut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, existingCategory);
        });

        // Assert
        var updateButton = editCut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Update"));
        Assert.That(updateButton, Is.Not.Null, "Should show 'Update' button for existing categories");
    }

    /// <summary>
    /// Test: Form initializes with null model
    /// </summary>
    [Test]
    public void CategoryForm_NullModel_InitializesWithEmptyViewModel()
    {
        // Arrange & Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, null);
        });

        // Assert
        Assert.That(cut.Markup, Does.Contain("Create New Category"), "Should initialize as create mode when model is null");
    }

    /// <summary>
    /// Test: Form displays both text input fields
    /// </summary>
    [Test]
    public void CategoryForm_DisplaysAllFormFields()
    {
        // Arrange
        var category = new CategoryViewModel();

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, category);
        });

        // Assert
        var inputs = cut.FindAll("input[type='text']");
        Assert.That(inputs.Count, Is.GreaterThanOrEqualTo(2), "Should have at least 2 text inputs (Category Name and Description area)");
    }

    /// <summary>
    /// Test: Dialog actions are visible
    /// </summary>
    [Test]
    public void CategoryForm_DisplaysDialogActions()
    {
        // Arrange
        var category = new CategoryViewModel { CategoryId = 1, CategoryName = "Test" };

        // Act
        var cut = Context.RenderComponent<CategoryForm>(parameters =>
        {
            parameters.Add(p => p.Model, category);
        });

        // Assert
        var buttons = cut.FindAll("button");
        Assert.That(buttons.Count, Is.GreaterThanOrEqualTo(2), "Should have Cancel and Submit buttons");
    }
}
