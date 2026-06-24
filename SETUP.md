# Local Setup

## Requirements

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Node.js | 20+ (LTS) | https://nodejs.org |
| npm | 10.9.3+ | bundled with Node.js |
| Docker | any recent | https://www.docker.com/products/docker-desktop |

---

## 1. SQL Server via Docker

Run a local SQL Server 2022 instance:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name chatapp-sql --hostname chatapp-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

To stop / start it later:

```bash
docker stop chatapp-sql
docker start chatapp-sql
```

---

## 2. Backend

### 2a. Restore local tools

```bash
cd backend/ChatApp.Api
dotnet tool restore
```

### 2b. Set user secrets

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=ChatApp;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:Key" "your-super-secret-key-minimum-32-characters-long"
```

> `Jwt:Issuer` and `Jwt:Audience` are already set to `ChatApp` in `appsettings.json`.

### 2c. Apply database migrations

```bash
dotnet ef database update
```

### 2d. Run the API

```bash
dotnet run
```

API is available at:
- http://localhost:5270
- https://localhost:7189
- Swagger UI: http://localhost:5270/swagger

---

## 3. Frontend

```bash
cd frontend/chat-app-web
npm install
npm start
```

App is available at http://localhost:4200.
