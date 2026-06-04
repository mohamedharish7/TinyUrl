# TinyURL API

ASP.NET Core Minimal API for shortening URLs with MSSQL storage.

## Requirements
- .NET 8 SDK
- SQL Server Express

## Setup

1. Update connection string in `TinyUrlApi/appsettings.json`
2. Run the API:

```bash
cd TinyUrlApi
dotnet run
```

3. Open Swagger: http://localhost:5000/swagger

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/shorten` | Create a short URL |
| GET | `/api/urls` | List all public URLs |
| GET | `/api/urls/{shortCode}` | Get URL by short code |
| DELETE | `/api/urls/{shortCode}` | Delete a short URL |
| GET | `/{shortCode}` | Redirect to original URL |

## POST /api/shorten Body

```json
{
  "originalUrl": "https://example.com",
  "isPrivate": false
}
```
