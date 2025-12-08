# Clean Architecture with Domain-Driven Design (DDD)

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen.svg)](#clean-architecture)
[![DDD](https://img.shields.io/badge/Design-Domain%20Driven-blue.svg)](#domain-driven-design)
[![Build Status](https://img.shields.io/badge/build-foundation%20complete-yellow.svg)](#)
[![Tests](https://img.shields.io/badge/tests-infrastructure%20ready-yellow.svg)](#testing)
[![Production Ready](https://img.shields.io/badge/status-implementation%20foundation-yellow.svg)](#status)

A comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**. This project demonstrates architectural patterns for building enterprise applications, featuring a **Monolithic Architecture** foundation with **Microservices Architecture** partially implemented, and plans for **Event-Driven** and **Serverless** architectures.
[![Tests](https://img.shields.io/badge/tests-infrastructure%20ready-yellow.svg)](#testing)
[![Production Ready](https://img.shields.io/badge/status-implementation%20foundation-yellow.svg)](#status)

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

This project is a comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**. It demonstrates different architectural approaches for building enterprise applications, currently featuring:

- **Monolithic Architecture** - Complete foundation with 2 main business modules (Product & Category)
- **Microservices Architecture** - Partial implementation with API Gateway and 2 microservices
- **Event-Driven Architecture** - Planning phase (documentation only)
- **Serverless Architecture** - Planning phase (documentation only)

The current implementations showcase enterprise architecture principles with patterns like **CQRS**, **Repository Pattern**, **Unit of Work**, **Mediator**, **API Versioning**, **Enhanced Security**, and **Advanced Middleware Pipeline**.

> **📅 Current Status (October 2025)**:
>
> - **Monolithic Architecture**: Has complete architectural structure with Product and Category management modules, enterprise security features, and comprehensive middleware pipeline. Requires solution file path fixes.
> - **Microservices Architecture**: Partially implemented with API Gateway, CustomerManagement service, and ProductCatalog service with full Clean Architecture layers.
> - **Event-Driven & Serverless**: Planning documentation ready for future implementation.

### ✨ Key Highlights

- 🏗️ **Clean Architecture**: Clear separation of layers and dependencies
- 🎯 **Domain-Driven Design**: Focus on business logic and domain model
- 🏛️ **Monolithic Foundation**: 2 core modules (Product & Category) with full CQRS implementation
- 🔄 **CQRS Pattern**: Complete Command Query Responsibility Segregation with MediatR
- 🏪 **Repository & Unit of Work**: Enterprise-grade data access patterns implemented
- 🧪 **Testing Infrastructure**: 17 test files (12 unit tests, 5 integration tests) with xUnit and Moq
- 📊 **Advanced APIs**: RESTful APIs with API versioning (v1.0, v2.0) and OData support
- 🔒 **Enterprise Security**: JWT authentication, CORS, rate limiting, IP whitelisting, request signing
- 🚀 **Performance Features**: Response caching, compression (Gzip/Brotli), entity locking
- 📝 **Rich API Documentation**: Interactive Swagger UI with comprehensive documentation
- 🌐 **Microservices Foundation**: API Gateway + 2 services with event bus infrastructure
- 🔧 **Advanced Middleware**: 8 middleware components for logging, rate limiting, exception handling

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
src/
├── MonolithArchitecture/           # 🏛️ Monolithic Architecture Implementation
│   ├── src/
│   │   ├── Application/           # Application layer
│   │   │   ├── ProductManager.Application/    # Use cases, CQRS handlers
│   │   │   └── ProductManager.Domain/         # Domain entities, business rules
│   │   ├── Infrastructure/        # Infrastructure layer
│   │   │   ├── ProductManager.Infrastructure/ # External services, caching
│   │   │   └── ProductManager.Persistence/    # Data access, Entity Framework
│   │   ├── Presentation/          # Presentation layer
│   │   │   └── APIs/              # RESTful Web API projects
│   │   └── CrossCuttingConcerns/  # Shared components
│   │       ├── ProductManager.Shared/         # Common utilities, DTOs
│   │       └── ProductManager.Constants/      # Application constants
│   ├── tests/                     # Test projects
│   │   ├── UnitTests/             # Unit tests
│   │   └── IntegrationTests/      # Integration tests
│   ├── docs/                      # Documentation
│   │   ├── OData_Integration_Guide.md
│   │   ├── OData_Integration_Summary.md
│   │   └── RESTful_API_Analysis.md
│   ├── docker-compose.yml         # Docker compose configuration
│   ├── Dockerfile                 # Docker configuration
│   ├── MonolithArchitecture.sln   # Solution file
│   └── README.md                  # Detailed setup and implementation guide
├── MicroservicesArchitecture/      # � Microservices Architecture (Partial Implementation)
│   ├── src/
│   │   ├── Gateway/
│   │   │   └── ApiGateway/        # API Gateway service
│   │   ├── Services/
│   │   │   ├── CustomerManagement/# Customer management service
│   │   │   │   ├── CustomerManagement.API/
│   │   │   │   ├── CustomerManagement.Application/
│   │   │   │   ├── CustomerManagement.Domain/
│   │   │   │   └── CustomerManagement.Infrastructure/
│   │   │   └── ProductCatalog/    # Product catalog service
│   │   │       ├── ProductCatalog.API/
│   │   │       ├── ProductCatalog.Application/
│   │   │       ├── ProductCatalog.Domain/
│   │   │       └── ProductCatalog.Infrastructure/
│   │   └── Shared/                # Shared libraries and contracts
│   │       ├── Shared.Common/     # Common utilities
│   │       ├── Shared.Contracts/  # Service contracts
│   │       └── Shared.Events/     # Domain events
│   ├── tests/
│   │   └── UnitTests/             # Unit tests for services
│   ├── MicroservicesArchitecture.sln
│   └── README.md                  # Microservices setup guide
├── EventDrivenArchitecture/        # 🔮 Event-Driven Architecture (Planned)
│   └── README.md                  # Architecture planning document
└── ServerlessArchitecture/         # 🔮 Serverless Architecture (Planned)
    └── README.md                  # Architecture planning document
```

## 🛠️ Technologies Used

### Backend Technologies

- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM
- **MediatR** - Mediator pattern implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

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

### 🏛️ Monolithic Architecture (Current - Implementation Foundation)

#### Business Modules (2 Core Modules Fully Implemented)

- ✅ **Product Management** - Complete CRUD with Commands (AddOrUpdate, Delete) and Queries (GetById, Gets)
  - Product entity with category relationship, pricing, stock management
  - Full CQRS implementation with MediatR handlers
  - Entity locking support for concurrent modification prevention
  - Comprehensive validation and error handling
- ✅ **Category Management** - Complete CRUD with hierarchical support
  - Category entity with product relationships
  - Full CQRS implementation with event handlers
  - Support for category images and descriptions
  - Comprehensive audit logging

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

### 🔄 Microservices Architecture (Partial Implementation)

#### Infrastructure Components

- 🌐 **API Gateway** - Fully implemented gateway service:

  - JWT authentication integration
  - Service proxy with HTTP client
  - Swagger documentation
  - Health monitoring
  - Controllers: Auth, Categories, Products, Orders, Gateway, Health
  - Request routing and load balancing foundation

- � **CustomerManagement Service** - Complete Clean Architecture layers:

  - **API Layer**: RESTful endpoints with Swagger
  - **Application Layer**: CQRS with MediatR, validators, mappers
  - **Domain Layer**: Customer entities, repositories, domain services
  - **Infrastructure Layer**: EF Core, DbContext, data access
  - Full dependency injection configuration

- 📦 **ProductCatalog Service** - Complete Clean Architecture layers:
  - **API Layer**: Product endpoints with full CRUD
  - **Application Layer**: CQRS implementation
  - **Domain Layer**: Product domain models
  - **Infrastructure Layer**: Database access layer

#### Shared Libraries

- **Shared.Common**: String extensions, domain events, EventBus interfaces, exception handling
- **Shared.Contracts**: BaseContract, Customer/Order/Product contracts for service communication
- **Shared.Events**: Integration event definitions for order and product events

#### Testing

- Unit test structure for ProductCatalog service
- Test infrastructure ready for expansion

### � Future Architecture Implementations

#### Event-Driven Architecture (Planned - 2026)

- 📡 **Event Sourcing** - Event-based state management
- 🔔 **Event Store** - Durable event storage with replay capability
- 📝 **Saga Pattern** - Distributed transaction management
- 🔄 **CQRS Enhancement** - Advanced read/write model separation
- 🎯 **Event Handlers** - Asynchronous event processing
- 📊 **Event Analytics** - Real-time stream processing

#### Serverless Architecture (Planned - 2026)

- ⚡ **Azure Functions** - HTTP, Event, Timer, Queue functions
- 🌩️ **Event Triggers** - Event-driven function execution
- 💰 **Cost Optimization** - Pay-per-execution model
- 🔗 **Managed Services** - Cosmos DB, Service Bus, Storage integration
- 🚀 **Auto-scaling** - Serverless compute scaling
- 📊 **Function Monitoring** - Application Insights integration

## 🚀 Getting Started

Choose the architectural pattern you want to explore. Each implementation has its own detailed README with specific setup instructions:

### 🏛️ Available Implementations

#### ✅ [Monolithic Architecture](src/MonolithArchitecture/README.md)

**Status**: 🟡 **Implementation Foundation** (October 2025)

- **2 Core Business Modules**: Product Management and Category Management with full CQRS
- **5 API Controllers**: Product, Category, Identity, Health, Logs
- **8 Middleware Components**: Exception handling, logging, rate limiting, entity locking, CORS, compression
- **Enterprise Security**: JWT authentication, refresh tokens, Identity framework
- **17 Test Files**: 12 unit tests (Product & Category), 5 integration tests
- **Advanced Features**: API versioning (v1.0, v2.0), OData, Swagger UI, health checks
- **Note**: Requires solution file path fixes (currently references old paths)
- **[📖 View Implementation Guide →](src/MonolithArchitecture/README.md)**

#### 🔄 [Microservices Architecture](src/MicroservicesArchitecture/README.md)

**Status**: 🟡 **Partial Implementation** (October 2025)

- **API Gateway**: Complete with 6 controllers (Auth, Categories, Products, Orders, Gateway, Health)
- **2 Microservices Implemented**:
  - **CustomerManagement**: Full Clean Architecture (API, Application, Domain, Infrastructure layers)
  - **ProductCatalog**: Full Clean Architecture with CQRS implementation
- **Shared Libraries**: 3 shared projects (Common, Contracts, Events)
- **Event Infrastructure**: EventBus interfaces and integration event support
- **Testing**: Unit test structure for ProductCatalog
- **Next Steps**: Add OrderManagement, Inventory, Identity services
- **[📖 View Implementation Guide →](src/MicroservicesArchitecture/README.md)**

#### 🔮 [Event-Driven Architecture](src/EventDrivenArchitecture/README.md)

**Status**: 🟡 **Planned for 2026**

- Event Sourcing implementation with event store
- Message-driven communication with publish-subscribe patterns
- Saga pattern for distributed transactions
- CQRS with event replay capabilities
- **[📖 View Architecture Planning →](src/EventDrivenArchitecture/README.md)**

#### 🔮 [Serverless Architecture](src/ServerlessArchitecture/README.md)

**Status**: 🟡 **Planned for 2026**

- Azure Functions implementation
- Event-driven triggers with auto-scaling
- Cloud-native approach with managed services
- Cost-effective serverless computing model
- **[📖 View Architecture Planning →](src/ServerlessArchitecture/README.md)**

### 🎯 Quick Start

For immediate exploration, start with the **Production-Ready Monolithic Architecture**:

```bash
# Clone the repository
git clone https://github.com/hammond01/CleanArchitecture.git
cd CleanArchitecture

# Navigate to Monolithic implementation
cd src/MonolithArchitecture

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project src/Presentation/APIs/ProductManager.Api

# Or use Docker Compose
docker-compose up -d

# Follow the complete setup guide for detailed instructions
# See: src/MonolithArchitecture/README.md
```

### 📋 System Requirements

All implementations share these common requirements:

- **.NET 8 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **SQL Server** or **SQL Server Express** (for database)
- **Git** for version control
- **Docker** (optional, for containerized deployments)
- **Postman** or similar (for API testing)

### 🎓 Learning Path

**Recommended order for studying the implementations:**

1. **🏛️ Monolithic Architecture** (Available Now - 70% Complete)

   - Master Clean Architecture fundamentals with 4-layer separation
   - Understand DDD principles through Product and Category modules
   - Learn CQRS pattern with MediatR implementation
   - Experience enterprise API development with versioning and OData
   - Study comprehensive security implementation (JWT, middleware, rate limiting)
   - Practice testing strategies with 17 test examples

2. **� Microservices Architecture** (In Progress - 40% Complete)

   - Learn service decomposition strategies
   - Understand API Gateway patterns and routing
   - Master inter-service communication with shared contracts
   - Explore database-per-service pattern
   - Study event-driven communication infrastructure
   - Practice distributed system testing

3. **🔮 Event-Driven Architecture** (Planned 2026)

   - Understand event-based patterns and event sourcing
   - Learn CQRS advanced concepts with event store
   - Master asynchronous communication patterns
   - Explore saga patterns for distributed transactions
   - Study eventual consistency strategies

4. **🔮 Serverless Architecture** (Planned 2026)
   - Explore cloud-native serverless approaches
   - Learn function-based architecture patterns
   - Understand event-driven triggers and scaling
   - Master Azure managed services integration
   - Study cost-effective scaling strategies

Each architectural pattern builds upon concepts from the previous ones, creating a comprehensive learning experience from monolithic to distributed systems.

## 👨‍💻 Development Guide

### Adding New Entity

1. **Create Domain Entity** in `ProductManager.Domain/Entities/`
2. **Create Repository Interface** in `ProductManager.Domain/Repositories/`
3. **Implement Repository** in `ProductManager.Persistence/Repositories/`
4. **Create DTOs** in `ProductManager.Shared/DTOs/`
5. **Create Commands/Queries** in `ProductManager.Application/Feature/`
6. **Create Handlers** for Commands/Queries
7. **Create Controller** in `ProductManager.Api/Controllers/`
8. **Write Tests** in test projects

### Code Standards

- Use **C# naming conventions**
- Follow **Clean Code principles**
- Implement **proper error handling**
- Add **comprehensive logging**
- Write **unit tests** for all business logic
- Document **public APIs** with XML comments

## 🧪 Testing

The project has comprehensive test infrastructure with enterprise-grade testing strategies:

### Current Test Status

#### Monolithic Architecture - 17 Test Files

**Unit Tests** (12 files in `tests/UnitTests/Application/`):

- **Product Module Tests** (6 files):

  - `AddOrUpdateProductHandlerTests.cs` - Create/update product tests
  - `DeleteProductHandlerTests.cs` - Product deletion tests
  - `GetProductByIdHandlerTests.cs` - Single product retrieval tests
  - `GetProductsHandlerTests.cs` - Product list query tests
  - `ProductCommandQueryTests.cs` - Command/query validation tests
  - `ProductFeatureSummaryTests.cs` - Feature integration tests

- **Category Module Tests** (6 files):
  - `AddOrUpdateCategoryHandlerTests.cs` - Create/update category tests
  - `DeleteCategoryHandlerTests.cs` - Category deletion tests
  - `GetCategoryByIdHandlerTests.cs` - Single category retrieval tests
  - `GetCategoriesHandlerTests.cs` - Category list query tests
  - `CategoryCommandQueryTests.cs` - Command/query validation tests
  - `CategoryFeatureSummaryTests.cs` - Feature integration tests

**Integration Tests** (5 files in `tests/IntegrationTests/Controllers/`):

- `ProductControllerTests.cs` - Product API endpoint tests
- `CategoryControllerTests.cs` - Category API endpoint tests
- `HealthControllerTests.cs` - Health check endpoint tests
- `IdentityControllerTests.cs` - Authentication/authorization tests
- `LogsControllerTests.cs` - Logging endpoint tests

#### Microservices Architecture

- Unit test structure available for ProductCatalog service
- Test infrastructure ready for expansion

### Test Architecture

- **Test Isolation** - Each test runs independently with proper setup/cleanup
- **Realistic Data** - AutoFixture for generating test data
- **Dependency Mocking** - Moq framework for unit tests
- **Test Patterns** - AAA (Arrange-Act-Assert) pattern
- **Custom Factory** - `CustomWebApplicationFactory.cs` for integration tests

### Running Tests

```bash
# Navigate to the monolithic architecture
cd src/MonolithArchitecture

# Restore dependencies (required due to solution file path issues)
dotnet restore

# Run all tests (after fixing solution file paths)
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"

# Run integration tests only
dotnet test tests/IntegrationTests/

# Run unit tests only
dotnet test tests/UnitTests/

# Run specific test category
dotnet test --filter Category=Product
```

### Known Issues

⚠️ **Solution File Path Issue**: The `MonolithArchitecture.sln` file contains references to old paths (`d:\NetCore\CleanArchitecture\...`). Tests can be run individually from project directories, but the solution file needs path corrections.

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

- **Q4 2025**:
  - Fix Monolithic solution file path issues
  - Complete testing and validation of existing features
  - Add more business modules to Monolithic architecture
- **Q1 2026**:
  - Complete Microservices Architecture with OrderManagement and Inventory services
  - Implement inter-service communication patterns
  - Add distributed tracing and monitoring
- **Q2 2026**:
  - Event-Driven Architecture implementation
  - Event Sourcing with event store
  - Saga pattern for distributed transactions
- **Q3 2026**:
  - Serverless Architecture with Azure Functions
  - Cloud-native managed services integration
- **Q4 2026**:
  - Performance benchmarking across all architectures
  - Complete documentation and deployment guides

### 🔄 Architecture Evolution

This project demonstrates the evolution from a well-designed monolith to distributed architectures:

1. **Monolithic Foundation** (Current) - 2 core modules with full CQRS, JWT security, and enterprise middleware
2. **Microservices Decomposition** (In Progress) - API Gateway + 2 services with shared libraries and event infrastructure
3. **Event-Driven Transformation** (Planned) - Async communication with event sourcing
4. **Serverless Optimization** (Planned) - Cloud-native scaling with Azure Functions

### 📊 Current Implementation Status

| Architecture      | Status              | Progress | Core Features                                    |
| ----------------- | ------------------- | -------- | ------------------------------------------------ |
| **Monolithic**    | 🟡 Foundation Ready | 70%      | 2 modules, 5 controllers, 8 middleware, 17 tests |
| **Microservices** | 🟡 Partial          | 40%      | API Gateway, 2 services, shared libraries        |
| **Event-Driven**  | 🔵 Planned          | 0%       | Documentation ready                              |
| **Serverless**    | 🔵 Planned          | 0%       | Documentation ready                              |

---

⭐ **If this project helped you learn Clean Architecture and modern .NET development, please give it a star!** ⭐

🚀 **Follow the project for updates on new architecture implementations!** 🚀
