# UI Implementation Plan (React + TypeScript + Vite + Tailwind + TanStack Query)

## 1) Objective

Build a production-minded UI layer for the current Modular Monolith API using:
- React
- TypeScript
- Vite
- Tailwind CSS
- TanStack Query

The UI must follow frontend Clean Architecture principles and map directly to current backend modules:
- Identity
- Catalog
- Auditing

---

## 2) Repository Structure (Target)

```text
src/
  ModularMonolith/   # backend
  UI/                # frontend
```

### UI internal structure

```text
src/UI/
  docs/
  public/
  src/
    app/
    shared/
    modules/
      identity/
      catalog/
      auditing/
    widgets/
```

---

## 3) Frontend Clean Architecture

Each feature module uses 4 layers:

- `domain/`
  - business entities/value types
  - business rules (framework-agnostic)
- `application/`
  - use-case orchestration
  - query/mutation hooks (TanStack Query)
  - command/query input contracts
- `infrastructure/`
  - API datasource implementations
  - DTO <-> domain mapper
- `presentation/`
  - pages, forms, view components
  - route-specific UI logic

Shared cross-cutting lives in `src/UI/src/shared`.

---

## 4) Core Technical Decisions

- Routing: React Router
- Server state: TanStack Query
- Form handling: react-hook-form (+ zod for schema validation)
- HTTP: centralized API client with interceptor pipeline
- Auth:
  - JWT access token attached to API requests
  - refresh-token flow through `/api/v1/authentication/refresh-token`
  - protected routes for Catalog/Auditing
- Config:
  - `VITE_API_BASE_URL` required
- Styling:
  - Tailwind + design tokens via CSS variables
  - reusable primitives in `shared/ui`

---

## 5) API Integration Strategy

Backend currently returns mixed success/error shapes, so UI must normalize responses:

- success wrapped: `{ success, data, ... }`
- success unwrapped: raw object/list
- error wrapped: `{ success: false, error, statusCode }`
- rate-limit error: ProblemDetails (429)

Plan:
1. Create `ApiResult<T>` normalization layer in `shared/http`.
2. Convert all responses into a single internal success/error contract.
3. Map transport errors into typed `ApiError`.

---

## 6) Route Map (v1)

- Public:
  - `/login`
  - `/register`
  - `/forgot-password`
  - `/reset-password`
  - `/confirm-email`
  - `/resend-confirmation`
- Protected:
  - `/catalog/categories`
  - `/catalog/products`
  - `/auditing/logs`
- App shell:
  - authenticated layout with nav + content area

---

## 7) Feature Scope (v1)

### Identity
- Login
- Register
- Request password reset
- Reset password
- Confirm email
- Resend confirmation
- Auto refresh token + logout fallback

### Catalog
- Categories:
  - list/filter
  - create/update/delete
- Products:
  - list/filter
  - create/update/delete
  - export CSV

### Auditing
- Audit logs read-only list
- basic pagination (`pageNumber`, `pageSize`)

---

## 8) Query Key and Cache Conventions

- `['identity', ...]`
- `['catalog', 'categories', params]`
- `['catalog', 'products', params]`
- `['auditing', 'logs', params]`

Mutation invalidation:
- category create/update/delete -> invalidate categories (+ products when needed)
- product mutations -> invalidate products
- auth actions -> invalidate relevant identity session queries

---

## 9) Delivery Phases

### Phase 0 — Workspace alignment
- Ensure backend under `src/ModularMonolith`
- Create frontend root `src/UI`
- Align docs/scripts references to new structure

### Phase 1 — UI bootstrap
- Init Vite React TS project
- Add Tailwind + base tokens
- Add TanStack Query, Router, form/validation stack
- Add lint/typecheck/build scripts

### Phase 2 — Architecture skeleton
- Create app/shared/modules/widgets structure
- Add HTTP client + response normalizer
- Add auth session store + route guard
- Add global error handling/toast/loading boundaries

### Phase 3 — Identity flows
- Implement all auth-related screens and API flows
- Wire refresh-token + protected route behavior

### Phase 4 — Catalog flows
- Implement categories/products pages and CRUD
- Add filtering + CSV export UX

### Phase 5 — Auditing + polish
- Implement audit logs page
- Empty/loading/error states
- Responsive refinement

### Phase 6 — Verification + docs
- Run lint/typecheck/build
- Document setup/run in `src/UI/README.md`
- Add integration notes (CORS, env, known constraints)

---

## 10) Definition of Done (v1)

- UI runs locally against API via configured base URL
- Auth flow works end-to-end with token refresh
- Catalog categories/products CRUD works
- Product CSV export works
- Auditing logs are visible in protected area
- Error handling is consistent across API response variants
- Build and typecheck pass
- Setup docs are complete and reproducible

---

## 11) Risks and Mitigations

- Mixed API response contracts
  - Mitigation: strict normalization adapter in HTTP layer
- Token refresh race conditions
  - Mitigation: single-flight refresh queue + replay request
- Backend path migration impact
  - Mitigation: do Phase 0 first and verify solution references before UI integration
