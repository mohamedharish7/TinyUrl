# TinyURL Project

A full-stack URL shortener built with **Angular 17** (frontend) and **ASP.NET Core 8** (backend).

---

## Live URLs

| Resource | URL |
|----------|-----|
| Frontend App | https://victorious-hill-033b1130f.7.azurestaticapps.net |
| Backend API | https://tinyurl-api-dvh7emgkfwbnccb7.australiaeast-01.azurewebsites.net |
| Swagger UI | https://tinyurl-api-dvh7emgkfwbnccb7.australiaeast-01.azurewebsites.net/swagger |


---

## Local URLs

| Resource | URL |
|----------|-----|
| Frontend App | http://localhost:4200 |
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |


---

## Project Structure

```
tinyurl-project/
├── frontend/   # Angular 17 SPA
└── backend/
    └── TinyUrlApi/   # ASP.NET Core 8 Minimal API
```

---

## Backend — ASP.NET Core 8

### Tech Stack
- .NET 8 Minimal API
- Entity Framework Core 8 with SQL Server
- Swagger / OpenApi (Swashbuckle)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express or full)

### Setup & Run

1. Connection string is already configured to point to Azure SQL Database in `backend/TinyUrlApi/TinyUrlApi/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=tcp:tinyurl-server.database.windows.net,1433;Initial Catalog=TinyUrl;Persist Security Info=False;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
   }
   ```

2. Run the API:
   ```bash
   cd backend/TinyUrlApi/TinyUrlApi
   dotnet run
   ```

   The API starts on `http://localhost:5000` by default.  
   Swagger UI is available at `http://localhost:5000/swagger`.

> The database and tables are created automatically on first run via `EnsureCreated()`.

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/shorten` | Create a shortened URL |
| `GET` | `/api/urls` | List all public URLs |
| `GET` | `/api/urls/{shortCode}` | Get a URL by short code |
| `DELETE` | `/api/urls/{shortCode}` | Delete a URL |
| `GET` | `/{shortCode}` | Redirect to the original URL |

#### POST `/api/shorten` — Request Body
```json
{
  "originalUrl": "https://example.com",
  "isPrivate": false
}
```

---

## Frontend — Angular 17

### Tech Stack
- Angular 17 (standalone components)
- Angular HttpClient
- Angular Router

### Prerequisites
- [Node.js](https://nodejs.org/) v18+
- Angular CLI v17

### Setup & Run

```bash
cd frontend
npm install
npm start
```

The app runs at `http://localhost:4200`.

### Environment Config

`src/environments/environment.ts`
```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

Change `apiUrl` to point to your backend if running on a different host/port.

### Features
- Shorten any valid URL
- Mark URLs as private (hidden from public list)
- View all public shortened URLs with hit counts
- Delete a shortened URL
- Click a short URL to be redirected to the original

---

## Running Both Together

1. Start the backend: `dotnet run` (port 5000)
2. Start the frontend: `npm start` (port 4200)
3. Open `http://localhost:4200` in your browser
