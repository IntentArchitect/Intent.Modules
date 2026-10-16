---
name: intent-mapping-architect
description: "Translate a designer-modelled advanced Mapping (terminal and object nodes, with from/to replacements) into C# statements via CSharpClassMappingManager, IMappingTypeResolver and CSharpMappingBase. USE ONLY WHEN a template must generate the assignment/traversal statements for a designer-defined mapping between two models. DO NOT USE FOR hardcoded property-to-property assignments, or handler bodies for a modelled interaction that isn't a Mapping (see intent-domain-interactions-expert). REQUIRES the mapping already modelled in the designer and a C# file-builder template to emit the statements into."
argument-hint: "[mapping type] [source model] [target model]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.IntentMappingArchitect_SkillMd_Agents
contentHash: 45D61B7C9D5061C001C09FD1F2509E80E34761D235A081625B6358ADEDB69C34
---
# Intent Mapping Architect

> [!TIP]
> **Read more if you want to know about** mapping managers, custom type resolvers, traversal pipelines, or API signatures:
> *   [Mapping Cheatsheet](./resources/mapping-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Replacement Resolution:** Configure replacements via `SetFromReplacement(...)`/`SetToReplacement(...)` on a `CSharpClassMappingManager` (inside an interaction strategy, `method.GetMappingManager()`), then generate statements with `GenerateCreationStatement(...)` or `GenerateUpdateStatements(...)`.
2. **Path Resolution:** Resolve assignments via mapping element IDs and paths, never by property names.
3. **Node Differentiation:** A terminal node has a drawn mapped end (`MappingModel.Mapping != null`); an object node has `Mapping == null` and only `Children`. With no resolver match, the manager falls back to `DefaultCSharpMapping` for terminals and `MapChildrenMapping` for object nodes.
4. **Custom Mapping Types:** Implement `ICSharpMapping`, normally by inheriting `CSharpMappingBase`, which already carries `Children`, `Parent`, replacements and the path helpers.
5. **Null Handling:** Use the real members: `GetSourceStatement(true)`, the protected `GetSourcePathText(path, targetIsNullable)` and `GetNullableAwareInstantiation(...)`, plus `GetTargetStatement(true)` and `IfNotNullMapping` from Intent.Common.CSharp 3.9.2 (see the cheatsheet's "Null handling"). There are no "Null-Safe" or "Validate All" mapping options.
6. **Custom Resolvers:** Match on `MappingModel.MappingTypeId`, implement the two-argument `ResolveMappings(mappingModel, next)`, and hand anything you don't handle on with `return next?.Invoke(mappingModel);`. Register with `AddMappingResolver(resolver, priority)`: lower runs first, the default is 0, and a negative value runs ahead of resolvers added at the default.
7. **Raise The Floor:** The two-argument resolver needs `Intent.Common.CSharp` 3.9.2+ and the priority overload 3.9.1+; Module Builder's default floor is 3.8.0, so raise the `Intent.Common.CSharp` `<dependency>` in the `.imodspec` and the `Intent.Modules.Common.CSharp` package reference in the `.csproj` to 3.9.2 or later. Raise `Intent.Modules.Common` (`.csproj`) and the `Intent.Common` `<dependency>` (`.imodspec`) with it, to at least the `Intent.Modules.Common` version that package depends on: 3.7.3 for 3.9.2, more for later versions (the error names it). From `Intent.Modules.Common` 3.9.0 the `Intent.SoftwareFactory.SDK` reference must rise too (3.10.0 for 3.9.0 and 3.9.1, more for later versions); the next `NU1605` names it. Module Builder writes 3.7.2, so otherwise restore fails with `NU1605: Detected package downgrade` (see the `known-build-gotchas` instructions).

## Must Nots

1. Never hardcode property-to-property assignments.
2. Never bypass MappingManager-driven replacement resolution.
3. Never treat object/collection mappings as scalar terminals.
4. Never return `null` from a two-argument resolver for a node you don't handle: every later resolver then receives a null `next`, and one that calls it throws a `NullReferenceException`.
5. Never guard a resolver with `if (mappingModel.Mapping == null) return ...;`: object nodes have no `Mapping`, so the guard stops your resolver from ever handling them.
6. Never place transaction/retrieval/persistence orchestration in this skill (belongs to `intent-domain-interactions-expert`).
