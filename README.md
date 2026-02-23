# Clean Architecture with Domain-Driven Design (DDD)

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)](CHANGELOG.md)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen.svg)](#clean-architecture)
[![DDD](https://img.shields.io/badge/Design-Domain%20Driven-blue.svg)](#domain-driven-design)
[![CQRS](https://img.shields.io/badge/Pattern-CQRS-orange.svg)](#custom-dispatcher)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![Completion](https://img.shields.io/badge/completion-75%25-yellow.svg)](#status)

A comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**. This v1.0.0 release showcases a production-ready **Modular Monolith** architecture with **Custom CQRS Dispatcher**, **Central Package Management**, and **DbMigrator** tooling for enterprise applications.

![Clean Architecture with DDD](/docs/imgs/CleanArchitecture-DDD.png)

[_(Open on draw.io)_](https://drive.google.com/file/d/1M1YRKcPkgmJCbUcSuJTkMa2N8wAqQrNp/view?usp=sharing)

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#️-architecture-overview)
- [Project Structure](#-project-structure)
- [Technologies](#️-technologies-used)
- [Features](#-key-features)
- [Getting Started](#-getting-started)
- [Development Guide](#-development-guide)
- [Testing](#-testing)
- [Contributing](#-contributing)

## 🚀 Overview

This project is a comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**.

**v1.0.0 represents a production-ready foundation** featuring:

- **Modular Monolith Architecture** - 3 business modules (Identity, Catalog, Auditing) with isolated schemas
- **Custom CQRS Dispatcher** - Unique implementation replacing MediatR with automatic validation and performance monitoring
- **DbMigrator Tool** - Production-ready database migration orchestrator for CI/CD pipelines
- **Central Package Management** - Unified dependency versioning across 18 projects
- **19 Working API Endpoints** - Fully functional REST APIs with Swagger documentation

The implementation showcases enterprise architecture principles with patterns like **CQRS**, **Repository Pattern**, **Unit of Work**, **Specification Pattern**, **Domain Events**, and **Modular Monolith** design.

> **📅 Current Status (February 2026) - v1.0.0 Released**:
>
> - **✅ Custom CQRS Dispatcher**: Complete replacement of MediatR - automatic validation, performance monitoring, structured logging with emojis (🔍⚡📢)
> - **✅ DbMigrator Tool**: Multi-DbContext orchestration, health checks, idempotent execution, Serilog logging
> - **✅ Central Package Management**: Directory.Packages.props managing all 18 projects
> - **✅ 3 Business Modules**: Identity (8 endpoints), Catalog (11 endpoints), Auditing (1 endpoint)
> - **✅ BuildingBlocks**: 5 shared projects providing domain base classes, CQRS infrastructure, API components
> - **🔄 Completion**: ~75% - Production foundation complete, testing infrastructure and advanced auth workflows planned for v1.1.0

### ✨ Key Highlights

- 🏗️ **Clean Architecture**: Clear separation of layers and dependencies with modular monolith structure
- 🎯 **Custom Dispatcher**: Unique CQRS implementation with built-in validation, logging, and performance monitoring (replaces MediatR)
- 🔄 **CQRS Pattern**: Complete Command Query Responsibility Segregation with automatic handler registration
- 🗄️ **DbMigrator**: Professional database migration tool orchestrating multiple DbContexts with health checks
- 📦 **Central Package Management**: Unified dependency versioning via Directory.Packages.props
- 🎯 **Domain-Driven Design**: Focus on business logic and domain model
- � **Repository & Unit of Work**: Enterprise-grade data access patterns with Specification Pattern
- 📊 **19 Working API Endpoints**: Identity (8), Catalog (11), Auditing (1)
- 🔒 **Security Foundation**: JWT authentication infrastructure, CORS configuration
- 📝 **Interactive Swagger UI**: Comprehensive API documentation with live testing
- 🚀 **Production Tooling**: DbMigrator for CI/CD, structured logging with Serilog

## 🏛️ Architecture Overview

This repository demonstrates **Clean Architecture** principles by Uncle Bob implemented across different architectural patterns. Currently, it includes:

### 🎯 Current Implementation: Monolithic Architecture

A complete monolithic application following Clean Architecture with 4 main layers:

### 1. Domain Layer (Core)

- **Entities**: Core business objects
- **Value Objects**: Immutable objects
- **Domain Events**: Business events
- **Repository Interfaces**: Data access contracts
- **Domain Services**: Business logic services

### 2. Application Layer

- **Use Cases**: Application business rules
- **Commands & Queries**: CQRS implementation
- **Handlers**: Command/Query handlers
- **DTOs**: Data transfer objects
- **Validators**: Input validation
- **Mappers**: Object mapping

### 3. Infrastructure Layer

- **Persistence**: Entity Framework Core implementation
- **External Services**: Third-party integrations
- **Caching**: In-memory and distributed caching
- **Logging**: Structured logging with Serilog
- **Authentication**: Identity management
- **Configuration**: Application settings

### 4. Presentation Layer

- **Web API**: RESTful APIs with ASP.NET Core
- **Controllers**: API endpoints
- **Middleware**: Request/Response pipeline
- **API Documentation**: Swagger/OpenAPI integration

### 🚀 Future Implementations

- **Microservices Architecture** - Distributed services with API Gateway (Partially Implemented)
- **Event-Driven Architecture** - Message-driven communication (Planned)
- **CQRS with Event Sourcing** - Advanced CQRS implementation (Planned)
- **Serverless Architecture** - Cloud-native serverless approach (Planned)

## 📁 Project Structure

```
CleanArchitecture/
├── src/
│   ├── BuildingBlocks/                    # 🧱 Shared Building Blocks
│   │   ├── BuildingBlocks.Domain/         # Base entities, domain events
│   │   │   ├── Entities/                  # Entity.cs, IHasKey, ITrackable
│   │   │   ├── Events/                    # IDomainEvent
│   │   │   ├── Repositories/              # IRepository, IUnitOfWork
│   │   │   └── Specifications/            # Specification Pattern
│   │   │
│   │   ├── BuildingBlocks.Application/    # 🎯 Custom Dispatcher & CQRS
│   │   │   ├── CQRS/                      # ICommand, IQuery, ICommandHandler, IQueryHandler
│   │   │   ├── Dispatcher/                # ⭐ Custom Dispatcher (v1.0)
│   │   │   │   ├── IDispatcher.cs         # Main dispatcher interface
│   │   │   │   ├── Dispatcher.cs          # Implementation with validation
│   │   │   │   └── IDomainEventHandler.cs # Domain event handler
│   │   │   ├── DTOs/                      # BaseDto
│   │   │   ├── Results/                   # Result pattern
│   │   │   └── Validation/                # IValidator interface
│   │   │
│   │   ├── BuildingBlocks.Infrastructure/ # Common infrastructure
│   │   │   └── Persistence/               # Base repositories
│   │   │
│   │   ├── BuildingBlocks.Api/            # API base components
│   │   │   ├── Controllers/               # BaseController
│   │   │   ├── Filters/                   # ApiExceptionFilter
│   │   │   └── Responses/                 # ApiResponse
│   │   │
│   │   └── BuildingBlocks.Shared/         # Shared utilities
│   │
│   ├── Modules/                           # 📦 Business Modules
│   │   ├── Identity/                      # 👤 Authentication & User Management
│   │   │   ├── Identity.Domain/           # User, Role entities
│   │   │   ├── Identity.Application/      # 8 Command/Query handlers
│   │   │   │   ├── Commands/              # Register, Login, ConfirmEmail, etc.
│   │   │   │   └── Queries/               # GetUser, ValidateToken
│   │   │   ├── Identity.Infrastructure/   # EF Core, IdentityDbContext
│   │   │   │   ├── Data/                  # DbContext, Migrations
│   │   │   │   └── Repositories/          # User repositories
│   │   │   └── Identity.Api/              # 8 API endpoints
│   │   │       └── Controllers/           # AuthenticationController
│   │   │
│   │   ├── Catalog/                       # 📦 Product & Category Management
│   │   │   ├── Catalog.Domain/            # Product, Category entities
│   │   │   │   ├── Entities/              # Domain models
│   │   │   │   └── Repositories/          # Repository interfaces
│   │   │   ├── Catalog.Application/       # CQRS handlers & Specifications
│   │   │   │   ├── Features/
│   │   │   │   │   ├── Products/          # Product commands & queries
│   │   │   │   │   └── Categories/        # Category commands & queries
│   │   │   │   └── Specifications/        # LowStockProducts, ProductsByPrice, etc.
│   │   │   ├── Catalog.Infrastructure/    # EF Core, CatalogDbContext
│   │   │   │   ├── Data/                  # DbContext, Migrations
│   │   │   │   └── Repositories/          # Repository implementations
│   │   │   └── Catalog.Api/               # 11 API endpoints
│   │   │       └── Controllers/           # ProductsController, CategoriesController
│   │   │
│   │   └── Auditing/                      # 📝 Audit Logging
│   │       ├── Auditing.Domain/           # AuditLog entity
│   │       ├── Auditing.Application/      # Audit query handlers
│   │       ├── Auditing.Infrastructure/   # AuditingDbContext
│   │       │   └── Data/                  # Separate audit schema
│   │       └── Auditing.Api/              # 1 API endpoint
│   │           └── Controllers/           # AuditLogsController
│   │
│   ├── DbMigrator/                        # 🗄️ Database Migration Tool
│   │   ├── DbMigrationService.cs          # Orchestrates 3 DbContexts
│   │   ├── MigrationSettings.cs           # Configuration model
│   │   ├── Program.cs                     # Console app entry point
│   │   ├── appsettings.json               # Migration settings
│   │   ├── README.md                      # Migration guide
│   │   └── DbMigrator.csproj
│   │
│   └── CleanArchitecture.Api/             # 🚀 Main API Gateway
│       ├── Extensions/                    # Module registration
│       │   ├── IdentityModuleExtensions.cs
│       │   ├── CatalogModuleExtensions.cs
│       │   └── AuditingModuleExtensions.cs
│       ├── Program.cs                     # Application entry point
│       ├── appsettings.json               # Configuration
│       └── CleanArchitecture.Api.csproj
│
├── docs/                                  # 📚 Documentation
│   └── imgs/
│       └── CleanArchitecture-DDD.png
│
├── Directory.Packages.props               # 📦 Central package management
├── CHANGELOG.md                           # Version history
├── RELEASE_NOTES_v1.0.0.md               # v1.0.0 release notes
├── QUICK_START_v1.0.0.md                 # Quick start guide
├── ARCHITECTURE.md                        # Architecture documentation
└── README.md                              # This file
```

### 🎯 Architecture Highlights

**Modular Monolith with Clean Architecture** (v1.0.0):

- **18 Projects** total across BuildingBlocks, Modules, DbMigrator, and API
- **3 Business Modules** (Identity, Catalog, Auditing) with separate schemas
- **Custom Dispatcher** replacing MediatR for unique identity
- **Central Package Management** for unified versioning
- **Production Tooling** with DbMigrator for CI/CD pipelines

## 🛠️ Technologies Used

### Backend Technologies

- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Entity Framework Core 8.0.10** - ORM
- **Custom Dispatcher** - CQRS implementation (replaces MediatR)
- **FluentValidation 11.9.0** - Input validation
- **Serilog 4.2.0** - Structured logging
- **Mapster** - Object mapping
- **Swagger/Swashbuckle 7.0.0** - API documentation

### Database & Caching

- **SQL Server** - Primary database
- **In-Memory Database** - Testing
- **IMemoryCache** - In-memory caching
- **Redis** (Optional) - Distributed caching

### DevOps & Tools

- **Docker** - Containerization
- **docker-compose** - Multi-container deployment
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **GitHub Actions** - CI/CD pipeline

## 🎯 Key Features

### 🏛️ Modular Monolith Architecture (v1.0.0 - Production Foundation)

#### Business Modules (3 Modules Implemented)

- ✅ **Identity Module** - Authentication & User Management (8 API endpoints)
    - User registration and login
    - JWT token generation & refresh token support
    - Email confirmation workflow
    - Password reset functionality
    - Full CQRS implementation with Custom Dispatcher

- ✅ **Catalog Module** - Product & Category Management (11 API endpoints)
    - Product CRUD with category relationships
    - Category management with hierarchical support
    - CSV export functionality
    - Specification Pattern for complex queries
    - Full CQRS implementation with Custom Dispatcher

- ✅ **Auditing Module** - Audit Logging (1 API endpoint)
    - Centralized audit log storage
    - Query with pagination support
    - Separate database schema for isolation

#### Enterprise Technical Features Implemented

- 🔐 **Authentication & Authorization** - Complete JWT-based security
    - User login and registration with Identity framework
    - Refresh token support
    - Role-based authorization ready
    - Password validation and security

- 🛡️ **Security Middleware** - 8 middleware components:
    - GlobalExceptionHandlerMiddleware - Centralized exception handling
    - ApiRequestLoggingMiddleware - Request/response logging
    - RateLimitingMiddleware - Request throttling (100 req/min default)
    - AutoEntityLockMiddleware - Automatic entity locking
    - ActionLoggingFilter - Action-level logging
    - LoggingStatusCodeMiddleware - Status code logging
    - CORS Configuration - Environment-specific policies
    - Response Compression - Gzip and Brotli support

- � **API Features**:
    - API Versioning (v1.0, v2.0) with Asp.Versioning
    - OData support for advanced querying
    - Swagger/OpenAPI with enhanced UI
    - Health checks (Database & Application)
    - XML documentation support

- 🗄️ **Data Access**:
    - Entity Framework Core with SQL Server
    - Repository pattern implementation
    - Unit of Work pattern
    - Generic repository with CRUD operations
    - Audit logging and entity tracking

- � **Logging & Monitoring**:
    - Serilog for structured logging
    - File and console logging
    - Request/response logging
    - Performance monitoring
    - Audit log entries tracking

- 🧪 **Testing Infrastructure** (17 test files):
    - **Product Tests** (6 files): AddOrUpdate, Delete, GetById, Gets, CommandQuery, FeatureSummary
    - **Category Tests** (6 files): AddOrUpdate, Delete, GetById, Gets, CommandQuery, FeatureSummary
    - **Integration Tests** (5 files): Category, Product, Health, Identity, Logs Controllers

### 🎯 Custom Dispatcher System (v1.0.0 - Unique Identity)

#### Core Features

- 🎯 **IDispatcher Interface** - Main dispatcher with Query, Command, and DomainEvent methods
    - Generic Query<TResponse> for read operations
    - Generic Command<TResponse> for write operations
    - PublishDomainEvent<TEvent> for domain events

- ⚡ **Automatic Validation** - FluentValidation integration in pipeline:
    - Validates all commands and queries before execution
    - Returns detailed validation errors
    - No need for manual validation in handlers

- 📊 **Performance Monitoring** - Built-in Stopwatch:
    - Measures execution time for all operations
    - Structured logging with performance metrics
    - Helps identify slow queries/commands

- 📢 **Structured Logging** - Emoji-based log indicators:
    - 🔍 Query execution logs
    - ⚡ Command execution logs
    - 📢 Domain event publication logs
    - Detailed error logging with stack traces

- 🔧 **Automatic Handler Registration** - Reflection-based discovery:
    - AddHandlersFromAssembly() extension method
    - Registers all ICommandHandler<,> and IQueryHandler<,>
    - Registers all IDomainEventHandler<> implementations
    - No manual registration needed

### 🗄️ DbMigrator Tool (Production-Ready)

#### Features

- 📦 **Multi-DbContext Support** - Orchestrates 3 separate contexts:
    - IdentityDbContext (identity schema)
    - CatalogDbContext (catalog schema)
    - AuditingDbContext (auditing schema)

- 🏥 **Health Checks** - Pre-migration validation:
    - Database connectivity tests
    - Schema existence verification
    - Detailed error reporting

- 📝 **Structured Logging** - Serilog integration:
    - Console output with color-coded levels
    - File logging (logs/dbmigrator-.txt)
    - Detailed migration progress tracking

- 🔄 **Idempotent Execution** - Safe to run multiple times:
    - Applies only pending migrations
    - Skips already-applied migrations
    - Transaction support

- 🚀 **CI/CD Ready** - Production deployment support:
    - Exit codes for automation (0=success, 1=failure)
    - Configuration via appsettings.json
    - Seed data infrastructure ready

### 📦 Central Package Management

- **Directory.Packages.props** - Single source of truth for all package versions
- **ManagePackageVersionsCentrally** - Enabled across all 18 projects
- **Organized Groups** - Core, EF Core, ASP.NET, Serilog, Testing, Tools
- **Version Consistency** - No version conflicts between projects

## 🚀 Getting Started

### 📋 Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** or **SQL Server Express** - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control

### 🎯 Quick Start (v1.0.0)

```bash
# Clone the repository
git clone https://github.com/hammond01/CleanArchitecture.git
cd CleanArchitecture

# Update connection string in appsettings.json
# Edit: src/CleanArchitecture.Api/appsettings.json
# ConnectionStrings:DefaultConnection = "Server=YOUR_SERVER;..."

# Run database migrations
dotnet run --project src/DbMigrator

# Or migrate manually for each module:
dotnet ef database update --project src/Modules/Identity/Identity.Infrastructure --startup-project src/CleanArchitecture.Api --context IdentityDbContext
dotnet ef database update --project src/Modules/Catalog/Catalog.Infrastructure --startup-project src/CleanArchitecture.Api --context CatalogDbContext
dotnet ef database update --project src/Modules/Auditing/Auditing.Infrastructure --startup-project src/CleanArchitecture.Api --context AuditingDbContext

# Run the API
dotnet run --project src/CleanArchitecture.Api

# Access Swagger UI
# Open: http://localhost:5000/swagger
```

### 📚 Documentation Links

- **[CHANGELOG.md](CHANGELOG.md)** - Version history and changes
- **[RELEASE_NOTES_v1.0.0.md](RELEASE_NOTES_v1.0.0.md)** - Complete v1.0.0 release notes (282 lines)
- **[QUICK_START_v1.0.0.md](QUICK_START_v1.0.0.md)** - Quick reference guide
- **[DbMigrator README](src/DbMigrator/README.md)** - Database migration guide
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Architecture documentation

### 📋 System Requirements

All implementations share these common requirements:

- **.NET 8 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **SQL Server** or **SQL Server Express** (for database)
- **Git** for version control
- **Docker** (optional, for containerized deployments)
- **Postman** or similar (for API testing)

### � API Endpoints (19 Total)

**Identity Module (8 endpoints)**:

- POST `/api/authentication/register` - User registration
- POST `/api/authentication/login` - User login
- POST `/api/authentication/refresh-token` - Refresh JWT token
- POST `/api/authentication/confirm-email` - Email confirmation
- POST `/api/authentication/forgot-password` - Initiate password reset
- POST `/api/authentication/reset-password` - Complete password reset
- GET `/api/authentication/user` - Get user profile
- POST `/api/authentication/logout` - User logout

**Catalog Module (11 endpoints)**:

- GET `/api/products` - List all products
- GET `/api/products/{id}` - Get product by ID
- POST `/api/products` - Create product
- PUT `/api/products/{id}` - Update product
- DELETE `/api/products/{id}` - Delete product
- GET `/api/products/export` - Export products to CSV
- GET `/api/categories` - List all categories
- GET `/api/categories/{id}` - Get category by ID
- POST `/api/categories` - Create category
- PUT `/api/categories/{id}` - Update category
- DELETE `/api/categories/{id}` - Delete category

**Auditing Module (1 endpoint)**:

- GET `/api/auditlogs` - Query audit logs with pagination

## 👨‍💻 Development Guide

### Adding New Module

1. **Create Module Structure** in `src/Modules/YourModule/`

    ```
    YourModule/
    ├── YourModule.Domain/
    ├── YourModule.Application/
    ├── YourModule.Infrastructure/
    └── YourModule.Api/
    ```

2. **Create Domain Entities** in `YourModule.Domain/Entities/`
    - Inherit from `Entity` base class from BuildingBlocks.Domain
    - Implement business rules and domain events

3. **Create Repository Interfaces** in `YourModule.Domain/Repositories/`
    - Inherit from `IRepository<T>` from BuildingBlocks.Domain

4. **Create Commands/Queries** in `YourModule.Application/Features/`
    - Commands implement `ICommand<TResponse>`
    - Queries implement `IQuery<TResponse>`
    - Add FluentValidation validators

5. **Create Handlers** in `YourModule.Application/Features/`
    - Command handlers implement `ICommandHandler<TCommand, TResponse>`
    - Query handlers implement `IQueryHandler<TQuery, TResponse>`
    - Use Custom Dispatcher (no MediatR!)

6. **Implement Infrastructure** in `YourModule.Infrastructure/`
    - Create DbContext inheriting from DbContext
    - Implement repositories
    - Add migrations: `dotnet ef migrations add InitialCreate`

7. **Create API Controllers** in `YourModule.Api/Controllers/`
    - Inject `IDispatcher` from BuildingBlocks.Application
    - Use `dispatcher.Query()` or `dispatcher.Command()`

8. **Register Module** in `src/CleanArchitecture.Api/Extensions/`
    - Create `YourModuleExtensions.cs`
    - Register DbContext, repositories, and handlers
    - Call `services.AddHandlersFromAssembly(typeof(YourHandler).Assembly)`

### Adding New Entity to Existing Module

1. **Create Domain Entity** in `ModuleName.Domain/Entities/`
2. **Create Repository Interface** in `ModuleName.Domain/Repositories/`
3. **Implement Repository** in `ModuleName.Infrastructure/Repositories/`
4. **Create Commands/Queries** in `ModuleName.Application/Features/EntityName/`
5. **Create Handlers** for Commands/Queries (use `IDispatcher`)
6. **Update DbContext** to include new `DbSet<Entity>`
7. **Create Migration**: `dotnet ef migrations add AddEntityName`
8. **Create Controller** in `ModuleName.Api/Controllers/`
9. **Write Tests** (if test infrastructure exists)

### Code Standards

- Use **C# naming conventions**
- Follow **Clean Code principles**
- Implement **proper error handling**
- Add **comprehensive logging** (use Serilog)
- Use **Custom Dispatcher** (not MediatR)
- Write **FluentValidation** validators for all commands/queries
- Document **public APIs** with XML comments
- Follow **Modular Monolith** principles - keep modules isolated

## 🧪 Testing

> **⚠️ Note**: Testing infrastructure is currently in planning phase for v1.0.0. The focus has been on building solid foundation with Custom Dispatcher, DbMigrator, and core business modules.

### 🔮 Planned Testing Strategy (v1.1.0+)

**Unit Testing** (Planned):

- Test Custom Dispatcher with mock handlers
- Test CQRS handlers in isolation
- Test domain entities and business rules
- Test FluentValidation validators
- Test Specification Pattern implementations

**Integration Testing** (Planned):

- Test API endpoints end-to-end
- Test database operations with TestContainers
- Test Custom Dispatcher with real handlers
- Test module integration

**Test Infrastructure** (To Be Implemented):

- **xUnit** - Testing framework
- **Moq** or **NSubstitute** - Mocking framework
- **FluentAssertions** - Assertion library
- **Testcontainers** - Docker-based integration tests
- **WebApplicationFactory** - API integration tests

### 🔧 Running Tests (Future)

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"

# Run specific module tests
dotnet test --filter "FullyQualifiedName~Catalog"
```

### 📋 Test Coverage Goals (v1.1.0)

- **Unit Tests**: 80%+ code coverage
- **Integration Tests**: All API endpoints
- **E2E Tests**: Critical user flows
- **Performance Tests**: DbMigrator and Dispatcher benchmarks

## 📚 Documentation

- [Clean Architecture Guide](docs/CleanArchitecture.md)
- [Domain-Driven Design](docs/DDD.md)
- [API Documentation](docs/API.md)
- [Database Schema](docs/Database.md)
- [Development Guide](docs/Development.md)
- [Deployment Guide](docs/Deployment.md)

## 🤝 Contributing

We welcome all contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) for details.

### How to contribute:

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Uncle Bob
- [Domain-Driven Design](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) by Eric Evans
- [.NET Community](https://dotnetfoundation.org/)

## 📞 Contact

- **Author**: hammond01
- **GitHub**: [https://github.com/hammond01](https://github.com/hammond01)
- **Project Link**: [https://github.com/hammond01/CleanArchitecture](https://github.com/hammond01/CleanArchitecture)
- **LinkedIn**: [Connect with me on LinkedIn](https://linkedin.com/in/hammond01)

### 🎯 Project Roadmap

**v1.0.0** (February 2026) - ✅ **RELEASED**:

- ✅ Custom CQRS Dispatcher replacing MediatR
- ✅ DbMigrator production tool
- ✅ Central Package Management
- ✅ 3 business modules (Identity, Catalog, Auditing)
- ✅ 19 working API endpoints
- ✅ Complete documentation (CHANGELOG, RELEASE_NOTES, QUICK_START)

**v1.1.0** (Q2 2026) - 🚧 **Planned**:

- 🔄 Complete authentication workflows (email verification, password reset)
- 🔄 Comprehensive testing infrastructure (Unit + Integration tests)
- 🔄 API health checks and monitoring endpoints
- 🔄 Docker Compose for complete environment
- 🔄 CI/CD pipeline with GitHub Actions

**v1.2.0** (Q3 2026) - 📋 **Planned**:

- 📋 Additional business modules (Orders, Inventory, Customers)
- 📋 Advanced querying with Specification Pattern expansion
- 📋 Caching layer (Redis integration)
- 📋 Background job processing
- 📋 API rate limiting and throttling

**v2.0.0** (Q4 2026) - 💡 **Future**:

- 💡 Event Sourcing implementation
- 💡 CQRS with separate read/write databases
- 💡 Real-time features with SignalR
- 💡 Advanced security (OAuth2, OpenID Connect)
- 💡 Multi-tenancy support

### 🔄 Architecture Journey

This project demonstrates **Modular Monolith** architecture with a clear path to distributed systems:

1. **✅ Modular Monolith Foundation** (v1.0.0 - Current)
    - 3 business modules with separate schemas
    - Custom Dispatcher for CQRS
    - Production tooling (DbMigrator)
    - Clean Architecture with DDD principles

2. **🔄 Enhanced Modular Monolith** (v1.x Future)
    - Additional business modules
    - Complete testing coverage
    - Advanced observability
    - Performance optimizations

3. **💡 Future Evolution Options** (v2.0+)
    - **Option A**: Event-Driven Modular Monolith (event sourcing within modules)
    - **Option B**: Microservices decomposition (when scaling demands require it)
    - **Option C**: Hybrid approach (some modules as microservices)

### 📊 Current Implementation Status (v1.0.0)

| Component             | Status      | Progress | Details                                       |
| --------------------- | ----------- | -------- | --------------------------------------------- |
| **Custom Dispatcher** | ✅ Complete | 100%     | CQRS, Validation, Logging, Perf Monitoring    |
| **DbMigrator**        | ✅ Complete | 100%     | Multi-DbContext, Health Checks, CI/CD         |
| **Central Packages**  | ✅ Complete | 100%     | 18 projects, unified versioning               |
| **Identity Module**   | 🟡 Partial  | 70%      | 8 endpoints, workflows need completion        |
| **Catalog Module**    | ✅ Complete | 100%     | 11 endpoints, full CRUD + Specifications      |
| **Auditing Module**   | ✅ Complete | 100%     | 1 endpoint, separate schema                   |
| **Testing**           | 🔴 Planned  | 0%       | Infrastructure planned for v1.1.0             |
| **Documentation**     | ✅ Complete | 100%     | README, CHANGELOG, RELEASE_NOTES, QUICK_START |
| **Overall Project**   | 🟡 v1.0.0   | **75%**  | Production foundation ready                   |

---

⭐ **If this project helped you learn Clean Architecture and modern .NET development, please give it a star!** ⭐

🚀 **Follow the project for updates on new architecture implementations!** 🚀
