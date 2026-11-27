# Context menus and creation options

A `Context Menu` element (at most one per parent) holds the options a right-click offers. Persisted as `<contextMenuOptions>`.

## Where a `Context Menu` can sit

`Package Settings`, `Package Extension`, `Element Settings`, `Element Extension`, `Association Source End Settings`, `Association Destination End Settings`, the two end extensions, `Diagram Settings` (the diagram's own menu), and under an `Association Creation Option`, `Element Creation Option` or `Run Script Option` (a **sub-menu**).

## Option types

| Option (Module Builder) | Persisted | Creates / does |
| --- | --- | --- |
| `Element Creation Option` | `<createElement>` | A child element of the referenced `Element Settings`. |
| `Association Creation Option` | `<createAssociation>` | An association of the referenced type; the type reference points at the association's target end. |
| `Stereotype-Definition Creation Option` | `<createStereotype>` | A stereotype definition (used by Module Builder and the Domain designer). |
| `Run Script Option` | `<runScript>` | Runs a script (see `functions-scripts-and-events.md`). |
| `Mapping Option` | `<openMapping>` | Opens a mapping dialog. Advanced mapping is out of scope here; a `Mapping Option` carries no `Default Name` or `Allow Multiple`. |

Each option's type reference selects what it creates or opens; this is required (marked `!` in the schema).

## `Option Settings` stereotype

Shared by every option type; which properties exist depends on the option:

| Property | Persisted | Applies to |
| --- | --- | --- |
| `Shortcut`, `Shortcut (macOS)` | `<shortcut>`, `<macShortcut>` | All. macOS defaults to the Windows one with Ctrl → Command. e.g. `ctrl + shift + e`. |
| `Is Option Visible Function` | `<isOptionVisibleFunction>` | All; returns boolean. |
| `Menu Group` | `group` attribute | All. Same number = same group; groups are separated by a divider. |
| `Top Divider`, `Bottom Divider` | `<topDivider>`, `<bottomDivider>` | All; prefer `Menu Group`. |
| `Order` | `order` attribute | Run Script and Mapping. |
| `Icon` | `<icon>` | Run Script and Mapping. |
| `Trigger On Double-Click` | `<triggerOnDoubleClick>` | Run Script and Mapping. |
| `Default Name` | `<defaultName>` | Creation options. Blank = `New` + the type name. |
| `Allow Multiple` | `<allowMultiple>` | Creation options; default true. |
| `Type Order` | `<typeOrder>` | Creation options; position among the child types. |
| `Api Model Name` | | Element creation: C# property name on the generated model. |

Association creation also takes `Source End Defaults` and `Target End Defaults` stereotypes (`Is Navigable`, `Is Nullable`, `Is Collection`; target defaults to navigable).

## Rules

- Without an `Association Creation Option` nobody can draw the association (see `add-designer-extension`).
- Put an `Association Creation Option` in the **source** element's or package's menu, with its type reference set to the association's target end. A `Mapping Option` belongs on the target end, never the source.
- Order inside a menu is the model order; use `Menu Group` to separate related options.
- A creation option adds a menu entry. Which child types an element accepts is the separate `Accepted Child Types` list; see `element-types.md`.
