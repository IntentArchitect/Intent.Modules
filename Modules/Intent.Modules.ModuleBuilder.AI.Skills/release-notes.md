### Version 1.0.0

- New Feature: Bundles the `file-builder-expert`, `intent-mapping-architect`, `intent-metadata-consumer`, `intent-domain-interactions-expert`, `intent-module-orchestrator`, `add-association-type`, `add-designer-extension`, `module-building-strategies`, and `module-docs` AI agent skills to `.agents/skills/` in the consuming repo, always overwritten on install/update.
- New Feature: Bundles a reusable module-building instructions file to `.agents/instructions/module-building-primer.md`, sourced from the accumulated C# and exception-handling conventions used across the Intent Architect module ecosystem.
- New Feature: Idempotently inserts an `@`-style import line for the bundled instructions file into an existing root `AGENTS.md`, without duplicating the line on repeated installs or touching the rest of the file.
