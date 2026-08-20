---
name: module-svg-icon
description: "Craft a house-style SVG icon for a module from a supplied description and apply it as the module's package icon. USE ONLY WHEN asked to create, set, or refresh a module's icon. DO NOT USE FOR deciding or sourcing what the icon should represent — it takes that description exactly as supplied, never invents one. REQUIRES the icon's intended visual description already supplied by the caller."
argument-hint: "[description of what the icon should visually represent]"
keywords: [icon, svg, branding, application.config, imodspec, package]
template-id: Intent.ModuleBuilder.AI.Skills.Skills.ModuleSvgIcon_SkillMd_Agents
contentHash: 7F9DCAAE11B10915604193D004810CB48CAE6FAFB175E4B683143B25058F8B53
---
# Skill: module-svg-icon

## When to Use

Use when a maintainer supplies (or points you to) a description of what a module's icon should
represent, and wants that icon created or refreshed. This skill only crafts and applies the icon
— it does not decide what the icon should depict, and does not go looking for that context itself.

## The Core Trap

A module's icon is **not** a scriptable model property. The `Module Settings` stereotype on the
`Intent Module` element has no `Icon` field, and neither does the package element itself. The icon
instead lives in the module's own `.application.config`, as a root-level, attribute-less
`<icon>data:image/svg+xml;base64,...</icon>` element with a sibling `<iconType>UrlImagePath</iconType>`.
`.imodspec`'s `<iconUrl>` is generated FROM that value by the Software Factory — exactly the same
trap `module-versioning` describes for `<version>`. Hand-editing `.imodspec` directly is silently
discarded on the next run.

## Why A Script, Not Direct File Edits

`.application.config` is normally off-limits to hand-editing — it is a large, mostly generated file.
This skill is a narrow, deliberate exception, scoped to **exactly** the root `<icon>` /
`<iconType>` pair. Nowhere else in that file should be touched — in particular, leave the many
other `<icon type="..." source="...">` entries elsewhere in the file alone; those configure
per-designer-element icons and are unrelated to the module's own package icon.

There is a second, non-functional reason to go through a script rather than `read_file` /
`patch_file`: the icon is a base64-encoded data URI that commonly runs into tens of thousands of
characters. Reading or patching that through your own context window is wasteful, and hand-copying
it risks a silent transcription error that corrupts the icon. **Never read or reproduce the base64
value yourself** — a script computes it and writes it directly; you only ever see the small SVG
source you authored.

## Step 1 — Craft The SVG

Default style — deviate from it only when the supplied description clearly calls for it:

| Aspect | Default |
|---|---|
| Canvas | `viewBox="0 0 50 50"`, square |
| Shape | Full-bleed circular badge: `<circle cx="25" cy="25" r="24.4">` |
| Fill | A 2–4 stop linear gradient in one hue family, chosen from the description (a flat single color is an acceptable fallback) |
| Glyph | One simple white (`fill="#fff"`) line-art or solid shape, centered, legible at ~16–24px render size |
| Color budget | Gradient (one hue family) + white glyph — nothing else: no drop shadows, no secondary accents, no photographic detail |
| Complexity | A handful of paths at most; simplify the concept rather than adding detail if it doesn't read at small size |
| Text | Avoid; a 1–2 letter monogram is acceptable if nothing else reads clearly |

Write the SVG to a real OS temp file via a shell command (not the project's file tools) — it is a
disposable draft, not a repo artifact.

## Step 2 — Encode And Apply, Via Script Only

Run a single shell script (PowerShell or Bash) that does all of the following. Do not split this
across `read_file`/`patch_file` calls — the point is that the base64 string never has to pass
through your own output:

1. Read the temp `.svg` file's bytes and compute base64 (`[Convert]::ToBase64String(...)` /

`base64`).

2. Build the data URI: `data:image/svg+xml;base64,<computed>`.
3. In the module's `.application.config`, replace only:
    - the attribute-less root `<icon>...</icon>` element — not any `<icon type="..." source="...">` entry
    - the sibling `<iconType>...</iconType>` element (should read `UrlImagePath`; set it if absent)
4. Before writing, assert **exactly one** match was found for each element. Abort rather than guess

if the count is zero or more than one.

5. Delete the temp SVG file once the config has been updated.

## Step 3 — Regenerate

Run the Software Factory. It regenerates `.imodspec`'s `<iconUrl>` from the `.application.config`
value you just set.

## Step 4 — Verify Without Reading The Payload

Confirm via `get_file_diffs` that exactly the icon-related line(s) changed in `.application.config`
and `.imodspec`. Judge this structurally — which files appear in the diff, how many lines changed —
rather than by reading the base64 content itself.

## Verification Checklist

- [ ] Description taken exactly as supplied — this skill did not source or invent it
- [ ] SVG follows the default style guide, or deviates for a stated reason
- [ ] Draft SVG written to a real OS temp location, never a tracked project path
- [ ] `.application.config` edited by script only — never `read_file`/`patch_file` on the base64 value
- [ ] Only the root `<icon>`/`<iconType>` pair touched; other `<icon type="..." source="...">` entries left alone
- [ ] Exactly one match asserted before replacing either element
- [ ] Software Factory run; `.imodspec`'s `<iconUrl>` confirmed updated
- [ ] Diff scoped to icon-only lines, confirmed without reading the base64 payload
- [ ] Temp SVG file cleaned up
