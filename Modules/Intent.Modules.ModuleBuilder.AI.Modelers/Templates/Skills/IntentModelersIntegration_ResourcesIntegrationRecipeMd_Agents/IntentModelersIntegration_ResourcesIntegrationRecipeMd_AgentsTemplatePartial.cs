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

namespace Intent.Modules.ModuleBuilder.AI.Modelers.Templates.Skills.IntentModelersIntegration_ResourcesIntegrationRecipeMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentModelersIntegration_ResourcesIntegrationRecipeMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Modelers.Skills.IntentModelersIntegration_ResourcesIntegrationRecipeMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentModelersIntegration_ResourcesIntegrationRecipeMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("integration-recipe", relativeLocation: "intent-modelers-integration/resources")
                .FromMarkdown(""""""
                    # Integration Recipe — Wiring A Module Against A Modelers Designer

                    The full three-file wiring, with every snippet lifted verbatim from `Intent.Modules.Metadata.RDBMS`
                    — the cleanest consumer in this repo, where all three files agree at the same version (`3.11.1`)
                    against the `Domain` designer. Substitute `Domain` for whichever designer you're integrating with
                    (`Services`, `Eventing`, `User Interface`) and use that designer's own resource file for its real
                    current version and identity — do not copy the `3.11.1` number itself, it is illustrative only.

                    ## 1. `.csproj` — compile-time

                    ```xml
                    <PackageReference Include="Intent.Modules.Modelers.Domain" Version="3.11.1" />
                    ```

                    The NuGet `PackageId` always carries the `Intent.Modules.` prefix — this is what your project
                    compiles against, and what gives you `using Intent.Modelers.Domain.Api;`.

                    ## 2. `.imodspec` — install-time

                    ```xml
                    <dependency id="Intent.Modelers.Domain" version="3.11.1" />
                    ```

                    **Note the different id string** — no `Modules.` segment. This is the identity the Intent Architect
                    installer resolves when your module is installed into a consuming application; get it wrong and the
                    install fails, not the compile, so the mistake surfaces much later than the `.csproj` one would.

                    ## 3. `modules.config` — design-time visibility in your own Module Builder designer

                    ```xml
                    <module moduleId="Intent.Modelers.Domain" version="3.11.1" includeAssets="none" supportedClientVersions="[4.3.0-a, 5.0.0-a)" />
                    ```

                    **You do not write this entry — you install it.** `modules.config` records what is actually installed
                    in the application; hand-editing it corrupts that record. Install the designer module into your
                    module-building application instead, and in the Installation Settings dialog tick **`Install Designer
                    Metadata` and nothing else**:

                    | Setting | Module-building application |
                    |---|---|
                    | **Install Designer Metadata** | tick — this is the one you want |
                    | **Install Designers** | rare; only if the element types still do not appear with metadata alone |
                    | Enable Factory Extensions | leave clear |
                    | Install Application Settings | leave clear |
                    | Install Template Outputs | leave clear |

                    That choice is what produces `includeAssets="none"` above — the attribute is the *result* of a
                    metadata-only install, not something you type. If it is absent, the install was done with the wrong
                    boxes ticked; redo the install rather than editing the file.

                    What this buys you: the designer's element/type vocabulary becomes available to *your own* Module
                    Builder package while you are modelling — e.g. so a `Template Settings` stereotype's `Model Type`
                    property can resolve `ClassModel` — without dragging that module's factory extensions, settings or
                    generated output into an application whose only job is to define a module.

                    ## 4. The `using` + accessor call

                    ```csharp
                    using Intent.Modelers.Domain.Api;

                    var classModels = metadataManager.Domain(application).GetClassModels();
                    ```

                    The API namespace (`Intent.Modelers.Domain.Api`) is one level deeper than the module id
                    (`Intent.Modelers.Domain`) — it always adds a trailing `.Api` segment. Never assume the namespace
                    equals the module id with `.Api` appended to the *NuGet* package id; it's appended to the module id.

                    ## 5. Publishing your own stereotypes onto a foreign designer

                    If your module needs to attach its own stereotypes onto elements owned by a designer you don't own
                    (e.g. RDBMS attaching `Table`/`Column`/`Index` stereotypes onto `Domain`'s `ClassModel`), model those
                    stereotype definitions in your own Module Builder package, then install them into both the target
                    designer and your own Module Builder designer so the stereotype editor works at design-time too:

                    ```xml
                    <install target="Domain;Module Builder" src="Metadata/Domain/Intent.Metadata.RDBMS.pkg.config" externalReference="AF8F3810-745C-42A2-93C8-798860DC45B1" />
                    ```

                    The `target` attribute is a semicolon-separated list of designer names — `Domain;Module Builder` is
                    the shape to copy, substituting the designer(s) you're actually extending.

                    ## 6. The shared model surface — what every designer model hands back

                    Designer models are thin typed wrappers. Their properties return SDK interfaces (`ITypeReference`,
                    `IElement`, `IStereotype`, mappings) and a few shared `Common.Types` models (`FolderModel`,
                    `EnumModel`, `TypeDefinitionModel`). That surface belongs to no single designer, so it is listed once
                    here. Together with §9 "Model API reference" in each designer file (and §2 of its `*-extensions.md`), it covers everything a template
                    touches — **there is no need to reflect over `Intent.SoftwareFactory.SDK.dll`, `Intent.Modules.Common.dll`
                    or a designer assembly to discover members.** Taken from `Intent.SoftwareFactory.SDK` `3.15.0`,
                    `Intent.Modules.Common` `3.11.4` and `Intent.Modules.Common.Types` `4.1.3`; every member is public.

                    ```csharp
                    namespace Intent.Metadata.Models;   // Intent.SoftwareFactory.SDK

                    public interface IMetadataModel { string Id { get; } }
                    public interface IHasName { string Name { get; } }
                    public interface IHasStereotypes { IEnumerable<IStereotype> Stereotypes { get; } }
                    public interface IHasTypeReference { ITypeReference TypeReference { get; } }

                    public interface ICanBeReferencedType : IMetadataModel, IHasStereotypes
                    {
                        string SpecializationType { get; }
                        string SpecializationTypeId { get; }
                        IEnumerable<IMetadataModelTrait> Traits { get; }
                        string Name { get; }
                        string DisplayText { get; }
                        string Comment { get; }
                        ITypeReference TypeReference { get; }
                        IPackage Package { get; }
                    }

                    public interface IElement : ICanBeReferencedType
                    {
                        bool IsChild { get; }
                        int Order { get; }
                        string ExternalReference { get; }
                        string Value { get; }
                        bool IsAbstract { get; }
                        bool IsStatic { get; }
                        IEnumerable<IGenericType> GenericTypes { get; }          // IGenericType : IMetadataModel { string Name }
                        ITypeReference TypeReference { get; }
                        IPackage Package { get; }
                        string ParentId { get; }
                        IElement ParentElement { get; }
                        IEnumerable<IElement> ChildElements { get; }
                        bool IsMapped { get; }
                        IElementMapping MappedElement { get; }
                        IEnumerable<IElementToElementMappedEnd> MappedToElements { get; }
                        IElementApplication Application { get; }                 // IElementApplication : IMetadataModel { string Name }
                        IEnumerable<IAssociationEnd> AssociatedElements { get; }
                        IEnumerable<IAssociation> OwnedAssociations { get; }
                        IDiagram Diagram { get; }
                        IEnumerable<IElementToElementMapping> Mappings { get; }
                        string Comment { get; }
                        IDictionary<string, string> Metadata { get; }
                        IList<IElement> GetParentPath();
                    }

                    public interface ITypeReference : IHasStereotypes
                    {
                        bool IsNullable { get; }
                        bool IsCollection { get; }
                        string ElementId { get; }
                        string GenericTypeId { get; }
                        ICanBeReferencedType Element { get; }                    // the referenced type — convert with As<X>Model()
                        IEnumerable<ITypeReference> GenericTypeParameters { get; }
                        bool Equals(ITypeReference other);
                    }

                    public interface IAssociation : IMetadataModel
                    {
                        IAssociationEnd SourceEnd { get; }
                        IAssociationEnd TargetEnd { get; }
                        string SpecializationType { get; }
                        string SpecializationTypeId { get; }
                        string Comment { get; }
                        bool IsSelfReference();
                    }

                    public interface IAssociationEnd : IElement
                    {
                        IAssociation Association { get; }
                        bool IsNavigable { get; }
                        IDictionary<string, string> Metadata { get; }
                        IAssociationEnd OtherEnd();
                        bool IsTargetEnd();
                        bool IsSourceEnd();
                    }

                    public interface IPackage : ICanBeReferencedType
                    {
                        string FileLocation { get; }
                        string ApplicationId { get; }
                        string DesignerId { get; }
                        IEnumerable<IElement> ChildElements { get; }
                        IEnumerable<IPackageReference> PackageReferences { get; }
                    }

                    public interface IDesigner
                    {
                        IEnumerable<IElement> Elements { get; }
                        IEnumerable<IAssociation> Associations { get; }
                        IEnumerable<IPackage> Packages { get; }
                        IEnumerable<IStereotypeDefinition> StereotypeDefinitions { get; }
                        IEnumerable<IElement> GetElementsOfType(string specializationTypeId);
                        IEnumerable<IPackage> GetPackagesOfType(string specializationTypeId);
                        IEnumerable<IAssociation> GetAssociationsOfType(string specializationTypeId);
                    }

                    public interface IStereotype
                    {
                        string DefinitionId { get; }
                        IStereotypeDefinition Definition { get; }
                        string Name { get; }
                        IEnumerable<IStereotypeProperty> Properties { get; }
                        IStereotypeProperty GetProperty(string nameOrId);
                        bool TryGetProperty(string nameOrId, out IStereotypeProperty property);
                    }

                    public interface IStereotypeProperty
                    {
                        string PropertyDefinitionId { get; }
                        string Key { get; }
                        string Value { get; }
                        StereotypePropertyControlType ControlType { get; }
                        StereotypePropertyOptionsSource OptionsSource { get; }
                    }
                    public interface IStereotypeProperty<T> : IStereotypeProperty { T Value { get; } }

                    // Legacy single mapping — what a model's IsMapped / Mapping / Get…Mapping() return.
                    public interface IElementMapping : ITypeReference           // .Element is the mapped-to element
                    {
                        string ApplicationId { get; }
                        string MetadataId { get; }
                        string ElementId { get; }
                        string MappingSettingsId { get; }
                        IList<IElementMappingPathTarget> Path { get; }
                    }

                    public interface IElementMappingPathTarget
                    {
                        string Id { get; }
                        string Name { get; }
                        string Type { get; }
                        string Specialization { get; }
                        string SpecializationId { get; }
                        ICanBeReferencedType Element { get; }
                    }

                    // Advanced mapping — what an association end's Mappings / Get…Mapping() return.
                    public interface IElementToElementMapping
                    {
                        string Type { get; }
                        string TypeId { get; }
                        ICanBeReferencedType HostElement { get; }
                        ICanBeReferencedType SourceElement { get; }
                        ICanBeReferencedType TargetElement { get; }
                        IList<IElementToElementMappedEnd> MappedEnds { get; }
                    }

                    public interface IElementToElementMappedEnd
                    {
                        string MappingType { get; }
                        string MappingTypeId { get; }
                        string MappingExpression { get; }
                        IElementToElementMapping OwnerMapping { get; }
                        IList<IElementMappingPathTarget> TargetPath { get; }
                        ICanBeReferencedType TargetElement { get; }
                        IEnumerable<IElementToElementMappedEndSource> Sources { get; }
                        IList<IElementMappingPathTarget> SourcePath { get; }
                        ICanBeReferencedType SourceElement { get; }
                        IElementToElementMappedEndSource GetSource(string identifier);
                    }

                    public interface IElementToElementMappedEndSource
                    {
                        string MappingType { get; }
                        string MappingTypeId { get; }
                        string ExpressionIdentifier { get; }
                        IList<IElementMappingPathTarget> Path { get; }
                        ICanBeReferencedType Element { get; }
                    }

                    namespace Intent.Engine;   // Intent.SoftwareFactory.SDK

                    public interface IMetadataManager   // the designer accessors (.Domain(app), .Services(app), …) extend this
                    {
                        IDesigner GetDesigner(string applicationId, string designerId);
                        IEnumerable<TMetaModel> GetMetadata<TMetaModel>(MetadataIdentifier name);
                        IEnumerable<TMetaModel> GetMetadata<TMetaModel>(MetadataIdentifier name, string applicationId);
                        IEnumerable<TMetaModel> GetSolutionMetadata<TMetaModel>(MetadataIdentifier name);
                        void Register(IMetadataProvider provider);
                    }

                    namespace Intent.Modules.Common;   // Intent.Modules.Common

                    public interface IElementWrapper { IElement InternalElement { get; } }

                    // Empty marker traits (IElementWrapper + IMetadataModel, no members of their own). Navigation
                    // extensions such as CreateEntityActions(this IProcessingHandlerModel) hang off them.
                    public interface IProcessingHandlerModel : IElementWrapper, IMetadataModel { }
                    public interface IProcessingActionModel : IElementWrapper, IMetadataModel { }
                    public interface IInvokableModel : IElementWrapper, IMetadataModel { }
                    public interface IAllowCommentModel : IElementWrapper, IMetadataModel { }

                    public static class StereotypeExtensions   // untyped fallback — prefer a designer's generated Get<Stereotype>()
                    {
                        static bool HasStereotype(this IHasStereotypes model, string stereotypeNameOrId);
                        static IStereotype GetStereotype(this IHasStereotypes model, string stereotypeNameOrId);
                        static IReadOnlyCollection<IStereotype> GetStereotypes(this IHasStereotypes model, string stereotypeNameOrId);
                        static T GetStereotypeProperty<T>(this IHasStereotypes model, string stereotypeName, string propertyName, T defaultIfNotFound = default);
                        static T GetPropertyValue<T>(this IHasStereotypes model, string stereotypeName, string propertyName, T defaultIfNotFound = default);
                        static T GetProperty<T>(this IStereotype stereotype, string propertyName, T defaultIfNotFound = default);
                    }

                    public static class ElementExtensions
                    {
                        static IEnumerable<IElement> GetElementsOfType(this IEnumerable<IElement> elements, string typeId, bool recursiveSearch = false);
                        static IEnumerable<IElement> GetMatchedElements(this IEnumerable<IElement> elements, Func<IElement, bool> matchFunc, bool recursiveSearch);
                    }

                    namespace Intent.Modules.Common.Types.Api;   // Intent.Modules.Common.Types

                    // "Folder" · 4d95d53a-8855-4f35-aa82-e312643f5c5f
                    public class FolderModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasName, IFolder, IHasFolder<IFolder>, IElementWrapper
                    {
                        // + common element members (Id, Name, Comment, Stereotypes, InternalElement)
                        FolderModel Folder { get; set; }          // parent folder, null at package root
                        IList<FolderModel> Folders { get; }
                    }
                    public interface IFolder : IHasStereotypes { string Name { get; } }
                    public interface IHasFolder { FolderModel Folder { get; } }
                    public interface IHasFolder<out TFolder> { TFolder Folder { get; } }

                    public static class FolderExtensions
                    {
                        static IList<FolderModel> GetParentFolders(this IHasFolder model);
                        static IList<IFolder> GetParentFolders(this IHasFolder<IFolder> model);
                        static IList<string> GetParentFolderNames(this IHasFolder model);
                        static IList<string> GetParentFolderNames(this IHasFolder<IFolder> model);
                        static IStereotype GetStereotypeInFolders(this IHasFolder model, string stereotypeName);
                    }

                    // "Enum" · 85fba0e9-9161-4c85-a603-a229ef312beb
                    public class EnumModel : IHasStereotypes, IMetadataModel, IHasFolder, IHasFolder<IFolder>, IHasName, IElementWrapper
                    {
                        // + common element members
                        FolderModel Folder { get; }
                        IList<EnumLiteralModel> Literals { get; }
                    }

                    // "Enum-Literal" · 4215f417-25d2-4509-9309-5076a1452eaa
                    public class EnumLiteralModel : IHasStereotypes, IMetadataModel, IHasName, IElementWrapper
                    {
                        // + common element members
                        string Value { get; }
                    }

                    // "Type-Definition" · d4e577cd-ad05-4180-9a2e-fff4ddea0e1e
                    public class TypeDefinitionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
                    {
                        // + common element members
                        IEnumerable<string> GenericTypes { get; }
                        IList<AttributeModel> Attributes { get; }        // Common.Types' own AttributeModel / OperationModel,
                        IList<OperationModel> Operations { get; }        // not Domain's — see domain.md §9 name collisions
                    }
                    ```

                    Common moves, all expressible with the members above:

                    | Need | Expression |
                    |---|---|
                    | A property's type name / kind | `attr.TypeReference.Element.Name`; `attr.TypeReference.Element.IsEnumModel()` / `.AsClassModel()` |
                    | Collection / nullable | `attr.TypeReference.IsCollection`, `attr.TypeReference.IsNullable` |
                    | Generic arguments | `attr.TypeReference.GenericTypeParameters` (each is an `ITypeReference`) |
                    | A stereotype with a generated accessor | the designer's typed `Get<Stereotype>()` / `Has<Stereotype>()` (listed in each designer's §9, or its extensions file) |
                    | A stereotype from another module, with no typed accessor | `model.GetStereotypeProperty<string>("Stereotype Name", "Property Name")` |
                    | A child element the model does not surface as a property | `model.InternalElement.ChildElements.GetElementsOfType(XModel.SpecializationTypeId)` — use the model's const, never a hand-typed GUID |
                    | The folder path of an element | `model.GetParentFolderNames()` |

                    ## The `Api/` naming convention — how to answer "what's in this designer" for one this skill doesn't cover

                    Every fact in `domain.md` / `services.md` / `eventing.md` / `user-interface.md` was derived
                    mechanically from a designer module's generated `Api/` folder. Use this table to answer the same
                    questions yourself for a fifth designer, or to re-verify a fact that may have drifted since these
                    resource files were written. For the four designers covered here you should not need it: §9 of
                    each designer's resource file lists every public member of the base designer, §2 of its
                    `*-extensions.md` file does the same for its extension modules, and §6 above covers the SDK
                    types those members return.

                    | Looking for | Read |
                    |---|---|
                    | Element types | `Api/<X>Model.cs` → `SpecializationType` + `SpecializationTypeId` consts |
                    | Association types | `Api/<X>AssociationModel.cs` (⚠️ Eventing spells it `Assocation`) |
                    | Association navigation | `Api/Extensions/<X>ModelAssociationExtensions.cs` — **Domain & Services**; `Api/<X>ModelAssociationExtensions.cs` directly — **Eventing & User Interface** |
                    | Stereotypes | `Api/<X>ModelStereotypeExtensions.cs` |
                    | Mappings | `Api/ElementToElementMappingExtensions.cs` (often in an extension module, not the base designer — e.g. Services' mapping lives in `.DomainInteractions`, not the base `Services` module) |
                    | Package model | `Api/<X>PackageModel.cs` |
                    | Cross-element abstractions | `Api/I<X>Model.cs` |
                    | Accessor + designer GUID | `ApiMetadataDesignerExtensions.cs` at the **module root**, not under `Api/` |
                    | Publish/subscribe-filtered accessors (Eventing only) | `MetadataManagerExtensions.cs` at the module root, namespace `Intent.Modules.Modelers.Eventing` — **not** `Api/ApiMetadataProviderExtensions.cs`, and **not** the `.Api`-suffixed namespace |

                    Stereotype ownership is not uniform across designers, and this is the single most likely thing to
                    get wrong: `Domain` and `Services` define **no stereotypes of their own** — every stereotype you'll
                    see on a `ClassModel` or `DTOModel` (`Table`, `Column`, `HttpSettings`, `Secured`, …) arrives from a
                    separate `Intent.Metadata.*` module that must be installed and referenced independently, with its
                    own full install identity. `User Interface` is the opposite — it ships
                    `ComponentModelStereotypeExtensions`, `PropertyModelStereotypeExtensions` and
                    `EventEmitterModelStereotypeExtensions` directly in its own `Api/`. Always check which case a
                    designer is in before assuming a stereotype either belongs to it or comes from elsewhere.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
