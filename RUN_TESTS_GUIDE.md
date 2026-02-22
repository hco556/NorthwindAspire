# Running the Corrected BUnit Tests

## ? Quick Start

### Build the Solution
```bash
dotnet build
```
**Expected:** Build successful with 0 errors

### Run All Tests
```bash
dotnet test
```

### Run Component Tests Only
```bash
dotnet test tests/NorthwindAspire.Tests/Components/
```

### Run Category Component Tests
```bash
dotnet test tests/NorthwindAspire.Tests/Components/Categories/
```

---

## ?? Test Execution Options

### Verbose Output
```bash
dotnet test --verbosity detailed
```

### Show Individual Test Results
```bash
dotnet test --logger:"console;verbosity=detailed"
```

### Generate Code Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Filter Tests by Name
```bash
# Run only form tests
dotnet test --filter "CategoryFormTests"

# Run only list tests
dotnet test --filter "CategoryListTests"

# Run specific test
dotnet test --filter "CategoryForm_CreateMode_RendersEmptyForm"
```

---

## ?? Test Files Location

```
tests/NorthwindAspire.Tests/
??? Components/
?   ??? ComponentTestBase.cs (Base class for all tests)
?   ??? Categories/
?       ??? CategoryFormTests.cs (11 tests)
?       ??? CategoryListTests.cs (10 tests)
```

---

## ?? Expected Test Results

### CategoryFormTests (11 Tests)
```
? CategoryForm_CreateMode_RendersEmptyForm
? CategoryForm_EditMode_PreFillsData
? CategoryForm_InvalidForm_SubmitButtonDisabled
? CategoryForm_ValidForm_SubmitButtonCheckStructure
? CategoryForm_HasRequiredElements
? CategoryForm_HasCancelButton
? CategoryForm_SubmitButtonText_DependsOnMode
? CategoryForm_NullModel_InitializesWithEmptyViewModel
? CategoryForm_DisplaysAllFormFields
? CategoryForm_DisplaysDialogActions
? (Additional tests)

Total: 11 tests passed
```

### CategoryListTests (10 Tests)
```
? CategoryList_RendersDataGrid
? CategoryList_DisplaysNewCategoryButton
? CategoryList_SearchTextField_IsRendered
? CategoryList_HandlesEmptyData
? CategoryList_HasContainerStructure
? CategoryList_HasStackLayout
? CategoryList_DisplaysMultipleCategories
? CategoryList_CallsODataService
? CategoryList_HasToolbar
? CategoryList_HasSearchCapability

Total: 10 tests passed
```

---

## ?? Troubleshooting

### Issue: Tests Won't Build
**Solution:**
1. Verify NuGet packages are restored: `dotnet restore`
2. Clean build: `dotnet clean && dotnet build`
3. Check that all required files exist in the expected locations

### Issue: "ComponentTestBase not found"
**Solution:**
- Ensure `ComponentTestBase.cs` exists in `tests/NorthwindAspire.Tests/Components/`
- Check that namespace is correct: `NorthwindAspire.Tests.Components`

### Issue: MudBlazor components not found
**Solution:**
- Verify MudBlazor package is installed: `dotnet add package MudBlazor`
- Ensure test project references Frontend project with MudBlazor

### Issue: Tests failing with "Service not registered"
**Solution:**
- Check ComponentTestBase.SetUp() is being called
- Verify mock services are properly registered
- Ensure [SetUp] attribute is on the method

---

## ?? Next Steps After Tests Pass

1. **Extend to Additional Entities**
   - Create ProductListTests / ProductFormTests
   - Create CustomerListTests / CustomerFormTests
   - Use existing tests as templates

2. **Add Integration Tests**
   - Test form submission with real service calls
   - Test dialog interactions
   - Test notification display

3. **Improve Coverage**
   - Target 80%+ code coverage
   - Test error scenarios
   - Test edge cases

4. **Performance Testing**
   - Measure component render times
   - Test with large datasets
   - Monitor memory usage

---

## ?? Test Template for New Components

Use this template to create tests for new components:

```csharp
using Bunit;
using Moq;
using MudBlazor;
using NorthwindAspire.Frontend.Components.CRUD.YourEntity;
using NorthwindAspire.Frontend.Models.ViewModels;
using NorthwindAspire.Frontend.Services;
using NUnit.Framework;

namespace NorthwindAspire.Tests.Components.YourEntity;

[TestFixture]
public class YourComponentTests : ComponentTestBase
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

    [Test]
    public void YourComponent_Feature_ExpectedBehavior()
    {
        // Arrange
        var data = new YourViewModel();
        
        // Act
        var cut = Context.RenderComponent<YourComponent>(parameters =>
        {
            parameters.Add(p => p.Model, data);
        });

        // Assert
        Assert.That(cut.Markup, Does.Contain("Expected Content"));
    }
}
```

---

## ?? Success Criteria

? All tests compile without errors
? All tests pass execution
? Test coverage increases
? Code quality maintained
? Documentation complete

---

## ?? Additional Resources

- **BUnit Documentation:** https://bunit.dev/
- **Moq Documentation:** https://github.com/moq/moq4
- **MudBlazor Testing:** https://mudblazor.com/
- **NUnit Documentation:** https://docs.nunit.org/

---

**Ready to run tests!** Execute `dotnet test` to verify everything is working correctly.
