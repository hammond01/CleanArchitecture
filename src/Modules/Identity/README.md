# Identity Module

## Overview

Authentication and Authorization module for user identity management.

## Structure

### Domain Layer (Identity.Domain)

- **Entities**: RefreshToken entity
- **Repositories**: IIdentityRepository interface
- **Exceptions**: IdentityDomainException
- **Events**: Domain events (UserLoginEvent, UserRegisteredEvent)
- **DTOs**: Data transfer objects for authentication, registration, password management

### Application Layer (Identity.Application)

- **Features**:
  - Authentication (Login, Logout, RefreshToken)
  - Registration (Create User, Confirm Email)
  - Password Management (Request Reset, Reset Password)
- **Validators**: Input validation using FluentValidation
- **DTOs**: Application-layer DTOs

### Infrastructure Layer (Identity.Infrastructure)

- **Persistence**: Database context and entity configurations
- **Services**: Identity services and extensions
- **Repositories**: IIdentityRepository implementation

### API Layer (Identity.Api)

- **Controllers**: HTTP endpoints for authentication
- **Extensions**: Module registration and dependency injection

## Dependencies

### Domain

- BuildingBlocks.Domain (for IDomainEvent)

### Application

- Identity.Domain
- BuildingBlocks.Application (for CQRS)

### Infrastructure

- Identity.Application
- BuildingBlocks.Infrastructure
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore

### API

- Identity.Infrastructure
- BuildingBlocks.Api
- MediatR
- FluentValidation

## Key Features

- User registration with email confirmation
- Secure password reset flow
- JWT token generation and refresh
- Role-based access control foundation
- Audit logging for security events

## Usage

Register this module in the main API project:

```csharp
builder.Services.AddIdentityModule();
```

## Notes

- Identity entities (User, Role) are managed by ASP.NET Core Identity
- RefreshToken is a custom entity for JWT refresh token management
- IdentityRepository bridges domain operations with ASP.NET Identity
