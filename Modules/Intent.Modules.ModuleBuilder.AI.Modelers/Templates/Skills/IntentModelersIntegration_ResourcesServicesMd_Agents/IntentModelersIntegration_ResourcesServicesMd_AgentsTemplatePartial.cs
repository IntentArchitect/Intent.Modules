using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Modelers.Templates.Skills.IntentModelersIntegration_ResourcesServicesMd_Agents
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class IntentModelersIntegration_ResourcesServicesMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.ModuleBuilder.AI.Modelers.Skills.IntentModelersIntegration_ResourcesServicesMd_Agents";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public IntentModelersIntegration_ResourcesServicesMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile("services", relativeLocation: "intent-modelers-integration/resources")
        .FromMarkdown(""""""
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

          **What it is not:** it does not model the domain/entity model (that's Domain). It does not define
          mapping behaviour itself — mapping between DTOs/Commands/Queries and domain entities is added by
          the `.DomainInteractions` extension module (§7, `services-extensions.md`), not the base designer. It defines no
          stereotypes of its own (§6) — HTTP/versioning/security metadata come from separate
          `Intent.Metadata.*` modules. It does not implement CQRS semantics itself — that's `.CQRS` (`services-extensions.md`).

          **Extension modules — read `services-extensions.md` for these.** Each is installed separately, and its
          install identity and full model API are in that file, not this one: CQRS (`CommandModel`, `QueryModel`),
          Domain Interactions (`CreateEntityActionModel`, `QueryEntityActionModel`, `UpdateEntityActionModel`,
          `DeleteEntityActionModel`, `CallServiceOperationModel`, `ProcessingActionModel`), Event Interactions
          (`IntegrationEventHandlerModel`, `PublishIntegrationEventModel`, `SendCommandModel`,
          `SendIntegrationCommandModel`, `SubscribeIntegrationEventModel`, `SubscribeIntegrationCommandModel`),
          GraphQL (`GraphQLQueryTypeModel`, `GraphQLMutationTypeModel`, `GraphQLSubscriptionTypeModel`,
          `GraphQLSchemaFieldModel`, `GraphQLMutationModel`) and Proxy Interactions (designer metadata only, no C# API).

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
          *pointer* (`IsMapped`, `Mapping`, and the `HasMapFromDomainMapping`/`HasProjectToDomainMapping`/
          `HasMapToDomainOperationMapping` helpers) — the actual mapping construction/traversal API belongs
          to the `.DomainInteractions` extension module — see `services-extensions.md`.

          ## 8. Worked snippet

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

          ### 9.1 Services — `Intent.Modules.Modelers.Services` (source at `4.0.16`)

          ```csharp
          namespace Intent.Modelers.Services.Api;

          public static class ApiMetadataDesignerExtensions
          {
              const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
              static IDesigner Services(this IMetadataManager metadataManager, IApplication application);
              static IDesigner Services(this IMetadataManager metadataManager, string applicationId);
          }

          public static class ApiMetadataPackageExtensions
          {
              static IList<ServicesPackageModel> GetServicesPackageModels(this IDesigner designer);
              static bool IsServicesPackageModel(this IPackage package);
          }

          public static class ApiMetadataProviderExtensions
          {
              static IList<CommentModel> GetCommentModels(this IDesigner designer);
              static IList<DiagramModel> GetDiagramModels(this IDesigner designer);
              static IList<DTOModel> GetDTOModels(this IDesigner designer);
              static IList<ServiceModel> GetServiceModels(this IDesigner designer);
          }

          namespace Intent.Modules.Modelers.Services.Settings;

          public static class ModuleSettingsExtensions
          {
              static ServiceSettings GetServiceSettings(this IApplicationSettingsProvider settings);
          }

          public class ServiceSettings : IGroupSettings
          {
              string Id { get; }
              string Title { get; set; }
              ISetting GetSetting(string settingId);
              PropertyNamingConventionOptions PropertyNamingConvention();
              public class PropertyNamingConventionOptions
              {
                  PropertyNamingConventionOptionsEnum AsEnum();
                  bool IsManual();
                  bool IsPascalCase();
                  bool IsCamelCase();
              }
              public enum PropertyNamingConventionOptionsEnum { Manual, PascalCase, CamelCase }
              EntityNamingConventionOptions EntityNamingConvention();
              public class EntityNamingConventionOptions
              {
                  EntityNamingConventionOptionsEnum AsEnum();
                  bool IsPascalCase();
                  bool IsCamelCase();
                  bool IsManual();
              }
              public enum EntityNamingConventionOptionsEnum { PascalCase, CamelCase, Manual }
              OperationNamingConventionOptions OperationNamingConvention();
              public class OperationNamingConventionOptions
              {
                  OperationNamingConventionOptionsEnum AsEnum();
                  bool IsCamelCase();
                  bool IsManual();
                  bool IsPascalCase();
              }
              public enum OperationNamingConventionOptionsEnum { CamelCase, Manual, PascalCase }
          }

          namespace Intent.Modelers.Services.Api;

          // "Comment Association" · eea9b48a-e5e9-48c2-b3ae-cd51d3f7b7bf
          public class CommentAssociationModel : IMetadataModel
          {
              static CommentAssociationModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              CommentSourceEndModel SourceEnd { get; }
              CommentTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Comment Source End" · 0952566b-9884-4ed0-92f3-10bc137b8f41
          public class CommentSourceEndModel : CommentAssociationEndModel { /* common association-end members */ }

          // "Comment Target End" · 2c33691a-e912-4d4f-99af-c82ff0fd9756
          public class CommentTargetEndModel : CommentAssociationEndModel { /* common association-end members */ }

          public class CommentAssociationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          // "Comment" · 32cb9020-2896-4dc0-9a6d-2aaae3cb431f
          public class CommentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
          }

          // "DTO-Field" · 7baed1fd-469b-4980-8fd9-4cefb8331eb2
          public class DTOFieldModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
          {
              // + common element members
              string Value { get; }
              ITypeReference TypeReference { get; }
              IElementMapping Mapping { get; }
          }

          // "DTO" · fee0edca-4aa0-4f77-a524-6bbd84e78734
          public class DTOModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
              bool IsAbstract { get; }
              IEnumerable<string> GenericTypes { get; }
              DTOModel ParentDto { get; }
              ITypeReference ParentDtoTypeReference { get; }
              bool IsMapped { get; }
              IElementMapping Mapping { get; }
              IList<DTOFieldModel> Fields { get; }
              IElementApplication Application { get; }
          }

          public static class DTOModelExtensions
          {
              static bool HasMapFromDomainMapping(this DTOModel type);
              static IElementMapping GetMapFromDomainMapping(this DTOModel type);
              static bool HasProjectToDomainMapping(this DTOModel type);
              static IElementMapping GetProjectToDomainMapping(this DTOModel type);
              static bool HasMapToDomainOperationMapping(this DTOModel type);
              static IElementMapping GetMapToDomainOperationMapping(this DTOModel type);
          }

          // "Diagram" · 8c90aca5-86f4-47f1-bd58-116fe79f5c55
          public class DiagramModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
          }

          public static class CommentAssociationModelAssociationExtensions
          {
              static IList<CommentTargetEndModel> CommentedClasses(this CommentModel model);
              static IList<CommentSourceEndModel> AssociatedComments(this CommentModel model);
              static IList<CommentSourceEndModel> AssociatedComments(this DTOModel model);
              static IList<CommentSourceEndModel> AssociatedComments(this ServiceModel model);
              static IList<CommentSourceEndModel> AssociatedComments(this DiagramModel model);
          }

          public static class GeneralizationModelAssociationExtensions
          {
              static IList<GeneralizationTargetEndModel> Generalizations(this DTOModel model);
              static IList<GeneralizationSourceEndModel> Specializations(this DTOModel model);
              static IList<GeneralizationEndModel> GeneralizationEnds(this DTOModel model);
          }

          public static class PerformInvocationModelAssociationExtensions
          {
              static IList<PerformInvocationTargetEndModel> PerformInvocationActions(this IProcessingHandlerModel model);
          }

          public class FolderExtensionModel : FolderModel
          {
              // + everything inherited from FolderModel
              IList<ServiceModel> Services { get; }
              IList<DTOModel> DTOs { get; }
              IList<CommentModel> Comments { get; }
              IList<TypeDefinitionModel> Types { get; }
              IList<EnumModel> Enums { get; }
              IList<DiagramModel> Diagrams { get; }
          }

          // "Generalization" · 5ba12bbf-122f-4c3e-af3c-4a88dc554597
          public class GeneralizationModel : IMetadataModel
          {
              static GeneralizationModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              GeneralizationSourceEndModel SourceEnd { get; }
              GeneralizationTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Generalization Source End" · 5ce8666c-89d2-4c9c-b30c-6e5fe42e1766
          public class GeneralizationSourceEndModel : GeneralizationEndModel { /* common association-end members */ }

          // "Generalization Target End" · e1b3d4e4-0ad0-4aec-97fb-780ab9e476db
          public class GeneralizationTargetEndModel : GeneralizationEndModel { /* common association-end members */ }

          public class GeneralizationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          // "Operation" · e030c97a-e066-40a7-8188-808c275df3cb
          public class OperationModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IProcessingHandlerModel, IElementWrapper
          {
              // + common element members
              ServiceModel ParentService { get; }
              IList<ParameterModel> Parameters { get; }
              ITypeReference ReturnType { get; }
              ITypeReference TypeReference { get; }
              IEnumerable<string> GenericTypes { get; }
          }

          // "Parameter" · 00208d20-469d-41cb-8501-768fd5eb796b
          public class ParameterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
          {
              // + common element members
              string Value { get; }
              ITypeReference TypeReference { get; }
              ITypeReference Type { get; }
          }

          // "Perform Invocation" · 3e69085c-fa2f-44bd-93eb-41075fd472f8
          public class PerformInvocationModel : IMetadataModel
          {
              static PerformInvocationModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              PerformInvocationSourceEndModel SourceEnd { get; }
              PerformInvocationTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Perform Invocation Source End" · ee56bd48-8eff-4fff-8d3a-87731d002335
          public class PerformInvocationSourceEndModel : PerformInvocationEndModel { /* common association-end members */ }

          // "Perform Invocation Target End" · 093e5909-ffe4-4510-b3ea-532f30212f3c
          public class PerformInvocationTargetEndModel : PerformInvocationEndModel, IProcessingActionModel
          {
              // + common association-end members
              IEnumerable<IElementToElementMapping> Mappings { get; }
          }

          public class PerformInvocationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          // "Service" · b16578a5-27b1-4047-a8df-f0b783d706bd
          public class ServiceModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IElementWrapper, IAllowCommentModel
          {
              // + common element members
              FolderModel Folder { get; }
              string ApplicationName { get; }
              IElementApplication Application { get; }
              IList<OperationModel> Operations { get; }
          }

          // "Services Package" · df45eaf6-9202-4c25-8dd5-677e9ba1e906
          public class ServicesPackageModel : IHasStereotypes, IMetadataModel
          {
              // + Id, Name, Stereotypes
              IPackage UnderlyingPackage { get; }
              string FileLocation { get; }
              IList<DTOModel> DTOs { get; }
              IList<CommentModel> Comments { get; }
              IList<EnumModel> Enums { get; }
              IList<DiagramModel> Diagrams { get; }
              IList<FolderModel> Folders { get; }
              IList<ServiceModel> Services { get; }
              IList<TypeDefinitionModel> Types { get; }
          }
          ```

          ### Name collisions and gaps

          - **`OperationModel` / `ParameterModel` / `CommentModel` / `DiagramModel` / `GeneralizationModel`** also exist in `Intent.Modelers.Domain.Api`, as different element types with the same simple names. Alias one set when a template reads both designers.

          """""");
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
