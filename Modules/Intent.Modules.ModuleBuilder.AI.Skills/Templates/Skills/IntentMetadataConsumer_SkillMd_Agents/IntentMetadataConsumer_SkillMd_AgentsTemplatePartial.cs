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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentMetadataConsumer_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentMetadataConsumer_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentMetadataConsumer_SkillMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentMetadataConsumer_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "intent-metadata-consumer")
                .FromMarkdown(""""""
                    ---
                    name: intent-metadata-consumer
                    description: "Read Intent Architect designer metadata — stereotypes, properties, models — inside a template or factory extension using generated typed accessors, never raw string lookups. USE ONLY WHEN a template needs to branch on or read a stereotype/property/model value to drive its generated output. DO NOT USE FOR emitting the C# builder statements themselves (see file-builder-expert) or cross-module DI/wiring (see intent-module-orchestrator). REQUIRES the relevant typed extension methods (*StereotypeExtensions.cs) already generated for the stereotype in question."
                    argument-hint: "[model type] [stereotype name] [target builder action]"
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

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
