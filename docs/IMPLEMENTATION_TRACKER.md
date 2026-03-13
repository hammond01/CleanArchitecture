# Implementation Tracker

Last updated: 2026-03-10

## Phase 1: Identity Authentication and Configuration
- [x] Bind `EmailSettings` and security settings from configuration instead of using hard-coded defaults
- [x] Add JWT settings and runtime authentication wiring in the API gateway
- [x] Implement real access token generation for login
- [x] Implement refresh token issuance and rotation
- [x] Return actual `UserId` from registration
- [x] Return real token payloads from login and refresh flows
- [x] Tighten logout flow to use actual authenticated identity semantics
- [x] Update affected unit and integration tests

## Phase 2: Catalog Safety and Consistency
- [x] Prevent product updates from overwriting stock/order values when fields are omitted
- [x] Prevent category deletion when products still exist
- [x] Align Catalog validation rules with domain and database constraints
- [x] Update affected tests

## Phase 3: Dispatcher and Event Registration Hardening
- [x] Prevent duplicate domain event handler registration across repeated host builds
- [x] Review domain event plumbing for current behavior and keep runtime behavior explicit
- [x] Update affected tests

## Phase 3.5: Auditing Activation
- [x] Add request-level audit persistence for mutating HTTP requests
- [x] Cover the audit write path in integration testing

## Phase 4: Verification
- [x] Run focused tests for changed areas
- [x] Run solution-level verification where environment allows
- [x] Document any remaining known gaps

## Phase 5: Production Hardening
- [x] Add practical authorization to protected endpoints
- [x] Tighten JWT/security configuration for production use
- [x] Upgrade auditing to capture entity-level changes
- [x] Implement automatic domain event collection and dispatch after persistence
- [x] Update affected tests and docs

## Phase 6: Gateway Hardening
- [x] Add health endpoints for liveness and readiness
- [x] Replace permissive CORS defaults with configuration-driven origins
- [x] Add reverse-proxy aware forwarded-header handling and HSTS
- [x] Add rate limiting to authentication endpoints
- [x] Update app configuration and affected integration tests

## Phase 7: Transactional Audit Outbox
- [x] Persist entity-change audit payloads into module-local outbox tables within the business transaction
- [x] Add outbox processing to project audit entries into the auditing module
- [x] Add background delivery and best-effort request-end flush
- [x] Generate migrations for the new outbox schema and audit action length change
- [x] Stabilize integration test email extraction for concurrent execution

### Remaining Known Gaps
- Domain events are now collected and dispatched automatically after persistence, but there are still no real business event handlers wired in the modules.
- Requests that do not produce entity changes still use request-end fallback auditing, so those specific audit entries are not yet transactional/outbox-backed.
- Forwarded header trust is now explicit via `ApiSecurity:KnownProxies`, but real production proxy IPs still need to be set per environment before deployment.
- CLI verification in this sandbox requires local overrides for `APPDATA`, `DOTNET_CLI_HOME`, and `NuGetAudit=false`; a normal developer or CI environment should not need those workarounds.
