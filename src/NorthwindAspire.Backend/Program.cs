using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add Entity Framework with SQLite
var sqliteConnection = "Data Source=northwind.db";
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlite(sqliteConnection));

// Add OpenAPI (Swagger)
builder.Services.AddOpenApi();

// Build OData Model
var odataModelBuilder = new ODataConventionModelBuilder();

// Configure EntitySets with keys
odataModelBuilder.EntitySet<Category>("Categories").EntityType.HasKey(c => c.CategoryId);
odataModelBuilder.EntitySet<Customer>("Customers").EntityType.HasKey(c => c.CustomerId);
odataModelBuilder.EntitySet<Employee>("Employees").EntityType.HasKey(e => e.EmployeeId);
odataModelBuilder.EntitySet<Order>("Orders").EntityType.HasKey(o => o.OrderId);
odataModelBuilder.EntitySet<Product>("Products").EntityType.HasKey(p => p.ProductId);
odataModelBuilder.EntitySet<Region>("Regions").EntityType.HasKey(r => r.RegionId);
odataModelBuilder.EntitySet<Shipper>("Shippers").EntityType.HasKey(s => s.ShipperId);
odataModelBuilder.EntitySet<Supplier>("Suppliers").EntityType.HasKey(s => s.SupplierId);
odataModelBuilder.EntitySet<Territory>("Territories").EntityType.HasKey(t => t.TerritoryId);

// Configure composite keys
var orderDetailSet = odataModelBuilder.EntitySet<OrderDetail>("OrderDetails");
orderDetailSet.EntityType.HasKey(od => new { od.OrderId, od.ProductId });

var employeeTerritorySet = odataModelBuilder.EntitySet<EmployeeTerritory>("EmployeeTerritories");
employeeTerritorySet.EntityType.HasKey(et => new { et.EmployeeId, et.TerritoryId });

var odataModel = odataModelBuilder.GetEdmModel();

// Add OData services
builder.Services.AddControllers().AddOData(options =>
    options
        .Select()
        .Expand()
        .Filter()
        .OrderBy()
        .SetMaxTop(1000)
        .Count()
        .SkipToken()
        .AddRouteComponents("odata", odataModel));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Display available endpoints
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("OData endpoints available at: http://localhost:port/odata");
    app.Logger.LogInformation("OData metadata available at: http://localhost:port/odata/$metadata");
    app.Logger.LogInformation("API documentation available at: http://localhost:port/openapi/v1.json");
}

app.Run();

public partial class Program { }
