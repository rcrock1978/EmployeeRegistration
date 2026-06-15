# Product Requirements Document — Member Registration

| | |
|---|---|
| **Product / Feature** | Member Registration (Member Information Form digitization) |
| **Document type** | Product Requirements Document (PRD) |
| **Version** | 1.0 |
| **Status** | Draft — for review |
| **Date** | 15 June 2026 |
| **Author** | Software Architect / Lead Engineer |
| **Owning org** | OPTODEV, INC. |
| **Reviewers** | _TBD — Product Owner, HR/Membership Lead, Data Privacy Officer, Security_ |

### Change log

| Version | Date | Author | Summary |
|---|---|---|---|
| 1.0 | 2026-06-15 | Architect | Initial draft from `Member_Information_Form.xlsx` + architecture brief |

---

## 1. Purpose & background

OPTODEV currently captures member/employee information on a paper/spreadsheet form (`Member_Information_Form.xlsx`). The form is dense and collects sensitive data across many groups — personal details, government identifiers, two addresses, a primary government ID, family/related persons, an emergency contact, and employment details — terminating in a wet signature block.

A flat digital rendering of this form would have a high abandonment rate. This document specifies a **multi-step web application** (React SPA) backed by a **modern .NET 10 REST API** built on Clean Architecture + Vertical Slice + CQRS, that:

- Reduces friction and cognitive load via a progressive wizard.
- Validates data end-to-end (client and server) before persistence.
- Treats government identifiers (TIN, SSS) and the primary ID as **Sensitive Personal Information** under the Philippine Data Privacy Act of 2012 (RA 10173).
- Produces a clean, auditable, queryable member record.

## 2. Goals & non-goals

### 2.1 Goals

- **G1** — Capture a complete, validated member record matching the source form's data points.
- **G2** — Increase completion rate by replacing a single dense page with a 5-step wizard with inline validation and progress indication.
- **G3** — Enforce one canonical, strongly-typed data contract shared in spirit across client (Zod) and server (FluentValidation / domain invariants).
- **G4** — Deliver a maintainable, testable backend with consistent error semantics (`Result<T>`), automatic auditing, observability, and health probes.
- **G5** — Handle Sensitive Personal Information in compliance with RA 10173 (consent, minimization, encryption, access control, retention).

### 2.2 Non-goals (this release)

- **NG1** — Payment / dues collection. (The brief mentions a Payment step generically; the source form has no payment fields. Out of scope unless added by Product.)
- **NG2** — Member self-service portal / profile editing after submission (read-back only for admins in v1).
- **NG3** — Document upload / OCR of physical IDs (a digital ID-image upload is a **candidate** for v1.1 — see Open Questions).
- **NG4** — Workflow/approval routing of submitted applications (a status field is included; routing engine is future).
- **NG5** — Bulk import / migration of historical paper records.

## 3. Scope

**In scope:** registration wizard UI, member create/read/list/update API, validation, persistence (PostgreSQL + EF Core 10), auditing, error handling, health checks, OpenAPI/Scalar docs, observability, security baseline for PII.

**Out of scope:** items in §2.2; SSO identity-provider provisioning beyond consuming standard OIDC; reporting/analytics dashboards; native mobile apps (responsive web only); automated admin-user provisioning (admin credentials are managed out-of-band).

## 4. Stakeholders & personas

| Persona | Description | Primary need |
|---|---|---|
| **Applicant / Member** | Person completing their own information. May be on mobile. Mixed digital literacy. | Fast, forgiving, clear form that doesn't lose their data. |
| **HR / Membership Admin** | Reviews, searches, and corrects submitted records. | Reliable read-back, search, edit, complete audit trail. |
| **Data Privacy Officer (NPC compliance)** | Accountable for lawful processing of PII. | Consent capture, minimization, retention, access logging. |
| **Platform / DevOps Engineer** | Operates the service. | Health probes, structured logs, clean failure modes. |
| **Integrator** | May push records downstream (HRIS, ServiceNow, etc.). | Stable, documented API and consistent response envelope. |

## 5. User stories (epics)

- **E1 — Self-registration.** As an applicant, I can complete my member information in clearly separated steps, see what's left, and only submit once everything is valid.
- **E2 — Resilient data entry.** As an applicant, I see validation as I leave each field, can copy my current address into my permanent address, and never lose entered data when moving between steps.
- **E3 — Record retrieval.** As an admin, I can retrieve a member by ID and list/search members with paging.
- **E4 — Record correction.** As an admin, I can update a member record; the system records who/when via audit fields.
- **E5 — Lawful processing.** As a DPO, I can confirm consent was captured, sensitive fields are protected, and access is logged.
- **E6 — Operability.** As an operator, I can probe liveness/readiness and consume structured request/response traces.
- **E7 — Member self-view.** As a member, I can view my own submitted member details, but I cannot see the member list or other members' details.

Each epic's testable acceptance criteria are in §13.

---

## 6. Functional requirements

### 6.1 Registration wizard (progressive disclosure)

The single dense form is decomposed into a **5-step wizard**, single-column, mobile-first. A step is only advanceable when its own fields validate (partial validation via `trigger`), but all data is held in a single typed client state until final submit.

| Step | Title | Field groups (from source form) |
|---|---|---|
| 1 | **Personal information** | Member name & demographics, contact, dependents, education |
| 2 | **Family / related persons** | Related-person name blocks (see §7.3) — *conditional* |
| 3 | **Government IDs & primary ID** | TIN, SSS, primary government ID details |
| 4 | **Residency** | Current address, permanent address (with "same as current" toggle) |
| 5 | **Contacts, employment & consent** | Emergency contact, employment details, consent + attestation, review & submit |

> **Note on the brief's "Account Details / Payment" steps:** the source form has no account-credential or payment fields. If SSO-based account creation or payment is required, Product must add those fields; otherwise the wizard above reflects the actual data. SSO ("Continue with Google/Microsoft/Apple") is supported as an *optional accelerator* for prefilling name/email and binding the submission to an authenticated identity — not as a data source the form depends on.

**FR-W1** — A persistent progress indicator shows current step and total steps.
**FR-W2** — "Next" validates only the current step; "Back" never triggers validation or data loss.
**FR-W3** — The final "Submit" is disabled until the entire schema is valid; it is the most visually distinct control on the screen and is visually distinct from "Cancel"/"Back".
**FR-W4** — Submission is asynchronous (fetch/AJAX); the page never reloads. Success, validation-failure, and server-error states are each rendered distinctly.
**FR-W5** — On a server-side validation failure, field-level errors returned by the API are mapped back onto the corresponding inputs and steps (the failing step is surfaced).

### 6.2 Address convenience

**FR-A1** — A "Permanent address same as current" checkbox copies the current-address state into permanent-address and hides the permanent-address inputs while checked. Unchecking restores an editable (empty or last-entered) permanent address.
**FR-A2** — When "same as current" is checked, permanent-address validation mirrors current-address validation.

### 6.3 Data masking & formatting

**FR-F1** — TIN and SSS inputs apply input masks that insert separators as the user types, conforming to the patterns in §8.
**FR-F2** — All date inputs present and enforce `dd/mm/yyyy` to the user but serialize to ISO-8601 (`yyyy-MM-dd`) in the API payload.
**FR-F3** — Email and phone inputs trigger the appropriate mobile keyboard (`type="email"`, `inputmode="tel"`).

### 6.4 Record management (admin/API)

**FR-R1** — Create a member (register).
**FR-R2** — Retrieve a member by identifier.
**FR-R3** — List members with paging and basic filtering (by last name, email, employment level, created date range).
**FR-R4** — Update a member record.
**FR-R5** — (Optional, v1.1) Soft-delete / deactivate a member; hard delete is reserved for data-subject erasure requests under RA 10173.

### 6.5 Role-based access to member records

**FR-AC1** — The system MUST distinguish at minimum two roles: **Member** (the applicant whose data was submitted) and **HR/Admin** (authorized membership or HR staff).
**FR-AC2** — An HR/Admin user MUST be able to view the paged member list and open any member's detail page.
**FR-AC3** — A Member user MUST be able to view only his/her own member detail page and MUST NOT be able to access the member list or another member's detail page.
**FR-AC4** — Attempts by a Member to access the list page or another member's detail page MUST be rejected with a `403 Forbidden` response (or equivalent access-denied handling).
**FR-AC5** — Role assignment and authentication are assumed to be provided by the hosting identity mechanism (e.g., OIDC/SSO or corporate identity provider); the system consumes the resolved role and identity claims.

### 6.6 Landing page & navigation

**FR-L1** — A landing page is served at the application root (`/`) with two primary calls to action: "Register as Member" navigates to the registration wizard; "Admin Login" navigates to the admin login page.

**FR-L2** — The landing page displays the OPTODEV branding and a brief description of the registration portal.

**FR-L3** — The app header includes a home link (back to landing page) and, when authenticated as an admin, a logout button.

### 6.7 Admin authentication

**FR-AU1** — An admin login page at `/admin/login` presents a form with email/username and password fields.

**FR-AU2** — A `POST /api/auth/login` endpoint validates credentials and returns a signed JWT containing the user's role claim (`Admin` or `HRAdmin`).

**FR-AU3** — The frontend stores the JWT in `localStorage` and attaches it as a `Bearer` token to all authenticated API requests.

**FR-AU4** — A logout action clears the stored token and redirects to the landing page.

**FR-AU5** — Expired or invalid tokens cause a `401 Unauthorized` response, which the frontend handles by clearing the token and redirecting to the login page.

**FR-AU6** — Unauthenticated access to any admin route (`/admin/*`) redirects to the login page.

### 6.8 Admin member list & detail

**FR-AD1** — An admin member list page at `/admin/members` displays a paginated table of all member records, accessed via `GET /api/members`.

**FR-AD2** — The table shows columns: Name, Email, Status, Employee Level, Created Date.

**FR-AD3** — A filter/search bar allows filtering by last name, email, employee level, and created date range (matching the backend query parameters).

**FR-AD4** — Pagination controls (previous/next, page numbers, page size selector) are displayed below the table.

**FR-AD5** — Clicking a member row navigates to the member detail page at `/admin/members/{id}`.

**FR-AD6** — The member detail page displays all member information read-only, fetched via `GET /api/members/{id}`.

**FR-AD7** — The detail page includes a "Back to list" link.

**FR-AD8** — Loading, empty (no members found), and error states are rendered distinctly on both list and detail pages.

**FR-AD9** — Two admin roles are distinguished: **Admin** (superuser — full CRUD on members, can change admin user roles) and **HRAdmin** (read-only — list members and view detail only).

---

## 7. Domain & data model

> **Provenance:** every field below is derived directly from `Member_Information_Form.xlsx` (Sheet1). The source form is **visually grouped but several name blocks are unlabeled**; inferred labels are marked **[INFERRED]** and listed in Open Questions (§16) for confirmation by the form owner. The pre-filled cell `Company Trade Name = "OPTODEV, INC."` is treated as a configurable default.

### 7.1 Aggregate

The aggregate root is **`Member`**, composed of value objects / owned entities:

```
Member (aggregate root)
├── PersonName            (member's own name)
├── Demographics          (DOB, birthplace, nationality, gender, civil status, religion, education)
├── ContactDetails        (email, phone)
├── DependentInfo         (number of dependents)
├── RelatedPersons        (Spouse[INFERRED], Mother[INFERRED], Father[INFERRED])
├── GovernmentIds         (TIN, SSS)
├── PrimaryIdentification  (type, number, issue/expiry dates, issue country)
├── Address (Current)
├── Address (Permanent)
├── EmergencyContact
├── EmploymentDetails
├── Consent                (privacy consent + attestation)
└── AuditMetadata         (CreatedOn/By, UpdatedOn/By, Status)
```

### 7.2 Data dictionary — member core

| Field | Type | Req. | Constraint / format | PII class¹ |
|---|---|---|---|---|
| `title` | enum | ✓ | `Miss \| Mrs. \| Mr.` (per form note) | Personal |
| `firstName` | string | ✓ | 2–100 chars | Personal |
| `middleName` | string | – | ≤100 chars | Personal |
| `lastName` | string | ✓ | 2–100 chars | Personal |
| `suffix` | string | – | e.g. Jr., Sr., III | Personal |
| `alias` | string | – | ≤100 chars | Personal |
| `dateOfBirth` | date | ✓ | `dd/mm/yyyy`; age ≥ 18 (configurable) | Personal |
| `placeOfBirth` | string | ✓ | ≤200 chars | Personal |
| `countryOfBirth` | string | ✓ | ISO country | Personal |
| `nationality` | string | ✓ | — | Personal |
| `gender` | enum | ✓ | configurable list | Personal |
| `civilStatus` | enum | ✓ | `Single \| Married \| Widowed \| Separated \| …` | Personal |
| `religion` | string | – | — | **Sensitive²** |
| `highestEducationalAttainment` | enum | ✓ | configurable list | Personal |
| `numberOfDependents` | int | ✓ | ≥ 0 | Personal |
| `emailAddress` | string | ✓ | RFC-5322 email; unique (see §16) | Personal |
| `contactNumber` | string | ✓ | E.164-compatible / PH mobile | Personal |

¹ PII classification drives encryption, access logging, and retention (§11).
² **Religion is Sensitive Personal Information under RA 10173.** It is optional on the form; recommend confirming it is genuinely required, and protecting it at the sensitive tier if retained.

### 7.3 Data dictionary — related persons (family) — **[INFERRED]**

The source form contains three unlabeled name blocks immediately after the dependents field. Inferred mapping (confirm in §16):

| Block | Inferred role | Fields | Conditionality |
|---|---|---|---|
| A | **Spouse [INFERRED]** | firstName, middleName, lastName | Required when `civilStatus = Married` |
| B | **Mother's maiden name [INFERRED]** | fullName | Optional / configurable |
| C | **Father's name [INFERRED]** | firstName, middleName, lastName, suffix | Optional / configurable |

### 7.4 Data dictionary — government IDs & primary ID

| Field | Type | Req. | Constraint / format | PII class |
|---|---|---|---|---|
| `tin` | string | ✓³ | `###-###-###-###` (mask + regex) | **Sensitive** |
| `sss` | string | ✓³ | PH SSS format `##-#######-#` | **Sensitive** |
| `primaryId.type` | enum | ✓ | Passport / Driver's License / UMID / PhilID / … | **Sensitive** |
| `primaryId.number` | string | ✓ | per-type rules | **Sensitive** |
| `primaryId.issueDate` | date | ✓ | `dd/mm/yyyy`; ≤ today | Personal |
| `primaryId.expiryDate` | date | ✓ | **must be after** `issueDate`; ≥ today on submit | Personal |
| `primaryId.issueCountry` | string | ✓ | ISO country | Personal |

³ Confirm whether TIN/SSS are mandatory for all member types (§16).

### 7.5 Data dictionary — addresses (current & permanent)

Both addresses share this shape; permanent may be copied from current (§6.2).

| Field | Type | Req. | Constraint |
|---|---|---|---|
| `streetNameAndNumber` | string | ✓ | ≤200 chars |
| `city` | string | ✓ | — |
| `postalCode` | string | ✓ | PH postal (4 digits) by default |
| `barangay` | string | ✓ | — |
| `subdivisionPurok` | string | – | — |
| `province` | string | ✓ | — |
| `country` | string | ✓ | ISO country |
| `ownerOrLessee` | enum | ✓ | `Owner \| Lessee` |
| `occupiedSince` | date | ✓ | `dd/mm/yyyy`; ≤ today |

### 7.6 Data dictionary — emergency contact

| Field | Type | Req. | Constraint |
|---|---|---|---|
| `contactName` | string | ✓ | — |
| `relationship` | string | ✓ | — |
| `contactNumber` | string | ✓ | E.164-compatible |

### 7.7 Data dictionary — employment

| Field | Type | Req. | Constraint |
|---|---|---|---|
| `employeeLevel` | enum | ✓ | `PTS \| RNF` (classification codes — confirm expansion §16) |
| `companyTradeName` | string | ✓ | default `OPTODEV, INC.` |
| `companyIdNumber` | string | ✓ | — |
| `grossIncome` | decimal | ✓ | ≥ 0; currency PHP assumed |
| `incomePeriod` | enum | ✓ | `Annual \| Monthly` (from "Annual/Monthly") |
| `occupation` | string | ✓ | — |
| `hiredFrom` | date | ✓ | `dd/mm/yyyy` |
| `hiredTo` | date | – | `dd/mm/yyyy`; if present, **after** `hiredFrom` (fixed-term) |

### 7.8 Attestation / signature

The paper form ends with **"Signature over Printed Name / Date."** In the digital flow this is replaced by:

| Field | Type | Req. | Notes |
|---|---|---|---|
| `consentGiven` | bool | ✓ | Explicit RA 10173 consent checkbox (unticked by default) |
| `attestation` | bool | ✓ | "I certify the information is true and correct" |
| `signatureName` | string | ✓ | Typed full name as e-signature |
| `signedAt` | datetime | ✓ (system) | Server timestamp at submission |

### 7.9 System / audit fields (server-managed)

| Field | Type | Notes |
|---|---|---|
| `id` | GUID | Primary key (server-generated) |
| `status` | enum | `Submitted \| UnderReview \| Approved \| Rejected` (default `Submitted`) |
| `createdOn` / `createdBy` | datetime / string | Set by audit interceptor |
| `updatedOn` / `updatedBy` | datetime / string | Set by audit interceptor |

---

## 8. Validation rules (client ⇄ server parity)

Validation is enforced **twice**: in the browser via Zod (UX) and on the server via FluentValidation + domain invariants (trust boundary). The server is authoritative. Representative rules:

| Field / rule | Client (Zod) | Server (FluentValidation / domain) |
|---|---|---|
| Required name fields | `min(2)` | `NotEmpty().MinimumLength(2)` |
| `title` | `z.enum(["Miss","Mrs.","Mr."])` | `IsInEnum()` |
| `email` | `z.string().email()` | `EmailAddress()` + uniqueness check |
| `tin` | `regex(/^\d{3}-\d{3}-\d{3}-\d{3}$/)` | matching regex rule |
| `sss` | PH SSS regex | matching regex rule |
| `dateOfBirth` | valid date; age ≥ 18 | `Must(BeAtLeast18)` |
| `primaryId.expiryDate` | `> issueDate` | cross-field rule `GreaterThan(issueDate)` |
| `occupiedSince` / `hiredFrom` | ≤ today | `LessThanOrEqualTo(DateTime.UtcNow)` |
| `hiredTo` (if present) | `> hiredFrom` | cross-field rule |
| Spouse block | required if `civilStatus = Married` | conditional `When(...)` rule |
| Permanent address | required unless "same as current" | conditional rule |
| `consentGiven` | must be `true` | `Equal(true)` |
| `numberOfDependents` | `int ≥ 0` | `GreaterThanOrEqualTo(0)` |

**VR-1** — Dates cross the wire as ISO-8601; client formats to `dd/mm/yyyy` for display only.
**VR-2** — Server validation failures return a structured field-error list (§9.3) that the client maps back to inputs.
**VR-3** — No business rule is enforced *only* on the client.

---

## 9. API specification

RESTful, JSON, built with **Minimal APIs** grouped by feature. Every response uses a uniform `Result<T>` envelope.

### 9.1 Endpoints

| Method | Route | Auth | Purpose | Success |
|---|---|---|---|---|---|
| `POST` | `/api/auth/login` | AllowAnonymous | Authenticate admin and return JWT | `200 OK` + token |
| `POST` | `/api/members` | AllowAnonymous | Register a member | `201 Created` + `Location` |
| `GET` | `/api/members/{id}` | RequireAuthorization | Get member by id | `200 OK` |
| `GET` | `/api/members` | HRAdminOnly | List/search (paged) | `200 OK` |
| `PUT` | `/api/members/{id}` | HRAdminOnly | Update member | `200 OK` |
| `DELETE` | `/api/members/{id}` | — | Deactivate (soft) — *v1.1* | `204 No Content` |
| `GET` | `/health/live` | AllowAnonymous | Liveness probe | `200 OK` |
| `GET` | `/health/ready` | AllowAnonymous | Readiness (DB) probe | `200 OK` |
| `GET` | `/api/members/{id}` | Get member by id | `200 OK` |
| `GET` | `/api/members` | List/search (paged) | `200 OK` |
| `PUT` | `/api/members/{id}` | Update member | `200 OK` |
| `DELETE` | `/api/members/{id}` | Deactivate (soft) — *v1.1* | `204 No Content` |
| `GET` | `/health/live` | Liveness probe | `200 OK` |
| `GET` | `/health/ready` | Readiness (DB) probe | `200 OK` |

Query params for list: `?page=1&pageSize=20&lastName=&email=&employeeLevel=&createdFrom=&createdTo=`.

### 9.2 `Result<T>` envelope (success)

```json
{
  "isSuccess": true,
  "value": {
    "id": "5d2f...-...",
    "fullName": "Juan P. Dela Cruz",
    "emailAddress": "juan@example.com",
    "status": "Submitted",
    "createdOn": "2026-06-15T09:30:00Z"
  },
  "error": null
}
```

### 9.3 `Result<T>` envelope (validation failure)

`HTTP 400`:

```json
{
  "isSuccess": false,
  "value": null,
  "error": {
    "code": "Validation.Failed",
    "message": "One or more fields are invalid.",
    "details": [
      { "field": "personalInfo.email", "code": "Email.Invalid", "message": "A valid email is required." },
      { "field": "identifications.tin", "code": "Tin.Format", "message": "TIN must match ###-###-###-###." },
      { "field": "primaryId.expiryDate", "code": "Date.Order", "message": "Expiry must be after issue date." }
    ]
  }
}
```

### 9.4 Status-code contract

| Code | When |
|---|---|
| `200 OK` | Successful read/update, login |
| `201 Created` | Member registered |
| `204 No Content` | Soft-delete |
| `400 Bad Request` | Validation failure (`Validation.Failed`) or invalid login credentials |
| `401 Unauthorized` | Missing, expired, or invalid JWT token |
| `403 Forbidden` | Authenticated but insufficient role |
| `404 Not Found` | Unknown member id |
| `409 Conflict` | Duplicate (e.g., email/TIN already registered) |
| `422 Unprocessable Entity` | Well-formed but domain-rule violation (if distinguished from 400) |
| `500 Internal Server Error` | Unhandled — returned via global handler as `ProblemDetails` |

**API-1** — All errors flow through global exception handling (§10, requirement R8); raw stack traces are never returned in non-development environments.
**API-2** — OpenAPI document is generated natively; **Scalar** renders the interactive docs at `/scalar/v1`.

---

## 10. System architecture (backend)

A modern **.NET 10** REST API combining Clean Architecture (concentric layers) with Vertical Slice organization (feature-first) and CQRS.

### 10.1 Layering & dependency rule

```
            ┌─────────────────────────────────────────┐
            │                 WebApi                    │  Minimal API endpoints, DI, Scalar,
            │  (composition root / HTTP boundary)       │  exception handler, health checks
            └───────────────┬───────────────────────────┘
                            │ depends on
            ┌───────────────▼───────────────────────────┐
            │             Infrastructure                 │  EF Core 10 DbContext, configs,
            │  (technology / I/O concerns)               │  audit interceptor, repositories,
            └───────────────┬───────────────────────────┘  health-check implementations
                            │ depends on
            ┌───────────────▼───────────────────────────┐
            │              Application                   │  Vertical slices (features),
            │  (use cases / CQRS / behaviors)            │  commands/queries, validators,
            └───────────────┬───────────────────────────┘  pipeline behaviors, Result<T>
                            │ depends on
            ┌───────────────▼───────────────────────────┐
            │                Domain                      │  Entities, value objects,
            │  (enterprise rules — no dependencies)      │  domain events, invariants
            └────────────────────────────────────────────┘
```

**Dependency rule:** references point inward only. Domain depends on nothing; Application depends on Domain; Infrastructure and WebApi depend inward. Abstractions (ports) live in Domain/Application; implementations (adapters) live in Infrastructure.

### 10.2 Vertical-slice solution structure

```
src/
├── Members.Domain/
│   ├── Members/                 (Member aggregate, value objects, enums)
│   ├── Common/                  (base entity, IAuditable, domain events)
│   └── Abstractions/            (ports: IMemberRepository, IUnitOfWork)
│
├── Members.Application/
│   ├── Common/
│   │   ├── Behaviors/           (ValidationBehavior, LoggingBehavior)
│   │   ├── Result/              (Result, Result<T>, Error)
│   │   └── Messaging/           (ICommand, IQuery, handler interfaces)
│   └── Features/
│       └── Members/
│           ├── RegisterMember/
│           │   ├── RegisterMemberCommand.cs
│           │   ├── RegisterMemberHandler.cs
│           │   ├── RegisterMemberValidator.cs
│           │   ├── RegisterMemberResponse.cs
│           │   └── RegisterMemberEndpoint.cs
│           ├── GetMemberById/   (Query + Handler + Endpoint)
│           ├── ListMembers/
│           └── UpdateMember/
│
├── Members.Infrastructure/
│   ├── Persistence/
│   │   ├── MembersDbContext.cs
│   │   ├── Configurations/      (EF Core entity type configs)
│   │   ├── Interceptors/        (AuditableEntityInterceptor)
│   │   └── Repositories/
│   └── HealthChecks/
│
└── Members.WebApi/
    ├── Program.cs               (composition root, pipeline, Scalar, health)
    ├── Endpoints/               (MapGroup registration, route discovery)
    └── Infrastructure/          (GlobalExceptionHandler : IExceptionHandler)

tests/
├── Members.Domain.UnitTests/
├── Members.Application.UnitTests/
└── Members.Api.IntegrationTests/   (WebApplicationFactory + Testcontainers/Postgres)
```

Each feature folder is a **self-contained slice**: command/query, handler, validator, response DTO, and endpoint co-located. Adding a feature means adding a folder — not editing layer-wide files.

### 10.3 CQRS

- **Commands** mutate state and return `Result<T>` (or `Result`): `RegisterMemberCommand`, `UpdateMemberCommand`.
- **Queries** are read-only and return projections/DTOs directly (no domain entities leak out): `GetMemberByIdQuery`, `ListMembersQuery`.
- Commands and queries are dispatched through a mediator with a behavior pipeline (validation → logging → handler).

> **Mediator/library decision:** the pipeline-behavior pattern can be implemented with **MediatR** (note: recent MediatR versions ship under a **commercial license** — budget accordingly), or a lightweight alternative (source-generated mediators, Wolverine, or a small hand-rolled `ISender`/`IPipelineBehavior` abstraction). The architecture does not depend on a specific library; this is a procurement/ops decision to confirm (§16).

### 10.4 Mandatory cross-cutting requirements

| # | Requirement | Implementation intent | Acceptance |
|---|---|---|---|
| R1 | **Clean Architecture** | Concentric layers; inward-only dependencies; ports in inner layers. | Architecture test asserts no outward references (e.g., NetArchTest). |
| R2 | **Vertical Slice Architecture** | Feature-first folders inside Application; co-located command/handler/validator/endpoint. | New feature added without touching unrelated files. |
| R3 | **CQRS separation** | Distinct command vs query paths; queries never mutate. | Commands return `Result`; queries are side-effect free. |
| R4 | **Minimal APIs** | `MapGroup`-based endpoints, `TypedResults`, native OpenAPI metadata. | No MVC controllers; endpoints discoverable & documented. |
| R5 | **FluentValidation pipeline** | A `ValidationBehavior<TRequest,TResponse>` runs all validators before the handler; failures short-circuit to a `Result` validation error. | Invalid request never reaches handler; returns structured `400`. |
| R6 | **Logging decorators** | A `LoggingBehavior` traces each request: name, correlation id, duration, outcome — with PII redaction. | Every request emits start/finish structured logs; TIN/SSS never logged in clear. |
| R7 | **Audit interceptor** | EF Core `SaveChanges` interceptor stamps `CreatedOn/By` and `UpdatedOn/By` on `IAuditable` entities. | Insert/update auto-populate audit fields without handler code. |
| R8 | **Global exception handling** | `IExceptionHandler` + `ProblemDetails`; maps known exceptions to status codes, unknown to `500` (no stack traces in prod). | All unhandled paths return consistent envelope; nothing leaks internals. |
| R9 | **`Result<T>` pattern** | Handlers return `Result`/`Result<T>`; a uniform mapper converts to HTTP responses + status codes. | No control-flow-by-exception for expected failures; consistent envelope (§9). |
| R10 | **Health checks** | `AddHealthChecks()` with a Npgsql/DB readiness check + a liveness self-check; mapped to `/health/ready` and `/health/live`. | Probes return correct status when DB up/down; suitable for k8s/orchestrator. |
| R11 | **PostgreSQL + EF Core 10** | Npgsql provider; migrations; entity configurations; owned types for value objects. | App provisions schema via migrations; CRUD round-trips verified. |
| R12 | **Scalar UI** | Native OpenAPI doc + `Scalar.AspNetCore` interactive UI at `/scalar/v1` (replacing Swashbuckle/Swagger UI). | Docs render; all endpoints/schemas visible in non-prod. |

### 10.5 Backend technology stack

| Concern | Choice |
|---|---|
| Runtime / framework | .NET 10, ASP.NET Core Minimal APIs |
| Language | C# (latest) |
| Persistence | PostgreSQL + EF Core 10 (Npgsql) |
| Validation | FluentValidation (pipeline behavior) |
| Mediation | MediatR (commercial) **or** lightweight alternative (§10.3) |
| API docs | Native OpenAPI + Scalar |
| Logging | `Microsoft.Extensions.Logging` → structured sink (e.g., Serilog/OpenTelemetry) |
| Health | `Microsoft.Extensions.Diagnostics.HealthChecks` |
| Testing | xUnit, FluentAssertions, Testcontainers (Postgres), `WebApplicationFactory` |
| Arch enforcement | NetArchTest (or equivalent) |

---

## 11. Non-functional requirements

### 11.1 Performance & scalability

- **NFR-P1** — p95 latency for `POST /api/members` ≤ 400 ms and for `GET` reads ≤ 200 ms under nominal load (excluding cold start).
- **NFR-P2** — Stateless API; horizontally scalable behind a load balancer.
- **NFR-P3** — List endpoints are paged (default 20, max 100) and indexed on common filters (last name, email, created date).

### 11.2 Availability & operability

- **NFR-O1** — Target availability 99.9% (excluding planned maintenance).
- **NFR-O2** — Liveness & readiness probes (R10) integrated with the orchestrator; readiness fails when the DB is unreachable so traffic is not routed to a broken instance.
- **NFR-O3** — Structured logs with correlation ids; request/response tracing (R6).

### 11.3 Security

- **NFR-S1** — TLS in transit; encryption at rest for the datastore.
- **NFR-S2** — **Field-level protection for Sensitive Personal Information** (TIN, SSS, primary ID number, religion): encrypted/tokenized at rest and **never** written to logs in clear (PII redaction in R6).
- **NFR-S3** — Admin/API access is authenticated and role-based (RBAC); read of sensitive fields is access-logged.
- **NFR-S4** — Standard input-handling protections (parameterized queries via EF Core, output encoding, anti-automation on the public submit endpoint — rate limiting / bot mitigation).
- **NFR-S5** — Secrets (DB credentials, keys) sourced from a secrets manager / key vault, not source or config files.

### 11.4 Privacy & compliance (RA 10173)

- **NFR-C1** — **Explicit, opt-in consent** captured and timestamped before processing (§7.8); consent text references the lawful purpose.
- **NFR-C2** — **Data minimization** — only fields with a stated purpose are collected (re-evaluate religion and any optional sensitive field).
- **NFR-C3** — **Retention policy** — define and enforce a retention period; support data-subject rights (access, correction, erasure) — erasure maps to hard delete.
- **NFR-C4** — Processing activities and access to sensitive fields are auditable for NPC accountability.

### 11.5 Accessibility & UX quality

- **NFR-U1** — WCAG 2.1 AA: sufficient contrast, keyboard navigability, visible focus, labelled inputs, error messages programmatically associated with fields.
- **NFR-U2** — Touch targets ≥ 44×44 px; correct mobile keyboards per input type.
- **NFR-U3** — Dark-mode support honoring system preference.
- **NFR-U4** — Single-column layout with generous white space throughout the wizard.

### 11.6 Maintainability & testability

- **NFR-M1** — Domain and Application layers are framework-agnostic and unit-testable without a database.
- **NFR-M2** — Integration tests exercise the full HTTP→DB path against a real Postgres (Testcontainers).
- **NFR-M3** — Architecture rules (R1–R3) enforced by automated tests in CI.

---

## 12. Frontend architecture & UX requirements

### 12.1 Stack

React SPA with **React Hook Form** (state), **Zod** (schema/validation via `zodResolver`), and **Tailwind CSS** (layout/theming). A single strictly-typed `MembershipFormData` (inferred from the Zod schema) holds all data client-side until submit.

### 12.2 Form mechanics

- **FE-1** — `useForm` configured with `mode: "onTouched"` (validate as users leave fields).
- **FE-2** — Step gating via `trigger(fieldsForStep)`; advance only when the current step's fields validate.
- **FE-3** — Default values initialize controlled inputs (no controlled/uncontrolled warnings).
- **FE-4** — "Same as current address" uses `setValue` to copy current→permanent and conditionally hides permanent inputs (FR-A1).
- **FE-5** — TIN/SSS masking integrated with RHF so the masked value still satisfies the Zod regex.
- **FE-6** — Dates use `dd/mm/yyyy` presentation, serialized to ISO-8601 for the API (FR-F2).
- **FE-7** — On submit, POST JSON to `/api/members`; render success/validation/error states distinctly; map server field errors (§9.3) back to inputs and surface the offending step (FR-W5).

### 12.3 Modern UX features (target set)

| Feature | Requirement | Benefit |
|---|---|---|
| Inline validation | Validate on blur/tab with clear pass/fail affordances. | Avoids end-of-form wall of errors. |
| Floating labels | Labels animate above input on focus. | Compact UI; field context retained. |
| Single Sign-On *(optional)* | "Continue with Google/Microsoft/Apple" to authenticate & prefill name/email. | Faster onboarding; ties submission to an identity. |
| Smart autofill | Leverage browser autocomplete tokens for address/contact. | Fewer keystrokes & typos. |
| Password toggle | *Only if account credentials are added.* | N/A unless auth fields introduced. |
| Dark mode | Honor system theme. | Accessibility & polish. |
| Async submission | Fetch/AJAX; no reload (FR-W4). | Fluid, app-like flow. |
| Progress bar | Step X of 5 always visible. | Sets expectation; reduces abandonment. |

> Password toggle / "confirm password" are only relevant if SSO is *not* used and the product introduces local credentials. The current data set has no password field.

### 12.4 Frontend stack table

| Concern | Choice |
|---|---|
| Framework | React (SPA) |
| Form state | React Hook Form |
| Schema / validation | Zod (`@hookform/resolvers/zod`) |
| Styling | Tailwind CSS |
| Dates | HTML5 `type="date"` or accessible date-picker primitive |
| Masking | RHF-compatible masking utility for TIN/SSS |
| Auth (optional) | OIDC provider(s) for SSO |

---

## 13. Acceptance criteria

**E1 — Self-registration**
- AC1.1 The form renders as 5 steps with a visible progress indicator.
- AC1.2 "Next" is blocked until the current step's fields pass validation; the message identifies the offending fields.
- AC1.3 Final submit is disabled until the full schema validates.

**E2 — Resilient data entry**
- AC2.1 Navigating Back/Next never clears entered data.
- AC2.2 Checking "same as current address" hides and auto-populates the permanent address; unchecking restores editability.
- AC2.3 Validation appears on field blur, not only on submit.

**E3 — Record retrieval**
- AC3.1 `GET /api/members/{id}` returns the member in the `Result<T>` envelope or `404` for unknown ids.
- AC3.2 `GET /api/members` returns a paged result with working filters.

**E4 — Record correction**
- AC4.1 `PUT /api/members/{id}` updates the record and the audit interceptor stamps `UpdatedOn/By` automatically.

**E5 — Lawful processing**
- AC5.1 Submission is rejected unless `consentGiven = true`.
- AC5.2 TIN/SSS do not appear in clear text in any application log.
- AC5.3 Sensitive fields are stored encrypted/tokenized.

**E6 — Operability**
- AC6.1 `/health/live` returns healthy while the process is up.
- AC6.2 `/health/ready` returns unhealthy when the database is unreachable and healthy when restored.
- AC6.3 Each request emits structured start/finish logs with a correlation id and duration.

**E7 — Member self-view**
- AC7.1 A user with the Member role can retrieve only his/her own member detail by authenticated identity.
- AC7.2 A user with the Member role receives `403 Forbidden` when requesting the member list endpoint.
- AC7.3 A user with the Member role receives `403 Forbidden` when requesting another member's detail endpoint.
- AC7.4 A user with the HR/Admin role can retrieve the member list and any member detail endpoint.

**Cross-cutting**
- AC-X1 Architecture tests confirm inward-only dependencies (R1).
- AC-X2 An invalid command never reaches its handler (R5) and returns the structured `400` envelope (§9.3).
- AC-X3 Scalar UI lists all endpoints and schemas in non-prod (R12).

---

## 14. Delivery phases

| Phase | Scope | Exit criteria |
|---|---|---|
| **P0 — Foundations** | Solution skeleton (R1–R4), `Result<T>`, DbContext + first migration (R11), health checks (R10), Scalar (R12), CI with arch tests. | Empty-but-wired API runs; probes & docs work. |
| **P1 — Register member** | `RegisterMember` slice end-to-end: validation pipeline (R5), audit interceptor (R7), logging (R6), global exception handling (R8). | A valid member persists; invalid returns structured `400`; audit fields populate. |
| **P2 — Read & manage** | `GetMemberById`, `ListMembers` (paged/filtered), `UpdateMember`. | Admin read/search/update flows pass integration tests. |
| **P3 — Frontend wizard** | React + RHF + Zod + Tailwind 5-step wizard; same-as-address toggle; masking; async submit + error mapping. | UX acceptance criteria (E1–E2) pass; WCAG 2.1 AA checks pass. |
| **P4 — Privacy & hardening** | Field-level encryption for sensitive PII, consent capture, RBAC, rate limiting, retention policy, access logging. | Security/privacy NFRs (§11.3–11.4) met; DPO sign-off. |
| **P5 — Admin UI & Authentication** | Landing page, login page, JWT token issuance (`POST /api/auth/login`), admin user store, member list page with pagination/filtering, member detail page, route guards, role distinction (Admin vs HRAdmin). | Admin can log in, view paginated member list, and open member detail. |
| **P6 (candidate)** | SSO accelerator, ID-image upload, approval workflow, soft-delete/erasure. | Per separate scope. |

---

## 15. Risks & assumptions

| Risk / assumption | Impact | Mitigation |
|---|---|---|
| **Unlabeled name blocks** misinterpreted (§7.3) | Wrong data model / fields | Confirm with form owner before P1 (Open Q1). |
| Sensitive PII (TIN/SSS/religion) handling non-compliant | Legal/NPC exposure | P4 privacy controls; DPO review; minimize religion. |
| MediatR commercial licensing | Cost / dependency | Confirm library decision (§10.3) in P0. |
| Scope creep from brief's "Payment/Account" steps | Schedule | Hold to actual form fields; Product must add fields explicitly. |
| `dd/mm/yyyy` vs ISO confusion | Data corruption | Single rule (VR-1): ISO on the wire, format on display. |
| Duplicate-detection key undecided | Data quality | Decide uniqueness key (email? TIN?) (Open Q3). |
| **Assumption:** currency is PHP; member ≥ 18 | Validation rules | Confirm thresholds (Open Q4). |

---

## 16. Open questions (require confirmation)

1. **Related-person blocks (§7.3):** confirm the semantic labels of the three unlabeled name blocks (Spouse / Mother's maiden name / Father — or other, e.g. beneficiaries) and their conditionality.
2. **`employeeLevel` codes:** confirm the full meaning of `PTS` and `RNF` and whether other levels exist.
3. **Uniqueness / duplicate key:** is a member uniquely identified by email, TIN, company ID, or a combination? This drives the `409 Conflict` rule.
4. **Business thresholds:** minimum age (assumed 18), currency (assumed PHP), and whether TIN/SSS are mandatory for all member types.
5. **Religion field:** is it genuinely required? It is Sensitive PII — confirm purpose or drop.
6. **Account/credentials & payment:** does the product require local accounts (passwords), SSO, and/or a payment step beyond the source form?
7. **ID-image upload:** should the primary government ID be uploaded as an image/scan (v1.1)?
8. **Approval workflow:** is post-submission routing/approval in scope, or is the `status` field sufficient for now?
9. **Mediation library:** MediatR (commercial) vs lightweight alternative — confirm the decision and budget.

---

## Appendix A — Sample submit payload (illustrative)

```json
{
  "personalInfo": {
    "title": "Mr.",
    "firstName": "Juan", "middleName": "Protacio", "lastName": "Dela Cruz", "suffix": "",
    "alias": "", "dateOfBirth": "1990-05-12", "placeOfBirth": "Manila",
    "countryOfBirth": "PH", "nationality": "Filipino", "gender": "Male",
    "civilStatus": "Married", "religion": "", "highestEducationalAttainment": "Bachelor's",
    "numberOfDependents": 2
  },
  "contactInfo": { "emailAddress": "juan@example.com", "contactNumber": "+639170000000" },
  "relatedPersons": {
    "spouse": { "firstName": "Maria", "middleName": "Reyes", "lastName": "Dela Cruz" },
    "motherMaidenName": "Ana Bautista Reyes",
    "father": { "firstName": "Pedro", "middleName": "Santos", "lastName": "Dela Cruz", "suffix": "Sr." }
  },
  "governmentIds": { "tin": "123-456-789-000", "sss": "01-2345678-9" },
  "primaryId": {
    "type": "Passport", "number": "P1234567A",
    "issueDate": "2021-01-10", "expiryDate": "2031-01-09", "issueCountry": "PH"
  },
  "currentAddress": {
    "streetNameAndNumber": "123 Mabini St.", "city": "Navotas", "postalCode": "1485",
    "barangay": "San Roque", "subdivisionPurok": "Purok 2", "province": "Metro Manila",
    "country": "PH", "ownerOrLessee": "Owner", "occupiedSince": "2015-06-01"
  },
  "permanentAddress": { "sameAsCurrent": true },
  "emergencyContact": { "contactName": "Maria Dela Cruz", "relationship": "Spouse", "contactNumber": "+639170000001" },
  "employment": {
    "employeeLevel": "RNF", "companyTradeName": "OPTODEV, INC.", "companyIdNumber": "OPT-00421",
    "grossIncome": 45000, "incomePeriod": "Monthly", "occupation": "Technician",
    "hiredFrom": "2019-12-01", "hiredTo": null
  },
  "consent": { "consentGiven": true, "attestation": true, "signatureName": "Juan P. Dela Cruz" }
}
```

## Appendix B — Glossary

| Term | Meaning |
|---|---|
| CQRS | Command Query Responsibility Segregation |
| Vertical Slice | Feature-first code organization; each use case self-contained |
| `Result<T>` | Envelope representing success/failure without exceptions for expected paths |
| PII | Personal / Sensitive Personal Information (per RA 10173) |
| RA 10173 | Philippine Data Privacy Act of 2012 (NPC-administered) |
| TIN | Tax Identification Number |
| SSS | Social Security System (PH) number |
| Scalar | Modern interactive OpenAPI documentation UI |
| NPC | National Privacy Commission (PH) |

---

*Source artifact: `Member_Information_Form.xlsx` (Sheet1). Fields marked **[INFERRED]** require confirmation per §16 before implementation.*
