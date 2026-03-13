# Tests

This directory contains the automated tests for the Clean Architecture modular monolith template.

The repository currently has **two test projects**:

- `CleanArchitecture.UnitTests`
- `CleanArchitecture.IntegrationTests`

---

## Current Status

Based on the current observed test run:

- **Unit tests:** 36 total, **32 passed / 4 failed**
- **Integration tests:** 38 total, **37 passed / 1 failed**

This means the test suite is **substantial and useful**, but the baseline is **not fully green yet**.

Documentation in this folder should reflect that reality.

---

## Test Projects

### CleanArchitecture.UnitTests

Focus:

- dispatcher behavior
- catalog command/query handlers
- identity command handlers
- application/domain-level logic in isolation

Representative files include:

- `Dispatcher/DispatcherTests.cs`
- `Catalog/CategoryCommandHandlerTests.cs`
- `Catalog/ProductCommandHandlerTests.cs`
- `Catalog/GetCategoriesQueryHandlerTests.cs`
- `Catalog/GetProductsQueryHandlerTests.cs`
- `Identity/IdentityCommandHandlerTests.cs`

### CleanArchitecture.IntegrationTests

Focus:

- API endpoint behavior
- authentication flows
- catalog flows
- gateway hardening/security behavior
- module wiring through the actual host pipeline

Representative files include:

- `Controllers/IdentityIntegrationTests.cs`
- `Controllers/CatalogIntegrationTests.cs`
- `Controllers/AuditingIntegrationTests.cs`
- `Controllers/GatewayHardeningIntegrationTests.cs`

---

## Known Issues

### Unit test failures

The current failing unit tests are concentrated in `DispatcherTests`.

Observed problem from the latest run:

- validator enumeration for some test request types is not registered
- the dispatcher throws `InvalidOperationException`
- the tests expect validation-related behavior instead

In practice, the current failures indicate a mismatch between:

- test DI setup
- dispatcher validation expectations

### Integration test failure

The integration suite is very close to green, but **1 test is still failing**.

This means the repository should not currently claim:

- all integration tests are passing
- production-ready test baseline

until that final failing case is fixed and re-run.

---

## What the Tests Already Prove

Even with the current failures, the test suite already proves a lot of real behavior:

- authentication endpoints work end-to-end
- password reset and email confirmation flows exist
- category and product CRUD paths are implemented
- CSV export is covered
- security middleware and gateway hardening are being exercised
- the project is more than a documentation-only template

---

## Recommended Next Steps

To turn testing into a stronger trust signal, the next steps should be:

1. fix the 4 failing unit tests in `DispatcherTests`
2. identify and fix the remaining failing integration test
3. re-run the full solution test suite
4. update this documentation with a fresh, honest status
5. add CI so the documented status stays trustworthy

---

## Running the Tests

Run the full suite:

```bash
dotnet test ModularMonolith.sln
```

Run unit tests only:

```bash
dotnet test tests/CleanArchitecture.UnitTests/CleanArchitecture.UnitTests.csproj
```

Run integration tests only:

```bash
dotnet test tests/CleanArchitecture.IntegrationTests/CleanArchitecture.IntegrationTests.csproj
```

---

## Documentation Rule

This folder should describe the **actual** state of the test suite, not the aspirational state.

If tests fail, the docs should say so clearly.
