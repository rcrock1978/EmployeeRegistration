# Member API Contracts

Base path: `/api/members`

Authentication: Bearer JWT with `sub` and `role` claims (`Member` or `HRAdmin`).

> **Role name convention**: prose uses "HR/Admin"; the JWT `role` claim and code use `HRAdmin`.

## Endpoints

### Register a member

- **Method**: `POST`
- **Route**: `/api/members`
- **Authorization**: Accepts an unauthenticated applicant request, a `Member`-role JWT, or an `HRAdmin`-role JWT. An `HRAdmin` JWT creates a record on behalf of another person; an applicant/Member JWT creates their own record.
- **Request**: See [register-member-request.md](register-member-request.md)
- **Success**: `201 Created` + `Result<MemberSummaryResponse>` + `Location: /api/members/{id}`
- **Failures**: `400` validation, `409` duplicate email

### Get member by id

- **Method**: `GET`
- **Route**: `/api/members/{id}`
- **Authorization**:
  - `HRAdmin`: any id
  - `Member`: only his/her own record; otherwise `403 Forbidden`
- **Success**: `200 OK` + `Result<MemberDetailResponse>`
- **Failures**: `403`, `404`

### List/search members

- **Method**: `GET`
- **Route**: `/api/members`
- **Authorization**: `HRAdmin` only; `Member` receives `403 Forbidden`
- **Query parameters**:
  - `page` (int, default 1)
  - `pageSize` (int, default 20, max 100)
  - `lastName` (string, optional) — case-insensitive partial match (`ILIKE`)
  - `email` (string, optional) — case-insensitive partial match (`ILIKE`)
  - `employeeLevel` (string, optional) — exact match
  - `createdFrom` (ISO date, optional) — inclusive
  - `createdTo` (ISO date, optional) — inclusive
- **Success**: `200 OK` + `Result<PagedList<MemberSummaryResponse>>`
- **Empty state**: When no members match, returns `{ items: [], totalCount: 0, page: 1, pageSize: 20 }` with HTTP 200.
- **Failures**: `403`

#### Paged list shape

```json
{
  "isSuccess": true,
  "value": {
    "items": [
      { /* MemberSummaryResponse */ }
    ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 20
  },
  "error": null
}
```

### Update member

- **Method**: `PUT`
- **Route**: `/api/members/{id}`
- **Authorization**: `HRAdmin` only
- **Request**: Same shape as register request plus `RowVersion` (byte array, concurrency token); omitted fields are not changed (full update of provided sections)
- **Success**: `200 OK` + `Result<MemberSummaryResponse>`
- **Failures**: `400`, `403`, `404`, `409` concurrency conflict (RowVersion mismatch)

## Response DTOs

### `MemberSummaryResponse`

```json
{
  "id": "guid",
  "fullName": "Juan P. Dela Cruz",
  "emailAddress": "juan@example.com",
  "status": "Submitted",
  "createdOn": "2026-06-15T09:30:00Z"
}
```

The success envelope also carries a `correlationId` header (`X-Correlation-Id`) for request tracing.

### `MemberDetailResponse`

Full member aggregate including all value objects. Sensitive fields are decrypted for authorized viewers only.

```json
{
  "id": "guid",
  "status": "Submitted",
  "personName": { /* PersonName */ },
  "demographics": { /* Demographics */ },
  "contactDetails": { /* ContactDetails */ },
  "dependentInfo": { /* DependentInfo */ },
  "relatedPersons": { /* RelatedPersons */ },
  "governmentIds": { /* GovernmentIds */ },
  "primaryIdentification": { /* PrimaryIdentification */ },
  "currentAddress": { /* Address */ },
  "permanentAddress": { /* Address or null with sameAsCurrent flag */ },
  "emergencyContact": { /* EmergencyContact */ },
  "employment": { /* EmploymentDetails */ },
  "consent": { /* Consent */ },
  "rowVersion": "AAAAAAAAB9E=",
  "createdOn": "2026-06-15T09:30:00Z",
  "updatedOn": "2026-06-15T09:30:00Z"
}
```
