# Agent Notes — Porfolio_001

## Repo state

- **P0–P4 complete** in `MembershipRegistration/` — .NET 10 REST API + React SPA wizard.
- **P5 (Admin UI & Auth)** : Requirements defined — not yet implemented.
- See `MembershipRegistration/specs/001-member-registration/` for full implementation plan and tasks.
- The `MembershipRegistration/` subdirectory has the full implemented codebase with build, tests, and CI pipeline.

## Source of truth

- `PRD-Member-Registration.md` is the authoritative product spec (updated with P5 admin UI & auth requirements in §§6.6–6.8).
- `Member Information Form.xlsx` is the source artifact the PRD was derived from.
- When docs conflict with the Excel form, prefer the Excel form for raw field order/format; prefer the PRD for architecture and behavior intent.

## Planned architecture (not implemented for P5)

- **P5 Backend additions**: `POST /api/auth/login`, `AdminUser` entity/table, JwtTokenService, admin seeding.
- **P5 Frontend additions**: Landing page, login page, admin member list (paginated + filtered), member detail page, React Router, auth context/guards.
