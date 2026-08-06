# Intent.ModuleBuilding.AI.Primer

This module does not generate application code. It drops a fixed set of AI agent skill files and
an instructions file into the repo it is installed in, so that an AI working in that repo has the
knowledge to build Intent Architect modules correctly. It generates no C# and has no designer
model dependency — every file it writes is always overwritten on install/update.

## What This Module Generates

- `.agents/skills/<skill-name>/SKILL.md` — one per bundled skill (`file-builder-expert`,
  `intent-mapping-architect`, `intent-metadata-consumer`, `intent-domain-interactions-expert`,
  `intent-module-orchestrator`, `add-association-type`, `add-designer-extension`,
  `module-building-strategies`, `module-docs`), plus each skill's `resources/` files where present.
- `.agents/instructions/module-building-primer.md` — reusable C# and exception-handling
  conventions for writing Intent Architect module code.
- An `@.agents/instructions/module-building-primer.md` import line inserted into the repo's root
  `AGENTS.md`, if it exists.

## Bundled Skill Output

Each skill's content is bundled once, in this module's own source, and always overwritten on
every install or update. Local edits to bundled skill content are not a supported use case —
standardization across consuming repos is the goal, not per-repo customization.

## Root Instruction Import

The bundled `.agents/instructions/module-building-primer.md` file is not injected directly into
the consumer's own `AGENTS.md` — that file remains fully under the consumer's control. Instead,
this module checks whether `AGENTS.md` exists at the repo root and, if so, idempotently appends a
single `@.agents/instructions/module-building-primer.md` import line to it if the line is not
already present. Repeated installs never duplicate the line, and no other part of the consumer's
file is touched.

If `AGENTS.md` does not exist at the repo root, no file is created and no import line is inserted.

## Related Modules

None. This module has no dependency on any designer, domain, or output-target module beyond the
Intent Architect core infrastructure required to run any Software Factory template.
