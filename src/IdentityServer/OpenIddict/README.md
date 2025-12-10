# 🔐 OpenIddict Identity Server

A complete, production-ready Identity Server built with **OpenIddict** and **Clean Architecture** principles.

## ✨ Features

### Core Features

- ✅ **OAuth 2.0 / OpenID Connect Server** - Full OpenIddict implementation
- ✅ **ASP.NET Core Identity** - User management & authentication
- ✅ **External Authentication** - Google, Facebook, Microsoft, GitHub
- ✅ **Role-Based Authorization** - Flexible role management
- ✅ **Permission-Based Authorization** - Granular permission control
- ✅ **JWT Tokens** - Access & Refresh token support
- ✅ **Session Management** - Track and manage user sessions with device detection
- ✅ **Email Confirmation** - Secure email verification
- ✅ **Password Reset** - Secure password recovery flow

### UI Features

- ✅ **Modern Login/Register Pages** - Beautiful, responsive UI
- ✅ **User Management Dashboard** - Admin panel for user management
- ✅ **Real-time Dashboard Statistics** - Live metrics and user activity monitoring
- ✅ **Role & Permission Management** - UI for authorization configuration
- ✅ **Profile Management** - User profile editing
- ✅ **Session Management** - View and revoke active sessions
- ✅ **Admin Session Control** - Monitor and manage all user sessions

### Technical Features

- ✅ **Clean Architecture** - Domain, Application, Infrastructure, Presentation layers
- ✅ **CQRS Pattern** - MediatR for command/query separation
- ✅ **Entity Framework Core** - SQL Server database
- ✅ **UAParser Integration** - Device and browser detection for sessions
- ✅ **Docker Support** - Containerized deployment
- ✅ **Health Checks** - Endpoint monitoring
- ✅ **Serilog Logging** - Structured logging
- ✅ **API Versioning** - Future-proof API design

## 🏗️ Architecture

```
src/
├── IdentityServer.Domain/          # Domain entities, interfaces, enums
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── Permission.cs
│   │   └── UserSession.cs          # NEW: Session tracking entity
│   ├── Enums/
│   └── Interfaces/
│       └── ISessionService.cs      # NEW: Session management interface
│
├── IdentityServer.Application/     # Business logic, CQRS handlers
│   ├── Commands/
│   │   ├── Auth/
│   │   ├── Users/
│   │   └── Roles/
│   ├── Queries/
│   ├── DTOs/
│   └── Interfaces/
│
├── IdentityServer.Infrastructure/  # Data access, external services
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Migrations/
│   │   └── Configurations/
│   ├── Identity/
│   ├── OpenIddict/
│   └── Services/
│       └── SessionService.cs       # NEW: Session management implementation
│
└── IdentityServer.Api/             # Presentation layer (API + UI)
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── SessionsController.cs   # NEW: User session management
    │   ├── UsersAdminController.cs # ENHANCED: Admin session control
    │   └── DashboardAdminController.cs # ENHANCED: Real-time statistics
    ├── Pages/                      # Razor Pages for UI
    │   ├── Account/
    │   ├── Admin/
    │   └── Profile/
    ├── wwwroot/                    # Static assets (CSS, JS)
    └── Program.cs
```

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### 1. Clone and Setup

```bash
cd f:\NET\CleanArchitecture\src\IdentityServer\OpenIddict
dotnet restore
```

### 2. Configure Database

Update `appsettings.json` in `IdentityServer.Api`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IdentityServerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Run Migrations

```bash
cd src/IdentityServer.Api
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

Navigate to: `https://localhost:5001`

## 🔐 OAuth2 / OIDC Flows

### Supported Flows

1. **Authorization Code Flow** - Web applications
2. **Client Credentials Flow** - Server-to-server
3. **Refresh Token Flow** - Token renewal
4. **Resource Owner Password Flow** - Legacy apps (not recommended)

### Example: Register Client Application

```csharp
// Seed in ApplicationDbContext or via Admin UI
var client = new OpenIddictApplicationDescriptor
{
    ClientId = "my-web-app",
    ClientSecret = "secret-key",
    DisplayName = "My Web Application",
    RedirectUris = { new Uri("https://localhost:7001/signin-oidc") },
    PostLogoutRedirectUris = { new Uri("https://localhost:7001/signout-callback-oidc") },
    Permissions =
    {
        OpenIddictConstants.Permissions.Endpoints.Authorization,
        OpenIddictConstants.Permissions.Endpoints.Token,
        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
        OpenIddictConstants.Permissions.ResponseTypes.Code,
        OpenIddictConstants.Permissions.Scopes.Email,
        OpenIddictConstants.Permissions.Scopes.Profile
    }
};
```

## 👥 External Providers Setup

### Google Authentication

1. Create OAuth 2.0 credentials at [Google Cloud Console](https://console.cloud.google.com/)
2. Add to `appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

### Facebook Authentication

1. Create app at [Facebook Developers](https://developers.facebook.com/)
2. Add to `appsettings.json`:

```json
{
  "Authentication": {
    "Facebook": {
      "AppId": "your-app-id",
      "AppSecret": "your-app-secret"
    }
  }
}
```

### Microsoft Authentication

1. Register app at [Azure Portal](https://portal.azure.com/)
2. Add to `appsettings.json`:

```json
{
  "Authentication": {
    "Microsoft": {
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

## 🔐 Session Management

### Features

- **Device Detection** - Automatically detect device type, OS, and browser using UAParser
- **Session Tracking** - Track login sessions with IP address and user agent
- **Session Control** - Users can view and revoke their active sessions
- **Admin Oversight** - Administrators can monitor and revoke any user's sessions
- **Security Enhancement** - Force logout suspicious sessions remotely

### Device Information

Sessions display user-friendly device information:
- `Desktop - Windows 10 - Chrome 120`
- `Mobile - iOS 17 - Safari 17`
- `Tablet - Android 13 - Chrome 120`

### Admin Session Control

Administrators can:
- View all active sessions for any user
- Force logout specific users
- Monitor session activity across the system
- Enhance security by revoking compromised sessions

## 📊 Dashboard Statistics

### Real-time Metrics

The admin dashboard provides live statistics including:
- **Total Users** - Current user count
- **Active Users** - Users with recent activity
- **Active Sessions** - Current login sessions
- **Total Clients** - Registered OAuth clients
- **Recent Registrations** - New users in last 30 days
- **Admin/User Distribution** - Role-based user counts

### Recent Activities

- **Login History** - Recent user logins with device information
- **Session Monitoring** - Track user session activity
- **Security Overview** - Monitor system access patterns

## 🔑 Role & Permission System

### Roles

- **Admin** - Full system access
- **User** - Standard user access
- **Guest** - Limited access

### Permissions

Granular permissions for fine-grained control:

- `users.read`, `users.create`, `users.update`, `users.delete`
- `roles.read`, `roles.create`, `roles.update`, `roles.delete`
- `permissions.read`, `permissions.assign`

### Usage in Code

```csharp
[Authorize(Policy = "users.delete")]
public async Task<IActionResult> DeleteUser(string userId)
{
    // Only users with "users.delete" permission can access
}
```

## 📊 API Endpoints

### Authentication

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password
- `POST /api/auth/confirm-email` - Confirm email address
- `POST /api/auth/send-confirmation-email` - Resend email confirmation

### Session Management

- `GET /api/sessions` - Get current user's active sessions
- `DELETE /api/sessions/{id}` - Revoke specific session
- `POST /api/sessions/revoke-others` - Revoke all other sessions
- `POST /api/sessions/revoke-all` - Revoke all sessions (logout everywhere)

### User Management

- `GET /api/users` - List all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/{id}/roles` - Get user roles
- `POST /api/users/{id}/roles` - Assign role to user

### Admin User Management

- `GET /api/admin/users` - List all users (Admin only)
- `GET /api/admin/users/{id}` - Get user details (Admin only)
- `PUT /api/admin/users/{id}` - Update user (Admin only)
- `DELETE /api/admin/users/{id}` - Delete user (Admin only)
- `GET /api/admin/users/{id}/sessions` - View user's sessions (Admin only)
- `POST /api/admin/users/{id}/sessions/revoke-all` - Revoke all user sessions (Admin only)

### Admin Dashboard

- `GET /api/admin/dashboard/stats` - Get real-time dashboard statistics
- `GET /api/admin/dashboard/activities` - Get recent user activities

### Role Management

- `GET /api/roles` - List all roles
- `POST /api/roles` - Create role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role
- `GET /api/roles/{id}/permissions` - Get role permissions
- `POST /api/roles/{id}/permissions` - Assign permission to role

## 🐳 Docker Support

### Build Docker Image

```bash
docker build -t identity-server:latest -f Dockerfile .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📖 Documentation

- [Architecture Overview](docs/ARCHITECTURE.md)
- [External Providers Setup](docs/EXTERNAL_PROVIDERS.md)
- [Role & Permission Guide](docs/AUTHORIZATION.md)
- [API Reference](docs/API_REFERENCE.md)
- [Deployment Guide](docs/DEPLOYMENT.md)

## 🤝 Contributing

This is part of a Clean Architecture demonstration project. Feel free to use as reference for your own implementations.

## 📄 License

MIT License - see LICENSE file for details

## 🙏 Acknowledgments

- **OpenIddict** - OAuth 2.0/OIDC server framework
- **ASP.NET Core Identity** - User management
- **MediatR** - CQRS implementation
- **Entity Framework Core** - Data access

---

**Status**: 🚧 In Development  
**Version**: 1.0.0-alpha  
**Last Updated**: December 2025