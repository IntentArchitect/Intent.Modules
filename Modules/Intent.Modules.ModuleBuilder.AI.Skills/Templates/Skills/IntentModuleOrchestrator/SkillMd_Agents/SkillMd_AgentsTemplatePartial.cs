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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentModuleOrchestrator.SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentModuleOrchestrator.SkillMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "")
                .FromMarkdown(""""""
---
name: intent-module-orchestrator
description: Wire cross-module logic, DI/appsettings events, priority bands, and template lookups.
argument-hint: "[event type | factory extension scenario] [target template role or id]"
---

# Intent Module Orchestrator

> [!TIP]
> **Read more if you want to know about** priority bands, broker filtering, Startup DSL, DI/Config registration events, or cross-template lookups:
> *   [Orchestration Cheatsheet](./resources/orchestration-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts
1. **Safe Resolution:** Prefer Role-based lookup via `TemplateRoles.*`. Guard templates (use `?.` or check null) before accessing `CSharpFile`.
2. **Callbacks:** Use `TryGetModel<T>` to verify model shape; use `TryGetTemplate(...)` for multi-fallback chains.
3. **DI/Config Events:** Publish `ContainerRegistrationRequest` / `AppSettingRegistrationRequest` from `OnBeforeTemplateExecution` (never from `OnAfterTemplateRegistrations`).
4. **Dependencies:** Declare with `.HasDependency(...)`. Set `ForConcern` for specific startup target files.
5. **Priority Bands:** Pass explicit priorities to `AfterBuild` (e.g. 0=Core, 100=Enrichment, 500=Extension, 1000=Final).
6. **Startup DSL:** Use `IAppStartupFile` DSL (e.g., `AddServiceConfiguration`) over manual `FindMethod` edits.
7. **Broker Filter:** Filter event subscriptions using `.FilterMessagesForThisMessageBroker(ExecutionContext, ...)` (pass `ExecutionContext`).
8. **NuGet Packaging:** Dispatch modules do not need to install target NuGets if the core module already does.

## Must Nots
1. Never use Regex to modify `Program.cs` or `appsettings.json`.
2. Never publish registration requests from `OnAfterTemplateRegistrations`.
3. Never call `AddAppConfigurationLambda("UseEndpoints", ...)`; use `AddUseEndpointsStatement` instead.

"""""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
