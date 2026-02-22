using MudBlazor.Services;
using NorthwindAspire.Frontend.Components;
using NorthwindAspire.Frontend.Models.Mappers;
using NorthwindAspire.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();

// Register mappers
builder.Services.AddSingleton<CategoryMapper>();
builder.Services.AddSingleton<CustomerMapper>();
builder.Services.AddSingleton<ProductMapper>();
builder.Services.AddSingleton<OrderDetailMapper>();
builder.Services.AddSingleton<OrderMapper>();
builder.Services.AddSingleton<EmployeeMapper>();
builder.Services.AddSingleton<SupplierMapper>();
builder.Services.AddSingleton<ShipperMapper>();
builder.Services.AddSingleton<RegionMapper>();
builder.Services.AddSingleton<TerritoryMapper>();
builder.Services.AddSingleton<EmployeeTerritoryMapper>();
builder.Services.AddSingleton<MapperRegistry>();

// Register OData Frontend Service
builder.Services.AddHttpClient<ODataFrontendService>((sp, client) =>
{
    var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7027/";
    client.BaseAddress = new Uri(backendUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IODataFrontendService>(sp => sp.GetRequiredService<ODataFrontendService>());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

//app.MapHealthChecks("/health");


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
