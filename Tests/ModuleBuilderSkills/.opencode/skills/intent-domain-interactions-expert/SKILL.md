---
name: intent-domain-interactions-expert
description: "Implement an IInteractionStrategy that turns a designer-modelled interaction into its generated C# handler body. USE ONLY WHEN a modelled interaction (e.g. a command/event handler action) needs its C# implementation generated via a matched strategy. DO NOT USE FOR translating a Mapping element specifically (see intent-mapping-architect) or cross-module DI wiring (see intent-module-orchestrator). REQUIRES the strategy registered in a factory extension's OnBeforeTemplateRegistrations, never a template constructor."
argument-hint: "[handler template id or role] [interaction kind]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.IntentDomainInteractionsExpert_SkillMd_Agents
contentHash: F2C3BE52B3058CF772525EEF910AF4FDA0E224AB44C0DB42E7CE5212C3C7BB44
---
# Intent Domain Interactions Expert

> [!TIP]
> **Read more if you want to know about** built-in interaction strategies, strategy registration, mapping code snippets, or execution phases:
> *   [Interactions Cheatsheet](./resources/interactions-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Implement `IInteractionStrategy`:** Expose `IsMatch(IElement interaction)` and `ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interaction)`.
2. **Early Registration:** Call `InteractionStrategyProvider.Instance.Register(strategy, priority)` in a factory extension's `OnBeforeTemplateRegistrations` (never inside constructors). When several strategies match, the lowest priority wins; the default is 0, so use a negative priority to override a built-in strategy for the same interaction.
3. **Cheap Match:** Keep `IsMatch` cheap and side-effect-free: the typed predicate for the association end (e.g. `IsPerformInvocationTargetEndModel()`) plus a check on the target element's type, the way Intent's own strategies do.
4. **Phased Statements:** Emit statements via `method.AddStatement(ExecutionPhases.X, ...)` / `method.AddStatements(ExecutionPhases.X, ...)`. The phases, in order, are `Validation`, `Retrieval`, `BusinessLogic`, `Persistence`, `IntegrationEvents`, `Response`; a statement added without a phase counts as `BusinessLogic`.
5. **Mapping Resolution:** Use `method.GetMappingManager()` (one manager per method, shared by every strategy on it) and add resolvers up-front inside `ImplementInteraction`.
6. **Register Type Sources:** Call `template.AddTypeSource(...)` for templates producing referenced types.
7. **Share Entities:** Read and add loaded entities through `method.TrackedEntities()` (keyed by interaction id) instead of re-querying.
8. **Raise The Floor:** The whole `Intent.Modules.Common.CSharp.Interactions` namespace needs `Intent.Common.CSharp` 3.9.0+, and `Register(strategy, priority)` needs 3.9.1+. Module Builder's default floor is 3.8.0, so raise the `Intent.Common.CSharp` `<dependency>` in the `.imodspec` and the `Intent.Modules.Common.CSharp` package reference in the `.csproj` to 3.9.1 or later. Raise `Intent.Modules.Common` (`.csproj`) and the `Intent.Common` `<dependency>` (`.imodspec`) with it, to at least the `Intent.Modules.Common` version that package depends on: 3.7.3 for 3.9.1, more for later versions (the error names it). From `Intent.Modules.Common` 3.9.0 the `Intent.SoftwareFactory.SDK` reference must rise too (3.10.0 for 3.9.0 and 3.9.1, more for later versions); the next `NU1605` names it. Module Builder writes 3.7.2, so otherwise restore fails with `NU1605: Detected package downgrade` (see the `known-build-gotchas` instructions). The designer packages the interaction predicates come from need more: `Intent.Modules.Modelers.Services` 4.x and `Intent.Modules.Modelers.Services.CQRS` 6.x depend on `Intent.Modules.Common` 3.8.0, so raise it to 3.8.0 once you reference either, and `Intent.Modules.Modelers.Services.DomainInteractions` 2.2.0 and later also depends on `Intent.SoftwareFactory.SDK` 3.9.0, so raise that package reference to 3.9.0 (Intent Architect 4.5.0 or later).

## Must Nots

1. Never register a strategy from inside a template constructor.
2. Never hardcode the handler's method name or signature inside the strategy.
3. Never call `template.CSharpFile.AfterBuild` from your strategy's own code. An SDK mapping you resolve to, such as `ObjectUpdateMapping`, may schedule its own helper that way; that is fine.
4. Never branch on stereotype string names inside `IsMatch` (use typed predicates).
5. Never rely on a phase-less `method.AddStatement(...)` when multiple strategies attach to the same handler: it is appended wherever the body ends at that moment, then treated as `BusinessLogic` when other strategies insert around it.
6. Never invent a phase name: a string outside `ExecutionPhases` is inserted at the top of the method body.
7. Never modify the handler's class structure (e.g. constructor/fields) directly from a strategy; use `@class.InjectService(...)` instead.
