---
contentHash: 9FF8533D73BC372F091C548C443818B849752296484EE928E20A65AEC09713EF
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

**What it is not:** it is not Domain (no aggregates/persistence). It is not Services' in-process
CQRS modelling — the `.Eventing(...)` accessor actually detects a legacy `Application` element
inside the *Services* designer and throws, telling the caller to migrate to this designer's
message-based modelling instead. It does not itself define transport/broker configuration — only
the logical messages and pub/sub relationships that other modules map onto a transport.

**No extension modules.** Nothing else extends the Eventing designer's element types. (A
`Intent.Modules.Modelers.Eventing.Metadata` folder exists, but it is a test/sample application harness
with no `.csproj`/`.imodspec`, not an extension module.)

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

## 8. Worked snippet

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

### 9.1 Eventing — `Intent.Modules.Modelers.Eventing` (source at `6.0.5`)

```csharp
namespace Intent.Modelers.Eventing.Api;

public static class ApiMetadataDesignerExtensions
{
    const string EventingDesignerId = "822e4254-9ced-4dd1-ad56-500b861f7e4d";
    static IDesigner Eventing(this IMetadataManager metadataManager, IApplication application);
    static IDesigner Eventing(this IMetadataManager metadataManager, string applicationId);
}

public static class ApiMetadataPackageExtensions
{
    static IList<EventingPackageModel> GetEventingPackageModels(this IDesigner designer);
    static bool IsEventingPackageModel(this IPackage package);
}

public static class ApiMetadataProviderExtensions
{
    static IList<ApplicationModel> GetApplicationModels(this IDesigner designer);
    static IList<EventingDTOModel> GetEventingDTOModels(this IDesigner designer);
    static IList<IntegrationCommandModel> GetIntegrationCommandModels(this IDesigner designer);
    static IList<MessageModel> GetMessageModels(this IDesigner designer);
}

namespace Intent.Modules.Modelers.Eventing;

public static class MetadataManagerExtensions
{
    static IList<EventingDTOModel> GetPublishedDtoModels(this IMetadataManager metadataManager, IApplication application);
    static IReadOnlyCollection<EnumModel> GetPublishedEnumModels(this IMetadataManager metadataManager, IApplication application);
    static IReadOnlyCollection<MessageModel> GetPublishedMessageModels(this IMetadataManager metadataManager, IApplication application);
    static IReadOnlyCollection<EventingDTOModel> GetSubscribedToDtoModels(this IMetadataManager metadataManager, IApplication application);
    static IReadOnlyCollection<EnumModel> GetSubscribedToEnumModels(this IMetadataManager metadataManager, IApplication application);
    static IReadOnlyCollection<MessageModel> GetSubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application);
}

namespace Intent.Modelers.Eventing.Api;

// "Application" · f40a4942-5d2d-4174-8ec1-af66cbd464db
public class ApplicationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

public class BasicFolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<TypeDefinitionModel> Types { get; }
    IList<EnumModel> Enums { get; }
}

// "Eventing DTO-Field" · 93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7
public class EventingDTOFieldModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
{
    // + common element members
    string Value { get; }
    ITypeReference TypeReference { get; }
}

// "Eventing DTO" · 544f1d57-27ce-4985-a4ec-cc01568d72b0
public class EventingDTOModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    IEnumerable<string> GenericTypes { get; }
    IList<EventingDTOFieldModel> Fields { get; }
    bool IsAbstract { get; }
    EventingDTOModel ParentDto { get; }
    ITypeReference ParentDtoTypeReference { get; }
}

public class EventingPackageExtensionModel : EventingPackageModel
{
    // + everything inherited from EventingPackageModel
    ApplicationModel Application { get; }
}

// "Eventing Package" · df96d537-7bb5-4c49-811f-973fa6e95beb
public class EventingPackageModel : IHasStereotypes, IMetadataModel
{
    // + Id, Name, Stereotypes
    IPackage UnderlyingPackage { get; }
    string FileLocation { get; }
    IList<MessageModel> IntegrationEvents { get; }
    IList<MessageModel> Messages { get; }
    IList<IntegrationCommandModel> IntegrationCommands { get; }
    IList<EventingDTOModel> EventingDTOs { get; }
    IList<TypeDefinitionModel> Types { get; }
    IList<EnumModel> Enums { get; }
    IList<FolderModel> Folders { get; }
}

public class FolderExtensionModel : FolderModel
{
    // + everything inherited from FolderModel
    IList<MessageModel> IntegrationEvents { get; }
    IList<MessageModel> Messages { get; }
    IList<IntegrationCommandModel> IntegrationCommands { get; }
    IList<EventingDTOModel> EventingDTOs { get; }
}

// "Generalization" · ccf59371-009d-44dd-9417-a907b463b223
public class GeneralizationModel : IMetadataModel
{
    static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    GeneralizationSourceEndModel SourceEnd { get; }
    GeneralizationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Generalization Source End" · 62984ee8-c933-4ad3-861f-51663756b44b
public class GeneralizationSourceEndModel : GeneralizationEndModel { /* common association-end members */ }

// "Generalization Target End" · 71b21217-2424-496d-be62-a147c8c3c1e7
public class GeneralizationTargetEndModel : GeneralizationEndModel { /* common association-end members */ }

public class GeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class GeneralizationModelAssociationExtensions
{
    static IList<GeneralizationTargetEndModel> Generalizations(this EventingDTOModel model);
    static IList<GeneralizationSourceEndModel> Specializations(this EventingDTOModel model);
    static IList<GeneralizationEndModel> GeneralizationEnds(this EventingDTOModel model);
}

// "Integration Command" · 7f01ca8e-0e3c-4735-ae23-a45169f71625
public class IntegrationCommandModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
{
    // + common element members
    FolderModel Folder { get; }
    IList<PropertyModel> Properties { get; }
}

// "Message" · cbe970af-5bad-4d92-a3ed-a24b9fdaa23e
public class MessageModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder, IElementWrapper
{
    // + common element members
    FolderModel Folder { get; }
    IList<PropertyModel> Properties { get; }
}

// "Message Publish Assocation" · 022d4c90-e1b6-4747-a15f-640c19503a8f
public class MessagePublishAssocationModel : IMetadataModel
{
    static MessagePublishAssocationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    MessagePublishAssocationSourceEndModel SourceEnd { get; }
    MessagePublishAssocationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Message Publish Assocation Source End" · 282eea12-2b98-4f0f-a28c-50c05b953c8e
public class MessagePublishAssocationSourceEndModel : MessagePublishAssocationEndModel { /* common association-end members */ }

// "Message Publish Assocation Target End" · 6be8d569-70b9-4451-bd1b-7b654499503e
public class MessagePublishAssocationTargetEndModel : MessagePublishAssocationEndModel { /* common association-end members */ }

public class MessagePublishAssocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class MessagePublishAssocationModelAssociationExtensions
{
    static IList<MessagePublishAssocationTargetEndModel> PublishedMessages(this ApplicationModel model);
    static IList<MessagePublishAssocationSourceEndModel> PublishingApplications(this MessageModel model);
}

// "Message Subscribe Assocation" · 50e0bed1-1387-4d67-8f66-1194763296b1
public class MessageSubscribeAssocationModel : IMetadataModel
{
    static MessageSubscribeAssocationModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    MessageSubscribeAssocationSourceEndModel SourceEnd { get; }
    MessageSubscribeAssocationTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Message Subscribe Assocation Source End" · 18780798-1ea8-462b-b4f9-b8605ad11636
public class MessageSubscribeAssocationSourceEndModel : MessageSubscribeAssocationEndModel { /* common association-end members */ }

// "Message Subscribe Assocation Target End" · 21c3df1c-5140-4f65-9814-a68e1a577768
public class MessageSubscribeAssocationTargetEndModel : MessageSubscribeAssocationEndModel { /* common association-end members */ }

public class MessageSubscribeAssocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class MessageSubscribeAssocationModelAssociationExtensions
{
    static IList<MessageSubscribeAssocationTargetEndModel> SubscribedMessages(this ApplicationModel model);
    static IList<MessageSubscribeAssocationSourceEndModel> ConsumingApplications(this MessageModel model);
}

// "Property" · bde29850-5fb9-4f47-9941-b9e182fd9bdc
public class PropertyModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IElementWrapper
{
    // + common element members
    ITypeReference TypeReference { get; }
}
```

### Notes

- `MetadataManagerExtensions` (the publish/subscribe-filtered accessors) is in namespace `Intent.Modules.Modelers.Eventing`, without the `.Api` suffix — see §3. It needs its own `using`.
- Association type names are spelled `Assocation` (sic) throughout — `MessagePublishAssocationModel`, `MessageSubscribeAssocationModel` and their ends. The misspelling is part of the real type name.
- `EventingPackageExtensionModel` and `BasicFolderExtensionModel` are the package/folder extension types that this module contributes to its own package and folders.
