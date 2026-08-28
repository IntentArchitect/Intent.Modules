---
name: module-element-icons
description: "Set the icon on a stereotype definition or on a custom element type in a designer you own. USE ONLY WHEN a stereotype or element type you define should show a distinct icon in the Intent Architect UI. DO NOT USE FOR a module's own package icon (see module-svg-icon) — different file, different mechanism, and the two are routinely confused. REQUIRES the stereotype definition or element type already modelled in the Module Builder designer."
keywords: [icon, stereotype, element type, designer, module builder, ui]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleElementIcons_SkillMd_Agents
contentHash: 7AF1437F472ED8D1ED1BAF2659611C188CE7029A70327ED2DE2EC2D0EF53151D
---
# Skill: module-element-icons

Two different things in a designer can carry an icon, and they are configured in two different places.
Neither is the module's own package icon — see the boundary below before going further.

| What you are icon-ing | Where the icon lives |
|---|---|
| A **stereotype** | Fields on the stereotype definition itself: an `icon` (a type + source pair), plus `displayIcon` and `displayIconFunction` |
| A **custom element type** (`Element Settings` / `Core Type`) | The `Settings` stereotype applied to it: `Icon`, `Expanded Icon`, `Icon Function` |

===

## Stereotype Icons

A stereotype's icon is **structural** — part of the stereotype definition, not a property you apply to it.
It is a type/source pair: a `FontAwesome` type takes an icon name as its source (`cogs`, `database`); a
`UrlImagePath` type takes a data URI.

Two companions control *when* it shows:

- **`displayIcon`** — whether the badge renders on elements carrying the stereotype at all.
- **`displayIconFunction`** — a script for conditional display, when the badge should only appear for

certain property values. Prefer plain `displayIcon` unless the condition is real; a function that always
returns the same answer is just a slower checkbox.

A stereotype badge competes for space with the element's own icon and its name. Reach for one when the
stereotype changes how the element should be *read* at a glance — not merely to show it was applied.

## Element Type Icons

For an element type you define — an `Element Settings` or `Core Type` node — the icon is a property on the
`Settings` stereotype that node carries:

| Property | Purpose |
|---|---|
| `Icon` | The element's icon in the model tree and on diagrams |
| `Expanded Icon` | Optional alternate shown when the node is expanded. Leave unset unless expanding genuinely changes what the node represents. |
| `Icon Function` | A script returning an icon per element, for types whose icon depends on their own state. **Returning null falls back to the default icon** — that is the designed escape, not a failure. |

`Icon` and `Icon Function` are alternatives. Set the static `Icon` unless the icon genuinely varies per
element; a function is re-evaluated as the model changes and is harder to reason about.

## Setting A Built-In Icon

For anything the shipped icon set already covers, use the designer's icon picker — or, for a stereotype,
a FontAwesome name. Both store a short reference, and there is nothing to hand-author:

| Target | Stored as |
|---|---|
| Stereotype | `<icon type="FontAwesome" source="cogs" />` on the definition |
| Element type | The `Icon` property, as JSON: `{"type":0,"source":"./img/icons/…"}` |

## Setting A Bespoke SVG Icon

When the built-in set does not carry the concept, the icon becomes a **base64 `data:` URI** in exactly the
same slot — the `source` stops being a path and becomes the encoded image:

| Target | Stored as |
|---|---|
| Stereotype | `<icon type="UrlImagePath" source="data:image/svg+xml;base64,…" />` |
| Element type | `Icon` property JSON with `"source":"data:image/svg+xml;base64,…"` |

### 1. Craft the SVG

Follow **`module-svg-icon`'s Step 1 house style verbatim** — same canvas, badge, gradient, single white
glyph, same colour budget. An element icon renders *smaller* than a package icon and sits in a dense tree,
so the "simplify rather than add detail" rule binds harder here, not less. Write it to an OS temp file, not
into the repo: it is a disposable draft.

Keeping both icon families in one visual language is the point — a bespoke element icon that ignores the
house style is more jarring than a generic built-in one.

### 2. Encode and apply without reading the payload

- *Never read, echo or copy the base64 string yourself.** A modest SVG encodes to well over a thousand

characters; passing it through your own output wastes context and risks a silent transcription corruption
that renders as a blank icon with no error at all.

The value is a designer property, so it is set with `run_designer_script` — not by editing designer
metadata files. To keep the payload out of your context, have a shell script compute the base64 and write a
small script file that assigns it, then pass that file to `run_designer_script` via `includedScriptPaths`
so it is prepended to your own script. You author the logic; the generated file carries the string.

### 3. Regenerate

Run the Software Factory. It propagates the value into the module's generated `.designer.settings`, which
is what consuming applications actually read — the designer model is the source, that file is the output.

### Reusing one icon across several elements

Set it on each element from the same source SVG rather than copying the encoded value between them. Copying
is where the silent truncation happens, and it is no faster.

===

## The Boundary With `module-svg-icon`

A module's **own package icon** — the one shown in the module registry — is a different thing entirely. It
lives in the module's `.application.config` as a root-level `icon`/`iconType` pair, and the `.imodspec`
`iconUrl` is regenerated from it. That is `module-svg-icon`'s job — and it goes through a script not because
nothing else can set it, but because the base64 payload would otherwise pass through your context.

The confusion is easy to hit because a designer's `.application.config` also contains many per-element icon
entries. **Those are not the package icon**, and the package icon is not one of them.

| If you want… | Use |
|---|---|
| the icon for a stereotype or element type you defined | this skill |
| the icon for the module itself, in the registry | `module-svg-icon` |

## Checklist

- [ ] Icon set on the right thing — stereotype definition vs element type's `Settings` stereotype
- [ ] `Icon` used rather than `Icon Function` unless the icon genuinely varies per element
- [ ] `Expanded Icon` left unset unless expanding changes what the node represents
- [ ] Built-in icon used where one fits; bespoke SVG only where the concept is not covered
- [ ] A bespoke SVG follows `module-svg-icon`'s house style, and reads at element size
- [ ] Base64 never read, echoed or copied by hand — computed by script, applied via the designer
- [ ] Software Factory run so the value reaches the generated `.designer.settings`
- [ ] The module's own package icon left alone (that is `module-svg-icon`)
- [ ] Software Factory run and the designer inspected to confirm the icon actually renders
