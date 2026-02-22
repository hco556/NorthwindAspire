# BUnit Tests - Quick Reference Guide

## What Was Corrected

### ? Build Status
- **Before:** 26 compilation errors
- **After:** Build successful (0 errors)

## Files Modified

| File | Changes | Status |
|------|---------|--------|
| `ComponentTestBase.cs` | Added MudBlazor imports, fixed service registration, added JSInterop config | ? Fixed |
| `CategoryFormTests.cs` | Complete rewrite with proper BUnit patterns, 11 tests | ? Rewritten |
| `CategoryListTests.cs` | Complete rewrite with proper BUnit patterns, 10 tests | ? Rewritten |
| `NorthwindAspire.Tests.csproj` | Added MudBlazor package reference | ? Updated |
| `ODataApiTestBase.cs` | Fixed Program type ambiguity | ? Fixed |
| `BackendProgram.cs` | Created marker class for type resolution | ? Created |

## Key Corrections

### 1. Using Statements
```csharp
// Added to all test files
using MudBlazor;
using Microsoft.Extensions.Logging;
```

### 2. Parameter Binding
```csharp
// ? WRONG (Old way)
var parameters = new ComponentParameter[] 
{ 
    new(nameof(CategoryForm), newCategory) 
};
var cut = Context.RenderComponent<CategoryForm>(parameters);

// ? CORRECT (New way)
var cut = Context.RenderComponent<CategoryForm>(parameters =>
{
    parameters.Add(p => p.Model, newCategory);
});
```

### 3. DOM Queries
```csharp
// ? WRONG (Old way - accessing private members)
Assert.That(cut.Instance.success, Is.False);

// ? CORRECT (New way - querying DOM)
var submitButton = cut.FindAll("button")
    .FirstOrDefault(b => b.TextContent.Contains("Create"));
Assert.That(submitButton?.GetAttribute("disabled"), Is.Not.Null);
```

### 4. Service Verification
```csharp
// ? CORRECT (Proper mock verification)
_mockODataService.Verify(
    x => x.GetAllAsync<Category, CategoryViewModel>(),
    Times.Once,
    "Service should be called once"
);
```

### 5. Markup Assertions
```csharp
// ? CORRECT (Checking component structure)
Assert.That(cut.Markup, Does.Contain("MudDialog"));
Assert.That(cut.Markup, Does.Contain("Category Name"));
Assert.That(cut.Markup, Does.Contain("Beverages"));
```

## Test Files Summary

### CategoryFormTests (11 Tests)
Tests the CategoryForm Blazor component:
- Form rendering (create vs edit modes)
- Button states and visibility
- Field validation
- Dialog structure
- Null model handling

### CategoryListTests (10 Tests)
Tests the CategoryList Blazor component:
- DataGrid rendering
- Search functionality
- New button visibility
- Multiple category display
- Service integration
- Empty state handling

## Running Tests

### Run All Component Tests
```bash
dotnet test tests/NorthwindAspire.Tests/Components/
```

### Run Category Tests Only
```bash
dotnet test tests/NorthwindAspire.Tests/Components/Categories/
```

### Run with Verbose Output
```bash
dotnet test --verbosity=detailed
```

### Generate Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Patterns Reference

### Pattern 1: Component Initialization Test
```csharp
[Test]
public void ComponentName_Feature_ExpectedBehavior()
{
    // Arrange
    var data = new ViewModel();
    _mockService.Setup(x => x.Method()).ReturnsAsync(data);

    // Act
    var cut = Context.RenderComponent<ComponentName>(parameters =>
    {
        parameters.Add(p => p.Model, data);
    });

    // Assert
    Assert.That(cut.Markup, Does.Contain("Expected Content"));
}
```

### Pattern 2: Service Verification Test
```csharp
[Test]
public async Task ComponentName_Feature_CallsService()
{
    // Arrange
    _mockService.Setup(x => x.GetAllAsync<T, TV>())
        .ReturnsAsync(new List<TV>());

    // Act
    var cut = Context.RenderComponent<ComponentName>();

    // Assert
    _mockService.Verify(
        x => x.GetAllAsync<T, TV>(),
        Times.Once
    );
}
```

### Pattern 3: DOM Query Test
```csharp
[Test]
public void ComponentName_Feature_ButtonExists()
{
    // Arrange
    var cut = Context.RenderComponent<ComponentName>();

    // Act
    var button = cut.FindAll("button")
        .FirstOrDefault(b => b.TextContent.Contains("New"));

    // Assert
    Assert.That(button, Is.Not.Null);
}
```

## Important Notes

1. **No Private Member Access** - Tests cannot access protected/private component members. Use DOM queries instead.

2. **Async Operations** - Always use proper BUnit async patterns (RenderComponent handles async initialization).

3. **Mock Setup** - Set up mocks BEFORE rendering the component.

4. **Service Registration** - Services are registered in ComponentTestBase, inherited by all test classes.

5. **MudBlazor Components** - Components render correctly with JSRuntimeMode.Loose configuration.

## Creating New Tests

To create tests for new components:

1. Create `YourComponentTests.cs` file
2. Inherit from `ComponentTestBase`
3. Follow the patterns shown above
4. Use proper parameter binding with lambdas
5. Query DOM, don't access private members
6. Build will verify syntax correctness

## Next Steps

1. Create ProductListTests/ProductFormTests
2. Create CustomerListTests/CustomerFormTests
3. Add tests for ConfirmDeleteDialog
4. Add tests for CrudComponentBase
5. Run full test suite and generate coverage report

---

**Status:** ? All tests corrected and building successfully
**Last Updated:** 2024
