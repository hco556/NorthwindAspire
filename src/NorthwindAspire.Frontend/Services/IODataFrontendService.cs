namespace NorthwindAspire.Frontend.Services;

public interface IODataFrontendService
{
    /// <summary>
    /// Retrieves a single object by its key property and returns it as a ViewModel.
    /// Uses OData $filter=eq filter on the key property.
    /// </summary>
    Task<TViewModel?> GetByKeyAsync<TModel, TViewModel>(object keyValue)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Retrieves all objects from a table/view and returns them as a List of ViewModels.
    /// </summary>
    Task<List<TViewModel>> GetAllAsync<TModel, TViewModel>()
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Creates a new object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API, and returns the created ViewModel.
    /// </summary>
    Task<TViewModel> CreateAsync<TModel, TViewModel>(TViewModel viewModel)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Updates an existing object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API.
    /// </summary>
    Task UpdateAsync<TModel, TViewModel>(object keyValue, TViewModel viewModel)
        where TModel : class
        where TViewModel : class;

    /// <summary>
    /// Deletes an object from the backend by its key property.
    /// </summary>
    Task DeleteAsync<TModel>(object keyValue)
        where TModel : class;
}
