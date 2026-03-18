# OpenAPI / Swagger Contract Review (Phase 2 Stabilization)

Date: 2026-03-18

## Scope

- Source-level review of API response contracts and Swagger metadata.
- Focus on error schema behavior, especially `ProblemDetails`.

## Current contract observations

1. Exception middleware emits `application/problem+json` using `ProblemDetails`.
2. Exception middleware includes legacy compatibility extensions:
   - `success`, `statusCode`, `error`, `traceId`
   - optional `errors` for `FluentValidation.ValidationException`
3. Rate limiter rejections now use the same compatibility extension set through `ApiProblemDetailsFactory`.
4. Invalid model-state responses currently return legacy `ApiResponse` error objects (`{ success, statusCode, message, error }`), not `ProblemDetails`.
5. Controller Swagger annotations are uneven:
   - Identity controller has richer `ProducesResponseType` coverage.
   - Catalog/Auditing controllers rely mostly on implicit response inference.

## Compatibility impact assessment

### Non-breaking changes (safe in patch/minor releases)

- Adding/standardizing optional `ProblemDetails` extensions.
- Adding missing OpenAPI response metadata for already-existing runtime statuses.
- Adding CI checks and documentation guardrails.

### Breaking changes (require versioned rollout/migration)

- Switching existing endpoint error shape from legacy `ApiResponse` to pure `ProblemDetails`.
- Removing legacy extension fields from current `ProblemDetails` payloads.
- Replacing existing success-body envelopes for established endpoints.

## Phase 2 decisions

- Keep runtime behavior stable for existing endpoint success/error shapes.
- Standardize middleware and throttling `ProblemDetails` extensions via shared factory.
- Add explicit process guardrails:
  - CI quality-gate workflow (`api-build`, `api-unit-tests`, `api-integration-tests`, `dispatcher-dynamic-guard`)
  - Response consistency guide for new endpoints.

## Follow-up (planned, versioned)

- Incrementally add explicit `ProducesResponseType` coverage to Catalog/Auditing controllers.
- Define a versioned client migration path if moving all endpoint errors to a single `ProblemDetails` contract.
