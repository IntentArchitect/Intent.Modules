# Designer and package configuration

## Model elements

| Element (Module Builder) | Where it lives | Purpose |
| --- | --- | --- |
| `Designer` | `Designers Folder` | A new designer surface. `Designer Config` stereotype: `Icon`, `Display Order`. `Output Configuration`: `Package Type`, `Role Type`, `Template Output Type`, `Folder Type` (which element types play those roles for template output). |
| `Designer Settings` | `Designers Folder` | The settings file for one designer. `Designer Settings` stereotype: `Is Reference`, `Extend Designers` (the designer(s) this file adds to). Holds every element, association, package, script and suggestion setting below it. |
| `Designer Reference` | Under `Designer` or `Designer Settings` | Makes another designer's types available here. |
| `Package Settings` | Under `Designer Settings` | A package type users create in this designer. |
| `Package Extension` | Under `Designer Settings` | Adds to a package type another module owns. |
| `Core Type` | Under `Designer Settings` | A built-in type element; carries the same `Settings` stereotype as an element type. |

A designer's own settings file and an extension of a foreign designer are both `Designer Settings`; the `Extend Designers` list is what makes the second kind apply. Extending a foreign designer is covered by `add-designer-extension`.

## Package Settings

Stereotype `Package Settings`:

| Designer property | Persisted | Notes |
| --- | --- | --- |
| `Default Name Function` | `<defaultName>` | JavaScript; name given to a new package. |
| `Icon` | `<icon>` | |
| `Sorting` | `<sortChildren>` | `Manually`, `By type, then manually`, `By type, then by name` (default), `By name`. Persisted `manually`, `by-type-then-manually`, `by-type-then-by-name`, `by-name`. |

Children: `Context Menu` (what the package can create, see `context-menus.md`), `Element Extension` (add to an element type under the package), `Package Event Settings` (handlers, see `functions-scripts-and-events.md`). Also persisted but not modelled as a property: `<requiredPackages>` (a list of package ids that must be present).

`Package Extension` takes the same children and adds them to a package type owned elsewhere. `AI Settings` → `Rules` on a package or package extension is **dropped**, so put rules on element types instead.

## Shipping it: Module Settings on the module package

The `Intent Module` package carries the `Module Settings` stereotype. These decide whether and where the configuration lands in consumers:

| Property | Effect |
| --- | --- |
| `Include in Module` | Required for the package's stereotypes to ship. |
| `Reference in` | `All Designers` or `Selected Designers`. |
| `Reference in Designer` | The designers that receive the package's elements (drives `<install target=…>`). |
| `Version`, `Include Release Notes` | Versioning; see **module-versioning**. |

Set `Reference in Designer` as you add a new designer element, not in a later sweep.

## Documentation topics

`Module Documentation` → `Documentation Topic` (`Settings` stereotype: `Description`, `Icon`, `Href`, `Designers`, `Elements`, `Tags`) attaches help links to a designer or to specific element types. Use it to point users from an element type to docs.
