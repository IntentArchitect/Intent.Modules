---
name: add-association-type
description: "Define a brand-new Association Settings type — source/target ends, target types, navigability, traits, mapping options — in the Module Builder designer. USE ONLY WHEN a new kind of association must be drawable between two element types that no existing association type covers. DO NOT USE FOR adding a context-menu extension to an existing foreign element or association (see add-designer-extension). REQUIRES the target element type GUIDs (typeId) for both ends already identified."
argument-hint: "[association name, source element type, target element type]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.AddAssociationType_SkillMd_Agents
contentHash: 7F0741B3C029361C22B4010A0201B198E1169D1067FDEC116FC9622A816B280A
---
# Add Association Type

> [!TIP]
> **Read more if you want to know about** association settings, source/target end configurations, or mapping context menus:
> *   [Workflow & Common Mistakes](./resources/association-type-workflow.md)
> *   [Conceptual Reference](../add-designer-extension/resources/designer-extensions-reference.md)
> *(To conserve tokens, avoid reading these files for simple or minor updates.)*

## Purpose

Create a new `Association Settings` definition in the Module Builder designer, configuring the source and target ends so users can draw the association between the correct element types.

## Musts

1. Identify target element type GUIDs (`typeId`) for both ends before creation.
2. Nest under `Designer Settings` linked to target designer GUID (Domain: `6ab29b31-27af-4f56-a67c-986d82097d63`, Services: `81104ae6-2bc5-4bae-b05a-f987b0372d81`).
3. Apply `Settings` stereotype to both the source and target ends.
4. Configure target types, navigability, and C# API property names for both ends.
5. Apply traits (`[Processing Action]` / `[Processing Handler]`) on target end if needed for mappings.
6. Wire creation option to source element/package extension context menu.
7. Run Software Factory and verify generated C# models compile.
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

1. Never swap source and target ends (source = origin element, target = destination).
2. Never set `Is Navigable Default = true` on source end for directed associations.
3. Never put a `Mapping Option` on the source end; it belongs on the target end.
4. Never edit `.designer.settings` directly (always use the Module Builder model).
