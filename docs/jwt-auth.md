# JWT Authentication Design

## Goals

- Secure the **OData API** with JWT Bearer tokens.
- Allow the **MudBlazor Server front-end** (Windows-authenticated) to call the API using JWT.
- Keep configuration simple and environment-driven.

## API configuration

In `Northwind.ODataApi`:

- Configure JWT Bearer authentication in `Program.cs`:
  - `Issuer`
  - `Audience`
  - `SigningKey` (symmetric key for demo; use a secure secret in production).
- Apply `[Authorize]` to:
  - OData controllers
  - Or globally via authorization policy.

Example configuration keys (conceptual):

```json
"Jwt": {
  "Issuer": "NorthwindIssuer",
  "Audience": "NorthwindApi",
  "SigningKey": "your-very-strong-signing-key"
}
