# Catalog Module

## Overview
This module handles [describe the business capability].

## Structure

### Domain Layer (Catalog.Domain)
- **Entities**: Core business entities
- **ValueObjects**: Immutable value objects
- **Events**: Domain events
- **Repositories**: Repository interfaces
- **Exceptions**: Domain-specific exceptions

### Application Layer (Catalog.Application)
- **Features**: Use cases organized by feature
- **DTOs**: Data transfer objects
- **Validators**: Input validation rules
- **Mappings**: Object mapping profiles
- **Contracts**: Service contracts

### Infrastructure Layer (Catalog.Infrastructure)
- **Persistence**: Database context and configurations
- **Repositories**: Repository implementations
- **Services**: External service integrations

### Api Layer (Catalog.Api)
- **Controllers**: HTTP endpoints
- **Extensions**: Module registration and configuration

## Dependencies

### Domain
- No external dependencies (pure business logic)

### Application
- Domain layer
- MediatR
- FluentValidation
- AutoMapper

### Infrastructure
- Application layer
- Entity Framework Core
- External service SDKs

### Api
- Application layer
- ASP.NET Core

## Usage

Register this module in the main API project:

\\\csharp
builder.Services.AddCatalogModule(builder.Configuration);
\\\

Add connection string to appsettings.json:

\\\json
{
  "ConnectionStrings": {
    "Catalog": "Server=...;Database=ProductManager.Catalog;..."
  }
}
\\\

## Database Migrations

\\\ash
# Add migration
dotnet ef migrations add InitialCreate \\
    --project src/Modules/Catalog/Catalog.Infrastructure \\
    --context CatalogDbContext

# Update database
dotnet ef database update \\
    --project src/Modules/Catalog/Catalog.Infrastructure \\
    --context CatalogDbContext
\\\
