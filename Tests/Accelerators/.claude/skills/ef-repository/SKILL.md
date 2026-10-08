---
name: ef-repository
description: Extends EF repository contracts and implementations with new query or persistence methods. MUST load before creating or editing any I*Repository.cs or its Infrastructure implementation.
template-id: Intent.EntityFrameworkCore.Repositories.EFRepositorySkillTemplate
contentHash: FC1FA73E0BAD6BF8D08450482469AC12E5C47798B8891B12CF6F1A01F52D15BB
---
# EF Repository Extension

- Repositories should encapsulate all logic for retrieving and persisting entities. This includes any necessary joins, filtering, or other data access logic.
- Aggregations of data should be handled by the repository layer, not by services or other layers. This ensures that all data access logic is centralized and can be optimized as needed.
- Only add additional methods to the repository for querying aggregations or complex queries. Otherwise just use the existing methods.

## Repository Interface

### Instructions

- Only add additional methods to the repository for querying aggregations or complex queries. Otherwise just use the existing methods.
- Always read the base repository interface to understand what is already provided and available before adding new methods.
- If you add a new method to the repository interface, do not put any [IntentManaged] attributes on it, especially not [IntentManaged(Mode.Fully)].

### Rules when adding methods

- Always add the method signature to the repository interface contract first, then implement it in the repository implementation.
- Never return tuples. If a complex return type is required, create a new Contract record in this file (below this interface) and add a `[IntentIgnore]` attribute over it.

## Repository Implementation (Concrete)

### Rules when adding methods

- Always add the method signature to the repository interface contract first, then implement it in the repository implementation.
- Always add the `[IntentIgnore]` attribute to any method added.
- Always read the base repository methods to understand what is already provided and available before adding new methods.
- Optimize for query performance and maintainability when adding new methods.
- Never return tuples. If a complex return type is required, create a new Contract record in the Application layer and return that instead.
