---
name: intent-domain-interactions-expert
description: "Implement an IInteractionStrategy that turns a designer-modelled interaction into its generated C# handler body. USE ONLY WHEN a modelled interaction (e.g. a command/event handler action) needs its C# implementation generated via a matched strategy. DO NOT USE FOR translating a Mapping element specifically (see intent-mapping-architect) or cross-module DI wiring (see intent-module-orchestrator). REQUIRES the strategy registered in a factory extension's OnBeforeTemplateRegistrations, never a template constructor."
argument-hint: "[handler template id or role] [interaction kind]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.IntentDomainInteractionsExpert_SkillMd_Agents
contentHash: 8C0A5CEA0627F45211FE169B8D6C1BB5B9B3FBF15E42DC8FAA3BCD02C61AF9E8
---
# Intent Domain Interactions Expert

> [!TIP]
> **Read more if you want to know about** built-in interaction strategies, strategy registration, mapping code snippets, or execution phases:
> *   [Interactions Cheatsheet](./resources/interactions-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Implement `IInteractionStrategy`:** Expose `IsMatch(IElement interaction)` and `ImplementInteraction(...)`.
2. **Early Registration:** Register strategies in factory extensions' `OnBeforeTemplateRegistrations` (never inside constructors).
3. **Cheap Match:** Keep `IsMatch` cheap and side-effect-free (check typed target end models).
4. **Phased Statements:** Emit statements via `method.AddStatement(...)` with explicit `ExecutionPhases` (e.g. `BusinessLogic`, `Return`).
5. **Mapping Resolution:** Use `method.GetMappingManager()` and add resolvers up-front inside `ImplementInteraction`.
6. **Register Type Sources:** Call `template.AddTypeSource(...)` for templates producing referenced types.

## Must Nots

1. Never register a strategy from inside a template constructor.
2. Never hardcode the handler's method name or signature inside the strategy.
3. Never call `template.CSharpFile.AfterBuild` from inside a strategy.
4. Never branch on stereotype string names inside `IsMatch` (use typed predicates).
5. Never call `method.AddStatement(...)` without a phase when multiple strategies attach to the same handler.
6. Never modify the handler's class structure (e.g. constructor/fields) directly from a strategy; use `@class.InjectService(...)` instead.
