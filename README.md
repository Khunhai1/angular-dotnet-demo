# 🚀 Kickoff — Chat App (Angular + .NET + Azure)

> **Goal**: Learning project deployed on Azure, valuable for CV / LinkedIn.
> **Option B selected**: 100% free using Azure free tiers.

---

## 🎯 Final stack

| Layer | Tech | Hosting |
|---|---|---|
| Frontend | Angular 18+ (standalone, signals) | Azure Static Web Apps (free) |
| REST API | ASP.NET Core 8 Web API | Azure App Service **F1** (free) |
| Real-time | SignalR (server-side) | Azure SignalR Service **Free tier** (20 connections, 20k msg/day) |
| Database | EF Core + SQL Server | Azure SQL **Free tier** (32 GB, S0) |
| Auth | ASP.NET Core Identity + JWT | Built into the API |
| CI/CD | GitHub Actions → Azure | Free |

---

## 🛠️ Prerequisites to install

- [x] **.NET SDK 8** ([download](https://dotnet.microsoft.com/download))
- [x] **Node.js 20+** + npm
- [x] **Angular CLI**: `npm install -g @angular/cli`
- [x] **Visual Studio 2022 Community** *or* **VS Code + C# Dev Kit**
- [x] **Git** + GitHub account
- [x] **Azure account** ([signup](https://azure.microsoft.com/free)) — $200 trial credit + free services
- [x] **Azure CLI** (`az`) — eases deployment
- [x] **Postman** or **Bruno** to test the API

---

## 📁 Repo structure

```
chat-app/
├── backend/
│   └── ChatApp.Api/           # ASP.NET Core Web API
├── frontend/
│   └── chat-app-web/          # Angular
├── .github/workflows/         # CI/CD
└── README.md                  # Project docs (very important for CV)
```

---

## ✅ Phase 0 — Initial setup

- [x] Create GitHub repo `chat-app` (public if you want to showcase it)
- [x] Initialize .NET solution: `dotnet new sln -n ChatApp`
- [x] Create API project: `dotnet new webapi -n ChatApp.Api -o backend/ChatApp.Api`
- [x] Create Angular project: `ng new chat-app-web --standalone --routing --style=scss`
- [x] Add a clean `.gitignore` (bin/, obj/, node_modules/, .vs/, etc.)
- [x] First commit + push

---

## ✅ Phase 1 — Backend foundations

- [x] Install NuGet packages:
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - `Microsoft.AspNetCore.SignalR`
  - `Microsoft.Azure.SignalR` (for the managed service)
  - `Swashbuckle.AspNetCore`
- [x] Models: `User`, `Conversation`, `ConversationParticipant`, `Message`
- [x] `DbContext` + first migration
- [x] Layered architecture: `Controllers/` `Hubs/` `Services/` `Repositories/` `Models/` `Dtos/`
- [x] Configure CORS (allow `http://localhost:4200`)
- [x] Enable Swagger in development

---

## ✅ Phase 2 — Authentication

- [x] Configure ASP.NET Core Identity
- [x] Endpoints: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`
- [x] JWT generation (access token + refresh token)
- [x] JWT Bearer middleware
- [x] Test with Postman before touching the frontend

---

## ✅ Phase 3 — REST API (conversations & history)

- [ ] `GET /api/conversations` — list current user's conversations
- [ ] `POST /api/conversations` — create a conversation (1-to-1)
- [ ] `GET /api/conversations/{id}/messages` — paginated history
- [ ] `GET /api/users/search?q=` — search a user
- [ ] `GET /api/users/me` + `PUT /api/users/me` — profile
- [ ] All endpoints protected with `[Authorize]`

---

## ✅ Phase 4 — SignalR (real-time)

- [ ] Create `ChatHub : Hub` with `[Authorize]`
- [ ] Server methods: `SendMessage`, `MarkAsRead`, `Typing`
- [ ] Client events: `ReceiveMessage`, `UserTyping`, `UserOnline`, `UserOffline`
- [ ] Group management: one SignalR group = one conversation
- [ ] Presence tracking (in-memory first, Redis later if needed)
- [ ] Wire up Azure SignalR Service via connection string (can test locally without it)

---

## ✅ Phase 5 — Angular frontend

- [ ] Structure: `core/` `shared/` `features/auth` `features/chat` `features/profile`
- [ ] `AuthService` + JWT interceptor + auth guard
- [ ] `ApiService` (HttpClient wrapper)
- [ ] `SignalRService` (`@microsoft/signalr`)
- [ ] Routes: `/login`, `/register`, `/chat`, `/chat/:conversationId`, `/profile`
- [ ] Layout: sidebar (conversations list) + messages pane
- [ ] Components: `MessageList`, `MessageInput`, `ConversationList`, `UserSearch`
- [ ] UI choice: Angular Material OR Tailwind (Tailwind = more modern look)

---

## ✅ Phase 6 — Polish

- [ ] "Typing…" indicator
- [ ] Read receipts
- [ ] Unread message badges
- [ ] Avatars (Gravatar to start, upload later)
- [ ] Infinite scroll for history
- [ ] SignalR reconnection handling
- [ ] Proper loading states + error handling
- [ ] Mobile responsive

---

## ☁️ Phase 7 — Azure deployment

### Provision Azure resources
- [ ] Create a **Resource Group** (e.g. `rg-chat-app`)
- [ ] Create **Azure SQL Database** in free tier
- [ ] Create **Azure SignalR Service** in Free tier
- [ ] Create **Azure App Service** plan **F1** + Web App for the API
- [ ] Create **Azure Static Web App** for the frontend

### Configure
- [ ] SQL connection string in App Settings
- [ ] SignalR connection string in App Settings
- [ ] CORS on App Service: allow the Static Web App URL
- [ ] Environment variables: `ASPNETCORE_ENVIRONMENT=Production`, JWT secret, etc.

### CI/CD
- [ ] GitHub Actions workflow to build + deploy the API
- [ ] Workflow to build + deploy the Angular frontend
- [ ] EF Core migrations applied on startup (or via separate job)

---

## 📝 To showcase the project (CV / LinkedIn)

- [ ] **Polished README** with:
  - Screenshots of the app
  - Architecture diagram (Excalidraw or draw.io)
  - Technical choices explained (why SignalR, why Azure SignalR Service…)
  - Link to the live demo
  - Instructions to run locally
- [ ] **Unit tests** on the service layer (xUnit + Moq)
- [ ] **Suggested LinkedIn posts**:
  - "Learning .NET + Angular: architecture I chose"
  - "First SignalR implementation — lessons learned"
  - "Setting up CI/CD with GitHub Actions → Azure"
  - "Project wrap-up: what I learned"
- [ ] **API documentation**: Swagger published

---

## ⚠️ Pitfalls to avoid

- Don't commit `appsettings.Development.json` with secrets → use **User Secrets** locally
- Don't forget to enable **WebSockets** in App Service config (even with SignalR Service, cleaner that way)
- App Service F1 **goes to sleep** after ~20 min of inactivity — first request is slow, normal
- SignalR Service free tier capped at **20 concurrent connections** — fine for demo, not production
- Misconfigured CORS = the #1 cause of "works locally, not in prod"

---

## 🎯 Recommended order to start right now

1. Phase 0 (repo setup + empty projects that boot)
2. Phases 1 → 3 locally **without touching the frontend** (test everything via Postman/Swagger)
3. Minimal Phase 5: auth + static chat screen
4. Phase 4 + wire SignalR from the frontend
5. Phase 6 (polish as you go)
6. Phase 7 once the app works locally

> 💡 **Tip**: deploy to Azure **as early as Phase 3** (before SignalR). It lets you debug deployment issues on a simple app, rather than all at once at the end.
