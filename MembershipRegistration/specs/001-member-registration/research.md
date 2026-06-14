# Research Notes: Member Registration Platform

## Decision 1: CQRS mediator library

**Decision**: Use a custom lightweight `ISender`/`IPipelineBehavior` abstraction instead of MediatR.

**Rationale**:
- Avoids MediatR's recent commercial licensing uncertainty and cost.
- Aligns with Principle IX (Simplicity & YAGNI): the project only needs request dispatching and two pipeline behaviors (validation, logging).
- A custom implementation is ~200 lines, fully testable, and keeps no external dependency that could change license terms.
- Preserves the Vertical Slice + CQRS intent without coupling to a third-party mediator's conventions.

**Alternatives considered**:
- **MediatR**: Industry standard, rich ecosystem, but licensing model recently shifted to commercial terms; budget/ops decision required.
- **Wolverine**: Full-featured, adds messaging and transport concerns beyond current scope; overkill for a single bounded context.
- **Source-generated mediators (e.g., Jab, Mediator.SourceGenerator)**: Reduce reflection but add build-time complexity and narrower tooling support.

## Decision 2: Authentication and role authorization

**Decision**: Externalize authentication to the hosting identity provider (OIDC/JWT) and enforce role-based authorization through standard claims.

**Rationale**:
- The spec explicitly states (FR-021) that the system consumes role and identity claims from the hosting identity mechanism rather than managing its own authentication.
- Keeps the platform aligned with corporate identity providers (Azure AD, Okta, Keycloak) and SSO accelerators mentioned in the PRD.
- The API validates a signed JWT containing a `role` claim (`Member` or `HRAdmin`) and a `sub` claim mapping to the user's identity; the member self-view links `sub` to the submitted member record via the user's email or subject identifier.

**Required OIDC configuration** (provided by the hosting identity provider, not stored in source):
- Issuer URL (`iss`) and expected audience (`aud`).
- JWKS endpoint for signature validation; keys are refreshed on a configurable interval (default 24 hours).
- Token expiry handling via standard `exp` claim validation with a short clock skew tolerance (default 5 minutes).
- The `sub` claim is treated as the user identifier; the `role` claim MUST be present and contain either `Member` or `HRAdmin`.

**Alternatives considered**:
- **Built-in ASP.NET Core Identity**: Adds password management, token, and account tables, contradicting the spec's external-auth requirement.
- **Custom identity microservice**: Adds operational complexity and network dependency not justified for v1.

## Decision 3: Sensitive field protection at rest

**Decision**: Encrypt sensitive scalar values (TIN, SSS, primary ID number) using AES-256-GCM with keys stored in a secrets manager; access logging captured for any read of sensitive fields.

**Rationale**:
- Satisfies RA 10173 and Principle V (Security & Privacy by Default).
- AES-256-GCM provides authenticated encryption; key rotation is managed outside the application.
- Access logging is implemented at the query/handler level for sensitive-field reads, not for every field, to balance observability and noise.

**Alternatives considered**:
- **Tokenization/Vault storage**: Stronger but adds external vault dependency and latency; defer to v1.1 if DPO requires it.
- **Column-level transparent data encryption (TDE) in PostgreSQL**: Platform-level but does not provide application-level access logging; use as defense-in-depth only.

## Decision 4: Frontend build tooling

**Decision**: Use Vite for the React SPA.

**Rationale**:
- Fast dev server and optimized production builds.
- First-class TypeScript and React support.
- Simpler configuration than Create React App and aligns with modern React ecosystem.

**Alternatives considered**:
- **Create React App**: Official but slower, deprecated in practice, larger bundle defaults.
- **Next.js**: Adds server-side rendering and routing complexity not needed for a single-form wizard SPA.

## Decision 5: Optimistic concurrency for member updates

**Decision**: Use EF Core `RowVersion` concurrency token on the `Member` aggregate. `PUT /api/members/{id}` requires the caller to supply the current `RowVersion`; if the stored token has changed, the update returns `409 Conflict`.

**Rationale**:
- Prevents silent last-writer-wins overwrites when two HR/Admin users edit the same record concurrently.
- Native EF Core support (`[Timestamp]` / `IsRowVersion()`) keeps the implementation simple and aligns with the spec clarification (Session 2026-06-15).
- The 409 response is already part of the uniform `Result<T>` envelope.

**Alternatives considered**:
- **Last-writer-wins**: Simpler but loses changes without warning, violating data-stewardship expectations.
- **Pessimistic locking**: Adds complexity and connection-state management; overkill for the expected low-contention admin workflow.
