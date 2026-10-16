---
contentHash: E08BEDE8C213C8D604C1B59B0F3E81C31F1EEAA03A9B1C882AB272BDACD6CDA8
---
# Intent Mapping Architect Cheatsheet

## Terminology Bridge

- Map Via -> Invocation Mapping
- Data Type Mapping -> Data Mapping

Use the current terms in implementation logic and docs, while preserving alias awareness for legacy discussions.

## 1. The Mapping Tree

The manager turns an `IElementToElementMapping` into a tree of `MappingModel` nodes, one level per step of the target path. Each node exposes:

- `Model`: the target element at this level.
- `Mapping`: the drawn `IElementToElementMappedEnd` when this node is itself mapped (a terminal node), otherwise `null` (an object node).
- `Children`: the nodes one level deeper.
- `MappingType` / `MappingTypeId`: the designer mapping type the whole tree came from.

Each node is resolved to an `ICSharpMapping` by the registered resolvers (section 4). When no resolver claims a node, the manager uses `DefaultCSharpMapping` for a terminal node and `MapChildrenMapping` for an object node. A `CSharpMappingBase` resolves its own `Children` the same way, so recursion is a matter of delegating to them:

```csharp
public class AssignOrRecurseMapping : CSharpMappingBase
{
    public AssignOrRecurseMapping(MappingModel model, ICSharpTemplate template)
        : base(model, template)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        // Object node: nothing drawn at this level, only children.
        if (Mapping == null)
        {
            return Children.SelectMany(x => x.GetMappingStatements()).ToList();
        }

        // Terminal node: one assignment.
        return new[]
        {
            new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement())
        };
    }
}
```

### Practical engine pattern

- If node is traversal-only: recurse through child nodes.
- If node is scalar terminal: emit one assignment.
- If node is object/collection: emit guarded object update or helper call, then recurse.

## 2. Resolution

Pick the mapping you need off the element by its designer mapping type, then generate the update assignments through MappingManager resolution.

```csharp
// updateAction is the IElement / IAssociationEnd that owns the mapping.
var updateMapping = updateAction.Mappings.SingleOrDefault(x => x.Type == "Update Entity Mapping");
if (updateMapping == null)
{
    return;
}

var manager = new CSharpClassMappingManager(template);

// Resolve identifiers via model element identities (no hardcoded member names).
manager.SetFromReplacement(sourceElement, "request");
manager.SetToReplacement(targetElement, "entity");

var statements = manager.GenerateUpdateStatements(updateMapping);
```

`GetUpdateEntityMapping()` and `GetQueryEntityMapping()`, which you will see in Intent's own .NET modules, are not part of the SDK. They are extension methods in the `Intent.Application.DomainInteractions` module (namespace `Intent.Modules.Application.DomainInteractions.Extensions`, NuGet package `Intent.Modules.Application.DomainInteractions`). Calling them means adding that package reference and an `.imodspec` `<dependency>` on `Intent.Application.DomainInteractions`, so every application that installs your module also gets that module. Each one is a single filter on the mapping type name (`"Update Entity Mapping"`, `"Query Entity Mapping"`), so write the filter inline as above unless you already depend on that module.

When creating custom mapping statements manually, source and target expressions must still come from mapping-resolved statements:

```csharp
var assignment = new CSharpAssignmentStatement(
    mapping.GetTargetStatement(),
    mapping.GetSourceStatement());
```

## 3. Recursive Patterns

### A. Scalar leaves (Terminal Mapping)

- Condition: no child mappings and direct source/target mapped ends.
- Output: one `CSharpAssignmentStatement`.

```csharp
yield return new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement());
```

### B. Nested objects (Object Mapping)

- Condition: traversal node or complex non-leaf node.
- Output: null-guarded initialization + recursive child statements.
- Written inside a `CSharpMappingBase` subclass (`GetSourcePathText`, `GetSourcePath` and `GetTargetPathText` are protected members); `targetTypeName` comes from `Template.GetTypeName(...)`.

```csharp
var ifStatement = new CSharpIfStatement($"{GetSourcePathText(GetSourcePath(), true)} != null");
ifStatement.AddStatement($"{GetTargetPathText()} ??= new {targetTypeName}();");
ifStatement.AddStatements(Children.SelectMany(x => x.GetMappingStatements()));
yield return ifStatement;
```

### C. Collections (Object Mapping)

- Condition: collection type reference on the current mapping node.
- Output: collection reconciliation helper call with recursive update callback.
- Don't hand-write this: resolve the node to the SDK's `ObjectUpdateMapping`, which emits `UpdateHelper.CreateOrUpdateCollection(target, source, (e, d) => <primary-key comparison>, CreateOrUpdate<Entity>)` and adds the `CreateOrUpdate<Entity>` helper method in an `AfterBuild` callback.
- `ObjectUpdateMapping` resolves the helper through the template role `"Domain.Common.UpdateHelper"` and the entity class through `"Domain.Entity.Primary"` (and `"Domain.Entity.Interface"` when entity interfaces are on), so the consuming application needs templates in those roles (in Intent's .NET stack the `Intent.Entities` module provides both).

### D. Null handling

There are no "Null-Safe" or "Validate All" mapping options. Null handling is done with these members:

- `GetSourceStatement(true)`: emits `?.` on nullable steps of the source path. With no argument it follows the nullability of the mapped target. Pass the flag positionally: the parameter is named `targetIsNullable` before Intent.Common.CSharp 3.9.2 and `withNullConditionalOperators` from 3.9.2.
- `GetTargetStatement(true)` and the protected `GetTargetPathText(true)`: the same for the target path (Intent.Common.CSharp 3.9.2+; at 3.8.0 only the parameterless forms exist).
- Protected `GetSourcePathText(path, targetIsNullable)`: the source path text with null-conditional access, used in pattern B's guard.
- Protected `GetNullableAwareInstantiation(model, children, instantiationStatement)`: wraps an instantiation in `source is not null ? ... : null` when a nullable target is built from a nested nullable source.
- `IfNotNullMapping` (Intent.Common.CSharp 3.9.2+): wraps another mapping in `if (source is not null) { ... }`. Return it from a resolver to guard a node: `new IfNotNullMapping(mappingModel, template, next?.Invoke(mappingModel))`.

## 4. §Resolvers

A resolver decides which `ICSharpMapping` a node of the mapping tree becomes. The manager offers every node, terminal and object alike, to its resolvers in priority order.

### A. Skeleton IMappingTypeResolver

```csharp
public class CustomUpdateResolver : IMappingTypeResolver
{
    // The id of the designer mapping type this resolver handles. Read it from the designer that
    // defines the mapping type, or log mappingModel.MappingTypeId once; never invent it.
    private const string UpdateMappingTypeId = "<mapping type id from the designer>";

    private readonly ICSharpFileBuilderTemplate _template;

    public CustomUpdateResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
    {
        // Match on the mapping type id, not on names. No `Mapping == null` guard:
        // object nodes have no Mapping and must reach this resolver too.
        if (mappingModel.MappingTypeId != UpdateMappingTypeId)
        {
            return next?.Invoke(mappingModel);
        }

        if (mappingModel.Model.SpecializationType == "Association Target End")
        {
            return new ObjectUpdateMapping(mappingModel, _template);
        }

        return next?.Invoke(mappingModel); // Not ours: hand it on.
    }
}
```

- Implement the two-argument overload. The one-argument `ResolveMappings(MappingModel)` is obsolete; it stays only so pre-3.9.2 resolvers keep working.
- For anything you don't handle, return `next?.Invoke(mappingModel)`, never `null`. When a resolver returns `null`, the manager calls every later resolver through the one-argument overload, which passes a null `next`, and a resolver that then calls `next(...)` throws a `NullReferenceException`. Some of Intent's own resolvers return `null` for mapping types they don't handle, which is why the skeleton uses `?.Invoke` rather than `next(...)`.
- `next` returns whatever the rest of the chain resolves (or `null` if nothing claims the node, in which case the manager falls back to its defaults), so you can also wrap it: `new IfNotNullMapping(mappingModel, _template, next?.Invoke(mappingModel))`.

Register the resolver before generation:

```csharp
var manager = new CSharpClassMappingManager(template);
manager.AddMappingResolver(new CustomUpdateResolver(template), priority: -100);
var statements = manager.GenerateUpdateStatements(updateMapping);
```

- Lower priority runs first. `AddMappingResolver(resolver)` registers at the default, 0, and resolvers with equal priority run in registration order.
- The first resolver in the order gets the first chance to claim a node. To take over a node that another module's resolver already handles at the default, register with a negative priority. Intent's own modules also use negative values, down to -10, so go well below that (for example -100) when you need to run before them; at equal priority, registration order decides.

> [!IMPORTANT]
> The two-argument `ResolveMappings(mappingModel, next)` needs `Intent.Common.CSharp` 3.9.2 or later, and `AddMappingResolver(resolver, priority)` needs 3.9.1 or later. Module Builder's default floor is 3.8.0, where neither exists (a build fails with `CS0246`, or `CS1739` for the named `priority:` argument). Raise the `Intent.Common.CSharp` `<dependency>` in the module's `.imodspec` and the `Intent.Modules.Common.CSharp` package reference in its `.csproj` to 3.9.2 or later. Raise `Intent.Modules.Common` (`.csproj`) and the `Intent.Common` `<dependency>` (`.imodspec`) with it, to at least the `Intent.Modules.Common` version that package depends on: 3.7.3 for 3.9.2, more for later versions (the error names it). From `Intent.Modules.Common` 3.9.0 the `Intent.SoftwareFactory.SDK` reference must rise too (3.10.0 for 3.9.0 and 3.9.1, more for later versions); the next `NU1605` names it. Module Builder writes 3.7.2, so otherwise restore fails with `NU1605: Detected package downgrade` (see the `known-build-gotchas` instructions).
>
> If the module must stay on 3.8.0, implement only `ResolveMappings(MappingModel)`, return `null` for nodes you don't handle, and register with `AddMappingResolver(resolver)`. Resolvers then run in registration order and the first non-null result wins.

### B. Recursive child aggregation pattern

Inside a `CSharpMappingBase`, aggregate child statements with:

```csharp
var childStatements = Children.SelectMany(x => x.GetMappingStatements());
```

There is no `GetUpdateStatements()` on a mapping node; `GetMappingStatements()` is the member. The full inheritance pattern is the `AssignOrRecurseMapping` sample in section 1.

## 5. `IElementToElementMapping` and `MappedEnds`

`IElementToElementMapping` is the raw designer record. `IElementToElementMappedEnd` (accessed via `.MappedEnds`) represents a single drawn source→target connection.

### Accessing the mapping from a model element

```csharp
// Retrieve by mapping type name (see section 2 for why this is inline rather than GetUpdateEntityMapping()):
IElementToElementMapping updateMapping = updateAction.Mappings.SingleOrDefault(x => x.Type == "Update Entity Mapping");
IElementToElementMapping queryMapping  = interaction.Mappings.SingleOrDefault(x => x.Type == "Query Entity Mapping");

// For single-mapping actions:
IElementToElementMapping mapping = createAction.Mappings.Single();
```

### Using `MappedEnds` for query predicates

`MappedEnds` gives you the flat list of drawn lines. Use it when building filter expressions, not for recursive code generation:

```csharp
// Build a predicate from query mapping ends:
var predicate = string.Join(" && ", queryMapping.MappedEnds
    .Where(x => x.SourceElement != null)
    .Select(x => $"x.{x.TargetElement.Name} == {mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}"));
// Result: "x.CustomerId == request.CustomerId && x.Status == request.Status" (add the "x => " yourself)
```

### Using `MappedEnds` to detect operation targets

`IsOperationModel()` comes from `Intent.Modelers.Domain.Api` (the `Intent.Modelers.Domain` module, NuGet package `Intent.Modules.Modelers.Domain`), so this needs that dependency.

```csharp
foreach (var end in updateMapping.MappedEnds.Where(me => me.TargetElement.IsOperationModel()))
{
    var operationName = ((IElement)end.TargetElement).Name;
    // post-process the generated invocation statement
}
```

### `MappingModel` construction recap

`new MappingModel(mapping, manager)` groups `mapping.MappedEnds` by `TargetPath` depth — one level per recursive `Children` expansion. This is what each resolver receives; you never construct it manually in normal usage.

## 6. Associations in Mappings

When the designer maps through a navigation property, the `MappingModel` node for that navigation property has:

```csharp
model.SpecializationType == "Association Target End"
model.TypeReference.Element.SpecializationType  // "Class" or "Value Object"
model.TypeReference.IsCollection                // true = collection, false = single
```

Gate your resolver to return the right mapping type:

```csharp
public ICSharpMapping ResolveMappings(MappingModel mappingModel, MappingTypeResolverDelegate next)
{
    if (mappingModel.MappingTypeId != UpdateMappingTypeId)
    {
        return next?.Invoke(mappingModel);
    }

    var model = mappingModel.Model;

    // Navigation to a single entity or entity collection
    if (model.SpecializationType == "Association Target End"
        && model.TypeReference?.Element?.SpecializationType == "Class")
    {
        return new ObjectUpdateMapping(mappingModel, _template);
    }

    // Navigation to a collection of value objects
    if (model.SpecializationType == "Association Target End"
        && model.TypeReference?.Element?.SpecializationType == "Value Object"
        && model.TypeReference.IsCollection)
    {
        return new ValueObjectCollectionUpdateMapping(mappingModel, _template);
    }

    return next?.Invoke(mappingModel);
}
```

`ObjectUpdateMapping` handles the `IsCollection == true` case automatically: it emits a `UpdateHelper.CreateOrUpdateCollection(target, source, pkComparison, CreateOrUpdateMethod)` call and generates the `CreateOrUpdate{Entity}` helper method via an `AfterBuild` callback (see section 3C for the template roles it needs).

## Implementation Notes

- Use MappingManager (`CSharpClassMappingManager`) generation APIs for final statement production.
- Resolve all paths from mapping metadata (source/target element IDs and paths), not string guesses.
- Custom mapping types implement `ICSharpMapping`, normally by inheriting `CSharpMappingBase`.
- Resolvers built on the two-argument overload need `Intent.Common.CSharp` 3.9.2 or later, with `Intent.Modules.Common` / `Intent.Common` raised to 3.7.3 alongside it; a later version can also need the `Intent.SoftwareFactory.SDK` raise described in section 4.
- Interaction-story orchestration belongs to `intent-domain-interactions-expert`; this cheatsheet stays focused on mapping statement generation.

## Further Reading

- Tutorial: Advanced Mapping: <https://docs.intentarchitect.com/articles/module-building/tutorial-advanced-mapping/tutorial-advanced-mapping.html>
- Designer Scripting: Advanced Mappings: <https://docs.intentarchitect.com/articles/module-building/designers/designer-scripting-advanced-mappings/designer-scripting-advanced-mappings.html>
- Intent.Application.DomainInteractions (owner of `GetUpdateEntityMapping()` / `GetQueryEntityMapping()`): <https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-domaininteractions/intent-application-domaininteractions.html>
