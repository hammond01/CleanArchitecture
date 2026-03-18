# Client Migration Notes (Phase 2 Stabilization)

Date: 2026-03-18

## Summary

This stabilization pass focuses on CI/process guardrails and contract clarity.  
No intentional breaking response-shape change was introduced.

## What changed for API consumers

- `429 Too Many Requests` responses now include additional compatibility fields in `ProblemDetails` payloads:
  - `success: false`
  - `statusCode`
  - `error`
  - `traceId`
- Existing response status codes and success-body shapes are unchanged.
- Existing model-validation error envelope behavior is unchanged.

## Breaking vs non-breaking impact

- **Non-breaking**:
  - additive `ProblemDetails` extension fields for `429`
  - OpenAPI/response-guideline documentation updates
  - CI quality-gate additions
- **No breaking changes** shipped in this phase.

## Client action required

- Optional: clients that already parse `ProblemDetails` can start using `traceId`/`statusCode` on `429`.
- No mandatory payload parser change is required for this phase.
