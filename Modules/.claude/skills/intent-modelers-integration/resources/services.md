---
contentHash: 074FBCEC8F5C3E1F874695DD7978FE61E228D5D1EE3E5A5CC245F9B11FB1ADCA
---
# Services Designer (`Intent.Modules.Modelers.Services`)

## 1. What this designer is for

The Services designer models an application's web/application service surface: named `Service`
groupings of `Operation`s, the `Parameter`s each operation accepts, its return type, and the `DTO`
contracts — with their `DTOField`s — that flow across that boundary. It is the contract layer of
an application: what operations exist, what they take in, what they hand back, and what shapes
those payloads have.

Consume this designer when you need to generate code from, or attach behaviour/metadata to, that
contract surface — e.g. a controller generator turning `ServiceModel`/`OperationModel` into REST
endpoints, a client generator turning `DTOModel` into request/response classes, or a validation
module inspecting `ParameterModel` types.

- *What it is not:** it does not model the domain/entity model (that's Domain). It does not define

mapping behaviour itself — mapping between DTOs/Commands/Queries and domain entities is added by
the `.DomainInteractions` extension module (§7/§8), not the base designer. It defines no
stereotypes of its own (§6) — HTTP/versioning/security metadata come from separate
`Intent.Metadata.*` modules. It does not implement CQRS semantics itself — that's `.CQRS` (§8).

## 2. Install identity

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.Services` |
| Intent module id | `Intent.Modelers.Services` |
| API namespace | `Intent.Modelers.Services.Api` |
| Accessor | `.Services(app)` |
| Designer GUID | `81104ae6-2bc5-4bae-b05a-f987b0372d81` |

Verified against `ApiMetadataDesignerExtensions.ServicesDesignerId` and the matching
`.imodspec` `<install src="modelers/Services.designer.config" externalReference="81104ae6-..." />`.

## 3. Entry points

```csharp
using Intent.Modelers.Services.Api;

IDesigner services = metadataManager.Services(application);

IList<ServiceModel>   serviceModels = services.GetServiceModels();
IList<DTOModel>       dtoModels      = services.GetDTOModels();
IList<CommentModel>   comments       = services.GetCommentModels();
IList<DiagramModel>   diagrams       = services.GetDiagramModels();

// Package-model accessor
IList<ServicesPackageModel> packages = services.GetServicesPackageModels();
```

There is **no** top-level provider for `OperationModel`, `ParameterModel`, or `DTOFieldModel` —
reach them only via their parent's child collection (`ServiceModel.Operations`,
`OperationModel.Parameters`, `DTOModel.Fields`).

## 4. Designer elements

| Element | `SpecializationType` | `SpecializationTypeId` | Notes |
|---|---|---|---|
| `ServiceModel` | `Service` | `b16578a5-27b1-4047-a8df-f0b783d706bd` | Exposes `Operations` |
| `OperationModel` | `Operation` | `e030c97a-e066-40a7-8188-808c275df3cb` | Implements `IProcessingHandlerModel` — the hook point `PerformInvocationModel` and `.DomainInteractions`/`.EventInteractions` action associations attach to. Exposes `Parameters`, `ReturnType`, `ParentService` |
| `ParameterModel` | `Parameter` | `00208d20-469d-41cb-8501-768fd5eb796b` | `IHasTypeReference` |
| `DTOModel` | `DTO` | `fee0edca-4aa0-4f77-a524-6bbd84e78734` | Has a genuine hand-written `.partial.cs` addition: `Application` accessor. Also carries `IsMapped`/`Mapping`, `ParentDto` (via `Generalizations()`), and `HasMapFromDomainMapping()`/`HasProjectToDomainMapping()`/`HasMapToDomainOperationMapping()` helpers keyed on fixed `MappingSettingsId`s |
| `DTOFieldModel` | `DTO-Field` | `7baed1fd-469b-4980-8fd9-4cefb8331eb2` | `IHasTypeReference`; carries `Value`, `Mapping` |
| `ServicesPackageModel` | `Services Package` | `df45eaf6-9202-4c25-8dd5-677e9ba1e906` | Package wrapper; exposes `DTOs`, `Comments`, `Enums`, `Diagrams`, `Folders`, `Services`, `Types` |
| `CommentModel` | `Comment` | `32cb9020-2896-4dc0-9a6d-2aaae3cb431f` | Fully generated |
| `DiagramModel` | `Diagram` | `8c90aca5-86f4-47f1-bd58-116fe79f5c55` | Fully generated |
| `FolderExtensionModel` | *(extends shared `FolderModel`)* | *(inherits)* | Adds `Services`, `DTOs`, `Comments`, `Types`, `Enums`, `Diagrams` to a folder |

Every element other than `ServicesPackageModel`/`FolderExtensionModel` ships an empty `.partial.cs`
stub reserved for hand overrides — only `DTOModel`'s is actually used.

## 5. Associations

| Association | `SpecializationType` | `SpecializationTypeId` |
|---|---|---|
| `GeneralizationModel` | `Generalization` | `5ba12bbf-122f-4c3e-af3c-4a88dc554597` |
| `PerformInvocationModel` | `Perform Invocation` | `3e69085c-fa2f-44bd-93eb-41075fd472f8` |
| `CommentAssociationModel` | `Comment Association` | `eea9b48a-e5e9-48c2-b3ae-cd51d3f7b7bf` |

Core association classes live directly in `Api/`; navigation extensions live in **`Api/Extensions/`**:

- `GeneralizationModelAssociationExtensions.cs`: `DTOModel.Generalizations()` (parent, ≤1 —

`ParentDto` throws if more), `.Specializations()`, `.GeneralizationEnds()`.

- `PerformInvocationModelAssociationExtensions.cs`: `IProcessingHandlerModel.PerformInvocationActions()` — how `OperationModel` (and later `CommandModel`/`QueryModel`) exposes "perform this action" links.
- `CommentAssociationModelAssociationExtensions.cs`: `CommentModel.CommentedClasses()`/ `AssociatedComments()`, plus `AssociatedComments()` overloads on `DTOModel`, `ServiceModel`, `DiagramModel`.

## 6. Stereotypes

Services defines **zero stereotypes of its own** — confirmed, no `*StereotypeExtensions.cs` under
its `Api/`. All stereotypes on `ServiceModel`/`OperationModel`/`DTOModel`/etc. come from elsewhere:

| Stereotype module | NuGet PackageId | Intent module id | API namespace | Example stereotypes |
|---|---|---|---|---|
| WebApi metadata | `Intent.Modules.Metadata.WebApi` | `Intent.Metadata.WebApi` | `Intent.Metadata.WebApi.Api` | `ApiVersionSettings`, `HttpServiceSettings`, `HttpSettings`, `ParameterSettings`, `FileTransfer` — attach to `OperationModel`, `ServiceModel`, `CommandModel`, `QueryModel`, `DTOModel`/`DTOFieldModel`, `ParameterModel`, `ServicesPackageModel` |
| Security metadata | `Intent.Modules.Metadata.Security` | `Intent.Metadata.Security` | `Intent.Metadata.Security.Api` | `Secured`/`Unsecured`, `PolicyModel`, `RoleModel`, `SecurityConfigurationModel` — attach to `ServiceModel`, `OperationModel`, `CommandModel`, `QueryModel`, `ServicesPackageModel` |

## 7. Mappings

Mapping is **not** part of the base Services designer. `DTOModel` only carries the mapping

- pointer* (`IsMapped`, `Mapping`, and the `HasMapFromDomainMapping`/`HasProjectToDomainMapping`/

`HasMapToDomainOperationMapping` helpers) — the actual mapping construction/traversal API belongs
to the `.DomainInteractions` extension module below.

## 8. Extension modules

| Module | NuGet PackageId | Intent module id | API namespace | Adds |
|---|---|---|---|---|
| CQRS | `Intent.Modules.Modelers.Services.CQRS` | `Intent.Modelers.Services.CQRS` | `Intent.Modelers.Services.CQRS.Api` — **its own namespace, isolated from the base `Services.Api`** | `CommandModel` (`ccf14eb6-...`), `QueryModel` (`e71b0662-...`), `FolderExtensionModel` |
| DomainInteractions | `Intent.Modules.Modelers.Services.DomainInteractions` | `Intent.Modelers.Services.DomainInteractions` | `Intent.Modelers.Services.DomainInteractions.Api` | `CreateEntityActionModel`, `QueryEntityActionModel`, `UpdateEntityActionModel`, `DeleteEntityActionModel`, `CallServiceOperationModel`, `ProcessingActionModel`, plus `ElementToElementMappingExtensions` — **this is where mapping actually lives** for this designer |
| EventInteractions | `Intent.Modules.Modelers.Services.EventInteractions` | `Intent.Modelers.Services.EventInteractions` | `Intent.Modelers.Services.EventInteractions.Api` | `IntegrationEventHandlerModel`, `PublishIntegrationEventModel`, `SendCommandModel`, `SendIntegrationCommandModel`, `SubscribeIntegrationCommandModel`, `SubscribeIntegrationEventModel`, `CallServiceOperationModel` |
| ProxyInteractions | `Intent.Modules.Modelers.Services.ProxyInteractions` | `Intent.Modelers.Services.ProxyInteractions` | — **ships no `Api/` folder at all** | Nothing — validation-only factory extension (validates Service Proxy references), adds no element types |
| GraphQL | `Intent.Modules.Modelers.Services.GraphQL` | `Intent.Modelers.Services.GraphQL` | `Intent.Modelers.Services.GraphQL.Api` | `DTOExtensionModel`, `GraphQLEventMessageModel`, `GraphQLMutationModel`, `GraphQLMutationTypeModel`, `GraphQLParameterModel`, `GraphQLQueryTypeModel`, `GraphQLSchemaFieldModel`, `GraphQLServicesPackageModel`, `GraphQLSubscriptionModel`, `GraphQLSubscriptionTypeModel` |

- *CQRS namespace — confirmed, not a trap today.** `CommandModel`/`QueryModel` are declared in their

own `Intent.Modelers.Services.CQRS.Api` namespace, matching their own module id — they do **not**
leak into the parent `Services.Api` namespace. Do not assume this holds for every extension module
of every designer, though — check each one (Must #6 in `SKILL.md`).

- *Version drift — real, and worth checking before copying a number from here.** At the time this

was written, `.DomainInteractions`' `.csproj` `PackageReference` for the base `Services` module
trailed its own `.imodspec` dependency floor (`4.0.5` compiled against vs. `4.0.14` declared as the
install-time minimum, while the base module itself had moved on to `4.0.16`). The same shape
repeats in `.EventInteractions`. Treat any specific version number in this file as illustrative,
not authoritative — always read the current numbers directly.

## 9. Worked snippet

```csharp
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace MyModule.Templates.MyServiceTemplate
{
  public class MyServiceTemplateRegistration : FilePerModelTemplateRegistration<ServiceModel>
  {
      public const string TemplateId = "MyModule.MyServiceTemplate";

      public override IEnumerable<ServiceModel> GetModels(IApplication application)
      {
          return application.MetadataManager.Services(application).GetServiceModels();
      }

      public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ServiceModel model)
      {
          return new MyServiceTemplate(TemplateId, outputTarget, model);
      }
  }
}
```

Factory-extension-style consumer, flagging no-op operations:

```csharp
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace MyModule.FactoryExtensions
{
  public class ValidateServiceOperationsFactoryExtension : IFactoryExtension
  {
      public void Execute(IApplication application, IExecutionContext executionContext)
      {
          var serviceModels = application.MetadataManager.Services(application).GetServiceModels();

          foreach (var service in serviceModels)
          {
              foreach (var operation in service.Operations)
              {
                  if (operation.Parameters.Count == 0 && operation.ReturnType == null)
                  {
                      // e.g. flag a no-op operation
                  }
              }
          }
      }
  }
}
```
