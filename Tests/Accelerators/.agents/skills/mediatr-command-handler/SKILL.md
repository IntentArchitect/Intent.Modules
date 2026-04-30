---
name: mediatr-command-handler
description: implement or revise mediatR command handler business logic in an existing handler file. use when a c# mediatR command handler has an incomplete or incorrect handle method and chatgpt should update the handle method, add private helper methods, and extend application or domain abstractions such as repositories or services if required, while avoiding direct infrastructure dependencies in the handler.
contentHash: 2F6B130BDA60FC80328C94F6FE226CF69236047BCF1CCFBD6CB0808D1D2EAFFE
---

# MediatR Command Handler

Implement command handler business logic inside an existing handler file. Favor domain intent, existing code patterns, and clean layer boundaries over convenience shortcuts.

## Core rules

- Treat the existing handler file as the starting point. Update the `Handle` method rather than rewriting unrelated parts.
- NEVER modify the method signature of the Handle method.
- ALWAYS if you modify the `Handle` method, ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the handler, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific data access abstractions.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, but extend them if required .
- ALWAYS if the response requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the service MUST NOT compute it in memory. The handler MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, the assistant must extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-service aggregation is allowed.
- Follow the modeled domain language already present in the codebase: aggregates, entities, value objects, policies, invariants, statuses, and business terms should drive the implementation.
- Search the codebase for similar handlers, repository methods, domain services, validation flows, and result patterns before introducing a new approach.
- Add private helper methods inside the handler when they improve clarity, keep business flow readable, or encapsulate repeated branching logic.
- Keep orchestration in the handler and place durable business rules in domain entities, value objects, specifications, or domain/application services when those patterns already exist nearby.
- Assume there is an ambient unit of work save in place unless nearby code shows otherwise. Explicitly save only when needed by the use case, such as when a surrogate key must be returned before control leaves the handler, or when an existing local convention requires an explicit save.

## Workflow

1. Inspect the existing handler, request, response, validator, repository interfaces, and related domain types.
2. Search for code usages of:
   - similar command handlers
   - repository interfaces and existing repository methods
   - domain operations on the target aggregate or entity
   - result and error patterns used in the solution
3. Infer the intended business flow from the request shape, naming, surrounding domain model, and nearby feature implementations.
4. Implement the `Handle` method using existing patterns first.
5. If the handler needs missing DAL capabilities, extend the relevant repository abstraction in an allowed layer instead of pulling infrastructure into the handler.
6. Add focused private helper methods when the flow becomes easier to read or test.
7. Ensure cancellation tokens are threaded through async calls where applicable.
8. Verify the final code preserves layer boundaries and does not introduce infrastructure leakage.

## Command-specific guidance

Commands usually change state. Prefer patterns like these when supported by the surrounding codebase:

- load aggregate or entity through a repository
- validate existence and ownership/eligibility rules
- invoke domain behavior rather than mutating state ad hoc
- persist through the repository abstraction
- return the established command result type used by the solution
- publish or coordinate domain/application events only if existing nearby patterns already do so

## Repository extension guidance

When a needed repository capability is missing:

- Extend the existing repository abstraction closest to the aggregate or read/write concern.
- Keep the new method shaped around domain intent, not storage mechanics.
- Prefer expressive methods such as `GetForUpdateAsync`, `ExistsBy...Async`, `FindActive...Async`, or `SaveAsync` over storage-oriented names.
- Do not add infrastructure comments or instructions to the handler.
- Do not reference how the repository will be implemented in EF, Dapper, SQL, or similar.

## Output expectations

Produce a concrete code update that:

- fills in or corrects the `Handle` method
- adds private helper methods in the handler if useful
- extends allowed-layer abstractions when needed for the handler to be correct
- preserves the existing feature style and naming
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- every injected dependency belongs to the Domain or Application layers
- the handler imports no infrastructure package namespaces
- repository changes, if any, are expressed as abstractions only
- the implementation follows existing code usage patterns rather than inventing a novel style without reason
- business language matches the surrounding domain model
- the code path is cancellation-aware and async-safe where relevant
