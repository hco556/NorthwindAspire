using System.Net;
using System.Text.Json;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Controllers;

[TestFixture]
public class CustomersControllerTests : ODataApiTestBase
{
    private const string EntitySet = "customers";

    [Test]
    public async Task GetAllCustomers_ReturnsOkStatus()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "CUST1",
            CompanyName = "Test Company",
            ContactName = "John Doe",
            City = "New York"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCustomerById_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "ALFKI",
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            City = "Berlin"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}('ALFKI')");
        var json = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Customer>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.CompanyName, Is.EqualTo("Alfreds Futterkiste"));
    }

    [Test]
    public async Task CreateCustomer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newCustomer = new 
        { 
            customerId = "NEW01",
            companyName = "New Company",
            contactName = "Jane Doe",
            city = "London"
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newCustomer);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateCustomer_WithValidId_UpdatesData()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "TEST1",
            CompanyName = "Old Company",
            ContactName = "John",
            City = "Old City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        var updateData = new { companyName = "Updated Company", city = "New City" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}('TEST1')", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var updated = await DbContext.Customers.FindAsync("TEST1");
        Assert.That(updated?.CompanyName, Is.EqualTo("Updated Company"));
    }

    [Test]
    public async Task DeleteCustomer_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "DEL01",
            CompanyName = "Company",
            ContactName = "Test",
            City = "City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}('DEL01')");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var deleted = await DbContext.Customers.FindAsync("DEL01");
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task GetCustomers_WithContainsFilter_ReturnsMatches()
    {
        // Arrange
        DbContext.Customers.AddRange(
            new Customer { CustomerId = "C1", CompanyName = "Alfreds Ltd", ContactName = "Maria", City = "Berlin" },
            new Customer { CustomerId = "C2", CompanyName = "Bob's Shop", ContactName = "Bob", City = "London" }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "contains(companyName,'Ltd')");

        // Act
        var response = await GetODataCollectionAsync<Customer>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
        Assert.That(response.Value[0].CompanyName, Is.EqualTo("Alfreds Ltd"));
    }

    [Test]
    public async Task GetCustomers_WithExpandOrders_IncludesRelatedData()
    {
        // Arrange
        var customer = new Customer 
        { 
            CustomerId = "CUST1",
            CompanyName = "Test",
            ContactName = "Test",
            City = "City"
        };
        DbContext.Customers.Add(customer);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "orders");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCustomers_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            DbContext.Customers.Add(new Customer 
            { 
                CustomerId = $"CUST{i:D2}",
                CompanyName = $"Company {i}",
                ContactName = $"Contact {i}",
                City = $"City {i}"
            });
        }
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, top: 5, skip: 5);

        // Act
        var response = await GetODataCollectionAsync<Customer>(url);

        // Assert
        Assert.That(response.Value.Count, Is.LessThanOrEqualTo(5));
    }
}
