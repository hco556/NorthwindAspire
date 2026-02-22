using Microsoft.AspNetCore.Components;
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
        // This method will be overridden in derived classes to specify the dialog component
        await LoadDataAsync();
        Snackbar.Add("Record created successfully", Severity.Success);
    }

    protected virtual async Task OpenEditDialogAsync(TViewModel item)
    {
        // This method will be overridden in derived classes to specify the dialog component
        await LoadDataAsync();
        Snackbar.Add("Record updated successfully", Severity.Success);
    }

    protected virtual async Task OpenDeleteDialogAsync(object keyValue)
    {
        // Delete the record directly (override in derived classes to show confirmation)
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
