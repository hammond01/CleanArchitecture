# Architecture

## Overview

This repository implements a **Clean Architecture Modular Monolith template** for .NET 8.

It is intended to be a **starter kit** for backend-first application development, with:

- explicit module boundaries
- reusable building blocks
- production-minded API defaults
- real sample modules
- a migration path toward a future UI layer

This project is intentionally **not** a microservices system and is also **not** positioned as a full business product.

The current repository demonstrates architecture and extension patterns through three sample modules:

- **Identity**
- **Catalog**
- **Auditing**

---

## Architectural Style

### Modular Monolith

The application is deployed as a **single runtime**, but internally structured into **isolated business modules**.

Each module represents a **business capability**, not a technical concern.

Key characteristics:

- one deployable unit
- one operational surface
- explicit internal boundaries
- lower ops complexity than microservices
- easier local development and debugging

This approach is meant to keep complexity controlled while still allowing the system to **evolve later if extraction becomes justified**.

### Clean Architecture

Each module follows Clean Architecture principles with inward dependency flow.

Dependency direction:

- **Domain** → business rules and invariants
- **Application** → use cases, commands, queries, orchestration
- **Infrastructure** → technical implementations
- **API** → HTTP entry points and composition wiring

The Domain layer stays isolated from frameworks, persistence, and delivery concerns.

---

## Actual Repository Structure

```text
src/
├── BuildingBlocks/
├── CleanArchitecture.Api/
├── DbMigrator/
└── Modules/
    ├── Auditing/
    ├── Catalog/
    └── Identity/

tests/
├── CleanArchitecture.IntegrationTests/
└── CleanArchitecture.UnitTests/
```

### BuildingBlocks

`BuildingBlocks/` contains shared technical abstractions and reusable infrastructure pieces.

This is where cross-cutting concerns live, such as:

- dispatcher abstractions/implementation
- shared domain/application contracts
- common API/controller infrastructure
- security and auditing abstractions

### API Host

`src/CleanArchitecture.Api/` is the composition root.

It is responsible for:

- module registration
- middleware pipeline setup
- health checks
- security defaults
- CORS setup
- rate limiting
- logging setup

### DbMigrator

`src/DbMigrator/` provides a dedicated migration entry point so schema updates can be run separately from the API host.

### Modules

Each module is structured with separate layers.

Current modules:

- `Identity`
- `Catalog`
- `Auditing`

---

## Module Design

### What Is a Module

A module represents a **business capability**.

In the current repository, modules are also used as **reference implementations** for how new capabilities should be added to the template.

Each module should:

- own its business rules
- own its application workflows
- own its persistence model
- expose its own API endpoints
- remain understandable in isolation

### Internal Layers per Module

#### Domain Layer

Contains:

- entities
- value objects
- domain events
- invariants
- business rules
- repository contracts

Rules:

- no infrastructure concerns
- no HTTP concerns
- no framework-driven business logic

#### Application Layer

Contains:

- commands and queries
- handlers
- validators
- DTOs
- orchestration logic

Rules:

- coordinates use cases
- calls domain behavior
- should not contain persistence implementation details

#### Infrastructure Layer

Contains:

- EF Core persistence
- repository implementations
- external/service integrations
- auth/persistence related technical code

Rules:

- implements interfaces/contracts defined elsewhere
- should not become a home for business rules

#### API Layer

Contains:

- controllers
- request/response exposure
- authorization attributes
- thin delivery logic over the application layer

Rules:

- no business logic
- keep controllers thin
- delegate into application handlers via dispatcher

---

## Current Module Responsibilities

### Identity

The Identity module currently demonstrates:

- user registration
- login
- refresh token flow
- logout
- email confirmation
- resend confirmation
- password reset request
- password reset execution

This module acts as the template's primary reference for authentication-related behavior.

### Catalog

The Catalog module currently demonstrates:

- categories CRUD
- products CRUD
- paginated and filtered reads
- CSV export
- command/query separation via dispatcher

This is the main business sample module in the repository.

### Auditing

The Auditing module currently demonstrates:

- audit log persistence
- audit querying
- background audit outbox processing

This module exists primarily to show how a cross-cutting capability can still live as a bounded module rather than a loose shared concern.

---

## Module Communication Rules

Modules should **not** reference each other's internals casually.

Preferred communication mechanisms:

- explicit contracts
- shared abstractions in BuildingBlocks when truly cross-cutting
- domain/application events where appropriate
- DTO-based interaction boundaries

This keeps the template aligned with its core goals:

- loose coupling
- clean ownership
- safe refactoring
- future extractability if needed

---

## Persistence Strategy

The repository is organized so modules can maintain clear persistence boundaries.

Current architecture intent:

- modules own their persistence concerns
- schemas and contexts should reflect module boundaries
- cross-module coupling through persistence should be minimized

This helps avoid:

- hidden schema coupling
- accidental shared data ownership
- unsafe changes across modules

---

## Operational Baseline in the Current API Host

The current API host already includes several production-minded concerns:

- Serilog logging to console and rolling files
- forwarded headers support
- CORS policy configuration
- HSTS outside development
- custom security headers middleware
- exception handling middleware
- health checks at `/health/live` and `/health/ready`
- auth endpoint rate limiting
- request auditing middleware

This is an important part of the template's value: it is not only structured well, it also starts with a more realistic API baseline than a toy sample.

---

## UI Direction

A dedicated UI layer is planned, but it is **not part of the current repository yet**.

The intended sequence is:

1. stabilize the backend/API template
2. tighten tests and documentation
3. define the UI technology and integration strategy
4. add a UI layer that consumes the API cleanly

This keeps the repository focused on a strong backend/template core before committing to a frontend stack.

---

## Design Principles

- business boundaries over framework convenience
- clarity over pattern theater
- reusable starter-kit structure over domain sprawl
- honest documentation over inflated claims
- evolutionary architecture over premature distribution

---

## Anti-Patterns to Avoid

- shared business logic folders with unclear ownership
- cross-module persistence shortcuts
- infrastructure concerns leaking into domain code
- controllers containing business logic
- adding modules just to increase feature count
- documenting aspirational architecture as if it already exists

---

## Final Note

This architecture is meant to be **pragmatic, extensible, and teachable**.

The goal is not to showcase every enterprise buzzword. The goal is to provide a backend foundation that people can actually clone, understand, and extend.
