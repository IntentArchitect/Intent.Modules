using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentDomainInteractionsExpert.SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentDomainInteractionsExpert.SkillMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "")
                .FromMarkdown(""""""
---
name: intent-domain-interactions-expert
description: "Implement an IInteractionStrategy that turns a designer-modelled interaction into its generated C# handler body. USE ONLY WHEN a modelled interaction (e.g. a command/event handler action) needs its C# implementation generated via a matched strategy. DO NOT USE FOR translating a Mapping element specifically (see intent-mapping-architect) or cross-module DI wiring (see intent-module-orchestrator). REQUIRES the strategy registered in a factory extension's OnBeforeTemplateRegistrations, never a template constructor."
argument-hint: "[handler template id or role] [interaction kind]"
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

"""""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
