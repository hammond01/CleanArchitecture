# Clean Architecture with Domain-Driven Design (DDD)

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen.svg)](#clean-architecture)
[![DDD](https://img.shields.io/badge/Design-Domain%20Driven-blue.svg)](#domain-driven-design)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![Tests](https://img.shields.io/badge/tests-137%2F137%20passing-brightgreen.svg)](#testing)
[![Production Ready](https://img.shields.io/badge/status-production%20ready-brightgreen.svg)](#status)

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

This project is a comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**. It demonstrates different architectural approaches for building enterprise applications, currently featuring a **Production-Ready Monolithic Architecture** implementation with plans to expand to **Microservices Architecture**, **Event-Driven Architecture**, and **Serverless Architecture**.

The current implementation showcases a complete e-commerce product management system built following enterprise architecture principles with patterns like **CQRS**, **Repository Pattern**, **Unit of Work**, **Mediator**, **API Versioning**, **Enhanced Security**, and **Advanced Middleware Pipeline**.

> **📅 Current Status (July 2025)**: The Monolithic Architecture is **production-ready** with 137/137 tests passing, complete API documentation, enterprise-grade security features, and comprehensive business modules. New architectural patterns are being planned for implementation.

### ✨ Key Highlights

- 🏗️ **Clean Architecture**: Clear separation of layers and dependencies
- 🎯 **Domain-Driven Design**: Focus on business logic and domain model
- 🏛️ **Enterprise-Grade Monolith**: Production-ready with advanced features
- 🔄 **CQRS Pattern**: Separation of Command and Query operations
- 🏪 **Repository & Unit of Work**: Enterprise-grade data access patterns
- 🧪 **Comprehensive Testing**: Unit tests and Integration tests (137/137 passing)
- 📊 **Advanced APIs**: RESTful APIs with versioning, caching, and security
- 🔒 **Enterprise Security**: JWT, CORS, Rate limiting, IP whitelisting
- 🚀 **Performance Optimized**: Response caching, compression, entity locking
- � **Rich Documentation**: Comprehensive API docs with Swagger UI
- 🌐 **Multi-Architecture Ready**: Prepared for expansion to multiple patterns

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

- **Microservices Architecture** - Distributed services with API Gateway
- **Event-Driven Architecture** - Message-driven communication
- **CQRS with Event Sourcing** - Advanced CQRS implementation
- **Serverless Architecture** - Cloud-native serverless approach

## 📁 Project Structure

```
src/
├── MonolithArchitecture/           # 🏛️ Production-Ready Monolithic Implementation
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
│   │   ├── UnitTests/             # Unit tests (62/62 passing)
│   │   └── IntegrationTests/      # Integration tests (75/75 passing)
│   ├── docs/                      # Documentation
│   │   ├── OData_Integration_Guide.md
│   │   ├── API_Documentation.md
│   │   └── Architecture_Decisions.md
│   └── README.md                  # Detailed setup and implementation guide
├── MicroservicesArchitecture/      # 🔮 Planned Q3 2025: Microservices implementation
│   ├── src/
│   │   ├── ApiGateway/            # API Gateway and routing
│   │   ├── ProductCatalog/        # Product management service
│   │   ├── OrderManagement/       # Order processing service
│   │   ├── CustomerManagement/    # Customer service
│   │   ├── InventoryManagement/   # Inventory service
│   │   └── Identity/              # Authentication service
│   ├── infrastructure/            # Kubernetes, Docker, configs
│   ├── tests/                     # Service tests, contract tests
│   └── README.md                  # Microservices setup guide
├── EventDrivenArchitecture/        # 🔮 Planned Q4 2025: Event-driven implementation
│   ├── src/
│   │   ├── EventStore/            # Event sourcing implementation
│   │   ├── CommandHandlers/       # Command processing
│   │   ├── EventHandlers/         # Event processing
│   │   ├── ReadModels/            # Query projections
│   │   └── Sagas/                 # Workflow coordination
│   ├── infrastructure/            # Event infrastructure
│   ├── tests/                     # Event-driven tests
│   └── README.md                  # Event-driven setup guide
└── ServerlessArchitecture/         # 🔮 Planned Q1 2026: Serverless implementation
    ├── src/
    │   ├── Functions/             # Azure Functions
    │   ├── Orchestrators/         # Durable Functions
    │   ├── Triggers/              # Event triggers
    │   └── Shared/                # Shared libraries
    ├── infrastructure/            # ARM templates, Bicep
    ├── tests/                     # Serverless function tests
    └── README.md                  # Serverless setup guide
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

### 🏛️ Monolithic Architecture (Current - Production Ready)

#### Business Modules (10 modules with full CRUD + Enhanced Features)

- ✅ **Product Management** - Complete product lifecycle management with advanced validation
- ✅ **Category Management** - Hierarchical product categorization system
- ✅ **Supplier Management** - Vendor and supplier information management
- ✅ **Customer Management** - Customer profiles and contact information
- ✅ **Employee Management** - Staff and employee records with roles
- ✅ **Order Management** - Order processing and tracking with comprehensive workflow
- ✅ **OrderDetail Management** - Detailed order line items management
- ✅ **Shipper Management** - Shipping and logistics provider management
- ✅ **Region Management** - Geographic region management for shipping
- ✅ **Territory Management** - Sales territory organization and management

#### Enterprise Technical Features

- 🔐 **Advanced Authentication & Authorization** - JWT-based security with role management
- �️ **Enhanced Security** - CORS policies, IP whitelisting, request signing, rate limiting
- 📊 **API Versioning** - Support for multiple API versions (v1.0, v2.0) with flexible strategies
- 🚀 **Performance Optimization** - Response caching, compression (Gzip/Brotli), entity locking
- 📝 **Rich API Documentation** - Enhanced Swagger UI with comprehensive documentation
- � **Entity Locking** - Distributed locking system for concurrent data modification prevention
- 📈 **Monitoring & Logging** - Comprehensive structured logging with Serilog
- 🔍 **Health Checks** - Application health monitoring and diagnostics
- 🛡️ **Security Headers** - Production-grade security headers and best practices
- 🧪 **Test Coverage** - 137/137 tests passing (Integration + Unit tests)
- 🏗️ **Clean Domain Models** - Pure domain entities with separate configuration
- 🎯 **CQRS Implementation** - Complete Command Query Responsibility Segregation

### 🔮 Future Architecture Implementations

#### Microservices Architecture (Planned - 2025)

- 🌐 **API Gateway** - Centralized entry point with routing and load balancing
- 🔄 **Service-to-Service Communication** - gRPC/HTTP with service mesh
- 📨 **Message Queues** - Asynchronous communication with RabbitMQ/Azure Service Bus
- 🗃️ **Database per Service** - Data isolation and service autonomy
- 🐳 **Container Orchestration** - Kubernetes deployment with auto-scaling
- 📊 **Distributed Tracing** - Observability across services with OpenTelemetry
- 🔒 **Service Security** - OAuth2, mTLS, and service-to-service authentication
- 🚀 **Independent Deployment** - CI/CD pipelines per service

#### Event-Driven Architecture (Planned - 2025)

- 📡 **Event Sourcing** - Event-based state management with event store
- 🔔 **Event Bus** - Event distribution mechanism with publish-subscribe patterns
- 📝 **Saga Pattern** - Distributed transaction management across services
- 🔄 **CQRS with Event Store** - Advanced CQRS implementation with event replay
- 🎯 **Event-Driven Microservices** - Services communicating through domain events
- 📊 **Event Analytics** - Real-time event stream processing and analytics

#### Serverless Architecture (Planned - 2025)

- ⚡ **Azure Functions** - Serverless compute with auto-scaling
- 🌩️ **Event-driven Triggers** - HTTP, Queue, Timer, and Blob triggers
- 💰 **Cost-effective Scaling** - Pay-per-execution pricing model
- 🔗 **Managed Services Integration** - Azure Cosmos DB, Storage, Service Bus
- 🚀 **Serverless APIs** - HTTP-triggered functions with API Management
- 📊 **Serverless Analytics** - Event-driven data processing workflows

## 🚀 Getting Started

Choose the architectural pattern you want to explore. Each implementation has its own detailed README with specific setup instructions:

### 🏛️ Available Implementations

#### ✅ [Monolithic Architecture](src/MonolithArchitecture/README.md)

**Status**: 🟢 **Production Ready** (July 2025)

- Complete enterprise e-commerce product management system
- 10 business modules with full CRUD operations and enhanced features
- 137/137 tests passing (Unit + Integration tests)
- Advanced API features (versioning, caching, security, documentation)
- Enterprise-grade security (JWT, CORS, rate limiting, IP whitelisting)
- Performance optimizations (response caching, compression, entity locking)
- Docker support with docker-compose for easy deployment
- **[📖 View Complete Setup Guide →](src/MonolithArchitecture/README.md)**

#### 🔮 [Microservices Architecture](src/MicroservicesArchitecture/README.md)

**Status**: 🟡 **Planned for 2025**

- Distributed services architecture based on the proven monolith
- API Gateway with service mesh and load balancing
- Service-to-service communication (gRPC/HTTP)
- Container orchestration with Kubernetes
- Event-driven communication patterns
- **[📖 View Architecture Planning →](src/MicroservicesArchitecture/README.md)**

#### 🔮 [Event-Driven Architecture](src/EventDrivenArchitecture/README.md)

**Status**: 🟡 **Planned for 2025**

- Event Sourcing implementation with event store
- Message-driven communication with publish-subscribe patterns
- Saga pattern for distributed transactions
- CQRS with event replay capabilities
- **[📖 View Architecture Planning →](src/EventDrivenArchitecture/README.md)**

#### 🔮 [Serverless Architecture](src/ServerlessArchitecture/README.md)

**Status**: 🟡 **Planned for 2025**

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

1. **🏛️ Monolithic Architecture** (Available Now)

   - Master Clean Architecture fundamentals
   - Understand DDD principles and patterns
   - Learn enterprise-grade API development
   - Experience comprehensive testing strategies

2. **🔮 Microservices Architecture** (Planned 2025)

   - Learn distributed systems concepts
   - Understand service decomposition strategies
   - Master inter-service communication patterns
   - Explore container orchestration

3. **🔮 Event-Driven Architecture** (Planned 2025)

   - Understand event-based patterns
   - Learn event sourcing and CQRS advanced concepts
   - Master asynchronous communication
   - Explore saga patterns for distributed transactions

4. **🔮 Serverless Architecture** (Planned 2025)
   - Explore cloud-native serverless approaches
   - Learn function-based architecture
   - Understand event-driven triggers
   - Master cost-effective scaling strategies

Each architectural pattern builds upon concepts from the previous ones, creating a comprehensive learning experience that covers the spectrum from monolithic to distributed systems.

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

The project has comprehensive test coverage with enterprise-grade testing strategies:

### Current Test Status (137/137 Passing)

#### Unit Tests (62/62 Passing)

- **Application Layer Tests** - Complete coverage of CQRS handlers
- **Domain Logic Tests** - Business rules and entity validation
- **Product Feature Tests** - Full CRUD operations testing
- **Command/Query Tests** - Comprehensive command and query validation
- **Error Handling Tests** - Exception scenarios and edge cases
- **Mock Integration Tests** - Proper dependency mocking with Moq

#### Integration Tests (75/75 Passing)

- **API Endpoint Tests** - End-to-end API testing for all modules
- **Database Tests** - Data access layer validation
- **Controller Tests** - HTTP response validation and status codes
- **Business Module Tests** - Complete CRUD testing for 10 modules
- **Authentication Tests** - JWT and security feature testing
- **Middleware Tests** - Request/response pipeline validation

### Test Architecture

- **Test Isolation** - Each test runs in isolation with proper cleanup
- **Realistic Data** - AutoFixture for generating test data
- **Dependency Mocking** - Moq framework for clean unit tests
- **Test Patterns** - Standardized AAA (Arrange-Act-Assert) pattern
- **CI/CD Integration** - Automated testing in build pipelines

### Running Tests

```bash
# Run all tests
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

- **Q3 2025**: Microservices Architecture implementation
- **Q4 2025**: Event-Driven Architecture with Event Sourcing
- **Q1 2026**: Serverless Architecture with Azure Functions
- **Q2 2026**: Performance benchmarking across all architectures

### 🔄 Architecture Evolution

This project demonstrates the evolution from a well-designed monolith to distributed architectures:

1. **Monolithic Foundation** (Current) - Solid, production-ready base
2. **Microservices Decomposition** - Service boundary identification
3. **Event-Driven Transformation** - Async communication patterns
4. **Serverless Optimization** - Cloud-native scaling strategies

---

⭐ **If this project helped you learn Clean Architecture and modern .NET development, please give it a star!** ⭐

🚀 **Follow the project for updates on new architecture implementations!** 🚀
