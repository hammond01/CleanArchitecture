# API Release Checklist (Template Baseline)

Use this checklist before tagging a release or publishing this template state.

---

## 1) Build and test gate

- [ ] `dotnet build ModularMonolith.sln --no-restore` passes
- [ ] `dotnet test ModularMonolith.sln --no-restore` passes
- [ ] integration tests pass on a clean environment
- [ ] `API Quality Gates` workflow is green (`dispatcher-dynamic-guard`, `api-build`, `api-unit-tests`, `api-integration-tests`)
- [ ] branch protection requires the 4 API Quality Gates checks on the target branch

---

## 2) API contract consistency

- [ ] each endpoint declares success + non-success `ProducesResponseType` contracts
- [ ] response shapes follow [`docs/API_RESPONSE_GUIDELINES.md`](API_RESPONSE_GUIDELINES.md)
- [ ] validation errors return the currently documented contract (no silent shape changes)
- [ ] exception middleware returns `ProblemDetails` with compatibility extensions (`success`, `statusCode`, `error`, `traceId`)
- [ ] unauthorized/forbidden behavior is expected and tested
- [ ] rate-limit behavior is expected and tested

---

## 3) Security baseline

- [ ] `ApiSecurity:AllowedOrigins` is configured for non-dev environments
- [ ] forwarded headers configuration reviewed for deployment topology
- [ ] security headers middleware enabled
- [ ] auth endpoint rate limiting enabled
- [ ] HTTPS redirection enabled
- [ ] HSTS enabled outside development

---

## 4) Identity readiness

- [ ] register/login/refresh/logout flow verified
- [ ] confirm-email/resend-confirmation flow verified
- [ ] request/reset-password flow verified
- [ ] email provider settings validated for target environment
- [ ] JWT/security settings reviewed

---

## 5) Data and migration readiness

- [ ] connection strings validated
- [ ] `DbMigrator` runs successfully on target DB
- [ ] module migrations are up to date and committed
- [ ] health checks report ready state after migration

---

## 6) Operational readiness

- [ ] logs available (console + rolling files)
- [ ] `/health/live` and `/health/ready` verified
- [ ] important startup errors are actionable
- [ ] production appsettings reviewed (no dev placeholders)

---

## 7) Documentation sync

- [ ] README reflects actual test baseline and module/API status
- [ ] architecture doc reflects current module responsibilities
- [ ] test docs reflect current passing/failing state
- [ ] extension docs are up to date (`ADDING_A_MODULE.md`)
- [ ] UI plan doc reflects current direction (`UI_INTEGRATION_PLAN.md`)

---

## 8) Release metadata

- [ ] changelog/release notes updated
- [ ] version/tag naming confirmed
- [ ] branch and PR history is clean
- [ ] no temporary branches left behind

---

## Definition of release-ready (API baseline)

The API baseline is release-ready when:

- build + tests are green,
- security + migration checks pass,
- docs match implementation,
- and the template can be cloned and run with predictable results.
