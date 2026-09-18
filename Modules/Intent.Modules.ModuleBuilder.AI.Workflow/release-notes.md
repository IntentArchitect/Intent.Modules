### Version 1.0.0

- New Feature: Bundles the module-building workflow skills — context capture, version increments, documentation upkeep and dependency audit — into the consuming repo's agent skills folder.
- New Feature: Adds an agent gate that deterministically denies edits to Software-Factory-owned files, illegal module-version changes, hand-edits to `modules.config`, and `.imodspec` edits to `<summary>`/`<description>`.
- New Feature: The gate warns at close-out — without blocking — when a changed module's tags aren't lowercase, its `docs/README.md` is missing, its `CONTEXT.md` wasn't touched alongside the change, or its release-notes heading still carries a `-pre` suffix.
- New Feature: The gate ships as a dependency-free .NET 10 file-based app, with a self-contained copy nested inside each harness's own folder, so no harness reaches into another's directory and there is no install step or tool manifest.
- New Feature: Generates hook configuration for Claude Code, Codex, Cursor, Kiro and OpenCode, each in that harness's own native format and wired to the gate copy beside it.
- New Feature: `.claude/settings.json` is merged rather than owned — missing entries are added, existing ones are never overwritten, and a file that does not parse is left untouched with a warning.
- New Feature: Adds `Install Agent Gate Hooks`, `Use Pre-Release Versions`, `Maintain Module README`, `Maintain Module Icon`, `Maintain Module Context` and `Maintain Release Notes` settings.
- Fixed: Cursor's write guard runs on `preToolUse`, which can deny the write, instead of `afterFileEdit`, which fires once the write has already landed and so could never block anything.
- Fixed: The gate no longer denies edits to `release-notes.md`, which the Software Factory seeds once and never rewrites — it was blocking the documentation chore this module itself mandates.
- Fixed: The gate no longer denies edits to a scaffolded `*TemplatePartial.cs`/`*TemplateRegistration.cs`, where hand-authoring the ignored method bodies is the only way to write a template at all.
- Fixed: Every generated hook command wraps `dotnet run` so a build failure in the gate's own source still blocks — `dotnet run` exits 1 on a broken build rather than 2, and Cursor treats any code other than exactly 0 or 2 as an allow.
- Fixed: A harness folder the module does not recognise no longer throws out of hook-config generation and fails the consumer's whole Software Factory run.
- Fixed: The `Intent.Common` dependency floor was 3.7.2 while the module compiles against 3.11.4 — a gap that would only surface once a consumer's install resolved the lower version.
- Fixed: `module-version-increment`'s already-moved check is now a universal gate that both Phase 2 and Phase 4 route through, ending the phantom version bump on every follow-up instruction.
- Fixed: `module-docs-chore` now names the Application Settings page as where a module's summary and description are edited — edits made in `.imodspec` are silently reverted by the Software Factory.
- Improvement: `module-version-increment`'s major/minor/patch rubric judges impact on the user's experience rather than on the module's own source, and the workflow skills' descriptions follow the Capability / `USE ONLY WHEN` / `DO NOT USE FOR` / `REQUIRES` contract for more reliable harness routing.
- Improvement: Added `docs/README.md`, covering the four-phase workflow, the module settings, and the agent gate.
