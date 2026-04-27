# Aspire orchestration and telemetry 
integrated into both the backend and frontend projects, providing comprehensive monitoring and diagnostics across the entire solution. The backend API is secured with JWT authentication, while the frontend utilizes Windows Authentication to seamlessly interact with the API. Each Northwind table is exposed through OData controllers, allowing for dynamic CRUD operations from the MudBlazor Server front-end.
## 1. Update Backend (Program.cs) - Add Health Check & Service Defaults
dotnet add src/NorthwindAspire.Backend package Aspire.Hosting.AppHost
## 2. Update Frontend (Program.cs) - Add Service Defaults
dotnet add src/NorthwindAspire.Frontend package Aspire.Hosting.AppHost

## 3. Update AppHost.cs in project NorthwindAspire.AppHost project
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.NorthwindAspire_Backend>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.NorthwindAspire_Frontend>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
