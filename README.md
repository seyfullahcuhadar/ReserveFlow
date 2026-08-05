# ReserveFlow

ReserveFlow is a .NET platform for event ticketing and appointment booking. It is developed with three complementary objectives:

1. To provide a controlled architecture laboratory for applying Domain-Driven Design, Clean Architecture, and measurable non-functional requirements.
2. To serve as a practical reference implementation for teams and developers building maintainable .NET applications with Clean Architecture.
3. To provide a realistic environment for designing, implementing, and validating CI/CD practices across the software delivery lifecycle.

The project prioritizes architectural clarity, explicit boundaries, repeatable delivery processes, and verifiable engineering outcomes over feature volume. Each capability is expected to exercise a domain boundary, provide measurable evidence for an NFR, or strengthen the automated delivery pipeline.

## Current status

Implemented:

- User registration with validation and password hashing
- PostgreSQL persistence through EF Core
- Versioned API routes under `/api/v1`
- OpenAPI and Swagger UI in Development
- OpenTelemetry tracing and metrics
- Jaeger, Prometheus, and Grafana development stack
- Custom registration span and metric
- Architecture and booking tests

The remaining ticketing, scheduling, payment, outbox, authentication, and authorization flows are planned incrementally. CI/CD automation will also be introduced progressively to validate build, test, containerization, security scanning, deployment, and rollback practices. See [the project plan](docs/PROJECT.md) and [use-case catalog](docs/USE_CASES.md).

## Architecture

ReserveFlow follows Clean Architecture with feature-first organization:

```text
Api  →  Infrastructure  →  Application  →  Domain
```

```text
src/
├── ReserveFlow.Api
├── ReserveFlow.Application
├── ReserveFlow.Domain
└── ReserveFlow.Infrastructure
tests/
├── ReserveFlow.Booking.Tests
└── ReserveFlow.Architecture.Tests
```

Key rules:

- Domain has no dependency on other project layers.
- Application depends only on Domain.
- Infrastructure implements Application and Domain ports.
- API is the composition root.
- Bounded contexts communicate through IDs, DTOs, and domain events instead of sharing entities.

## Technology

- .NET 10 and ASP.NET Core
- Entity Framework Core 10
- PostgreSQL 16
- FluentValidation
- OpenTelemetry
- Jaeger for distributed traces
- Prometheus for metrics
- Grafana for dashboards
- xUnit and NetArchTest
- Docker Compose
- CI/CD automation (planned)

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://docs.docker.com/get-docker/)
- EF Core CLI tools:

```bash
dotnet tool install --global dotnet-ef
```

### Run the infrastructure

```bash
docker compose up -d
```

This starts PostgreSQL and the local observability stack.

### Apply database migrations

```bash
dotnet ef database update \
  --project src/ReserveFlow.Infrastructure \
  --startup-project src/ReserveFlow.Api
```

### Run the API

```bash
dotnet run --project src/ReserveFlow.Api --launch-profile http
```

The API is available at `http://localhost:5189`.

Useful endpoints:

- Health: `http://localhost:5189/api/v1/health`
- Swagger UI: `http://localhost:5189/swagger`
- OpenAPI document: `http://localhost:5189/openapi/v1.json`
- User registration: `POST http://localhost:5189/api/v1/users/register`

Example registration request:

```bash
curl -X POST http://localhost:5189/api/v1/users/register \
  -H "Content-Type: application/json" \
  -d '{"email":"customer@example.com","password":"SecurePass123!"}'
```

## Observability

The API emits OpenTelemetry signals through OTLP to the host Alloy collector (LGTM stack):

```text
ReserveFlow.Api
├── traces  (OTLP/gRPC) → localhost:4317 → Alloy → Tempo
├── metrics (OTLP/HTTP) → localhost:4318 → Alloy → Mimir/Prometheus
└── logs    (OTLP/HTTP) → localhost:4318 → Alloy → Loki
```

Docker Compose still runs Jaeger UI, Prometheus, and Grafana for convenience, but **does not** bind host ports 4317/4318 (those belong to Alloy).

Local interfaces:

- Alloy OTLP: `localhost:4317` (gRPC), `localhost:4318` (HTTP)
- Jaeger UI: [http://localhost:16686](http://localhost:16686)
- Prometheus: [http://localhost:9090](http://localhost:9090)
- Grafana (compose): [http://localhost:3000](http://localhost:3000)

Compose Grafana still provisions Prometheus/Jaeger data sources; primary signal path for the running API is Alloy/LGTM.

Configuration is under the `Observability` section. Override with env vars if needed:

```bash
Observability__TracesEndpoint=http://localhost:4317
Observability__MetricsEndpoint=http://localhost:4318/v1/metrics
Observability__LogsEndpoint=http://localhost:4318/v1/logs
```

## Testing

Run the complete test suite:

```bash
dotnet test ReserveFlow.sln
```

Run only architecture rules:

```bash
dotnet test tests/ReserveFlow.Architecture.Tests
```

Build the solution:

```bash
dotnet build ReserveFlow.sln
```

These commands are intended to become mandatory quality gates in the CI pipeline.

## CI/CD roadmap

ReserveFlow also serves as a practical environment for exercising modern software delivery processes. Planned pipeline stages include:

- Restore, build, and automated test execution
- Architecture-rule enforcement
- Code quality, dependency, and security checks
- Reproducible container image builds
- Database migration validation
- Environment-based deployment workflows
- Health verification, rollback, and delivery evidence

CI/CD capabilities will be added incrementally and documented with the same measurable-evidence approach used for other non-functional requirements.

## Documentation

- [Project scope and phase plan](docs/PROJECT.md)
- [Domain model and bounded contexts](docs/DOMAIN.md)
- [Non-functional requirements](docs/NFR.md)
- [Solution structure](docs/STRUCTURE.md)
- [Use-case catalog](docs/USE_CASES.md)

## Development services

Stop the containers:

```bash
docker compose down
```

Stop the containers and remove local data:

```bash
docker compose down -v
```

The second command permanently deletes the local PostgreSQL, Prometheus, and Grafana volumes.

## License

No license has been added yet.
