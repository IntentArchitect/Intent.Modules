# Intent.ModuleBuilder.AI.Modelers

This module does not generate application code. It drops one AI agent skill into the repo it is installed in: `intent-modelers-integration` — the skill an agent needs before it can wire a module against one of Intent Architect's own built-in designers (Domain, Services, Eventing, User Interface). It generates no C# and has no designer model dependency — the files it writes are always overwritten on install/update.

## The gap this fills

An agent told to "generate a validator for every entity in the Domain designer" has to grep the repo to discover facts that are wrong by default if guessed: the NuGet package is `Intent.Modules.Modelers.Domain`, but the `.imodspec` dependency id is `Intent.Modelers.Domain` — different strings for the same thing; the namespace is `Intent.Modelers.Domain.Api`, with no `Intent.Modules.Modelers.Domain.Api` NuGet package despite appearances; the dependency must be declared in three separate files at matching versions; and the entry point is `_metadataManager.Domain(application).GetClassModels()`. After installing this skill, that is a lookup in a table rather than an archaeology expedition.

The existing `intent-metadata-consumer` skill (bundled by `Intent.ModuleBuilder.AI.Skills`) starts one step later — it assumes you already hold a model instance (e.g. a `ClassModel`) and teaches you how to read stereotypes off it. This skill stops exactly where that one starts: once you hold the model instance, hand off to `intent-metadata-consumer`.

## What This Module Generates

- `.agents/skills/intent-modelers-integration/SKILL.md` — the cross-cutting Musts/Must Nots: the four identities (NuGet PackageId / Intent module id / API namespace / assembly) that must never be interchanged, the three-file version-matched wiring obligation, and the traps every designer's extension modules independently get wrong.
- `.agents/skills/intent-modelers-integration/resources/integration-recipe.md` — the exact three-file wiring recipe, snippets lifted verbatim from `Intent.Modules.Metadata.RDBMS`; the shared model surface every designer model returns (the SDK's `IElement`, `ITypeReference`, `IStereotype` and mapping interfaces, plus `Common.Types`' `FolderModel`/`EnumModel`/`TypeDefinitionModel`); and the `Api/` naming convention table for answering "what's in this designer" for one this skill doesn't cover.
- `.agents/skills/intent-modelers-integration/resources/domain.md`, `.../resources/services.md`, `.../resources/eventing.md`, `.../resources/user-interface.md` — one full reference per base designer: install identity, entry points, every element/association/stereotype it defines, a worked snippet, and a §9 "Model API reference" listing every public type and member of the base designer as C# stubs, plus the name collisions to alias.
- `.agents/skills/intent-modelers-integration/resources/domain-extensions.md`, `.../resources/services-extensions.md`, `.../resources/user-interface-extensions.md` — the separately installed extension modules of each designer (e.g. Domain Events, Services CQRS, UI Core): each module's own full install identity, the model API reference for everything it adds, and the collisions between them. Eventing has no extension modules, so it has no extensions file.

The model API reference exists so an agent does not have to reflect over a designer assembly to find out what a `ClassModel`, `DTOModel` or `ComponentModel` exposes. It is extracted mechanically from each designer module's `Api/` source, so when a designer's API changes, regenerate it rather than editing it by hand (see `CONTEXT.md`).

## Bundled Output

The content is bundled once, in this module's own source, and always overwritten on every install or update. Local edits to the bundled content are not a supported use case.

## Related Modules

- **Intent.ModuleBuilder.AI.Skills** — bundles the complementary `intent-metadata-consumer` skill, which this module's `SKILL.md` names by description, and vice versa. There is deliberately no package dependency between the two bundles — each must remain independently installable.
