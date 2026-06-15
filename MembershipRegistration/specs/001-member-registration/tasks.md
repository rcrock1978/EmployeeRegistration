---

description: "Task list for 001-member-registration feature implementation"

---

# Tasks: Member Registration Platform

**Input**: Design documents from `specs/001-member-registration/`

**Prerequisites**: plan.md, spec.md (6 user stories P1–P3), research.md, data-model.md, contracts/

**Tests**: Included per constitution Principle VII (Quality via Tests — no feature merges without green quality gates). Each user story phase includes TDD integration tests that exercise the full HTTP→database path.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Path Conventions

- **Backend**: `src/Members.{Domain,Application,Infrastructure,WebApi}/`
- **Frontend**: `frontend/src/`
- **Tests**: `tests/Members.{Domain,Application}.UnitTests/` and `tests/Members.Api.IntegrationTests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the project skeleton, solution structure, and dependency configuration for all layers.

- [X] T001 Create .NET solution `OPTODEV.Members.slnx` at repo root with solution folders: `src/`, `tests/`, `frontend/`
- [X] T002 Initialize `src/Members.Domain/` as a C# class library (no external dependencies)
- [X] T003 [P] Initialize `src/Members.Application/` as a C# class library with project reference to `Members.Domain`
- [X] T004 [P] Initialize `src/Members.Infrastructure/` as a C# class library with references to `Members.Application`; add NuGet deps: EF Core 10, Npgsql, EF Core Npgsql
- [X] T005 [P] Initialize `src/Members.WebApi/` as an ASP.NET Core Minimal API project with references to `Members.Infrastructure`; add NuGet deps: FluentValidation, Scalar.AspNetCore
- [X] T006 [P] Initialize `tests/Members.Domain.UnitTests/` as xUnit project with reference to `Members.Domain`; add FluentAssertions
- [X] T007 [P] Initialize `tests/Members.Application.UnitTests/` as xUnit project with reference to `Members.Application`; add FluentAssertions, NSubstitute
- [X] T008 [P] Initialize `tests/Members.Api.IntegrationTests/` as xUnit project with reference to `Members.WebApi`; add FluentAssertions, Testcontainers.PostgreSQL, WebApplicationFactory
- [X] T009 [P] Create `frontend/` with Vite + React 18 + TypeScript 5, install Tailwind CSS, React Hook Form, Zod
- [X] T010 [P] Configure `.editorconfig` and `Directory.Build.props` at solution root with nullable enabled, implicit usings, and analyzers
- [X] T010a [P] Configure secrets manager integration placeholder in `src/Members.WebApi/Program.cs` — configuration sourced externally; verify no secrets in `appsettings.*` files

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can begin. Every downstream task depends on these.

**⚠️ CRITICAL**: No user story work can start until this phase is complete.

- [X] T011 Implement `Result<T>` and `Result` (void variant) in `src/Members.Application/Common/Results/Result.cs` with success/failure, implicit errors collection, and status mapping
- [X] T012 [P] Implement `ISender` interface and `Sender` default implementation (in-process dispatch) in `src/Members.Application/Common/Messaging/`
- [X] T013 [P] Implement `IQueryHandler<TQuery, TResponse>` and `ICommandHandler<TCommand, TResponse>` base abstractions in `src/Members.Application/Common/Messaging/`
- [X] T014 [P] Implement `IPipelineBehavior` interface and `Sender` mediator dispatch in `src/Members.Application/Common/Messaging/`
- [X] T015 [P] Implement `ValidationBehavior<TRequest, TResponse>` pipeline behavior in `src/Members.Application/Common/Behaviors/` that runs FluentValidation validators registered for `TRequest`
- [X] T016 [P] Implement `LoggingBehavior<TRequest, TResponse>` pipeline behavior in `src/Members.Application/Common/Behaviors/` with structured log start/finish and PII redaction
- [X] T017 [P] Create `MembersDbContext` in `src/Members.Infrastructure/Persistence/MembersDbContext.cs` with EF Core 10, entity configurations
- [X] T018 [P] Implement `AuditableEntity` base class and `AuditInterceptor` in `src/Members.Infrastructure/Persistence/` that auto-sets `CreatedOn`, `CreatedBy`, `UpdatedOn`, `UpdatedBy` on save
- [X] T019 [P] Implement `GlobalExceptionHandler` middleware in `src/Members.WebApi/Infrastructure/GlobalExceptionHandler.cs` that maps unhandled exceptions to the uniform error envelope
- [X] T020 [P] Configure `/health/live` (always 200) and `/health/ready` (reflects DB connectivity via EF Core ping) in `src/Members.WebApi/Endpoints/HealthEndpoints.cs`
- [X] T021 [P] Configure Scalar v1 OpenAPI UI in `src/Members.WebApi/Program.cs` — enabled only in non-production
- [ ] T022 [P] Create initial EF Core migration in `src/Members.Infrastructure/Persistence/Migrations/` — requires PostgreSQL running; run `dotnet ef migrations add InitialCreate`
- [ ] T023 [P] Create `tests/Members.Api.IntegrationTests/HealthCheckTests.cs` — verify `/health/live` returns 200 and `/health/ready` returns 200/503 based on DB state
- [ ] T024 [P] Configure CI pipeline (GitHub Actions or equivalent) that runs build, all test projects, and NetArchTest architecture rule enforcement
- [X] T025 [P] Write NetArchTest rules in `tests/Members.Domain.UnitTests/ArchitectureTests.cs` enforcing inward-only Clean Architecture dependency direction
- [X] T026 [P] Register all services in `src/Members.WebApi/Program.cs` — DI setup for DbContext, Sender, validators, behaviors, exception handler, health checks, Scalar

**Checkpoint**: Foundation ready — all user story implementation can begin.

---

## Phase 3: User Story 1 — Complete member registration in a guided wizard (Priority: P1) 🎯 MVP

**Goal**: Applicants can submit a complete member registration through the API. A `POST /api/members` endpoint receives the full payload, validates it, persists the member record, and returns the created resource with a uniform envelope.

**Independent Test**: A valid `POST /api/members` request returns `201 Created` with the member ID; an invalid request returns `422 Unprocessable Entity` with field-level errors; a duplicate email returns `409 Conflict`.

### Tests for User Story 1

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation.

- [ ] T027 [P] [US1] Write `RegisterMember_ValidRequest_Returns201` integration test in `tests/Members.Api.IntegrationTests/Members/RegisterMemberTests.cs`
- [ ] T028 [P] [US1] Write `RegisterMember_InvalidPayload_Returns422` integration test in `tests/Members.Api.IntegrationTests/Members/RegisterMemberTests.cs`
- [ ] T029 [P] [US1] Write `RegisterMember_DuplicateEmail_Returns409` integration test in `tests/Members.Api.IntegrationTests/Members/RegisterMemberTests.cs`
- [ ] T030 [P] [US1] Write unit tests for `RegisterMemberCommandValidator` in `tests/Members.Application.UnitTests/Features/Members/RegisterMember/RegisterMemberCommandValidatorTests.cs`
- [ ] T031 [US1] Write unit tests for domain value-object invariants (PersonName, Demographics, ContactDetails, Address, GovernmentIds, EmploymentDetails) in `tests/Members.Domain.UnitTests/`

### Implementation for User Story 1

- [ ] T032 [P] [US1] Create value objects in `src/Members.Domain/Members/`: PersonName, Demographics, BirthDetails, ContactDetails, DependentInfo, RelatedPersons (Spouse, Mother, Father), GovernmentIds, PrimaryIdentification, Address (current/permanent), EmergencyContact, EmploymentDetails, MemberStatus enum
- [ ] T033 [P] [US1] Create `Member` aggregate root in `src/Members.Domain/Members/Member.cs` with all value objects, audit metadata, and factory method `Member.Create(...)`
- [ ] T034 [P] [US1] Create `RegisterMemberCommand` and `RegisterMemberResponse` in `src/Members.Application/Features/Members/RegisterMember/`
- [ ] T035 [P] [US1] Create `RegisterMemberCommandValidator` (FluentValidation) in `src/Members.Application/Features/Members/RegisterMember/` — mirror all domain rules
- [ ] T036 [US1] Create `RegisterMemberCommandHandler` in `src/Members.Application/Features/Members/RegisterMember/` — validates, checks email uniqueness, creates Member, persists, returns response
- [ ] T037 [P] [US1] Create `MemberConfiguration` EF Core entity configuration in `src/Members.Infrastructure/Persistence/Configurations/MemberConfiguration.cs` — map all value objects as owned entities or value conversions
- [ ] T038 [P] [US1] Create `Persistence/Migrations/<timestamp>_CreateMemberTable.cs` migration
- [ ] T039 [US1] Implement `POST /api/members` endpoint in `src/Members.WebApi/Endpoints/MembersEndpoints.cs` — calls `ISender.Send`, maps `Result<T>` to HTTP responses
- [ ] T040 [US1] Wire `RegisterMember` vertical slice in `src/Members.WebApi/Program.cs` — register validator, handler, and endpoint

**Checkpoint**: US1 fully functional — `POST /api/members` creates valid member records and rejects invalid/duplicate submissions.

---

## Phase 4: User Story 2 — Resilient, low-friction data entry (Priority: P1)

**Goal**: Applicants can navigate the wizard forward and backward without losing data, use an address-copy toggle, and see inline validation on field exit.

**Wizard steps** (authoritative order defined in `spec.md`):
1. Personal Information
2. Family / Related Persons
3. Government IDs & Primary ID
4. Residency
5. Employment, Emergency Contact & Consent

**Independent Test**: A user can fill step 1, navigate to step 2, return to step 1 and find all data intact; toggling "Permanent address same as current" copies data and hides the permanent-address section; leaving a field with invalid input shows a field-level message on blur.

### Tests for User Story 2

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation.

- [ ] T040a [P] [US2] Write `RegistrationWizard_PreservesDataOnBackNav` component test in `frontend/tests/RegistrationWizard.test.tsx`
- [ ] T040b [P] [US2] Write `AddressToggle_CopiesAndHidesPermanentAddress` component test in `frontend/tests/AddressToggle.test.tsx`
- [ ] T040c [P] [US2] Write `Validation_ShowsOnBlur` component test in `frontend/tests/Validation.test.tsx`
- [ ] T040d [P] [US2] Write `memberApi_Submit_HandlesSuccessAndError` unit test in `frontend/tests/memberApi.test.ts`

### Implementation for User Story 2

- [ ] T041 [P] [US2] Create multi-step wizard layout in `frontend/src/features/registration/RegistrationWizard.tsx` with visible step indicator and navigation (Next/Back) following the 5-step order in spec.md
- [ ] T042 [P] [US2] Create step components under `frontend/src/features/registration/steps/`: `PersonalInfoStep`, `FamilyAndRelatedPersonsStep`, `GovernmentIdsStep`, `ResidencyStep`, `EmploymentAndConsentStep`
- [ ] T043 [P] [US2] Create Zod validation schemas in `frontend/src/schemas/registrationSchema.ts` — one schema per step, mirroring server FluentValidation rules
- [ ] T044 [P] [US2] Create `useRegistrationWizard` hook in `frontend/src/hooks/useRegistrationWizard.ts` — manages current step, form state persistence across steps, async submission
- [ ] T045 [P] [US2] Implement address copy toggle ("Permanent address same as current") in `ContactAndAddressStep.tsx` — RHF `watch` triggers copy on check, restores editable form on uncheck
- [ ] T046 [P] [US2] Implement field-level validation on blur with Zod + React Hook Form `mode: 'onBlur'` in each step component
- [ ] T047 [P] [US2] Create `frontend/src/services/memberApi.ts` — API client for `POST /api/members` with typed request/response and error handling
- [ ] T048 [US2] Wire submission in `ReviewAndConsentStep.tsx` — calls API client, handles success/validation-failure/server-error states, maps server errors back to fields
- [ ] T049 [P] [US2] Configure Tailwind CSS for mobile-first responsive layout, dark mode, 44×44 px touch targets, correct input keyboards (type="email", type="tel")
- [ ] T050 [US2] Implement `App.tsx` in `frontend/src/` — renders `RegistrationWizard` at `/` with React Router
- [ ] T050a [P] [US2] Create `ThemeProvider` React context in `frontend/src/hooks/useTheme.ts` — reads `prefers-color-scheme`, persists choice in localStorage, exposes toggle function
- [ ] T050b [US2] Add dark mode toggle button (sun/moon icon) to `App.tsx` or wizard header — applies `dark` class to `<html>` element when active

**Checkpoint**: US2 fully functional — frontend wizard navigates with data persistence, address toggle works, inline validation shows on blur, async submit succeeds with server feedback.

---

## Phase 5: User Story 3 — Lawful processing of personal information (Priority: P1)

**Goal**: The platform captures explicit consent, encrypts sensitive personal information (SPI) at rest, prevents clear-text SPI in logs, and logs sensitive-field access.

**Independent Test**: A registration without consent is rejected (422); sensitive identifier values (TIN, SSS) are not visible in clear text in application logs or database queries; access to sensitive fields is logged.

### Tests for User Story 3

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation.

- [ ] T051 [P] [US3] Write `RegisterMember_WithoutConsent_Returns422` integration test in `tests/Members.Api.IntegrationTests/Members/RegisterMemberConsentTests.cs`
- [ ] T052 [P] [US3] Write `SensitiveFields_EncryptedAtRest` integration test in `tests/Members.Api.IntegrationTests/Members/SensitiveFieldEncryptionTests.cs` — verify raw DB column contains ciphertext
- [ ] T053 [P] [US3] Write `SensitiveFields_NotInLogOutput` integration test in `tests/Members.Api.IntegrationTests/Members/SensitiveFieldEncryptionTests.cs` — capture logged output, verify no SPI in clear
- [ ] T054 [US3] Write unit test for `ConsentRequiredValidator` in `tests/Members.Application.UnitTests/Features/Members/RegisterMember/ConsentValidationTests.cs`

### Implementation for User Story 3

- [ ] T055 [P] [US3] Create `Consent` value object in `src/Members.Domain/Members/Consent.cs` with `PrivacyAccepted`, `AttestationAccepted`, `SignatureName`, `SignedAtUtc`; add invariant that both must be true
- [ ] T056 [US3] Update `Member` aggregate root to include `Consent` value object
- [ ] T057 [P] [US3] Implement `Aes256GcmEncryption` service in `src/Members.Infrastructure/Encryption/Aes256GcmEncryption.cs` — encrypt/decrypt methods, key fetched from a secrets manager (e.g., Azure Key Vault, AWS Secrets Manager, or HashiCorp Vault) via `IConfiguration`
- [ ] T058 [US3] Create EF Core value converter for encrypted fields in `src/Members.Infrastructure/Persistence/Converters/EncryptedStringConverter.cs` — auto-encrypt on write, auto-decrypt on read for TIN, SSS, PrimaryIdNumber
- [ ] T059 [US3] Update `RegisterMemberCommandValidator` to reject submission when consent flags are not both true
- [ ] T060 [P] [US3] Add PII redaction patterns to `LoggingBehavior` in `src/Members.Application/Common/Behaviors/LoggingBehavior.cs` — mask TIN, SSS, phone, email in log output
- [ ] T061 [P] [US3] Implement `SensitiveFieldAccessLogger` in `src/Members.Infrastructure/Security/SensitiveFieldAccessLogger.cs` — logs who accessed which sensitive fields and when
- [ ] T062 [US3] Wire encryption, redaction, access logger in `src/Members.WebApi/Program.cs`

**Checkpoint**: US3 fully functional — consent gate enforced, sensitive identifiers encrypted at rest, no SPI in logs, access logged.

---

## Phase 6: User Story 4 — Retrieve and correct member records (Priority: P2)

**Goal**: An HR/Admin user can retrieve a member by ID, list members with paging/filtering, and update a member record.

**Independent Test**: Admin `GET /api/members/{id}` returns the record; `GET /api/members?lastName=Doe&page=1&pageSize=20` returns a paged result; `PUT /api/members/{id}` with valid body persists and returns the updated record; a `PUT` for a nonexistent ID returns 404.

### Tests for User Story 4

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation.

- [ ] T063 [P] [US4] Write `GetMemberById_ExistingMember_Returns200` integration test in `tests/Members.Api.IntegrationTests/Members/GetMemberByIdTests.cs`
- [ ] T064 [P] [US4] Write `GetMemberById_NonExistent_Returns404` integration test in `tests/Members.Api.IntegrationTests/Members/GetMemberByIdTests.cs`
- [ ] T065 [P] [US4] Write `ListMembers_WithFilters_ReturnsPagedResults` integration test in `tests/Members.Api.IntegrationTests/Members/ListMembersTests.cs`
- [ ] T066 [P] [US4] Write `UpdateMember_ValidRequest_Returns200` integration test in `tests/Members.Api.IntegrationTests/Members/UpdateMemberTests.cs`
- [ ] T067 [P] [US4] Write `UpdateMember_NonExistent_Returns404` integration test in `tests/Members.Api.IntegrationTests/Members/UpdateMemberTests.cs`
- [ ] T068 [P] [US4] Write unit tests for `GetMemberByIdQueryValidator` and `UpdateMemberCommandValidator` in `tests/Members.Application.UnitTests/Features/Members/`

### Implementation for User Story 4

- [ ] T069 [P] [US4] Create `GetMemberByIdQuery` and `GetMemberByIdResponse` in `src/Members.Application/Features/Members/GetMemberById/`
- [ ] T070 [P] [US4] Create `GetMemberByIdQueryValidator` in `src/Members.Application/Features/Members/GetMemberById/`
- [ ] T071 [US4] Create `GetMemberByIdQueryHandler` in `src/Members.Application/Features/Members/GetMemberById/` — reads member by ID from DbContext, returns response or NotFound
- [ ] T072 [P] [US4] Create `ListMembersQuery` and `ListMembersResponse` in `src/Members.Application/Features/Members/ListMembers/` — support filters: lastName, email, employmentLevel, createdDateFrom, createdDateTo; paging: page, pageSize (default 20, max 100)
- [ ] T073 [P] [US4] Create `ListMembersQueryValidator` in `src/Members.Application/Features/Members/ListMembers/`
- [ ] T074 [US4] Create `ListMembersQueryHandler` in `src/Members.Application/Features/Members/ListMembers/` — applies filters, paginates, returns paged result with total count
- [ ] T075 [P] [US4] Create `UpdateMemberCommand` and `UpdateMemberResponse` in `src/Members.Application/Features/Members/UpdateMember/`
- [ ] T076 [P] [US4] Create `UpdateMemberCommandValidator` in `src/Members.Application/Features/Members/UpdateMember/`
- [ ] T077 [US4] Create `UpdateMemberCommandHandler` in `src/Members.Application/Features/Members/UpdateMember/` — loads, applies changes, saves, updates audit metadata, returns response
- [ ] T078 [P] [US4] Implement `GET /api/members/{id}` endpoint in `src/Members.WebApi/Endpoints/MembersEndpoints.cs`
- [ ] T079 [P] [US4] Implement `GET /api/members` endpoint with query string filters in `src/Members.WebApi/Endpoints/MembersEndpoints.cs`
- [ ] T080 [US4] Implement `PUT /api/members/{id}` endpoint in `src/Members.WebApi/Endpoints/MembersEndpoints.cs`

**Checkpoint**: US4 fully functional — full CRUD for member records via REST API with paged listing and filtering.

---

## Phase 7: User Story 5 — Member self-view (Priority: P2)

**Goal**: A user with the Member role can view only his/her own record; HR/Admin role retains full list and detail access. Role and identity claims come from the external OIDC/JWT mechanism.

**Independent Test**: With a Member JWT (`sub` matching the record's email), `GET /api/members/{ownId}` returns 200; `GET /api/members` returns 403; `GET /api/members/{otherId}` returns 403. With an HR/Admin JWT, both the list and any detail endpoint return 200.

### Tests for User Story 5

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation.

- [ ] T081 [P] [US5] Write `MemberRole_OwnDetail_Returns200` integration test in `tests/Members.Api.IntegrationTests/Members/RoleBasedAccessTests.cs`
- [ ] T082 [P] [US5] Write `MemberRole_List_Returns403` integration test in `tests/Members.Api.IntegrationTests/Members/RoleBasedAccessTests.cs`
- [ ] T083 [P] [US5] Write `MemberRole_OtherDetail_Returns403` integration test in `tests/Members.Api.IntegrationTests/Members/RoleBasedAccessTests.cs`
- [ ] T084 [P] [US5] Write `HrAdminRole_ListAndDetail_Returns200` integration test in `tests/Members.Api.IntegrationTests/Members/RoleBasedAccessTests.cs`

### Implementation for User Story 5

- [ ] T085 [P] [US5] Create `MemberAuthorizationHandler` in `src/Members.WebApi/Infrastructure/MemberAuthorizationHandler.cs` — reads `sub` and `role` JWT claims, resolves member by `sub` (email), enforces access rules
- [ ] T086 [US5] Add authorization policy to `GET /api/members/{id}` — Member role: only if `sub` matches record email; HR/Admin role: always allowed
- [ ] T087 [US5] Add authorization to `GET /api/members` — Member role: 403; HR/Admin role: allowed
- [ ] T088 [US5] Update `RegisterMemberCommand` to capture `CreatedBy` from JWT `sub` claim
- [ ] T089 [US5] Wire authorization policies and JWT validation in `src/Members.WebApi/Program.cs`

**Checkpoint**: US5 fully functional — RBAC enforced per spec scenarios for both Member and HR/Admin roles.

---

## Phase 8: User Story 6 — Operability and health visibility (Priority: P3)

**Goal**: Operators can verify service health via probes, and every request leaves a structured trace with a correlation ID.

**Independent Test**: `/health/live` returns 200; `/health/ready` returns 200 when DB is reachable and 503 when not; structured log entries for any request include a `CorrelationId` field.

### Tests for User Story 6

- [ ] T089a [P] [US6] Write `HealthProbes_ReflectDataStoreState` integration test in `tests/Members.Api.IntegrationTests/HealthCheckTests.cs`

### Implementation for User Story 6

- [ ] T090 [P] [US6] Implement `CorrelationIdMiddleware` in `src/Members.WebApi/Infrastructure/CorrelationIdMiddleware.cs` — reads/generates correlation ID, sets on `HttpContext`, includes in structured log scope
- [ ] T091 [P] [US6] Update health check endpoints in `src/Members.WebApi/Endpoints/HealthEndpoints.cs` — readiness checks datastore via EF Core, liveness simple 200
- [X] T092 [P] [US6] Add structured logging configuration (Serilog or `ILogger` with structured templates) in `src/Members.WebApi/Program.cs` — log level, output template with correlation ID
**Checkpoint**: US6 fully functional — health probes accurately reflect service state, every request traceable via correlation ID.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories and final quality gates.

- [ ] T094 [P] Run all integration tests with Testcontainers PostgreSQL — green build required
- [ ] T095 [P] Run NetArchTest architecture rules — verify inward-only Clean Architecture dependencies
- [ ] T096 [P] Run frontend build (`npm run build`) — verify clean TypeScript compilation
- [ ] T097 [P] Validate all 11 quickstart scenarios from `specs/001-member-registration/quickstart.md`
- [ ] T098 [P] Final review: verify no secrets, connection strings, or keys in source code
- [ ] T099 [P] Final review: verify PII redaction active in logging behavior
- [ ] T100 [P] Update `AGENTS.md` SPECKIT markers to reference tasks.md and reflect implementation status
- [ ] T100a [P] Run WCAG 2.1 AA accessibility audit using axe-core (`@axe-core/react` or Pa11y CI) in `frontend/` — assert zero critical/serious violations
- [ ] T100b [P] Write performance benchmark script (k6 or `dotnet-counters`) that asserts p95 `POST /api/members` ≤ 400ms, p95 `GET /api/members/{id}` ≤ 200ms, p95 `GET /api/members` ≤ 200ms in `tests/Members.PerformanceTests/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **US1 (Phase 3)**: Depends on Foundational — no other story dependencies (MVP!)
- **US2 (Phase 4)**: Depends on Foundational — frontend independent of backend API initially; final integration requires US1 backend to be deployed
- **US3 (Phase 5)**: Depends on Foundational + US1 (encryption applied to US1's fields)
- **US4 (Phase 6)**: Depends on Foundational + US3 encryption for sensitive field reads
- **US5 (Phase 7)**: Depends on US4 endpoints + authorization infrastructure
- **US6 (Phase 8)**: Depends on Foundational (health probes exist in Phase 2, this phase adds polish)
- **Polish (Phase 9)**: Depends on all desired user stories complete

### User Story Dependencies

- **US1 (P1)**: Starts after Foundational — No dependencies on other stories
- **US2 (P1)**: Frontend-only — Can be built in parallel with US1 backend; final wire-up needs US1 API
- **US3 (P1)**: Requires US1's Member entity and DbContext for encryption configuration
- **US4 (P2)**: Requires US3's encryption for safe reading of sensitive fields
- **US5 (P2)**: Requires US4's member read endpoints + Foundational auth infrastructure
- **US6 (P3)**: Can start after Foundational — independent of other stories

### Within Each User Story

- Tests (included) MUST be written and FAIL before implementation
- Models → Services → Endpoints → Integration
- Story complete before moving to next priority

---

## Parallel Opportunities

| Phase | Parallel Tasks | Rationale |
|---|---|---|
| Phase 1 | T003, T004, T005, T006, T007, T008, T009, T010 | Independent project initializations |
| Phase 2 | T012–T026 (all [P] tasks) | Each is a different file/class |
| Phase 3 | T027–T030, T032–T035, T037–T038 | Tests independent of implementation |
| Phase 5 | T051–T054 (tests), T055, T057, T060–T061 | Independent modules |
| Phase 6 | T063–T068 (tests), T069–T070, T072–T073, T075–T076, T078–T079 | Slices are independent |
| Phase 7 | T081–T084 (tests), T085 | Auth handler independent |
| Phase 8 | T090–T092 | Middleware, health, logging independent |

### Parallel Execution Example

```bash
# Phase 1 — all project init in parallel:
Task: "T002 Create Members.Domain project"
Task: "T003 Create Members.Application project"
Task: "T004 Create Members.Infrastructure project"
Task: "T005 Create Members.WebApi project"
Task: "T009 Create frontend project"

# Phase 3 US1 — tests and models in parallel:
Task: "T027 Write RegisterMember_ValidRequest_Returns201 test"
Task: "T032 Create domain value objects"
Task: "T034 Create RegisterMemberCommand"

# Phase 4 US2 — all frontend components in parallel:
Task: "T041 Create RegistrationWizard layout"
Task: "T042 Create step components"
Task: "T043 Create Zod schemas"
Task: "T044 Create useRegistrationWizard hook"
```

---

## Implementation Strategy

### MVP First (US1 Only) 🎯

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: US1 (registration command + Docker/dotnet run)
4. **STOP and VALIDATE**: `POST /api/members` creates member records
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add US1 (P1) → **MVP: Registration API works!**
3. Add US3 (P1) → Compliant registration
4. Add US2 (P1) → Frontend wizard consumes US1 API
5. Add US4 (P2) → Admin management API
6. Add US5 (P2) → RBAC on US4 endpoints
7. Add US6 (P3) → Observable ops

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: US1 (registration API)
   - Developer B: US2 (frontend wizard, in parallel with A)
   - Developer C: US4 (read/manage, can start before US3 encryption is finalized)
3. Developer A continues to: US3 (encryption applied to US1)
4. Developer B integrates with US1 endpoint when available
5. Developer C hands off to Developer A for: US5 (RBAC on US4)
6. Anyone: US6 (health polish)

---

## Notes

- [P] tasks = different files, no dependencies — can be done in parallel
- [US#] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing (TDD)
- Commit after each task or logical group
- Stop at any checkpoint to validate the story independently
- Avoid: vague tasks, same-file conflicts, cross-story dependencies that break independence
