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
