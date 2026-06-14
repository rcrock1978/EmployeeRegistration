# Quickstart Validation Guide

This guide proves the Member Registration platform works end-to-end after implementation.

## Prerequisites

- .NET 10 SDK installed
- Node.js 20+ installed
- PostgreSQL 15+ running locally or via Docker
- `dotnet-ef` tool available
- A configured JWT/OIDC test issuer for local development (see research.md)

## 1. Start the Backend

```bash
cd src/Members.WebApi
dotnet restore
dotnet ef database update
dotnet run
```

Expected: API listens on `https://localhost:5001` (or configured port).

## 2. Verify Health Probes

```bash
curl https://localhost:5001/health/live
# Expected: 200 OK

curl https://localhost:5001/health/ready
# Expected: 200 OK when DB is reachable
```

## 3. Register a Member

```bash
curl -X POST https://localhost:5001/api/members \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <any-authenticated-token>" \
  -d @register-payload.json
```

Use the sample payload in [contracts/register-member-request.md](contracts/register-member-request.md).

Expected: `201 Created` with `Location: /api/members/{id}` and a `Result<MemberSummaryResponse>` body. The response includes an `X-Correlation-Id` header.

## 4. Register with All Optional Fields Blank

Submit a payload with all optional fields omitted or empty (middle name, suffix, alias, religion, father, mother maiden name, subdivision, hiredTo, permanent address with `sameAsCurrent: true`).

Expected: `201 Created` — the system accepts the submission as long as all required fields are valid and consent is given.

## 5. Validate Duplicate Rejection

Repeat the same POST.

Expected: `409 Conflict` because the email address is already registered.

## 6. Admin List and Detail

Obtain an `HRAdmin` token and call:

```bash
# List
curl https://localhost:5001/api/members \
  -H "Authorization: Bearer <hradmin-token>"

# Detail
curl https://localhost:5001/api/members/{id} \
  -H "Authorization: Bearer <hradmin-token>"
```

Expected: `200 OK` with paged list and full member detail.

## 7. Member Self-View

Obtain a `Member` token whose `sub` claim matches the registered email and call:

```bash
curl https://localhost:5001/api/members/{id} \
  -H "Authorization: Bearer <member-token>"
```

Expected: `200 OK` only for the member's own record.

## 8. Member Access Denied

With the same `Member` token, call:

```bash
curl https://localhost:5001/api/members \
  -H "Authorization: Bearer <member-token>"

curl https://localhost:5001/api/members/{other-member-id} \
  -H "Authorization: Bearer <member-token>"
```

Expected: `403 Forbidden` for both.

## 9. Verify Concurrent Update Conflict

```bash
# First HRAdmin reads the member to obtain RowVersion
MEMBER=$(curl -s https://localhost:5001/api/members/{id} \
  -H "Authorization: Bearer <hradmin-token>")
ROW_VERSION=$(echo $MEMBER | jq -r '.data.rowVersion')

# Two concurrent updates with the same RowVersion — the first succeeds, the second gets 409
curl -X PUT https://localhost:5001/api/members/{id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <hradmin-token>" \
  -d "{ \"rowVersion\": \"$ROW_VERSION\", \"status\": \"UnderReview\" }"
# Expected: 200 OK

curl -X PUT https://localhost:5001/api/members/{id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <hradmin-token>" \
  -d "{ \"rowVersion\": \"$ROW_VERSION\", \"status\": \"Approved\" }"
# Expected: 409 Conflict (stale RowVersion)
```

## 10. Verify Structured Error Response

Submit an invalid payload (missing consent, bad TIN format):

```bash
curl -X POST https://localhost:5001/api/members \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <any-authenticated-token>" \
  -d '{ "personalInfo": { "firstName": "A" } }'
```

Expected: `400 Bad Request` with `Result` envelope containing field-level errors.

## 11. Verify PII Redaction in Logs

Search application logs for the TIN/SSS values submitted in step 3.

Expected: No clear-text occurrences of TIN or SSS in logs.

## 12. Start the Frontend Wizard

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` and complete the 5-step wizard. Confirm:
- Progress indicator updates each step.
- Validation appears on field blur.
- "Same as current address" copies and hides permanent address.
- Submit is disabled until the full form is valid.
- Loading state appears during submission.
- Success confirmation screen renders without page reload.

## 13. Run Automated Quality Gates

```bash
dotnet build
dotnet test
```

Expected: All tests pass, including architecture tests that enforce inward-only dependencies.

---

> **Note on ordering**: Steps are sequenced for a practical end-to-end demo (health → register → admin/self-view → edge cases → frontend → quality gates), not strictly by spec user-story priority.
