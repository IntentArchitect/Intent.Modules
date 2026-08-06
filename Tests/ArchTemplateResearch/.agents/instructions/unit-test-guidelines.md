---
name: unit-test-guidelines
description: Instructions for implementing unit tests for Intent-managed .NET applications, covering command handlers, query handlers, domain event handlers, integration event handlers, domain services, and application service operations.
template-id: Intent.UnitTesting.UnitTestInstructionTemplate
appliesTo:
contentHash: E52C8BF92B3843DB6CA6EE6B6D2C128C7D70BC3583A697584A90436A8E721C62
---
## Code Preservation Rules

1. PRESERVE all `[IntentManaged]` attributes on the existing test class constructor, class, or file — never remove or alter them.
2. NEVER remove or modify existing class members, methods, or properties including their attributes.
3. NEVER change existing method signatures or implementations.
4. NEVER add comments to existing code.
5. NEVER remove any existing using clauses.
6. Ensure `using Intent.RoslynWeaver.Attributes;` is always present.
7. Only add new test methods — never replace or rewrite existing ones.

## Implementation Rules

- Mock ALL external dependencies (repositories, DbContext, domain services, IMapper, IEventBus, external services).
- NEVER use anonymous objects for entities — always use the actual domain entity class (e.g. `new Order { Id = id }`, never `new { Id = id }`).
- Identify the entity type from the repository interface signature (e.g. `IOrderRepository : IEFRepository<Order, Guid>` → use `Order`).
- Add `using` statements for ALL types used in tests: entities, repositories, DTOs, event types.
- DTO construction: check whether the DTO uses a static `Create(...)` factory method or property initializer — use whichever pattern the DTO defines.

## Test Structure

- Use xUnit `[Fact]`.
- Follow the AAA pattern with `// Arrange`, `// Act`, `// Assert` comments.
- For handlers: `Handle_{ExpectedBehavior}_When{Condition}` — omit "When" if the condition is obvious.
- For service and domain service operations: `{MethodName}_{ExpectedBehavior}_When{Condition}`.

## Test Data Quality

- Only set entity properties that are directly used in assertions or affect test behaviour.
- Avoid meaningless placeholder strings — use "", Guid.Empty, or 0 only for fields that are NOT part of the behavior under test and are NOT asserted.
- When the test asserts an assignment/update/mapping, use distinct, non-default values for "before" and "after" so the test fails if the assignment is removed.
    - Example: if asserting customer.Name is updated from the command, ensure original customer.Name != command.Name.
- Prefer minimal but semantically different values over "realistic" ones (e.g. "A" → "B" is fine; don't generate lorem ipsum).
- Do not create multiple tests that differ only in data quantity (e.g. 1 item vs 2 items) — one representative happy path is sufficient.
- Do not create separate tests solely to re-verify mock calls already covered in the happy path.
- If a test's purpose is "changes X", add at least one assertion that would fail if X were unchanged (either Assert.NotEqual(old, new) or by arranging different before/after values and asserting the after value).

## Tests to Avoid

- Do not create tests solely for parameter verification — functional tests already cover this.
- Do not test `CancellationToken` propagation unless the handler has specific cancellation logic.
- Do not create trivial variations of the same scenario.
- --

## For CQRS Handlers (Command and Query)

- *Entity types in mocks (CRITICAL):**

When mocking `FindAllProjectToAsync<TDto>`, the predicate must use the concrete entity type:

- WRONG: `It.IsAny<Expression<Func<object, bool>>>()`
- CORRECT: `It.IsAny<Expression<Func<Order, bool>>>()`
- *AutoMapper (CRITICAL):**
- Mock the singular `Map<TDto>(entity)` method — NOT `Map<List<TDto>>(list)`.
- Extension methods like `.MapToOrderDtoList()` call `Map<TDto>` per item individually.
- Example: `_mapperMock.Setup(x => x.Map<OrderDto>(It.IsAny<Order>())).Returns((Order o) => new OrderDto { Id = o.Id });`
- *Filtered query testing (TWO SCENARIOS — inspect the handler to determine which applies):**
- Scenario 1 — Generic `FindAllAsync` with a predicate lambda:*

Set up the repository to compile and apply the predicate so the filter logic is actually exercised:

```
_repositoryMock
    .Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((Expression<Func<Order, bool>> predicate, CancellationToken _) =>
        testData.Where(predicate.Compile()).ToList());
```

- Scenario 2 — Domain-specific repository method (e.g. `FindCompletedOrdersAsync`):*

Mock the method directly — do not compile a predicate. Add a comment noting the method filters internally:

```
// FindCompletedOrdersAsync filters by status internally
_repositoryMock
    .Setup(x => x.FindCompletedOrdersAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new List<Order>());
```

- --

## For Command Handlers

- *Test coverage required:**
- **Happy Path**: handler executes successfully.
- **Entity Not Found**: test `NotFoundException` when `FindByIdAsync` returns `null` (for Update and Delete operations).
- **State Changes**: assert entity properties are updated correctly (for Update operations).
- **Return Values**: `Guid` for Create, `void` for Delete.
- *Create operations — capture the added entity and assign an Id on save:**

```
Order? captured = null;
_repositoryMock.Setup(x => x.Add(It.IsAny<Order>())).Callback<Order>(o => captured = o);
_repositoryMock.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
    .Callback(() => captured!.Id = expectedId)
    .ReturnsAsync(1);
```

- *Update/Delete operations:** always test both the success path and the `NotFoundException` path.
- --

## For Query Handlers

- *Test coverage required:**
- **Happy Path**: returns the expected DTO or list.
- **Empty Results**: primary edge case — test behaviour when the repository returns an empty collection.
- **Filtered Queries**: for predicate-based queries, test when data exists but does not match the filter criteria (use predicate compilation — see CQRS section).
- **Entity Not Found**: for GetById queries, test `NotFoundException` or `null` return depending on handler behaviour.
- *Return values** are DTOs or lists — verify the mapped properties, not just that the result is non-null.
- --

## For Domain Event Handlers

- *Handler pattern:** `INotificationHandler<DomainEventNotification<TDomainEvent>>` — access event data via `notification.DomainEvent.PropertyName`.
- *Test coverage required:**
- **Happy Path**: handler processes the event and performs all expected operations.
- **Entity Interactions**: verify entities are created or modified with correct properties from the domain event.
- **Repository Operations**: verify Add/Update/Remove and `SaveChangesAsync` calls.
- **Service Interactions**: verify service methods are called with correct parameters.
- **Event Bus Operations**: verify `Publish<T>()` and `Send<T>()` are called with correct data.
- **Error Handling**: entity not found, service throws — verify exception propagates and subsequent operations are not called.
- **Operation Ordering**: when sequence matters, use a `List<string> callOrder` with `.Callback(() => callOrder.Add("OperationName"))` on each mock, then assert the order.
- --

## For Integration Event Handlers

- *Handler pattern:** `IIntegrationEventHandler<TMessage>` with method `HandleAsync` — message properties are accessed directly (no `DomainEvent` wrapper).
- *Test coverage required:**
- **Happy Path**: handler processes the message and calls all expected services.
- **Service Interactions**: verify service methods receive the correct values extracted from the message.
- **Event Bus Operations**: verify `Publish<T>()` and `Send<T>()` calls with correct data (capture with `.Callback<T>(e => captured = e)`).
- **Exception Propagation**: when a service throws, assert the exception propagates and that subsequent operations (event bus calls, further service calls) were NOT invoked.
- **Edge Cases**: empty or default-valued message properties (e.g. `""`, `Guid.Empty`, `false`).
- *Unknown dependencies:** if the handler injects a dependency whose interface is not available in context, add:

```
// NOTE: Unable to locate source files for 'IUnknownService'. Mock manually and verify expected calls.
```

- --

## For Domain Services

- *Core principle (CRITICAL):** domain services coordinate business logic by calling methods on domain entities. Tests MUST verify the entity's **state changed correctly**, not just that repository methods were called. Verifying `FindByIdAsync` was called is insufficient — inspect entity properties after the service executes.
- *Test coverage required:**
- **Happy Path**: operation completes and entity state reflects the expected change.
- **Entity State Changes (HIGHEST PRIORITY)**: after the service executes, assert the entity's properties or collections have the expected values.
- **Entity Not Found**: test exceptions when `FindByIdAsync` returns `null`.
- **Domain Invariant Violations**: test that domain rules are enforced (e.g. invalid state transitions).
- **Unit of Work**: verify `SaveChangesAsync` is called when state changes occur.
- **Multiple Aggregates**: when the service coordinates across aggregates, verify state changes on all affected entities.
- *Pattern — inspect entity state using the mock reference:**

```
var order = new Order { Id = orderId };
_repositoryMock.Setup(x => x.FindByIdAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

await _service.CompleteOrderAsync(orderId);

// Assert entity state — not just that FindByIdAsync was called
Assert.Equal(OrderStatus.Completed, order.Status);
Assert.NotNull(order.CompletedAt);
```

- --

## For Service Operations

- *Test coverage required** (same scope as command handlers, applied per public method):
- **Create**: capture added entity, assign Id in `SaveChangesAsync` callback, verify returned `Guid`.
- **Update**: load entity, verify state changes, test `NotFoundException`.
- **Delete**: verify `Remove` called, test `NotFoundException`.
- **GetById**: verify mapped DTO returned, test `NotFoundException`.
- **GetAll / filtered**: verify list returned, test empty collection, test filtered queries (TWO SCENARIOS — see CQRS section).

Each public method in the service gets its own set of tests within the same test class. Use `{MethodName}_` as the test name prefix to group tests by operation.
