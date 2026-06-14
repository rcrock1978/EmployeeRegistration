# Data Model: Member Registration Platform

## Aggregate Root

### `Member`

The `Member` aggregate is the single unit of consistency. It is identified by a server-generated GUID and owns all value objects and child entities related to a registration.

```text
Member
├── Id (GUID, server-generated)
├── RowVersion (byte[], concurrency token — auto-managed by EF Core)
├── Status (enum: Submitted, UnderReview, Approved, Rejected)
├── PersonName
├── Demographics
├── ContactDetails
├── DependentInfo
├── RelatedPersons
├── GovernmentIds
├── PrimaryIdentification
├── Address CurrentAddress
├── Address PermanentAddress
├── EmergencyContact
├── EmploymentDetails
├── Consent
└── AuditMetadata
```

## Value Objects / Owned Entities

### `PersonName`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `Title` | enum | Yes | `Miss`, `Mrs.`, `Mr.` |
| `FirstName` | string | Yes | 2–100 chars |
| `MiddleName` | string | No | ≤100 chars |
| `LastName` | string | Yes | 2–100 chars |
| `Suffix` | string | No | e.g. Jr., Sr., III |
| `Alias` | string | No | ≤100 chars |

### `Demographics`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `DateOfBirth` | date | Yes | `dd/mm/yyyy` display; age ≥ 18 |
| `PlaceOfBirth` | string | Yes | ≤200 chars |
| `CountryOfBirth` | string | Yes | ISO country code |
| `Nationality` | string | Yes | — |
| `Gender` | enum | Yes | configurable list |
| `CivilStatus` | enum | Yes | `Single`, `Married`, `Widowed`, `Separated`, ... |
| `Religion` | string | No | sensitive PII; confirm purpose before requiring |
| `HighestEducationalAttainment` | enum | Yes | configurable list |

### `ContactDetails`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `EmailAddress` | string | Yes | RFC-5322; unique across members |
| `ContactNumber` | string | Yes | E.164-compatible / PH mobile |

### `DependentInfo`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `NumberOfDependents` | int | Yes | ≥ 0 |

### `RelatedPersons`

| Block | Role | Required | Fields |
|---|---|---|---|
| Spouse | string | If `CivilStatus = Married` | firstName, middleName, lastName |
| MotherMaidenName | string | No | fullName |
| Father | object | No | firstName, middleName, lastName, suffix |

### `GovernmentIds` *(Sensitive PII)*

| Field | Type | Required | Constraints |
|---|---|---|---|
| `Tin` | string | Yes | `###-###-###-###` |
| `Sss` | string | Yes | `##-#######-#` |

### `PrimaryIdentification` *(Sensitive PII)*

| Field | Type | Required | Constraints |
|---|---|---|---|
| `Type` | enum | Yes | Passport, Driver's License, UMID, SSS ID, GSIS ID, PRC ID, Voter's ID, PhilHealth ID, National ID, Postal ID, Others |
| `Number` | string | Yes | per-type rules |
| `IssueDate` | date | Yes | ≤ today |
| `ExpiryDate` | date | Yes | > issueDate; ≥ today on submit |
| `IssueCountry` | string | Yes | ISO country code |

### `Address`

Applies to both `CurrentAddress` and `PermanentAddress`.

| Field | Type | Required | Constraints |
|---|---|---|---|
| `StreetNameAndNumber` | string | Yes | ≤200 chars |
| `City` | string | Yes | — |
| `PostalCode` | string | Yes | PH postal, 4 digits by default |
| `Barangay` | string | Yes | — |
| `SubdivisionPurok` | string | No | — |
| `Province` | string | Yes | — |
| `Country` | string | Yes | ISO country code |
| `OwnerOrLessee` | enum | Yes | `Owner`, `Lessee` |
| `OccupiedSince` | date | Yes | ≤ today |

### `EmergencyContact`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `ContactName` | string | Yes | — |
| `Relationship` | string | Yes | — |
| `ContactNumber` | string | Yes | E.164-compatible |

### `EmploymentDetails`

| Field | Type | Required | Constraints |
|---|---|---|---|
| `EmployeeLevel` | enum | Yes | `PTS`, `RNF` (confirm expansions and whether additional values exist with the form owner) |
| `CompanyTradeName` | string | Yes | default `OPTODEV, INC.` |
| `CompanyIdNumber` | string | Yes | — |
| `GrossIncome` | decimal | Yes | ≥ 0; PHP assumed |
| `IncomePeriod` | enum | Yes | `Annual`, `Monthly` |
| `Occupation` | string | Yes | — |
| `HiredFrom` | date | Yes | ≤ today |
| `HiredTo` | date | No | > hiredFrom if present |

### `Consent`

| Field | Type | Required | Notes |
|---|---|---|---|
| `ConsentGiven` | bool | Yes | explicit opt-in; default false |
| `Attestation` | bool | Yes | default false |
| `SignatureName` | string | Yes | typed full name as e-signature |
| `SignedAt` | datetime | Yes | server timestamp at submission |

### `AuditMetadata`

| Field | Type | Notes |
|---|---|---|
| `CreatedOn` | datetime | set by EF Core audit interceptor |
| `CreatedBy` | string | set by EF Core audit interceptor |
| `UpdatedOn` | datetime | set by EF Core audit interceptor |
| `UpdatedBy` | string | set by EF Core audit interceptor |

## Validation Rules

- All text fields accept Unicode input. Server-side validation truncates or rejects strings that exceed the documented maximum lengths; no client-side truncation is allowed.
- Required name fields: minimum length 2.
- `EmailAddress`: valid email format; uniqueness enforced at persistence.
- `DateOfBirth`: applicant must be ≥ 18 years old.
- `PrimaryIdentification.ExpiryDate` must be after `IssueDate`.
- `HiredTo` (if present) must be after `HiredFrom`.
- `OccupiedSince` and `HiredFrom` must be ≤ today.
- Spouse block is required only when `CivilStatus = Married`.
- Permanent address is required unless "same as current" is checked.
- `ConsentGiven` and `Attestation` must both be `true`.
- `NumberOfDependents` ≥ 0.
- `RowVersion`: managed by EF Core concurrency token; `PUT` returns `409 Conflict` on mismatch.

## State Transitions

```text
[Submitted] ──► [UnderReview] ──► [Approved]
                      │
                      ▼
                  [Rejected]
```

- New registrations default to `Submitted`.
- Status transitions are performed by HR/Admin via `PUT /api/members/{id}`.
- `UnderReview`, `Approved`, and `Rejected` are set by admin action in v1.
- Soft-delete/deactivate is a candidate for v1.1; hard delete is reserved for data-subject erasure requests.

## Uniqueness and Identity

- Primary key: server-generated GUID (`Id`).
- Duplicate detection: `EmailAddress` must be unique across members.
- Member self-view links authenticated user identity (JWT `sub` claim) to the member record via `EmailAddress` or subject identifier; the record can only be retrieved by its owner.

## Sensitive PII Classification

| Field / Group | PII Class |
|---|---|
| TIN, SSS | Sensitive |
| Primary ID number | Sensitive |
| Religion | Sensitive |
| Government ID type, issue/expiry dates | Personal |
| Names, birth info, contact info | Personal |
| Address details | Personal |
| Employment details | Personal |

Sensitive fields are encrypted at rest, access-controlled, and access-logged.
