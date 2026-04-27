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
- JWT Bearer authentication.
- OData controllers for all Northwind tables:
  - `CustomersController`
  - `OrdersController`
  - `OrderDetailsController`
  - `ProductsController`
  - `SuppliersController`
  - etc.

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
  - `NorthwindDbContext` with `DbSet<T>` for each table.
  - Migrations (optional) for schema evolution.
- **OData:**
  - EDM model built from EF Core entities.
  - Routes like `/odata/Customers`, `/odata/Orders`, etc.

## Security model

- **API:**
  - JWT Bearer authentication.
  - All OData controllers require `[Authorize]`.
  - Token validation configured with issuer, audience, and signing key.

- **Front-end:**
  - Uses Windows Authentication for user identity.
  - Uses a configured mechanism to obtain a JWT:
    - Either by:
      - Calling a token endpoint with a service credential (secured passing and don't store credentials or passwords in plain text), or
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

