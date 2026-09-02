---
contentHash: 38EEE4B902197FAB4E94911DFD66D97568D335DD10CA59586A00086C46156251
---
# Intent Mapping Architect Cheatsheet

## Terminology Bridge

- Map Via -> Invocation Mapping
- Data Type Mapping -> Data Mapping

Use the current terms in implementation logic and docs, while preserving alias awareness for legacy discussions.

## 1. The Core Loop

Use recursive traversal over child mapping nodes to decide whether each node is terminal (leaf assignment) or object-level (nested recursion).

```csharp
// Conceptual loop: some APIs expose ChildMappings; in the current C# engine this is represented as Children.
IEnumerable<CSharpStatement> BuildStatements(MappingNode mapping)
{
    // Object mapping / traversal node
    if (mapping.Mapping == null)
    {
        foreach (var child in mapping.ChildMappings) // alias to engine-level children collection
        {
            foreach (var statement in BuildStatements(child))
            {
                yield return statement;
            }
        }

        yield break;
    }

    // Terminal mapping / leaf assignment
    yield return new CSharpAssignmentStatement(
        mapping.GetTargetStatement(),
        mapping.GetSourceStatement());
}
```

### Practical engine pattern

- If node is traversal-only: recurse through child nodes.
- If node is scalar terminal: emit one assignment.
- If node is object/collection: emit guarded object update or helper call, then recurse.

## 2. Resolution

Use `GetUpdateEntityMapping()` to locate the update mapping, then generate update assignments through MappingManager resolution.

```csharp
var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();
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

```csharp
var ifStatement = new CSharpIfStatement($"{GetSourcePathText(GetSourcePath(), true)} != null");
ifStatement.AddStatement($"{GetTargetPathText()} ??= new {targetTypeName}();");
ifStatement.AddStatements(Children.SelectMany(x => x.GetMappingStatements()));
yield return ifStatement;
```

### C. Collections (Object Mapping)

- Condition: collection type reference on the current mapping node.
- Output: collection reconciliation helper call with recursive update callback.

```csharp
yield return new CSharpAssignmentStatement(
    GetTargetStatement(),
    $"{updateHelperType}.CreateOrUpdateCollection({GetTargetPathText()}, {GetSourcePathText()}, (e, d) => {GetPrimaryKeyComparisonMappings()}, CreateOrUpdate{elementName})");
```

### D. Null-Safe and Validate All (MappingOptions)

- Always read and honor mapping options before emitting code.
- Null-Safe: emit guards and null-conditional behavior for nullable paths.
- Validate All: emit/retain validation flow so required mapping constraints are enforced.

## 4. §Resolvers

### A. Skeleton IMappingTypeResolver

```csharp
public class CustomUpdateResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CustomUpdateResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Mapping == null)
        {
            return null;
        }

        // Match your mapping type/specialization here.
        if (mappingModel.Mapping.Type == "Update Entity Mapping")
        {
            return new ObjectUpdateMapping(mappingModel, _template);
        }

        return null; // Delegate to next resolver in pipeline.
    }
}
```

Register the resolver before generation:

```csharp
var manager = new CSharpClassMappingManager(template);
manager.AddMappingResolver(new CustomUpdateResolver(template), priority: 100);
var statements = manager.GenerateUpdateStatements(updateMapping);
```

### B. Recursive child aggregation pattern

Requested conceptual form:

```csharp
var childStatements = Children.SelectMany(x => x.GetUpdateStatements());
```

Current C# mapping engine equivalent:

```csharp
var childStatements = Children.SelectMany(x => x.GetMappingStatements());
```

Use the engine-equivalent form in concrete implementations.

### C. CSharpMappingBase inheritance pattern

```csharp
public class CustomObjectMapping : CSharpMappingBase
{
    public CustomObjectMapping(MappingModel model, ICSharpTemplate template)
        : base(model, template)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        if (Mapping == null)
        {
            return Children.SelectMany(x => x.GetMappingStatements()).ToList();
        }

        return new[]
        {
            new CSharpAssignmentStatement(GetTargetStatement(), GetSourceStatement())
        };
    }
}
```

## 5. `IElementToElementMapping` and `MappedEnds`

`IElementToElementMapping` is the raw designer record. `IElementToElementMappedEnd` (accessed via `.MappedEnds`) represents a single drawn source→target connection.

### Accessing the mapping from a model element

```csharp
// Retrieve by type name (extension methods in MappingExtensions):
IElementToElementMapping? updateMapping = updateAction.Mappings.GetUpdateEntityMapping();
IElementToElementMapping? queryMapping  = interaction.Mappings.GetQueryEntityMapping();

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
// Result: "x => x.CustomerId == request.CustomerId && x.Status == request.Status"
```

### Using `MappedEnds` to detect operation targets

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
public ICSharpMapping? ResolveMappings(MappingModel mappingModel)
{
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

    return null;
}
```

`ObjectUpdateMapping` handles the `IsCollection == true` case automatically: it emits a `UpdateHelper.CreateOrUpdateCollection(target, source, pkComparison, CreateOrUpdateMethod)` call and generates the `CreateOrUpdate{Entity}` helper method via an `AfterBuild` callback.

## Implementation Notes

- Use MappingManager (`CSharpClassMappingManager`) generation APIs for final statement production.
- Resolve all paths from mapping metadata (source/target element IDs and paths), not string guesses.
- Any custom recursive mapping statement type must implement `IHasMapping` to stay mapping-aware.
- Interaction-story orchestration belongs to `intent-domain-interactions-expert`; this cheatsheet stays focused on mapping statement generation.

## Source of Truth

- Repository: <https://github.com/IntentArchitect/Intent.Modules>
- Documentation: <https://github.com/IntentArchitect/Docs>
