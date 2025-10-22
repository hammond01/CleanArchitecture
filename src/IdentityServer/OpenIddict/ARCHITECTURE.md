# 🏗️ Architecture Documentation

## Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                      Presentation Layer                      │
│                  (IdentityServer.Api)                       │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐  │
│  │ Controllers  │  │ Razor Pages  │  │  Middlewares    │  │
│  │              │  │              │  │                 │  │
│  │ - Auth       │  │ - Account/   │  │ - Exception     │  │
│  │ - Token      │  │ - Admin/     │  │ - Audit         │  │
│  │ - Users      │  │ - Profile/   │  │ - RateLimit     │  │
│  │ - Roles      │  │              │  │                 │  │
│  └──────────────┘  └──────────────┘  └─────────────────┘  │
│                                                              │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                     Application Layer                        │
│                (IdentityServer.Application)                  │
│                                                              │
│  ┌──────────────────────┐         ┌──────────────────────┐ │
│  │      Commands        │         │       Queries        │ │
│  │                      │         │                      │ │
│  │ - RegisterUser       │         │ - GetUsers           │ │
│  │ - LoginUser          │         │ - GetUserDetail      │ │
│  │ - Enable2FA          │         │ - GetAuditLogs       │ │
│  │ - CreateRole         │         │ - GetRoles           │ │
│  │ - AssignPermission   │         │ - GetSessions        │ │
│  └──────────────────────┘         └──────────────────────┘ │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                  Handlers (Mediator)                  │  │
│  │                                                        │  │
│  │  Commands ──> CommandHandlers ──> Domain Services     │  │
│  │  Queries  ──> QueryHandlers   ──> Read Models         │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                       Domain Layer                           │
│                  (IdentityServer.Domain)                     │
│                                                              │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────┐ │
│  │    Entities      │  │    Contracts     │  │  Common  │ │
│  │                  │  │                  │  │          │ │
│  │ - ApplicationUser│  │ - IIdentityServ  │  │ - Result │ │
│  │ - ApplicationRole│  │ - IEmailSender   │  │          │ │
│  │ - Permission     │  │ - LoginRequest   │  │          │ │
│  │ - UserSession    │  │ - RegisterReq    │  │          │ │
│  │ - AuditLog       │  │                  │  │          │ │
│  └──────────────────┘  └──────────────────┘  └──────────┘ │
│                                                              │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                   Infrastructure Layer                       │
│               (IdentityServer.Infrastructure)                │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                    Data Access                        │  │
│  │                                                        │  │
│  │  - ApplicationDbContext                               │  │
│  │  - Configurations (Fluent API)                        │  │
│  │  - Migrations                                          │  │
│  │  - OpenIddictSeeder                                    │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                     Services                          │  │
│  │                                                        │  │
│  │  - IdentityService (UserManager, SignInManager)       │  │
│  │  - EmailSender (MailKit)                              │  │
│  │  - TotpService (2FA)                                   │  │
│  │  - SessionService                                      │  │
│  │  - AuditService                                        │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                  Authorization                        │  │
│  │                                                        │  │
│  │  - PermissionAuthorizationHandler                     │  │
│  │  - PermissionRequirement                              │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
└───────────────────────────┬──────────────────────────────────┘
                            │
                            ▼
                    ┌──────────────┐
                    │   Database   │
                    │  SQL Server  │
                    └──────────────┘
```

---

## Dependency Flow

```
Presentation ──> Application ──> Domain <── Infrastructure
      │              │                           │
      │              └───────────────────────────┘
      │
      └────────────> Infrastructure (DI Registration)
```

**Rules:**

-   Domain has NO dependencies (pure business logic)
-   Application depends ONLY on Domain
-   Infrastructure depends on Domain and Application (implements interfaces)
-   Presentation depends on all layers (composition root)

---

## Authentication Flow

### 1. Password Grant Flow (Resource Owner Password Credentials)

```
┌──────┐                                                      ┌────────────┐
│Client│                                                      │ API Server │
└──┬───┘                                                      └─────┬──────┘
   │                                                                │
   │  POST /connect/token                                          │
   │  grant_type=password                                          │
   │  username=user@example.com                                    │
   │  password=secret                                              │
   │ ──────────────────────────────────────────────────────────> │
   │                                                                │
   │                         TokenController                        │
   │                               │                                │
   │                               ▼                                │
   │                    UserManager.FindByNameAsync()              │
   │                               │                                │
   │                               ▼                                │
   │                 SignInManager.CheckPasswordSignInAsync()      │
   │                               │                                │
   │                               ▼                                │
   │                      Create ClaimsIdentity                     │
   │                               │                                │
   │                               ▼                                │
   │                       Generate Tokens                          │
   │                               │                                │
   │  { access_token, refresh_token, expires_in }                  │
   │ <────────────────────────────────────────────────────────── │
   │                                                                │
```

---

### 2. Authorization Code Flow (OAuth2/OIDC)

```
┌──────┐       ┌─────────┐       ┌────────────┐       ┌──────────┐
│Client│       │ Browser │       │ Auth Server│       │   User   │
└──┬───┘       └────┬────┘       └─────┬──────┘       └────┬─────┘
   │                │                   │                    │
   │  Redirect to Authorization Endpoint│                    │
   │ ───────────> │                   │                    │
   │                │                   │                    │
   │                │  GET /connect/authorize               │
   │                │  ?response_type=code                  │
   │                │  &client_id=xxx                       │
   │                │  &redirect_uri=xxx                    │
   │                │  &scope=openid profile                │
   │                │ ───────────────> │                    │
   │                │                   │                    │
   │                │          Show Login Page              │
   │                │ <─────────────── │                    │
   │                │                   │                    │
   │                │       User Enters Credentials         │
   │                │ ──────────────────────────────────> │
   │                │                   │                    │
   │                │          POST /connect/authorize      │
   │                │ ───────────────> │                    │
   │                │                   │                    │
   │                │        Validate Credentials           │
   │                │                   │                    │
   │                │          Show Consent Screen          │
   │                │ <─────────────── │                    │
   │                │                   │                    │
   │                │       User Clicks "Allow"             │
   │                │ ──────────────────────────────────> │
   │                │                   │                    │
   │                │    Generate Authorization Code        │
   │                │                   │                    │
   │                │  Redirect with code                   │
   │                │  ?code=xyz123                         │
   │ <────────────  │ <─────────────── │                    │
   │                │                   │                    │
   │  POST /connect/token              │                    │
   │  grant_type=authorization_code    │                    │
   │  code=xyz123                      │                    │
   │  redirect_uri=xxx                 │                    │
   │ ───────────────────────────────> │                    │
   │                                    │                    │
   │         Exchange code for tokens  │                    │
   │                                    │                    │
   │  { access_token, id_token,        │                    │
   │    refresh_token }                │                    │
   │ <─────────────────────────────── │                    │
   │                                    │                    │
```

---

## Registration Flow

```
┌──────┐                                                ┌────────────┐
│Client│                                                │ API Server │
└──┬───┘                                                └─────┬──────┘
   │                                                          │
   │  POST /api/auth/register                                │
   │  { username, email, password, firstName, lastName }     │
   │ ──────────────────────────────────────────────────> │
   │                                                          │
   │                  AuthController                          │
   │                        │                                 │
   │                        ▼                                 │
   │              Send(RegisterUserCommand)                   │
   │                        │                                 │
   │                        ▼                                 │
   │               RegisterUserHandler                        │
   │                        │                                 │
   │                        ▼                                 │
   │             IIdentityService.RegisterAsync()             │
   │                        │                                 │
   │                        ▼                                 │
   │            Check if username/email exists                │
   │                        │                                 │
   │                        ▼                                 │
   │            UserManager.CreateAsync(user, password)       │
   │                        │                                 │
   │                        ▼                                 │
   │         Generate Email Confirmation Token                │
   │                        │                                 │
   │                        ▼                                 │
   │            EmailSender.SendConfirmationEmail()           │
   │                        │                                 │
   │  { userId, message: "User registered successfully" }     │
   │ <────────────────────────────────────────────────── │
   │                                                          │
```

---

## 2FA Flow

```
┌──────┐                                                ┌────────────┐
│Client│                                                │ API Server │
└──┬───┘                                                └─────┬──────┘
   │                                                          │
   │  POST /api/account/enable-2fa                           │
   │ ──────────────────────────────────────────────────> │
   │                                                          │
   │              Generate TOTP Secret                        │
   │                        │                                 │
   │                        ▼                                 │
   │              Generate QR Code (base64)                   │
   │                        │                                 │
   │  { qrCode, secret, manualEntryKey }                     │
   │ <────────────────────────────────────────────────── │
   │                                                          │
   │  User scans QR code with Authenticator app              │
   │                                                          │
   │  POST /api/account/verify-2fa                           │
   │  { code: "123456" }                                     │
   │ ──────────────────────────────────────────────────> │
   │                                                          │
   │              Verify TOTP code                            │
   │                        │                                 │
   │                        ▼                                 │
   │           Set TwoFactorEnabled = true                    │
   │                        │                                 │
   │                        ▼                                 │
   │              Generate Backup Codes                       │
   │                        │                                 │
   │  { backupCodes: [...] }                                 │
   │ <────────────────────────────────────────────────── │
   │                                                          │
   │                                                          │
   │  === Next Login ===                                     │
   │                                                          │
   │  POST /connect/token (with password)                    │
   │ ──────────────────────────────────────────────────> │
   │                                                          │
   │  { requires_2fa: true }                                 │
   │ <────────────────────────────────────────────────── │
   │                                                          │
   │  POST /api/auth/verify-2fa-login                        │
   │  { code: "789012" }                                     │
   │ ──────────────────────────────────────────────────> │
   │                                                          │
   │              Verify TOTP code                            │
   │                        │                                 │
   │  { access_token, refresh_token }                        │
   │ <────────────────────────────────────────────────── │
   │                                                          │
```

---

## Permission-Based Authorization

```
┌──────────────────────────────────────────────────────────┐
│                    Permission Check                       │
└───────────────────────────┬──────────────────────────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │  [Authorize(Policy="users.    │
            │           delete")]           │
            └───────────────┬───────────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │  PermissionAuthorizationHandler│
            └───────────────┬───────────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │  Get User's Roles             │
            └───────────────┬───────────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │  Get Roles' Permissions       │
            │  (from RolePermission table)  │
            └───────────────┬───────────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │  Check if "users.delete"      │
            │  exists in permissions        │
            └───────────────┬───────────────┘
                            │
                ┌───────────┴──────────┐
                │                      │
                ▼                      ▼
        ┌──────────────┐      ┌──────────────┐
        │   Success    │      │    Forbid    │
        │  (200 OK)    │      │  (403)       │
        └──────────────┘      └──────────────┘
```

---

## Database Schema

```sql
-- Identity Tables
Users
├── Id (Guid, PK)
├── UserName (unique)
├── Email (unique)
├── EmailConfirmed
├── PasswordHash
├── TwoFactorEnabled
├── FirstName
├── LastName
├── ProfilePictureUrl
├── CreatedAt
└── LastLoginAt

Roles
├── Id (Guid, PK)
├── Name
├── Description
├── IsSystemRole
└── CreatedAt

UserRoles (Many-to-Many)
├── UserId (FK -> Users)
├── RoleId (FK -> Roles)
└── AssignedAt

Permissions
├── Id (Guid, PK)
├── Name (e.g., "users.delete")
├── DisplayName
├── Description
├── Category
└── CreatedAt

RolePermissions (Many-to-Many)
├── RoleId (FK -> Roles)
├── PermissionId (FK -> Permissions)
└── AssignedAt

UserSessions
├── Id (Guid, PK)
├── UserId (FK -> Users)
├── RefreshToken
├── RefreshTokenExpiresAt
├── IpAddress
├── UserAgent
├── DeviceInfo
├── CreatedAt
├── LastActivityAt
├── IsActive
└── RevokedAt

AuditLogs
├── Id (Guid, PK)
├── UserId (FK -> Users, nullable)
├── Action (enum)
├── Entity
├── EntityId
├── OldValue (JSON)
├── NewValue (JSON)
├── IpAddress
├── UserAgent
├── Timestamp
└── IsSuccess

-- OpenIddict Tables (auto-generated)
OpenIddictApplications
OpenIddictAuthorizations
OpenIddictScopes
OpenIddictTokens
```

---

## Technology Stack

### Backend

-   **.NET 8.0** - Latest LTS
-   **ASP.NET Core Identity** - User management
-   **OpenIddict** - OAuth2/OIDC server
-   **Entity Framework Core** - ORM
-   **SQL Server** - Database
-   **Mediator** - CQRS pattern (source generator)
-   **MailKit** - Email sending
-   **QRCoder** - QR code generation

### Frontend

-   **Razor Pages** - Server-side rendering
-   **Tailwind CSS** - Utility-first CSS
-   **Alpine.js** - Lightweight JavaScript
-   **Chart.js** - Data visualization

### Testing

-   **xUnit** - Test framework
-   **Moq** - Mocking
-   **FluentAssertions** - Fluent assertions
-   **TestContainers** - Docker containers for testing

### DevOps

-   **Docker** - Containerization
-   **GitHub Actions** - CI/CD
-   **Azure** / **AWS** - Cloud hosting

---

## Security Measures

### Password Security

-   [x] Minimum 8 characters
-   [x] Require uppercase, lowercase, digit, special char
-   [x] Password hashing (ASP.NET Core Identity default)
-   [x] Account lockout after 5 failed attempts
-   [ ] Password history (prevent reuse)
-   [ ] Password expiration policy

### Account Security

-   [x] Email confirmation required
-   [ ] Two-factor authentication (TOTP)
-   [ ] Session management
-   [ ] Security audit logs
-   [ ] Suspicious activity alerts

### API Security

-   [ ] Rate limiting (per IP, per user)
-   [ ] CORS configuration
-   [ ] Security headers (CSP, HSTS, etc.)
-   [ ] Input validation
-   [ ] SQL injection prevention (EF Core)
-   [ ] XSS prevention (Razor encoding)
-   [ ] CSRF protection (anti-forgery tokens)

### Token Security

-   [x] JWT tokens with expiration
-   [x] Refresh token rotation
-   [ ] Token revocation
-   [x] HTTPS only
-   [x] Secure cookie flags

---

## Performance Optimization

### Database

-   [ ] Proper indexing (Email, UserName, CreatedAt)
-   [ ] Query optimization (eager loading vs lazy loading)
-   [ ] Connection pooling
-   [ ] Pagination for large datasets

### Caching

-   [ ] Memory cache for frequently accessed data
-   [ ] Distributed cache (Redis) for scalability
-   [ ] Response caching for static content

### API

-   [ ] Async/await everywhere
-   [ ] Compression (Gzip, Brotli)
-   [ ] CDN for static assets
-   [ ] API versioning

---

## Scalability Considerations

### Horizontal Scaling

-   [ ] Stateless API design
-   [ ] Distributed caching (Redis)
-   [ ] Load balancer configuration
-   [ ] Session affinity (if needed)

### Database Scaling

-   [ ] Read replicas for queries
-   [ ] Write master for commands
-   [ ] Database sharding (future)
-   [ ] Connection pooling

### Monitoring

-   [ ] Application Insights / Prometheus
-   [ ] Health checks endpoint
-   [ ] Log aggregation (ELK stack)
-   [ ] Performance metrics

---

## Deployment Architecture

```
                        ┌─────────────┐
                        │   CDN       │
                        │ (Static)    │
                        └──────┬──────┘
                               │
                        ┌──────▼──────┐
                        │ Load Balancer│
                        └──────┬──────┘
                               │
                ┌──────────────┼──────────────┐
                │              │              │
         ┌──────▼──────┐┌─────▼──────┐┌─────▼──────┐
         │  API Server ││ API Server ││ API Server │
         │  Instance 1 ││ Instance 2 ││ Instance 3 │
         └──────┬──────┘└─────┬──────┘└─────┬──────┘
                │              │              │
                └──────────────┼──────────────┘
                               │
                        ┌──────▼──────┐
                        │   Redis     │
                        │  (Cache)    │
                        └──────┬──────┘
                               │
                        ┌──────▼──────┐
                        │ SQL Server  │
                        │  (Primary)  │
                        └──────┬──────┘
                               │
                        ┌──────▼──────┐
                        │ SQL Server  │
                        │  (Replica)  │
                        └─────────────┘
```

---

## Best Practices

### Code Quality

-   Follow SOLID principles
-   Keep Clean Architecture boundaries
-   Write unit and integration tests
-   Use meaningful names
-   Document complex logic
-   Code reviews

### Security

-   Never trust user input
-   Validate everything
-   Use parameterized queries
-   Keep dependencies updated
-   Regular security audits
-   Follow OWASP guidelines

### Performance

-   Minimize database round-trips
-   Use async/await properly
-   Cache appropriately
-   Monitor and profile
-   Optimize hot paths

---

**Last Updated**: October 22, 2025
**Version**: 1.0.0-alpha
