---
name: intent-metadata-consumer
description: "Read Intent Architect metadata (stereotypes, attributes, models) to drive code generation."
argument-hint: "[model type] [stereotype name] [target builder action]"
contentHash: 4CEAF043CAB760F33FD106F327C12E34BBE66D69274E450FF39E5781E4DB50D8
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
