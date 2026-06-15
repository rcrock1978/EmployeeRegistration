# OPTODEV Member Registration — Quickstart

## What this is

A full-stack member registration platform:

- **Backend**: .NET 10 ASP.NET Core Minimal APIs — Clean Architecture + CQRS + PostgreSQL, JWT auth (Admin/HRAdmin roles)
- **Frontend**: React 19 SPA — landing page, 5-step registration wizard, admin login, admin member list, member detail
- **Infrastructure**: Docker Compose (PostgreSQL + API + Frontend), Serilog logging, GitHub Actions CI

---

## Prerequisites

| Tool | Minimum | Check |
|------|---------|-------|
| Docker | 24+ | `docker --version` |
| .NET SDK | 10.0 | `dotnet --version` |
| Node.js | 20+ | `node --version` |
| npm | 10+ | `npm --version` |

---

## Quick start (Docker Compose)

This is the fastest way to get everything running — database, API, and frontend:

```bash
docker compose up --build
```

Then open **http://localhost:3000** for the landing page.

| Service | URL |
|---------|------|
| Frontend (Landing) | http://localhost:3000 |
| Registration Wizard | http://localhost:3000/register |
| Admin Login | http://localhost:3000/admin/login |
| Admin Member List | http://localhost:3000/admin/members |
| API | http://localhost:5001 |
| Scalar API docs | http://localhost:5001/scalar/v1 |
| Database | `localhost:5432` (postgres/postgres) |

On first startup, the API automatically:
1. Applies pending EF Core migrations
2. Seeds **1000 random member records** with realistic Filipino data
3. Seeds **2 admin accounts** (see credentials below)

> Logs are written to `logs/` in the project root (daily rolling file + per-member JSON).

---

## Local development (without Docker)

### 1. Start PostgreSQL

```bash
docker run -d --name optodev-pg \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=optodev_members \
  -p 5432:5432 \
  postgres:16-alpine
```

### 2. Run the API

```bash
dotnet run --project src/Members.WebApi
```

The API starts on **http://localhost:5001** and auto-migrates + seeds 1000 records on startup.

### 3. Run the frontend

```bash
cd frontend
npm install
npm run dev
```

Opens at **http://localhost:5173** (landing page). The Vite dev server proxies API calls to `http://localhost:5001`.

---

## Dev tokens (authentication)

The API uses a dev signing key (`appsettings.json` → `Authentication.UseDevelopmentKey: true`).
Generate tokens with this Python snippet or any JWT library:

```python
import jwt, time
claims = {
    "sub": "juan@email.com",
    "role": "HRAdmin",
    "email": "juan@email.com",
    "exp": int(time.time()) + 3600
}
token = jwt.encode(claims, "ThisIsADevelopmentSigningKeyThatIsAtLeast32Bytes!", algorithm="HS256")
print(token)
```

Or use the built-in helper from the test project:

```bash
# Generate an HRAdmin token (requires dotnet-script or running a quick console app)
dotnet run --project tests/Members.Api.IntegrationTests -token
```

Available roles:

| Role | Access |
|------|--------|
| `Admin` | List all members, view any detail, update records, manage admin user roles (superuser) |
| `HRAdmin` | List all members, view any detail (read-only, no update) |
| `Member` | View own record only (403 for list / other records) |

Without a token: all endpoints except `POST /api/members` and `POST /api/auth/login` return 401.
`POST /api/members` is anonymous (anyone can register).
`POST /api/auth/login` is anonymous (admin login).

### Dev admin credentials

In development mode, the following admin accounts are seeded:

| Email | Password | Role |
|-------|----------|------|
| `admin@optodev.com` | `Admin123!` | Admin (superuser — full access + role management) |
| `hradmin@optodev.com` | `HRAdmin123!` | HRAdmin (read-only) |

---

## API overview

### Endpoints

| Method | Route | Auth | Purpose |
|--------|-------|------|---------|
| `POST` | `/api/auth/login` | None | Admin login, returns JWT |
| `POST` | `/api/members` | None | Register a new member |
| `GET` | `/api/members/{id}` | Required | View member (admin or HRAdmin=any, member=own) |
| `GET` | `/api/members` | AdminOrHRAdmin | List members (paged, filterable) |
| `PUT` | `/api/members/{id}` | AdminOnly | Update a member record |
| `GET` | `/health/live` | None | Liveness probe |
| `GET` | `/health/ready` | None | Readiness probe (DB check) |

### Quick validation

```bash
# Health
curl http://localhost:5001/health/live
curl http://localhost:5001/health/ready

# Admin login
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@optodev.com", "password": "Admin123!"}'

# Register a member (anonymous)
curl -X POST http://localhost:5001/api/members \
  -H "Content-Type: application/json" \
  -d '{
    "personalInfo": {
      "title": "Mr.", "firstName": "Juan", "lastName": "Dela Cruz",
      "dateOfBirth": "1990-05-12", "placeOfBirth": "Manila",
      "countryOfBirth": "Philippines", "nationality": "Filipino",
      "gender": "Male", "civilStatus": "Single",
      "highestEducationalAttainment": "College Graduate",
      "numberOfDependents": 0
    },
    "contactInfo": {
      "emailAddress": "juan.delacruz@email.com",
      "contactNumber": "+639170000000"
    },
    "governmentIds": { "tin": "123-456-789-000", "sss": "01-2345678-9" },
    "primaryId": {
      "type": "Passport", "number": "P1234567A",
      "issueDate": "2020-01-10", "expiryDate": "2030-01-09",
      "issueCountry": "Philippines"
    },
    "currentAddress": {
      "streetNameAndNumber": "123 Rizal St.", "city": "Manila",
      "postalCode": "1000", "barangay": "Poblacion",
      "province": "Metro Manila", "country": "Philippines",
      "ownerOrLessee": "Owner", "occupiedSince": "2020-01-01"
    },
    "permanentAddress": { "sameAsCurrent": true },
    "emergencyContact": {
      "contactName": "Maria Santos", "relationship": "Spouse",
      "contactNumber": "+639170000001"
    },
    "employment": {
      "employeeLevel": "RNF", "companyTradeName": "OPTODEV Inc.",
      "companyIdNumber": "EMP-001", "grossIncome": 45000,
      "incomePeriod": "Monthly", "occupation": "Technician",
      "hiredFrom": "2019-06-01"
    },
    "consent": {
      "consentGiven": true, "attestation": true,
      "signatureName": "Juan Dela Cruz"
    }
  }'

# List members (requires Admin or HRAdmin token)
curl http://localhost:5001/api/members?page=1&pageSize=10 \
  -H "Authorization: Bearer <token>"

# Get member by ID (requires Admin or HRAdmin token)
curl http://localhost:5001/api/members/a2b8a3e6-... \
  -H "Authorization: Bearer <token>"
```

---

## Running tests

```bash
# All backend tests
dotnet test

# Architecture + domain unit tests
dotnet test tests/Members.Domain.UnitTests

# Application validator tests
dotnet test tests/Members.Application.UnitTests

# Integration tests (requires Docker — spins up Testcontainers PostgreSQL)
dotnet test tests/Members.Api.IntegrationTests

# Frontend tests
cd frontend && npm test

# Frontend type-check
cd frontend && npx tsc --noEmit
```

---

## Project structure

```
├── src/
│   ├── Members.Domain/        # Entities, value objects, enums (Member, AdminUser)
│   ├── Members.Application/   # CQRS commands/queries, validators, behaviors (Register, List, Auth)
│   ├── Members.Infrastructure/ # EF Core, encryption, JWT, logging, repositories
│   └── Members.WebApi/        # Minimal API endpoints, auth, middleware
├── tests/
│   ├── Members.Domain.UnitTests/      # 35 tests (architecture + value objects)
│   ├── Members.Application.UnitTests/ # 20 tests (validators)
│   └── Members.Api.IntegrationTests/  # 5 tests (HTTP → Postgres)
├── frontend/
│   ├── src/
│   │   ├── components/        # Landing, Login, Wizard, MemberList, MemberDetail
│   │   ├── contexts/          # AuthContext (JWT token, login/logout)
│   │   ├── hooks/             # useRegistration, useAuth
│   │   ├── lib/               # API client, schemas
│   │   └── types/             # TypeScript API types
│   └── Dockerfile             # Multi-stage Node + Nginx build
└── docker-compose.yml         # PostgreSQL + API + Frontend
```
