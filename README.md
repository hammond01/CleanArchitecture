# Clean Architecture with Domain-Driven Design (DDD)

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen.svg)](#clean-architecture)
[![DDD](https://img.shields.io/badge/Design-Domain%20Driven-blue.svg)](#domain-driven-design)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](#)
[![Tests](https://img.shields.io/badge/tests-137%2F137%20passing-brightgreen.svg)](#testing)

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

This project is a comprehensive implementation of **Clean Architecture** combined with **Domain-Driven Design (DDD)** using **.NET 8**. It demonstrates different architectural approaches for building enterprise applications, currently featuring a **Monolithic Architecture** implementation with plans to expand to **Microservices Architecture** and other patterns.

The current implementation showcases an e-commerce product management system built following enterprise architecture principles with patterns like **CQRS**, **Repository Pattern**, **Unit of Work**, and **Mediator**.

### ✨ Key Highlights

- 🏗️ **Clean Architecture**: Clear separation of layers and dependencies
- 🎯 **Domain-Driven Design**: Focus on business logic and domain model
- 🏛️ **Multiple Architecture Patterns**: Currently Monolithic, expanding to Microservices
- 🔄 **CQRS Pattern**: Separation of Command and Query operations
- 🏪 **Repository & Unit of Work**: Enterprise-grade data access patterns
- 🧪 **Comprehensive Testing**: Unit tests and Integration tests
- 📊 **Standardized APIs**: RESTful APIs with OpenAPI/Swagger documentation
- 🌐 **Blazor UI**: Modern web interface
- 🐳 **Docker Support**: Containerization ready

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
- **Blazor UI**: Interactive web interface
- **Controllers**: API endpoints
- **Middleware**: Request/Response pipeline

### 🚀 Future Implementations

- **Microservices Architecture** - Distributed services with API Gateway
- **Event-Driven Architecture** - Message-driven communication
- **CQRS with Event Sourcing** - Advanced CQRS implementation
- **Serverless Architecture** - Cloud-native serverless approach

## 📁 Project Structure

```
src/
├── MonolithArchitecture/           # 🏛️ Monolithic implementation (Current)
│   ├── src/
│   │   ├── Application/           # Application layer
│   │   │   ├── ProductManager.Application/    # Use cases, CQRS
│   │   │   └── ProductManager.Domain/         # Domain entities, rules
│   │   ├── Infrastructure/        # Infrastructure layer
│   │   │   ├── ProductManager.Infrastructure/ # External services
│   │   │   └── ProductManager.Persistence/    # Data access
│   │   ├── Presentation/          # Presentation layer
│   │   │   ├── APIs/              # Web API projects
│   │   │   └── UIs/               # Blazor UI projects
│   │   └── CrossCuttingConcerns/  # Shared components
│   │       ├── ProductManager.Shared/         # Common utilities
│   │       └── ProductManager.Constants/      # Application constants
│   ├── tests/                     # Test projects
│   │   ├── UnitTests/             # Unit tests
│   │   └── IntegrationTests/      # Integration tests
│   └── docs/                      # Documentation
├── MicroservicesArchitecture/      # 🔮 Future: Microservices implementation
├── EventDrivenArchitecture/        # 🔮 Future: Event-driven implementation
└── ServerlessArchitecture/         # 🔮 Future: Serverless implementation
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

### Frontend Technologies

- **Blazor Server** - Interactive web UI
- **Bootstrap 5** - CSS framework
- **JavaScript/TypeScript** - Client-side scripting

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

### 🏛️ Monolithic Architecture (Current Implementation)

#### Business Modules (10 modules with full CRUD)

- ✅ **Product Management** - Complete product lifecycle management
- ✅ **Category Management** - Product categorization system
- ✅ **Supplier Management** - Vendor and supplier information
- ✅ **Customer Management** - Customer profiles and contact information
- ✅ **Employee Management** - Staff and employee records
- ✅ **Order Management** - Order processing and tracking
- ✅ **OrderDetail Management** - Detailed order line items
- ✅ **Shipper Management** - Shipping and logistics
- ✅ **Region Management** - Geographic region management
- ✅ **Territory Management** - Sales territory organization

#### Technical Features

- 🔐 **Authentication & Authorization** - JWT-based security
- 📊 **Comprehensive Logging** - Structured logging with Serilog
- 🚀 **Performance Monitoring** - Application metrics
- 🔍 **Health Checks** - Application health monitoring
- 📈 **Rate Limiting** - API rate limiting
- 🛡️ **Security Headers** - Security best practices
- 📝 **API Documentation** - Swagger/OpenAPI
- 🧪 **Test Coverage** - 137/137 tests passing

### 🔮 Future Architecture Implementations

#### Microservices Architecture (Planned)

- 🌐 **API Gateway** - Centralized entry point
- 🔄 **Service-to-Service Communication** - gRPC/HTTP
- 📨 **Message Queues** - Asynchronous communication
- 🗃️ **Database per Service** - Data isolation
- 🐳 **Container Orchestration** - Kubernetes/Docker Swarm
- 📊 **Distributed Tracing** - Observability across services

#### Event-Driven Architecture (Planned)

- 📡 **Event Sourcing** - Event-based state management
- 🔔 **Event Bus** - Event distribution mechanism
- 📝 **Saga Pattern** - Distributed transaction management
- 🔄 **CQRS with Event Store** - Advanced CQRS implementation

#### Serverless Architecture (Planned)

- ⚡ **Azure Functions** - Serverless compute
- 🌩️ **Event-driven triggers** - Response to events
- 💰 **Cost-effective scaling** - Pay-per-execution
- 🔗 **Managed services integration** - Cloud-native approach

## 🚀 Getting Started

Choose the architectural pattern you want to explore. Each implementation has its own detailed README with specific setup instructions:

### 🏛️ Available Implementations

#### ✅ [Monolithic Architecture](src/MonolithArchitecture/README.md)

**Status**: 🟢 **Production Ready**

- Complete e-commerce product management system
- 10 business modules with full CRUD operations
- 137/137 tests passing
- Docker support included
- **[📖 View Setup Guide →](src/MonolithArchitecture/README.md)**

#### 🔮 [Microservices Architecture](src/MicroservicesArchitecture/README.md)

**Status**: 🟡 **Coming Soon**

- Distributed services with API Gateway
- Service-to-service communication (gRPC/HTTP)
- Container orchestration with Kubernetes
- **[📖 View Planning →](src/MicroservicesArchitecture/README.md)**

#### 🔮 [Event-Driven Architecture](src/EventDrivenArchitecture/README.md)

**Status**: 🟡 **Coming Soon**

- Event Sourcing implementation
- Message-driven communication
- Saga pattern for distributed transactions
- **[📖 View Planning →](src/EventDrivenArchitecture/README.md)**

#### 🔮 [Serverless Architecture](src/ServerlessArchitecture/README.md)

**Status**: 🟡 **Coming Soon**

- Azure Functions implementation
- Event-driven triggers
- Cloud-native approach
- **[📖 View Planning →](src/ServerlessArchitecture/README.md)**

### 🎯 Quick Start

For immediate exploration, start with the **Monolithic Architecture**:

```bash
# Clone the repository
git clone https://github.com/hammond01/CleanArchitecture.git
cd CleanArchitecture

# Navigate to Monolithic implementation
cd src/MonolithArchitecture

# Follow the detailed setup guide
# See: src/MonolithArchitecture/README.md
```

### 📋 General Requirements

All implementations share these common requirements:

- **.NET 8 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control
- **Docker** (optional, for containerized deployments)

### 🎓 Learning Path

**Recommended order for studying the implementations:**

1. **🏛️ Monolithic Architecture** - Master the fundamentals
2. **🔮 Microservices Architecture** - Learn distributed systems
3. **🔮 Event-Driven Architecture** - Understand event-based patterns
4. **🔮 Serverless Architecture** - Explore cloud-native approaches

Each pattern builds upon concepts from the previous ones, creating a comprehensive learning experience.

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

The project has comprehensive test coverage including:

### Unit Tests

- **Domain Tests** - Entity validation, business rules
- **Application Tests** - Use case testing, handlers
- **Infrastructure Tests** - Repository implementations

### Integration Tests

- **API Tests** - End-to-end API testing
- **Database Tests** - Data access testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"

# Run integration tests
dotnet test tests/IntegrationTests/

# Run unit tests
dotnet test tests/UnitTests/
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

---

⭐ **If this project helped you, please give it a star!** ⭐
