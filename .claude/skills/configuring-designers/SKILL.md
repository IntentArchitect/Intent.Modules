---
description: "Module Builder: configuring a designer in a module. The entry point for everything a module can set on a designer: the designer and its packages, element types, association types, context menus, diagram visuals (shapes, anchors, lines), scripts and event handlers, suggestions and AI rules. Use first whenever a module changes how a designer looks, what it offers, or how it behaves; it routes to the resource that holds the detail. Not for advanced mapping, templates, or module settings."
short-description: "Entry point for a module's designer: element and association types, menus, diagram visuals and anchors, scripts, suggestions."
argument-hint: "[designer, element type or association and what should change]"
intent-updates: automatic
requiredTools:
  - read_file
  - get_designer_rules
  - run_designer_query
  - run_designer_script
  - run_software_factory
group: Module building
---
# Configuring Designers

> [!TIP]
> **`read_file` the resource for the capability you need** from this skill's directory (`use_skill` reports it). Read only the one that matches; they are independent.
> *   Applying any of these with `run_designer_script`: which stereotype goes on which element, a recipe, verifying (`resources/scripting-the-module-builder.md`)
> *   Designer, packages, module package settings (`resources/designer-and-packages.md`)
> *   Element types (`resources/element-types.md`)
> *   Association types (`resources/association-types.md`)
> *   Context menus and creation options (`resources/context-menus.md`)
> *   Diagram visuals: shapes, text, icons, lines, end markers (`resources/diagram-visuals.md`)
> *   Diagram visual recipes: proven card, container, note, flow-node and line looks per diagram type (`resources/diagram-visual-recipes.md`)
> *   Diagram anchors: where lines attach, even spreading, non-rectangular shapes (`resources/diagram-anchors.md`)
> *   Functions, scripts, event handlers, suggestions (`resources/functions-scripts-and-events.md`)

## Purpose

A module configures a designer in one place: the Module Builder model. This skill tells you which model element carries each capability and where the detail lives. It holds no settings detail itself.

## Which resource

| You want to… | Read | Related skill |
| --- | --- | --- |
| Make any change below in the model: which stereotype goes on which element, property values, a script recipe, verifying | `scripting-the-module-builder.md` | `scripting-the-model` |
| Create or reference a designer, set a package type's name, icon or sorting, ship a package or stereotype to a designer | `designer-and-packages.md` | `add-designer-extension` |
| Add a new element type, set its icon, label, validation, rename/sort rules, type reference, or which children it accepts | `element-types.md` | |
| Add or change a kind of association, its two ends, or multiplicity defaults | `association-types.md` | `add-designer-extension` |
| Add "New X", "New association", run-a-script or open-mapping entries to a right-click menu, with shortcuts, groups and visibility | `context-menus.md` | `add-designer-extension` |
| Change how an element or association is drawn on a diagram: shape, fill, text, icon, line style, arrow heads, labels | `diagram-visuals.md` | |
| Make a diagram look good for a use case: entity model, flow, screen map, services; ready palette, shapes, lines | `diagram-visual-recipes.md` | |
| Change where associations meet an element, spread lines that share a side, attach along a diamond or hexagon | `diagram-anchors.md` | |
| Write the JavaScript behind a setting, react to create/rename/change/delete, offer suggestions | `functions-scripts-and-events.md` | |
| Give an AI agent guidance about an element type | see **Attaching AI Rules** in `add-designer-extension` | `add-designer-extension` |

Out of scope here: advanced mapping (`Mapping Settings`, `Mappable Elements`), code templates, module settings fields (`Module Settings Configuration`) and stereotype definitions. The `Module Settings` stereotype that ships a package is covered in `designer-and-packages.md`.

## Musts

1. **The Module Builder model is the source of truth.** The `modelers/*.designer.settings` file in a module is generated from it by the Software Factory; edit the model, regenerate, and read the file only to verify. Never hand-edit it. If the module has no Module Builder model (`get_applications` lists no *Module Builder* designer for its application), tell the user and ask before editing the file.
2. Load **scripting-the-model** and call `get_designer_rules` before the first `run_designer_query` / `run_designer_script`, then read `scripting-the-module-builder.md`. You set the Module Builder labels (`Display Text Function`), never the persisted XML names (`displayFunction`); confirm each label against `getStereotypeDefinitions()`.
3. Find a foreign `typeId` with `run_designer_query`, never from memory (see `add-designer-extension` Must #1).
4. A capability that is persisted in `.designer.settings` and honoured by the client may still have **no Module Builder field yet**. Check the schema before promising it; if the field is missing, do not edit generated output: tell the user the module's Module Builder needs updating and ask them to do that first. `diagram-anchors.md` (older Module Builders lack `Anchor Segments`) is an example.
5. Verify in the model with `getDesignerValidationErrors()`, run the Software Factory, then read the generated `.designer.settings` for the element you changed.
6. A settings change needs a module version bump and a release note. Do not invent either: tell the user, and apply them with **module-versioning** / **module-docs** only when asked or given the version.

## Where a setting lives

| Module Builder element | Persisted as (`.designer.settings`) |
| --- | --- |
| `Designer Settings` | root `<settings>` |
| `Package Settings` / `Package Extension` | `<packageSetting>` / `<packageExtension>` |
| `Element Settings` / `Element Extension` | `<elementSetting>` / `<elementExtension>`; nested settings under `<childElementSettings>`, nested extensions under `<childElementExtensions>` |
| `Association Settings` / `Association Extension` | `<associationSetting>` / `<associationExtension>` |
| `Context Menu` | `<contextMenuOptions>` |
| `Diagram Settings` | `<diagramSettings>` |
| `Element Visual Settings` | `<visualSettings type typeId>` (child rows: `<childElementVisualSettings>`) |
| `Script`, `Suggestions Settings` | `<scripts>`, `<suggestionSettings>` |
