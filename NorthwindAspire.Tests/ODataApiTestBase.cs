using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests;

/// <summary>
/// Custom WebApplicationFactory for OData API testing
/// </summary>
public class ODataWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        builder.ConfigureServices(services =>
        {
            // Remove all DbContext options and related services
            var descriptorsToRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<NorthwindContext>) ||
                           d.ServiceType == typeof(DbContextOptions) ||
                           d.ServiceType?.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Use isolated in-memory database for this factory instance
            services.AddDbContext<NorthwindContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}

[TestFixture]
public abstract class ODataApiTestBase
{
    protected HttpClient HttpClient { get; private set; } = null!;
    protected ODataWebApplicationFactory Factory { get; private set; } = null!;
    protected NorthwindContext DbContext { get; private set; } = null!;
    private IServiceScope? _scope;

    private const string BaseODataUrl = "/odata";
    private const string DefaultMediaType = "application/json";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Create factory with custom configuration
        Factory = new ODataWebApplicationFactory();

        HttpClient = Factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(DefaultMediaType));
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        HttpClient?.Dispose();
        Factory?.Dispose();
    }

    [SetUp]
    public virtual void SetUp()
    {
        // Create a fresh scope for each test that lives for the duration of the test
        _scope = Factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<NorthwindContext>();
        
        // Ensure database schema is created
        DbContext.Database.EnsureCreated();
        
        // Clear database before each test
        CleanDatabase();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DbContext?.Dispose();
        _scope?.Dispose();
    }

    protected void CleanDatabase()
    {
        DbContext.Categories.RemoveRange(DbContext.Categories);
        DbContext.Customers.RemoveRange(DbContext.Customers);
        DbContext.Products.RemoveRange(DbContext.Products);
        DbContext.Orders.RemoveRange(DbContext.Orders);
        DbContext.OrderDetails.RemoveRange(DbContext.OrderDetails);
        DbContext.Employees.RemoveRange(DbContext.Employees);
        DbContext.Suppliers.RemoveRange(DbContext.Suppliers);
        DbContext.Shippers.RemoveRange(DbContext.Shippers);
        DbContext.Regions.RemoveRange(DbContext.Regions);
        DbContext.Territories.RemoveRange(DbContext.Territories);
        DbContext.EmployeeTerritories.RemoveRange(DbContext.EmployeeTerritories);
        DbContext.SaveChanges();
    }

    protected string BuildODataUrl(string entitySet, string? filter = null, string? select = null, 
        string? expand = null, string? orderby = null, int? top = null, int? skip = null)
    {
        var url = $"{BaseODataUrl}/{entitySet}";
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filter))
            queryParams.Add($"$filter={Uri.EscapeDataString(filter)}");
        if (!string.IsNullOrEmpty(select))
            queryParams.Add($"$select={select}");
        if (!string.IsNullOrEmpty(expand))
            queryParams.Add($"$expand={expand}");
        if (!string.IsNullOrEmpty(orderby))
            queryParams.Add($"$orderby={orderby}");
        if (top.HasValue)
            queryParams.Add($"$top={top}");
        if (skip.HasValue)
            queryParams.Add($"$skip={skip}");

        if (queryParams.Any())
        {
            url += "?" + string.Join("&", queryParams);
        }

        return url;
    }

    protected async Task<T?> GetAsync<T>(string url) where T : class
    {
        var response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return null;
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T content) where T : class
    {
        var json = JsonSerializer.Serialize(content);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, DefaultMediaType);
        return await HttpClient.PostAsync(url, httpContent);
    }

    protected async Task<HttpResponseMessage> PatchAsync<T>(string url, T content) where T : class
    {
        var json = JsonSerializer.Serialize(content);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, DefaultMediaType);
        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = httpContent
        };
        return await HttpClient.SendAsync(request);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await HttpClient.DeleteAsync(url);
    }

    protected async Task<ODataResponse<T>> GetODataCollectionAsync<T>(string url) where T : class
    {
        var response = await HttpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ODataResponse<T>>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? 
                new ODataResponse<T> { Value = new List<T>() };
        }
        return new ODataResponse<T> { Value = new List<T>() };
    }

    protected async Task RefreshEntityAsync<T>(T entity) where T : class
    {
        await DbContext.Entry(entity).ReloadAsync();
    }

    protected T? RefreshEntity<T>(T? entity) where T : class
    {
        if (entity != null)
        {
            DbContext.Entry(entity).Reload();
        }
        return entity;
    }
}

public class ODataResponse<T> where T : class
{
    [System.Text.Json.Serialization.JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();

    [System.Text.Json.Serialization.JsonPropertyName("@odata.count")]
    public long? Count { get; set; }
}
