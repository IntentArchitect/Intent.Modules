---
applyTo: '**'
description: >
Reference for authoring Intent Architect Module Builder designer extensions: 
keywords: [intent architect, module builder, designer extension, element extension, package extension, association, context menu, stereotype, mapping, type id]
contentHash: 970EAC985F837D137E431D5D13DB55C44670FAFBA0E444D7A460E0EFA5A86888
---
# Module Builder Designer Extensions — Reference

## Two-Layer Architecture

Every designer extension lives in two places:

| Layer | Location | Purpose |
|---|---|---|
| **Module Builder model** | `Intent.Metadata/Module Builder/` XML | Source of truth; edited via Intent Architect designer |
| **`.designer.settings` XML** | `modelers/*.designer.settings` | Generated output consumed at runtime — **never edit directly** |

Run the Software Factory on the module to regenerate `.designer.settings` from the model. Direct edits to `.designer.settings` are overwritten on the next SF run.

- --

## How a Module "Plugs In" to a Foreign Designer

A `Designer Settings` node carries an **Extend Designers** list. This contains the GUID(s) of foreign designer surfaces your module extends. Without this link, none of your extensions appear in the target designer.

Key foreign designer IDs:

| Designer | ID |
|---|---|
| Domain | `6ab29b31-27af-4f56-a67c-986d82097d63` |
| Services | `81104ae6-2bc5-4bae-b05a-f987b0372d81` |

One module can extend multiple foreign designers by having multiple `Designer Settings` nodes, each with a different `Extend Designers` value (e.g., `Domain.Events` extends both Domain and Services in two separate `Designer Settings` nodes).

- --

## Three Extension Entry Points

### 1. `packageExtension` — Extend an Existing Package

Adds context menu options and new element types to an existing **package** type from another module.

Use when: your module needs to allow users to create new top-level elements inside another module's package (e.g., adding "New Integration Event Handler" to a Services Package).

- *Target identification:** set the type/typeId to the foreign package specialization.

### 2. `elementExtension` — Extend an Existing Element

Adds context menu options to an existing **element** type from another module.

Use when: you need to attach new behaviour (usually new association creation options) to an element type you don't own — e.g., adding "Publish Integration Event" to a `Command`.

- *Key insight:** `elementExtension` only adds context menu options; it does not change the element's own properties. Use a `Stereotype Definition` if you need to add new properties to the element.

### 3. `associationExtension` — Extend an Existing Association's Ends

Adds context menu options to the **source end** or **target end** of an existing association type.

Use when: you need new actions available when a user right-clicks a specific end of an association they didn't create — e.g., adding "Publish Integration Event" to the target end of a `Domain Event Handler Association`.

- *Important:** the context menu is placed on the *end* (source/target extension child), not on the association itself.
- --

## Defining New Types

### Element Settings

Creates a brand-new element specialization within your module. Required when your module introduces a new concept (e.g., `Integration Event Handler`, `Domain Event`).

Key configuration via the `Settings` stereotype:

- `Save Mode` — usually `Default`
- `Icon` — base64 SVG/PNG
- `Display Text Function` — JavaScript; controls rendered label (see below)
- `Allow Rename`, `Name Must Be Unique`, `Allow Sorting`

Key configuration via the `Type Reference Settings` stereotype:

- `Mode` — `Disabled` (no type reference), `Required`, `Optional`
- `Target Types` — array of element type GUIDs this element can reference

### Association Settings

Creates a new association type with a `Source End` and `Target End`. Each end has its own `Settings` stereotype:

- `Target Types` — array of type GUIDs the end can connect to
- `Api Property Name` — C# property name on the generated model
- `Display Text Function` — JavaScript
- `Name Accessibility` — `Hidden` / `Visible` / `ReadOnly`
- `Is Navigable/Nullable/Collection Default` — default multiplicity
- `Allow Multiple` — whether multiple associations of this type are permitted

Traits are applied as stereotypes on the target end — e.g., `[Processing Action]`, `[Processing Handler]`. These are how the mapping system and runtime identify the role of each end.

Add a `[context menu]` child under the target end's settings to give users mapping or creation options directly from the association end.

- --

## Context Menu Options

Inside any context menu (on a package extension, element extension, element settings, or association end):

| Option Type | Creates |
|---|---|
| **Element Creation Option** | A new child element of the configured specialization type |
| **Association Creation Option** | A new association of the configured specialization type |
| **Mapping Option** | Opens the mapping dialog for a configured Mapping Settings |

All three share these `Option Settings` properties:

- `Shortcut` — keyboard shortcut (e.g., `ctrl + shift + i`)
- `Default Name` — initial name for the created element
- `Allow Multiple` — whether more than one can be created
- `Menu Group` — integer; options with the same group number are visually grouped with a separator between groups
- `Is Option Visible Function` — JavaScript returning `true`/`false` to conditionally hide the option
- --

## JavaScript Function Contracts

All functions run in a sandboxed context scoped to the current element. Available globals:

| Global | Type | Description |
|---|---|---|
| `name` | string | Element's current name |
| `element` | object | The element itself |
| `application` | object | Application-level settings access |
| `getAssociations(typeId)` | function | Returns associations of the given type |
| `theme` | object | `theme.isDark` boolean for dark/light theming |

- *`displayFunction`** — Controls how an element renders its label. Returns an array of text segment objects:

```javascript
// Simple example
return [{ text: name }];

// With type annotation
const assoc = getAssociations("fa57ec52-...").filter(x => x.isTargetEnd())[0];
const result = [{ text: `${name}${genericTypes}` }];
if (assoc != null) {
    result.push({ text: " : ", cssClass: "annotation" });
    result.push({ text: assoc.typeReference.display, cssClass: "typeref", targetId: assoc.typeReference.typeId });
}
return result;
```

- *`filterFunction`** — Controls visibility of mapping elements or menu options. Returns boolean:

```javascript
// Only show if no existing association of this type
let myAssocTypeId = "4c0cc50b-...";
return element.getAssociations(myAssocTypeId).length == 0;
```

- *Element Event Handler scripts** — Lifecycle hooks: `On Created`, `On Name Changed`, `On Property Changed`. Access settings via `application.getSettings(...)`.
- --

## Mapping Settings

Used to configure how elements from one designer map to elements in another. The top-level `Mapping Settings` node carries:

- `defaultModeler` — target designer GUID
- `optionsSource` — usually `elements-of-type`
- `autoSyncTypeReferences` — whether mapped type references stay in sync

Child `mapping` nodes describe how each source element type maps to a target type:

- `criteria` — filter by specialization type, whether it has children, a type reference, etc.
- `mapTo` with `childMappingMode` — `traverse` (walk into children) or `map-to-child` (project to a specific child type)
- `filterFunction` — optional JavaScript to further restrict mappable elements
- --

## Finding Type IDs of Foreign Elements

The most critical prerequisite for any extension is knowing the `typeId` of the element/package/association you want to target. Three reliable sources:

1. **MCP `find_designer_elements`** — search by name in the target application's Module Builder designer; the result includes the element's `id`.
2. **Generated C# API** — every element type produces a model class with a `SpecializationTypeId` constant (e.g., `CommandModel.SpecializationTypeId`).
3. **Target module's `.designer.settings`** — every `<specializationTypeId>` attribute.

Never hardcode IDs from memory. Always read them from the live model or generated code.

- --

## C# API Layer (Generated by Software Factory)

Every new element/association type in your module generates corresponding C# in `Api/`:

| File | Content |
|---|---|
| `ApiMetadataDesignerExtensions.cs` | `IMetadataManager.MyDesigner(app)` extension methods |
| `ApiMetadataProviderExtensions.cs` | `designer.GetMyElementModels()` LINQ-style queries |
| `*Model.cs` | Strongly-typed wrapper; exposes `SpecializationTypeId` constant, child accessors, stereotype accessors |
| `*ModelAssociationExtensions.cs` | Extension methods for navigating association ends |

Element and association *extensions* (not definitions) do not generate a dedicated model class — they appear only in `.designer.settings`.

- --

## MCP Tool Sequence for Any Extension Task

```
1. get_status(workingDirectory)              → confirm solution is open
2. get_full_instructions()                  → mandatory; governs all tool use
3. get_designers(applicationId)             → find Module Builder designer ID
4. get_designer_model_structure(...)        → overview of existing extensions
5. find_designer_elements(query, ...)       → locate target element by name/type
6. get_designer_element_details(element)    → full detail: stereotypes, members,
                                              codeFiles, type IDs
7. [make model changes via MCP tools]
8. run_software_factory(...)
9. get_staged_file_diffs(...)
10. apply_staged_file_changes(...)
11. [verify compilation]
```

- *Never call Intent Architect MCP tools in parallel.** Each call must complete before the next begins.
