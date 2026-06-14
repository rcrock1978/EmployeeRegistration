# Agent Notes — MembershipRegistration

<!-- SPECKIT START -->
For the latest implementation plan and feature context, read `specs/001-member-registration/plan.md`. The plan was refreshed on 2026-06-15 with clarifications from `/speckit.clarify` (concurrency handling, primary ID types, admin-create endpoint, status lifecycle). The authoritative product spec remains `../PRD-Member-Registration.md` in the parent directory (`Porfolio_001/`).
<!-- SPECKIT END -->

## Repo state

- **Implementation-ready.** The spec, plan, data model, contracts, tasks, and quality checklists are complete in `specs/001-member-registration/`. No source code has been written yet.
- Do **not** run `dotnet build`, `npm install`, `npm run dev`, or similar until `/speckit.implement` begins generating code — they will fail.

## Source of truth

- `../PRD-Member-Registration.md` — authoritative product spec.
- `../Member Information Form.xlsx` — source artifact the PRD was derived from.
- When the two conflict: prefer the Excel file for raw field order/format; prefer the PRD for architecture and behavior intent.

## Speckit workflow tooling

- This repo has `.opencode/` and `.specify/` directories installed by speckit. They provide planning/implementation commands and templates.
- Do **not** manually edit the `<!-- SPECKIT START -->` / `<!-- SPECKIT END -->` markers above; the `speckit.agent-context.update` extension refreshes them.
- `.specify/memory/constitution.md` is the ratified project constitution; `/speckit.plan` and `/speckit.implement` must verify adherence to every principle.

## Before generating code

- Fields marked **[INFERRED]** in PRD §7.3 and §7.4 may still need confirmation from the form owner (see PRD §16). The spec's Assumptions section documents these.
- Ask the user before scaffolding a solution or choosing implementation libraries.
- If asked to start implementation, begin with the **P0 foundation phase** described in PRD §14.

## Planned architecture (not implemented)

- Backend: .NET 10 Minimal APIs, Clean Architecture + Vertical Slice + CQRS, PostgreSQL + EF Core 10, FluentValidation, native OpenAPI + Scalar.
- Frontend: React SPA with React Hook Form + Zod + Tailwind CSS.
- Target endpoints (from PRD §9):
  - `POST /api/members`
  - `GET /api/members/{id}`
  - `GET /api/members`
  - `PUT /api/members/{id}`
  - `GET /health/live`, `GET /health/ready`
