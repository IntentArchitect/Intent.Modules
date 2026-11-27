# Element types

An element type is an `Element Settings` under `Designer Settings` (top level) or under another `Element Settings` (a child type that only exists inside its parent). Persisted as `<elementSetting type typeId>`; children as `<childElementSetting>`.

## `Settings` stereotype (Element Settings and Core Type)

| Designer property | Persisted | Meaning |
| --- | --- | --- |
| `Save Mode` | `<saveAsOwnFile>` | `Default`, `Own File`, `As Child`. Persisted as a boolean, so only own-file vs not survives in the XML. |
| `Icon`, `Expanded Icon` | `<icon>`, `<expandedIcon>` | |
| `Icon Function` | `<iconFunction>` | Per-element icon; `null` falls back to the default. |
| `Display Text Function` | `<displayFunction>` | Returns a string or `[{ text, cssClass?, targetId?, color?, markdown? }]`. |
| `Validate Function` | `<validateFunction>` | Returns a string or `{ text, type?: "error"\|"warning" }`; several can accumulate through extensions. |
| `Allow Rename` | `<allowRename>` | Default true. |
| `Name Must Be Unique` | `<nameMustBeUnique>` | Default true. |
| `Allow Abstract`, `Allow Static`, `Allow Generic Types`, `Allow Set Value` | `<allowAbstract>` … | Toggle those fields in the properties pane. |
| `Allow Sorting`, `Sort Children` | `<allowSorting>`, `<sortChildren>` | `Sort Children` default is by type, then name. |
| `Allow Find in View` | `<allowFindInView>` | |

## `Type Reference Settings` stereotype

Whether the element points at another type (like an attribute's type).

| Designer property | Persisted (`<typeReferenceSetting>`) |
| --- | --- |
| `Mode`: `Disabled`, `Optional`, `Required` | `<allowTypeReference>`, `<isRequired>` |
| `Display Name`, `Hint` | `<displayName>`, `<hint>` (hint supports markdown) |
| `Target Types`, `Target Traits` | `<targetTypes>` of `<type typeId>` |
| `Represents`: `Reference`, `Inheritance` | |
| `Default Type Id` | `<defaultTypeId>` |
| `Allow Nullable`, `Allow Collection` | `<allowIsNullable>`, `<allowIsCollection>` |

## Children and what an element accepts

| Model element | Effect |
| --- | --- |
| Child `Element Settings` | A type that lives only inside this one. |
| `Accepted Child Types` → `Accepted Type` (type reference) | The types this element accepts and their order (`<typeOrder>`). `Accepted Traits` (`Types with Stereotype`) accepts any type that carries a trait stereotype. |
| `Context Menu` | The creation entries a user sees; see `context-menus.md`. |
| `Element Extension` | Extends another type's behaviour for a child element. |
| `Diagram Settings` | How it appears on a diagram; see `diagram-visuals.md`. |
| `Element Visual Settings` | Visual settings used when it is itself a diagram element. |
| `Element Event Settings` → `Element Event Handler` | Reacts to create, rename and so on; see `functions-scripts-and-events.md`. |
| `Mapping Projection Settings` | Advanced mapping; out of scope for this skill. |

## `Element Extension`

Adds behaviour to an element type the module does not own. Stereotypes: `Extension Settings` (`Display Text Function`, `Validate Function`, persisted as overrides) and `Type Reference Extension Settings` (mandatory; `Mode` of `Inherit` keeps the original; others as above, with `Allow Collection` / `Allow Nullable` of `Inherit`, `Allow`, `Disallow`). Full workflow: `add-designer-extension`.

## AI rules

`AI Settings` → `Rules` on `Element Settings`, `Element Extension`: plain prose telling an agent what to do and when; persisted `<rules>`. See `add-designer-extension`.
