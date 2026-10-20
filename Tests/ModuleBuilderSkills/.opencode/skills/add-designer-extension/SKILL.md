---
name: add-designer-extension
description: "Extend a designer from the Module Builder: add a context-menu element, association-creation or mapping option to a foreign package, element or association end (packageExtension/elementExtension), define a brand-new Association Settings type drawable between two element types, or attach AI context (AI Settings → Rules) to an element type. USE ONLY WHEN a designer must gain a new option on an existing element, a new kind of association, or AI rules. DO NOT USE FOR building an architecture template's component picker (see architecture-templates) or setting an element or stereotype icon (see module-svg-icon). REQUIRES the typeIds of the element types involved and the target designer's id already identified."
argument-hint: "[target element/package type name and what to add]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.AddDesignerExtension_SkillMd_Agents
contentHash: 66F5DCAF3DD1DFAD74C4FD6F957AFC8387DFE81FF4A06064FDA33218F2B27571
---
# Add Designer Extension

> [!TIP]
> **Read more if you want to know about** designer extensions, package/element extensions, mapping options, or context menu items:
> *   [Conceptual Reference](./resources/designer-extensions-reference.md)
> *   [Workflow & Common Mistakes](./resources/designer-extension-workflow.md)
> *   [New Association Type: Workflow & Common Mistakes](./resources/association-type-workflow.md)
> *(To conserve tokens, avoid reading these files for simple or minor updates.)*

## Purpose

Extend an existing element, package, or association end from a foreign module by adding new context menu options (e.g. child element/association creation or mapping options), or define a new kind of association users can draw between two element types.

## Musts

1. Identify foreign target `typeId` with `run_designer_query` — `findElements({ ..., includeReferences: true })` or `searchElements(...)` — or generated C# `SpecializationTypeId` constants. The foreign type lives in a referenced, read-only package, and the `id` on that referenced match is the typeId. Do not call `getElementDetails` on it: it only works on elements the designer owns, and throws for a referenced one.
2. Link `Designer Settings` to foreign designer GUID (Domain: `6ab29b31-27af-4f56-a67c-986d82097d63`, Services: `81104ae6-2bc5-4bae-b05a-f987b0372d81`).
3. For Element Extensions, apply the `Type Reference Extension Settings` stereotype (mandatory: the Software Factory reads it unguarded) with `Mode = Inherit` unless you deliberately override the type reference (`Required` / `Optional`). `Extension Settings` is optional (display/validate function overrides).
4. Add all context menu options under a `[context menu]` child element.
5. Set type reference of `Association Creation Option` to target end of association settings.
6. Verify via `run_designer_query` → `getDesignerValidationErrors()` and the generated `.designer.settings` file.

**Package settings are the companion step to linking `Designer Settings`.** Linking the extension to a
foreign designer says *what* you are extending; two properties on the owning package's `Module Settings`
decide whether any of it actually ships, and where it lands:

| When the package gains… | Set on that package's `Module Settings` |
|---|---|
| a **stereotype** | `Include in Module` = true — without it the package produces no `<install>` entry and the stereotype ships nowhere |
| a **new or extension designer element** | `Reference in Designer` = the target designer(s), or `Reference in` = *All Designers* — this drives the `<install target=…>` so the element lands in the right designer |

Set them as you add the element, not in a later sweep. The Software Factory warns when a package has
stereotype definitions and is not included — this gets ahead of that warning rather than waiting for it.

If the new element type should carry its own icon, that is optional craft rather than an obligation — see
`module-svg-icon`.

## A New Association Type

When no existing association type connects the two element types, define one. The full steps are in
[association-type-workflow.md](./resources/association-type-workflow.md).

1. Collect the `typeId` of every element type either end can connect to (Must #1) before creating anything.
2. Create an `Association Settings` element under the `Designer Settings` linked to the target designer (Must #2).
3. Apply the `Settings` stereotype to both the source and the target end, and set each end's target types, navigability and C# API property name.
4. Apply a trait (`[Processing Action]` / `[Processing Handler]`) to the target end when mappings need to find it.
5. Wire an `Association Creation Option` into the source element's or package's extension `[context menu]` (Must #5); without it nobody can draw the association.
6. Run the Software Factory and confirm the generated C# models compile.

Never swap the source and target ends (source = the origin element, target = the destination), never set
`Is Navigable Default = true` on the source end of a directed association, and never put a `Mapping Option`
on the source end: it belongs on the target end.

## Attaching AI Rules

To give an AI agent working in the consuming designer context about an element type — including one you
do not own — apply the `AI Settings` stereotype and write the guidance in its `Rules` property. The
Software Factory copies it into a `<rules>` element in the generated `.designer.settings`, but only from
these element types:

| Carries `Rules` into `.designer.settings` | Silently dropped (the stereotype applies, nothing is emitted) |
|---|---|
| `Element Extension`, `Element Settings`, `Association Settings`, `Association Extension`, `Designer Settings` | `Package Settings`, `Package Extension`, `Designer` |

- Write each rule as short, plain prose: state what to do and when. It is read by an agent, not rendered.
- Put a rule on the element type it is about (element-level) when the guidance belongs to that concept;
  put it on `Designer Settings` (designer-level) only when it cuts across every type in that designer.
- The `Element Extension`'s type reference only resolves if the module that owns the foreign type is
  installed in your module's application. Install it first, or the extension has nothing to point at.
- Verify by reading the `<rules>` element under your extension in the generated `.designer.settings`
  after running the Software Factory, not by trusting that the stereotype was applied.

## Must Nots

1. Never guess or hardcode a type GUID from memory.
2. Never remove the `Type Reference Extension Settings` stereotype from an element extension. Its `Mode` may be `Inherit`, `Required` or `Optional`, but the stereotype itself must be present.
3. Never edit `.designer.settings` directly (always change the Module Builder model).
4. Never put AI rules on a `Package Settings` or `Package Extension` and expect them to ship; they are dropped.
