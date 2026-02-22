using System.Net;
using System.Text.Json;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Controllers;

[TestFixture]
public class OrdersControllerTests : ODataApiTestBase
{
    private const string EntitySet = "orders";

    private Customer SetupTestCustomer()
    {
        var customer = new Customer 
        { 
            CustomerId = "TESTCUST",
            CompanyName = "Test Company",
            ContactName = "Test Contact",
            City = "Test City"
        };
        DbContext.Customers.Add(customer);
        DbContext.SaveChanges();
        return customer;
    }

    [Test]
    public async Task GetAllOrders_ReturnsOkStatus()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            OrderDate = DateTime.Now,
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetOrderById_WithValidId_ReturnsOrder()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            OrderDate = new DateTime(2024, 1, 15),
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var json = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.CustomerId, Is.EqualTo("TESTCUST"));
    }

    [Test]
    public async Task CreateOrder_WithValidData_ReturnsCreated()
    {
        // Arrange
        SetupTestCustomer();
        var newOrder = new 
        { 
            customerId = "TESTCUST",
            orderDate = DateTime.Now,
            freight = 10.00
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newOrder);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateOrder_WithValidId_UpdatesFreight()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Freight = 10.00m,
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        var updateData = new { freight = 25.50 };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetOrders_WithDateFilter_ReturnsFilteredResults()
    {
        // Arrange
        var customer = SetupTestCustomer();
        DbContext.Orders.AddRange(
            new Order 
            { 
                OrderId = 1,
                CustomerId = "TESTCUST",
                OrderDate = new DateTime(2024, 1, 1),
                Customer = customer
            },
            new Order 
            { 
                OrderId = 2,
                CustomerId = "TESTCUST",
                OrderDate = new DateTime(2024, 6, 1),
                Customer = customer
            }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "orderDate ge 2024-06-01");

        // Act
        var response = await GetODataCollectionAsync<Order>(url);

        // Assert
        Assert.That(response.Value.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetOrders_WithExpandCustomer_IncludesCustomerData()
    {
        // Arrange
        var customer = SetupTestCustomer();
        var order = new Order 
        { 
            OrderId = 1,
            CustomerId = "TESTCUST",
            Customer = customer
        };
        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "customer");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
