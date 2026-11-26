### Version 1.0.2

- Improvement: `module-docs-chore` now says only when a module's docs are due, where that sits in the workflow and which artifact carries what, and tells the agent to load the built-in `module-docs` skill for formats. Its description opens by naming it the module-building workflow's documentation chore.
- Improvement: `Maintain Release Notes` now makes release notes a requirement for every module changed: the agent ticks a module's `Include Release Notes` and lets the Software Factory create the file. Off, entries go only to modules that already ticked it.
- Improvement: `Maintain Release Notes` was recreated with a new id, so it starts off after updating. If you had it on in 1.0.1, switch it on again.
- Improvement: `module-svg-icon` ships a `scripts/set-module-icon.cs` beside its `SKILL.md`, a .NET file-based app run with `dotnet run` on any OS, needing the .NET 10 SDK. The agent points it at the module's folder to set the package icon, to check the result with `--verify` (hashes compared across the SVG, the stored icon, the `.imodspec` and the built `.imod`), and to export the current icon with `--export` so it can be rendered. If the script cannot run, because no .NET 10 SDK is installed or a `global.json` pins an older one, the skill tells the agent what the message means and to stop rather than set the icon another way. The script prints only lengths, hashes and fixed messages, never the icon's value, and the skill no longer sends the agent into the file where Intent Architect stores the icon.
- Improvement: The agent gate's release-notes and tag warnings now point at the `module-docs` skill.
- Improvement: `module-version-increment` now covers only whether a version moves and how far: the in-flight gate, choosing the component, and moving dependents at close-out. For setting the version, what the Software Factory writes to the `.imodspec`, the downgrade guard and the supported client range, it sends the agent to the built-in `module-versioning` skill.
- Improvement: The orchestration cheatsheet in `intent-module-orchestrator` no longer repeats the built-in `file-builder-expert` skill. Template lookups, the `"model"` metadata bridge and callback priorities are a pointer to it; request publishing, `EmitOrPublish`, the startup DSL, the role-string table and the cross-module interface-type rule stay.
- Improvement: `known-build-gotchas` and `exception-guidelines` now scope themselves to `**/*.cs` for Claude Code (`paths`), GitHub Copilot (`applyTo`) and Intent Architect (`appliesTo`). `exception-guidelines` also gains a description.
- Improvement: Requires `Intent.Common` 3.11.10.

### Version 1.0.1

- Improvement: The agent gate lets an agent resolve a merge or rebase conflict in Intent metadata, an `.imodspec` or a `modules.config` with its file-editing tools. It denies only a resolution that writes conflict markers back, and the workflow instructions say how to resolve such a conflict.
- Improvement: The close-out check runs about three times faster in large repositories, so it no longer times out.
- Fixed: The close-out check now attributes a new file whose name has non-ASCII characters to its module.

### Version 1.0.0

- New Feature: Guides AI agents through a four-phase module-building workflow with bundled skills and instructions, and an agent gate for Claude Code, Codex, GitHub Copilot CLI, Cursor, Kiro and OpenCode that blocks hand-edits to Intent Architect metadata and invalid version changes.
