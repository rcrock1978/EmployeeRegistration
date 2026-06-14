# Health Probe Contracts

## Liveness

- **Method**: `GET`
- **Route**: `/health/live`
- **Authorization**: None
- **Success**: `200 OK` + plain/text or JSON indicating healthy
- **Purpose**: Kubernetes/orchestrator liveness check

## Readiness

- **Method**: `GET`
- **Route**: `/health/ready`
- **Authorization**: None
- **Success**: `200 OK` + healthy when database is reachable
- **Failure**: `503 Service Unavailable` when database is unreachable
- **Purpose**: Traffic routing decision
