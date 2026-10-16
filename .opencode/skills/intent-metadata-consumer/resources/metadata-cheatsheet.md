---
contentHash: 60C5E39ACA34F737A76909613581CB89ADE62CEE72B73400F120B3E2D073200A
---
# Intent Metadata Consumer Cheatsheet

The examples use one placeholder stereotype, **My Settings**, applied to `ClassModel`. `GetMySettings()`,
`HasMySettings()`, `TryGetMySettings()` and the `MySettings` wrapper are what Module Builder generates in
*your* module once you model that stereotype — substitute your own stereotype and property names.

## Stereotype Property → C# Builder Action Map

| Property Type | Typed Accessor | Builder Action | Example |
|---|---|---|---|
| **Checkbox → `bool`** | `model.GetMySettings()?.IsTimestamped()` | Conditional `AddAttribute` / gate entire block | `if (model.GetMySettings()?.IsTimestamped() == true) { @class.AddAttribute(UseType("MyModule.TimestampedAttribute")); }` |
| **Text → `string`** | `model.GetMySettings()?.SerializedName()` | `AddAttribute(name, value)` / `AddArgument($"\"...\"")` | `prop.AddAttribute("JsonPropertyName", a => a.AddArgument($"\"{serializedName}\""));` |
| **Text → XML doc** | `model.GetMySettings()?.ExampleValue()` | `WithComments(...)` | `if (model.GetMySettings()?.ExampleValue() is { } example) { prop.WithComments($"/// <example>{example}</example>"); }` |
| **Number → `int?`** | `model.GetMySettings()?.MaxLength()` | Null-gate, then `AddChainStatement` / `AddArgument` | `if (model.GetMySettings()?.MaxLength() is { } maxLength) { chain.AddChainStatement($"MaximumLength({maxLength})"); }` |
| **Number → `int?` (optional)** | `model.GetMySettings()?.Priority()` | Null-gate, then `AddArgument` | `if (model.GetMySettings()?.Priority() is { } p) { invoc.AddArgument($"priority: {p}"); }` |
| **Select → options wrapper** | `model.GetMySettings()?.Mode().AsEnum()` | switch-on-enum | `switch (model.GetMySettings()?.Mode().AsEnum()) { case MySettings.ModeOptionsEnum.Basic: ... }` |
| **Select → boolean helper** | `model.GetMySettings()?.Mode().IsExtended()` | Single-branch guard | `if (model.GetMySettings()?.Mode().IsExtended() == true) { ... }` |
| **Element lookup → `IElement`** | `model.GetMySettings()?.Provider()` | Type-resolve + conditional code branch | `var providerEl = model.GetMySettings()?.Provider(); // branch on providerEl?.SpecializationType` |

Module Builder generates **Number** as `int?` and **Decimal** as `decimal?` — never use either value without a
null check.

===

## Strongly-Typed Extension Anatomy

Generated extension files (`<ModelClass>StereotypeExtensions.cs`) expose three tiers of API for each stereotype:

```csharp
// Tier 1 — existence check (use for filtering/guard)
public static bool HasMySettings(this ClassModel model) { ... }

// Tier 2 — typed accessor (returns wrapper or null)
public static MySettings GetMySettings(this ClassModel model)
{
    var stereotype = model.GetStereotype(MySettings.DefinitionId);
    return stereotype != null ? new MySettings(stereotype) : null;
}

// Tier 3 — TryGet pattern (preferred when consuming inside loops)
public static bool TryGetMySettings(
    this ClassModel model, out MySettings stereotype) { ... }
```

The wrapper class `MySettings` is nested inside the extensions class, and each Select property's
`<Property>Options` class and `<Property>OptionsEnum` are nested inside the wrapper.

===

## Consuming Enum Options

```csharp
// DON'T — raw string comparison
if (model.GetMySettings()?.Mode().Value == "Extended") { }

// DO — enum helper
if (model.GetMySettings()?.Mode().IsExtended() == true) { }

// DO — discriminated switch on the nested enum
switch (model.GetMySettings()?.Mode().AsEnum())
{
    case MySettings.ModeOptionsEnum.Basic:
        // generate the basic shape
        break;
    case MySettings.ModeOptionsEnum.Extended:
        // generate the extended shape
        break;
}
```

`AsEnum()` throws `ArgumentOutOfRangeException` for a value outside the modelled options, including an
unset (null) value — use the `IsX()` helpers when the property may be empty.

===

## Filtering Model Collections

### Simple flag (use typed IsXxx where available)

```csharp
var aggregates = domain.GetClassModels()
    .Where(x => x.IsAggregateRoot())
    .ToArray();
```

### Composite condition

```csharp
// HasTable() is generated in Intent.Metadata.RDBMS — reading it needs a dependency on that module.
var repositoryTargets = _metadataManager.Domain(application).GetClassModels()
    .Where(x => (x.IsAggregateRoot() && !x.IsAbstract) || x.HasTable())
    .ToArray();
```

`GetClassModels()`, `Domain(...)` and `IsAggregateRoot()` come from `Intent.Modules.Modelers.Domain`.

===

## Intent Model Wrapper Hierarchy

Every designer element in Intent Architect is surfaced to code generation as a **typed model wrapper** around the raw `IElement` (from `Intent.SoftwareFactory.SDK`).

### Base Interfaces

| Interface | Namespace (package) | Contract |
|---|---|---|
| `IMetadataModel` | `Intent.Metadata.Models` (`Intent.SoftwareFactory.SDK`) | `string Id { get; }` |
| `IHasStereotypes` | `Intent.Metadata.Models` (`Intent.SoftwareFactory.SDK`) | `IEnumerable<IStereotype> Stereotypes { get; }` |
| `IHasName` | `Intent.Metadata.Models` (`Intent.SoftwareFactory.SDK`) | `string Name { get; }` |
| `IHasTypeReference` | `Intent.Metadata.Models` (`Intent.SoftwareFactory.SDK`) | `ITypeReference TypeReference { get; }` |
| `IElementWrapper` | `Intent.Modules.Common` (`Intent.Modules.Common`) | `IElement InternalElement { get; }` |
| `IHasFolder` | `Intent.Modules.Common.Types.Api` (`Intent.Modules.Common.Types`) | `FolderModel Folder { get; }` |

### Domain Modeler Model Types (namespace `Intent.Modelers.Domain.Api`, package `Intent.Modules.Modelers.Domain`)

| Model Class | Implements (abridged) | Key Typed Children |
|---|---|---|
| `ClassModel` | `IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper` | `Attributes`, `Operations`, `Constructors`, `AssociatedClasses` |
| `AttributeModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Class` (parent) |
| `OperationModel` | `IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference` | `Parameters` |
| `AssociationEndModel` | `ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper` | Directly IS a `ITypeReference` |

===

## Creating a Missing Typed Extension

### Step 1 — Immediate Fallback (GUID-based)

```csharp
private const string MyStereotypeDefinitionId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
if (!model.HasStereotype(MyStereotypeDefinitionId)) return;
var stereotype = model.GetStereotype(MyStereotypeDefinitionId);
bool isEnabled    = stereotype.GetProperty<bool>("Is Enabled");
```

### Step 2 — Generate the Typed Extension (never hand-write it)

Model the stereotype in the **Module Builder designer** (a Stereotype Definition with its target element
types and properties) and run the Software Factory. Module Builder then generates
`<ModelClass>StereotypeExtensions.cs` with the `Has` / `Get` / `TryGet` trio and the wrapper class shown in
*Strongly-Typed Extension Anatomy*, and you replace the GUID fallback with the typed accessor.

Do not write that file by hand. Generated extension files carry the generator's assembly attribute,
`[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]`,
and belong to the Software Factory — a hand-written copy is overwritten on the next run, or drifts out of
step with the modelled stereotype.

===

## Shared Vocabulary Layer (`Intent.Modules.Common.Types`)

| Type | Specialization | Key Members |
|---|---|---|
| `FolderModel` | `"Folder"` | `Folder` (parent), `Folders` (children) |
| `EnumModel` | `"Enum"` | `Literals` (`IList<EnumLiteralModel>`) |
| `EnumLiteralModel` | `"Enum-Literal"` | `Name`, `Value` |

### Primitive Type Checks (`TypeCheckExtensions`, namespace `Intent.Modules.Common`)

```csharp
bool isString  = typeRef.Element.IsStringType();
bool isGuid    = typeRef.Element.IsGuidType();
bool isInt     = typeRef.Element.IsIntType();
```
