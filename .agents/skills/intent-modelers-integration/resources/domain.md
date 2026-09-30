---
contentHash: AFCDCD3DB80D3169106807E6902A4E6DDC277A3362F6BE7223E19AEA25D0A612
---
# Domain Designer (`Intent.Modules.Modelers.Domain`)

## 1. What this designer is for

The Domain designer models an application's domain model: entities (classes), their attributes,
operations, constructors, generalization (inheritance) hierarchies, associations between classes,
comments, diagrams, and lightweight DTO-style "Data Contracts". It is the canonical place where a
solution's domain entities and their relationships are modelled — most other designers and modules
(repositories, RDBMS mapping, document DB mapping, domain events, value objects, domain services,
stored procedures, service proxies) attach to or read from this model rather than duplicating it.

Consume this designer when your module needs to read or react to the shape of the user's domain
entities — e.g. to generate EF Core entity classes, repository interfaces, DTOs, database
mappings, or any code that mirrors "the domain model" itself.

**What it is not:** it has no mapping canvas of its own — the `IsMapped`/`Mapping` properties seen
on some of its elements only report that some *other* designer has mapped onto them; Domain is the
mapping target, never the mapping author. It also defines no stereotypes of its own — persistence
concerns (RDBMS columns/keys/indexes, Document DB providers) and validation constraints are added
by separate `Intent.Metadata.*` modules layered on top (§6). Domain Events, Value Objects,
Repositories, Domain Services and Stored Procedures are separate sibling modules that *extend*
Domain's `FolderModel`/`DomainPackageModel` with their own element types (`domain-extensions.md`) — they are not part
of the base designer.

**Extension modules — read `domain-extensions.md` for these.** Each is installed separately, and its
install identity and full model API are in that file, not this one: Domain Events (`DomainEventModel`,
`DomainEventHandlerModel`), Repositories (`RepositoryModel`), Domain Services (`DomainServiceModel`),
Stored Procedures (`StoredProcedureModel`, `StoredProcedureParameterModel`, `StoredProcedureInvocationModel`)
and Value Objects (`ValueObjectModel`).

## 2. Install identity

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.Domain` |
| Intent module id | `Intent.Modelers.Domain` |
| API namespace | `Intent.Modelers.Domain.Api` |
| Assembly | `Intent.Modules.Modelers.Domain.dll` |
| Designer GUID | `6ab29b31-27af-4f56-a67c-986d82097d63` |

Verified against `Intent.Modules.Modelers.Domain.csproj` (`PackageId`/`RootNamespace`),
`Intent.Modelers.Domain.imodspec`, and `ApiMetadataDesignerExtensions.cs`
(`DomainDesignerId` constant + the `.Domain(...)` accessor).

## 3. Entry points

```csharp
using Intent.Modelers.Domain.Api;

IDesigner domain = metadataManager.Domain(application); // or metadataManager.Domain(applicationId)

IList<ClassModel>        classes   = domain.GetClassModels();
IList<CommentModel>      comments  = domain.GetCommentModels();
IList<DataContractModel> contracts = domain.GetDataContractModels();
IList<DiagramModel>      diagrams  = domain.GetDiagramModels();

// Package-model accessor
IList<DomainPackageModel> packages = domain.GetDomainPackageModels();
```

`DomainPackageModel` (the package/root wrapper) additionally exposes typed children directly:
`Classes`, `Comments`, `Diagrams`, `Enums`, `Folders`, `DomainContracts`/`DomainObjects` (both
aliases returning the same `DataContractModel` list — a pre-existing quirk), and `Types`
(`TypeDefinitionModel`, from the shared `Intent.Modules.Common.Types` vocabulary, not Domain's own).

## 4. Designer elements

| Model class | `SpecializationType` | `SpecializationTypeId` | Represents | Key navigation |
|---|---|---|---|---|
| `ClassModel` | `Class` | `04e12b51-ed12-42a3-9667-a6aa81bb6d10` | A domain entity/class | `Attributes`, `Operations`, `Constructors`, `ParentClass`/`ChildClasses`, `AssociatedClasses`/`AssociationEnds()`, `Folder` |
| `AttributeModel` | `Attribute` | `0090fb93-483e-41af-a11d-5ad2dc796adf` | A field/property on a class | `TypeReference`, `Class` |
| `OperationModel` | `Operation` | `e042bb67-a1df-480c-9935-b26210f78591` | A method on a class | `Parameters`, `ReturnType`, `ParentClass`, mapping via `HasMapOperationMapping()` |
| `ParameterModel` | `Parameter` | `c26d8d0a-a26b-4b5f-b449-e9bdb60b3a4b` | A parameter on an operation or constructor | `TypeReference` |
| `ClassConstructorModel` | `Class Constructor` | `dec2bd12-4699-4f45-8ec9-3b62dc692d2b` | A constructor on a class | `Parameters`, `ParentClass`, mapping via `HasMapConstructorMapping()` |
| `DataContractModel` | `Data Contract` | `4464fabe-c59e-4d90-81fc-c9245bdd1afd` | A lightweight DTO-style contract | `Attributes`, `BaseDataContract` (via `Generalizations()`), `Folder` |
| `CommentModel` | `Comment` | `c4c0c77f-720b-4e91-9c48-b58d2164d30a` | A free-text annotation | `Folder`; linked via `CommentAssociationModel` |
| `DiagramModel` | `Diagram` | `4d66fecd-e9b8-436f-aa50-c59040ad0879` | A visual diagram grouping | `Folder` |
| `AssociationModel` | `Association` | `eaf9ed4e-0b61-4ac1-ba88-09f912c12087` | A relationship between two `ClassModel`s | `SourceEnd`, `TargetEnd`, computed `AssociationType` |
| `GeneralizationModel` | `Generalization` | `5de35973-3ac7-4e65-b48c-385605aec561` | Inheritance between two `ClassModel`s | `SourceEnd`, `TargetEnd` |
| `DataContractGeneralizationModel` | `Data Contract Generalization` | `4199ae15-0ecc-4086-82f3-bfa885c9d3e8` | Inheritance between two `DataContractModel`s | `SourceEnd`, `TargetEnd` |
| `CommentAssociationModel` | `Comment Association` | `5264c135-e856-468d-8bd7-154b75842256` | Links a `CommentModel` to the element it annotates | `SourceEnd`, `TargetEnd` |
| `DomainPackageModel` | `Domain Package` | `1a824508-4623-45d9-accc-f572091ade5a` | The root package | `Classes`, `Comments`, `Diagrams`, `Enums`, `Folders`, `DomainContracts`/`DomainObjects`, `Types` |
| `FolderExtensionModel` | *(extends shared `FolderModel`)* | *(inherits `FolderModel`'s id)* | Adds Domain-specific children to a folder | `Classes`, `Types`, `Enums`, `Comments`, `Diagrams`, `DomainContracts` |
| `IStaticConstructorModel` | — | — | Marker trait interface; **no element in this module currently implements it** — a forward-looking hook | — |

`ClassModel.IsAggregateRoot()` (in `Api/Extensions/ClassModelAssociationExtensions.cs`) is a
convenience predicate: a class with no owning (non-nullable, non-collection, source-end)
association pointing at it is treated as an Aggregate Root.

## 5. Associations

| Association | Source end | Target end | Between |
|---|---|---|---|
| `AssociationModel` (`eaf9ed4e-...`) | `AssociationSourceEndModel` (`8d9d2e5b-...`) | `AssociationTargetEndModel` (`0a66489f-...`) | `ClassModel` ↔ `ClassModel` |
| `GeneralizationModel` (`5de35973-...`) | `GeneralizationSourceEndModel` (`8190bf43-...`) | `GeneralizationTargetEndModel` (`4686cc1d-...`) | `ClassModel` ↔ `ClassModel` (inheritance) |
| `DataContractGeneralizationModel` (`4199ae15-...`) | `DataContractGeneralizationSourceEndModel` (`12c2ffdc-...`) | `DataContractGeneralizationTargetEndModel` (`4ea029c6-...`) | `DataContractModel` ↔ `DataContractModel` |
| `CommentAssociationModel` (`5264c135-...`) | `CommentSourceEndModel` (`7e98213c-...`) | `CommentTargetEndModel` (`b7edce45-...`) | `CommentModel` ↔ annotated element |

Each `AssociationModel` end carries `IsNullable`/`IsCollection`, from which `Multiplicity`
(`ZeroToOne`/`One`/`Many`) is computed. `AssociationModel.AssociationType` is likewise
**computed, not stored**: a non-nullable, non-collection source end is treated as `Composition`,
otherwise `Aggregation`.

Navigation extensions live in **`Api/Extensions/`** (not directly under `Api/`):

- `AssociationModelAssociationExtensions.cs`: `ClassModel.AssociatedToClasses()`, `.AssociatedFromClasses()`, `.AssociationEnds()`.
- `GeneralizationModelAssociationExtensions.cs`: `ClassModel.Generalizations()` (parent side), `.Specializations()` (child side), `.GeneralizationEnds()`.
- `DataContractGeneralizationModelAssociationExtensions.cs`: same shape for `DataContractModel`.
- `CommentAssociationModelAssociationExtensions.cs`: `CommentModel.CommentedClasses()`, `ClassModel.AssociatedComments()` (and an overload on `CommentModel` itself).
- `ClassModelAssociationExtensions.cs`: `ClassModel.IsAggregateRoot()`.

`ClassModel.ParentClass`/`ChildClasses` (declared directly on `ClassModel` itself) are convenience
wrappers over `Generalizations()`/`Specializations()`, returning `ClassModel` directly.

## 6. Stereotypes

Domain defines **no stereotypes of its own** — confirmed, zero `*StereotypeExtensions.cs` files
under its `Api/`. Stereotypes for Domain elements are added by separate downstream modules:

| Module | NuGet PackageId | Intent module id | Adds stereotypes for |
|---|---|---|---|
| Metadata RDBMS | `Intent.Modules.Metadata.RDBMS` | `Intent.Metadata.RDBMS` | `ClassModel` (table/key), `AssociationSourceEndModel`/`AssociationTargetEndModel` (foreign keys), `AttributeModel`, `FolderModel`, plus its own `IndexModel`/`IndexColumnModel`/`ClassExtensionModel`/`TriggerModel` |
| Metadata DocumentDB | `Intent.Modules.Metadata.DocumentDB` | `Intent.Metadata.DocumentDB` | `AttributeModel`, `AssociationTargetEndModel`, `DomainPackageModel`, plus its own `DocumentDbProviderModel` |
| Metadata Domain.Constraints | `Intent.Modules.Metadata.Domain.Constraints` | `Intent.Metadata.Domain.Constraints` | Validation/text constraints on `AttributeModel` (e.g. max length) |

All three drop the `Modules.` segment in their C# namespace (`Intent.Metadata.RDBMS.Api`, etc.) —
consistent with the Must #1 rule in `SKILL.md`, but confirm per-module rather than assume.

## 7. Mappings

Domain has no mapping/interaction designer surface of its own — no mapping canvas, no dedicated
mapping element types. Domain is always the mapping **target**, never a mapping **author**; that
role belongs to whichever designer maps onto it (e.g. Services' `.DomainInteractions`).

## 8. Worked snippet

```csharp
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace MyModule.Templates.Entities
{
  public class EntityTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
  {
      public const string TemplateId = "MyModule.Entity";

      public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
      {
          return new EntityTemplate(outputTarget, model);
      }

      public override IEnumerable<ClassModel> GetModels(IApplication application)
      {
          return application.MetadataManager.Domain(application).GetClassModels();
      }
  }
}
```

Factory-extension-style consumer, filtering to aggregate roots:

```csharp
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;

namespace MyModule.FactoryExtensions
{
  public class EntityRegistrationFactoryExtension : FactoryExtensionBase
  {
      public override IApplication Execute(IApplication application)
      {
          var aggregateRoots = application.MetadataManager
              .Domain(application)
              .GetClassModels()
              .Where(x => x.IsAggregateRoot() && !x.IsAbstract)
              .ToArray();

          foreach (var entity in aggregateRoots)
          {
              // e.g. register a repository, add a stereotype, etc.
          }

          return base.Execute(application);
      }
  }
}
```

## 9. Model API reference

Every public type and member that this designer ships — extracted
mechanically from source, not sampled. **Prefer this to reflecting over the assembly**
(`Assembly.LoadFrom`, `GetProperties()`, decompiling the NuGet DLL): if a member is not listed here,
it is not part of the version shown. If you are pinned to a different version and something seems
missing, check that package's own XML docs or `Api/` source rather than guessing a member name.

**How to read the stubs.** C#-style declarations; every member listed is `public`. A
`// "Name" · guid` line above a type is its `SpecializationType` · `SpecializationTypeId` pair. Three
things are stated once here instead of on every type:

- **`// + common element members`** = `string Id`, `string Name`, `string Comment`, `IEnumerable<IStereotype> Stereotypes`, and `IElement InternalElement` — the raw SDK element, the escape hatch for anything not surfaced as a typed property. Also present on every model: `ToString()`, value equality over the wrapped element (`==`, `!=`, `Equals`, `GetHashCode`), the `SpecializationType`/`SpecializationTypeId` consts, and a constructor taking `(IElement element, ...)`.
- **`Is<X>Model()` / `As<X>Model()`** — every element model `<X>Model` has `bool Is<X>Model(this ICanBeReferencedType)` and `<X>Model As<X>Model(this ICanBeReferencedType)` (returns `null` on a type mismatch) in a static `<X>ModelExtensions` class. They are omitted below unless that class holds more than these two. This is how you turn a raw reference into a typed model — `attribute.TypeReference.Element.AsClassModel()`. Prefer it to `new <X>Model(element)`: generated constructors throw on a specialization mismatch.
- **`// + common association-end members`** = the `ITypeReference` surface (`Element` — the element this end points at — plus `IsNullable`, `IsCollection`, `GenericTypeParameters`, `Stereotypes`), and `string Id`, `Name`, `Comment`, `SpecializationType`, `SpecializationTypeId`, `bool IsNavigable`, `ITypeReference TypeReference` (the end itself), `IPackage Package`, `IElement InternalElement`, `IAssociationEnd InternalAssociationEnd`, `IAssociation InternalAssociation`, the owning association model as `Association`, `OtherEnd()`, `IsSourceEnd()`, `IsTargetEnd()` and `static Create(IAssociationEnd)`.

`FolderModel`, `EnumModel`, `EnumLiteralModel` and `TypeDefinitionModel` come from
`Intent.Modules.Common.Types.Api`, not from this designer. They, and the SDK interfaces every model
returns (`IElement`, `ITypeReference`, `IStereotype`, `IElementMapping`, `IElementToElementMapping`,
`IDesigner`), are listed once in `integration-recipe.md` §6.

### 9.1 Domain — `Intent.Modules.Modelers.Domain` (source at `3.13.2`)

```csharp
namespace Intent.Modelers.Domain.Api;

public static class ApiMetadataDesignerExtensions
{
    const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
    static IDesigner Domain(this IMetadataManager metadataManager, IApplication application);
    static IDesigner Domain(this IMetadataManager metadataManager, string applicationId);
}

public static class ApiMetadataPackageExtensions
{
    static IList<DomainPackageModel> GetDomainPackageModels(this IDesigner designer);
    static bool IsDomainPackageModel(this IPackage package);
    static DomainPackageModel AsDomainPackageModel(this IPackage package);
}

public static class ApiMetadataProviderExtensions
{
    static IList<ClassModel> GetClassModels(this IDesigner designer);
    static IList<CommentModel> GetCommentModels(this IDesigner designer);
    static IList<DataContractModel> GetDataContractModels(this IDesigner designer);
    static IList<DiagramModel> GetDiagramModels(this IDesigner designer);
}

namespace Intent.Modules.Modelers.Domain.Settings;

public static class ModuleSettingsExtensions
{
    static DomainSettings GetDomainSettings(this IApplicationSettingsProvider settings);
}

public class DomainSettings : IGroupSettings
{
    string Id { get; }
    string Title { get; set; }
    ISetting GetSetting(string settingId);
    AttributeNamingConventionOptions AttributeNamingConvention();
    public class AttributeNamingConventionOptions
    {
        AttributeNamingConventionOptionsEnum AsEnum();
        bool IsManual();
        bool IsPascalCase();
        bool IsCamelCase();
    }
    public enum AttributeNamingConventionOptionsEnum { Manual, PascalCase, CamelCase }
    EntityNamingConventionOptions EntityNamingConvention();
    public class EntityNamingConventionOptions
    {
        EntityNamingConventionOptionsEnum AsEnum();
        bool IsManual();
        bool IsPascalCase();
        bool IsCamelCase();
    }
    public enum EntityNamingConventionOptionsEnum { Manual, PascalCase, CamelCase }
    OperationNamingConventionOptions OperationNamingConvention();
    public class OperationNamingConventionOptions
    {
        OperationNamingConventionOptionsEnum AsEnum();
        bool IsManual();
        bool IsPascalCase();
        bool IsCamelCase();
    }
    public enum OperationNamingConventionOptionsEnum { Manual, PascalCase, CamelCase }
}

namespace Intent.Modelers.Domain.Api;

// "Association" · eaf9ed4e-0b61-4ac1-ba88-09f912c12087
public class AssociationModel : IMetadataModel
{
    static AssociationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    AssociationSourceEndModel SourceEnd { get; }
    AssociationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
    AssociationType AssociationType { get; }
}

public class AssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
{
    // + common association-end members
    string Value { get; }
    ClassModel Class { get; }
    Multiplicity Multiplicity { get; }
}

// "Association Source End" · 8d9d2e5b-bd55-4f36-9ae4-2b9e84fd4e58
public class AssociationSourceEndModel : AssociationEndModel { /* common association-end members */ }

// "Association Target End" · 0a66489f-30aa-417b-a75d-b945863366fd
public class AssociationTargetEndModel : AssociationEndModel { /* common association-end members */ }

public enum AssociationType { Association, Aggregation, Composition, Generalization }

// "Attribute" · 0090fb93-483e-41af-a11d-5ad2dc796adf
public class AttributeModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
{
    // + common element members
    string Value { get; }
    ITypeReference TypeReference { get; }
    ITypeReference Type { get; }
    ClassModel Class { get; }
}

// "Class Constructor" · dec2bd12-4699-4f45-8ec9-3b62dc692d2b
public class ClassConstructorModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
    ClassModel ParentClass { get; }
    IList<ParameterModel> Parameters { get; }
    bool IsMapped { get; }
    IElementMapping Mapping { get; }
}

public static class ClassConstructorModelExtensions
{
    static bool HasMapConstructorMapping(this ClassConstructorModel type);
    static IElementMapping GetMapConstructorMapping(this ClassConstructorModel type);
}

// "Class" · 04e12b51-ed12-42a3-9667-a6aa81bb6d10
public class ClassModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasFolder<IFolder>, IHasName, IElementWrapper
{
    // + common element members
    string UniqueKey { get; }
    FolderModel Folder { get; }
    bool IsAbstract { get; }
    IEnumerable<string> GenericTypes { get; }
    ClassModel ParentClass { get; }
    ITypeReference ParentClassTypeReference { get; }
    IEnumerable<ClassModel> ChildClasses { get; }
    IElementApplication Application { get; }
    IList<ClassConstructorModel> Constructors { get; }
    IList<AttributeModel> Attributes { get; }
    IList<OperationModel> Operations { get; }
    bool IsSubclassOf(ClassModel @class);
    bool IsSuperclassOf(ClassModel @class);
    IEnumerable<ClassModel> GetTypesInHierarchy();
    IEnumerable<AssociationEndModel> AssociatedClasses { get; set; }
}

// "Comment Association" · 5264c135-e856-468d-8bd7-154b75842256
public class CommentAssociationModel : IMetadataModel
{
    static CommentAssociationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    CommentSourceEndModel SourceEnd { get; }
    CommentTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

public class CommentAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

// "Comment Source End" · 7e98213c-4a9c-4d6f-98fc-f185948cc9e8
public class CommentSourceEndModel : CommentAssociationEndModel { /* common association-end members */ }

// "Comment Target End" · b7edce45-ccf0-47ed-b79f-86d5145c9f62
public class CommentTargetEndModel : CommentAssociationEndModel { /* common association-end members */ }

// "Comment" · c4c0c77f-720b-4e91-9c48-b58d2164d30a
public class CommentModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper
{
    // + common element members
    FolderModel Folder { get; }
}

// "Data Contract Generalization" · 4199ae15-0ecc-4086-82f3-bfa885c9d3e8
public class DataContractGeneralizationModel : IMetadataModel
{
    static DataContractGeneralizationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    DataContractGeneralizationSourceEndModel SourceEnd { get; }
    DataContractGeneralizationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Data Contract Generalization Source End" · 12c2ffdc-9a54-4e99-9e09-a441fa260bef
public class DataContractGeneralizationSourceEndModel : DataContractGeneralizationEndModel { /* common association-end members */ }

// "Data Contract Generalization Target End" · 4ea029c6-e963-46c7-8d2f-e4ea73e05a07
public class DataContractGeneralizationTargetEndModel : DataContractGeneralizationEndModel { /* common association-end members */ }

public class DataContractGeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

// "Data Contract" · 4464fabe-c59e-4d90-81fc-c9245bdd1afd
public class DataContractModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    IEnumerable<string> GenericTypes { get; }
    ITypeReference TypeReference { get; }
    ITypeReference BaseType { get; }
    DataContractModel BaseDataContract { get; }
    IList<AttributeModel> Attributes { get; }
}

// "Diagram" · 4d66fecd-e9b8-436f-aa50-c59040ad0879
public class DiagramModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
}

// "Domain Package" · 1a824508-4623-45d9-accc-f572091ade5a
public class DomainPackageModel : IHasStereotypes, IMetadataModel
{
    // + Id, Name, Stereotypes
    IPackage UnderlyingPackage { get; }
    string FileLocation { get; }
    IList<ClassModel> Classes { get; }
    IList<CommentModel> Comments { get; }
    IList<DiagramModel> Diagrams { get; }
    IList<EnumModel> Enums { get; }
    IList<FolderModel> Folders { get; }
    IList<DataContractModel> DomainContracts { get; }
    IList<DataContractModel> DomainObjects { get; }
    IList<TypeDefinitionModel> Types { get; }
}

public static class AssociationModelAssociationExtensions
{
    static IList<AssociationTargetEndModel> AssociatedToClasses(this ClassModel model);
    static IList<AssociationSourceEndModel> AssociatedFromClasses(this ClassModel model);
    static IList<AssociationEndModel> AssociationEnds(this ClassModel model);
}

public static class ClassModelAssociationExtensions
{
    static bool IsAggregateRoot(this ClassModel classModel);  // Is the ClassModel an Aggregate Root? An Aggregate Root is owned by nothing and can be instantiated.
}

public static class CommentAssociationModelAssociationExtensions
{
    static IList<CommentTargetEndModel> CommentedClasses(this CommentModel model);
    static IList<CommentSourceEndModel> AssociatedComments(this ClassModel model);
    static IList<CommentSourceEndModel> AssociatedComments(this CommentModel model);
    static IList<CommentAssociationEndModel> CommentAssociationEnds(this CommentModel model);
}

public static class DataContractGeneralizationModelAssociationExtensions
{
    static IList<DataContractGeneralizationTargetEndModel> Generalizations(this DataContractModel model);
    static IList<DataContractGeneralizationSourceEndModel> Specializations(this DataContractModel model);
    static IList<DataContractGeneralizationEndModel> DataContractGeneralizationEnds(this DataContractModel model);
}

public static class GeneralizationModelAssociationExtensions
{
    static IList<GeneralizationTargetEndModel> Generalizations(this ClassModel model);
    static IList<GeneralizationSourceEndModel> Specializations(this ClassModel model);
    static IList<GeneralizationEndModel> GeneralizationEnds(this ClassModel model);
}

public class FolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<ClassModel> Classes { get; }
    IList<TypeDefinitionModel> Types { get; }
    IList<EnumModel> Enums { get; }
    IList<CommentModel> Comments { get; }
    IList<DiagramModel> Diagrams { get; }
    IList<DataContractModel> DomainContracts { get; }
}

// "Generalization" · 5de35973-3ac7-4e65-b48c-385605aec561
public class GeneralizationModel : IMetadataModel
{
    static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    GeneralizationSourceEndModel SourceEnd { get; }
    GeneralizationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

public class GeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

// "Generalization Source End" · 8190bf43-222c-4b53-8a44-14626efe3574
public class GeneralizationSourceEndModel : GeneralizationEndModel { /* common association-end members */ }

// "Generalization Target End" · 4686cc1d-b4d8-4b99-b45b-f77bd5496946
public class GeneralizationTargetEndModel : GeneralizationEndModel { /* common association-end members */ }

public interface IStaticConstructorModel : IElementWrapper, IMetadataModel { }

public enum Multiplicity { ZeroToOne, One, Many }

// "Operation" · e042bb67-a1df-480c-9935-b26210f78591
public class OperationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
{
    // + common element members
    ITypeReference TypeReference { get; }
    bool IsMapped { get; }
    IElementMapping Mapping { get; }
    ITypeReference ReturnType { get; }
    bool IsAbstract { get; }
    bool IsStatic { get; }
    ClassModel ParentClass { get; }
    IList<ParameterModel> Parameters { get; }
    IEnumerable<string> GenericTypes { get; }
}

public static class OperationModelExtensions
{
    static bool HasMapOperationMapping(this OperationModel type);
    static IElementMapping GetMapOperationMapping(this OperationModel type);
}

// "Parameter" · c26d8d0a-a26b-4b5f-b449-e9bdb60b3a4b
public class ParameterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
{
    // + common element members
    string Value { get; }
    ITypeReference TypeReference { get; }
    ITypeReference Type { get; }
    IEnumerable<string> GenericTypes { get; }
}
```

### Name collisions to alias

- **`AttributeModel`, `OperationModel` and `ParameterModel` exist in both `Intent.Modelers.Domain.Api` and `Intent.Modules.Common.Types.Api`, with the same `SpecializationTypeId`s.** Importing both namespaces makes the type names *and* their `As…Model()` extensions ambiguous. Import only the one you mean, or alias one of them.
