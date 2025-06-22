# ProductManager - Clean Architecture Monolith

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)

A modern e-commerce product management system built with **Clean Architecture** principles using **.NET 8**, **Entity Framework Core**, and **Blazor**. This monolithic application demonstrates enterprise-level patterns including **CQRS**, **Repository Pattern**, **Unit of Work**, and **Domain-Driven Design (DDD)**.

> **Project Status**: This is an active development project implementing Clean Architecture patterns. Some features like comprehensive testing, health checks, and Swagger documentation are planned for future implementation.

## 🏗️ Architecture Overview

This project follows **Clean Architecture** (Onion Architecture) principles with clear separation of concerns across four main layers:

```
┌─────────────────────────────────────┐
│           Presentation              │ ← Web API & Blazor UI
│     (Controllers, Views, etc.)      │
├─────────────────────────────────────┤
│            Application              │ ← Business Use Cases
│     (Commands, Queries, DTOs)       │
├─────────────────────────────────────┤
│              Domain                 │ ← Business Logic & Rules
│     (Entities, Repositories)        │
├─────────────────────────────────────┤
│           Infrastructure            │ ← Data Access & External Services
│   (DbContext, Repositories, etc.)   │
└─────────────────────────────────────┘
```

### 📁 Project Structure

```
src/
├── Application/                      # Application Layer
│   ├── ProductManager.Domain/        # Domain Entities & Contracts
│   └── ProductManager.Application/   # Use Cases, Commands & Queries
├── Infrastructure/                   # Infrastructure Layer
│   ├── ProductManager.Infrastructure/ # External Services
│   └── ProductManager.Persistence/   # Data Access Layer
├── Presentation/                     # Presentation Layer
│   ├── APIs/
│   │   └── ProductManager.Api/       # Web API
│   └── UIs/
│       └── ProductManager.Blazor/    # Blazor Web UI
└── CrossCuttingConcerns/            # Shared Components
    ├── ProductManager.Shared/        # Common DTOs & Utilities
    └── ProductManager.Constants/     # Application Constants
```

## 🚀 Getting Started

### Prerequisites

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB or SQL Server instance)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**

    ```bash
    git clone <repository-url>
    cd ProductManager
    ```

2. **Restore NuGet packages**

    ```bash
    dotnet restore
    ```

3. **Update database connection string**

    Update the connection string in `appsettings.json` files for your SQL Server instance:

    ```json
    {
        "ConnectionStrings": {
            "SQL": "Data Source=your-server;Initial Catalog=ProductManager;Persist Security Info=True;User Id=sa;password=your-password;TrustServerCertificate=true",
            "IDENTITY": "Data Source=your-server;Initial Catalog=ProductManager.Identity;Persist Security Info=True;User Id=sa;password=your-password;TrustServerCertificate=true"
        }
    }
    ```

4. **Run database migrations**

    ```bash
    dotnet ef database update --project src/Infrastructure/ProductManager.Persistence
    ```

5. **Build and run the application**

    ```bash
    # Run Web API
    dotnet run --project src/Presentation/APIs/ProductManager.Api

    # Run Blazor UI (in another terminal)
    dotnet run --project src/Presentation/UIs/ProductManager.Blazor
    ```

## 🛠️ Technologies & Patterns

### Core Technologies

-   **.NET 8.0** - Latest .NET framework
-   **ASP.NET Core** - Web framework
-   **Entity Framework Core 8.0** - ORM for data access
-   **Blazor Server** - Interactive web UI
-   **SQL Server** - Database

### Design Patterns & Principles

-   **Clean Architecture** - Separation of concerns
-   **CQRS** (Command Query Responsibility Segregation)
-   **Repository Pattern** - Data access abstraction
-   **Unit of Work** - Transaction management
-   **Domain-Driven Design (DDD)** - Business logic organization
-   **Dependency Injection** - IoC container

### Key Libraries

-   **MediatR** (12.4.1) - CQRS implementation
-   **AutoMapper** (13.0.1) - Object mapping
-   **Serilog** (4.2.0) - Structured logging
-   **Identity Framework** - Authentication & authorization
-   **Swashbuckle** - API documentation
-   **NSwag** - API client generation

## 📋 Features

### Core Functionality

-   🛍️ **Product Management** - CRUD operations for products
-   📦 **Category Management** - Product categorization
-   👥 **Supplier Management** - Supplier information
-   📊 **Order Management** - Order processing
-   👤 **User Management** - Authentication & authorization
-   📝 **Audit Logging** - Track all system changes

### Technical Features

-   🔐 **Identity Framework** - Authentication & authorization
-   📝 **Audit Logging** - Track system changes via AuditLogEntry
-   📊 **API Logging** - Request/response logging with ApiLogItem
-   🏥 **Health Checks** - Application monitoring (planned)
-   📚 **API Documentation** - Ready for Swagger integration
-   🔄 **Real-time Updates** - SignalR integration (planned)
-   📱 **Responsive UI** - Blazor-based interface
-   🌐 **Multi-database Support** - Separate databases for main data and identity

## 🏛️ Architecture Details

### Domain Layer (`ProductManager.Domain`)

-   **Entities**: Core business entities (Product, Category, Order, etc.)
-   **Repository Interfaces**: Data access contracts
-   **Domain Events**: Business event definitions
-   **Value Objects**: Immutable domain objects
-   **Enums**: Domain-specific enumerations

```csharp
public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
{
    public TKey Id { get; set; } = default!;
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
```

### Application Layer (`ProductManager.Application`)

-   **Commands**: Write operations (Create, Update, Delete)
-   **Queries**: Read operations (Get, List, Search)
-   **Handlers**: Business logic implementation
-   **DTOs**: Data transfer objects
-   **Mappings**: Object transformation logic

```csharp
public class AddOrUpdateProductCommand : ICommand<ApiResponse>
{
    public Products Products { get; set; }
}

internal class AddOrUpdateProductHandler : ICommandHandler<AddOrUpdateProductCommand, ApiResponse>
{
    // Implementation details...
}
```

### Infrastructure Layer

#### Persistence (`ProductManager.Persistence`)

-   **DbContext**: Entity Framework configuration
-   **Repository Implementations**: Data access logic
-   **Migrations**: Database schema changes
-   **Configurations**: Entity mappings

#### Infrastructure (`ProductManager.Infrastructure`)

-   **External Services**: Third-party integrations
-   **Caching**: Redis/Memory cache
-   **File Storage**: Blob/File system
-   **Email Services**: SMTP configuration

### Presentation Layer

#### Web API (`ProductManager.Api`)

-   **Controllers**: HTTP endpoints
-   **Middleware**: Request/response pipeline
-   **Filters**: Cross-cutting concerns
-   **Configuration**: Startup logic

#### Blazor UI (`ProductManager.Blazor`)

-   **Pages**: Blazor components
-   **Services**: UI business logic
-   **Models**: View models
-   **wwwroot**: Static assets

## 🔧 Configuration

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "SQL": "Data Source=127.0.0.1;Initial Catalog=ProductManager;Persist Security Info=True;User Id=sa;password=123456;TrustServerCertificate=true",
        "IDENTITY": "Data Source=127.0.0.1;Initial Catalog=ProductManager.Identity;Persist Security Info=True;User Id=sa;password=123456;TrustServerCertificate=true"
    },
    "AllowedHosts": "*"
}
    }
}
```

### Database Configuration

The application uses **Entity Framework Core** with **SQL Server**:

-   **Code-First approach** with migrations
-   **Automatic relationship mapping**
-   **Audit trail tracking**
-   **Soft delete patterns**

## 🧪 Testing

> **Note**: Test projects are not yet implemented in this solution. This is a planned feature for future development.

### Planned Test Structure

-   **Unit Tests** - Domain & Application logic (To be implemented)
-   **Integration Tests** - API endpoints (To be implemented)
-   **Functional Tests** - End-to-end scenarios (To be implemented)

### Future Testing Implementation

```bash
# Planned test commands (when implemented)
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

## 📦 Deployment

### Development

```bash
dotnet run --project src/Presentation/APIs/ProductManager.Api --environment Development
```

### Production

```bash
# Publish the application
dotnet publish -c Release -o ./publish

# Run published application
cd publish
dotnet ProductManager.Api.dll
```

### Docker (Optional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Presentation/APIs/ProductManager.Api/ProductManager.Api.csproj", "src/Presentation/APIs/ProductManager.Api/"]
# ... other COPY commands
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductManager.Api.dll"]
```

## 📚 API Documentation

Once the application is running, you can access:

-   **Swagger UI**: `https://localhost:5001/swagger` (when configured)
-   **API Endpoints**: `https://localhost:5001/api`

### Available API Endpoints

Based on the current controllers in the project:

-   `GET /api/Product` - List all products
-   `GET /api/Product/{id}` - Get product by ID
-   `POST /api/Product` - Create new product
-   `PUT /api/Product` - Update product
-   `DELETE /api/Product/{id}` - Delete product
-   `GET /api/Category` - Category management endpoints
-   `GET /api/Supplier` - Supplier management endpoints
-   `GET /api/Order` - Order management endpoints
-   `POST /api/Identity` - Identity/Authentication endpoints
-   `GET /api/TestLog` - Test logging endpoint

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Coding Standards

-   Follow **C# coding conventions**
-   Use **meaningful names** for variables and methods
-   Write **unit tests** for new features
-   Update **documentation** as needed
-   Follow **SOLID principles**

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

-   **Your Name** - _Initial work_ - [YourProfile](https://github.com/yourprofile)

## 🙏 Acknowledgments

-   **Clean Architecture** by Robert C. Martin
-   **Domain-Driven Design** by Eric Evans
-   **.NET Community** for excellent documentation
-   **Microsoft** for the amazing .NET platform

## 🔗 Useful Links

-   [Clean Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
-   [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
-   [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
-   [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

---

**Happy Coding! 🚀**
