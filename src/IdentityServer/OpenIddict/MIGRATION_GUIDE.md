# 🔄 Migration Guide - Using OpenIddict Token Endpoints

## Overview

This project has been refactored to use **OpenIddict's standard OAuth2/OpenID Connect endpoints** instead of custom JWT token generation. This provides better standards compliance and feature support.

---

## ⚠️ Breaking Changes

### 1. **Login Flow Changed**

#### ❌ Old Way (Deprecated)
```http
POST /api/auth/login
Content-Type: application/json

{
  "userNameOrEmail": "admin@example.com",
  "password": "Admin@123456"
}

Response:
{
  "token": "eyJhbGc...",
  "message": "Login successful"
}
```

#### ✅ New Way (Recommended)
```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&username=admin@example.com
&password=Admin@123456
&scope=openid profile email roles
```

**Response:**
```json
{
  "access_token": "eyJhbGc...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "...",
  "id_token": "..."
}
```

---

## 🚀 Migration Steps

### For API Clients

**Step 1: Update Login Endpoint**
- Change from `/api/auth/login` → `/connect/token`
- Change HTTP method to `POST`
- Change Content-Type to `application/x-www-form-urlencoded`

**Step 2: Update Request Format**
```javascript
// Old
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userNameOrEmail: 'user@example.com',
    password: 'password123'
  })
});

// New
const response = await fetch('/connect/token', {
  method: 'POST',
  headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
  body: new URLSearchParams({
    grant_type: 'password',
    username: 'user@example.com',
    password: 'password123',
    scope: 'openid profile email roles'
  })
});
```

**Step 3: Update Response Handling**
```javascript
// Old
const { token } = await response.json();
localStorage.setItem('token', token);

// New
const { access_token, refresh_token, expires_in } = await response.json();
localStorage.setItem('access_token', access_token);
localStorage.setItem('refresh_token', refresh_token);
```

---

## 📚 Available Grant Types

### 1. **Password Grant** (For trusted clients)
```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&username=admin@example.com
&password=Admin@123456
&scope=openid profile email roles
```

### 2. **Authorization Code** (For web apps)
```
Step 1: Redirect to /connect/authorize
Step 2: Exchange code for token at /connect/token
```

### 3. **Client Credentials** (For machine-to-machine)
```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=postman-client
&client_secret=postman-secret
&scope=api
```

### 4. **Refresh Token**
```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=refresh_token
&refresh_token=YOUR_REFRESH_TOKEN
```

---

## 🔐 Available Scopes

| Scope | Description |
|-------|-------------|
| `openid` | Required for OpenID Connect |
| `profile` | User's profile information (name, etc.) |
| `email` | User's email address |
| `roles` | User's assigned roles |
| `api` | Access to custom API resources |
| `offline_access` | Request refresh token |

---

## 🛠️ Additional Endpoints

### Get User Info
```http
GET /connect/userinfo
Authorization: Bearer YOUR_ACCESS_TOKEN
```

### Introspect Token
```http
POST /connect/introspect
Content-Type: application/x-www-form-urlencoded

token=YOUR_TOKEN
&client_id=postman-client
&client_secret=postman-secret
```

### Revoke Token
```http
POST /connect/revoke
Content-Type: application/x-www-form-urlencoded

token=YOUR_TOKEN
&client_id=postman-client
&client_secret=postman-secret
```

### Logout
```http
POST /connect/logout
```

---

## 💡 Benefits of This Approach

✅ **Standards Compliant** - Full OAuth2/OIDC support
✅ **Better Security** - Industry-standard token handling
✅ **More Features** - Refresh tokens, introspection, revocation
✅ **Interoperability** - Works with standard OAuth2 clients
✅ **ID Tokens** - OpenID Connect identity tokens included

---

## 🔧 Configuration

### Pre-seeded Clients

The system comes with these OAuth clients:

#### 1. Postman Client
- **Client ID**: `postman-client`
- **Client Secret**: `postman-secret`
- **Grant Types**: Password, Authorization Code, Refresh Token, Client Credentials
- **Scopes**: openid, profile, email, roles

#### 2. Swagger Client
- **Client ID**: `swagger-client`
- **Grant Types**: Authorization Code, Refresh Token

#### 3. Web Client
- **Client ID**: `web-client`
- **Grant Types**: Authorization Code, Refresh Token

### Default User Account
- **Email**: `admin@example.com`
- **Password**: `Admin@123456`
- **Roles**: Admin, User

---

## ⚙️ Environment Setup

### Required Settings in appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=IdentityServerDb;..."
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@identityserver.com"
  }
}
```

---

## 🧪 Testing

See `OpenIddict_Usage.http` for comprehensive examples of all authentication flows.

---

## ❓ FAQ

**Q: Can I still use `/api/auth/login`?**
A: It's marked as deprecated and only validates credentials. Use `/connect/token` for actual tokens.

**Q: Do I need a client_id/client_secret for password grant?**
A: Not required but recommended for better security and tracking.

**Q: How do I test in Postman?**
A: Import `OpenIddict_Usage.http` or manually configure OAuth2 in Postman's Authorization tab.

**Q: What happened to JwtTokenService?**
A: Removed. OpenIddict handles all token generation now.

---

## 📞 Support

For issues or questions, check:
- `ARCHITECTURE.md` - System architecture
- `OpenIddict_Usage.http` - API examples
- [OpenIddict Documentation](https://documentation.openiddict.com/)
