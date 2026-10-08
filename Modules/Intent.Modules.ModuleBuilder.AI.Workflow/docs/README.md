# Intent.ModuleBuilder.AI.Workflow

This module does not generate application code. It drops a set of AI agent workflow skills and a standing instruction file into the repo it is installed in, so that an AI working in that repo follows the same lifecycle when building or changing an Intent Architect module. Where `Intent.ModuleBuilder.AI.Skills` covers the _craft_ of module building, this module covers the _process_ around it — what to read before starting, when to move a version, and what has to be true before a change is finished.

Unlike the skills module, some of this module's content is settings-driven: the same skill renders differently depending on how the consuming application is configured.

## What This Module Generates

- `module-building-workflow.instructions.md` — the standing four-phase workflow every module task moves through, naming which skill to load in each phase. It is generated into every folder of the consuming application that carries an `AI.Context.Instructions` output anchor. With the agent gate installed, it also explains what to do when a gate hook itself fails.
- `<skill-name>/SKILL.md` — one per bundled skill, into every `AI.Context.Skills` anchor: `module-context-capture`, `module-version-increment`, `module-docs-chore`, `module-dependency-audit`.
- The agent gate — only when `Install Agent Gate Hooks` is on (see below).

### How the instructions reach each harness

No harness is given a root-level instruction file (`AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md` and the like). Each harness loads the workflow from its own folder instead, which means the consuming application has to model that folder as an output anchor in its Codebase Structure:

| Harness | Instructions | Skills |
| ------- | ------------ | ------ |
| Claude Code | `.claude/rules/` (an `AI.Context.Instructions` anchor) | `.claude/skills/` |
| GitHub Copilot CLI | `.github/instructions/`; it also reads `.claude/rules/` | `.agents/skills/`, `.claude/skills/` or `.github/skills/` |
| Cursor | `.cursor/rules/` — written as `.mdc` with `alwaysApply: true`, the only form Cursor's rules system reads | `.agents/skills/` or `.cursor/skills/` |
| Kiro | `.kiro/steering/` (`inclusion: always`) | `.kiro/skills/` — Kiro reads no other skills folder |
| OpenCode | `.opencode/instructions/`, listed in `.opencode/opencode.json`'s `instructions` (merged, not owned) | `.agents/skills/`, `.opencode/skills/` or `.claude/skills/` |
| Codex | `.codex/config.toml`, as `developer_instructions` (merged, not owned) | `.agents/skills/` |

Codex has no instructions folder: outside `AGENTS.md`, its only always-on route is the `developer_instructions` key, which takes the text itself. So the workflow is written into a marked block at the top of `.codex/config.toml`, rewritten on every run and never touching the rest of the file. Codex reads a project's `config.toml` only once the project is **trusted** — the same condition its project hooks need. If the file already sets `developer_instructions` itself, it is left alone with a warning, because TOML rejects a key defined twice.

### The agent gate

Generated only when `Install Agent Gate Hooks` is on, and only into the harness folders already present in the repo. Each harness gets its **own self-contained copy**, so nothing reaches across into another harness's directory:

| Harness folder | Hook configuration | Gate |
| -------------- | ------------------ | ---- |
| `.claude` | `settings.json` (merged, not owned) | `.claude/hooks/gate/` |
| `.codex` | `hooks.json` (merged, not owned) | `.codex/hooks/gate/` |
| `.cursor` | `hooks.json` (merged, not owned) | `.cursor/hooks/gate/` |
| `.kiro` | `hooks/intent-agent-gate.json` | `.kiro/hooks/gate/` |
| `.opencode` | `plugins/intent-agent-gate.ts` | `.opencode/hooks/gate/` |
| `.github` | `hooks/intent-agent-gate.json` (GitHub Copilot CLI) | `.github/hooks/gate/` |

The gate is a single, dependency-free .NET 10 file-based app (`dotnet run gate.cs` — no install step, no dotnet-tool manifest). It protects Intent Architect's own **metadata**, denies an illegal module version change before it is written, and warns — without blocking — when a module's version, tags, `docs/README.md`, `CONTEXT.md`, or release-notes heading fall behind a change.

| Denied — use the Intent MCP server or a skill | Allowed |
| --- | --- |
| `*.application.config` | generated output of any kind |
| `*.application.managed-files.xml` | `release-notes.md`, `CONTEXT.md`, `docs/README.md` |
| `*.application.output.config.xml` | scaffolded `*TemplatePartial.cs` / `*TemplateRegistration.cs` |
| `modules.config` | `.csproj`, including package versions |
| anything under `Intent.Metadata/` or `.intent/` | `.claude/settings.json` |

Generated **output** is deliberately not covered. Editing it is usually merely futile — the next run overwrites it — whereas editing metadata corrupts state no regeneration puts right. Hand-editing generated output is also frequently the intended workflow, so blocking it obstructs far more often than it helps.

`.imodspec` is a deliberate middle case:

- `<summary>` and `<description>` are denied, because the Software Factory silently discards edits to them.
- `<tags>` and `<dependency>` entries stay hand-editable.
- `<version>` is hand-editable, but held to the same rules as setting the version through the designer: a second bump in one line of work, a bare release version under `Use Pre-release Versions`, or an invalid version is denied, with a pointer to `module-version-increment`. A valid change is allowed, and Claude Code, Codex and Kiro are reminded to load that skill. A downgrade stays allowed, because the Software Factory will not write a lower version from the designer.

Every harness blocks via exit code 2 and allows via 0. OpenCode's plugin invokes the gate directly and throws on any non-zero exit, reaching the same contract without a JSON hook config at all. `.claude/settings.json`, `.codex/hooks.json` and `.cursor/hooks.json` are merged rather than owned outright, because the developer and other tools register their own hooks in them. Missing entries are added, an entry this module generated earlier is upgraded in place, anyone else's is never touched, and a file that does not parse is left untouched with a warning.

Copilot CLI and Cursor also run the hooks in `.claude/settings.json`. When Copilot runs Claude Code's copy of the gate, that copy recognises it and steps aside silently, so Copilot's own hook makes the one decision.

#### Requirements

- **Any .NET 10 SDK** on `PATH`. The gate does not run on .NET 9 or earlier, including when a `global.json` pins an older SDK.
- **On Windows, Git Bash for Claude Code.** Claude Code runs hooks in Git Bash, which ships with Git for Windows. Codex and Copilot get a PowerShell form of each command automatically, and Cursor and Kiro get a form that works in every shell they use — Kiro CLI runs it in `cmd.exe`, the Kiro IDE in PowerShell.
- **Kiro CLI 2.27.1 or later, with the v3 engine (`--v3`),** for hooks in headless runs. Earlier versions load `.kiro/hooks` only in the IDE and in interactive v3 sessions.
- **Launch the harness from the application's output folder.** Every harness loads its hook configuration only from the folder it is started in.

#### Observing the gate

Set `INTENT_GATE_LOG` to a file path and the gate appends one JSON line per run: the command, its exit code and duration, what it read and wrote, the harness-related environment variables (values only for non-secret ones) and the chain of processes that started it. Each harness logs hooks differently, if at all, so this is the one view that is the same everywhere. Logging never changes a verdict.

#### When the gate cannot run

A guard hook that cannot run — no SDK, an SDK that is too old, a gate that has not been built yet — **blocks** rather than letting the action through. Only Kiro lets it through, because it has no way to block on a failed hook. Close-out never blocks, so a broken gate cannot trap an agent in a loop of turns. The generated workflow instructions tell the agent what each of these errors means and which fix to ask the user for, so a setup problem is reported as one rather than retried or worked around.

## Known Limitations

- **Shell commands are not guarded.** The gate sees file-edit tools and the Intent MCP server's `run_designer_script`, not what a shell command writes. An agent that edits metadata or a version through PowerShell or bash goes around it. This is accepted: the gate is a guard rail for an agent following the workflow, not a sandbox.
- **Verified on Windows only.** Every harness run behind this module's evaluation was on Windows. The POSIX command forms follow each harness's documentation but have not been run on macOS or Linux.
- **Cursor runs the gate twice** when its third-party hooks are on, because it also loads `.claude/settings.json` and its payload cannot be told apart from Claude Code's. Both copies reach the same verdict, so the cost is latency, not correctness. Cursor's CLI was not run; its wiring is checked statically.
- **Kiro's write guard is verified on Kiro CLI 2.28 only** (headless, v3). The Kiro IDE and interactive sessions follow the same hook file but were not run.
- **No reminder on an allowed version edit for Copilot and OpenCode.** Neither offers a pre-tool hook a way to pass the agent a note when the action is allowed; an invalid version edit is still denied, with the reason.
- **OpenCode's close-out warning goes to the person, not the agent** — OpenCode's log and a toast — like Claude Code's. Putting it into the session would start another turn.
- **Copilot loads the workflow twice** in a repository with both `.github/instructions` and `.claude/rules`, because it reads both folders.
- **The harness folders must be modelled.** Instructions and skills only reach a harness whose folders are output anchors in the consuming application's Codebase Structure. An application created from a template without them gets nothing for that harness.

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
| Use Pre-release Versions | Off     | Switches `module-version-increment` between standard semantic versions and `-pre.#` iteration. With the agent gate installed, the gate also denies moving to a new version without a `-pre.#` suffix (promoting a pre-release to its own final version stays allowed). |
| Maintain Module README   | Off     | When on, `module-docs-chore` treats `docs/README.md` as an artifact to create and maintain.                                                                                      |
| Maintain Module Icon     | Off     | When on, `module-docs-chore` creates a module's SVG icon when it has none. An existing icon is never overwritten.                                                                |
| Maintain Module Context  | Off     | When on, `module-context-capture` also creates a `CONTEXT.md` for a module that has none, once its first durable decision lands. Off keeps the read-and-maintain-only behaviour. |
| Maintain Release Notes   | Off     | When on, `module-docs-chore` creates a `release-notes.md` for a module whose `Include Release Notes` is ticked but whose file is missing. Off reports that mismatch instead of fixing it. |
| Install Agent Gate Hooks | Off    | Generates a nested copy of the gate plus the matching hook configuration into each harness folder already present in the repo — `.claude`, `.codex`, `.cursor`, `.kiro`, `.opencode`, `.github`. Off generates nothing, and warns once if harness folders are present. |

Each setting only ever widens what the generated guidance covers. Left at their defaults, the skills maintain what already exists and introduce nothing.

Release notes are governed by two things, and both apply. Whether a module keeps them at all is decided per module by the `Include Release Notes` checkbox on that module's own `Module Settings` in the Module Builder designer — no application setting overrides it. `Maintain Release Notes` only decides what happens to a module that has ticked that box but has no `release-notes.md`: off reports the mismatch, on creates the file.

## Bundled Skill Output

Each skill's content is bundled in this module's own source and always overwritten on every install or update. Local edits to bundled skill content are not a supported use case — standardization across consuming repos is the goal, not per-repo customization.

## Related Modules

- **`Intent.ModuleBuilder.AI.Skills`** — the craft half of the same pairing. Deliberately **not** a package dependency: each bundle must stay independently installable, so where these skills reference one of its skills they do so by name only, degrading gracefully when it is absent.
- **`Intent.ModuleBuilder.AI.Modelers`** — a sibling bundle, likewise with no dependency in either direction.
