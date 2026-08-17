---
contentHash: 63482D0C1192A6F7C0779EC11BD174D1C97B834C269295CD55CEFAD66BF3A172
---
# Intent Metadata Consumer Cheatsheet

## Stereotype Property → C# Builder Action Map

| Property Type | Typed Accessor | Builder Action | Example |
|---|---|---|---|
| **Bool** | `model.GetXxx().IsRequired()` | Conditional `AddAttribute` / gate entire block | `if (model.GetScheduling().DisallowConcurrentExecution()) { @class.AddAttribute(UseType("Quartz.DisallowConcurrentExecution")); }` |
| **Bool** | `model.GetXxx().HasIndexed()` | Conditional `AddAttribute` | `if (attr.HasIndexed()) { prop.AddAttribute(UseType("Redis.OM.Modeling.Indexed")); }` |
| **String (literal)** | `model.GetXxx().Name()` | `AddAttribute(name, value)` / `AddArgument($"\"...\"")` / `WithComments(...)` | `prop.AddAttribute($"JsonPropertyName", a => a.AddArgument($"\"{serializedName}\""));` |
| **String (XML doc)** | `model.GetXxx().ExampleValue()` | `WithComments(xmlComments: ...)` | `prop.WithComments($"/// <example>{model.GetOpenApiSettings().ExampleValue()}</example>");` |
| **Int (constraint)** | `model.GetXxx().MaxLength()` | `AddChainStatement($"MaximumLength({value})")` or `AddArgument($"{value}")` | `chain.AddChainStatement($"MaximumLength({attr.GetTextConstraints().MaxLength()})");` |
| **Int? (optional)** | `model.GetXxx().Priority()` | Null-gate then `AddArgument` | `if (msg.GetXxx().Priority() is {} p) { invoc.AddArgument($"priority: {p}"); }` |
| **Enum wrapper** | `model.GetXxx().TemplatingMethod().AsEnum()` | switch-on-enum | `switch (model.GetFileSettings().TemplatingMethod().AsEnum()) { case T4Template: ... }` |
| **Enum boolean** | `model.GetXxx().TemplatingMethod().IsT4Template()` | Single-branch guard | `if (model.GetFileSettings().TemplatingMethod().IsT4Template()) { ... }` |
| **IElement ref** | `model.GetXxx().Provider()` | Type-resolve + conditional code branch | `var providerEl = pkg.GetDocumentDatabase().Provider(); // branch per element SpecializationType` |

- --

## Strongly-Typed Extension Anatomy

Generated extension files expose three tiers of API for each stereotype:

```csharp
// Tier 1 — existence check (use for filtering/guard)
public static bool HasComponentSettings(this ComponentModel model) { ... }

// Tier 2 — typed accessor (returns wrapper or null)
public static ComponentSettings GetComponentSettings(this ComponentModel model)
{
    var stereotype = model.GetStereotype(ComponentSettings.DefinitionId);
    return stereotype != null ? new ComponentSettings(stereotype) : null;
}

// Tier 3 — TryGet pattern (preferred when consuming inside loops)
public static bool TryGetComponentSettings(
    this ComponentModel model, out ComponentSettings stereotype) { ... }
```

- --

## Consuming Enum Options

```csharp
// DON'T — raw string comparison
if (model.GetFileSettings().TemplatingMethod().Value == "T4 Template") { }

// DO — enum helper
if (model.GetFileSettings().TemplatingMethod().IsT4Template()) { }

// DO — discriminated switch on full enum
switch (model.GetFileSettings().TemplatingMethod().AsEnum())
{
    case TemplatingMethodOptionsEnum.T4Template:
        // generate T4 registration
        break;
    case TemplatingMethodOptionsEnum.CSharpFileBuilder:
        // generate CSharpFile registration
        break;
}
```

- --

## Filtering Model Collections

### Simple flag (use typed IsXxx where available)

```csharp
var aggregates = domain.GetClassModels()
    .Where(x => x.IsAggregateRoot())
    .ToArray();
```

### Composite condition

```csharp
var repositoryTargets = _metadataManager.Domain(application).GetClassModels()
    .Where(x => (x.IsAggregateRoot() && (!x.IsAbstract || x.HasStereotype("Table")))
                || x.HasRepository())
    .ToArray();
```

- --

## Intent Model Wrapper Hierarchy

Every designer element in Intent Architect is surfaced to code generation as a **typed model wrapper** around the raw `IElement` (from `Intent.SoftwareFactory.SDK`).

### SDK Base Interfaces (`Intent.Metadata.Models`)

| Interface | Contract |
|---|---|
| `IMetadataModel` | `string Id { get; }` |
| `IHasStereotypes` | `IEnumerable<IStereotype> Stereotypes { get; }` |
| `IHasName` | `string Name { get; }` |
| `IHasTypeReference` | `ITypeReference TypeReference { get; }` |
| `IElementWrapper` | `IElement InternalElement { get; }` |
| `IHasFolder` | `FolderModel Folder { get; }` |

### Domain Modeler Model Types (`Intent.Modules.Modelers.Domain`)

| Model Class | Implements | Key Typed Children |
|---|---|---|
| `ClassModel` | `IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper` | `Attributes`, `Operations`, `Constructors`, `AssociatedClasses` |
| `AttributeModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Class` (parent) |
| `OperationModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Parameters` |
| `AssociationEndModel` | `ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper` | Directly IS a `ITypeReference` |

- --

## Creating a Missing Typed Extension

### Step 1 — Immediate Fallback (GUID-based)

```csharp
private const string MyStereotypeDefinitionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
if (!model.HasStereotype(MyStereotypeDefinitionId)) return;
var stereotype = model.GetStereotype(MyStereotypeDefinitionId);
bool isEnabled    = stereotype.GetProperty<bool>("Is Enabled");
```

### Step 2 — Promote to a Full Typed Extension in `Api/Extensions/`

```csharp
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

public static class ClassModelStereotypeExtensions
{
    public static bool HasMyStereotype(this ClassModel model)
        => model.HasStereotype(MyStereotype.DefinitionId);

    public static MyStereotype GetMyStereotype(this ClassModel model)
    {
        var stereotype = model.GetStereotype(MyStereotype.DefinitionId);
        return stereotype != null ? new MyStereotype(stereotype) : null;
    }

    public class MyStereotype
    {
        private readonly IStereotype _stereotype;
        public const string DefinitionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        public MyStereotype(IStereotype stereotype) => _stereotype = stereotype;
        public bool IsEnabled() => _stereotype.GetProperty<bool>("Is Enabled");
    }
}
```

- --

## Shared Vocabulary Layer (`Intent.Modules.Common.Types`)

| Type | Specialization | Key Members |
|---|---|---|
| `FolderModel` | `"Folder"` | `Folder` (parent), `Folders` (children) |
| `EnumModel` | `"Enum"` | `Literals` (`IList<EnumLiteralModel>`) |
| `EnumLiteralModel` | `"Enum-Literal"` | `Name`, `Value` |

### Primitive Type Checks (`TypeCheckExtensions`)

```csharp
bool isString  = typeRef.Element.IsStringType();
bool isGuid    = typeRef.Element.IsGuidType();
bool isInt     = typeRef.Element.IsIntType();
```
