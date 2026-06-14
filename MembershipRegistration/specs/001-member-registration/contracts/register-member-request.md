# Register Member Request

**Endpoint**: `POST /api/members`

## Request Body

```json
{
  "personalInfo": {
    "title": "Mr.",
    "firstName": "Juan",
    "middleName": "Protacio",
    "lastName": "Dela Cruz",
    "suffix": "",
    "alias": "",
    "dateOfBirth": "1990-05-12",
    "placeOfBirth": "Manila",
    "countryOfBirth": "PH",
    "nationality": "Filipino",
    "gender": "Male",
    "civilStatus": "Married",
    "religion": "",
    "highestEducationalAttainment": "Bachelor's",
    "numberOfDependents": 2
  },
  "contactInfo": {
    "emailAddress": "juan@example.com",
    "contactNumber": "+639170000000"
  },
  "relatedPersons": {
    "spouse": {
      "firstName": "Maria",
      "middleName": "Reyes",
      "lastName": "Dela Cruz"
    },
    "motherMaidenName": "Ana Bautista Reyes",
    "father": {
      "firstName": "Pedro",
      "middleName": "Santos",
      "lastName": "Dela Cruz",
      "suffix": "Sr."
    }
  },
  "governmentIds": {
    "tin": "123-456-789-000",
    "sss": "01-2345678-9"
  },
  "primaryId": {
    "type": "Passport",
    "number": "P1234567A",
    "issueDate": "2021-01-10",
    "expiryDate": "2031-01-09",
    "issueCountry": "PH"
  },
  "currentAddress": {
    "streetNameAndNumber": "123 Mabini St.",
    "city": "Navotas",
    "postalCode": "1485",
    "barangay": "San Roque",
    "subdivisionPurok": "Purok 2",
    "province": "Metro Manila",
    "country": "PH",
    "ownerOrLessee": "Owner",
    "occupiedSince": "2015-06-01"
  },
  "permanentAddress": {
    "sameAsCurrent": true
  },
  "emergencyContact": {
    "contactName": "Maria Dela Cruz",
    "relationship": "Spouse",
    "contactNumber": "+639170000001"
  },
  "employment": {
    "employeeLevel": "RNF",
    "companyTradeName": "OPTODEV, INC.",
    "companyIdNumber": "OPT-00421",
    "grossIncome": 45000,
    "incomePeriod": "Monthly",
    "occupation": "Technician",
    "hiredFrom": "2019-12-01",
    "hiredTo": null
  },
  "consent": {
    "consentGiven": true,
    "attestation": true,
    "signatureName": "Juan P. Dela Cruz"
  }
}
```

## Validation Highlights

- `personalInfo.dateOfBirth`: applicant must be ≥ 18.
- `contactInfo.emailAddress`: valid email; unique across members.
- `governmentIds.tin`: `###-###-###-###`.
- `governmentIds.sss`: `##-#######-#`.
- `primaryId.expiryDate`: must be after `issueDate`.
- `relatedPersons.spouse`: required only if `civilStatus = Married`.
- `permanentAddress`: either `sameAsCurrent: true` or a full address object.
- `consent.consentGiven` and `consent.attestation`: must be `true`.

## Response

- `201 Created` with `Result<MemberSummaryResponse>` and `Location` header.
- `400 Bad Request` with structured field errors.
- `409 Conflict` if email already registered.
