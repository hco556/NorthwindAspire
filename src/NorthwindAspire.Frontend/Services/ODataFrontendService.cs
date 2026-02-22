using System.Net.Http.Json;
using NorthwindAspire.Frontend.Models.Mappers;

namespace NorthwindAspire.Frontend.Services;

public class ODataFrontendService : IODataFrontendService
{
    private readonly HttpClient _httpClient;
    private readonly MapperRegistry _mapperRegistry;
    private readonly ILogger<ODataFrontendService> _logger;

    public ODataFrontendService(
        HttpClient httpClient,
        MapperRegistry mapperRegistry,
        ILogger<ODataFrontendService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _mapperRegistry = mapperRegistry ?? throw new ArgumentNullException(nameof(mapperRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a single object by its key property and returns it as a ViewModel.
    /// Uses OData $filter=eq filter on the key property.
    /// Example: /api/Categories?$filter=CategoryId eq 1
    /// </summary>
    public async Task<TViewModel?> GetByKeyAsync<TModel, TViewModel>(object keyValue)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var keyProperty = GetKeyPropertyName<TModel>();
            
            // Build OData filter: $filter=KeyProperty eq keyValue
            var filterValue = keyValue is string ? $"'{keyValue}'" : keyValue;
            var filter = $"$filter={keyProperty} eq {filterValue}";
            
            var url = $"/api/{entityName}?{filter}";
            _logger.LogInformation("Requesting: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ODataCollectionResponse<TModel>>();
            
            if (result?.Value == null || result.Value.Count == 0)
            {
                _logger.LogWarning("No {EntityName} found with {KeyProperty} = {KeyValue}", 
                    entityName, keyProperty, keyValue);
                return null;
            }
            
            var model = result.Value[0];
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            return mapper.MapToViewModel(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all objects from a table/view and returns them as a List of ViewModels.
    /// Example: /api/Categories
    /// </summary>
    public async Task<List<TViewModel>> GetAllAsync<TModel, TViewModel>()
        where TModel : class
        where TViewModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var url = $"/api/{entityName}";
            
            _logger.LogInformation("Requesting: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ODataCollectionResponse<TModel>>();
            
            if (result?.Value == null || result.Value.Count == 0)
            {
                _logger.LogInformation("No {EntityName} records found", entityName);
                return new List<TViewModel>();
            }
            
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            return result.Value
                .Select(model => mapper.MapToViewModel(model))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all {Entity} records", typeof(TModel).Name);
            throw;
        }
    }

    /// <summary>
    /// Creates a new object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API, 
    /// and returns the created ViewModel with any server-generated values.
    /// </summary>
    public async Task<TViewModel> CreateAsync<TModel, TViewModel>(TViewModel viewModel)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            
            var entityName = GetEntityName<TModel>();
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            
            // Map ViewModel to Data Model
            var model = mapper.MapToModel(viewModel);
            
            var url = $"/api/{entityName}";
            _logger.LogInformation("Creating new {Entity}", entityName);
            
            var response = await _httpClient.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            
            var createdModel = await response.Content.ReadFromJsonAsync<TModel>();
            
            // Map the created Data Model back to ViewModel
            return mapper.MapToViewModel(createdModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {Entity}", typeof(TModel).Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing object in the backend.
    /// Accepts a ViewModel, maps it to the Data Model, sends it to the API.
    /// </summary>
    public async Task UpdateAsync<TModel, TViewModel>(object keyValue, TViewModel viewModel)
        where TModel : class
        where TViewModel : class
    {
        try
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            
            var entityName = GetEntityName<TModel>();
            var mapper = _mapperRegistry.GetMapper<TModel, TViewModel>();
            
            // Map ViewModel to Data Model
            var model = mapper.MapToModel(viewModel);
            
            var url = $"/api/{entityName}/{keyValue}";
            _logger.LogInformation("Updating {Entity} with key {KeyValue}", entityName, keyValue);
            
            var response = await _httpClient.PutAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Deletes an object from the backend by its key property.
    /// </summary>
    public async Task DeleteAsync<TModel>(object keyValue)
        where TModel : class
    {
        try
        {
            var entityName = GetEntityName<TModel>();
            var url = $"/api/{entityName}/{keyValue}";
            
            _logger.LogInformation("Deleting {Entity} with key {KeyValue}", entityName, keyValue);
            
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {Entity} with key {KeyValue}", 
                typeof(TModel).Name, keyValue);
            throw;
        }
    }

    /// <summary>
    /// Gets the entity name from the model class name.
    /// Strips 'Model' suffix if present.
    /// Examples: Category -> Categories, Product -> Products, OrdersQry -> OrdersQries
    /// </summary>
    private string GetEntityName<TModel>() where TModel : class
    {
        var className = typeof(TModel).Name;
        
        // Remove 'Model' suffix if present
        if (className.EndsWith("Model"))
            className = className[..^5]; // Remove last 5 characters
        
        // Pluralize: simple approach, add 's' or 'es' based on ending
        return className.EndsWith("y") 
            ? $"{className[..^1]}ies" 
            : $"{className}s";
    }

    /// <summary>
    /// Gets the key property name for a model.
    /// Conventional: TypeName + "Id" (e.g., CategoryId, ProductId)
    /// </summary>
    private string GetKeyPropertyName<TModel>() where TModel : class
    {
        var className = typeof(TModel).Name;
        return $"{className}Id";
    }
}

/// <summary>
/// Represents the OData response collection format.
/// The OData API returns data in a collection with a 'value' property.
/// </summary>
public class ODataCollectionResponse<T>
{
    public List<T>? Value { get; set; }
}
