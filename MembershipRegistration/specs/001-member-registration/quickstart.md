# Quickstart Validation Guide

This guide validates the Member Registration platform end-to-end after implementation.

## Prerequisites

- Docker 24+ (for PostgreSQL and Compose)
- .NET 10 SDK (for local dev)
- Node.js 20+ (for frontend dev)

---

## 1. Start Everything (Docker Compose)

```bash
docker compose up --build
```

Expected:
- API listens on **http://localhost:5000**
- Frontend on **http://localhost:3000**
- Database on `localhost:5432`

On startup, the API automatically applies migrations and seeds **1000 random members**.

---

## 2. Verify Health Probes

```bash
curl http://localhost:5000/health/live
# Expected: 200 OK

curl http://localhost:5000/health/ready
# Expected: 200 OK when DB is reachable
```

Each response includes an `X-Correlation-Id` header.

---

## 3. Generate a Dev Token

Use the `DevTokenHelper` (dotnet script or test helper):

```bash
# Quick python one-liner:
python3 -c "
import jwt, time
t = jwt.encode({'sub':'admin@optodev.com','role':'HRAdmin','email':'admin@optodev.com','exp':int(time.time())+3600},
  'ThisIsADevelopmentSigningKeyThatIsAtLeast32Bytes!', algorithm='HS256')
print(t)
"
```

Set the output as `TOKEN` for subsequent steps.

---

## 4. Register a Member

```bash
curl -X POST http://localhost:5000/api/members \
  -H "Content-Type: application/json" \
  -d @- <<'JSON'
{
  "personalInfo": {
    "title": "Mr.", "firstName": "Juan", "lastName": "Dela Cruz",
    "dateOfBirth": "1990-05-12", "placeOfBirth": "Manila",
    "countryOfBirth": "Philippines", "nationality": "Filipino",
    "gender": "Male", "civilStatus": "Married",
    "highestEducationalAttainment": "College Graduate", "numberOfDependents": 2
  },
  "contactInfo": {
    "emailAddress": "juan.delacruz@email.com",
    "contactNumber": "+639170000000"
  },
  "relatedPersons": {
    "spouse": { "firstName": "Maria", "middleName": "Reyes", "lastName": "Dela Cruz" },
    "motherMaidenName": "Ana Bautista",
    "father": { "firstName": "Pedro", "lastName": "Dela Cruz", "suffix": "Sr." }
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
    "contactName": "Maria Dela Cruz", "relationship": "Spouse",
    "contactNumber": "+639170000001"
  },
  "employment": {
    "employeeLevel": "RNF", "companyTradeName": "OPTODEV Inc.",
    "companyIdNumber": "EMP-001", "grossIncome": 45000,
    "incomePeriod": "Monthly", "occupation": "Technician",
    "hiredFrom": "2019-06-01"
  },
  "consent": { "consentGiven": true, "attestation": true, "signatureName": "Juan Dela Cruz" }
}
JSON
```

Expected: `201 Created` with `Location: /api/members/{id}` and a `Result<RegisterMemberResponse>` envelope.

---

## 5. Register with All Optional Fields Blank

Submit a payload with optional fields omitted (middle name, suffix, alias, religion, father, mother maiden name, subdivision, hiredTo, permanent address with `sameAsCurrent: true`).

Expected: `201 Created` — the system accepts the submission as long as all required fields are valid and consent is given.

---

## 6. Validate Duplicate Rejection

Repeat the same POST from step 4.

Expected: `409 Conflict` because the email address is already registered.

---

## 7. Admin List and Detail

```bash
# List (paged — 1000 seed records should exist)
curl http://localhost:5000/api/members?page=1&pageSize=5 \
  -H "Authorization: Bearer $TOKEN"

# Detail (use an ID from the list)
curl http://localhost:5000/api/members/{id} \
  -H "Authorization: Bearer $TOKEN"
```

Expected: `200 OK` with paged list and full member detail. Sensitive fields (TIN, SSS) are decrypted in the response.

---

## 8. Member Self-View

Generate a Member token with `sub` matching the email from step 4, then:

```bash
curl http://localhost:5000/api/members/{id} \
  -H "Authorization: Bearer <member-token>"
```

Expected: `200 OK` only for the member's own record.

---

## 9. Member Access Denied

With the same Member token:

```bash
curl http://localhost:5000/api/members \
  -H "Authorization: Bearer <member-token>"

curl http://localhost:5000/api/members/{other-id} \
  -H "Authorization: Bearer <member-token>"
```

Expected: `403 Forbidden` for both (Member role cannot list or view other records).

---

## 10. Verify Concurrency Conflict

```bash
# Admin reads a member (xmin concurrency token is managed by PostgreSQL/EF Core)
# Two concurrent PUTs with version mismatch:

curl -X PUT http://localhost:5000/api/members/{id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"status": "UnderReview"}' \
  --data-binary @update-payload.json

# Repeat immediately — second gets 409 Conflict
```

Expected: First returns `200 OK`, second returns `409 Conflict` because the `xmin` row version changed.

---

## 11. Validate Structured Error Response

Submit an invalid payload (missing consent, bad data):

```bash
curl -X POST http://localhost:5000/api/members \
  -H "Content-Type: application/json" \
  -d '{ "personalInfo": { "firstName": "" } }'
```

Expected: `400 Bad Request` with `Result` envelope containing field-level errors.

---

## 12. Verify PII Redaction in Logs

Search `logs/` for the TIN/SSS values submitted in step 4.

Expected: No clear-text occurrences of TIN or SSS in any log file.

---

## 13. Start the Frontend Wizard (without Docker)

```bash
cd frontend
npm install
npm run dev
```

Open **http://localhost:5173** and complete the 5-step wizard. Confirm:
- Progress indicator updates each step.
- Validation appears on field blur.
- "Same as current address" copies and hides permanent address.
- Submit is disabled until the full form is valid.
- Loading state appears during submission.
- Success confirmation screen renders without page reload.
- Dark mode toggle works.

---

## 14. Run Automated Quality Gates

```bash
# Backend
dotnet build && dotnet test

# Frontend
cd frontend && npm test && npx tsc --noEmit && npm run build
```

Expected: All tests pass (67 total — architecture, validators, integration, frontend). Production bundle builds cleanly.

---

## Notes

- The API auto-migrates and seeds 1000 members on startup in Development mode only.
- Sensitive fields are encrypted at rest (AES-256-GCM). Logs have PII redaction for TIN, SSS, email, and phone.
- Optimistic concurrency uses PostgreSQL native `xmin` — no manual version tracking needed.
- All responses include `X-Correlation-Id` for request tracing.
