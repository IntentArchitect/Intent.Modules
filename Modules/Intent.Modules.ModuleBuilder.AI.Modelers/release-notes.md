### Version 1.0.0-pre.1

- Fixed: `intent-modelers-integration` implied `modules.config` should be hand-written alongside the `.csproj` and `.imodspec` entries. It is never hand-edited — it records what is installed — so the skill now says to install the designer module with `Install Designer Metadata` ticked and nothing else, which is what produces the `includeAssets="none"` entry.

### Version 1.0.0-pre.0

- New Feature: Adds the `intent-modelers-integration` skill — an Intent Architect AI agent skill documenting how to wire a module against Intent's built-in Domain, Services, Eventing, and User Interface designers. Bundles `SKILL.md` (the four-identities rule, the three-file wiring obligation, and the traps that are wrong-by-default) plus one full reference resource file per designer (`domain.md`, `services.md`, `eventing.md`, `user-interface.md`) covering install identity, entry points, every designer element/association/stereotype, extension modules with their own full install identity, and a worked snippet — plus `integration-recipe.md`, the exact three-file wiring recipe lifted verbatim from `Intent.Modules.Metadata.RDBMS`.
