---
name: add-designer-extension
description: "Add a context-menu-driven element, association-creation, or mapping option to a foreign package, element, or association end via a packageExtension/elementExtension. USE ONLY WHEN extending an element/package/association end owned by another module, not one you own. DO NOT USE FOR defining a brand-new association type from scratch (see add-association-type) or building an architecture template's component picker (see architecture-templates). REQUIRES the foreign target's typeId and designer GUID already identified."
argument-hint: "[target element/package type name and what to add]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.AddDesignerExtension_SkillMd_Agents
contentHash: 2B12A3987DCFAAFBF0D1D6323918FED4A5FA7AA3EE1B49C2DDA230BBCB296848
---
# Add Designer Extension

> [!TIP]
> **Read more if you want to know about** designer extensions, package/element extensions, mapping options, or context menu items:
> *   [Conceptual Reference](./resources/designer-extensions-reference.md)
> *   [Workflow & Common Mistakes](./resources/designer-extension-workflow.md)
> *(To conserve tokens, avoid reading these files for simple or minor updates.)*

## Purpose

Extend an existing element, package, or association end from a foreign module by adding new context menu options (e.g. child element/association creation or mapping options).

## Musts

1. Identify foreign target `typeId` via MCP `find_designer_elements` or generated C# `SpecializationTypeId` constants.
2. Link `Designer Settings` to foreign designer GUID (Domain: `6ab29b31-27af-4f56-a67c-986d82097d63`, Services: `81104ae6-2bc5-4bae-b05a-f987b0372d81`).
3. For Element Extensions, apply `Type Reference Extension Settings` (`Mode = Inherit`) and `Extension Settings` stereotypes.
4. Add all context menu options under a `[context menu]` child element.
5. Set type reference of `Association Creation Option` to target end of association settings.
6. Verify via `get_designer_validation_errors` and generated `.designer.settings` file.
- *Package settings are the companion step to linking `Designer Settings`.** Linking the extension to a

foreign designer says *what* you are extending; two properties on the owning package's `Module Settings`
decide whether any of it actually ships, and where it lands:

| When the package gains… | Set on that package's `Module Settings` |
|---|---|
| a **stereotype** | `Include in Module` = true — without it the package produces no `<install>` entry and the stereotype ships nowhere |
| a **new or extension designer element** | `Reference in Designer` = the target designer(s), or `Reference in` = *All Designers* — this drives the `<install target=…>` so the element lands in the right designer |

Set them as you add the element, not in a later sweep. The Software Factory warns when a package has
stereotype definitions and is not included — this gets ahead of that warning rather than waiting for it.

If the new element type should carry its own icon, that is optional craft rather than an obligation — see
`module-element-icons`.

## Must Nots

1. Never guess or hardcode a type GUID from memory.
2. Never skip `Mode = Inherit` on `Type Reference Extension Settings` for element extensions.
3. Never edit `.designer.settings` directly (always change the Module Builder model).
