# ARCHITECTURE

## Overview

This repository implements an **Enterprise Backend System** using a  
**Modular Monolith architecture combined with Clean Architecture**.

The system is designed to support **complex business workflows** such as:
- ERP
- OMS
- Payment / Financial systems

while keeping:
- Deployment simple
- Operations predictable
- Long-term maintenance sustainable

This project is intentionally **NOT** a microservices system.

---

## Architectural Style

### Modular Monolith

The application is deployed as a **single runtime**, but internally structured into
**isolated business modules**.

Each module represents a **business capability**, not a technical concern.

Key characteristics:
- One deployable unit
- One operational surface (logging, monitoring, alerting)
- Explicit internal boundaries
- Strong ownership per module

This approach minimizes operational complexity while still allowing
the system to **evolve toward microservices when justified**.

---

### Clean Architecture

Each module follows Clean Architecture principles with strict dependency rules.

Dependency direction:

- **Domain**: Business rules and invariants
- **Application**: Use case orchestration
- **Infrastructure**: Technical implementation details

The Domain layer is fully isolated from frameworks, persistence, and external services.

---

## Project Structure

/src
├── Api
│ └── Application entry point & composition root
│
├── Modules
│ ├── {ModuleName}
│ │ ├── Domain
│ │ ├── Application
│ │ ├── Infrastructure
│ │ └── Api
│
├── BuildingBlocks
│ └── Shared technical abstractions (NO business logic)
│
├── .copilot-context.md
└── ARCHITECTURE.md

---

## Module Design

### What Is a Module

A **module** represents a **business capability**, for example:
- Purchasing
- Payment
- Inventory
- Identity

Each module:
- Owns its business rules
- Owns its persistence model
- Can be reasoned about independently
- Is a candidate for future extraction

---

### Internal Layers

#### Domain Layer

Contains:
- Aggregate Roots
- Entities
- Value Objects
- Domain Events
- Business invariants

Rules:
- No framework dependencies
- No persistence concerns
- No HTTP or external service calls

**Example: Aggregate enforcing invariants**

```csharp
public class PurchaseOrder : AggregateRoot
{
    public void Approve(UserId approver)
    {
        if (Status != PurchaseOrderStatus.Pending)
            throw new DomainException("Only pending orders can be approved.");

        Status = PurchaseOrderStatus.Approved;
        AddDomainEvent(new PurchaseOrderApprovedEvent(Id, approver));
    }
}

* Application Layer

Responsibilities:

- Use case orchestration
- Transaction coordination
- Authorization checks
- Calling domain behavior

Rules:
- No infrastructure-specific code
- Business rules remain in Domain
- Depends only on Domain abstractions

Example: Application use case
public class ApprovePurchaseOrderCommandHandler
{
    public async Task Handle(ApprovePurchaseOrderCommand command)
    {
        var order = await _repository.GetByIdAsync(command.OrderId);

        order.Approve(command.ApproverId);

        await _unitOfWork.CommitAsync();
    }
}

* Infrastructure Layer

Contains:
- EF Core / database access
- External API integrations
- Messaging / Outbox pattern
- Repository implementations

Rules:
- Implements interfaces defined in Domain/Application
- Contains no business rules
- Fully replaceable

Example: Repository implementation
public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    public Task<PurchaseOrder?> GetByIdAsync(Guid id)
        => _dbContext.PurchaseOrders.FindAsync(id).AsTask();
}

* Module API Layer

Contains:
- HTTP Controllers
- Request / Response contracts

Rules:
- Thin layer
- No business logic
- Delegates directly to Application layer

** Module Contracts & Communication

- Modules MUST NOT reference each other’s internal layers.

Allowed communication mechanisms

- Domain Events
- Application-level interfaces
- Explicit DTO contracts

Example: Application-level contract
public interface IPaymentService
{
    Task<PaymentResult> CaptureAsync(PaymentRequest request);
}
This ensures:
- Loose coupling
- Clear ownership
- Safe refactoring
- Future extractability

** Persistence Strategy

- Each module owns its database schema
- No shared DbContext across modules
- No cross-module foreign keys

This avoids:

- Hidden coupling
- Accidental schema dependencies
- Unsafe cross-module changes

** Migration Strategy

The system supports incremental architectural evolution.

Typical migration path

- Start with a Modular Monolith
- Strengthen module boundaries
- Introduce domain events for decoupling

- Extract a module into a microservice only when justified

Extraction criteria

- A module may be extracted when:

- Independent scaling is required
- Independent deployment becomes necessary
- Team ownership grows significantly

Until then, modular monolith provides:

- Strong consistency
- Simpler debugging

Lower operational cost

** Design Principles

- Business problems over patterns
- Explicit boundaries over shared abstractions
- Simplicity over premature optimization

Evolutionary architecture over upfront complexity

** Forbidden Anti-Patterns

- Shared business logic folders
- Cross-module DbContext usage
- Infrastructure code inside Domain
- God services or shared repositories
- Pattern-driven over-engineering

** Interview Talking Points

When explaining this architecture:

- Emphasize business boundaries
- Explain trade-offs, not just patterns
- Highlight operational simplicity
- Position microservices as an evolution path

** Final Note

- This architecture prioritizes clarity, discipline, and pragmatism.
- Architecture exists to serve the business,
not to showcase patterns or trends.