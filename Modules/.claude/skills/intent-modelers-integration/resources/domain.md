---
contentHash: AB715300C18B3CB643145FA97512EBE6F8EC943626E799104C1AC403888DF28E
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

- *What it is not:** it has no mapping canvas of its own — the `IsMapped`/`Mapping` properties seen

on some of its elements only report that some *other* designer has mapped onto them; Domain is the
mapping target, never the mapping author. It also defines no stereotypes of its own — persistence
concerns (RDBMS columns/keys/indexes, Document DB providers) and validation constraints are added
by separate `Intent.Metadata.*` modules layered on top (§6). Domain Events, Value Objects,
Repositories, Domain Services and Stored Procedures are separate sibling modules that *extend*
Domain's `FolderModel`/`DomainPackageModel` with their own element types (§8) — they are not part
of the base designer.

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

- *computed, not stored**: a non-nullable, non-collection source end is treated as `Composition`,

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

## 8. Extension modules

| Extension | NuGet PackageId | Intent module id | API namespace | Adds |
|---|---|---|---|---|
| Domain Events | `Intent.Modules.Modelers.Domain.Events` | `Intent.Modelers.Domain.Events` | `Intent.Modelers.Domain.Events.Api` | `DomainEventModel` (`0814e459-...`), `DomainEventHandlerModel` (`d80e61c5-...`, implements `IProcessingHandlerModel`), `PropertyModel` (`b4d69073-...`), plus four association families: `DomainEventAssociationModel`, `DomainEventHandlerAssociationModel`, `DomainEventGeneralizationModel`, `DomainEventOriginAssociationModel` |
| Repositories | `Intent.Modules.Modelers.Domain.Repositories` | `Intent.Modelers.Domain.Repositories` | `Intent.Modelers.Domain.Repositories.Api` | `RepositoryModel` (`96ffceb2-...`, reuses Domain's own `OperationModel`), `PackageExtensionsModel`, `FolderExtensionsModel` |
| Value Objects | `Intent.Modules.Modelers.Domain.ValueObjects` | `Intent.Modelers.Domain.ValueObjects` | `Intent.Modelers.Domain.ValueObjects.Api` | `ValueObjectModel` (`5fe6bb0a-...`, reuses Domain's own `AttributeModel`), `DomainPackageExtensionModel`, `FolderExtensionModel`. **Also defines its own stereotype**: `ValueObjectModelStereotypeExtensions.SerializationSettings` (`4ced3df6-...`) |
| Domain Services | `Intent.Modelers.Domain.Services` ⚠️ **no `Modules.` segment** | `Intent.Modelers.Domain.Services` | `Intent.Modelers.Domain.Services.Api` | `DomainServiceModel` (`07f936ea-...`, reuses Domain's own `OperationModel`), `DomainPackageExtensionModel`, `FolderExtensionModel` |
| Stored Procedures | `Intent.Modules.Modelers.Domain.StoredProcedures` | `Intent.Modules.Modelers.Domain.StoredProcedures` ⚠️ **keeps `Modules.` in BOTH module id and namespace, unlike all four siblings above** | `Intent.Modules.Modelers.Domain.StoredProcedures.Api` | `StoredProcedureModel` (`575edd35-...`, implements `IInvokableModel`), `StoredProcedureParameterModel`, `PackageExtensionModel`, `FolderElementExtensionModel`, plus association `StoredProcedureInvocationModel` (navigation directly under `Api/`, not `Api/Extensions/`) |

This table is exactly why `SKILL.md`'s Must #6 / Must Not #5 exist: five sibling extension modules
of the *same* designer, and no two of them are guaranteed to follow the same `Modules.`-segment
convention. Always check the specific module's own `.csproj`/`.imodspec`.

## 9. Worked snippet

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
