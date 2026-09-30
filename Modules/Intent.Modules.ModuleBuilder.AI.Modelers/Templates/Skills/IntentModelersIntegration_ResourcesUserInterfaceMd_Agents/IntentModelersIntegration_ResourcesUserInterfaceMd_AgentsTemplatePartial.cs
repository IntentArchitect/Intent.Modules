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

namespace Intent.Modules.ModuleBuilder.AI.Modelers.Templates.Skills.IntentModelersIntegration_ResourcesUserInterfaceMd_Agents
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class IntentModelersIntegration_ResourcesUserInterfaceMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.ModuleBuilder.AI.Modelers.Skills.IntentModelersIntegration_ResourcesUserInterfaceMd_Agents";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public IntentModelersIntegration_ResourcesUserInterfaceMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile("user-interface", relativeLocation: "intent-modelers-integration/resources")
        .FromMarkdown(""""""
          # User Interface Designer (`Intent.Modules.Modelers.UI`)

          The richest of the four designers — this file is deliberately the longest. Only read the sections
          you need for the widget/association/stereotype you're touching.

          ## 1. What this designer is for

          The User Interface designer models the client-side surface of an application: pages, dialogs,
          reusable UI components, layout shells (header/sider/body/footer/profile menu), the widgets that
          live inside a component's view tree, the events a component can raise, the operations a component
          can invoke (including calls to service-proxy operations and navigation to other components), and
          lightweight view-model definitions (`Model Definition`) used for form/table binding.

          The base module ships **no templates itself** — `<templates></templates>` is empty in its
          `.imodspec`. It is a pure metadata/API module; concrete rendering templates live in downstream
          client-stack modules (Blazor/Angular/React/etc.) that read this designer's model through the `Api`
          classes described below.

          **What it is not:** it is not a widget library — the base module defines no concrete widgets
          (no Button, Form, Table); those are added by the `.Core` extension module (`user-interface-extensions.md`). It is not a
          domain/service designer — it has its own `Model Definition` (a lightweight DTO-shaped view model)
          rather than reusing Domain's `ClassModel`, and its own `Call Service Operation Action` association
          rather than reusing Services' invocation model. The base module's own `.csproj` deliberately
          excludes a `PackageReference` to `.Core` (commented out, with the comment "we don't want this
          dependency — that module is the base for Component modules") — the dependency direction is
          `.Core` → base, never the reverse.

          **Extension modules — read `user-interface-extensions.md` for these.** Each is installed separately, and
          its install identity and full model API are in that file, not this one: `.Core`, the widget library
          (`ButtonModel`, `FormModel`, `TableModel`/`ColumnModel`, `TextInputModel`, `SelectModel`, `CheckboxModel`,
          `RadioGroupModel`, `AutoCompleteModel`, `DatePickerModel`, `DialogModel`, `CardModel`, `NavigationMenuModel`/
          `MenuItemModel`, `ContainerModel`, `TextModel`, `IconModel`, `ImageModel`, `LinkModel`, plus the
          `ShowDialogModel` association), and `.ServiceProxies` (`UserInterfacePackageExtensionModel`, which exposes the
          package's `ServiceProxies`).

          ## 2. Install identity

          | Item | Value |
          |---|---|
          | NuGet PackageId | `Intent.Modules.Modelers.UI` |
          | Intent module id | `Intent.Modelers.UI` |
          | API namespace (concrete models, associations, package model, stereotypes) | `Intent.Modelers.UI.Api` |
          | API namespace (the six trait interfaces only — see §4) | `Intent.Modules.Modelers.UI.Api` ⚠️ **extra `.Modules.` segment, different from every concrete class's namespace** |
          | Designer GUID | `f492faed-0665-4513-9853-5a230721786f` |
          | Accessor | **`.UserInterface(app)`** ⚠️ deliberately different from the module name `UI` — there is no `.UI(...)` accessor anywhere |

          ```csharp
          // ApiMetadataDesignerExtensions.cs, namespace Intent.Modelers.UI.Api
          public const string UserInterfaceDesignerId = "f492faed-0665-4513-9853-5a230721786f";
          public static IDesigner UserInterface(this IMetadataManager metadataManager, IApplication application) { ... }
          ```

          ⚠️ The `.csproj`'s `<RootNamespace>` is set to `Intent.Modelers.Services` — a leftover
          copy-paste from the Services modeler template. It has no effect on any actual namespace in the
          code (every file uses an explicit `namespace` declaration) — ignore it, trust the per-file
          declarations instead.

          ## 3. Entry points

          All at the module root (`ApiMetadataDesignerExtensions.cs`, `ApiMetadataProviderExtensions.cs`,
          `ApiMetadataPackageExtensions.cs`), namespace `Intent.Modelers.UI.Api`:

          ```csharp
          IDesigner ui = metadataManager.UserInterface(application);

          IList<ComponentModel>       components = ui.GetComponentModels();
          IList<ComponentViewModel>   views       = ui.GetComponentViewModels();
          IList<DiagramModel>         diagrams    = ui.GetDiagramModels();
          IList<LayoutModel>          layouts     = ui.GetLayoutModels();
          IList<ModelDefinitionModel> modelDefs   = ui.GetModelDefinitionModels();

          IList<UserInterfacePackageModel> packages = ui.GetUserInterfacePackageModels();
          ```

          Not independently enumerable — reached only by navigating down from a parent: `PropertyModel`,
          `EventEmitterModel`, `ComponentOperationModel`, `ReturnModel`, `InvocationModel`, association ends,
          and `ModelDefinitionModel`'s children.

          ## 4. Designer elements

          ### Elements

          | Class | `SpecializationType` | `SpecializationTypeId` | Notes |
          |---|---|---|---|
          | `ComponentModel` | `Component` | `b1c481e1-e91e-4c29-9817-00ab9cad4b6b` | Implements `IComponentModel`, `IHasFolder`. Children: `Properties`, `EventEmitters`, `Operations`, single `View`, `ModelDefinitions`. Pages, dialogs and composables are ALL `Component` elements — distinguished only by which stereotype is attached (§6) |
          | `ComponentViewModel` | `Component View` | `624513a6-cba8-4dde-8ebe-6b19f00f0364` | `IHasTypeReference`; the single child of a `ComponentModel` carrying its rendered/bound type |
          | `ComponentOperationModel` | `Component Operation` | `e030c97a-e066-40a7-8188-808c275df3cb` | `IHasTypeReference`, `IProcessingHandlerModel`. Children: `Parameters`, `Invocations`, single `Return` |
          | `DisplayComponentModel` | `Display Component` | `866a90f7-4044-43b9-bb05-7270c7889796` | Implements `IComponentModel`, `IHasTypeReference`. Leaf/presentational component reference |
          | `LayoutModel` | `Layout` | `776a9393-6b23-4a8c-8937-fd7e833fa0ef` | Application shell: single `Header`, `Sider`, `Body`, `Footer`, `ProfileMenu`, plus `Properties`/`Operations` |
          | `LayoutHeaderModel` | `Layout Header` | `a6c3a89e-5932-4ab6-a406-75444f05beee` | |
          | `LayoutBodyModel` | `Layout Body` | `11636699-8bad-4693-8c15-8141bd66d04f` | |
          | `LayoutFooterModel` | `Layout Footer` | `5d4fe8e9-2ccf-42ea-af60-04c6970c9ecb` | |
          | `LayoutSiderModel` | `Layout Sider` | `c505f35f-7148-46a3-a812-d9f53a174490` | |
          | `LayoutProfileMenuModel` | `Layout Profile Menu` | `2362db31-5a1f-4db1-b437-4b3b5a193ea4` | |
          | `PropertyModel` | `Property` | `356fbe17-bc63-4e16-b915-feefbc063cbe` | `IHasTypeReference`; has `Value` — a field on a Component, Layout or Model Definition |
          | `EventEmitterModel` | `Event Emitter` | `d6739ffc-30e6-4170-a105-bf28e69aa578` | `IHasTypeReference`; children: `Parameters` — an event a component raises |
          | `InvocationModel` | `Invocation` | `18f87cd6-d8d8-4518-8931-58653d537467` | `IInvokableModel`, `IHasTypeReference`; exposes `ResponseType` |
          | `ModelDefinitionModel` | `Model Definition` | `bd3941b5-e3b3-4a40-96e6-b9c87cea0101` | `IHasFolder`. Own DTO/view-model shape: `Constructors`, `Properties`, `Operations` (using the **shared** `Intent.Modules.Common.Types.Api` types, not UI-specific ones). `IsMapped`/`Mapping`, `HasMapFromDTOMapping()` keyed on `MappingSettingsId "31b3d3a7-bf3c-4bb4-8b1d-9e18b6a8bcdd"` |
          | `ReturnModel` | `Return` | `415ffab7-9865-4200-89a0-b592d24919dd` | At most one per `ComponentOperationModel` |
          | `DiagramModel` | `Diagram` | `4912c89a-77eb-497e-a3f4-7408b6a20886` | `IHasFolder` |
          | `FolderExtensionModel` | *(extends shared `FolderModel`)* | *(inherits)* | Adds `Components`, `Layouts`, `ModelDefinitions`, `Diagrams`, `TypeDefinitions` |

          `FolderModel`, `ParameterModel`, `ConstructorModel`, `OperationModel`, `TypeDefinitionModel` are
          reused from the shared `Intent.Modules.Common.Types.Api` package, not owned by this designer.

          ### Abstractions (trait interfaces)

          All six live in `Intent.Modules.Modelers.UI.Api` (⚠️ different from the concrete classes'
          `Intent.Modelers.UI.Api`), generated from Stereotype Definitions into
          `I{Name}Model : IElementWrapper, IMetadataModel` marker interfaces:

          | Interface | Meaning | Actually implemented? |
          |---|---|---|
          | `IComponentModel` | Any reusable UI component | ✅ Yes — `ComponentModel`, `DisplayComponentModel`, and every `.Core` widget. **Safe to bind to** for "any renderable widget" |
          | `IComposableModel` | A Component that is neither Page nor Dialog | ❌ No concrete class implements it (base, `.Core`, `.ServiceProxies` all checked) — documentation/intent only |
          | `IPageModel` | A routable page | ❌ No implementer. `ComponentModel` with the `Page` stereotype (§6) is conceptually a page but does not declare `: IPageModel`, so `NavigateBackComponents(this IPageModel)` is unreachable from it |
          | `IDialogModel` | A modal dialog | ❌ No implementer, including `.Core`'s own `DialogModel` — so `.Core`'s `ShowDialogSources(this IDialogModel)` is likewise unreachable as shipped |
          | `IInvokableModel` | Directly callable | ✅ Yes — every association target end that represents a call target (`CompositionTargetEndModel`, `NavigationTargetEndModel`, `CallServiceOperationActionTargetEndModel`, `.Core`'s `ShowDialogTargetEndModel`), plus `InvocationModel`. **Safe to bind to** |
          | `IInvokableServiceOperationModel` | A directly callable operation | ❌ No implementer — `ComponentOperationModel` implements `IProcessingHandlerModel` instead |

          **Practical guidance:** only `IComponentModel` and `IInvokableModel` are safe to code against. For
          "give me all pages", use `ui.GetComponentModels().Where(c => c.HasPage())` against the concrete
          `ComponentModel` — don't try to obtain an `IPageModel`, nothing produces one.

          ## 5. Associations

          | Association | `SpecializationType` | `SpecializationTypeId` | Target-end trait | Notes |
          |---|---|---|---|---|
          | `CompositionModel` | `Composition` | `503b9ea9-4e8b-41d7-bbc6-92c97666c476` | `IInvokableModel` | ⚠️ **No `CompositionModelAssociationExtensions.cs` exists** — no generated `.Compositions()` helper, unlike the other two. Filter `AssociatedElements` by specialization manually |
          | `NavigationModel` | `Navigation` | `6d2b2070-c1cb-4cd2-88b4-4e5f8414bd9e` | `IInvokableModel`; exposes `Parameters`, `Mappings` | Has a full extension file: `NavigateToComponents()` on `ComponentModel`/`ComponentOperationModel`/`LayoutModel`/layout-part models, `NavigateBackComponents()` on `IPageModel` (unreachable — see §4) |
          | `CallServiceOperationActionModel` | `Call Service Operation Action` | `fe5a5cd8-aabd-472f-8d42-f5c233e658dc` | `IInvokableModel`; exposes `Mappings` | Extension file exposes `CallServiceOperationActionTargets(this IProcessingHandlerModel)`, `GetMapInvocationMapping()` (`e4a4111b-...`), `GetMapResponseMapping()` (`e60890c6-...`) |

          ⚠️ **Navigation extension files sit directly under `Api/`, not `Api/Extensions/`** — same
          convention as Eventing, differing from Domain/Services.

          ## 6. Stereotypes

          Unlike Domain/Services, UI **defines and owns its own stereotype-typed-accessor classes**:

          - **`ComponentModelStereotypeExtensions`** on `ComponentModel`: `Composable` (`5a2ba6fc-...`, just a name), `Dialog` (`1f4165ee-...`, just a name), `Page` (`ea4adc09-...`, `Route()`/`Title()`), `Secured` (`012f5173-...`, `Roles()`/`Policy()`, multi-apply).
          - **`PropertyModelStereotypeExtensions`** on `PropertyModel`: `Bindable` (`12ba7bea-...`), `RouteParameter` (`f324c4ea-...`), `QueryParameter` (`5c99275d-...`).
          - **`EventEmitterModelStereotypeExtensions`** on `EventEmitterModel`: `Bindable` (same `12ba7bea-...` id as `PropertyModel`'s — pairs a bindable property with its change emitter).

          `.Core` ships a **second, duplicate** `ComponentModelStereotypeExtensions` (namespace `Intent.Modelers.UI.Core.Api`) re-exposing `Secured` with the identical `DefinitionId` — harmless, but two classes share the name across two namespaces/assemblies.

          ## 7. Mappings

          No bespoke mapping element types — UI plugs into the shared `IElementToElementMapping`/
          `IElementMapping` mechanism at three points: `NavigationTargetEndModel.Mappings`,
          `CallServiceOperationActionTargetEndModel.Mappings` (with `GetMapInvocationMapping()`/
          `GetMapResponseMapping()` typed helpers), and `ModelDefinitionModel.Mapping`
          (`GetMapFromDTOMapping()`, keyed on `31b3d3a7-...`).

          ## 8. Worked snippet

          Registration binds the concrete, enumerable root type — `ComponentModel` — since there is no
          `GetIComponentModels()` provider; `IComponentModel` is a capability check, not an enumeration
          surface:

          ```csharp
          using Intent.Engine;
          using Intent.Modelers.UI.Api;
          using Intent.Modules.Common.Templates;

          namespace MyModule.Templates
          {
            public class ComponentPartialTemplateRegistration : FilePerModelTemplateRegistration<ComponentModel>
            {
                public const string TemplateId = "MyModule.ComponentPartial";
            }
          }
          ```

          Inside the template, bind to `IComponentModel` for logic that must work uniformly over the base
          `ComponentModel` or any `.Core` widget:

          ```csharp
          public string RenderChild(IComponentModel widget)
          {
            // Works for ComponentModel, DisplayComponentModel, FormModel, ButtonModel, TableModel, ...
            return widget.Name;
          }
          ```

          Factory-extension-style access, pulling pages/dialogs/layouts out of the designer:

          ```csharp
          using System.Linq;
          using Intent.Engine;
          using Intent.Modelers.UI.Api;

          namespace MyModule.FactoryExtensions
          {
            public class UIDesignerScanner : FactoryExtensionBase
            {
                public override void Execute(IApplication application)
                {
                    var ui = application.MetadataManager.UserInterface(application);

                    var pages = ui.GetComponentModels().Where(c => c.HasPage()).ToList();
                    var dialogs = ui.GetComponentModels().Where(c => c.HasDialog()).ToList();
                    var layouts = ui.GetLayoutModels();
                    var uiPackages = ui.GetUserInterfacePackageModels();
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

          ### 9.1 UI — `Intent.Modules.Modelers.UI` (source at `1.1.5`)

          ```csharp
          namespace Intent.Modelers.UI.Api;

          public static class ApiMetadataDesignerExtensions
          {
              const string UserInterfaceDesignerId = "f492faed-0665-4513-9853-5a230721786f";
              static IDesigner UserInterface(this IMetadataManager metadataManager, IApplication application);
              static IDesigner UserInterface(this IMetadataManager metadataManager, string applicationId);
          }

          public static class ApiMetadataPackageExtensions
          {
              static IList<UserInterfacePackageModel> GetUserInterfacePackageModels(this IDesigner designer);
              static bool IsUserInterfacePackageModel(this IPackage package);
          }

          public static class ApiMetadataProviderExtensions
          {
              static IList<ComponentModel> GetComponentModels(this IDesigner designer);
              static IList<ComponentViewModel> GetComponentViewModels(this IDesigner designer);
              static IList<DiagramModel> GetDiagramModels(this IDesigner designer);
              static IList<LayoutModel> GetLayoutModels(this IDesigner designer);
              static IList<ModelDefinitionModel> GetModelDefinitionModels(this IDesigner designer);
          }

          // "Call Service Operation Action" · fe5a5cd8-aabd-472f-8d42-f5c233e658dc
          public class CallServiceOperationActionModel : IMetadataModel
          {
              static CallServiceOperationActionModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              CallServiceOperationActionSourceEndModel SourceEnd { get; }
              CallServiceOperationActionTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Call Service Operation Action Source End" · 936e090c-8408-429d-b2f6-2eb8deecc428
          public class CallServiceOperationActionSourceEndModel : CallServiceOperationActionEndModel { /* common association-end members */ }

          // "Call Service Operation Action Target End" · 475f0810-2b4a-40da-8eb8-697cb62f7dbe
          public class CallServiceOperationActionTargetEndModel : CallServiceOperationActionEndModel, IInvokableModel
          {
              // + common association-end members
              IEnumerable<IElementToElementMapping> Mappings { get; }
          }

          public class CallServiceOperationActionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          public static class CallServiceOperationActionModelAssociationExtensions
          {
              static IList<CallServiceOperationActionTargetEndModel> CallServiceOperationActionTargets(this IProcessingHandlerModel model);
              static IElementToElementMapping GetMapInvocationMapping(this CallServiceOperationActionTargetEndModel model);
              static IElementToElementMapping GetMapResponseMapping(this CallServiceOperationActionTargetEndModel model);
          }

          // "Component" · b1c481e1-e91e-4c29-9817-00ab9cad4b6b
          public class ComponentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
              IList<PropertyModel> Properties { get; }
              IList<EventEmitterModel> EventEmitters { get; }
              IList<ComponentOperationModel> Operations { get; }
              ComponentViewModel View { get; }
              IList<ModelDefinitionModel> ModelDefinitions { get; }
          }

          public static class ComponentModelStereotypeExtensions
          {
              static Composable GetComposable(this ComponentModel model);
              static bool HasComposable(this ComponentModel model);
              static bool TryGetComposable(this ComponentModel model, out Composable stereotype);
              static Dialog GetDialog(this ComponentModel model);
              static bool HasDialog(this ComponentModel model);
              static bool TryGetDialog(this ComponentModel model, out Dialog stereotype);
              static Page GetPage(this ComponentModel model);
              static bool HasPage(this ComponentModel model);
              static bool TryGetPage(this ComponentModel model, out Page stereotype);
              static Secured GetSecured(this ComponentModel model);
              static IReadOnlyCollection<Secured> GetSecureds(this ComponentModel model);
              static bool HasSecured(this ComponentModel model);
              static bool TryGetSecured(this ComponentModel model, out Secured stereotype);
              public class Composable
              {
                  const string DefinitionId = "5a2ba6fc-8512-4801-8b14-9d532c9c2616";
                  string Name { get; }
              }
              public class Dialog
              {
                  const string DefinitionId = "1f4165ee-41a0-4520-a193-9ae4d3413d1f";
                  string Name { get; }
              }
              public class Page
              {
                  const string DefinitionId = "ea4adc09-8978-4ede-ba5f-265debb2b60c";
                  string Name { get; }
                  string Route();
                  string Title();
              }
              public class Secured
              {
                  const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
                  string Name { get; }
                  string Roles();
                  string Policy();
              }
          }

          // "Component Operation" · e030c97a-e066-40a7-8188-808c275df3cb
          public class ComponentOperationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference, IProcessingHandlerModel
          {
              // + common element members
              IEnumerable<string> GenericTypes { get; }
              ITypeReference TypeReference { get; }
              ITypeReference ReturnType { get; }
              IList<ParameterModel> Parameters { get; }
              IList<InvocationModel> Invocations { get; }
              ReturnModel Return { get; }
          }

          // "Component View" · 624513a6-cba8-4dde-8ebe-6b19f00f0364
          public class ComponentViewModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
          {
              // + common element members
              ITypeReference TypeReference { get; }
          }

          // "Composition" · 503b9ea9-4e8b-41d7-bbc6-92c97666c476
          public class CompositionModel : IMetadataModel
          {
              static CompositionModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              CompositionSourceEndModel SourceEnd { get; }
              CompositionTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Composition Source End" · e5e22007-149b-499c-b2e9-5c17064b606a
          public class CompositionSourceEndModel : CompositionEndModel { /* common association-end members */ }

          // "Composition Target End" · 15131ce2-31a6-461a-9889-42ec0f8f980b
          public class CompositionTargetEndModel : CompositionEndModel, IInvokableModel { /* common association-end members */ }

          public class CompositionEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          // "Diagram" · 4912c89a-77eb-497e-a3f4-7408b6a20886
          public class DiagramModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
          }

          // "Display Component" · 866a90f7-4044-43b9-bb05-7270c7889796
          public class DisplayComponentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel, IHasTypeReference
          {
              // + common element members
              ITypeReference TypeReference { get; }
          }

          // "Event Emitter" · d6739ffc-30e6-4170-a105-bf28e69aa578
          public class EventEmitterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
          {
              // + common element members
              ITypeReference TypeReference { get; }
              IList<ParameterModel> Parameters { get; }
          }

          public static class EventEmitterModelStereotypeExtensions
          {
              static Bindable GetBindable(this EventEmitterModel model);
              static bool HasBindable(this EventEmitterModel model);
              static bool TryGetBindable(this EventEmitterModel model, out Bindable stereotype);
              public class Bindable
              {
                  const string DefinitionId = "12ba7bea-ceb9-44d4-8819-835fe36af7b3";
                  string Name { get; }
              }
          }

          public class FolderExtensionModel : FolderModel
          {
              // + everything inherited from FolderModel
              IList<ComponentModel> Components { get; }
              IList<LayoutModel> Layouts { get; }
              IList<ModelDefinitionModel> ModelDefinitions { get; }
              IList<DiagramModel> Diagrams { get; }
              IList<TypeDefinitionModel> TypeDefinitions { get; }
          }

          namespace Intent.Modules.Modelers.UI.Api;

          // Marks the element as a reusable UI component, representing a piece of the user interface with its own logic, parameters, and rendering behavior.
          public interface IComponentModel : IElementWrapper, IMetadataModel { }

          // Auto-managed marker. Applied to Components that are not Pages or Dialogs. Used by Composition target filtering.
          public interface IComposableModel : IElementWrapper, IMetadataModel { }

          // Marks the component as a dialog (modal) opened from other UI, not via routing.
          public interface IDialogModel : IElementWrapper, IMetadataModel { }

          // Marks the element as directly callable.
          public interface IInvokableModel : IElementWrapper, IMetadataModel { }

          // Marks the operation as directly callable.
          public interface IInvokableServiceOperationModel : IElementWrapper, IMetadataModel { }

          // Marks the component as a routable page within the application.
          public interface IPageModel : IElementWrapper, IMetadataModel { }

          namespace Intent.Modelers.UI.Api;

          // "Invocation" · 18f87cd6-d8d8-4518-8931-58653d537467
          public class InvocationModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IInvokableModel, IHasTypeReference
          {
              // + common element members
              ITypeReference TypeReference { get; }
              ITypeReference ResponseType { get; }
          }

          // "Layout Body" · 11636699-8bad-4693-8c15-8141bd66d04f
          public class LayoutBodyModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "Layout Footer" · 5d4fe8e9-2ccf-42ea-af60-04c6970c9ecb
          public class LayoutFooterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "Layout Header" · a6c3a89e-5932-4ab6-a406-75444f05beee
          public class LayoutHeaderModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "Layout" · 776a9393-6b23-4a8c-8937-fd7e833fa0ef
          public class LayoutModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
              LayoutHeaderModel Header { get; }
              LayoutSiderModel Sider { get; }
              LayoutBodyModel Body { get; }
              LayoutFooterModel Footer { get; }
              LayoutProfileMenuModel ProfileMenu { get; }
              IList<PropertyModel> Properties { get; }
              IList<ComponentOperationModel> Operations { get; }
          }

          // "Layout Profile Menu" · 2362db31-5a1f-4db1-b437-4b3b5a193ea4
          public class LayoutProfileMenuModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "Layout Sider" · c505f35f-7148-46a3-a812-d9f53a174490
          public class LayoutSiderModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "Model Definition" · bd3941b5-e3b3-4a40-96e6-b9c87cea0101
          public class ModelDefinitionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
          {
              // + common element members
              FolderModel Folder { get; }
              IEnumerable<string> GenericTypes { get; }
              bool IsMapped { get; }
              IElementMapping Mapping { get; }
              IList<ConstructorModel> Constructors { get; }
              IList<PropertyModel> Properties { get; }
              IList<OperationModel> Operations { get; }
          }

          public static class ModelDefinitionModelExtensions
          {
              static bool HasMapFromDTOMapping(this ModelDefinitionModel type);
              static IElementMapping GetMapFromDTOMapping(this ModelDefinitionModel type);
          }

          // "Navigation" · 6d2b2070-c1cb-4cd2-88b4-4e5f8414bd9e
          public class NavigationModel : IMetadataModel
          {
              static NavigationModel CreateFromEnd(IAssociationEnd associationEnd);
              string Id { get; }
              NavigationSourceEndModel SourceEnd { get; }
              NavigationTargetEndModel TargetEnd { get; }
              IAssociation InternalAssociation { get; }
          }

          // "Navigation Source End" · 97a3de8a-c9bf-4cf2-bc0a-b8692b02211b
          public class NavigationSourceEndModel : NavigationEndModel { /* common association-end members */ }

          // "Navigation Target End" · 2b191288-ecae-4743-b069-cbdd927ef349
          public class NavigationTargetEndModel : NavigationEndModel, IInvokableModel
          {
              // + common association-end members
              IList<ParameterModel> Parameters { get; }
              IEnumerable<IElementToElementMapping> Mappings { get; }
          }

          public class NavigationEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

          public static class NavigationModelAssociationExtensions
          {
              static IList<NavigationTargetEndModel> NavigateToComponents(this ComponentModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this ComponentOperationModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this LayoutModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this LayoutHeaderModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this LayoutBodyModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this LayoutSiderModel model);
              static IList<NavigationTargetEndModel> NavigateToComponents(this LayoutFooterModel model);
              static IList<NavigationSourceEndModel> NavigateBackComponents(this IPageModel model);
          }

          // "Property" · 356fbe17-bc63-4e16-b915-feefbc063cbe
          public class PropertyModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
          {
              // + common element members
              string Value { get; }
              ITypeReference TypeReference { get; }
          }

          public static class PropertyModelStereotypeExtensions
          {
              static Bindable GetBindable(this PropertyModel model);
              static bool HasBindable(this PropertyModel model);
              static bool TryGetBindable(this PropertyModel model, out Bindable stereotype);
              static RouteParameter GetRouteParameter(this PropertyModel model);
              static bool HasRouteParameter(this PropertyModel model);
              static bool TryGetRouteParameter(this PropertyModel model, out RouteParameter stereotype);
              static QueryParameter GetQueryParameter(this PropertyModel model);
              static bool HasQueryParameter(this PropertyModel model);
              static bool TryGetQueryParameter(this PropertyModel model, out QueryParameter stereotype);
              public class Bindable
              {
                  const string DefinitionId = "12ba7bea-ceb9-44d4-8819-835fe36af7b3";
                  string Name { get; }
              }
              public class RouteParameter
              {
                  const string DefinitionId = "f324c4ea-bac2-450d-b1b1-cc7f09ca3472";
                  string Name { get; }
              }
              public class QueryParameter
              {
                  const string DefinitionId = "5c99275d-be5b-4bc9-849a-a283cdb80b75";
                  string Name { get; }
              }
          }

          // "Return" · 415ffab7-9865-4200-89a0-b592d24919dd
          public class ReturnModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
          {
              // + common element members
          }

          // "User Interface Package" · 911c35b4-4ba3-404c-a0c6-e5258e53333a
          public class UserInterfacePackageModel : IHasStereotypes, IMetadataModel
          {
              // + Id, Name, Stereotypes
              IPackage UnderlyingPackage { get; }
              string FileLocation { get; }
              IList<FolderModel> Folders { get; }
          }
          ```

          """""");
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
