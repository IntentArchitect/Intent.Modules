---
contentHash: 6D6C18F44EA828EBDE6E0D88F87FCFBC71F1834A9D364F3CA7375C5EC827F919
---
# User Interface Designer — Extension Modules

The modules below are installed separately from the base User Interface designer (`Intent.Modules.Modelers.UI`). Each one adds
its own element types, package/folder extensions or navigation on top of it. Read `user-interface.md`
first for the base designer; this file covers only what the extension modules add. Load it only when
your module actually installs one of them.

Every extension module has its **own** install identity. Never infer one from its parent designer or
from a sibling (`SKILL.md` Must #6).

## 1. Install identities

### 1.1 `.Core`

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.UI.Core` |
| Intent module id | `Intent.Modelers.UI.Core` |
| API namespace | `Intent.Modelers.UI.Core.Api` |

The widget library — every concrete, renderable UI control, all implementing `IComponentModel`
unless noted:

**Field/input:** `AutoCompleteModel` (`ff1ddb80-...`), `ButtonModel` (`4474d808-...`),
`CheckboxModel` (`be9ecdbd-...`), `DatePickerModel` (`9451fcdc-...`), `LinkModel` (`a274918b-...`),
`RadioGroupModel` (`4af9a7f0-...`), `SelectModel` (`78e0bdf7-...`), `TextInputModel` (`4803bf60-...`).

**Display/layout:** `TextModel` (`922150d2-...`), `IconModel` (`3c5f8ea8-...`), `ImageModel`
(`329f635b-...`), `ContainerModel` (`b97ea181-...`).

**Data:** `TableModel` (`eee93c29-...`, has `Columns`), `ColumnModel` (`d372c640-...`, **not**
`IComponentModel` — a column descriptor, not a widget), `FormModel` (`1cfd2d9d-...`).

**Navigation:** `NavigationMenuModel` (`d7282bf2-...`, has `MenuItems`), `MenuItemModel`
(`adbf2fa8-...`, **not** `IComponentModel`; self-recursive via `NavigationItems`).

**Card family** (`CardModel` `dfe420aa-...` + `Header`/`Content`/`Actions` singular optional part
models `CardHeaderModel`/`CardContentModel`/`CardActionsModel` — part models excluded from
`IComponentModel`).

**Dialog family** (`DialogModel` `1260ae89-...`, implements `IComponentModel` but **not**
`IDialogModel` — see `user-interface.md` §4 gap — + `TitleContainer`/`ContentContainer`/`ActionsContainer` part models
`DialogTitleModel`/`DialogContentModel`/`DialogActionsModel`).

A **fourth association**, owned by `.Core` (also directly under `Api/`): `ShowDialogModel`
(`Show Dialog`, `2a309fb2-...`) — target end `IInvokableModel`, exposes `Mappings`.
`ShowDialogSources(this IDialogModel)` is unreachable (see `user-interface.md` §4); `ShowDialogTargets(...)` overloads
on `ComponentOperationModel`/`ComponentModel` do work.

**Stereotype extensions** (one file per widget, `Get*`/`Has*`/`TryGet*` pattern, most also
re-exposing `Secured`): `AutoCompleteModelStereotypeExtensions` (`Interaction`, `LabelAddon`),
`ButtonModelStereotypeExtensions` (`Interaction`: `Type`/`Form`/`OnClick`/`LinkTo`/`Disabled`),
`CheckboxModelStereotypeExtensions`, `DatePickerModelStereotypeExtensions`,
`LinkModelStereotypeExtensions`, `RadioGroupModelStereotypeExtensions`,
`SelectModelStereotypeExtensions` (adds `Options`/`Key`/`Value`/`OnSelected`),
`TextInputModelStereotypeExtensions`, `FormModelStereotypeExtensions` (`OnSubmit`),
`TableModelStereotypeExtensions` (`Interaction`: `OnRowClick`; `Pagination`),
`MenuItemModelStereotypeExtensions` (`Secured` only).

### 1.2 `.ServiceProxies`

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.Modelers.UI.ServiceProxies` |
| Intent module id | `Intent.Modelers.UI.ServiceProxies` |
| API namespace | `Intent.Modelers.UI.ServiceProxies.Api` |
| Depends on | `Intent.Modelers.Services`, `Intent.Modelers.Services.CQRS`, `Intent.Modelers.Types.ServiceProxies` |

Thin by design — a single `UserInterfacePackageExtensionModel : UserInterfacePackageModel` adding
a `ServiceProxies: IList<ServiceProxyModel>` accessor (from
`Intent.Modelers.Types.ServiceProxies.Api`), so `CallServiceOperationActionModel` (`user-interface.md` §5) has
something concrete to target.

## 2. Model API reference

Every public type and member each extension module ships — extracted mechanically from source, not
sampled. **Prefer this to reflecting over the assembly:** if a member is not listed here, it is
not part of the version shown. The stubs follow the conventions defined in
`user-interface.md` §9 — `// + common element members`, the omitted `Is<X>Model()` / `As<X>Model()`
casts, and `// + common association-end members` mean exactly what they mean there.

### 2.1 UI.Core — `Intent.Modules.Modelers.UI.Core` (source at `1.0.4`)

```csharp
namespace Intent.Modelers.UI.Core.Api;

// "AutoComplete" · ff1ddb80-16e4-4e63-ace3-4ef96a03b4b0
public class AutoCompleteModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class AutoCompleteModelStereotypeExtensions
{
    static Interaction GetInteraction(this AutoCompleteModel model);
    static bool HasInteraction(this AutoCompleteModel model);
    static bool TryGetInteraction(this AutoCompleteModel model, out Interaction stereotype);
    static LabelAddon GetLabelAddon(this AutoCompleteModel model);
    static bool HasLabelAddon(this AutoCompleteModel model);
    static bool TryGetLabelAddon(this AutoCompleteModel model, out LabelAddon stereotype);
    static Secured GetSecured(this AutoCompleteModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this AutoCompleteModel model);
    static bool HasSecured(this AutoCompleteModel model);
    static bool TryGetSecured(this AutoCompleteModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "44dcdc3d-3445-435c-9013-197ab9836f09";
        string Name { get; }
        string OnSelected();
        string SearchFunction();
    }
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Button" · 4474d808-f6fd-405f-a9c7-6998cd000ede
public class ButtonModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
}

public static class ButtonModelStereotypeExtensions
{
    static Interaction GetInteraction(this ButtonModel model);
    static bool HasInteraction(this ButtonModel model);
    static bool TryGetInteraction(this ButtonModel model, out Interaction stereotype);
    static Secured GetSecured(this ButtonModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this ButtonModel model);
    static bool HasSecured(this ButtonModel model);
    static bool TryGetSecured(this ButtonModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "a7de29e5-4bee-4e5d-93f0-740569ac6130";
        string Name { get; }
        TypeOptions Type();
        IElement Form();
        string OnClick();
        string LinkTo();
        string Disabled();
        public class TypeOptions
        {
            TypeOptionsEnum AsEnum();
            bool IsDefault();
            bool IsSubmit();
        }
        public enum TypeOptionsEnum { Default, Submit }
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Card Actions" · 8dd9e894-be78-41ec-89f2-d2bd2d26d8d6
public class CardActionsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Card Content" · 57319385-c7e0-43eb-a2d8-b7cb1ddce2e2
public class CardContentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Card Header" · 61bd2c09-fb56-4f0a-9d45-7a04c649a965
public class CardHeaderModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Card" · dfe420aa-426a-4517-bcd1-83cf5ec074fe
public class CardModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    CardHeaderModel Header { get; }
    CardContentModel Content { get; }
    CardActionsModel Actions { get; }
}

// "Checkbox" · be9ecdbd-6ded-4057-af76-1b09f3a6cea3
public class CheckboxModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class CheckboxModelStereotypeExtensions
{
    static Interaction GetInteraction(this CheckboxModel model);
    static bool HasInteraction(this CheckboxModel model);
    static bool TryGetInteraction(this CheckboxModel model, out Interaction stereotype);
    static LabelAddon GetLabelAddon(this CheckboxModel model);
    static bool HasLabelAddon(this CheckboxModel model);
    static bool TryGetLabelAddon(this CheckboxModel model, out LabelAddon stereotype);
    static Secured GetSecured(this CheckboxModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this CheckboxModel model);
    static bool HasSecured(this CheckboxModel model);
    static bool TryGetSecured(this CheckboxModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "6e04ff81-f043-4ac6-8632-798aedbaaf20";
        string Name { get; }
        string OnValueChanged();
        bool IsRequired();
        bool IsEnabled();
    }
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Column" · d372c640-e649-4e67-84a8-4446f45288db
public class ColumnModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

public static class ComponentModelStereotypeExtensions
{
    static Secured GetSecured(this ComponentModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this ComponentModel model);
    static bool HasSecured(this ComponentModel model);
    static bool TryGetSecured(this ComponentModel model, out Secured stereotype);
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Container" · b97ea181-2462-431f-8f62-f90f79509b9e
public class ContainerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
}

// "Date Picker" · 9451fcdc-9406-4323-b354-2eaaabbf9ac5
public class DatePickerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
}

public static class DatePickerModelStereotypeExtensions
{
    static LabelAddon GetLabelAddon(this DatePickerModel model);
    static bool HasLabelAddon(this DatePickerModel model);
    static bool TryGetLabelAddon(this DatePickerModel model, out LabelAddon stereotype);
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
}

// "Dialog Actions" · 247a4768-7c46-472c-b5ea-565b37194956
public class DialogActionsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Dialog Content" · 69495016-6e72-4c18-bae5-2391e45cd234
public class DialogContentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Dialog" · 1260ae89-b0cc-4ae0-b21c-7e26d45f16a5
public class DialogModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    DialogTitleModel TitleContainer { get; }
    DialogContentModel ContentContainer { get; }
    DialogActionsModel ActionsContainer { get; }
}

// "Dialog Title" · c3a01bf4-20a8-4761-b62b-527d3005baff
public class DialogTitleModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
}

// "Form" · 1cfd2d9d-1061-4c45-8b4e-074cfa8dacfd
public class FormModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class FormModelStereotypeExtensions
{
    static Interaction GetInteraction(this FormModel model);
    static bool HasInteraction(this FormModel model);
    static bool TryGetInteraction(this FormModel model, out Interaction stereotype);
    public class Interaction
    {
        const string DefinitionId = "e242cccf-5826-478f-9e7a-c1fd9df4e5c8";
        string Name { get; }
        string OnSubmit();
    }
}

// "Icon" · 3c5f8ea8-061a-4e71-826f-59f9281abe71
public class IconModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

// "Image" · 329f635b-a55c-465f-b966-fe8411a5f57c
public class ImageModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

// "Link" · a274918b-72d1-4913-95de-0a801ac90ce4
public class LinkModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class LinkModelStereotypeExtensions
{
    static Interaction GetInteraction(this LinkModel model);
    static bool HasInteraction(this LinkModel model);
    static bool TryGetInteraction(this LinkModel model, out Interaction stereotype);
    static Secured GetSecured(this LinkModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this LinkModel model);
    static bool HasSecured(this LinkModel model);
    static bool TryGetSecured(this LinkModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "5d0a10f7-ef5c-4f1e-af13-3471671b46a7";
        string Name { get; }
        string OnClick();
        string LinkTo();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Menu Item" · adbf2fa8-6833-4c24-960a-31d8a41fd1ed
public class MenuItemModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
{
    // + common element members
    string Value { get; }
    IList<MenuItemModel> NavigationItems { get; }
}

public static class MenuItemModelStereotypeExtensions
{
    static Secured GetSecured(this MenuItemModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this MenuItemModel model);
    static bool HasSecured(this MenuItemModel model);
    static bool TryGetSecured(this MenuItemModel model, out Secured stereotype);
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Navigation Menu" · d7282bf2-1626-4b8b-9446-1d530527db06
public class NavigationMenuModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    IList<MenuItemModel> MenuItems { get; }
}

// "Radio Group" · 4af9a7f0-ca24-4681-8e32-2a8d3dacb6ed
public class RadioGroupModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class RadioGroupModelStereotypeExtensions
{
    static DisplayOptions GetDisplayOptions(this RadioGroupModel model);
    static bool HasDisplayOptions(this RadioGroupModel model);
    static bool TryGetDisplayOptions(this RadioGroupModel model, out DisplayOptions stereotype);
    static Interaction GetInteraction(this RadioGroupModel model);
    static bool HasInteraction(this RadioGroupModel model);
    static bool TryGetInteraction(this RadioGroupModel model, out Interaction stereotype);
    static LabelAddon GetLabelAddon(this RadioGroupModel model);
    static bool HasLabelAddon(this RadioGroupModel model);
    static bool TryGetLabelAddon(this RadioGroupModel model, out LabelAddon stereotype);
    static Secured GetSecured(this RadioGroupModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this RadioGroupModel model);
    static bool HasSecured(this RadioGroupModel model);
    static bool TryGetSecured(this RadioGroupModel model, out Secured stereotype);
    public class DisplayOptions
    {
        const string DefinitionId = "0773bfc9-e976-4549-aa31-47713105614d";
        string Name { get; }
        AlignmentOptions Alignment();
        public class AlignmentOptions
        {
            AlignmentOptionsEnum AsEnum();
            bool IsHorizontal();
            bool IsVertical();
        }
        public enum AlignmentOptionsEnum { Horizontal, Vertical }
    }
    public class Interaction
    {
        const string DefinitionId = "6a75e4ef-66bd-45b3-b74a-2d75b384c8cd";
        string Name { get; }
        string Options();
        string Key();
        string Value();
        string OnSelected();
    }
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Select" · 78e0bdf7-424f-4820-95d5-b756b0ebc87a
public class SelectModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class SelectModelStereotypeExtensions
{
    static Interaction GetInteraction(this SelectModel model);
    static bool HasInteraction(this SelectModel model);
    static bool TryGetInteraction(this SelectModel model, out Interaction stereotype);
    static LabelAddon GetLabelAddon(this SelectModel model);
    static bool HasLabelAddon(this SelectModel model);
    static bool TryGetLabelAddon(this SelectModel model, out LabelAddon stereotype);
    static Secured GetSecured(this SelectModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this SelectModel model);
    static bool HasSecured(this SelectModel model);
    static bool TryGetSecured(this SelectModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "6a75e4ef-66bd-45b3-b74a-2d75b384c8cd";
        string Name { get; }
        string Options();
        string Key();
        string Value();
        string OnSelected();
    }
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Show Dialog" · 2a309fb2-c487-4c00-9462-16dac9824731
public class ShowDialogModel : IMetadataModel
{
    static ShowDialogModel CreateFromEnd(IAssociationEnd associationEnd);
    string Id { get; }
    ShowDialogSourceEndModel SourceEnd { get; }
    ShowDialogTargetEndModel TargetEnd { get; }
    IAssociation InternalAssociation { get; }
}

// "Show Dialog Source End" · ad3de626-cb72-4dde-966b-122a6b386fb7
public class ShowDialogSourceEndModel : ShowDialogEndModel { /* common association-end members */ }

// "Show Dialog Target End" · c44a7969-abfa-4073-ab2c-d2d0f1f6bd2f
public class ShowDialogTargetEndModel : ShowDialogEndModel, IInvokableModel
{
    // + common association-end members
    IEnumerable<IElementToElementMapping> Mappings { get; }
}

public class ShowDialogEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper { /* common association-end members */ }

public static class ShowDialogModelAssociationExtensions
{
    static IList<ShowDialogSourceEndModel> ShowDialogSources(this IDialogModel model);
    static IList<ShowDialogTargetEndModel> ShowDialogTargets(this ComponentOperationModel model);
    static IList<ShowDialogTargetEndModel> ShowDialogTargets(this ComponentModel model);
}

// "Table" · eee93c29-3d58-4e42-aea9-c00451d17469
public class TableModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
    IList<ColumnModel> Columns { get; }
}

public static class TableModelStereotypeExtensions
{
    static Interaction GetInteraction(this TableModel model);
    static bool HasInteraction(this TableModel model);
    static bool TryGetInteraction(this TableModel model, out Interaction stereotype);
    static Pagination GetPagination(this TableModel model);
    static bool HasPagination(this TableModel model);
    static bool TryGetPagination(this TableModel model, out Pagination stereotype);
    static Secured GetSecured(this TableModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this TableModel model);
    static bool HasSecured(this TableModel model);
    static bool TryGetSecured(this TableModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "74533b82-2078-44e1-84bb-4cd34d00ef16";
        string Name { get; }
        string OnRowClick();
    }
    public class Pagination
    {
        const string DefinitionId = "f0663cd4-da7c-43cc-9cda-bbc7923d5431";
        string Name { get; }
        string CurrentPage();
        string PagesCount();
        string OnPageChanged();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Text Input" · 4803bf60-c626-4cbe-94ed-0f1eafff9fe3
public class TextInputModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}

public static class TextInputModelStereotypeExtensions
{
    static Interaction GetInteraction(this TextInputModel model);
    static bool HasInteraction(this TextInputModel model);
    static bool TryGetInteraction(this TextInputModel model, out Interaction stereotype);
    static LabelAddon GetLabelAddon(this TextInputModel model);
    static bool HasLabelAddon(this TextInputModel model);
    static bool TryGetLabelAddon(this TextInputModel model, out LabelAddon stereotype);
    static Secured GetSecured(this TextInputModel model);
    static IReadOnlyCollection<Secured> GetSecureds(this TextInputModel model);
    static bool HasSecured(this TextInputModel model);
    static bool TryGetSecured(this TextInputModel model, out Secured stereotype);
    public class Interaction
    {
        const string DefinitionId = "6e04ff81-f043-4ac6-8632-798aedbaaf20";
        string Name { get; }
        string OnValueChanged();
        bool IsRequired();
        bool IsEnabled();
    }
    public class LabelAddon
    {
        const string DefinitionId = "2c099977-e5ca-4a80-ba70-6f2edc593681";
        string Name { get; }
        string Label();
    }
    public class Secured
    {
        const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        string Name { get; }
        string Roles();
        string Policy();
    }
}

// "Text" · 922150d2-42e6-4805-a002-f9580cdf7f6f
public class TextModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
{
    // + common element members
    string Value { get; }
}
```

### 2.2 UI.ServiceProxies — `Intent.Modules.Modelers.UI.ServiceProxies` (source at `1.0.3`)

```csharp
namespace Intent.Modelers.UI.ServiceProxies.Api;

public class UserInterfacePackageExtensionModel : UserInterfacePackageModel
{
    // + everything inherited from UserInterfacePackageModel
    IList<ServiceProxyModel> ServiceProxies { get; }
}
```

## 3. Name collisions and gaps

- **`GetSecured(this ComponentModel)` is declared twice** — in `Intent.Modelers.UI.Api`'s `ComponentModelStereotypeExtensions` and again in `Intent.Modelers.UI.Core.Api`'s class of the same name, each with its own nested `Secured` type. With both namespaces imported, `component.GetSecured()` does not compile (ambiguous call). Import only one namespace in that file, or call the static class explicitly.
- **`.Core` ships no `Get…Models()` provider.** Its elements (`ButtonModel`, `FormModel`, `TableModel`, …) live inside a component's view. Reach them by walking `ComponentModel.InternalElement.ChildElements` recursively and converting each element with its `As<X>Model()` extension.
- Nested stereotype types are redeclared per element class. `Secured` and `LabelAddon` carry the same `DefinitionId` on every element, but each is still a distinct C# type. `Interaction` differs per element in both `DefinitionId` and properties — read the one under the element you hold.
- `ServiceProxyModel` (used by `.ServiceProxies`) is declared in `Intent.Modelers.Types.ServiceProxies.Api`, a module outside this reference.
