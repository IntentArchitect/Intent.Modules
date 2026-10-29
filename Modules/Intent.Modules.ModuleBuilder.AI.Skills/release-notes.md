### Version 1.2.0

> ⚠️ **NOTE**
>
> Obsolete. This module can safely be uninstalled. Its remaining skills (`intent-module-orchestrator`, `module-building-strategies` and `module-svg-icon`) and the `exception-guidelines` and `known-build-gotchas` instructions moved to `Intent.ModuleBuilder.AI.Guidelines`, which is installed with this version. The other seven are built into Intent Architect from 5.4.0. This version requires Intent Architect 5.4.0 or later.

### Version 1.1.0

> ⚠️ **NOTE**
>
> Seven skills are removed because they ship built into Intent Architect from 5.4.0: `add-designer-extension`, `add-module-migration`, `architecture-templates`, `file-builder-expert`, `intent-metadata-consumer`, `module-docs` and `module-versioning`. The Software Factory run after updating deletes their files. The bundle now ships `intent-module-orchestrator`, `module-building-strategies` and `module-svg-icon`, plus the `exception-guidelines` and `known-build-gotchas` instructions.

> ⚠️ **NOTE**
>
> Five skills are removed and their content merged into others: `add-association-type` into `add-designer-extension`, `module-element-icons` into `module-svg-icon`, `intent-mapping-architect` and `module-debugging` into `file-builder-expert`, and `intent-domain-interactions-expert` is dropped because interaction strategies are deprecated. The Software Factory run after updating deletes their files.

- Improvement: Supports Intent Architect 5.1.0 up to, but not including, 5.4.0. From 5.4.0, seven of these skills ship built into Intent Architect and this module is no longer offered, so uninstall it once you upgrade.
- Improvement: Where the remaining skills and instructions point at a removed skill, they now say "if available", so the wording holds whether or not that skill is installed.
- Improvement: The bundle ships 10 skills instead of 15, so agents choose from fewer, broader skills. `file-builder-expert` now covers designer mappings (with a mapping cheatsheet) and debugging any module code, module tasks included, by logging a runtime value (with a debugging resource, and named in its description so a logging question finds it), `add-designer-extension` covers defining a new association type, and `module-svg-icon` covers stereotype and element-type icons as well as the module's package icon.
- Improvement: `module-building-strategies` lists interaction strategies (`IInteractionStrategy`) as a pattern new modules must not copy: generate the handler's structure and leave its body to the developer or AI.
- Improvement: Version-raise guidance names the exact versions each API needs in one line and points to `known-build-gotchas` for the `NU1605` chain, instead of repeating the chain in every skill.
- Fixed: Code samples across the skills now compile against Module Builder's default dependency floors (`Intent.Common` 3.7.2, `Intent.Common.CSharp` 3.8.0) or name the version to raise to, and invented or wrong APIs and guidance are corrected — among them `IHasMapping` and `MappingNode`, the OnBuild/AfterBuild phases, single-instance type resolution, `SingleFileListModel` file names, and which exception type renders Markdown.
- Fixed: The skills no longer rely on code third-party module authors cannot use: `TemplateRoles` constants are replaced by literal role strings, with a role table in `intent-module-orchestrator` that calls out the `Domain.Entity.Primary` trap, Intent.Modules.NET internals are no longer presented as SDK, and every tool name matches Intent Architect's current MCP tools.
- Improvement: New guidance on attaching AI Rules to another designer's element types, carrying AI guidance without AI task providers (now deprecated), checking whether another template exists, editing collection items and switch cases, publishing requests safely with `EmitOrPublish`, and registering mapping resolvers.
- Improvement: The documentation, versioning, migration, architecture-template and icon skills describe what the Software Factory actually writes and owns; `module-docs` uses hyphenated tags and never labels anything beta or pre-release.
- Fixed: Where a skill says to raise `Intent.Common.CSharp`, it also names the `Intent.Modules.Common` and `Intent.SoftwareFactory.SDK` versions to raise with it, so restore does not fail with `NU1605`. Where it teaches an API that needs a version your feed may not offer yet, it says how to check first. Template lookups in the `file-builder-expert` cheatsheet now sit in `OnBuild`, and `architecture-templates` pins Component Module versions to published releases.
- Improvement: `known-build-gotchas` explains that a template's `Role` names the consuming application's Output Anchor its output is placed under, with `Default Location` only a sub-folder beneath it. The `file-builder-expert` cheatsheet shows public constants and static values at the 3.8.0 floor, and names 3.9.8 for a field's `Public()` and `ReadOnly()`.
- Improvement: `module-building-strategies` and `file-builder-expert` now make `Body = Mode.Ignore` the default for a new template's "implement here" body, a stub the developer or AI writes, and keep code the template can generate in its own fully managed member. Merge is for bodies the module's `CONTEXT.md` or the developer says are shared, and `// IntentInitialGen` only applies there. An existing template's body mode is never changed without the developer.
- Improvement: `module-building-strategies` says to prove each code pattern in hand-written, tested reference code before templating it for large work, to stop and ask when a pattern has none, and when to stop for review. Generated skills find pending work by the `NotImplementedException` stub rather than `// IntentInitialGen`, which never reaches the generated file.
- Improvement: Depends on `Intent.Common` 3.11.9, whose Markdown parser no longer splits a bullet that wraps onto a second line.
- Improvement: `module-svg-icon` can now design a module's icon when asked, not only craft one from a supplied description. It renders drafts at 16–128px on light and dark backgrounds beside the neighbouring modules' icons, measures them against legibility rules for gaps, detail size, glyph fill, margins and contrast, and applies an icon only after you approve the rendered image. It applies only the `.imodspec` from the Software Factory run and checks by hash that the approved SVG reached the `.application.config`, the `.imodspec` and the built `.imod`. It applies the icon with a ready-made script that never prints the file, and confirms the result by line counts and hashes rather than diffs, which would print the encoded icon.

### Version 1.0.3

- New Feature: `known-build-gotchas` now explains that a scaffolded template file is part designer-owned and part hand-authored — `[IntentManaged(Mode.Fully)]` members are rewritten on every regeneration while `Mode.Ignore`/`Mode.Merge` bodies are preserved — and that `TemplateId` is derived from the File Template element's own name and folder, so editing the constant achieves nothing while silently breaking every lookup that resolves against the real id.
- Fixed: `known-build-gotchas` stated that an explicit `install_or_update_modules` never belongs in the routine fix for a template change that is not taking effect. A rebuilt module is not always re-detected, so an agent following that advice would see stale output and start correcting a template that was already right — the guidance now says to confirm the change actually reached the generated output and to re-install the module by hand when it did not, one module at a time.

### Version 1.0.2

- Improvement: `known-build-gotchas` now carries an ordered diagnostic ladder for a module change that does not reach generated output — rule out protected output, confirm the build reached the cache, force a re-install of the same version, force a rewrite — ending in "report what you ruled out" rather than renumbering to force it.
- New Feature: `known-build-gotchas` covers module discoverability for install, naming `intent.repositories.config` and the global Asset Repositories settings as the two places a locally-built `.imod` can be found, and directing the agent to inform the user rather than configure either.
- Fixed: `known-build-gotchas`'s frontmatter `description` spanned multiple physical lines, which this project's parser silently drops — the file had been shipping with its description truncated to its first line. Rewritten as a single-line quoted string.
- Fixed: Two headings in the new sections began with `**bold**`, which the generator rewrites into a `- *` list marker. Reworded so the bold sits inside the sentence.
- Improvement: `known-build-gotchas` condensed throughout — the five pre-existing gotchas tightened and their correct/wrong example pairs reduced to the correct form, keeping the file's always-on cost down now that it also carries the diagnostic ladder.

### Version 1.0.1

- Improvement: `known-build-gotchas` now covers why a template change can appear to have no effect — `.imod` packaging runs off the `.csproj` compilation, so a non-C# change may not trigger it; use `dotnet build --no-incremental` to force it.
- New Feature: Added `module-svg-icon` skill that crafts a house-style SVG icon for a module from a supplied description and applies it via the module's `.application.config`, regenerated into `.imodspec` by the Software Factory.
- Improvement: `intent-module-orchestrator` substantially expanded — the `"model"` metadata bridge for reading the originating designer element, `TryGetTypeName` for optional integration, three further registration-request types with the host's lifetime mapping, and the two-tier rule for when an `.imodspec` dependency is actually required.
- Improvement: `file-builder-expert` now frames metadata as having two uses — your own cross-step state, and reading the designer model the host template already attached — instead of only the former.
- Improvement: Every skill's `description` frontmatter rewritten to a strict Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES` contract, so an AI harness can route to the right skill from that line alone, without opening its body.
- Fixed: `module-docs` and `module-svg-icon` used a `description` value spanning multiple physical lines, which this project's frontmatter parser silently drops to an empty value — both rewritten as single-line quoted strings.
- New Feature: Added `add-module-migration` skill for authoring Version, On-Install, and On-Uninstall migrations that programmatically update a consumer's already-installed metadata.
- Fixed: Bundled skill files now generate flat under `Skills/` instead of a `Skills/{SkillName}/` subfolder; a Version Migration bundled with this update moves already-installed applications onto the flat layout, including ones with multiple root folders (e.g. `.agents`, `.claude`).
- New Feature: Added `module-element-icons`, covering icons on stereotype definitions and on custom element types — distinct from the module's own package icon, which `module-svg-icon` continues to own.
- Fixed: `module-versioning` said "never hand-edit `.imodspec`", but the manifest is merged rather than regenerated — it now names the six Software-Factory-owned elements and where each is actually set, leaving the rest hand-authored by design.
- Fixed: `module-docs` routed `<summary>`/`<description>` edits to `.imodspec` where they are silently discarded; both now point at the Application Settings page, and the unachievable "expand to two sentences" option is gone since one string fills both fields.
- Improvement: `add-designer-extension` and `add-association-type` now carry the `Include in Module` and `Reference in Designer` rules inline where a stereotype or designer element is introduced, with `module-building-strategies` covering the `Include in Module` half at its stereotype decision point.

### Version 1.0.0

- New Feature: Bundles a curated set of AI agent skills and module-building instructions into the consuming repo's `.agents/` folder, kept up to date on every install or update.
