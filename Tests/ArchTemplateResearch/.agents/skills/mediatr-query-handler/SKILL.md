---
name: mediatr-query-handler
description: Implement or fix business logic in a MediatR query handler's Handle method, following this codebase's established architectural conventions. Use when a C# query handler's Handle method is missing, incomplete, or needs correction.
template-id: Intent.Application.MediatR.QueryHandlerSkillTemplate
contentHash: D14146E2B6B53947AA479725A6DB970A4E5D9D7AFB76CF3E6507E6B6F5964CD6
---
# MediatR Query Handler

Implement query handler business logic inside an existing handler file. Keep handlers aligned with the modeled domain and existing query patterns while protecting architectural boundaries.

## Core rules

- Treat the existing handler file as the starting point. Update the `Handle` method rather than rewriting unrelated code.
- NEVER modify the method signature of the Handle method.
- ALWAYS if you modify the `Handle` method, ensure that the `IntentManaged` attribute indicates that the body of the method must be in `Mode.Ignore` (e.g. `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`).
- Inject only dependencies from the Domain or Application layers.
- Never introduce direct dependencies on infrastructure packages or infrastructure types in the handler, including Entity Framework, Dapper, concrete DbContexts, SQL clients, or vendor-specific read/query APIs.
- If data access is required, favor using an existing repository abstraction from the Domain or Application layers, but extend them if required .
- ALWAYS if the response requires any aggregation (Count/Sum/Average/Min/Max/First/Last/GroupBy), the service MUST NOT compute it in memory. The handler MUST call a Domain/Application abstraction that returns an aggregated result/summary/shaped dataset. If no suitable method exists, the assistant must extend the appropriate repository/read abstraction (contract first, then implementation) and then call it. No temporary in-service aggregation is allowed.
- Follow the modeled domain language already present in the codebase, including terms for statuses, projections, identities, filters, and business concepts.
- Search the codebase for similar query handlers, read models, repository methods, projections, pagination patterns, and mapping conventions before inventing a new approach.
- Add private helper methods inside the handler when they improve readability, mapping clarity, or reuse of branching/query composition logic.
- Keep the handler focused on orchestrating retrieval, filtering, and mapping. Push reusable rules or interpretation logic into existing domain/application abstractions when nearby code already uses them.
- Assume there is an ambient unit of work save in place unless nearby code shows otherwise. Explicitly save only when needed by the use case, such as when a surrogate key must be returned before control leaves the handler, or when an existing local convention requires an explicit save.

## Workflow

1. Inspect the existing handler, request, response, repository or read-service abstractions, and related domain/read-model types.
2. Search for code usages of:
  - similar query handlers
  - projection or DTO mapping patterns
  - pagination, sorting, filtering, and authorization rules
  - repository or read-service methods serving similar data
3. Infer the intended read behavior from the request shape, response contract, naming, and nearby feature implementations.
4. Implement the `Handle` method using existing query patterns first.
5. If the handler needs missing DAL capabilities, extend the relevant repository or read abstraction in an allowed layer instead of introducing infrastructure access into the handler.
6. Add focused private helper methods when they make filtering, mapping, or result composition easier to understand.
7. Ensure cancellation tokens are threaded through async calls where applicable.
8. Verify the final code preserves layer boundaries and avoids infrastructure leakage.

## Query-specific guidance

Queries usually read and shape data. Prefer patterns like these when supported by the surrounding codebase:

- retrieve through an existing repository or read abstraction
- apply business-relevant filters and visibility rules consistently with nearby handlers
- reuse established paging, sorting, and projection conventions
- map to the existing response contract with minimal surprise
- avoid over-fetching or domain mutation in the query flow

## Repository and read abstraction guidance

When a needed read capability is missing:

- Extend the existing repository or read abstraction closest to the business concept.
- Keep method signatures expressive in domain/application terms rather than storage terms.
- Prefer names such as `GetDetailsAsync`, `ListByCriteriaAsync`, `SearchActive...Async`, or `GetSummaryAsync` over schema-oriented names.
- Do not explain or encode infrastructure implementation details in the handler.
- Do not reference EF includes, Dapper SQL, joins, or storage-specific tuning from the handler.

## EF Related Data Loading guidance

- NEVER use `Include` or `ThenInclude` in the Application Layer, these are only available in the Infrastructure layer.
- Lazy loading with proxies is enabled. 
- Entities are configured using the `Owns` apis, so compsitional children will be automatically loaded with their parents.
- You can rely on navigation properties being automatically loaded when accessed.
- (CRITICAL) If your implementation will cause a lot of Lazy loading consider other alternatives, like moving the data loading into the repository layer.

## Unit of Work guidance

- SaveChanges rule (STRICT): Do not call UnitOfWork.SaveChangesAsync(...) / SaveChangesAsync(...) in a handler/service method unless the operation returns a payload that requires DB-generated values, such as a generated Id, surrogate key, RowVersion/concurrency token, DB-generated timestamp, or computed column.
- If the operation returns Unit, void, Task, or IRequest with no result: do not call SaveChangesAsync.
- If the operation returns an identifier or DTO that needs generated fields: call SaveChangesAsync before returning.
- If unsure, omit SaveChangesAsync and assume an outer unit-of-work/pipeline commit.
- When reviewing code, remove SaveChangesAsync unless there is a clear generated-value or immediate-commit requirement.

## Entity Framework repository guidance

- Repository update rule (STRICT): Do not call repository.Update(...) / repo.Update(...) when using EF repositories.
- EF tracks loaded entities automatically. Modify the entity properties directly and let the Unit of Work persist the tracked changes.
- Only call Add/Create/Delete operations when inserting or removing entities.
- When reviewing code, remove unnecessary Update calls for entities loaded from an EF repository.

## AutoMapper guidance

- Any read/query method, including MediatR query handlers and application services, that returns Application-layer DTOs (`*Dto`) derived from Domain entities **MUST** use AutoMapper.
    - Do not manually construct DTOs (`new XxxDto { ... }`) on read/query paths.
- **AutoMapper gate (absolute):** If you use any `ProjectTo*`, `Find*ProjectTo*`, `FindAllProjectTo*`, or `*ProjectToAsync*` method anywhere in the call chain, you **MUST**:
    - **verify mapping exists** by locating `CreateMap<TDomain, TDto>()` in a `Profile` **and cite file path + excerpt**, **OR**
    - if verification fails, **immediately create** the required AutoMapper `Profile`(s) (including **all required nested mappings**).
    - **No assumptions allowed** (a generic projection method or other feature usage is not verification).
- **Registration assumption (do not block on DI):**
    - Assume AutoMapper is registered via assembly scanning, e.g.:services.AddAutoMapper(Assembly.GetExecutingAssembly());
    - Therefore, **do not delay profile creation** because DI registration details are not currently visible.
    - Do not modify DI registration as part of this guidance unless the user explicitly asks.
- Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and AutoMapper is not reasonable.
    - This must include an inline code comment explaining why AutoMapper is not reasonable.
    - “Mapping doesn’t exist yet” is not a valid exception.
- If you can't find any existing mappings, create them in the same project as the services under:
    - `./Mappings/<FeatureOrAggregate>/<Entity>DtoProfile.cs`
    - Example: `MyApp.Application/Mappings/Invoices/InvoiceDtoProfile.cs`            

**Example:**
```csharp

public class CustomerDtoProfile : Profile
{
    public CustomerDtoProfile()
    {
        CreateMap<Customer, CustomerDto>();
    }
}

public static class CustomerDtoMappingExtensions
{
    public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) =>
        mapper.Map<CustomerDto>(projectFrom);

    public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) =>
        projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
}
```

## Output expectations

Produce a concrete code update that:

- fills in or corrects the `Handle` method
- adds private helper methods in the handler if useful
- extends allowed-layer abstractions when needed for the handler to be correct
- preserves the existing feature style, mapping conventions, and response shape
- keeps unrelated refactors out of scope

## Review checklist

Before finishing, check that:

- every injected dependency belongs to the Domain or Application layers
- the handler imports no infrastructure package namespaces
- repository or read-service changes, if any, are expressed as abstractions only
- the implementation follows existing usage patterns for projections, filtering, and pagination
- business language matches the surrounding model and response contract
- the code path is cancellation-aware and async-safe where relevant
