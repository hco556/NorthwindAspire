# Northwind Aspire Sample – OData API + MudBlazor Server

This solution is a .NET Aspire–based sample that demonstrates:

- A **Northwind OData API back-end** (JWT-secured) using:
  - ASP.NET Core
  - Entity Framework Core
  - SQLite (Microsoft Northwind sample database)
  - OData controllers with full CRUD for all Northwind tables
- A **MudBlazor Server front-end** using:
  - Windows Authentication
  - JWT-based calls to the OData API
  - Dynamic table list and CRUD grids per table
- **Telemetry, diagnostics, and logging** wired through Aspire.

## Solution structure

```text
.
├─ README.md
├─ docs
│  ├─ architecture.md
│  └─ jwt-auth.md
└─ src
   ├─ NorthwindAspire.Backend
   │  └─ README.md
   └─ NorthwindAspire.Frontend
      └─ README.md

# Generate projects
# Create the Backend API project (ASP.NET Core Web API)
dotnet new webapi -n NorthwindAspire.Backend -o src/NorthwindAspire.Backend --framework net10.0

# Navigate to the backend project directory
cd src/NorthwindAspire.Backend

# Add required NuGet packages
dotnet add package Microsoft.AspNetCore.OData
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Aspire.Microsoft.EntityFrameworkCore.Sqlite

# Go back to solution root
cd ../..

# Add the backend project to the solution
dotnet sln add src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj

# Create the Frontend Blazor Server project
dotnet new blazor -n NorthwindAspire.Frontend -o src/NorthwindAspire.Frontend --framework net10.0 --interactive Server

# Navigate to the frontend project directory
cd src/NorthwindAspire.Frontend

# Add required NuGet packages
dotnet add package MudBlazor
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.Negotiate

# Go back to solution root
cd ../..

# Add the frontend project to the solution
dotnet sln add src/NorthwindAspire.Frontend/NorthwindAspire.Frontend.csproj


# Create the Frontend Blazor Server project
dotnet new mudblazor -n NorthwindAspire.Frontend --interactivity Server -o src/NorthwindAspire.Frontend --all-interactive  --framework net10.0 

# Navigate to the frontend project directory
cd src/NorthwindAspire.Frontend

# Add required NuGet packages
dotnet add package MudBlazor
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.Negotiate

# Go back to solution root
cd ../..

# Add the frontend project to the solution
dotnet sln add src/NorthwindAspire.Frontend/NorthwindAspire.Frontend.csproj

# Build the solution to verify everything is set up correctly
dotnet build

# Run the Aspire app host to test
dotnet run --project NorthwindAspire.AppHost