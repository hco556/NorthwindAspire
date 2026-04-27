# NorthwindAspire.Backend

This project exposes the **Northwind SQLite database** as an **OData API** secured with **JWT Bearer authentication**.

## Features

- ASP.NET Core Web API with OData.
- Entity Framework Core with SQLite.
- OData controllers for all Northwind tables:
  - `Customers`, `Orders`, `OrderDetails`, `Products`, `Suppliers`, etc.
- Full CRUD:
  - `GET`, `POST`, `PATCH`, `PUT`, `DELETE`.
- OData query options:
  - `$filter`, `$select`, `$orderby`, `$top`, `$skip`, etc.
- JWT-secured:
  - All controllers require authenticated users.

## Configuration

### Connection string

Set in `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "Northwind": "Data Source=path-to-your-northwind.db"
}
