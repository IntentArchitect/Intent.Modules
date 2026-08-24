# Context: Intent.ModuleBuilder.AI.Skills

## Purpose

Bundles a fixed set of AI agent skills and instruction files for building Intent Architect modules into the consuming repo's `.agents/` folder. Every bundled file is generated from this module's own source and always overwritten on install/update — there is no per-repo customization, no designer model dependency for consumers, and no settings gating which skills are included.

## Architectural Decisions

- **A module's package icon is not a model-scriptable property.** Checked the `Module Settings` stereotype on `Intent Module` (properties: Version, API Namespace, NuGet Package Id/Version, Include in Module, Reference in [Designer], Include Release Notes, Project URL) and the package element itself — neither carries an `Icon` field, unlike `Version`/`Description` which the model does own. The icon instead lives in the module's own `.application.config`, as a root-level, attribute-less `<icon>data:image/svg+xml;base64,...</icon>` element with a sibling `<iconType>UrlImagePath</iconType>`. `.imodspec`'s `<iconUrl>` is generated FROM that value by the Software Factory on the next run — the same trap as `<version>`, just via a different source file.

- **`module-svg-icon` is a scoped, deliberate exception to "never hand-edit `.application.config`."** That file is otherwise off-limits because it is large and mostly generated. The exception is narrowed to exactly the root `<icon>`/`<iconType>` pair — never the many other `<icon type="..." source="...">` entries elsewhere in the same file, which configure unrelated per-designer-element icons.

- **The edit must be done by a script, never by `read_file`/`patch_file` on the base64 value itself.** This is a non-functional constraint, not a permissions one: the icon's base64 payload commonly runs into tens of thousands of characters. Passing it through the AI's own context is wasteful and risks a silent transcription error that corrupts the icon. The skill's script reads the drafted SVG file, computes the base64 itself, and writes the substitution directly — the AI never sees or reproduces the encoded string.

- **Default SVG house style is baked into the skill, not left open-ended.** Derived from real `<iconUrl>`/`<icon>` values already shipping in installed modules: `viewBox="0 0 50 50"`, a full-bleed circular badge, a gradient (or flat) fill in one hue family, and a single simple white glyph. Giving the AI a concrete default (deviate only when the supplied description clearly calls for it) keeps output consistent with the existing module gallery instead of reinventing a style per module.

- **Never edit a `...TemplatePartial.cs` markdown raw string with Intent's `patch_file`.** The tool auto-adjusts indentation to the surrounding C#, and because the markdown lives inside a raw string literal at column 0 it re-indents the _entire_ string to the constructor's nesting level and flattens the relative indentation inside every fenced code sample. The damage is whole-file, not local to the patch, and it is silent — the tool reports success and only a non-zero `indentDelta` in the result hints at it. Use a plain text editor that does no reformatting. This was hit and reverted while adding the cross-module-integration content in 1.0.1-pre.1.

- **Flattened `Skills/{SkillName}/` into `Skills/` directly (1.0.1-pre.3).** The per-skill subfolder existed only to disambiguate the repeated `SkillMd_Agents` name across skills — the `Skills/` folder already provided that grouping, so the extra nesting was pure ceremony. Flattening, plus renaming each template with its former folder name as a prefix, removes the redundant folder while keeping every element name unique. Also caught mid-flatten: the Software Factory's rename-detection is not reliable across a simultaneous namespace + physical-path change — on at least one sibling module it silently reset a hand-authored `TemplatePartial.cs` body to placeholder content instead of preserving it (`get_file_diffs` caught it before it was applied). Treat every such rename's `get_file_diffs` output as unverified until read, not just "rename" vs "create" in the change summary.
- **`Default Location` replaced by the `MarkdownFile` constructor's `relativeLocation` argument (1.0.1-pre.3).** Discovered that `Default Location` only seeds the constructor's output-path argument at the moment a File Template element is first created; afterward the constructor's `Body = Mode.Ignore` means the model setting is never read again, so it looked live but did nothing on every subsequent regeneration. Baking the value directly into the constructor removes that dead, misleading setting. Existing consuming applications that installed an earlier version still show the old nested `Skills/{SkillName}/` shape in their own `Codebase Structure` designer until they update through the `Version Migration` added alongside this change.

- **The no-new-`.imod` diagnosis is filed as a build gotcha.** Building a module compiles its
  `.csproj`, and the packaging step that produces the `.imod` runs off that compilation — so a
  change confined to non-C# files may not trigger it, leaving templates generating from previously
  packaged content with nothing reported. `dotnet build --no-incremental` forces it. It belongs in
  `known-build-gotchas` rather than a versioning skill because it reaches every session through an
  `applyTo: '**'` instruction file, and because it is the diagnosis behind `AI.Workflow`'s "a
  version number is not a debugging tool" rule rather than a versioning decision itself.
- **Editing these raw-string templates: the closing delimiter's indentation is load-bearing.**
  `KnownBuildGotchasMd` indents its content 10 spaces and closes at the same indent, which is what
  de-indents the emitted markdown. A programmatic splice that leaves the closing delimiter at
  column 0 compiles and packages perfectly while generating an effectively empty document — caught
  only by reading the staged diff before applying (it showed every section being deleted). Always
  diff before apply on these templates.

## Invariants & Constraints

- Every skill's `File Template` (type `Single File`) sits directly under the `Skills/` folder, named `<SkillName>_SkillMd_Agents` (and `<SkillName>_Resources...` for resource files) — there is no per-skill subfolder. `File Settings(Output File Content=Text, Templating Method=Markdown File
  Builder)`; `Template Settings(Source=Lookup Type, Role=AI.Context.Skills)` with `Default Location` left **unset**. New skills should mirror this flat shape exactly — see `ModuleVersioning` or `ModuleSvgIcon` as reference.
- The `MarkdownFile` constructor's `relativeLocation` argument carries the real output path directly — e.g. `new MarkdownFile("SKILL", relativeLocation: "module-versioning")` — never the `Template
  Settings.Default Location` model setting. `Default Location` only ever seeds that argument's value at the moment a File Template is first created; the constructor's `Body = Mode.Ignore` means the model setting is never read again afterward, so leaving it set is misleading. See the `add-module-skill-template` skill's mechanism step 6 for the full anchor-resolution rules.
- This module has no `Module Settings Configuration`/settings of its own — every bundled skill is unconditional. Gating by setting belongs to `Intent.ModuleBuilder.AI.Workflow`, not here.

## Module Interactions

- **Intent.ModuleBuilder.AI.Workflow** — a separate module bundling conditional, process-shaped workflow skills (version increment timing, doc-chore cadence, context capture). No package dependency is declared between the two modules, and that stays deliberate — each bundle must remain independently installable. The one exception: AI.Workflow's `module-docs-chore` names `module-svg-icon` by name, but only as a **soft, conditional** pointer ("if the skill is available in your environment, use it") gated behind its own `MaintainModuleIcon` setting — never assumed present. Do not upgrade this to a declared dependency without a deliberate decision to do so; keep any further cross-references to this same soft-pointer shape.

## Superseded

- Earlier version of this file claimed AI.Workflow's skills reference AI.Skills' reference skills "for the underlying mechanics." That was asserted, not verified, and was false at the time. Superseded again above once `module-docs-chore` was deliberately given the one soft reference described.
- **Skills/{SkillName}/ per-skill subfolder + `Default Location`-driven output (through 1.0.1-pre.2).** Replaced in 1.0.1-pre.3 by the flat `Skills/` layout with `relativeLocation` baked into each constructor — see the Architectural Decisions above.
