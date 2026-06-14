# Agent Notes — Porfolio_001

## Repo state

- This repository is currently **requirements-only**: it contains the source form and a PRD, but no source code, build system, tests, or package manifests yet.
- Do not run `dotnet build`, `npm install`, `npm run dev`, or similar — there is nothing to build or serve.

## Source of truth

- `PRD-Member-Registration.md` is the authoritative product spec.
- `Member Information Form.xlsx` is the source artifact the PRD was derived from.
- When docs conflict with the Excel form, prefer the Excel form for raw field order/format; prefer the PRD for architecture and behavior intent.

## Planned architecture (not implemented)

- Backend: .NET 10 Minimal APIs, Clean Architecture + Vertical Slice + CQRS, PostgreSQL + EF Core 10, FluentValidation, native OpenAPI + Scalar.
- Frontend: React SPA with React Hook Form + Zod + Tailwind CSS.
- Target endpoints (from PRD §9):
  - `POST /api/members`
  - `GET /api/members/{id}`
  - `GET /api/members`
  - `PUT /api/members/{id}`
  - `GET /health/live`, `GET /health/ready`

## Before generating code

- Several fields in §7.3 and §7.4 are marked **[INFERRED]** and need confirmation from the form owner (see PRD §16).
- Ask the user before scaffolding a solution or choosing implementation libraries.
- If asked to start implementation, begin with the P0 foundation phase described in PRD §14.
