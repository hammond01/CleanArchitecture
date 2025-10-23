# IdentityServer với OpenIddict - Admin Management System

## 🏗️ Kiến trúc

### Projects

1. **IdentityServer.Web** (Port 5002/5003)

    - Razor Pages UI
    - Login, Register, OAuth Authorization
    - **Admin Dashboard** - Quản lý toàn bộ hệ thống
    - Tailwind CSS + Alpine.js

2. **IdentityServer.Api** (Port 5000/5001)

    - REST API cho Admin CRUD operations
    - OpenIddict OAuth2/OIDC Server
    - Swagger UI tại `/swagger`

3. **IdentityServer.Application**

    - CQRS với Mediator pattern
    - Commands & Handlers cho Login/Register

4. **IdentityServer.Infrastructure**

    - EF Core + SQL Server
    - OpenIddict data stores
    - Identity services

5. **IdentityServer.Domain**
    - Entities (ApplicationUser, ApplicationRole, etc.)
    - Contracts & interfaces

## 🚀 Cách chạy

### Prerequisites

-   .NET 8.0 SDK
-   SQL Server
-   Visual Studio 2022 hoặc VS Code

### 1. Cấu hình Database

Cập nhật connection string trong `appsettings.json`:

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=IdentityServerDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
    }
}
```

### 2. Chạy Migration

```powershell
cd src/IdentityServer.Infrastructure
dotnet ef database update --startup-project ../IdentityServer.Web
```

### 3. Chạy cả 2 projects

**Terminal 1 - API:**

```powershell
cd src/IdentityServer.Api
dotnet run
```

→ API: https://localhost:5001
→ Swagger: https://localhost:5001/swagger

**Terminal 2 - Web:**

```powershell
cd src/IdentityServer.Web
dotnet run
```

→ Web: https://localhost:5003

**Default Admin Account:**

-   Email: `admin@example.com`
-   Username: `admin`
-   Password: `Admin@123456`

> ⚠️ **Important:** Change the default password after first login!

## 📋 Admin Features

### 🎯 Dashboard (`/admin/dashboard`)

-   Thống kê: Total Users, Active Sessions, OAuth Clients, Active Tokens
-   Charts: User Growth, Login Activity (Chart.js)
-   Recent Activity feed

### 👥 Users Management (`/admin/users`)

-   CRUD users với search, filter, pagination
-   Email confirmation status
-   Role assignments
-   Lock/Unlock accounts

### 🔐 Roles Management (`/admin/roles`)

-   CRUD roles với descriptions
-   Permission assignments
-   User count per role
-   System role protection (Admin, User không thể xóa)

### 🔑 OAuth Clients (`/admin/clients`)

-   CRUD OAuth2/OIDC clients
-   Grant types: Authorization Code, Client Credentials, Refresh Token, Password
-   Redirect URIs management
-   Scopes selection
-   Client secrets

### 🎫 Scopes Management (`/admin/scopes`)

-   CRUD OAuth scopes
-   Resource identifiers (API audiences)
-   Standard OIDC scopes (openid, profile, email)
-   Custom API scopes

### ✅ Authorizations (`/admin/authorizations`)

-   View active authorization codes/tokens
-   Filter by status (valid/revoked), type (permanent/adhoc)
-   Revoke authorizations
-   Subject & client information

## 🔧 API Endpoints

### Admin APIs (Require `[Authorize(Roles = "Admin")]`)

**Dashboard:**

```
GET /api/admin/dashboard/stats
```

**Users:**

```
GET    /api/admin/users?search=&status=&page=1&pageSize=10
GET    /api/admin/users/{id}
POST   /api/admin/users
PUT    /api/admin/users/{id}
DELETE /api/admin/users/{id}
```

**Roles:**

```
GET    /api/admin/roles
GET    /api/admin/roles/{id}
POST   /api/admin/roles
PUT    /api/admin/roles/{id}
DELETE /api/admin/roles/{id}
```

**Clients:**

```
GET    /api/admin/clients
GET    /api/admin/clients/{id}
POST   /api/admin/clients
PUT    /api/admin/clients/{id}
DELETE /api/admin/clients/{id}
```

**Scopes:**

```
GET    /api/admin/scopes?search=
GET    /api/admin/scopes/{id}
POST   /api/admin/scopes
PUT    /api/admin/scopes/{id}
DELETE /api/admin/scopes/{id}
```

**Authorizations:**

```
GET    /api/admin/authorizations?search=&status=&type=&page=1&pageSize=10
GET    /api/admin/authorizations/{id}
POST   /api/admin/authorizations/{id}/revoke
DELETE /api/admin/authorizations/{id}
```

## 🔐 OAuth2/OIDC Endpoints

```
GET  /connect/authorize - Authorization endpoint
POST /connect/token     - Token endpoint
```

## 🎨 UI Components

-   **Tailwind CSS 3.x** - Utility-first CSS framework
-   **Alpine.js 3.x** - Lightweight JavaScript (15KB)
-   **Chart.js 4.4.0** - Charts & visualizations
-   **Toast Notifications** - Global showToast() helper
-   **Responsive Design** - Mobile-friendly admin panel

## 🔒 Security

-   **CORS** configured: Web (5003) → Api (5001)
-   **Authorization**: All admin pages/APIs require Admin role
-   **HTTPS**: Required in production
-   **Cookie Authentication**: Persistent login sessions
-   **OpenIddict**: Industry-standard OAuth2/OIDC implementation

## 📦 NuGet Packages

**OpenIddict 7.1.0:**

-   OpenIddict.AspNetCore
-   OpenIddict.EntityFrameworkCore

**EF Core 8.0.11:**

-   Microsoft.EntityFrameworkCore.SqlServer
-   Microsoft.EntityFrameworkCore.Design

**Mediator 3.0.1:**

-   Mediator.Abstractions
-   Mediator.SourceGenerator

## 🐛 Troubleshooting

### CORS errors

Verify CORS policy in `IdentityServer.Api/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminUI", policy =>
    {
        policy.WithOrigins("https://localhost:5003", "http://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### API not responding

1. Check both Api (5001) and Web (5003) are running
2. Verify database connection
3. Check Swagger at https://localhost:5001/swagger

### Can't login to Admin

1. Create Admin user via SQL:

```sql
-- Check if admin user exists
SELECT * FROM AspNetUsers WHERE UserName = 'admin'

-- Assign Admin role
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id FROM AspNetUsers u, AspNetRoles r
WHERE u.UserName = 'admin' AND r.Name = 'Admin'
```

## 📚 Resources

-   [OpenIddict Documentation](https://documentation.openiddict.com/)
-   [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
-   [Alpine.js Docs](https://alpinejs.dev/)
-   [Tailwind CSS](https://tailwindcss.com/)

## 🎯 Next Steps

-   [ ] Implement DashboardStats với real data
-   [ ] Add user activity logging
-   [ ] Email confirmation flow
-   [ ] 2FA support
-   [ ] API rate limiting
-   [ ] Audit logs for admin actions

---

**Built with ❤️ using Clean Architecture + OpenIddict 7.x**
