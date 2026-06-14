# Common Response Envelope

All API endpoints return a uniform `Result<T>` envelope.

## Success

```json
{
  "isSuccess": true,
  "value": { /* T */ },
  "error": null
}
```

## Validation Failure (HTTP 400)

```json
{
  "isSuccess": false,
  "value": null,
  "error": {
    "code": "Validation.Failed",
    "message": "One or more fields are invalid.",
    "details": [
      {
        "field": "personalInfo.email",
        "code": "Email.Invalid",
        "message": "A valid email is required."
      }
    ]
  }
}
```

## Status Codes

| Code | When |
|---|---|
| `200 OK` | Successful read/update |
| `201 Created` | Member registered |
| `400 Bad Request` | Validation failure |
| `403 Forbidden` | Authenticated user lacks required role |
| `404 Not Found` | Unknown member id |
| `409 Conflict` | Duplicate email, concurrency conflict (stale RowVersion), or other unique constraint violation |
| `500 Internal Server Error` | Unhandled error (returned as ProblemDetails; no stack trace in non-dev) |

## Conflict Response (HTTP 409)

```json
{
  "isSuccess": false,
  "value": null,
  "error": {
    "code": "Conflict.DuplicateEmail",
    "message": "A member with this email address already exists.",
    "details": [
      {
        "field": "contactInfo.emailAddress",
        "code": "Email.Duplicate",
        "message": "This email is already registered."
      }
    ]
  }
}
```
