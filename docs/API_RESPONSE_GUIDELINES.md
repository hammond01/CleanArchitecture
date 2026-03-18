# API Response Consistency Guidelines

This guide defines response-shape rules for **new** API endpoints without forcing broad breaking changes on existing endpoints.

## 1) Success responses

- Use explicit success contracts per endpoint with `ProducesResponseType`.
- Return a typed JSON payload for resource endpoints (`200`, `201`), unless an existing controller contract already uses a legacy envelope.
- For file endpoints, declare the file response and content type explicitly.

## 2) Error responses

- Use `ProblemDetails` (`application/problem+json`) for middleware-driven failures and throttling responses.
- Keep legacy compatibility extensions in `ProblemDetails`:
  - `success` = `false`
  - `statusCode`
  - `error`
  - `traceId`
  - optional `errors` for validation details
- If an existing endpoint already exposes legacy `ApiResponse` errors, do not silently switch it in a patch release.

## 3) Swagger / OpenAPI metadata

- For each endpoint, document:
  - primary success schema (`200`/`201`)
  - known non-success statuses (`400`, `401`, `403`, `404`, `409`, `429`, `500` as applicable)
- Do not leave non-success responses undocumented.
- Keep schema and runtime behavior aligned; if runtime cannot change yet, document the current shape and migration path.

## 4) Breaking-change policy for clients

- **Breaking**:
  - replacing a success body shape
  - switching an established error envelope (`ApiResponse` vs `ProblemDetails`)
  - removing existing fields
- **Non-breaking**:
  - adding optional fields/extensions
  - adding missing OpenAPI metadata for existing runtime behavior
  - adding new documented status codes when runtime already emits them

## 5) Required quality checks

- Update integration tests when response shape changes.
- Update `docs/OPENAPI_CONTRACT_REVIEW_PHASE2.md` and changelog/release notes in the same PR.
- Ensure `API Quality Gates` workflow is green.
