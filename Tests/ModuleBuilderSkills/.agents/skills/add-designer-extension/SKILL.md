---
name: add-designer-extension
description: "Add a context menu item, new element, or association creation option to a foreign package, element, or association end. Covers packageExtension and elementExtension patterns."
argument-hint: "[target element/package type name and what to add]"
contentHash: AC568EB4FAB87338A415111B98BF6FBD18850A96FDF41F3C78A127963D4863E3
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

## Must Nots

1. Never guess or hardcode a type GUID from memory.
2. Never skip `Mode = Inherit` on `Type Reference Extension Settings` for element extensions.
3. Never edit `.designer.settings` directly (always change the Module Builder model).
