# NorthwindAspire.Frontend

This is a **Blazor Server** app using **MudBlazor** as the UI library.  
It uses **Windows Authentication** and calls the **Northwind OData API** using **JWT**.

## Features

- Blazor Server with MudBlazor components.
- Windows Authentication for user identity.
- HTTP client configured to:
  - Obtain a JWT (see `docs/jwt-auth.md`).
  - Call the OData API with `Authorization: Bearer <token>`.
- Home page:
  - Displays a list of Northwind table names.
  - Clicking a table:
    - Loads the table’s columns and records via OData.
    - Renders a MudBlazor grid:
      - First column: **Update** and **Delete** icon buttons.
      - Above the grid: **Create** button.

## UI behavior

### Home page

- On load:
  - Calls the API (e.g., a metadata endpoint or a predefined list) to get table names.
- Renders a list (or MudList/MudTable) of table names.
- On table click:
  - Navigates to `/tables/{tableName}`.

### Table page

- Uses the `{tableName}` route parameter.
- Calls the OData API:
  - `/odata/{tableName}` to get records.
  - Optionally uses `$select` to control columns.
- Renders a MudBlazor table:
  - First column:
    - **Edit** icon button → opens a dialog/form for updating the record.
    - **Delete** icon button → confirms and deletes the record.
  - Header:
    - **Create** button → opens a dialog/form for creating a new record.

## Authentication and API calls

- The app is configured with **Windows Authentication**.
- A server-side service:
  - Issues or retrieves a JWT for the current user.
  - Attaches it to outgoing HTTP requests to the OData API.

## Telemetry and logging

- Uses ASP.NET Core logging and Aspire/OpenTelemetry integration.
- Logs:
  - User actions (table selection, CRUD operations).
  - API call results and failures.
- Traces:
  - Front-end → API calls.
  - API → DB calls.

## Running the app

```bash
cd src/Northwind.MudBlazorApp
dotnet run
