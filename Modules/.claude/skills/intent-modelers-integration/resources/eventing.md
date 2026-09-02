---
contentHash: 9172311527E391CEE3A9793D248DEF793B2CDE9946EE7F2D60CAF76C7119A5C0
---
# Eventing Designer (`Intent.Modules.Modelers.Eventing`)

## 1. What this designer is for

The Eventing designer models the integration messages — commands and events — that flow **across
application boundaries**, together with which applications publish and which subscribe to each
message. It is the modelling surface for message-based integration in a multi-application /
microservices landscape.

Consume this designer when you need to turn a `MessageModel`/`IntegrationCommandModel` into
serializable integration event/command classes, message-bus publishers/subscribers, contract
schema files, or client SDKs for other applications.

- *What it is not:** it is not Domain (no aggregates/persistence). It is not Services' in-process

CQRS modelling — the `.Eventing(...)` accessor actually detects a legacy `Application` element
inside the *Services* designer and throws, telling the caller to migrate to this designer's
message-based modelling instead. It does not itself define transport/broker configuration — only
the logical messages and pub/sub relationships that other modules map onto a transport.

## 2. Install identity

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.Eventing` |
| Intent module id | `Intent.Modelers.Eventing` |
| API namespace | `Intent.Modelers.Eventing.Api` |
| Accessor | `.Eventing(app)` |
| Designer GUID | `822e4254-9ced-4dd1-ad56-500b861f7e4d` |

## 3. Entry points

Normal accessor + providers, both in `Intent.Modelers.Eventing.Api`:

```csharp
IDesigner eventing = metadataManager.Eventing(application);

IList<ApplicationModel>      applications = eventing.GetApplicationModels();
IList<EventingDTOModel>      dtos          = eventing.GetEventingDTOModels();
IList<IntegrationCommandModel> commands    = eventing.GetIntegrationCommandModels();
IList<MessageModel>          messages      = eventing.GetMessageModels();
IList<EventingPackageModel>  packages      = eventing.GetEventingPackageModels();
```

⚠️ **The `.Eventing(...)` accessor is not a plain pass-through** — it performs a one-time check
for a legacy `Application` element inside the *Services* designer and throws if found, instructing
migration away from the old Eventing paradigm. Don't be surprised if a first call throws on an
old/migrating codebase — it's a deliberate guard, not a bug.

⚠️ **Publish/subscribe-filtered accessors live in a DIFFERENT file, at the module root, in a
DIFFERENT namespace** — `MetadataManagerExtensions.cs`, namespace `Intent.Modules.Modelers.Eventing`
(note: **not** `.Api`, differing from every other file in the module by only the word "Modules"):

```csharp
using Intent.Modules.Modelers.Eventing; // NOT Intent.Modelers.Eventing.Api — a different namespace

IReadOnlyCollection<MessageModel>      published   = metadataManager.GetPublishedMessageModels(application);
IReadOnlyCollection<MessageModel>      subscribed  = metadataManager.GetSubscribedToMessageModels(application);
IReadOnlyCollection<EventingDTOModel>  pubDtos     = metadataManager.GetPublishedDtoModels(application);
IReadOnlyCollection<EventingDTOModel>  subDtos     = metadataManager.GetSubscribedToDtoModels(application);
IReadOnlyCollection<EnumModel>         pubEnums    = metadataManager.GetPublishedEnumModels(application);
IReadOnlyCollection<EnumModel>         subEnums    = metadataManager.GetSubscribedToEnumModels(application);
```

The DTO/Enum variants recursively walk each published/subscribed message's properties, following
DTO generalization chains and field types, to collect every transitively-referenced DTO/enum.

## 4. Designer elements

| Class | `SpecializationType` | `SpecializationTypeId` |
|---|---|---|
| `MessageModel` | `Message` | `cbe970af-5bad-4d92-a3ed-a24b9fdaa23e` |
| `IntegrationCommandModel` | `Integration Command` | `7f01ca8e-0e3c-4735-ae23-a45169f71625` |
| `EventingDTOModel` | `Eventing DTO` | `544f1d57-27ce-4985-a4ec-cc01568d72b0` |
| `EventingDTOFieldModel` | `Eventing DTO-Field` | `93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7` |
| `PropertyModel` | `Property` | `bde29850-5fb9-4f47-9941-b9e182fd9bdc` |
| `ApplicationModel` | `Application` | `f40a4942-5d2d-4174-8ec1-af66cbd464db` |
| `EventingPackageModel` (package) | `Eventing Package` | `df96d537-7bb5-4c49-811f-973fa6e95beb` |
| `EventingPackageExtensionModel` | *(extends `EventingPackageModel`)* | *(inherits)* |
| `BasicFolderExtensionModel` | *(extends shared `FolderModel`)* | *(inherits)* — adds `Types`/`Enums` |
| `FolderExtensionModel` | *(extends shared `FolderModel`)* | *(inherits)* — adds `IntegrationEvents`/`Messages`/`IntegrationCommands`/`EventingDTOs` |

`PropertyModel` is used on `MessageModel`/`IntegrationCommandModel` (`.Properties`);
`EventingDTOFieldModel` is the analogous field type on `EventingDTOModel` (`.Fields`) — distinct
classes with distinct GUIDs despite the conceptual overlap.

## 5. Associations

⚠️ **The "Assocation" misspelling is real, verbatim, and load-bearing** — not a typo introduced
anywhere along the way. The literal class names are:

| Association | `SpecializationType` | `SpecializationTypeId` |
|---|---|---|
| `MessagePublishAssocationModel` | `Message Publish Assocation` | `022d4c90-e1b6-4747-a15f-640c19503a8f` |
| `MessageSubscribeAssocationModel` | `Message Subscribe Assocation` | `50e0bed1-1387-4d67-8f66-1194763296b1` |
| `GeneralizationModel` | `Generalization` | `ccf59371-009d-44dd-9417-a907b463b223` |

Code referencing these types must use the misspelled names exactly — `MessagePublishAssociationModel`
(correctly spelled) does not exist and will not compile.

⚠️ **Navigation extensions sit directly under `Api/`, NOT `Api/Extensions/`** — there is no
`Api/Extensions/` folder in this module at all (unlike Domain/Services):

- `MessagePublishAssocationModelAssociationExtensions.cs`: `ApplicationModel.PublishedMessages()`,

`MessageModel.PublishingApplications()`.

- `MessageSubscribeAssocationModelAssociationExtensions.cs`: `ApplicationModel.SubscribedMessages()`,

`MessageModel.ConsumingApplications()`.

- `GeneralizationModelAssociationExtensions.cs`: `EventingDTOModel.Generalizations()`,

`.Specializations()`, `.GeneralizationEnds()`.

## 6. Stereotypes

Eventing defines **no stereotypes of its own** — zero `*StereotypeExtensions.cs` anywhere in the
module. Elements implement `IHasStereotypes` generically, but any stereotypes seen on Eventing
elements arrive from other modules, not from Eventing itself.

## 7. Mappings

Eventing has **no mapping or interaction elements of its own** — no `Mapping`/`Interaction` model
anywhere in the module. It is limited to messages/DTOs/fields and publish/subscribe/generalization
associations.

## 8. Extension modules

- *None.** No genuine sibling module extends the Eventing designer's element types. (A

`Intent.Modules.Modelers.Eventing.Metadata` folder exists but is a test/sample application
harness — no `.csproj`/`.imodspec` — not an extension module.)

## 9. Worked snippet

```csharp
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Eventing; // for the published/subscribed accessors
using Intent.RoslynWeaver.Attributes;

namespace MyModule.Templates.MyMessageTemplate
{
  public class MyMessageTemplateRegistration : FilePerModelTemplateRegistration<MessageModel>
  {
      private readonly IMetadataManager _metadataManager;

      public MyMessageTemplateRegistration(IMetadataManager metadataManager)
      {
          _metadataManager = metadataManager;
      }

      public override string TemplateId => MyMessageTemplate.TemplateId;

      public override IEnumerable<MessageModel> GetModels(IApplication application)
      {
          // All messages modelled in this application's Eventing designer:
          return _metadataManager.Eventing(application).GetMessageModels();
      }

      public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, MessageModel model)
      {
          return new MyMessageTemplate(outputTarget, model);
      }
  }
}
```

Factory-extension-style snippet, consuming the publish-filtered accessor so generation only
covers messages this application actually publishes:

```csharp
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Modelers.Eventing; // GetPublishedMessageModels lives here, not Api

public class PublishedMessageFactoryExtension : IApplicationFactoryExtension
{
  private readonly IMetadataManager _metadataManager;

  public PublishedMessageFactoryExtension(IMetadataManager metadataManager)
  {
   _metadataManager = metadataManager;
  }

  public void BeforeTemplateExecution(IApplication application)
  {
      var publishedMessages = _metadataManager.GetPublishedMessageModels(application);
      var publishedDtos = _metadataManager.GetPublishedDtoModels(application);
      var publishedEnums = _metadataManager.GetPublishedEnumModels(application);

      foreach (var message in publishedMessages)
      {
          // e.g. register/emit only the contract types this app actually publishes
      }
  }
}
```
