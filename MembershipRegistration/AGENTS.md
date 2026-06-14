# Agent Notes — MembershipRegistration

<!-- SPECKIT START -->
For the latest implementation plan and feature context, read `specs/001-member-registration/plan.md`. The plan was refreshed on 2026-06-15 with clarifications from `/speckit.clarify` (concurrency handling, primary ID types, admin-create endpoint, status lifecycle). The authoritative product spec remains `../PRD-Member-Registration.md` in the parent directory (`Porfolio_001/`).
<!-- SPECKIT END -->

## Repo state

- **All implementation phases complete (P0–P4).** Full-stack member registration platform: .NET 10 REST API + React SPA wizard.
- Build: 0 errors, 2 warnings (MSB3277 pre-release EF Core version conflict, non-blocking).
- Architecture tests: 6/6 passing (inward-only Clean Architecture dependency enforcement).
- Frontend: 0 errors, 317 KB JS production bundle (93 KB gzipped).
- Integration tests require Docker (PostgreSQL via Testcontainers) — cannot run on this machine.

## Source of truth

- `../PRD-Member-Registration.md` — authoritative product spec.
- `../Member Information Form.xlsx` — source artifact the PRD was derived from.
- When the two conflict: prefer the Excel file for raw field order/format; prefer the PRD for architecture and behavior intent.
- `specs/001-member-registration/tasks.md` — granular task list with checkboxes for all 6 user stories.

## Architecture

### Backend: .NET 10 ASP.NET Core Minimal APIs
- **Pattern**: Clean Architecture + Vertical Slice + CQRS
- **Mediator**: Custom lightweight `ISender`/`IPipelineBehavior` (no MediatR licensing)
- **ORM**: EF Core 10 + Npgsql (PostgreSQL 15+)
- **Validation**: FluentValidation + `ValidationBehavior` pipeline
- **Persistence**: `MembersDbContext` (optional `IEncryptionService`), `AuditInterceptor`, `MemberRepository` implementing `IMemberRepository`
- **Auth**: JWT Bearer (dev symmetric key or OIDC provider), `HRAdminOnly` policy, `MemberOwnerAuthorizationFilter` for owner vs admin check
- **Encryption**: AES-256-GCM via `AesGcmEncryptionService`, value converters applied in `OnModelCreating` for TIN/SSS/PrimaryId.Number
- **Logging**: Structured with `CorrelationIdMiddleware` (X-Correlation-Id header), PII redaction (TIN/SSS patterns), `LoggingBehavior` pipeline
- **Health**: `/health/live` (200), `/health/ready` (200 DB ok / 503 DB down)
- **OpenAPI**: Native + Scalar UI at `/scalar/v1` in development

### Frontend: React 19 SPA
- **State**: React Hook Form + Zod v4 schemas (one per step)
- **Build**: Vite 8 + TypeScript 6 + Tailwind CSS v4
- **Wizard**: 5-step registration (Personal Info → Family → Government IDs → Residency → Employment & Consent)
- **Features**: Step indicator, "same as current" address toggle, field validation on blur, consent checkboxes, dark mode

### API Endpoints
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/members` | AllowAnonymous | Register new member |
| GET | `/api/members/{id}` | RequireAuthorization + owner check | View member (admin=any, member=own) |
| GET | `/api/members` | HRAdminOnly | List members with paging/filtering |
| PUT | `/api/members/{id}` | HRAdminOnly | Update member record |
| GET | `/health/live` | AllowAnonymous | Liveness probe |
| GET | `/health/ready` | AllowAnonymous | Readiness probe (DB check) |

## Key Decisions
- Duplicate detection: email address uniqueness only (TIN/company ID not detected in v1)
- Optimistic concurrency on PUT: EF Core RowVersion → 409 Conflict on mismatch
- Admin create: same POST endpoint, gated by HRAdmin role (not enforced on POST)
- Member status transitions: HR/Admin via PUT /api/members/{id}
- CQRS mediator: custom lightweight instead of MediatR (avoids commercial licensing)
- Sensitive field encryption: AES-256-GCM, value converters at ModelBuilder level
- Dev tokens: symmetric key HMAC-SHA256, used by DevTokenHelper for integration tests
- Connection string in `appsettings.Development.json` (`postgres/postgres`) — local dev only

## Files Created/Modified (src/)

### Application layer (`src/Members.Application/`)
- `Common/Results/Result.cs` — `Result<T>` envelope with AppError/FieldError
- `Common/Messaging/` — `ISender`, `Sender`, `ICommandHandler`, `IQueryHandler`, `IPipelineBehavior`
- `Common/Behaviors/` — `ValidationBehavior`, `LoggingBehavior` (PII redaction)
- `Common/IEncryptionService.cs` — encryption contract
- `Common/ICurrentUserService.cs` — authenticated user abstraction
- `Features/Members/RegisterMember/` — command, validator, handler
- `Features/Members/GetMemberById/` — query, validator, handler
- `Features/Members/ListMembers/` — query, validator, handler
- `Features/Members/UpdateMember/` — command, validator, handler

### Domain layer (`src/Members.Domain/`)
- `Common/AuditableEntity.cs` — audit timestamps base class
- `Members/Member.cs` — aggregate root with Create() factory and Update() method
- `Members/IMemberRepository.cs` — repository interface (+ MemberListItem, PagedResult)
- `Members/{value objects}.cs` — PersonName, Demographics, ContactDetails, GovernmentIds, etc.

### Infrastructure layer (`src/Members.Infrastructure/`)
- `Persistence/MembersDbContext.cs` — EF Core DbContext with optional encryption
- `Persistence/MemberRepository.cs` — repository implementation with access audit logging
- `Persistence/Configurations/MemberConfiguration.cs` — EF Core entity config
- `Persistence/Migrations/` — InitialCreate migration
- `Security/AesGcmEncryptionService.cs` — AES-256-GCM encryption
- `Security/EncryptionOptions.cs`, `EncryptionModelBuilderExtensions.cs`

### WebApi layer (`src/Members.WebApi/`)
- `Endpoints/MembersEndpoints.cs` — all member endpoints
- `Endpoints/HealthEndpoints.cs` — liveness/readiness probes
- `Infrastructure/CorrelationIdMiddleware.cs` — X-Correlation-Id header propagation
- `Infrastructure/CurrentUserService.cs` — reads JWT claims
- `Infrastructure/MemberOwnerAuthorizationFilter.cs` — RBAC owner check
- `Infrastructure/AuthOptions.cs` — dev/prod auth config
- `Infrastructure/GlobalExceptionHandler.cs` — structured error responses
- `Program.cs` — DI, middleware pipeline, auth, logging configuration
- `appsettings.json`, `appsettings.Development.json`

## Remaining / Blocked
- Integration tests require Docker with PostgreSQL container (`docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:16-alpine`)
- Performance benchmarks (k6 script not written)
- WCAG 2.1 AA accessibility audit (requires Pa11y CI or axe-core runner)
- Frontend component tests (React Testing Library not set up)

## Speckit workflow tooling

- This repo has `.opencode/` and `.specify/` directories installed by speckit. They provide planning/implementation commands and templates.
- Do **not** manually edit the `<!-- SPECKIT START -->` / `<!-- SPECKIT END -->` markers above; the `speckit.agent-context.update` extension refreshes them.
- `.specify/memory/constitution.md` is the ratified project constitution; `/speckit.plan` and `/speckit.implement` must verify adherence to every principle.
