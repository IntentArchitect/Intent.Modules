---
contentHash: 301F671A9B12F7B1CCDED153AFA242FBA7068B1C192DCFFF4FDEA0A32C32F617
---
# Intent Domain Interactions Expert Cheatsheet

## Workflow Overview

```
Designer model:                                          Generated code (handler body):
Operation "PlaceOrder"                                 ┌─ var customer = _customers.FindByIdAsync(...);
├─ QueryEntity → Customer            ──ImplementInteractions──►
├─ CreateEntity → Order                                ├─ _orders.Add(order);
├─ Publish → OrderPlacedEvent                          └─ _bus.Publish(new OrderPlacedEvent(...));
```

> [!IMPORTANT]
> Everything here lives in `Intent.Modules.Common.CSharp.Interactions`, which needs `Intent.Common.CSharp` 3.9.0 or later; `Register(strategy, priority)` needs 3.9.1 or later. Module Builder's default floor is 3.8.0, where the namespace does not exist. Raise the `Intent.Common.CSharp` `<dependency>` in the module's `.imodspec` and the `Intent.Modules.Common.CSharp` package reference in its `.csproj` to 3.9.1 or later. Raise `Intent.Modules.Common` (`.csproj`) and the `Intent.Common` `<dependency>` (`.imodspec`) with it, to at least the `Intent.Modules.Common` version that package depends on: 3.7.3 for 3.9.1, more for later versions (the error names it). From `Intent.Modules.Common` 3.9.0 the `Intent.SoftwareFactory.SDK` reference must rise too (3.10.0 for 3.9.0 and 3.9.1, more for later versions); the next `NU1605` names it. Module Builder writes 3.7.2, so otherwise restore fails with `NU1605: Detected package downgrade` (see the `known-build-gotchas` instructions). The designer packages the interaction predicates come from need more: `Intent.Modules.Modelers.Services` 4.x and `Intent.Modules.Modelers.Services.CQRS` 6.x depend on `Intent.Modules.Common` 3.8.0, so raise it to 3.8.0 once you reference either, and `Intent.Modules.Modelers.Services.DomainInteractions` 2.2.0 and later also depends on `Intent.SoftwareFactory.SDK` 3.9.0, so raise that package reference to 3.9.0 (Intent Architect 4.5.0 or later).

===

## Dispatch

```
┌──────────────────────────────────┐
│  Handler Factory Extension       │  ◄── lives in the handler's module (e.g. MediatR)
│  - OnBeforeTemplateExecution     │      finds handler templates, attaches mapping resolvers,
│    iterates handlers,            │      calls method.ImplementInteractions(handler.Model)
│    sets up mapping resolvers     │
└──────────────┬───────────────────┘
               │
               ▼
┌──────────────────────────────────┐
│  ImplementInteractions ext.      │  ◄── from Intent.Modules.Common.CSharp.Interactions
│  - GetInteractions(model)        │      child elements + target-end associations, in
│  - per interaction:              │      designer order, kept only if a registered
│    GetInteractionStrategy(...)   │      strategy IsMatch()es them
│    .ImplementInteraction(...)    │
└──────────────┬───────────────────┘
               │   GetInteractionStrategy: every strategy whose IsMatch is true,
               │   ordered by priority; the lowest wins. None matched: a warning
               │   is logged and the interaction is skipped.
               ▼
┌──────────────────────────────────┐
│  IInteractionStrategy            │  ◄── strategy logic
│  - IsMatch(IElement)             │
│  - ImplementInteraction(...)     │
│    └─ emits phased statements    │
│       into method body           │
└──────────────────────────────────┘
```

- `method.ImplementInteractions(processingHandlerModel)` only passes on interactions that some strategy matches, so an unmatched one is dropped silently. Passing elements yourself (`method.ImplementInteractions(elements)` or `method.ImplementInteraction(element)`) logs `No interaction strategy matched for ...` and skips it.
- Several matching strategies are not an error: the lowest priority wins, and the choice is logged at debug level.

===

## Strategy Skeleton

```csharp
public class SendCommandStrategy : IInteractionStrategy
{
    public bool IsMatch(IElement interaction)
    {
        // Typed predicate for the association end, plus a check on what it targets. No side effects.
        return interaction.IsPerformInvocationTargetEndModel()
               && interaction.TypeReference?.Element?.IsCommandModel() == true
               && interaction.Mappings.Any();
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        ArgumentNullException.ThrowIfNull(method);

        var interaction = (IAssociationEnd)interactionElement;
        var handlerClass = method.Class;
        var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;

        // 1. Inject required services (returns the field name). The role is your own template's.
        var senderField = handlerClass.InjectService(template.GetTypeName("MyModule.SenderInterface"), "sender");

        // 2. Register mapping resolvers on the method's shared mapping manager
        var mapping = method.GetMappingManager();
        mapping.AddMappingResolver(new MyMappingResolver(template));

        // 3. Register type sources (a literal template role or template id)
        template.AddTypeSource("Application.Command");

        // 4. Emit body with explicit phases
        var command = mapping.GenerateCreationStatement(interaction.Mappings.Single());
        method.AddStatement(ExecutionPhases.BusinessLogic, new CSharpStatement($"await {senderField}.SendAsync({command});"));
    }
}
```

- `IsPerformInvocationTargetEndModel()` comes from `Intent.Modelers.Services.Api` (the `Intent.Modelers.Services` module, 4.0.0 or later); `IsCommandModel()` from `Intent.Modelers.Services.CQRS.Api` (the `Intent.Modelers.Services.CQRS` module). Each predicate you use is a NuGet package reference plus an `.imodspec` `<dependency>` on the module that defines it.
- Other designer interactions have their own predicates, e.g. `IsCreateEntityActionTargetEndModel()`, `IsQueryEntityActionTargetEndModel()`, `IsUpdateEntityActionTargetEndModel()`, `IsDeleteEntityActionTargetEndModel()` (`Intent.Modelers.Services.DomainInteractions`) and `IsPublishIntegrationEventTargetEndModel()` / `IsSendIntegrationCommandTargetEndModel()` (`Intent.Modelers.Services.EventInteractions`).

===

## Registration

```csharp
public class MyInteractionStrategiesRegistration : FactoryExtensionBase
{
    public override string Id => "MyCompany.MyModule.MyInteractionStrategiesRegistration";

    protected override void OnBeforeTemplateRegistrations(IApplication application)
    {
        // Default priority 0. When several strategies match an interaction, the lowest priority wins;
        // at equal priority the one registered first wins, so this can tie with a built-in strategy at 0.
        InteractionStrategyProvider.Instance.Register(new SendCommandStrategy());

        // A negative priority overrides a built-in strategy that matches the same interaction.
        InteractionStrategyProvider.Instance.Register(new MyUpdateEntityStrategy(), priority: -10);
    }
}
```

- Register in a factory extension's `OnBeforeTemplateRegistrations`, so the strategy is in place before any template is created or built.
- From `Intent.Common.CSharp` 3.9.6 the registry is cleared at the start of every Software Factory run, so registering again on each run is expected and does not duplicate anything.

===

## Built-in Strategies

None of these are SDK: each lives in an Intent .NET module, and your strategy competes with them through priority. The public ones can be reused only by taking a NuGet and `.imodspec` dependency on their module; the internal ones cannot be reused at all.

| Strategy | Module | Matches | Purpose | Reusable |
|---|---|---|---|---|
| `QueryInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsQueryEntityActionTargetEndModel()` with a "Query Entity Mapping" | Load entity (single or list) into a local variable and track it | Public |
| `ODataQueryInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsQueryEntityActionTargetEndModel()` whose source has the `ODataQuery` stereotype | OData query | Public |
| `CreateEntityInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsCreateEntityActionTargetEndModel()` with mappings | New-up entity from the mapping, add it to the repository (in `Persistence`), track it | Public |
| `UpdateEntityInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsUpdateEntityActionTargetEndModel()` with mappings | Load the entity, track it and apply the update mapping. Emits an explicit `Update(...)` only when the repository requires one, and `SaveChangesAsync()` in `Persistence` only when the unit of work is not persisted automatically | Public |
| `DeleteEntityInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsDeleteEntityActionTargetEndModel()` with mappings | Load + remove | Public |
| `CallDomainServiceInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsPerformInvocationTargetEndModel()` targeting an Operation or Stored Procedure not owned by a Class, with mappings | Inject domain service + invoke | Public |
| `CallEntityServiceInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsPerformInvocationTargetEndModel()` targeting an Operation owned by a Class, with mappings | Call an operation on a previously queried entity | No (`internal`) |
| `ProcessingActionInteractionStrategy` | `Intent.Application.DomainInteractions` | `IsProcessingActionModel()` with mappings | Emit the processing action's mapped statements | Public |
| `CallServiceInteractionStrategy` | `Intent.Application.Contracts` | `IsPerformInvocationTargetEndModel()` targeting an Operation or Stored Procedure not owned by a Class, with mappings | Inject application service + invoke | Public |
| `SendOnMediatorInteractionStrategy` | `Intent.Application.MediatR` | `IsPerformInvocationTargetEndModel()` targeting a Command or Query, with exactly one mapping | Send through MediatR | Public |
| `SendOnWolverineInteractionStrategy` | `Intent.Application.Wolverine` | `IsPerformInvocationTargetEndModel()` targeting a Command or Query, with exactly one mapping | Send through Wolverine | Public |
| `PublishIntegrationMessageInteractionStrategy` | `Intent.Eventing.Contracts` | `IsPublishIntegrationEventTargetEndModel()` or `IsSendIntegrationCommandTargetEndModel()` | Map source → new message, then publish/send | Public |
| `CommandPatchEntityInteractionStrategy`, `TradServicePatchEntityInteractionStrategy` | `Intent.AspNetCore.Controllers.JsonPatch` | Update-entity action from an HTTP PATCH command or service operation; registered at priority -100 to override `UpdateEntityInteractionStrategy` | Apply a JSON Patch | No (`internal`) |
| `CqrsInvocationHttpInteractionStrategy` | `Intent.Integration.HttpClients`, `Intent.Dapr.AspNetCore.ServiceInvocation` | Perform Invocation of an element with HTTP settings that lives in another application | Call it over HTTP | No (`internal`) |

===

## Handler Discovery in a Factory Extension

```csharp
protected override void OnBeforeTemplateExecution(IApplication application)
{
    var templates = application
        .FindTemplateInstances<ITemplate>("Application.Eventing.EventHandler")
        .OfType<ICSharpFileBuilderTemplate>();

    foreach (var template in templates)
    {
        foreach (var handler in template.CSharpFile.GetProcessingHandlers())
        {
            var method = handler.Method;
            var mappingManager = method.GetMappingManager();

            // Your own resolvers; this mirrors what Intent.Eventing.Contracts does with its own.
            mappingManager.AddMappingResolver(new MyMappingResolver(template));

            template.AddTypeSource("Domain.Entity");

            mappingManager.SetFromReplacement(handler.Model, "message");
            method.ImplementInteractions(handler.Model);
        }
    }
}
```

- Role strings are literals; `Intent.Modules.Constants` / `TemplateRoles` is not published for third-party modules. `"Domain.Entity"` is the role Intent's entity templates fulfil for type resolution; `"Domain.Entity.Primary"` is a different role (the entity template's own registration role), despite what the old `TemplateRoles.Domain.Entity.Primary` constant name suggests. See the role table in `intent-module-orchestrator`.
- `GetProcessingHandlers()` returns the methods a handler template registered with `RegisterAsProcessingHandlerForModel(...)`, so this works for any module's handlers without referencing their template types.

===

## Execution Phases

`ExecutionPhases` constants, in execution order:

| Phase | Typical contents |
|---|---|
| `Validation` | Guards and early validation |
| `Retrieval` | Loading entities and data |
| `BusinessLogic` | Domain logic, entity mutations, service calls. Statements added without a phase count as this phase |
| `Persistence` | Explicit `SaveChangesAsync()` and similar |
| `IntegrationEvents` | `_bus.Publish(...)`, `_bus.Send(...)` |
| `Response` | The terminal `return` expression |

- `method.AddStatement(phase, statement)` / `method.AddStatements(phase, statements)` insert before the first statement of a later phase, so strategies can run in any order and still produce an ordered body.
- `method.GetStatementsInPhase(phase)` returns what is already in a phase, e.g. to check whether a `Response` statement exists before adding one.
- `method.TrackedEntities()` is a per-method `Dictionary<string, EntityDetails>` keyed by interaction id: strategies that load or create an entity add it there, and later strategies read the variable name and data-access provider from it instead of loading it again.
- `method.SetExecutionPhases(order)` stores a phase order on the method as metadata, but `AddStatement(phase, ...)` always orders by the fixed `ExecutionPhases.ExecutionPhaseOrder`, so it does not change insertion order today.
- A phase string that is not one of these constants is inserted at the top of the method body.
