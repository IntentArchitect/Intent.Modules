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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentMappingArchitect.SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentMappingArchitect.SkillMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "")
                .FromMarkdown(""""""
---
name: intent-mapping-architect
description: Translate designer-defined advanced mappings into recursive C# Builder statements.
argument-hint: "[mapping type] [source model] [target model]"
---

# Intent Mapping Architect

> [!TIP]
> **Read more if you want to know about** mapping managers, custom type resolvers, traversal pipelines, or API signatures:
> *   [Mapping Cheatsheet](./resources/mapping-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts
1. **Replacement Resolution:** Configure replacements via `SetFromReplacement(...)`/`SetToReplacement(...)` and generate statements via MappingManager APIs (`GenerateUpdateStatements(...)` etc.).
2. **Path Resolution:** Resolve assignments via mapping element IDs and paths, never by property names.
3. **Node Differentiation:** Explicitly handle Terminal Mappings (leaf) vs Object Mappings (nested/collection).
4. **Metadata Preservation:** Custom mapping statements in recursive generation must implement `IHasMapping`.
5. **Mapping Options:** Honor designer model `MappingOptions`: Null-Safe (emit guards) and Validate All.
6. **Custom Resolver Registration:** Register custom `IMappingTypeResolver` implementations with explicit priority.
7. **Inherit CSharpMappingBase:** Inherit custom mappings from `CSharpMappingBase` to leverage recursive tree traversal.

## Must Nots
1. Never hardcode property-to-property assignments.
2. Never bypass MappingManager-driven replacement resolution.
3. Never treat object/collection mappings as scalar terminals.
4. Never create mapping statement types that omit `IHasMapping` inside recursive mapping flow.
5. Never ignore `MappingOptions` Null-Safe and Validate All settings.
6. Never place transaction/retrieval/persistence orchestration in this skill (belongs to `intent-domain-interactions-expert`).

"""""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
