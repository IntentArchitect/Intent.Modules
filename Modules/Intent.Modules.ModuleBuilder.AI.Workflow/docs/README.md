# Intent.ModuleBuilder.AI.Workflow

This module does not generate application code. It drops a set of AI agent workflow skills and a standing instruction file into the repo it is installed in, so that an AI working in that repo follows the same lifecycle when building or changing an Intent Architect module. Where `Intent.ModuleBuilder.AI.Skills` covers the _craft_ of module building, this module covers the _process_ around it — what to read before starting, when to move a version, and what has to be true before a change is finished.

Unlike the skills module, some of this module's content is settings-driven: the same skill renders differently depending on how the consuming application is configured.

## What This Module Generates

- `.agents/instructions/module-building-workflow.instructions.md` — the standing four-phase workflow every module task moves through, naming which skill to load in each phase.
- `.agents/skills/<skill-name>/SKILL.md` — one per bundled skill: `module-context-capture`, `module-version-increment`, `module-docs-chore`, `module-dependency-audit`.
- `.agents/hooks/*.cs`, `.agents/hooks/Directory.Build.props`, `.agents/hooks/CLAUDE_SETUP.md`, per-harness hook configs (`.codex/hooks.json`, `.kiro/hooks/intent-agent-gate.json`, `.cursor/hooks.json`), and `.opencode/plugins/intent-agent-gate.ts` — only when `Install Agent Gate Hooks` is on, and only for whichever harness folders already exist in the repo. The gate is a dependency-free .NET 10 file-based app (`dotnet run gate.cs`, no install step, no dotnet-tool manifest) that denies edits to Software-Factory-owned generated files, `modules.config`, and non-`<tags>`/`<dependency>`/downgrade `.imodspec` edits; denies an illegal module version change before it is written; and warns — without blocking — when a module's version, tags, `docs/README.md`, `CONTEXT.md`, or release-notes heading fall behind a change. Every harness blocks via exit code 2 and allows via 0 (OpenCode's plugin invokes the gate directly and throws on any non-zero exit, achieving the same contract without needing JSON-configured hooks at all). Claude Code needs one manual step: `.claude/settings.json` is a shared file this module does not safely auto-merge into, so `CLAUDE_SETUP.md` documents the one-time block to paste in instead.

## The Four-Phase Workflow

The instruction file is the entry point; the skills are loaded from it as each phase comes up.

| Phase                 | What it covers                                                                                                                              | Skill                      |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------- |
| 1 — Understand        | Read each affected module's `CONTEXT.md` and surface any conflict before proceeding                                                         | `module-context-capture`   |
| 2 — Classify and plan | Classify the change, then run the version gate for every affected module **up front** — move it if published, leave it if already in flight | `module-version-increment` |
| 3 — Implement         | Record decisions and update documentation as you go, not at the end                                                                         | `module-docs-chore`        |
| 4 — Close out         | Version, then dependencies, then documentation, then context — in that order                                                                | all four                   |

Phase 4 runs dependencies before documentation deliberately: a dependency fix is itself an observable change, so the documentation step immediately after has to describe it.

## Module Settings

| Setting                  | Default | Effect                                                                                                                                                                           |
| ------------------------ | ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Use Pre-release Versions | Off     | Switches `module-version-increment` between standard semantic versions and `-pre.#` iteration.                                                                                   |
| Maintain Module README   | Off     | When on, `module-docs-chore` treats `docs/README.md` as an artifact to create and maintain.                                                                                      |
| Maintain Module Icon     | Off     | When on, `module-docs-chore` creates a module's SVG icon when it has none. An existing icon is never overwritten.                                                                |
| Maintain Module Context  | Off     | When on, `module-context-capture` also creates a `CONTEXT.md` for a module that has none, once its first durable decision lands. Off keeps the read-and-maintain-only behaviour. |
| Maintain Release Notes   | Off     | When on, `module-docs-chore` creates a `release-notes.md` for a module whose `Include Release Notes` is ticked but whose file is missing. Off reports that mismatch instead of fixing it. |
| Install Agent Gate Hooks | Off    | Generates the gate scripts (`.agents/hooks/*.cs`) plus a hook config for each harness folder already present in the repo (`.codex`, `.kiro`, `.cursor`) and `CLAUDE_SETUP.md`'s manual-paste instructions for Claude Code. Off means none of it is generated. |

Each setting only ever widens what the generated guidance covers. Left at their defaults, the skills maintain what already exists and introduce nothing.

Release notes are governed by two things, and both apply. Whether a module keeps them at all is decided per module by the `Include Release Notes` checkbox on that module's own `Module Settings` in the Module Builder designer — no application setting overrides it. `Maintain Release Notes` only decides what happens to a module that has ticked that box but has no `release-notes.md`: off reports the mismatch, on creates the file.

## Bundled Skill Output

Each skill's content is bundled in this module's own source and always overwritten on every install or update. Local edits to bundled skill content are not a supported use case — standardization across consuming repos is the goal, not per-repo customization.

## Related Modules

- **`Intent.ModuleBuilder.AI.Skills`** — the craft half of the same pairing. Deliberately **not** a package dependency: each bundle must stay independently installable, so where these skills reference one of its skills they do so by name only, degrading gracefully when it is absent.
- **`Intent.ModuleBuilder.AI.Modelers`** and **`Intent.ModuleBuilder.AI.SDD`** — sibling bundles, likewise with no dependency in either direction.
