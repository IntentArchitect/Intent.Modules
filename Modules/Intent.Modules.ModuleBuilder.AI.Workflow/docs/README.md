# Intent.ModuleBuilder.AI.Workflow

This module does not generate application code. It drops a set of AI agent workflow skills and a standing
instruction file into the repo it is installed in, so that an AI working in that repo follows the same
lifecycle when building or changing an Intent Architect module. Where `Intent.ModuleBuilder.AI.Skills`
covers the *craft* of module building, this module covers the *process* around it — what to read before
starting, when to move a version, and what has to be true before a change is finished.

Unlike the skills module, some of this module's content is settings-driven: the same skill renders
differently depending on how the consuming application is configured.

## What This Module Generates

- `.agents/instructions/module-building-workflow.instructions.md` — the standing four-phase workflow every
  module task moves through, naming which skill to load in each phase.
- `.agents/skills/<skill-name>/SKILL.md` — one per bundled skill: `module-context-capture`,
  `module-version-increment`, `module-docs-chore`, `module-dependency-audit`.

## The Four-Phase Workflow

The instruction file is the entry point; the skills are loaded from it as each phase comes up.

| Phase | What it covers | Skill |
|---|---|---|
| 1 — Understand | Read each affected module's `CONTEXT.md` and surface any conflict before proceeding | `module-context-capture` |
| 2 — Classify and plan | Classify the change and increment every affected module's version **up front** | `module-version-increment` |
| 3 — Implement | Record decisions and update documentation as you go, not at the end | `module-docs-chore` |
| 4 — Close out | Version, then dependencies, then documentation, then context — in that order | all four |

Phase 4 runs dependencies before documentation deliberately: a dependency fix is itself an observable
change, so the documentation step immediately after has to describe it.

## Module Settings

| Setting | Default | Effect |
|---|---|---|
| Use Pre-release Versions | Off | Switches `module-version-increment` between standard semantic versions and `-pre.#` iteration. |
| Maintain Module README | Off | When on, `module-docs-chore` treats `docs/README.md` as an artifact to create and maintain. |
| Maintain Module Icon | Off | When on, `module-docs-chore` creates a module's SVG icon when it has none. An existing icon is never overwritten. |
| Maintain Module Context | Off | When on, `module-context-capture` also creates a `CONTEXT.md` for a module that has none, once its first durable decision lands. Off keeps the read-and-maintain-only behaviour. |

Each setting only ever widens what the generated guidance covers. Left at their defaults, the skills
maintain what already exists and introduce nothing.

Release-notes maintenance is not settings-gated. It is governed per module by the `Include Release Notes`
checkbox on that module's own `Module Settings` in the Module Builder designer.

## Bundled Skill Output

Each skill's content is bundled in this module's own source and always overwritten on every install or
update. Local edits to bundled skill content are not a supported use case — standardization across
consuming repos is the goal, not per-repo customization.

## Related Modules

- **`Intent.ModuleBuilder.AI.Skills`** — the craft half of the same pairing. Deliberately **not** a package
  dependency: each bundle must stay independently installable, so where these skills reference one of its
  skills they do so by name only, degrading gracefully when it is absent.
- **`Intent.ModuleBuilder.AI.Modelers`** and **`Intent.ModuleBuilder.AI.SDD`** — sibling bundles, likewise
  with no dependency in either direction.
