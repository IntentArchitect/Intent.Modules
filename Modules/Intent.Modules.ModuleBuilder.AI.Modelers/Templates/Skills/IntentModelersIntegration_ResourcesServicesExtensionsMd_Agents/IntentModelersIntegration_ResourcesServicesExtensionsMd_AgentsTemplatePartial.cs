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

namespace Intent.Modules.ModuleBuilder.AI.Modelers.Templates.Skills.IntentModelersIntegration_ResourcesServicesExtensionsMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentModelersIntegration_ResourcesServicesExtensionsMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Modelers.Skills.IntentModelersIntegration_ResourcesServicesExtensionsMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentModelersIntegration_ResourcesServicesExtensionsMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("services-extensions", relativeLocation: "intent-modelers-integration/resources")
                .FromMarkdown(""""""
                    # Services Designer — Extension Modules

                    The modules below are installed separately from the base Services designer (`Intent.Modules.Modelers.Services`). Each one adds
                    its own element types, package/folder extensions or navigation on top of it. Read `services.md`
                    first for the base designer; this file covers only what the extension modules add. Load it only when
                    your module actually installs one of them.

                    Every extension module has its **own** install identity. Never infer one from its parent designer or
                    from a sibling (`SKILL.md` Must #6).

                    ## 1. Install identities

                    | Module | NuGet PackageId | Intent module id | API namespace | Adds |
                    |---|---|---|---|---|
                    | CQRS | `Intent.Modules.Modelers.Services.CQRS` | `Intent.Modelers.Services.CQRS` | `Intent.Modelers.Services.CQRS.Api` — **its own namespace, isolated from the base `Services.Api`** | `CommandModel` (`ccf14eb6-...`), `QueryModel` (`e71b0662-...`), `FolderExtensionModel` |
                    | DomainInteractions | `Intent.Modules.Modelers.Services.DomainInteractions` | `Intent.Modelers.Services.DomainInteractions` | `Intent.Modelers.Services.DomainInteractions.Api` | `CreateEntityActionModel`, `QueryEntityActionModel`, `UpdateEntityActionModel`, `DeleteEntityActionModel`, `CallServiceOperationModel`, `ProcessingActionModel`, plus `ElementToElementMappingExtensions` — **this is where mapping actually lives** for this designer |
                    | EventInteractions | `Intent.Modules.Modelers.Services.EventInteractions` | `Intent.Modelers.Services.EventInteractions` | `Intent.Modelers.Services.EventInteractions.Api` | `IntegrationEventHandlerModel`, `PublishIntegrationEventModel`, `SendCommandModel`, `SendIntegrationCommandModel`, `SubscribeIntegrationCommandModel`, `SubscribeIntegrationEventModel`, `CallServiceOperationModel` |
                    | ProxyInteractions | `Intent.Modules.Modelers.Services.ProxyInteractions` | `Intent.Modelers.Services.ProxyInteractions` | — **ships no `Api/` folder at all** | Nothing — validation-only factory extension (validates Service Proxy references), adds no element types |
                    | GraphQL | `Intent.Modules.Modelers.Services.GraphQL` | `Intent.Modelers.Services.GraphQL` | `Intent.Modelers.Services.GraphQL.Api` | `DTOExtensionModel`, `GraphQLEventMessageModel`, `GraphQLMutationModel`, `GraphQLMutationTypeModel`, `GraphQLParameterModel`, `GraphQLQueryTypeModel`, `GraphQLSchemaFieldModel`, `GraphQLServicesPackageModel`, `GraphQLSubscriptionModel`, `GraphQLSubscriptionTypeModel` |

                    **CQRS namespace — confirmed, not a trap today.** `CommandModel`/`QueryModel` are declared in their
                    own `Intent.Modelers.Services.CQRS.Api` namespace, matching their own module id — they do **not**
                    leak into the parent `Services.Api` namespace. Do not assume this holds for every extension module
                    of every designer, though — check each one (Must #6 in `SKILL.md`).

                    **Version drift — real, and worth checking before copying a number from here.** At the time this
                    was written, `.DomainInteractions`' `.csproj` `PackageReference` for the base `Services` module
                    trailed its own `.imodspec` dependency floor (`4.0.5` compiled against vs. `4.0.14` declared as the
                    install-time minimum, while the base module itself had moved on to `4.0.16`). The same shape
                    repeats in `.EventInteractions`. Treat any specific version number in this file as illustrative,
                    not authoritative — always read the current numbers directly.

                    ## 2. Model API reference

                    Every public type and member each extension module ships — extracted mechanically from source, not
                    sampled. **Prefer this to reflecting over the assembly:** if a member is not listed here, it is
                    not part of the version shown. The stubs follow the conventions defined in
                    `services.md` §9 — `// + common element members`, the omitted `Is<X>Model()` / `As<X>Model()`
                    casts, and `// + common association-end members` mean exactly what they mean there.

                    ### 2.1 Services.CQRS — `Intent.Modules.Modelers.Services.CQRS` (source at `6.0.8`)

                    ```csharp
                    namespace Intent.Modelers.Services.CQRS.Api;

                    public static class ApiMetadataProviderExtensions
                    {
                        static IList<CommandModel> GetCommandModels(this IDesigner designer);
                        static IList<QueryModel> GetQueryModels(this IDesigner designer);
                    }

                    public class ServicesPackageExtensionModel : ServicesPackageModel
                    {
                        // + everything inherited from ServicesPackageModel
                        IList<CommandModel> Commands { get; }
                        IList<QueryModel> Queries { get; }
                    }

                    // "Command" · ccf14eb6-3a55-4d81-b5b9-d27311c70cb9
                    public class CommandModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder, IElementWrapper, IProcessingHandlerModel, IAllowCommentModel, IInvokableModel
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                        string GetConceptName();
                        IList<DTOFieldModel> Properties { get; }
                        FolderModel Folder { get; }
                        bool IsMapped { get; }
                        IElementMapping Mapping { get; }
                    }

                    public static class CommandModelExtensions
                    {
                        static bool HasMapToDomainDataMapping(this CommandModel type);
                        static IElementMapping GetMapToDomainDataMapping(this CommandModel type);
                        static bool HasMapToDomainOperationMapping(this CommandModel type);
                        static IElementMapping GetMapToDomainOperationMapping(this CommandModel type);
                        static bool HasMapToDomainMapping(this CommandModel type);
                        static IElementMapping GetMapToDomainMapping(this CommandModel type);
                    }

                    public class FolderExtensionModel : FolderModel
                    {
                        // + everything inherited from FolderModel
                        IList<CommandModel> Commands { get; }
                        IList<QueryModel> Queries { get; }
                    }

                    // "Query" · e71b0662-e29d-4db2-868b-8a12464b25d0
                    public class QueryModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder, IElementWrapper, IProcessingHandlerModel, IAllowCommentModel, IInvokableModel
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                        bool IsMapped { get; }
                        IElementMapping Mapping { get; }
                        string GetConceptName();
                        IList<DTOFieldModel> Properties { get; }
                        FolderModel Folder { get; }
                    }

                    public static class QueryModelExtensions
                    {
                        static bool HasProjectFromDomainMapping(this QueryModel type);
                        static IElementMapping GetProjectFromDomainMapping(this QueryModel type);
                    }
                    ```

                    ### 2.2 Services.DomainInteractions — `Intent.Modules.Modelers.Services.DomainInteractions` (source at `2.4.8`)

                    ```csharp
                    namespace Intent.Modules.Modelers.Services.DomainInteractions.Settings;

                    public static class ModuleSettingsExtensions
                    {
                        static DomainInteractionSettings GetDomainInteractionSettings(this IApplicationSettingsProvider settings);
                    }

                    public class DomainInteractionSettings : IGroupSettings
                    {
                        string Id { get; }
                        string Title { get; set; }
                        ISetting GetSetting(string settingId);
                        DefaultMappingModeOptions DefaultMappingMode();
                        public class DefaultMappingModeOptions
                        {
                            DefaultMappingModeOptionsEnum AsEnum();
                            bool IsBasic();
                            bool IsAdvanced();
                        }
                        public enum DefaultMappingModeOptionsEnum { Basic, Advanced }
                        DefaultQueryImplementationOptions DefaultQueryImplementation();
                        public class DefaultQueryImplementationOptions
                        {
                            DefaultQueryImplementationOptionsEnum AsEnum();
                            bool IsDefault();
                            bool IsProjectTo();
                        }
                        public enum DefaultQueryImplementationOptionsEnum { Default, ProjectTo }
                        NullChildUpdateImplementationOptions NullChildUpdateImplementation();
                        public class NullChildUpdateImplementationOptions
                        {
                            NullChildUpdateImplementationOptionsEnum AsEnum();
                            bool IsIgnore();
                            bool IsSetToNull();
                        }
                        public enum NullChildUpdateImplementationOptionsEnum { Ignore, SetToNull }
                    }

                    namespace Intent.Modelers.Services.DomainInteractions.Api;

                    // "Call Service Operation" · 3e69085c-fa2f-44bd-93eb-41075fd472f8
                    public class CallServiceOperationModel : IMetadataModel
                    {
                        static CallServiceOperationModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        CallServiceOperationSourceEndModel SourceEnd { get; }
                        CallServiceOperationTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Call Service Operation Source End" · ee56bd48-8eff-4fff-8d3a-87731d002335
                    public class CallServiceOperationSourceEndModel : CallServiceOperationEndModel { /* common association-end members */ }

                    // "Call Service Operation Target End" · 093e5909-ffe4-4510-b3ea-532f30212f3c
                    public class CallServiceOperationTargetEndModel : CallServiceOperationEndModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class CallServiceOperationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class CallServiceOperationModelAssociationExtensions
                    {
                        static IList<CallServiceOperationTargetEndModel> CallServiceOperationActions(this IProcessingHandlerModel model);
                    }

                    // "Create Entity Action" · 7a3f0474-3cf8-4249-baac-8c07c49465e0
                    public class CreateEntityActionModel : IMetadataModel
                    {
                        static CreateEntityActionModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        CreateEntityActionSourceEndModel SourceEnd { get; }
                        CreateEntityActionTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Create Entity Action Source End" · a3e7c59e-b0a1-47e1-ba29-66f2c7047b0a
                    public class CreateEntityActionSourceEndModel : CreateEntityActionEndModel { /* common association-end members */ }

                    // "Create Entity Action Target End" · 328f54e5-7bad-4b5f-90ca-03ce3105d016
                    public class CreateEntityActionTargetEndModel : CreateEntityActionEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class CreateEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class CreateEntityActionModelAssociationExtensions
                    {
                        static IList<CreateEntityActionTargetEndModel> CreateEntityActions(this IProcessingHandlerModel model);
                        static IList<CreateEntityActionSourceEndModel> CreateEntityCommands(this ClassModel model);
                        static IList<CreateEntityActionSourceEndModel> CreateEntityCommands(this ClassConstructorModel model);
                    }

                    // "Delete Entity Action" · bfc823fb-60ab-451d-ba62-12671fe7e28e
                    public class DeleteEntityActionModel : IMetadataModel
                    {
                        static DeleteEntityActionModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        DeleteEntityActionSourceEndModel SourceEnd { get; }
                        DeleteEntityActionTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Delete Entity Action Source End" · 8c2d9fed-bd14-44b2-9f98-8a801aaf157e
                    public class DeleteEntityActionSourceEndModel : DeleteEntityActionEndModel { /* common association-end members */ }

                    // "Delete Entity Action Target End" · 4a04cfc2-5841-438c-9c16-fb58b784b365
                    public class DeleteEntityActionTargetEndModel : DeleteEntityActionEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class DeleteEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class DeleteEntityActionModelAssociationExtensions
                    {
                        static IList<DeleteEntityActionTargetEndModel> DeleteEntityActions(this IProcessingHandlerModel model);
                        static IList<DeleteEntityActionSourceEndModel> DeleteEntityCommands(this ClassModel model);
                    }

                    public static class ElementToElementMappingExtensions
                    {
                        static bool IsCommandToClassCreationMapping(this IElementToElementMapping mapping);
                        static bool IsCommandToClassUpdateMapping(this IElementToElementMapping mapping);
                    }

                    // "Processing Action" · 405a2857-b911-431f-8142-719a0e9f15f3
                    public class ProcessingActionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IProcessingActionModel
                    {
                        // + common element members
                    }

                    public static class ProcessingHandlerModelExtensions
                    {
                        static IList<ProcessingActionModel> ProcessingActions(this IProcessingHandlerModel model);
                    }

                    // "Query Entity Action" · 47ab5888-a258-4bec-a9fc-a83de69eb79d
                    public class QueryEntityActionModel : IMetadataModel
                    {
                        static QueryEntityActionModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        QueryEntityActionSourceEndModel SourceEnd { get; }
                        QueryEntityActionTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Query Entity Action Source End" · 32a65f26-2555-4616-8a2c-6a90805600bb
                    public class QueryEntityActionSourceEndModel : QueryEntityActionEndModel { /* common association-end members */ }

                    // "Query Entity Action Target End" · 93ef6675-cba4-4998-adff-cb22d5343ed4
                    public class QueryEntityActionTargetEndModel : QueryEntityActionEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class QueryEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class QueryEntityActionModelAssociationExtensions
                    {
                        static IList<QueryEntityActionTargetEndModel> QueryEntityActions(this IProcessingHandlerModel model);
                        static IList<QueryEntityActionSourceEndModel> QueryEntitySources(this ClassModel model);
                    }

                    // "Update Entity Action" · 9ea0382a-4617-412a-a8c8-af987bbce226
                    public class UpdateEntityActionModel : IMetadataModel
                    {
                        static UpdateEntityActionModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        UpdateEntityActionSourceEndModel SourceEnd { get; }
                        UpdateEntityActionTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Update Entity Action Source End" · 6bc95978-6def-4d0c-a4f5-25bdeda8a9f6
                    public class UpdateEntityActionSourceEndModel : UpdateEntityActionEndModel { /* common association-end members */ }

                    // "Update Entity Action Target End" · 516069f6-09cc-4de8-8e31-3c71ca823452
                    public class UpdateEntityActionTargetEndModel : UpdateEntityActionEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class UpdateEntityActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class UpdateEntityActionModelAssociationExtensions
                    {
                        static IList<UpdateEntityActionTargetEndModel> UpdateEntityActions(this IProcessingHandlerModel model);
                        static IList<UpdateEntityActionSourceEndModel> UpdateEntityCommands(this ClassModel model);
                        static IList<UpdateEntityActionSourceEndModel> UpdateEntityCommands(this OperationModel model);
                    }
                    ```

                    ### 2.3 Services.EventInteractions — `Intent.Modules.Modelers.Services.EventInteractions` (source at `2.0.8`)

                    ```csharp
                    namespace Intent.Modelers.Services.EventInteractions;

                    // Extension methods of Event Interactions in the Services designer.
                    public static class ApiMetadataManagerCustomExtensions
                    {
                        static IList<EventingDTOModel> GetExplicitlyPublishedDtoModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all DTOs that are explicitly published through messages or integration commands in the specified application.
                        static IList<EventingDTOModel> GetExplicitlyPublishedDtoModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all DTOs that are explicitly published through messages or integration commands in the specified application.
                        static IReadOnlyCollection<EnumModel> GetExplicitlyPublishedEnumModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Enum models that are explicitly published through messages or integration commands in the specified application.
                        static IReadOnlyCollection<EnumModel> GetExplicitlyPublishedEnumModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Enum models that are explicitly published through messages or integration commands in the specified application.
                        static IReadOnlyCollection<MessageModel> GetExplicitlyPublishedMessageModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Message models that are explicitly published through integration events in the specified application.
                        static IReadOnlyCollection<MessageModel> GetExplicitlyPublishedMessageModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Message models that are explicitly published through integration events in the specified application.
                        static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySentIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Integration Command models that are explicitly sent in the specified application.
                        static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySentIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Integration Command models that are explicitly sent in the specified application.
                        static IReadOnlyCollection<SendIntegrationCommandTargetEndModel> GetExplicitlySentIntegrationCommandDispatches(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Integration Command dispatch models that are explicitly sent in the specified application.
                        static IReadOnlyCollection<EventingDTOModel> GetExplicitlySubscribedToDtoModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all DTO models that are explicitly subscribed to through messages or integration commands in the specified application.
                        static IReadOnlyCollection<EventingDTOModel> GetExplicitlySubscribedToDtoModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all DTO models that are explicitly subscribed to through messages or integration commands in the specified application.
                        static IReadOnlyCollection<EnumModel> GetExplicitlySubscribedToEnumModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Enum models that are explicitly subscribed to through messages or integration commands in the specified application.
                        static IReadOnlyCollection<EnumModel> GetExplicitlySubscribedToEnumModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Enum models that are explicitly subscribed to through messages or integration commands in the specified application.
                        static IReadOnlyCollection<MessageModel> GetExplicitlySubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Message models that are explicitly subscribed to through integration events in the specified application.
                        static IReadOnlyCollection<MessageModel> GetExplicitlySubscribedToMessageModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Message models that are explicitly subscribed to through integration events in the specified application.
                        static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySubscribedToIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application);  // Retrieves all Integration Command models that are explicitly subscribed to in the specified application.
                        static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySubscribedToIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId);  // Retrieves all Integration Command models that are explicitly subscribed to in the specified application.
                        static IReadOnlyCollection<MessageModel> GetAssociatedMessageModels(this IMetadataManager metadataManager, IApplication application);  // Returns all Messages referenced by an association in the Services designer.
                        static IReadOnlyCollection<MessageModel> GetAssociatedMessageModels(this IMetadataManager metadataManager, string applicationId);  // Returns all Messages referenced by an association in the Services designer.
                        static IReadOnlyCollection<IntegrationCommandModel> GetAssociatedIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application);  // Returns all integration command models referenced by an association in the Services designer.
                        static IReadOnlyCollection<IntegrationCommandModel> GetAssociatedIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId);  // Returns all integration command models referenced by an association in the Services designer.
                        static IReadOnlyCollection<EnumModel> GetAssociatedMessageEnumModels(this IMetadataManager metadataManager, IApplication application);  // Returns all enums used by messages referenced by an association in the services designer.
                        static IReadOnlyCollection<EnumModel> GetAssociatedMessageEnumModels(this IMetadataManager metadataManager, string applicationId);  // Returns all enums used by messages referenced by an association in the services designer.
                        static IReadOnlyCollection<EventingDTOModel> GetAssociatedMessageDtoModels(this IMetadataManager metadataManager, IApplication application);  // Returns all DTOs used by messages referenced by an association in the Services designer.
                        static IReadOnlyCollection<EventingDTOModel> GetAssociatedMessageDtoModels(this IMetadataManager metadataManager, string applicationId);  // Returns all DTOs used by messages referenced by an association in the Services designer.
                    }

                    public static class ApiMetadataProviderExtensions
                    {
                        static IList<IntegrationEventHandlerModel> GetIntegrationEventHandlerModels(this IDesigner designer);
                    }

                    // "Call Service Operation" · 9510ff76-fba3-4eca-a5dd-0cfefc8f5bb6
                    public class CallServiceOperationModel : IMetadataModel
                    {
                        static CallServiceOperationModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        CallServiceOperationSourceEndModel SourceEnd { get; }
                        CallServiceOperationTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Call Service Operation Source End" · 01693f59-b89f-4e03-934a-dfcb4cbd90df
                    public class CallServiceOperationSourceEndModel : CallServiceOperationEndModel { /* common association-end members */ }

                    // "Call Service Operation Target End" · 752c289a-5961-4172-9017-0eca6fa09fd9
                    public class CallServiceOperationTargetEndModel : CallServiceOperationEndModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class CallServiceOperationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class CallServiceOperationModelAssociationExtensions
                    {
                        static IList<CallServiceOperationTargetEndModel> CalledServiceOperations(this IProcessingHandlerModel model);
                    }

                    public class FolderExtensionModel : FolderModel
                    {
                        // + everything inherited from FolderModel
                        IList<IntegrationEventHandlerModel> IntegrationEventHandlers { get; }
                    }

                    // "Integration Event Handler" · c0582230-22f5-4f74-8eb0-ec6fc9364900
                    public class IntegrationEventHandlerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder, IProcessingHandlerModel
                    {
                        // + common element members
                        FolderModel Folder { get; }
                    }

                    // "Publish Integration Event" · 580b6b26-eab5-4602-a408-e76e2d292d2c
                    public class PublishIntegrationEventModel : IMetadataModel
                    {
                        static PublishIntegrationEventModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        PublishIntegrationEventSourceEndModel SourceEnd { get; }
                        PublishIntegrationEventTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Publish Integration Event Source End" · eab91b3a-9903-40a2-90e8-ddb714883eab
                    public class PublishIntegrationEventSourceEndModel : PublishIntegrationEventEndModel { /* common association-end members */ }

                    // "Publish Integration Event Target End" · 6feb1511-849a-4aa3-85eb-d0c736ac1fec
                    public class PublishIntegrationEventTargetEndModel : PublishIntegrationEventEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class PublishIntegrationEventEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class PublishIntegrationEventModelAssociationExtensions
                    {
                        static IList<PublishIntegrationEventTargetEndModel> PublishedIntegrationEvents(this IProcessingHandlerModel model);
                        static IList<PublishIntegrationEventSourceEndModel> IntegrationEventsSources(this MessageModel model);
                    }

                    // "Send Command" · 38a3de5a-ca88-4f6e-88b9-88e5953936b2
                    public class SendCommandModel : IMetadataModel
                    {
                        static SendCommandModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        SendCommandSourceEndModel SourceEnd { get; }
                        SendCommandTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Send Command Source End" · 6b9e34dd-b50f-4998-a3cf-93dde7b2d51e
                    public class SendCommandSourceEndModel : SendCommandEndModel { /* common association-end members */ }

                    // "Send Command Target End" · d3096261-1268-440f-8db3-0a6b8b4786cc
                    public class SendCommandTargetEndModel : SendCommandEndModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class SendCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class SendCommandModelAssociationExtensions
                    {
                        static IList<SendCommandTargetEndModel> SentCommandDestinations(this IProcessingHandlerModel model);
                    }

                    // "Send Integration Command" · 389a7478-a8f1-4acc-adff-a73ce4aa7e6d
                    public class SendIntegrationCommandModel : IMetadataModel
                    {
                        static SendIntegrationCommandModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        SendIntegrationCommandSourceEndModel SourceEnd { get; }
                        SendIntegrationCommandTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Send Integration Command Source End" · c5f4f98f-e464-48de-b202-c0724bacebb7
                    public class SendIntegrationCommandSourceEndModel : SendIntegrationCommandEndModel { /* common association-end members */ }

                    // "Send Integration Command Target End" · 35a14f76-71e0-45f2-a17f-f8d1483510f7
                    public class SendIntegrationCommandTargetEndModel : SendIntegrationCommandEndModel, IProcessingActionModel
                    {
                        // + common association-end members
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                    }

                    public class SendIntegrationCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

                    public static class SendIntegrationCommandModelAssociationExtensions
                    {
                        static IList<SendIntegrationCommandTargetEndModel> SentIntegrationCommands(this IProcessingHandlerModel model);
                        static IList<SendIntegrationCommandTargetEndModel> GetSentIntegrationCommands(this IEnumerable<IAssociation> associations);
                        static IList<SendIntegrationCommandSourceEndModel> IntegrationCommandSources(this IntegrationCommandModel model);
                    }

                    public class ServicesPackageExtensionModel : ServicesPackageModel
                    {
                        // + everything inherited from ServicesPackageModel
                        IList<IntegrationEventHandlerModel> IntegrationEventHandlers { get; }
                    }

                    // "Subscribe Integration Command" · f485e4aa-a032-4b9a-aa85-5d2c62d75799
                    public class SubscribeIntegrationCommandModel : IMetadataModel
                    {
                        static SubscribeIntegrationCommandModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        SubscribeIntegrationCommandSourceEndModel SourceEnd { get; }
                        SubscribeIntegrationCommandTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Subscribe Integration Command Source End" · cdc0ae0a-1199-4450-8e21-2da80e03bc26
                    public class SubscribeIntegrationCommandSourceEndModel : SubscribeIntegrationCommandEndModel { /* common association-end members */ }

                    // "Subscribe Integration Command Target End" · efa73bdb-69b7-4f52-aa10-15c19874b394
                    public class SubscribeIntegrationCommandTargetEndModel : SubscribeIntegrationCommandEndModel, IProcessingHandlerModel { /* common association-end members */ }

                    public class SubscribeIntegrationCommandEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper, IProcessingHandlerModel { /* common association-end members */ }

                    public static class SubscribeIntegrationCommandModelAssociationExtensions
                    {
                        static IList<SubscribeIntegrationCommandTargetEndModel> IntegrationCommandSubscriptions(this IntegrationEventHandlerModel model);
                        static IList<SubscribeIntegrationCommandSourceEndModel> IntegrationCommandHandlers(this IntegrationCommandModel model);
                    }

                    // "Subscribe Integration Event" · 80aa7f6d-64e5-4d24-a81e-6bc212925ca7
                    public class SubscribeIntegrationEventModel : IMetadataModel
                    {
                        static SubscribeIntegrationEventModel CreateFromEnd(IAssociationEnd associationEnd);
                        string Id { get; }
                        SubscribeIntegrationEventSourceEndModel SourceEnd { get; }
                        SubscribeIntegrationEventTargetEndModel TargetEnd { get; }
                        IAssociation InternalAssociation { get; }
                    }

                    // "Subscribe Integration Event Source End" · 5d8f5c33-f9c9-4629-b637-fad9b0096894
                    public class SubscribeIntegrationEventSourceEndModel : SubscribeIntegrationEventEndModel { /* common association-end members */ }

                    // "Subscribe Integration Event Target End" · 16fa2952-79e6-4150-b5ab-45aa4c106de4
                    public class SubscribeIntegrationEventTargetEndModel : SubscribeIntegrationEventEndModel, IProcessingHandlerModel { /* common association-end members */ }

                    public class SubscribeIntegrationEventEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper, IProcessingHandlerModel { /* common association-end members */ }

                    public static class SubscribeIntegrationEventModelAssociationExtensions
                    {
                        static IList<SubscribeIntegrationEventTargetEndModel> IntegrationEventSubscriptions(this IntegrationEventHandlerModel model);
                        static IList<SubscribeIntegrationEventSourceEndModel> IntegrationEventHandlers(this MessageModel model);
                    }
                    ```

                    ### 2.4 Services.GraphQL — `Intent.Modules.Modelers.Services.GraphQL` (source at `1.1.1`)

                    ```csharp
                    namespace Intent.Modelers.Services.GraphQL.Api;

                    public static class ApiMetadataPackageExtensions
                    {
                        static IList<GraphQLServicesPackageModel> GetGraphQLServicesPackageModels(this IDesigner designer);
                        static bool IsGraphQLServicesPackageModel(this IPackage package);
                    }

                    public static class ApiMetadataProviderExtensions
                    {
                        static IList<GraphQLMutationTypeModel> GetGraphQLMutationTypeModels(this IDesigner designer);
                        static IList<GraphQLQueryTypeModel> GetGraphQLQueryTypeModels(this IDesigner designer);
                        static IList<GraphQLSubscriptionTypeModel> GetGraphQLSubscriptionTypeModels(this IDesigner designer);
                    }

                    public class DTOExtensionModel : DTOModel
                    {
                        // + everything inherited from DTOModel
                        IList<GraphQLSchemaFieldModel> Resolvers { get; }
                    }

                    // "GraphQL Event Message" · e68932de-3dcf-4b2f-8796-59ff28a81efa
                    public class GraphQLEventMessageModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                    }

                    // "GraphQL Mutation" · 66cca984-b1dc-445c-9685-e3abb4e5795a
                    public class GraphQLMutationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                        bool IsMapped { get; }
                        IElementMapping Mapping { get; }
                        IList<GraphQLParameterModel> Parameters { get; }
                    }

                    public static class GraphQLMutationModelExtensions
                    {
                        static bool HasMapToDomainMapping(this GraphQLMutationModel type);
                        static IElementMapping GetMapToDomainMapping(this GraphQLMutationModel type);
                        static bool HasMapToServiceMapping(this GraphQLMutationModel type);
                        static IElementMapping GetMapToServiceMapping(this GraphQLMutationModel type);
                    }

                    // "GraphQL Mutation Type" · 02864de4-afe0-42a1-a655-1eeb68c8a098
                    public class GraphQLMutationTypeModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
                    {
                        // + common element members
                        IList<GraphQLMutationModel> Mutations { get; }
                    }

                    // "GraphQL Parameter" · 22565fd3-564a-4406-a502-81d2b6b0691f
                    public class GraphQLParameterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                    }

                    // "GraphQL Query Type" · 4504e87f-d092-46d6-bbb9-2b39c7307d41
                    public class GraphQLQueryTypeModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
                    {
                        // + common element members
                        IList<GraphQLSchemaFieldModel> Queries { get; }
                    }

                    // "GraphQL Schema Field" · 150aa241-479b-442e-9962-21b79de85648
                    public class GraphQLSchemaFieldModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                        bool IsMapped { get; }
                        IElementMapping Mapping { get; }
                        IList<GraphQLParameterModel> Parameters { get; }
                    }

                    public static class GraphQLSchemaFieldModelExtensions
                    {
                        static bool HasMapToDomainMapping(this GraphQLSchemaFieldModel type);
                        static IElementMapping GetMapToDomainMapping(this GraphQLSchemaFieldModel type);
                        static bool HasMapToServiceMapping(this GraphQLSchemaFieldModel type);
                        static IElementMapping GetMapToServiceMapping(this GraphQLSchemaFieldModel type);
                    }

                    // "GraphQL Services Package" · 4aeae542-361b-4c38-b575-10f0224b17d5
                    public class GraphQLServicesPackageModel : IHasStereotypes, IMetadataModel
                    {
                        // + Id, Name, Stereotypes
                        IPackage UnderlyingPackage { get; }
                        string FileLocation { get; }
                        IList<GraphQLQueryTypeModel> QueryTypes { get; }
                        IList<GraphQLMutationTypeModel> MutationTypes { get; }
                        IList<GraphQLSubscriptionTypeModel> SubscriptionTypes { get; }
                        IList<TypeDefinitionModel> Types { get; }
                    }

                    // "GraphQL Subscription" · 37e9cf40-ac8e-4880-9a4a-92b540a4fea7
                    public class GraphQLSubscriptionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
                    {
                        // + common element members
                        ITypeReference TypeReference { get; }
                        GraphQLEventMessageModel EventMessage { get; }
                    }

                    // "GraphQL Subscription Type" · b09d3b7f-63ad-4518-9a6a-5c7401c57c1b
                    public class GraphQLSubscriptionTypeModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
                    {
                        // + common element members
                        IList<GraphQLSubscriptionModel> Subscriptions { get; }
                    }
                    ```

                    ## 3. Name collisions and gaps

                    - **Two different `CallServiceOperationModel` types.** `Intent.Modelers.Services.DomainInteractions.Api` and `Intent.Modelers.Services.EventInteractions` each declare one, for *different* association types (`3e69085c-…` and `9510ff76-…` respectively). Their navigation methods also differ: `CallServiceOperationActions()` versus `CalledServiceOperations()`. Check the namespace you import.
                    - The DomainInteractions `CallServiceOperationModel` shares its `SpecializationTypeId` with Services' own `PerformInvocationModel` (`3e69085c-…`) — two typed wrappers over the same association type.
                    - `FolderExtensionModel` and `ServicesPackageExtensionModel` are declared in several of these modules (and `ServicesPackageExtensionModel` also in Domain Events). Alias them when you import more than one.
                    - `CommandModel.Properties` and `QueryModel.Properties` return Services' own `DTOFieldModel` — there is no separate command-field type.
                    - **Service Proxy Interactions (`Intent.Modules.Modelers.Services.ProxyInteractions`) ships no C# API** — it contributes designer metadata only. There is nothing to reference from a template.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}