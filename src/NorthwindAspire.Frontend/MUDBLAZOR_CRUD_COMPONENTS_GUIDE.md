# MudBlazor CRUD Components Implementation Guide

## Overview

This guide provides instructions for creating MudBlazor CRUD (Create, Read, Update, Delete) components based on your ViewModels. These components will integrate seamlessly with the `IODataFrontendService` for backend communication and use the `MapperRegistry` for data transformation.

## Architecture

The component structure will follow this pattern:
- **List Component** - Displays all records in a MudDataGrid with sorting, filtering, and pagination
- **Create Dialog** - Modal form for creating new records
- **Edit Dialog** - Modal form for updating existing records
- **Delete Dialog** - Confirmation dialog for deleting records
- **Container Component** - Manages the list and orchestrates CRUD operations

## Directory Structure

```
src/NorthwindAspire.Frontend/Components/
??? CRUD/
?   ??? Categories/
?   ?   ??? CategoryList.razor
?   ?   ??? CategoryList.razor.cs
?   ?   ??? CategoryForm.razor
?   ?   ??? CategoryForm.razor.cs
?   ??? Products/
?   ?   ??? ProductList.razor
?   ?   ??? ProductList.razor.cs
?   ?   ??? ProductForm.razor
?   ?   ??? ProductForm.razor.cs
?   ??? Customers/
?   ?   ??? CustomerList.razor
?   ?   ??? CustomerList.razor.cs
?   ?   ??? CustomerForm.razor
?   ?   ??? CustomerForm.razor.cs
?   ??? Shared/
?       ??? ConfirmDeleteDialog.razor
```

## Implementation Steps

### Step 1: Create Base CRUD Component Code-Behind Class

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/CrudComponentBase.cs`

This base class provides common CRUD functionality for all entity components:

```csharp
using MudBlazor;
using NorthwindAspire.Frontend.Services;

namespace NorthwindAspire.Frontend.Components.CRUD;

public abstract class CrudComponentBase<TModel, TViewModel> : ComponentBase
    where TModel : class
    where TViewModel : class
{
    [Inject]
    protected IODataFrontendService ODataService { get; set; } = null!;

    [Inject]
    protected IDialogService DialogService { get; set; } = null!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = null!;

    protected List<TViewModel> Items = new();
    protected bool IsLoading;
    protected string SearchString = "";
    protected MudDataGrid<TViewModel>? DataGrid;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected virtual async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = await ODataService.GetAllAsync<TModel, TViewModel>();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading data: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected virtual async Task OpenCreateDialogAsync()
    {
        var dialog = await DialogService.ShowAsync<TViewModel>(
            $"Create New {typeof(TViewModel).Name.Replace("ViewModel", "")}",
            new DialogParameters { ["Model"] = CreateNewViewModel() });

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadDataAsync();
            Snackbar.Add("Record created successfully", Severity.Success);
        }
    }

    protected virtual async Task OpenEditDialogAsync(TViewModel item)
    {
        var dialog = await DialogService.ShowAsync<TViewModel>(
            $"Edit {typeof(TViewModel).Name.Replace("ViewModel", "")}",
            new DialogParameters { ["Model"] = item });

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadDataAsync();
            Snackbar.Add("Record updated successfully", Severity.Success);
        }
    }

    protected virtual async Task OpenDeleteDialogAsync(object keyValue)
    {
        var dialog = await DialogService.ShowAsync<ConfirmDeleteDialog>(
            "Delete Record",
            new DialogParameters { ["EntityName"] = typeof(TViewModel).Name.Replace("ViewModel", "") });

        var result = await dialog.Result;

        if (!result.Canceled && (bool?)result.Data == true)
        {
            try
            {
                await ODataService.DeleteAsync<TModel>(keyValue);
                await LoadDataAsync();
                Snackbar.Add("Record deleted successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting record: {ex.Message}", Severity.Error);
            }
        }
    }

    protected virtual TViewModel CreateNewViewModel()
    {
        return Activator.CreateInstance<TViewModel>();
    }

    protected bool FilterFunc(TViewModel element)
    {
        if (string.IsNullOrWhiteSpace(SearchString))
            return true;

        return element.ToString()?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
```

### Step 2: Create Shared Delete Confirmation Dialog

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Shared/ConfirmDeleteDialog.razor`

```razor
@using MudBlazor

<MudDialog>
    <TitleContent>
        <MudText Typo="Typo.h6">
            <MudIcon Icon="@Icons.Material.Filled.Delete" Class="mr-3" />
            Confirm Delete
        </MudText>
    </TitleContent>
    <DialogContent>
        <MudText>
            Are you sure you want to delete this @EntityName? This action cannot be undone.
        </MudText>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Error" Variant="Variant.Filled" OnClick="Confirm">
            Delete
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public string EntityName { get; set; } = "record";

    private void Cancel() => MudDialog.Cancel();

    private void Confirm() => MudDialog.Close(true);
}
```

### Step 3: Create Category List Component

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Categories/CategoryList.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Components.CRUD
@inherits CrudComponentBase<Category, CategoryViewModel>

<MudContainer MaxWidth="MaxWidth.Large" Class="py-8">
    <MudStack Spacing="4">
        <!-- Header -->
        <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
            <MudText Typo="Typo.h4">Categories</MudText>
            <MudButton Variant="Variant.Filled" 
                       Color="Color.Primary" 
                       StartIcon="@Icons.Material.Filled.Add"
                       OnClick="OpenCreateDialogAsync">
                New Category
            </MudButton>
        </MudStack>

        <!-- Search Bar -->
        <MudTextField @bind-Value="SearchString"
                      Placeholder="Search categories..."
                      Adornment="Adornment.Start"
                      AdornmentIcon="@Icons.Material.Filled.Search"
                      IconSize="Size.Medium"
                      Class="mt-0" />

        <!-- Data Grid -->
        <MudDataGrid @ref="DataGrid"
                     Items="@Items"
                     Loading="@IsLoading"
                     Hover="true"
                     Striped="true"
                     Bordered="true">
            <Columns>
                <PropertyColumn Property="x => x.CategoryId" Title="ID" Sortable="true" />
                <PropertyColumn Property="x => x.CategoryName" Title="Name" Sortable="true" />
                <PropertyColumn Property="x => x.Description" Title="Description" Sortable="true" />
                <TemplateColumn CellClass="d-flex gap-2">
                    <CellTemplate>
                        <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                       Size="Size.Small" 
                                       Color="Color.Primary"
                                       OnClick="@(() => OpenEditDialogAsync(context.Item))" />
                        <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                       Size="Size.Small" 
                                       Color="Color.Error"
                                       OnClick="@(() => OpenDeleteDialogAsync(context.Item.CategoryId))" />
                    </CellTemplate>
                </TemplateColumn>
            </Columns>
            <PagerContent>
                <MudDataGridPager T="CategoryViewModel" />
            </PagerContent>
        </MudDataGrid>
    </MudStack>
</MudContainer>

@code {
    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }
}
```

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Categories/CategoryList.razor.cs`

```csharp
using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Components.CRUD.Categories;

public partial class CategoryList
{
    // Code-behind logic if needed
}
```

### Step 4: Create Category Form Component (Create/Edit Dialog)

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Categories/CategoryForm.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Services
@using MudBlazor

<MudDialog>
    <TitleContent>
        <MudText Typo="Typo.h6">
            @(Model?.CategoryId == 0 ? "Create New Category" : "Edit Category")
        </MudText>
    </TitleContent>
    <DialogContent>
        <MudForm @ref="form" @bind-IsValid="@success" @bind-Errors="@errors">
            <MudStack Spacing="3">
                <MudTextField @bind-Value="Model!.CategoryName"
                              For="@(() => Model!.CategoryName)"
                              Label="Category Name"
                              Required="true"
                              RequiredError="Category Name is required"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="Model!.Description"
                              For="@(() => Model!.Description)"
                              Label="Description"
                              Lines="3"
                              Variant="Variant.Outlined" />
            </MudStack>
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Primary" 
                   Variant="Variant.Filled" 
                   OnClick="Submit"
                   Disabled="@(!success)">
            @(Model?.CategoryId == 0 ? "Create" : "Update")
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public CategoryViewModel? Model { get; set; }

    [Inject]
    protected IODataFrontendService ODataService { get; set; } = null!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = null!;

    private MudForm? form;
    private bool success;
    private int[] errors = Array.Empty<int>();
    private bool isSubmitting;

    protected override void OnInitialized()
    {
        if (Model == null)
        {
            Model = new CategoryViewModel();
        }
    }

    private async Task Submit()
    {
        if (!success)
            return;

        try
        {
            isSubmitting = true;

            if (Model!.CategoryId == 0)
            {
                // Create
                var result = await ODataService.CreateAsync<Category, CategoryViewModel>(Model);
                Snackbar.Add("Category created successfully", Severity.Success);
            }
            else
            {
                // Update
                await ODataService.UpdateAsync<Category, CategoryViewModel>(Model.CategoryId, Model);
                Snackbar.Add("Category updated successfully", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving category: {ex.Message}", Severity.Error);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
```

### Step 5: Create Product List Component (Template for Other Entities)

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Products/ProductList.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Components.CRUD
@inherits CrudComponentBase<Product, ProductViewModel>

<MudContainer MaxWidth="MaxWidth.Large" Class="py-8">
    <MudStack Spacing="4">
        <!-- Header -->
        <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
            <MudText Typo="Typo.h4">Products</MudText>
            <MudButton Variant="Variant.Filled" 
                       Color="Color.Primary" 
                       StartIcon="@Icons.Material.Filled.Add"
                       OnClick="OpenCreateDialogAsync">
                New Product
            </MudButton>
        </MudStack>

        <!-- Search Bar -->
        <MudTextField @bind-Value="SearchString"
                      Placeholder="Search products..."
                      Adornment="Adornment.Start"
                      AdornmentIcon="@Icons.Material.Filled.Search"
                      IconSize="Size.Medium"
                      Class="mt-0" />

        <!-- Data Grid -->
        <MudDataGrid @ref="DataGrid"
                     Items="@Items"
                     Loading="@IsLoading"
                     Hover="true"
                     Striped="true"
                     Bordered="true">
            <Columns>
                <PropertyColumn Property="x => x.ProductId" Title="ID" Sortable="true" />
                <PropertyColumn Property="x => x.ProductName" Title="Name" Sortable="true" />
                <PropertyColumn Property="x => x.UnitPrice" Title="Price" Format="C" Sortable="true" />
                <PropertyColumn Property="x => x.UnitsInStock" Title="Stock" Sortable="true" />
                <TemplateColumn CellClass="d-flex gap-2">
                    <CellTemplate>
                        <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                       Size="Size.Small" 
                                       Color="Color.Primary"
                                       OnClick="@(() => OpenEditDialogAsync(context.Item))" />
                        <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                       Size="Size.Small" 
                                       Color="Color.Error"
                                       OnClick="@(() => OpenDeleteDialogAsync(context.Item.ProductId))" />
                    </CellTemplate>
                </TemplateColumn>
            </Columns>
            <PagerContent>
                <MudDataGridPager T="ProductViewModel" />
            </PagerContent>
        </MudDataGrid>
    </MudStack>
</MudContainer>
```

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Products/ProductForm.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Services
@using MudBlazor

<MudDialog>
    <TitleContent>
        <MudText Typo="Typo.h6">
            @(Model?.ProductId == 0 ? "Create New Product" : "Edit Product")
        </MudText>
    </TitleContent>
    <DialogContent>
        <MudForm @ref="form" @bind-IsValid="@success" @bind-Errors="@errors">
            <MudStack Spacing="3">
                <MudTextField @bind-Value="Model!.ProductName"
                              For="@(() => Model!.ProductName)"
                              Label="Product Name"
                              Required="true"
                              RequiredError="Product Name is required"
                              Variant="Variant.Outlined" />

                <MudNumericField @bind-Value="Model!.UnitPrice"
                                 For="@(() => Model!.UnitPrice)"
                                 Label="Unit Price"
                                 Format="C2"
                                 Required="true"
                                 Variant="Variant.Outlined" />

                <MudNumericField @bind-Value="Model!.UnitsInStock"
                                 For="@(() => Model!.UnitsInStock)"
                                 Label="Units in Stock"
                                 Required="true"
                                 Variant="Variant.Outlined" />
            </MudStack>
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Primary" 
                   Variant="Variant.Filled" 
                   OnClick="Submit"
                   Disabled="@(!success)">
            @(Model?.ProductId == 0 ? "Create" : "Update")
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public ProductViewModel? Model { get; set; }

    [Inject]
    protected IODataFrontendService ODataService { get; set; } = null!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = null!;

    private MudForm? form;
    private bool success;
    private int[] errors = Array.Empty<int>();

    protected override void OnInitialized()
    {
        if (Model == null)
        {
            Model = new ProductViewModel();
        }
    }

    private async Task Submit()
    {
        if (!success)
            return;

        try
        {
            if (Model!.ProductId == 0)
            {
                await ODataService.CreateAsync<Product, ProductViewModel>(Model);
                Snackbar.Add("Product created successfully", Severity.Success);
            }
            else
            {
                await ODataService.UpdateAsync<Product, ProductViewModel>(Model.ProductId, Model);
                Snackbar.Add("Product updated successfully", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving product: {ex.Message}", Severity.Error);
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
```

### Step 6: Create Customer List Component

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Customers/CustomerList.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Components.CRUD
@inherits CrudComponentBase<Customer, CustomerViewModel>

<MudContainer MaxWidth="MaxWidth.Large" Class="py-8">
    <MudStack Spacing="4">
        <!-- Header -->
        <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
            <MudText Typo="Typo.h4">Customers</MudText>
            <MudButton Variant="Variant.Filled" 
                       Color="Color.Primary" 
                       StartIcon="@Icons.Material.Filled.Add"
                       OnClick="OpenCreateDialogAsync">
                New Customer
            </MudButton>
        </MudStack>

        <!-- Search Bar -->
        <MudTextField @bind-Value="SearchString"
                      Placeholder="Search customers..."
                      Adornment="Adornment.Start"
                      AdornmentIcon="@Icons.Material.Filled.Search"
                      IconSize="Size.Medium"
                      Class="mt-0" />

        <!-- Data Grid -->
        <MudDataGrid @ref="DataGrid"
                     Items="@Items"
                     Loading="@IsLoading"
                     Hover="true"
                     Striped="true"
                     Bordered="true">
            <Columns>
                <PropertyColumn Property="x => x.CustomerId" Title="ID" Sortable="true" />
                <PropertyColumn Property="x => x.CompanyName" Title="Company" Sortable="true" />
                <PropertyColumn Property="x => x.ContactName" Title="Contact" Sortable="true" />
                <PropertyColumn Property="x => x.City" Title="City" Sortable="true" />
                <TemplateColumn CellClass="d-flex gap-2">
                    <CellTemplate>
                        <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                       Size="Size.Small" 
                                       Color="Color.Primary"
                                       OnClick="@(() => OpenEditDialogAsync(context.Item))" />
                        <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                       Size="Size.Small" 
                                       Color="Color.Error"
                                       OnClick="@(() => OpenDeleteDialogAsync(context.Item.CustomerId))" />
                    </CellTemplate>
                </TemplateColumn>
            </Columns>
            <PagerContent>
                <MudDataGridPager T="CustomerViewModel" />
            </PagerContent>
        </MudDataGrid>
    </MudStack>
</MudContainer>
```

**File:** `src/NorthwindAspire.Frontend/Components/CRUD/Customers/CustomerForm.razor`

```razor
@using NorthwindAspire.Backend.Models
@using NorthwindAspire.Frontend.Models.ViewModels
@using NorthwindAspire.Frontend.Services
@using MudBlazor

<MudDialog>
    <TitleContent>
        <MudText Typo="Typo.h6">
            @(string.IsNullOrEmpty(Model?.CustomerId) ? "Create New Customer" : "Edit Customer")
        </MudText>
    </TitleContent>
    <DialogContent>
        <MudForm @ref="form" @bind-IsValid="@success" @bind-Errors="@errors">
            <MudStack Spacing="3">
                <MudTextField @bind-Value="Model!.CustomerId"
                              For="@(() => Model!.CustomerId)"
                              Label="Customer ID"
                              Required="true"
                              RequiredError="Customer ID is required"
                              Disabled="@(!string.IsNullOrEmpty(Model.CustomerId))"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="Model!.CompanyName"
                              For="@(() => Model!.CompanyName)"
                              Label="Company Name"
                              Required="true"
                              RequiredError="Company Name is required"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="Model!.ContactName"
                              For="@(() => Model!.ContactName)"
                              Label="Contact Name"
                              Variant="Variant.Outlined" />

                <MudTextField @bind-Value="Model!.City"
                              For="@(() => Model!.City)"
                              Label="City"
                              Variant="Variant.Outlined" />
            </MudStack>
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="Cancel">Cancel</MudButton>
        <MudButton Color="Color.Primary" 
                   Variant="Variant.Filled" 
                   OnClick="Submit"
                   Disabled="@(!success)">
            @(string.IsNullOrEmpty(Model?.CustomerId) ? "Create" : "Update")
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public CustomerViewModel? Model { get; set; }

    [Inject]
    protected IODataFrontendService ODataService { get; set; } = null!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = null!;

    private MudForm? form;
    private bool success;
    private int[] errors = Array.Empty<int>();

    protected override void OnInitialized()
    {
        if (Model == null)
        {
            Model = new CustomerViewModel();
        }
    }

    private async Task Submit()
    {
        if (!success)
            return;

        try
        {
            if (string.IsNullOrEmpty(Model!.CustomerId))
            {
                await ODataService.CreateAsync<Customer, CustomerViewModel>(Model);
                Snackbar.Add("Customer created successfully", Severity.Success);
            }
            else
            {
                await ODataService.UpdateAsync<Customer, CustomerViewModel>(Model.CustomerId, Model);
                Snackbar.Add("Customer updated successfully", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving customer: {ex.Message}", Severity.Error);
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
```

## MudBlazor Component Reference

### Key Components Used

#### MudDataGrid
Displays tabular data with built-in sorting, filtering, and pagination:
```razor
<MudDataGrid Items="@Items" Hover="true" Striped="true">
    <Columns>
        <PropertyColumn Property="x => x.Name" Title="Name" Sortable="true" />
    </Columns>
</MudDataGrid>
```

#### MudDialog
Modal dialog for forms and confirmations:
```razor
<MudDialog>
    <TitleContent>...</TitleContent>
    <DialogContent>...</DialogContent>
    <DialogActions>...</DialogActions>
</MudDialog>
```

#### MudForm
Form validation container:
```razor
<MudForm @ref="form" @bind-IsValid="@success">
    <MudTextField ... />
</MudForm>
```

#### MudTextField
Text input field with validation:
```razor
<MudTextField @bind-Value="@Model.Name"
              Label="Name"
              Required="true"
              Variant="Variant.Outlined" />
```

#### MudNumericField
Numeric input field:
```razor
<MudNumericField @bind-Value="@Model.Price"
                 Label="Price"
                 Format="C2"
                 Variant="Variant.Outlined" />
```

#### MudButton
Interactive button element:
```razor
<MudButton Variant="Variant.Filled" Color="Color.Primary">Click Me</MudButton>
```

#### MudIconButton
Icon-only button:
```razor
<MudIconButton Icon="@Icons.Material.Filled.Edit" Color="Color.Primary" />
```

#### MudSnackbar
Toast notifications:
```csharp
Snackbar.Add("Success!", Severity.Success);
```

#### MudContainer
Responsive container with max-width:
```razor
<MudContainer MaxWidth="MaxWidth.Large">...</MudContainer>
```

#### MudStack
Flexbox-based layout (Row or Column):
```razor
<MudStack Spacing="4">...</MudStack>
<MudStack Row="true" Spacing="2">...</MudStack>
```

## Integration Points

### Dialog Service
Opening dialogs for forms:
```csharp
var dialog = await DialogService.ShowAsync<CategoryForm>(
    "Create Category",
    new DialogParameters { ["Model"] = new CategoryViewModel() });
```

### Snackbar Service
Displaying notifications:
```csharp
Snackbar.Add("Operation successful", Severity.Success);
```

### OData Service Integration
All CRUD operations use the IODataFrontendService:
```csharp
// Read
var items = await ODataService.GetAllAsync<Category, CategoryViewModel>();

// Create
var created = await ODataService.CreateAsync<Category, CategoryViewModel>(viewModel);

// Update
await ODataService.UpdateAsync<Category, CategoryViewModel>(id, viewModel);

// Delete
await ODataService.DeleteAsync<Category>(id);
```

## Best Practices

### 1. Error Handling
Always wrap service calls in try-catch:
```csharp
try
{
    await ODataService.CreateAsync<Category, CategoryViewModel>(model);
    Snackbar.Add("Success", Severity.Success);
}
catch (Exception ex)
{
    Snackbar.Add($"Error: {ex.Message}", Severity.Error);
}
```

### 2. Form Validation
Use MudForm's IsValid state to control button activation:
```razor
<MudButton Disabled="@(!success)">Save</MudButton>
```

### 3. Loading States
Show loading indicator while fetching data:
```razor
<MudDataGrid Loading="@IsLoading">...</MudDataGrid>
```

### 4. Search/Filter Implementation
Filter items client-side using LINQ:
```csharp
protected bool FilterFunc(TViewModel element)
{
    if (string.IsNullOrWhiteSpace(SearchString))
        return true;
    
    return element.ToString()?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false;
}
```

### 5. Component Naming
Follow convention: `{EntityName}List` and `{EntityName}Form`
- CategoryList.razor
- CategoryForm.razor
- ProductList.razor
- ProductForm.razor

### 6. ViewModels
Ensure ViewModels have default constructors:
```csharp
public class CategoryViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
```

### 7. Key Property Detection
Override CreateNewViewModel for correct ID initialization:
```csharp
protected override CategoryViewModel CreateNewViewModel()
{
    return new CategoryViewModel { CategoryId = 0 };
}
```

## Navigation Integration

### Add to Main Navigation
Update your main layout component to include links:

```razor
<MudNavMenu>
    <MudNavLink Href="/categories" Icon="@Icons.Material.Filled.Category">
        Categories
    </MudNavLink>
    <MudNavLink Href="/products" Icon="@Icons.Material.Filled.Shopping">
        Products
    </MudNavLink>
    <MudNavLink Href="/customers" Icon="@Icons.Material.Filled.People">
        Customers
    </MudNavLink>
</MudNavMenu>
```

### Create Route Pages
Create corresponding route pages in `Pages/`:

**File:** `src/NorthwindAspire.Frontend/Pages/Categories.razor`
```razor
@page "/categories"

<CategoryList />
```

**File:** `src/NorthwindAspire.Frontend/Pages/Products.razor`
```razor
@page "/products"

<ProductList />
```

**File:** `src/NorthwindAspire.Frontend/Pages/Customers.razor`
```razor
@page "/customers"

<CustomerList />
```

## Advanced Features

### 1. Inline Editing
Add row editing capability to MudDataGrid:
```razor
<MudDataGrid EditMode="DataGridEditMode.Form" EditTrigger="DataGridEditTrigger.OnDoubleClick">
    <!-- columns -->
</MudDataGrid>
```

### 2. Export to CSV
Add export functionality:
```csharp
private async Task ExportCsv()
{
    var csv = string.Join(",", Items.Select(x => $"{x.Id},{x.Name}"));
    // Download logic
}
```

### 3. Bulk Operations
Add checkboxes for selecting multiple records:
```razor
<MudDataGrid SelectOnRowClick="false" MultiSelection="true">
    <!-- columns -->
</MudDataGrid>
```

### 4. Master-Detail View
Show related data when row is expanded:
```razor
<MudDataGrid>
    <ChildRowContent>
        @foreach (var item in @context.Item.RelatedItems)
        {
            <MudText>@item.Name</MudText>
        }
    </ChildRowContent>
</MudDataGrid>
```

## Testing

Create unit tests for your components:

```csharp
[Test]
public async Task CategoryList_LoadsOnInitialize()
{
    // Arrange
    var mockODataService = new Mock<IODataFrontendService>();
    mockODataService
        .Setup(x => x.GetAllAsync<Category, CategoryViewModel>())
        .ReturnsAsync(new List<CategoryViewModel>());

    // Act
    var component = new CategoryList { ODataService = mockODataService.Object };
    await component.OnInitializedAsync();

    // Assert
    Assert.That(component.Items, Is.Not.Null);
}
```

## CSS Styling

Override MudBlazor styles in your `app.css`:

```css
/* Custom button styles */
.mud-button {
    text-transform: none;
    font-weight: 500;
}

/* Custom grid styles */
.mud-table-container {
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}
```

## Summary

This guide provides a complete template for creating MudBlazor CRUD components that:
- ? Leverage MudDataGrid for data display
- ? Use MudDialog for Create/Edit forms
- ? Integrate with IODataFrontendService
- ? Provide search and pagination
- ? Include form validation
- ? Handle errors gracefully
- ? Display user notifications
- ? Follow .NET 10 and C# 14 best practices

Repeat the pattern for all your entities (Employees, Suppliers, Orders, etc.) to build a complete CRUD interface!
