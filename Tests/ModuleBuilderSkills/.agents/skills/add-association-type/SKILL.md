---
name: add-association-type
description: "Define a new association type (source and target ends, target types, display functions, navigability, traits, mapping options) in the Module Builder designer."
argument-hint: "[association name, source element type, target element type]"
contentHash: D2301A2FD0FF0C3E2C0E6B2B05E53DC8147D9446DA1CF6166BF7ADECF7EB05FF
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

## Must Nots

1. Never swap source and target ends (source = origin element, target = destination).
2. Never set `Is Navigable Default = true` on source end for directed associations.
3. Never put a `Mapping Option` on the source end; it belongs on the target end.
4. Never edit `.designer.settings` directly (always use the Module Builder model).
