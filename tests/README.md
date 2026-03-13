# Tests

This directory contains the automated tests for the Clean Architecture modular monolith template.

## Projects

- `CleanArchitecture.UnitTests`
- `CleanArchitecture.IntegrationTests`

---

## Current Status (latest baseline)

- **Unit tests:** 36 total, **36 passed / 0 failed**
- **Integration tests:** 40 total, **40 passed / 0 failed**

The baseline is currently **fully green**.

---

## Coverage Focus

### Unit tests

Primary focus:

- dispatcher behavior
- catalog command/query handlers
- identity command handlers
- application/domain logic in isolation

### Integration tests

Primary focus:

- identity/authentication flows
- catalog flows (read + write + export)
- auditing API flows
- gateway hardening/security behavior
- end-to-end host wiring through test server

---

## Running tests

Run all tests:

```bash
dotnet test ModularMonolith.sln
```

Run unit tests:

```bash
dotnet test tests/CleanArchitecture.UnitTests/CleanArchitecture.UnitTests.csproj
```

Run integration tests:

```bash
dotnet test tests/CleanArchitecture.IntegrationTests/CleanArchitecture.IntegrationTests.csproj
```

---

## Maintenance rule

Keep this document synchronized with the actual test run state.
If baseline changes, update counts/status in the same PR.
