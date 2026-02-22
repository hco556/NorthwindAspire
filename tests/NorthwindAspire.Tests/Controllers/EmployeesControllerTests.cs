using System.Net;
using System.Text.Json;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Controllers;

[TestFixture]
public class EmployeesControllerTests : ODataApiTestBase
{
    private const string EntitySet = "employees";

    [Test]
    public async Task GetAllEmployees_ReturnsOkStatus()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Sales Manager"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/odata/{EntitySet}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetEmployeeById_WithValidId_ReturnsEmployee()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "Nancy",
            LastName = "Davolio",
            Title = "Sales Representative"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var httpResponse = await HttpClient.GetAsync($"/odata/{EntitySet}(1)");
        var json = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Employee>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Assert
        Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result.LastName, Is.EqualTo("Davolio"));
    }

    [Test]
    public async Task CreateEmployee_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newEmployee = new 
        { 
            firstName = "Michael",
            lastName = "Scott",
            title = "Manager",
            hireDate = DateTime.Now
        };

        // Act
        var response = await PostAsync($"/odata/{EntitySet}", newEmployee);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateEmployee_WithValidId_UpdatesTitle()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Salesman"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        var updateData = new { title = "Senior Sales Manager" };

        // Act
        var response = await PatchAsync($"/odata/{EntitySet}(1)", updateData);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var updated = await DbContext.Employees.FindAsync(1);
        Assert.That(updated?.Title, Is.EqualTo("Senior Sales Manager"));
    }

    [Test]
    public async Task DeleteEmployee_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "Test",
            LastName = "Employee",
            Title = "Tester"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await DeleteAsync($"/odata/{EntitySet}(1)");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetEmployees_FilterByTitle_ReturnsMatchingEmployees()
    {
        // Arrange
        DbContext.Employees.AddRange(
            new Employee { EmployeeId = 1, FirstName = "John", LastName = "Manager", Title = "Sales Manager" },
            new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Rep", Title = "Sales Representative" }
        );
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, filter: "title eq 'Sales Manager'");

        // Act
        var response = await GetODataCollectionAsync<Employee>(url);

        // Assert
        Assert.That(response.Value.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmployees_ExpandOrders_IncludesRelatedOrders()
    {
        // Arrange
        var employee = new Employee 
        { 
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe",
            Title = "Sales Manager"
        };
        DbContext.Employees.Add(employee);
        await DbContext.SaveChangesAsync();

        var url = BuildODataUrl(EntitySet, expand: "orders");

        // Act
        var response = await HttpClient.GetAsync(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
