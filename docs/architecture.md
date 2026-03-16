
### `docs/architecture.md`

```markdown
# Architecture

## Overview

This solution uses **.NET Aspire** to orchestrate:

- A **Northwind OData API** (backend)
- A **MudBlazor Server app** (front-end)
- Shared **telemetry, diagnostics, and logging**

The goal is to provide a clean, observable full-stack sample with:

- Strong separation of concerns
- JWT-secured OData API
- Windows-authenticated front-end that calls the API with JWT

## Projects

### Northwind.ODataApi

- ASP.NET Core Web API with OData.
- Entity Framework Core with SQLite.
- **Repository Pattern:**
  - Generic and entity-specific repositories for data access.
  - Abstracts EF Core details from controllers.
  - Simplifies testing and future database changes.
- JWT Bearer authentication.
- OData controllers for all Northwind tables:
  - `CustomersController`
  - `OrdersController`
  - `OrderDetailsController`
  - `ProductsController`
  - `SuppliersController`
  - etc.
- Each controller uses injected repository to perform CRUD operations.

### Northwind.MudBlazorApp

- Blazor Server app using MudBlazor.
- Windows Authentication enabled.
- Uses an HTTP client configured to:
  - Attach a JWT in the `Authorization` header.
  - Call the OData API endpoints.
- UI:
  - Home page lists all table names (from metadata or a dedicated endpoint).
  - Clicking a table:
    - Loads columns and data via OData.
    - Renders a MudBlazor table/grid.
    - Provides CRUD actions.

## Data access

- **Database:** SQLite Northwind sample.
- **EF Core:**
  - `NorthwindContext` with `DbSet<T>` for each table.
  - Migrations (optional) for schema evolution.
- **Repository Pattern:**
  - `IGenericRepository<T>` interface for abstraction.
  - `GenericRepository<T>` implementation for common CRUD operations.
  - Entity-specific repositories (e.g., `ICustomerRepository`) for specialized queries.
  - All repositories are dependency-injected into controllers.
  - Provides loose coupling between controllers and data access logic.
- **OData:**
  - EDM model built from EF Core entities.
  - Routes like `/odata/Customers`, `/odata/Orders`, etc.
  - Controllers delegate to repositories for data operations.

## Security model

- **API:**
  - JWT Bearer authentication.
  - All OData controllers require `[Authorize]`.
  - Token validation configured with issuer, audience, and signing key.

- **Front-end:**
  - Uses Windows Authentication for user identity.
  - Uses a configured mechanism to obtain a JWT:
    - Either by:
      - Calling a token endpoint with a service credential, or
      - Locally issuing a JWT (for demo scenarios) based on Windows identity.
  - Attaches the JWT to API calls.

See `jwt-auth.md` for more details.

## Telemetry and logging

- **Logging:**
  - Structured logging in both API and front-end.
  - Correlation IDs propagated between front-end and API.
- **Telemetry:**
  - OpenTelemetry (or Aspire defaults) for:
    - Traces (HTTP calls, DB calls)
    - Metrics (request duration, error counts)
- **Diagnostics:**
  - Developer-friendly logging in `Development`.
  - Reduced noise and more structured logs in `Production`.

## Error handling

- API:
  - Global exception handling middleware.
  - Returns problem details for unexpected errors.
- Front-end:
  - User-friendly error messages.
  - Logs details for diagnostics.

## Documentation

- **Repository Pattern Implementation**: See `REPOSITORY_PATTERN_GUIDE.md` for detailed guide on implementing and using repositories
- **OData Testing**: See `ODATA-TESTING.md` for testing examples and query patterns
- **JWT Authentication**: See `jwt-auth.md` for security configuration details


