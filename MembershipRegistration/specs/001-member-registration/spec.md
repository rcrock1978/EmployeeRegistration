# Feature Specification: Member Registration Platform

**Feature Branch**: `001-member-registration`

**Created**: 2026-06-15

**Status**: Draft

**Input**: User description: "`/Users/rcrock1978/Documents/PROJECTS/Porfolio_001/PRD-Member-Registration.md`"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Complete member registration in a guided wizard (Priority: P1)

An applicant can enter their member information through a clear, multi-step wizard that breaks a dense form into manageable sections, validates input as they progress, and submits only when all required information is valid.

**Why this priority**: This is the core user journey; without it, the platform cannot collect member information.

**Independent Test**: A user can start a new registration, fill each step, and submit a valid application that creates a member record.

**Acceptance Scenarios**:

1. **Given** the applicant has started registration, **When** they view the wizard, **Then** they see a visible progress indicator showing numbered steps 1–5, the label of the current step, completed steps, and the upcoming steps.
2. **Given** the applicant is on any step, **When** they click "Next", **Then** the current step is validated and the applicant cannot advance until errors are resolved.
3. **Given** the applicant has completed all steps, **When** they click "Submit", **Then** the action is disabled until the entire form is valid, a loading state is shown while the submission is in flight, and the submission completes without reloading the page.
4. **Given** the submission fails server validation, **When** errors are returned, **Then** they are mapped back to the offending fields and steps so the applicant can correct them.

---

### User Story 2 - Resilient, low-friction data entry (Priority: P1)

Applicants can move back and forth through the wizard without losing data, copy their current address to their permanent address with a single toggle, and receive validation feedback as they leave each field.

**Why this priority**: Reduces abandonment and rework, especially on mobile devices where re-entering data is painful.

**Independent Test**: A user can partially fill the form, navigate back and forward, use the address toggle, and see inline validation messages on field exit.

**Acceptance Scenarios**:

1. **Given** the applicant has entered data on earlier steps, **When** they navigate backward and forward, **Then** all previously entered data remains intact.
2. **Given** the applicant has filled their current address, **When** they check "Permanent address same as current", **Then** the permanent address is populated and hidden; unchecking restores an editable permanent address. If the user checks the toggle again after manually editing the permanent address, the permanent address is re-copied from the current address.
3. **Given** the applicant leaves a field with invalid input, **When** the field loses focus, **Then** a clear, field-associated validation message appears.
4. **Given** the applicant is on a mobile device, **When** they focus email or phone fields, **Then** the appropriate on-screen keyboard is shown.

---

### User Story 3 - Lawful processing of personal information (Priority: P1)

The platform captures explicit consent, protects sensitive personal information, and rejects submissions that do not meet privacy requirements.

**Why this priority**: The form collects sensitive personal information under the Philippine Data Privacy Act of 2012 (RA 10173); lawful processing is non-negotiable.

**Independent Test**: A registration cannot be submitted without explicit consent, and sensitive identifiers are not exposed in clear text in logs or casual access.

**Acceptance Scenarios**:

1. **Given** the applicant has filled the entire form, **When** they attempt to submit without checking the privacy consent and attestation boxes, **Then** the submission is rejected.
2. **Given** sensitive identifiers are submitted, **When** system logs are reviewed, **Then** those values do not appear in clear text.
3. **Given** an admin attempts to view sensitive fields, **When** access is granted, **Then** the access is logged for accountability.

---

### User Story 4 - Retrieve and correct member records (Priority: P2)

An HR or membership admin can look up a member by identifier and list members with filtering, then update a record when corrections are needed.

**Why this priority**: Enables downstream review and data stewardship after submission.

**Independent Test**: An admin can search for a member, open a record, and save a correction that is reflected on subsequent reads.

**Acceptance Scenarios**:

1. **Given** a member record exists, **When** an admin requests it by identifier, **Then** the record is returned or a clear "not found" indication is shown.
2. **Given** multiple member records exist, **When** an admin lists members with filters, **Then** a paged result matching the filters is returned.
3. **Given** an admin updates an existing record, **When** the update is saved, **Then** the record reflects the change and audit metadata captures who made the change and when.

---

### User Story 5 - Member self-view (Priority: P2)

A registered member can view his/her own submitted member details, but cannot access the member list or another member's records. HR/Admin users retain full list and detail access.

**Why this priority**: Satisfies privacy expectations and least-privilege access under RA 10173 by ensuring members cannot browse other members' personal information.

**Independent Test**: A user authenticated as a Member can open only his/her own detail page; attempts to open the list page or another member's detail page are denied.

**Acceptance Scenarios**:

1. **Given** a user is authenticated with the Member role, **When** he/she requests his/her own member detail, **Then** the record is returned.
2. **Given** a user is authenticated with the Member role, **When** he/she requests the member list, **Then** the request is rejected with an access-denied response.
3. **Given** a user is authenticated with the Member role, **When** he/she requests another member's detail, **Then** the request is rejected with an access-denied response.
4. **Given** a user is authenticated with the HR/Admin role, **When** he/she requests the member list or any member detail, **Then** the requested data is returned.

---

### User Story 6 - Operability and health visibility (Priority: P3)

Platform operators can verify that the service is alive and ready to receive traffic, and every request leaves a traceable audit trail.

**Why this priority**: Supports reliable operations and incident response in production.

**Independent Test**: A non-functional probe confirms liveness/readiness, and request traces include a correlation identifier.

**Acceptance Scenarios**:

1. **Given** the service is running, **When** a liveness probe is called, **Then** it returns a healthy response.
2. **Given** the datastore is unreachable, **When** a readiness probe is called, **Then** it returns an unhealthy response so traffic can be rerouted.
3. **Given** any request is processed, **When** logs are reviewed, **Then** structured start/finish entries include a correlation identifier.

---

### Edge Cases

- What happens when an applicant loses network connectivity during submission?
- How does the system handle a duplicate email address, tax identifier, or company ID?
- What happens if an admin tries to update a member identifier that no longer exists?
- How does the wizard behave when all optional fields are left blank?
- What happens when a member's primary identification has already expired?
- What happens when a Member-role user attempts to access the member list or another member's detail page?
- How does the system resolve identity and role claims when authentication is handled externally?
- How does the system handle two HR/Admin users updating the same member record concurrently?

## Wizard Steps

The registration wizard is organized into five steps, presented in this order:

1. **Personal Information** — name, demographics, contact, dependents, education
2. **Family / Related Persons** — spouse (conditional), mother, father
3. **Government IDs & Primary ID** — TIN, SSS, primary government ID details
4. **Residency** — current address and permanent address with "same as current" toggle
5. **Employment, Emergency Contact & Consent** — employment details, emergency contact, privacy consent, attestation, and review

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Applicants MUST be able to register through a multi-step wizard with a visible progress indicator showing numbered steps 1–5, the current step label, completed steps, and upcoming steps.
- **FR-002**: The wizard MUST validate the current step before allowing the applicant to advance.
- **FR-003**: The final submit action MUST be disabled until the entire form is valid.
- **FR-004**: Submission MUST occur asynchronously without reloading the page, with distinct loading, success, validation-failure, and server-error states.
- **FR-005**: The wizard MUST preserve entered data when the applicant navigates backward and forward.
- **FR-006**: A "Permanent address same as current" toggle MUST copy the current address into the permanent address and hide the permanent-address inputs while checked.
- **FR-007**: Validation feedback MUST appear when an applicant leaves a field, not only on submit.
- **FR-008**: Tax and social-security identifiers MUST be formatted and validated according to the defined patterns. TIN pattern: `^\d{3}-\d{3}-\d{3}-\d{3}$`. SSS pattern: `^\d{2}-\d{7}-\d{1}$`.
- **FR-009**: Dates MUST be presented to applicants consistently while being serialized correctly for backend processing. Display format: `MM/DD/YYYY` in the UI. Serialization format: ISO 8601 (`yyyy-MM-dd`) in API payloads.
- **FR-010**: Email and phone inputs MUST trigger the appropriate mobile keyboards.
- **FR-011**: Admins MUST be able to create (via the same `POST /api/members` endpoint as applicant registration, gated by HR/Admin role), retrieve by identifier, list/search, and update (including status transitions) member records.
- **FR-012**: List/search results MUST be paged and support filters such as last name, email, employment level, and created date range.
- **FR-013**: Submission MUST be rejected unless explicit privacy consent and attestation are provided.
- **FR-014**: Sensitive personal information MUST be protected at rest and MUST NOT appear in clear text in logs.
- **FR-015**: Access to sensitive fields MUST be access-controlled and logged.
- **FR-016**: Liveness and readiness health probes MUST be available, with readiness reflecting datastore reachability.
- **FR-017**: The system MUST support at minimum two roles: **Member** and **HR/Admin**.
- **FR-018**: An HR/Admin user MUST be able to view the paged member list and open any member's detail page.
- **FR-019**: A Member user MUST be able to view only his/her own member detail page and MUST NOT access the member list or another member's detail page.
- **FR-020**: Requests by a Member user for the member list or another member's detail page MUST be rejected with an access-denied response.
- **FR-021**: The system MUST consume role and identity claims from the hosting identity mechanism (e.g., OIDC/SSO or corporate identity provider) rather than managing its own authentication.
- **FR-022**: After a successful submission, the applicant MUST see a confirmation screen that includes the generated member identifier or reference and clearly indicates that registration is complete.

### Key Entities *(include if feature involves data)*

- **Member**: The aggregate root representing a complete member record. It is identified by a server-generated GUID and has a `Status` property and a `RowVersion` concurrency token.
- **PersonName**: The member's full name, including title, first, middle, last, suffix, and alias.
- **Demographics**: Date and place of birth, nationality, gender, civil status, religion, and education.
- **ContactDetails**: Email address and contact number.
- **DependentInfo**: Number of dependents.
- **RelatedPersons**: Spouse, mother's maiden name, and father information, where applicable.
- **GovernmentIds**: Tax identification number and social-security number.
- **PrimaryIdentification**: Government-issued primary ID details, including type (accepted values: Passport, Driver's License, UMID, SSS ID, GSIS ID, PRC ID, Voter's ID, PhilHealth ID, National ID, Postal ID, Others), number, issue/expiry dates, and issue country.
- **Address**: Current and permanent addresses, including street, city, postal code, barangay, province, country, occupancy status, and occupied-since date.
- **EmergencyContact**: Name, relationship, and contact number of the emergency contact.
- **EmploymentDetails**: Employee level, company trade name, company ID number, gross income, income period, occupation, and hire dates.
- **Consent**: Privacy consent, attestation, signature name, and signed-at timestamp.
- **AuditMetadata**: Creation and update timestamps, and creation/update identifiers.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Applicants can complete registration in a single session without losing data when navigating between steps.
- **SC-002**: At least 90% of applicants who start the wizard reach the review step on their first attempt.
- **SC-003**: Submission errors are resolved within two correction cycles for 95% of applicants.
- **SC-004**: `GET /api/members/{id}` and `GET /api/members` return p95 response times ≤ 200 ms under the nominal load profile defined in plan.md.
- **SC-005**: Sensitive personal information does not appear in clear text in any application log.
- **SC-006**: 100% of submissions without explicit consent are rejected.
- **SC-007**: Readiness probe correctly reports unhealthy when the datastore is unreachable and healthy when restored.
- **SC-008**: The registration interface passes accessibility checks equivalent to WCAG 2.1 AA.
- **SC-009**: 100% of Member-role requests for the member list or another member's detail page are rejected; HR/Admin-role requests for the same resources succeed.

## Clarifications

### Session 2026-06-15

- **Q**: What are the correct labels and conditionality for the three related-person name blocks?  
  **A**: Spouse (required only if civil status is Married), Mother's maiden name (optional), Father (optional).
- **Q**: Which field(s) define a unique member record for duplicate prevention?  
  **A**: Email address only.
- **Q**: Does the product require local account creation, SSO integration, or payment collection beyond the source form fields?  
  **A**: No accounts or payments in v1; record submission only.
- **Q**: Is the admin create endpoint (FR-011) the same as the applicant registration endpoint, or a separate flow?  
  **A**: Same `POST /api/members` endpoint, role-gated by HR/Admin role vs anonymous/applicant.
- **Q**: Who controls the MemberStatus transitions (Submitted, UnderReview, Approved, Rejected)?  
  **A**: HR/Admin sets status via `PUT /api/members/{id}`.
- **Q**: Which primary ID types are accepted?  
  **A**: Standard PH government IDs: Passport, Driver's License, UMID, SSS ID, GSIS ID, PRC ID, Voter's ID, PhilHealth ID, National ID, Postal ID, and "Others" (free text).
- **Q**: How should concurrent updates to the same member record be handled?  
  **A**: Optimistic concurrency via a RowVersion/concurrency token; 409 Conflict on collision.

## Assumptions

- The source paper/spreadsheet form (`Member Information Form.xlsx`) is the authoritative field set.
- Currency is assumed to be Philippine Peso (PHP) unless Product specifies otherwise.
- Minimum member age is assumed to be 18 unless Product specifies otherwise.
- Tax and social-security identifiers are treated as required for all member types until confirmed otherwise.
- Religion is treated as optional and classified as sensitive personal information.
- Document upload/OCR, payment collection, and post-submission approval workflow are out of scope for this release.
- Single sign-on is an optional accelerator for prefilling data, not a required data source.
- The three related-person name blocks are labeled and conditioned as follows: **Spouse** (required only if civil status is Married), **Mother's maiden name** (optional), **Father** (optional).
- Duplicate member records are detected by email address; the system rejects a submission that uses an email already associated with an existing member.
- Local account creation, passwords, payment collection, data-retention/erasure UI, offline resilience/retry queues, token refresh during the wizard, rate limiting/bot mitigation, and PostgreSQL backup/DR are out of scope for this release; the platform captures and stores the submitted member record only.
- The 99.9% availability target, scalability thresholds, and responsive breakpoints are inherited from the hosting platform and front-end implementation choices rather than being v1 deliverable requirements.
- Duplicate TIN or company ID detection is not implemented in v1; only email uniqueness is enforced.
- Network connectivity loss during submission and JWT expiry during a long wizard session are handled by native browser behavior and standard 401 responses, respectively; dedicated retry/refresh flows are out of scope for v1.
