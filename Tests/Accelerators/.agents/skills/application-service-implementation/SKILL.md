---
name: traditional-service-implementation
description: implement or revise traditional application service business logic in an existing service file. use when a c# application service class has incomplete or incorrect operation logic and chatgpt should update service methods, add private helper methods, and extend application or domain abstractions such as repositories, read services, or domain services if required, while avoiding direct infrastructure dependencies in the service.
contentHash: D1579E96CB19574C9CF6EAF41DFC2E565C6AC0C0A244E66ABB7C19D1D260A1F9
---

# Traditional Service Implementation

Implement business logic inside an existing traditional application service file. Keep service implementations aligned with the modeled domain, nearby service patterns, and application-layer boundaries.

## Core rules

- Treat the existing service file as the starting point. Update the relevant service operation(s) rather than rewriting unrelated parts.
- NEVER modify the signature of an existing service operation unless the user explicitly asks for that change.
- ALWAYS if you modify a service operation body, ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the service, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific APIs.
- If data access is required, favor using an existing repository, read service, or domain/application abstraction, but extend them if required.
- ALWAYS if the response requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the service MUST NOT compute it in memory. The service MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, the assistant must extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-service aggregation is allowed.
- Favor domain methods on aggregates/entities when they exist. If no such methods exist, follow the general implementation style already used by similar services in the solution.
- Follow the modeled domain language already present in the codebase: aggregates, entities, value objects, statuses, DTOs, projections, policies, filters, and business terms should drive the implementation.
- Search the codebase for similar services, repository methods, domain operations, mapping conventions, validation flows, error patterns, and save conventions before introducing a new approach.
- Add private helper methods inside the service when they improve readability, encapsulate repeated branching logic, or keep the main business flow clear.
- Keep orchestration in the service and place durable business rules in domain entities, value objects, specifications, or domain/application services when those patterns already exist nearby.
- Assume there is an ambient unit of work save in place unless nearby code shows otherwise. Explicitly save only when needed by the use case, such as when a surrogate key must be returned before control leaves the operation, or when an existing local convention requires an explicit save.

## Workflow

1. Inspect the existing service class, interface, operation signatures, DTOs, repositories/read services/domain services, and related domain types.
2. Search for code usages of:
   - similar traditional application services
   - repository, read-service, or domain-service abstractions
   - domain operations on the target aggregate or entity
   - DTO mapping conventions
   - validation, authorization, and error/result patterns
   - save and unit-of-work conventions in nearby services
3. Infer the intended business flow from the operation names, parameters, return types, surrounding domain model, and nearby feature implementations.
4. Implement the service operation using existing patterns first.
5. If the service needs missing DAL capabilities, extend the relevant repository/read/application abstraction in an allowed layer instead of introducing infrastructure access into the service.
6. Add focused private helper methods when the flow becomes easier to read, reuse, or validate.
7. Ensure cancellation tokens are threaded through async calls where applicable.
8. Verify the final code preserves layer boundaries, follows surrounding service conventions, and avoids infrastructure leakage.

## Service-specific guidance

Traditional services may combine command-style and query-style operations. Prefer patterns like these when supported by the surrounding codebase:

### For state-changing operations

- load the aggregate or entity through a repository abstraction
- validate existence, eligibility, ownership, or other business preconditions
- invoke domain behavior rather than mutating state ad hoc when domain methods exist
- fall back to nearby established mutation style only when domain methods do not exist
- persist through the established repository/unit-of-work conventions
- return the established command/service result type used by the solution

### For read operations

- retrieve through an existing repository or read abstraction
- apply business-relevant filters and visibility rules consistently with nearby services
- avoid over-fetching and avoid in-memory aggregation where an abstraction can shape the data more efficiently
- map to DTOs or response contracts using the existing mapping conventions
- keep read flows free of domain mutation or hidden write behavior

## Repository and abstraction guidance

When a needed capability is missing:

- Extend the existing repository, read service, or domain/application abstraction closest to the business concept.
- Keep new methods shaped around domain or application intent, not storage mechanics.
- Prefer expressive names such as `FindByIdAsync`, `GetDetailsAsync`, `ListByCriteriaAsync`, `ExistsBy...Async`, `SearchActive...Async`, or `GetSummaryAsync`.
- Do not add infrastructure comments or instructions to the service.
- Do not reference EF includes, Dapper SQL, joins, transactions, or storage-specific tuning from the service.

## Mapping guidance

- Reuse the existing DTO mapping approach already present in the solution, whether that is mapper-based, extension-method-based, projection-based, or another established convention.
- Keep mapping logic lightweight in the service.
- If mapping is repetitive or already centralized elsewhere, reuse the existing mapping helpers/extensions instead of inlining ad hoc conversions.
- For query-like operations, allow simple fetch-and-map flows in the service, but push complex shaping, filtering, or aggregation into abstractions where appropriate for performance and consistency.

## Error and result guidance

- Follow the existing application error/result conventions already used in the solution.
- Reuse nearby patterns for not found, validation failures, business rule failures, and authorization failures.
- Do not invent a new exception or result style when the surrounding code already establishes one.

## Output expectations

Produce a concrete code update that:

- fills in or corrects the relevant service operation bodies
- adds private helper methods in the service if useful
- extends allowed-layer abstractions when needed for the service to be correct and performant
- preserves the existing feature style, naming, mapping conventions, and response shape
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- every injected dependency belongs to the Domain or Application layers
- the service imports no infrastructure package namespaces
- repository/read-service/domain-service changes, if any, are expressed as abstractions only
- the implementation follows existing usage patterns for service orchestration, mapping, filtering, and persistence
- aggregation is not performed in memory when it should live behind an abstraction
- business language matches the surrounding domain model
- the code path is cancellation-aware and async-safe where applicable
- explicit save behavior matches nearby conventions and is only added when the use case requires it