# Context: Intent.ModuleBuilder.AI.Skills

## Purpose
Bundles a fixed set of AI agent skills and instruction files for building Intent Architect modules
into the consuming repo's `.agents/` folder. Every bundled file is generated from this module's own
source and always overwritten on install/update — there is no per-repo customization, no designer
model dependency for consumers, and no settings gating which skills are included.

## Architectural Decisions

- **A module's package icon is not a model-scriptable property.** Checked the `Module Settings`
  stereotype on `Intent Module` (properties: Version, API Namespace, NuGet Package Id/Version,
  Include in Module, Reference in [Designer], Include Release Notes, Project URL) and the package
  element itself — neither carries an `Icon` field, unlike `Version`/`Description` which the model
  does own. The icon instead lives in the module's own `.application.config`, as a root-level,
  attribute-less `<icon>data:image/svg+xml;base64,...</icon>` element with a sibling
  `<iconType>UrlImagePath</iconType>`. `.imodspec`'s `<iconUrl>` is generated FROM that value by the
  Software Factory on the next run — the same trap as `<version>`, just via a different source file.

- **`module-svg-icon` is a scoped, deliberate exception to "never hand-edit `.application.config`."**
  That file is otherwise off-limits because it is large and mostly generated. The exception is
  narrowed to exactly the root `<icon>`/`<iconType>` pair — never the many other
  `<icon type="..." source="...">` entries elsewhere in the same file, which configure unrelated
  per-designer-element icons.

- **The edit must be done by a script, never by `read_file`/`patch_file` on the base64 value
  itself.** This is a non-functional constraint, not a permissions one: the icon's base64 payload
  commonly runs into tens of thousands of characters. Passing it through the AI's own context is
  wasteful and risks a silent transcription error that corrupts the icon. The skill's script reads
  the drafted SVG file, computes the base64 itself, and writes the substitution directly — the AI
  never sees or reproduces the encoded string.

- **Default SVG house style is baked into the skill, not left open-ended.** Derived from real
  `<iconUrl>`/`<icon>` values already shipping in installed modules: `viewBox="0 0 50 50"`, a
  full-bleed circular badge, a gradient (or flat) fill in one hue family, and a single simple white
  glyph. Giving the AI a concrete default (deviate only when the supplied description clearly calls
  for it) keeps output consistent with the existing module gallery instead of reinventing a style
  per module.

## Invariants & Constraints

- Every skill folder under `Skills/` is a `Folder` (`Folder Options` stereotype, Namespace
  Provider=true) containing one `SkillMd_Agents` `File Template` (type `Single File`), with `File
  Settings(Output File Content=Text, Templating Method=Markdown File Builder)` and `Template
  Settings(Source=Lookup Type, Role=AI.Context.Skills, Default Location=<skill-name>)`. New skills
  should mirror this shape exactly — see `ModuleVersioning` or `ModuleSvgIcon` as reference.
- The `MarkdownFile` constructor argument is always `new MarkdownFile("SKILL", relativeLocation: "")`
  — the output path comes from the `Template Settings` stereotype's `Default Location`, not from
  these constructor arguments. A freshly scaffolded `File Template` defaults to different (wrong)
  values here; fix them before authoring content.
- This module has no `Module Settings Configuration`/settings of its own — every bundled skill is
  unconditional. Gating by setting belongs to `Intent.ModuleBuilder.AI.Workflow`, not here.

## Module Interactions

- **Intent.ModuleBuilder.AI.Workflow** — a separate module bundling conditional, process-shaped
  workflow skills (version increment timing, doc-chore cadence, context capture). No package
  dependency is declared between the two modules, and that stays deliberate — each bundle must
  remain independently installable. The one exception: AI.Workflow's `module-docs-chore` names
  `module-svg-icon` by name, but only as a **soft, conditional** pointer ("if the skill is available
  in your environment, use it") gated behind its own `MaintainModuleIcon` setting — never assumed
  present. Do not upgrade this to a declared dependency without a deliberate decision to do so; keep
  any further cross-references to this same soft-pointer shape.

## Superseded

- Earlier version of this file claimed AI.Workflow's skills reference AI.Skills' reference skills
  "for the underlying mechanics." That was asserted, not verified, and was false at the time.
  Superseded again above once `module-docs-chore` was deliberately given the one soft reference
  described.
