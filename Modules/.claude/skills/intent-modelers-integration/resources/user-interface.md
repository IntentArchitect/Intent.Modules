---
contentHash: 9895D7A26BC7D8A98C0DBE4BACA1B7DB4412CF489223D74D035BD4C0243DA22F
---
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

- *What it is not:** it is not a widget library — the base module defines no concrete widgets

(no Button, Form, Table); those are added by the `.Core` extension module (§8). It is not a
domain/service designer — it has its own `Model Definition` (a lightweight DTO-shaped view model)
rather than reusing Domain's `ClassModel`, and its own `Call Service Operation Action` association
rather than reusing Services' invocation model. The base module's own `.csproj` deliberately
excludes a `PackageReference` to `.Core` (commented out, with the comment "we don't want this
dependency — that module is the base for Component modules") — the dependency direction is
`.Core` → base, never the reverse.

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

- *Practical guidance:** only `IComponentModel` and `IInvokableModel` are safe to code against. For

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

## 8. Extension modules

### `.Core`

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.UI.Core` |
| Intent module id | `Intent.Modelers.UI.Core` |
| API namespace | `Intent.Modelers.UI.Core.Api` |

The widget library — every concrete, renderable UI control, all implementing `IComponentModel`
unless noted:

- *Field/input:** `AutoCompleteModel` (`ff1ddb80-...`), `ButtonModel` (`4474d808-...`),

`CheckboxModel` (`be9ecdbd-...`), `DatePickerModel` (`9451fcdc-...`), `LinkModel` (`a274918b-...`),
`RadioGroupModel` (`4af9a7f0-...`), `SelectModel` (`78e0bdf7-...`), `TextInputModel` (`4803bf60-...`).

- *Display/layout:** `TextModel` (`922150d2-...`), `IconModel` (`3c5f8ea8-...`), `ImageModel`

(`329f635b-...`), `ContainerModel` (`b97ea181-...`).

- *Data:** `TableModel` (`eee93c29-...`, has `Columns`), `ColumnModel` (`d372c640-...`, **not**

`IComponentModel` — a column descriptor, not a widget), `FormModel` (`1cfd2d9d-...`).

- *Navigation:** `NavigationMenuModel` (`d7282bf2-...`, has `MenuItems`), `MenuItemModel`

(`adbf2fa8-...`, **not** `IComponentModel`; self-recursive via `NavigationItems`).

- *Card family** (`CardModel` `dfe420aa-...` + `Header`/`Content`/`Actions` singular optional part

models `CardHeaderModel`/`CardContentModel`/`CardActionsModel` — part models excluded from
`IComponentModel`).

- *Dialog family** (`DialogModel` `1260ae89-...`, implements `IComponentModel` but **not**

`IDialogModel` — see §4 gap — + `TitleContainer`/`ContentContainer`/`ActionsContainer` part models
`DialogTitleModel`/`DialogContentModel`/`DialogActionsModel`).

A **fourth association**, owned by `.Core` (also directly under `Api/`): `ShowDialogModel`
(`Show Dialog`, `2a309fb2-...`) — target end `IInvokableModel`, exposes `Mappings`.
`ShowDialogSources(this IDialogModel)` is unreachable (see §4); `ShowDialogTargets(...)` overloads
on `ComponentOperationModel`/`ComponentModel` do work.

- *Stereotype extensions** (one file per widget, `Get*`/`Has*`/`TryGet*` pattern, most also

re-exposing `Secured`): `AutoCompleteModelStereotypeExtensions` (`Interaction`, `LabelAddon`),
`ButtonModelStereotypeExtensions` (`Interaction`: `Type`/`Form`/`OnClick`/`LinkTo`/`Disabled`),
`CheckboxModelStereotypeExtensions`, `DatePickerModelStereotypeExtensions`,
`LinkModelStereotypeExtensions`, `RadioGroupModelStereotypeExtensions`,
`SelectModelStereotypeExtensions` (adds `Options`/`Key`/`Value`/`OnSelected`),
`TextInputModelStereotypeExtensions`, `FormModelStereotypeExtensions` (`OnSubmit`),
`TableModelStereotypeExtensions` (`Interaction`: `OnRowClick`; `Pagination`),
`MenuItemModelStereotypeExtensions` (`Secured` only).

### `.ServiceProxies`

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.UI.ServiceProxies` |
| Intent module id | `Intent.Modelers.UI.ServiceProxies` |
| API namespace | `Intent.Modelers.UI.ServiceProxies.Api` |
| Depends on | `Intent.Modelers.Services`, `Intent.Modelers.Services.CQRS`, `Intent.Modelers.Types.ServiceProxies` |

Thin by design — a single `UserInterfacePackageExtensionModel : UserInterfacePackageModel` adding
a `ServiceProxies: IList<ServiceProxyModel>` accessor (from
`Intent.Modelers.Types.ServiceProxies.Api`), so `CallServiceOperationActionModel` (§5) has
something concrete to target.

## 9. Worked snippet

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
