# Implementation Plan: Member Registration Platform

**Branch**: `001-member-registration` | **Date**: 2026-06-15 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-member-registration/spec.md`

## Summary

Build the OPTODEV Member Registration platform: a responsive React single-page wizard that captures member/employee information, backed by a .NET 10 REST API using Clean Architecture + Vertical Slice + CQRS, persisted in PostgreSQL, with role-based access control, audit logging, health probes, and interactive OpenAPI documentation. The work is delivered in phases: foundation (P0), compliant registration (P1), admin read/manage + frontend wizard (P2), member self-view (P3), and operability/polish (P4).

## Technical Context

**Language/Version**: C# 13 on .NET 10 SDK; TypeScript 5.x on React 18+

**Primary Dependencies**:
- Backend: ASP.NET Core Minimal APIs, EF Core 10, Npgsql, FluentValidation, Scalar.AspNetCore, MediatR alternative: lightweight custom `ISender`/`IPipelineBehavior` (see research.md)
- Frontend: React 18+, React Hook Form, Zod, Tailwind CSS, Vite
- Testing: xUnit, FluentAssertions, Testcontainers.PostgreSQL, WebApplicationFactory, React Testing Library

**Storage**: PostgreSQL 15+

**Testing**: Unit tests for Domain/Application; integration tests for full HTTP→DB path; architecture tests with NetArchTest

**Target Platform**: Cross-platform ASP.NET Core server; responsive web client (mobile-first)

**Project Type**: Web service + SPA

**Performance Goals** (release-gate acceptance criteria):
- p95 `POST /api/members` ≤ 400 ms under nominal load
- p95 `GET /api/members/{id}` and `GET /api/members` ≤ 200 ms
- These targets replace the less specific SC-004 "under 3 seconds" wording.
- Measurement: k6 or `dotnet-counters` against a local Testcontainers PostgreSQL instance with a warm API and 50 concurrent virtual users (nominal load profile).
- Stateless API; horizontally scalable behind a load balancer.

**Constraints**:
- Philippine Data Privacy Act of 2012 (RA 10173) compliance for Sensitive Personal Information
- No stack traces or internal details leaked outside development
- No secrets stored in source or config files
- Inward-only Clean Architecture dependency rule enforced in CI
- RBAC: Member role sees only own record; HR/Admin sees list and all details
- Optimistic concurrency: `PUT /api/members/{id}` uses EF Core concurrency token (`RowVersion`); `409 Conflict` on mismatch

**Scale/Scope**: Internal OPTODEV employee/member registration; initial target hundreds to low thousands of registrants; expected concurrent users <50; list endpoints paged (default 20, max 100). Horizontal scaling is supported by the stateless API design.

**Non-functional Constraints**:
- Target availability of 99.9% is inherited from the hosting platform and is not a v1 implementation requirement.
- Responsive breakpoints are left to the frontend implementation; the UI must meet WCAG 2.1 AA, mobile-first layout, and 44×44 px touch targets.
- PostgreSQL backups and disaster recovery are infrastructure concerns and are out of scope for v1.
- PostgreSQL 15+ is required. The implementation uses only standard SQL/EF Core features available in PostgreSQL 12+; environments running older versions should upgrade before deployment.

**Decisions resolved in research.md**:
- CQRS mediator approach (custom lightweight vs. MediatR)
- Authentication/authorization integration pattern for external identity mechanism
- Sensitive field protection at rest (AES-256-GCM)
- Frontend build tooling (Vite)
- Optimistic concurrency: EF Core RowVersion concurrency token with 409 Conflict response

## Phase Mapping

| Plan Phase | Tasks.md Phase(s) | Scope |
|---|---|---|
| P0 — Foundation | Phase 1 (Setup) + Phase 2 (Foundational) | Solution skeleton, DI, DbContext, health probes, Scalar, CI, architecture tests |
| P1 — Compliant Registration | Phase 3 (US1) + Phase 5 (US3) | Register member end-to-end, encryption, consent, PII redaction, access logging |
| P2 — Admin Read/Manage + Frontend | Phase 6 (US4) + Phase 4 (US2) | Get/list/update endpoints, React wizard, address toggle, validation, async submit |
| P3 — Member Self-View | Phase 7 (US5) | RBAC enforcement on read endpoints |
| P4 — Operability + Polish | Phase 8 (US6) + Phase 9 (Polish) | Correlation ID, structured logging, final quality gates |

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design. Re-verified 2026-06-15 after `/speckit.clarify` — all gates still PASS.*

| Principle | Gate | Status |
|---|---|---|
| I. Architectural Integrity | Solution skeleton uses concentric Clean Architecture layers; inward-only dependencies verified by NetArchTest in CI | PASS |
| II. Feature-First Organization | Each use case implemented as a self-contained vertical slice with command/query, handler, validator, response, endpoint co-located | PASS |
| III. Explicit, Consistent Outcomes | Handlers return `Result<T>`; API uses uniform envelope; global exception handler maps to status codes | PASS |
| IV. Validation at the Trust Boundary | FluentValidation pipeline runs before every command handler; client mirrors server rules | PASS |
| V. Security & Privacy by Default | SPI encrypted at rest; no clear-text logs; RBAC; access logging; secrets from secrets manager; explicit consent gate | PASS |
| VI. Observability & Operability | Structured request logging with correlation ID and PII redaction; audit interceptor; `/health/live` and `/health/ready` | PASS |
| VII. Quality via Tests | Domain/Application unit-testable without DB; integration tests with real Postgres; architecture tests | PASS |
| VIII. Accessible, Low-Friction UX | 5-step wizard, inline validation, async submit, WCAG 2.1 AA, mobile-first, dark mode | PASS |
| IX. Simplicity & YAGNI | No speculative abstractions; custom lightweight mediator chosen to avoid unnecessary licensing/complexity | PASS |
| X. Contract Documentation | Native OpenAPI generation + Scalar UI at `/scalar/v1` in non-production | PASS |

## Project Structure

### Documentation (this feature)

```text
specs/001-member-registration/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/
├── Members.Domain/              # Entities, value objects, enums, domain events, abstractions
│   ├── Members/
│   ├── Common/
│   └── Abstractions/
├── Members.Application/         # Vertical slices, CQRS, behaviors, Result<T>
│   ├── Common/
│   │   ├── Behaviors/
│   │   ├── Result/
│   │   └── Messaging/
│   └── Features/
│       └── Members/
│           ├── RegisterMember/
│           ├── GetMemberById/
│           ├── ListMembers/
│           └── UpdateMember/
├── Members.Infrastructure/      # EF Core, migrations, repositories, interceptors, health checks
│   ├── Persistence/
│   └── HealthChecks/
└── Members.WebApi/              # Minimal API endpoints, DI, Scalar, exception handler, health
    ├── Endpoints/
    └── Infrastructure/

tests/
├── Members.Domain.UnitTests/
├── Members.Application.UnitTests/
└── Members.Api.IntegrationTests/

frontend/
├── src/
│   ├── components/
│   ├── features/
│   │   └── registration/
│   ├── hooks/
│   ├── schemas/
│   └── services/
└── tests/
```

**Structure Decision**: Four-project backend solution satisfies Clean Architecture inward-only dependency rule; `frontend/` is a separate Vite React SPA. Each backend feature is a vertical slice containing command/query, handler, validator, response DTO, and endpoint.

## Complexity Tracking

No constitution violations requiring justification. The four backend projects and separate frontend are necessary to satisfy the Clean Architecture, Vertical Slice, and separation-of-concerns principles ratified in the constitution.
