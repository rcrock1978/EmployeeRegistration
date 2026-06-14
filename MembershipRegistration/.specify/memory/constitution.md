<!--
SYNC IMPACT REPORT
Version change: 0.0.0-placeholder -> 1.0.0
Modified principles: All principles replaced from placeholder tokens to ratified text
Added sections:
  - I. Architectural Integrity
  - II. Feature-First Organization
  - III. Explicit, Consistent Outcomes
  - IV. Validation at the Trust Boundary
  - V. Security & Privacy by Default
  - VI. Observability & Operability
  - VII. Quality via Tests
  - VIII. Accessible, Low-Friction UX
  - IX. Simplicity & YAGNI
  - X. Contract Documentation
  - Technology Constraints
  - Governance
Removed sections: None (previous file was a template with placeholders)
Templates requiring updates:
  - .specify/templates/plan-template.md: Constitution Check section remains generic; no change required
  - .specify/templates/spec-template.md: No direct constitution references; no change required
  - .specify/templates/tasks-template.md: No direct constitution references; no change required
  - AGENTS.md: Updated to reflect constitution is now ratified
Follow-up TODOs: None
-->

# OPTODEV Member Registration Constitution

## Core Principles

### I. Architectural Integrity

The system MUST follow Clean Architecture with concentric layers ordered inward:
Domain → Application → Infrastructure → WebApi. Dependencies point inward only;
Domain depends on nothing. Abstractions and ports live in the inner layers, while
technology adapters live only in Infrastructure. Automated architecture tests in CI
enforce this rule; any violation fails the build.

**Rationale**: Inward-only dependencies keep business rules isolated from framework
and infrastructure churn, making the system testable, replaceable, and long-lived.

### II. Feature-First Organization

Every use case MUST be organized as a self-contained Vertical Slice with CQRS.
Each slice co-locates its command or query, handler, validator, response DTO, and
endpoint. Commands mutate state and return a Result; queries are read-only
projections and MUST never leak domain entities. Adding a feature means adding a
folder, not editing layer-wide files. Cross-feature coupling is prohibited.

**Rationale**: Feature-first organization keeps a single change localized, reduces
merge conflicts, and makes each use case independently understandable and testable.

### III. Explicit, Consistent Outcomes

Expected failures MUST be returned as values through the Result<T> pattern; they
MUST NOT be thrown as exceptions for control flow. Every API endpoint MUST return
one uniform response envelope with a consistent error model, and HTTP status code
mapping MUST be centralized. Unhandled errors flow through a single global
exception handler and MUST NOT leak stack traces or internal details outside
development environments.

**Rationale**: A single, predictable outcome model makes error handling explicit at
the type level and prevents accidental information disclosure.

### IV. Validation at the Trust Boundary

The server is the sole authority on validity. Every command MUST pass through an
automatic validation pipeline before its handler executes; invalid input
short-circuits to a structured error response. Client-side validation exists only
to improve UX and MUST mirror server rules; no business rule MAY live only on the
client.

**Rationale**: Client code can be bypassed, so the trust boundary must sit on the
server where every input is verified before processing.

### V. Security & Privacy by Default

The system handles Sensitive Personal Information under the Philippine Data
Privacy Act of 2012 (RA 10173). Sensitive data MUST be encrypted at rest, MUST
NOT be written to logs in clear text, MUST be access-controlled through RBAC, and
MUST be access-logged. Explicit opt-in consent, data minimization, and a defined
retention and erasure policy are mandatory. Secrets MUST come from a secrets
manager and MUST NOT be stored in source or configuration files. This principle
overrides convenience and cannot be waived.

**Rationale**: Privacy and security are legal and ethical prerequisites when
processing SPI; treating them as defaults prevents compliance debt and breach risk.

### VI. Observability & Operability

Every request MUST emit structured start/finish traces that include a correlation
ID and automatic PII redaction. Audit metadata (CreatedOn/By, UpdatedOn/By) MUST
be applied automatically at the persistence layer, not by hand in handlers.
Liveness and readiness health probes MUST be exposed, and readiness MUST reflect
datastore reachability so traffic is never routed to a broken instance.

**Rationale**: Observable, self-auditing systems let operators detect, diagnose,
and recover from failures before users are impacted.

### VII. Quality via Tests

Domain and Application layers MUST be unit-testable without infrastructure.
Integration tests MUST exercise the full HTTP→database path against a real
database instance. Test-first development (TDD) is the default where practical.
No feature MAY merge without accompanying tests and green quality gates: build,
tests, and architecture checks.

**Rationale**: Comprehensive, fast tests are the only practical way to preserve
correctness as the system evolves and to prevent regressions at the trust boundary.

### VIII. Accessible, Low-Friction UX

The registration experience MUST use single-column progressive disclosure through
a multi-step wizard with a visible progress indicator. Validation MUST appear on
field exit, and submission MUST be asynchronous with no page reload. The UI MUST
meet WCAG 2.1 AA, MUST be mobile-first with touch targets at least 44×44 px and
correct input keyboards, MUST support dark mode, and MUST use an obvious,
visually distinct primary call-to-action. UX consistency is a first-class
requirement, not a nice-to-have.

**Rationale**: A forgiving, accessible form reduces abandonment and ensures every
applicant, regardless of device or ability, can complete registration.

### IX. Simplicity & YAGNI

The design MUST prefer the simplest solution that satisfies the specification.
No speculative abstractions, layers, or components that are not traceable to an
explicit requirement are permitted. Any added complexity MUST be justified
against these principles; over-engineering is treated as a defect.

**Rationale**: Unnecessary complexity slows delivery, increases failure modes, and
makes the system harder to secure, test, and operate.

### X. Contract Documentation

Public API contracts MUST be documented through natively generated OpenAPI and
rendered via an interactive docs UI in non-production environments. The
specification is the single source of truth for the data model.

**Rationale**: Living, interactive documentation keeps consumers, testers, and
operators aligned with the actual contract without duplicating effort.

## Technology Constraints

These technology choices are committed for this project because they serve the
principles above. Concrete versions are ratified in the implementation plan.

- **Backend**: .NET 10 / ASP.NET Core Minimal APIs, EF Core 10 with PostgreSQL,
  FluentValidation for the validation pipeline, Scalar-based OpenAPI UI.
- **Frontend**: React with React Hook Form, Zod, and Tailwind CSS.
- **CQRS Mediation**: The mediation library for the CQRS pipeline is selected in
the plan, accounting for current licensing of candidate libraries.

## Governance

This constitution supersedes ad-hoc practices. Plans, tasks, and implementations
that conflict with it MUST be revised; the constitution is not bent to fit a
deviation.

- **Amendments**: Any change requires a stated rationale and a semantic-version
  bump of the constitution.
  - **MAJOR**: principle removal or redefinition.
  - **MINOR**: new principle or materially expanded guidance.
  - **PATCH**: clarification, wording improvement, or non-semantic refinement.
- **Ratification**: Each amendment MUST include a ratification date.
- **Compliance Review**: The `/speckit.plan` and `/speckit.implement` phases MUST
  explicitly verify adherence to every principle and surface any deviation as a
  blocking item.

**Version**: 1.0.0 | **Ratified**: 2026-06-15 | **Last Amended**: 2026-06-15
