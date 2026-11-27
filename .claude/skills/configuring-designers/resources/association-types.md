# Association types

Workflow, ordering rules and common mistakes: `add-designer-extension` → `resources/association-type-workflow.md`. This file lists what each model element carries.

## Model

| Element | Persisted | Notes |
| --- | --- | --- |
| `Association Settings` | `<associationSetting type typeId>` | Under `Designer Settings`. Children: one `Association Source End Settings`, one `Association Destination End Settings`, optional `Association Visual Settings`, `Association Event Settings`. |
| `Association Source End Settings` / `Association Destination End Settings` | `<sourceEnd>` / `<targetEnd>` | The origin and destination ends. |
| `Association Extension` | `<associationExtension>` | Adds to the ends of an association another module owns, through `Association Source End Extension` / `Association Target End Extension`. |

## End `Settings` stereotype

| Designer property | Persisted | Meaning |
| --- | --- | --- |
| `Icon` | `<icon>` | |
| `Target Types`, `Target Traits` | `<typeReferenceSetting>/<targetTypes>` | The types this end's reference points at: on the source end the element types the association starts from, on the destination end the types it can point to. Swapping them is the common mistake; see `association-type-workflow.md`. |
| `Api Property Name` | | C# property name generated on the model. |
| `Display Text Function` | `<displayFunction>` | |
| `Validate Function` | `<validateFunction>` | |
| `Name Accessibility` | `<nameAccessibilityMode>` | `Optional`, `Required`, `Disabled`, `Hidden`; persisted lower-case (`optional`, `required`, `disabled`, `hidden`). |
| `Default Name Function`, `Name Must Be Unique` | `<defaultNameFunction>`, `<nameMustBeUnique>` | |
| `Is Navigable/Nullable/Collection Enabled` | `<allowIsNavigable>` … | Whether the user can change it. |
| `Is Navigable/Nullable/Collection Default` | `<isNavigableDefault>` … | Initial value. |
| `Allow Multiple` | | More than one of this association between two elements. |
| `Allow Set Value`, `Allow Sorting`, `Sort Children` | | As for element types. |

`Association End Extension Settings` (on an end extension): `Display Name`, `Hint`, `Target Types`, `Target Traits`, `Default Type Id`, and `Allow Navigable/Nullable/Collection` of `Inherit`, `Allow`, `Disallow`.

## Around the association

- **Context menu on an end**: `Association Source End Settings` / `Destination End Settings` / end extensions carry a `Context Menu`; see `context-menus.md`. An `Association Creation Option` in the **source** element's or package's menu is what lets users draw the association.
- **Appearance on a diagram**: `Association Visual Settings`, see `diagram-visuals.md`.
- **Events**: `Association Event Settings` → `Association Event Handler`, see `functions-scripts-and-events.md`.
- **Traits**: stereotype traits on the target end identify its role to other modules (`[Processing Action]` etc.).
