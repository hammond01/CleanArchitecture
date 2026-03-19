# Clean Architecture Modular Monolith Template for .NET 8

> A production-minded Clean Architecture starter kit for .NET 8 with modular monolith boundaries, authentication, catalog and auditing sample modules, database migration tooling, and a clear path toward future UI integration.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2B%20Modular%20Monolith-brightgreen.svg)](#architecture-overview)
[![Status](https://img.shields.io/badge/status-template%20starter%20kit-blue.svg)](#roadmap)
[![API CI/CD](https://github.com/hammond01/CleanArchitecture/actions/workflows/api-ci-cd.yml/badge.svg)](https://github.com/hammond01/CleanArchitecture/actions/workflows/api-ci-cd.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

## Table of Contents

- [What This Project Is](#what-this-project-is)
- [Current State](#current-state)
- [Vision](#vision)
- [What's Included Today](#whats-included-today)
- [Why Modular Monolith](#why-modular-monolith)
- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Current Modules](#current-modules)
- [API Surface](#api-surface)
- [Planned UI Layer](#planned-ui-layer)
- [Quick Start](#quick-start)
- [Testing](#testing)
- [Extending the Template](#extending-the-template)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## What This Project Is

This repository is a **Clean Architecture modular monolith template / starter kit** for building maintainable .NET 8 applications.

It is aimed at:

- developers who want a strong backend foundation for new projects
- teams that prefer a modular monolith before taking on distributed-system complexity
- engineers learning Clean Architecture through a non-trivial codebase
- portfolio/demo use cases where structure and trade-offs matter more than feature count

This project focuses on:

- clear module boundaries
- reusable building blocks
- production-minded API defaults
- sample modules that demonstrate how to structure business capabilities
- a foundation that can later support a dedicated UI layer

### What this project is not

This project is **not**:

- a full ERP or e-commerce application
- a feature-maximized business product
- a microservices system
- a repo trying to model every possible domain up front

The goal is to provide a **trustworthy, extensible starting point**, not to ship every possible module.

---

## Current State

As of the current codebase, this template includes:

- **.NET 8** solution using central package management
- **Modular Monolith + Clean Architecture** structure
- **3 sample modules**:
  - Identity
  - Catalog
  - Auditing
- **Shared BuildingBlocks** for cross-cutting concerns
- **API host** in `src/ModularMonolith/CleanArchitecture.Api`
- **DbMigrator** in `src/ModularMonolith/DbMigrator`
- **4 API controllers / 20 HTTP endpoints** across the sample modules
- **unit and integration test projects**
- **security middleware baseline** including:
  - CORS policy
  - forwarded headers
  - HSTS outside development
  - custom security headers middleware
  - auth endpoint rate limiting
  - health checks (`/health/live`, `/health/ready`)

### Important honesty note

The repository already has a meaningful testing setup, but it is **green end-to-end in the current baseline**:

- `CleanArchitecture.UnitTests`: **36 passed / 0 failed**
- `CleanArchitecture.IntegrationTests`: **44 passed / 0 failed**

So the current status is best described as:

> **usable template foundation with real tests, but now a fully green baseline**.

---

## Vision

The long-term vision of this project is to serve as a **full application foundation** with:

- a clean and extensible backend core
- modular business boundaries
- a stable API layer
- a future UI layer integrated on top of the API
- a development workflow suitable for real-world project bootstrapping

### Current focus

The current focus is on strengthening the backend/API starter kit:

- architecture clarity
- module boundaries
- authentication foundation
- auditing
- migrations
- testing reliability
- production baseline concerns

### Next-stage focus

Once the API foundation is stable enough, the next step is to evaluate and implement the **UI/frontend layer**, including:

- frontend technology selection
- API integration strategy
- authentication flow integration
- frontend project structure
- developer experience across backend + UI

---

## What's Included Today

### Core Architecture

- Clean Architecture layering
- Modular Monolith structure
- DDD-inspired module boundaries
- Custom dispatcher for command/query execution with DI-resolved domain-event handlers
- Shared building blocks for common concerns
- Single composition-root registration of `AddApplicationServices()` in API host

### Platform Features

- JWT-based authentication flows
- email confirmation and password reset flows
- auditing support with background processing
- database migration tooling
- health checks
- Serilog logging to console and rolling files
- security middleware baseline
- rate limiting for auth endpoints
- central package management via `Directory.Packages.props`

### Sample Modules

- **Identity** — authentication and user lifecycle flows
- **Catalog** — sample business CRUD/query module with categories and products
- **Auditing** — audit trail and cross-cutting observability support

---

## Why Modular Monolith

This template uses a **modular monolith** because it offers a strong balance between:

- architectural discipline
- lower operational complexity
- easier debugging
- simpler deployment
- clear separation of business capabilities
- future extraction options if scaling needs change later

Instead of starting with microservices too early, this template favors:

- explicit module boundaries
- clear ownership
- pragmatic evolution
- maintainable complexity

---

## Architecture Overview

This project follows **Clean Architecture** inside a **Modular Monolith**.

### Layers

#### Domain

Contains:

- entities
- value objects
- domain events
- business rules and invariants
- repository contracts

#### Application

Contains:

- commands and queries
- handlers
- validators
- DTOs
- orchestration logic

#### Infrastructure

Contains:

- EF Core persistence
- repository implementations
- technical services
- auth and persistence integrations
- migration-related setup

#### API

Contains:

- controllers
- middleware configuration
- authentication/authorization entry points
- health endpoints
- composition root wiring

#### Shared Building Blocks

Contains reusable cross-cutting components such as:

- base abstractions
- dispatcher infrastructure
- common API utilities
- shared domain/application helpers
- security and auditing abstractions

### Architectural principles

- business modules remain isolated by default
- dependencies point inward
- infrastructure does not leak into domain logic
- modules communicate through explicit contracts/events rather than shared internals
- the architecture stays pragmatic rather than pattern-driven for its own sake

---

## Project Structure

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

### Notes

- `BuildingBlocks/` contains reusable cross-cutting abstractions and infrastructure
- `Modules/` contains business capabilities organized by module
- `CleanArchitecture.Api/` is the API host and composition root
- `DbMigrator/` handles database migration orchestration
- `tests/` contains unit and integration test coverage for current behavior
- a future UI project may be introduced once the frontend direction is finalized

---

## Current Modules

### Identity

Purpose:

- provide authentication and user account workflows

Currently demonstrates:

- register
- login
- refresh token
- logout
- email confirmation
- resend confirmation
- password reset request
- password reset execution

### Catalog

Purpose:

- serve as the main sample business module for CRUD/query workflows

Currently demonstrates:

- categories CRUD
- products CRUD
- paginated queries
- filtered product/category reads
- CSV export
- module-level validation and command/query handling

### Auditing

Purpose:

- provide audit logging and cross-cutting observability support

Currently demonstrates:

- audit log persistence
- background audit outbox processing
- authenticated audit log querying

---

## API Surface

The current API is organized into **4 controllers / 20 endpoints**.

### Identity API

`/api/v1/authentication`

- `POST /login`
- `POST /refresh-token`
- `POST /logout`
- `POST /register`
- `POST /confirm-email`
- `POST /resend-confirmation`
- `POST /request-password-reset`
- `POST /reset-password`

### Catalog API

`/api/v1/categories`

- `GET /`
- `GET /{id}`
- `POST /`
- `PUT /{id}`
- `DELETE /{id}`

`/api/v1/products`

- `GET /`
- `GET /export`
- `GET /{id}`
- `POST /`
- `PUT /{id}`
- `DELETE /{id}`

### Auditing API

`/api/v1/auditlogs`

- `GET /`

### Health Endpoints

- `GET /health/live`
- `GET /health/ready`

---

## Planned UI Layer

This project is being built with a **future UI layer in mind**.

The current implementation focuses on establishing a strong backend/API foundation first. Once that foundation is stable, the project can move into UI planning and implementation.

### UI goals

The future UI layer should aim to provide:

- a maintainable frontend architecture
- clean API integration
- authentication flow support
- a good developer experience
- alignment with the modular philosophy of the backend

### UI decisions intentionally deferred

The following decisions are intentionally deferred until the API baseline is stronger:

- frontend framework choice
- SPA vs SSR strategy
- frontend project structure
- API client generation strategy
- authentication/session handling approach
- monorepo vs separate frontend repository strategy

This keeps the current phase focused on making the backend/template baseline trustworthy first.

---

## Quick Start

### Prerequisites

- .NET 8 SDK
- PostgreSQL 16+ (or Docker)
- Git

### 1. Clone the repository

```bash
git clone <your-repository-url>
cd <your-project-folder>
```

### 2. Configure the database connection

Update the appropriate configuration file, for example:

- `src/ModularMonolith/CleanArchitecture.Api/appsettings.json`
- `src/ModularMonolith/DbMigrator/appsettings.json`

Set:

- `ConnectionStrings:DefaultConnection`
- format: `Host=<host>;Port=5432;Database=CleanArchitecture;Username=<user>;Password=<password>`

### 3. Run database migrations

```bash
dotnet run --project src/ModularMonolith/DbMigrator
```

Or run everything with Docker Compose (PostgreSQL + DbMigrator + API):

```bash
docker compose up --build
```

### 4. Run the API

```bash
dotnet run --project src/ModularMonolith/CleanArchitecture.Api
```

### 5. Open Swagger (Development)

Swagger is enabled in Development:

```text
https://localhost:<port>/swagger
```

### 6. Run tests

```bash
dotnet test ModularMonolith.sln
```

> Current baseline is green in repository test docs. See [Testing](#testing).

### PostgreSQL Migration Notes

- Baseline migrations are now PostgreSQL-specific for Identity, Catalog, and Auditing modules.
- The previous SQL Server migration files were replaced on branch `feat/postgres-migration`.
- Migration history is isolated per schema via `__ef_migrations_history` in `identity`, `catalog`, and `auditing`.
- See [docs/POSTGRES_MIGRATION_NOTES.md](docs/POSTGRES_MIGRATION_NOTES.md) for local/staging run steps.

---

## Testing

This repository includes both unit and integration test projects.

### Current observed status

Based on the current codebase:

- **Unit tests:** 36 total, **36 passed / 0 failed**
- **Integration tests:** 44 total, **44 passed / 0 failed**

### What the tests already cover

- command/query dispatching
- catalog handlers and queries
- identity command handlers
- authentication flows
- catalog API flows
- gateway hardening/security behavior
- health/security middleware behavior through integration tests

---

## Extending the Template

One of the main goals of this project is to make it easier to add new business capabilities without breaking architectural clarity.

This template is strongest when used as a foundation for:

- adding a new module
- extending an existing module
- reusing shared building blocks
- preserving boundaries between domain, application, infrastructure, and API
- later integrating a UI layer on top of the API

Module extension guides:

- `docs/ADDING_A_MODULE.md`
- `docs/UI_INTEGRATION_PLAN.md`
- `docs/API_RELEASE_CHECKLIST.md`
- `docs/API_RESPONSE_GUIDELINES.md`
- `docs/OPENAPI_CONTRACT_REVIEW_PHASE2.md`
- `docs/DEPLOY_DOCKER_APACHE_GITHUB.md`

---

## Roadmap

### v1.1 — Trustworthy API Starter Kit

Focus areas:

- align docs with code
- polish quick start
- improve extension guidance
- stabilize API baseline
- establish CI baseline

### v1.2 — Better Extension & Dev Experience

Focus areas:

- better module scaffolding guidance
- local/dev environment polish
- Docker/dev infra improvements
- stronger testing/reporting workflow
- improved operational baseline

### Future — UI Integration Phase

Focus areas:

- UI technology selection
- frontend architecture
- API-to-UI integration patterns
- authentication integration for UI
- improved full-stack developer experience

---

## Contributing

Contributions are welcome, especially those that improve:

- architectural clarity
- documentation accuracy
- testing reliability
- developer experience
- extension guidance
- production-minded defaults

### Contribution expectations

- keep docs aligned with code
- respect module boundaries
- avoid unnecessary architectural complexity
- prefer clarity over pattern-chasing
- keep the template reusable

---

## License

This project is licensed under the MIT License.

See [LICENSE](LICENSE) for details.
