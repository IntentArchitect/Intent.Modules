# Intent.ModuleBuilder.AI.Skills

This module does not generate application code. It drops a fixed set of AI agent skill files and
instruction files into the repo it is installed in, so that an AI working in that repo has the
knowledge to build Intent Architect modules correctly. It generates no C# and has no designer
model dependency — every file it writes is always overwritten on install/update.

## What This Module Generates

- `.agents/skills/<skill-name>/SKILL.md` — one per bundled skill (`file-builder-expert`,
  `intent-mapping-architect`, `intent-metadata-consumer`, `intent-domain-interactions-expert`,
  `intent-module-orchestrator`, `add-association-type`, `add-designer-extension`,
  `module-building-strategies`, `module-docs`), plus each skill's `resources/` files where present.
- `.agents/instructions/exception-guidelines.instructions.md` — how to choose the right
  exception type (`FriendlyException`, `ElementException`, or a plain developer exception) when
  writing module code.
- `.agents/instructions/known-build-gotchas.instructions.md` — recurring template-authoring
  pitfalls (NuGet dependency registration, `SingleFileListModel` filename stability, naming
  conflicts, package version drift) and how to avoid them.

## Bundled Skill Output

Each skill's content is bundled once, in this module's own source, and always overwritten on
every install or update. Local edits to bundled skill content are not a supported use case —
standardization across consuming repos is the goal, not per-repo customization.

## Related Modules

None. This module has no dependency on any designer, domain, or output-target module beyond the
Intent Architect core infrastructure required to run any Software Factory template.
