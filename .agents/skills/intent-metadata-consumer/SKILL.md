---
name: intent-metadata-consumer
description: "Read Intent Architect designer metadata — stereotypes, properties, models — inside a template or factory extension using generated typed accessors, never raw string lookups. USE ONLY WHEN a template needs to branch on or read a stereotype/property/model value to drive its generated output. DO NOT USE FOR emitting the C# builder statements themselves (see file-builder-expert), cross-module DI/wiring (see intent-module-orchestrator), or finding out how to install/reference a designer module and reach its models in the first place (see intent-modelers-integration, if available in your environment). REQUIRES the relevant typed extension methods (*StereotypeExtensions.cs) already generated for the stereotype in question."
argument-hint: "[model type] [stereotype name] [target builder action]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.IntentMetadataConsumer_SkillMd_Agents
contentHash: 4DCDD7766B89CF44CFA65E1FBA77E1A8CAF1E3AA1D538346947E2697549ED236
---
# Intent Metadata Consumer

> [!TIP]
> **Read more if you want to know about** typed extension wrapper code patterns, stereotype GUID lookups, enum helpers, or metadata navigation API:
> *   [Metadata Cheatsheet](./resources/metadata-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Typed Accessors:** Use generated typed extension methods for stereotype access (e.g. `*StereotypeExtensions.cs`).
2. **Access Wrapper Properties:** Access properties through typed wrapper methods, never via raw property name strings.
3. **Null Guards:** Guard optional accessors with null-conditional operators or `TryGet` patterns.
4. **Enum Helpers:** Use `.AsEnum()` or the `.IsX()` boolean helpers (avoid raw string comparisons on `.Value`).
5. **Guid Resolution:** If a typed extension doesn't exist, resolve by `DefinitionId` (GUID) rather than display name.
6. **Primitive Checks:** Use `TypeCheckExtensions` (e.g., `IsStringType()`, `IsGuidType()`) for primitive metadata check.

## Must Nots

1. Never call `model.GetStereotype("StereotypeName")` when a typed extension method exists.
2. Never call `.GetProperty("PropertyName")` with a string literal for properties that are surfaced by generated wrappers.
3. Never branch on `.Value` of a stereotype option property using raw string comparison.
4. Never compose multi-stereotype LINQ queries using only string-based `HasStereotype` predicates when typed helpers are available.
5. Never skip the null guard on an optional stereotype accessor.
6. Never introduce display-name string lookups as a fallback when a `DefinitionId`-based lookup is available.
