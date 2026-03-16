# Adding a New Module

This guide describes the **recommended path** for adding a new business module to the template.

The goal is consistency, not ceremony.

---

## 1) Create module structure

Create a new folder under `src/ModularMonolith/Modules/<ModuleName>/` with these projects:

- `<ModuleName>.Domain`
- `<ModuleName>.Application`
- `<ModuleName>.Infrastructure`
- `<ModuleName>.Api`

Keep naming and folder structure consistent with existing modules (`Catalog`, `Identity`, `Auditing`).

---

## 2) Respect layer boundaries

### Domain

Contains:

- entities / value objects
- domain events
- business rules
- contracts needed by domain logic

Must **not** reference Infrastructure or API.

### Application

Contains:

- commands / queries
- handlers
- validators
- DTOs / use-case orchestration

May reference Domain.

### Infrastructure

Contains:

- EF Core DbContext + mappings
- repository implementations
- technical integrations

Implements contracts from Domain/Application.

### API

Contains:

- controllers / request endpoints
- thin HTTP mapping to application handlers
- module registration extension

No business logic in controllers.

---

## 3) Wire DI registration

Follow the same pattern used by existing modules:

1. add module registration extension in `<ModuleName>.Api/Extensions`
2. register Application + Infrastructure dependencies there
3. call `builder.Services.Add<ModuleName>Module(...)` from `src/ModularMonolith/CleanArchitecture.Api/Program.cs`

---

## 4) Persistence and migrations

- create a module DbContext in Infrastructure
- keep schema ownership clear per module
- avoid cross-module persistence coupling
- add EF Core migrations under module Infrastructure migration folder
- ensure `DbMigrator` can apply new migrations

---

## 5) API conventions

Use:

- route prefix: `api/v1/[controller]`
- thin controllers
- command/query dispatch through `IDispatcher`
- auth/rate-limit attributes where needed

Maintain response consistency with existing modules.

---

## 6) Testing checklist

Add at least:

- unit tests for core handlers/validation
- integration tests for critical endpoints

Recommended minimum for a new module:

- create flow
- update flow
- read flow
- delete flow (or equivalent lifecycle)
- one failure/validation path

---

## 7) Documentation checklist

When adding a module, update:

- `README.md` (Current Modules + API surface)
- `ARCHITECTURE.md` (module responsibilities if needed)
- `tests/` docs if testing scope changes significantly

Docs must reflect reality.

---

## 8) Definition of Done (module)

A module is considered integrated when:

- project structure is complete
- DI registration works from API host
- migrations apply cleanly
- endpoints are reachable and tested
- docs are updated and accurate
