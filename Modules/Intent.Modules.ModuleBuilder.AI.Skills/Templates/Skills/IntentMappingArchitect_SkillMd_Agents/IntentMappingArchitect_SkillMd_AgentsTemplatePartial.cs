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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentMappingArchitect_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentMappingArchitect_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentMappingArchitect_SkillMd_Agents";

        internal const string SkillName = "intent-mapping-architect";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentMappingArchitect_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Translate a designer-modelled advanced Mapping (Terminal or Object, with replacements and MappingOptions) into recursive C# via the MappingManager API. USE ONLY WHEN a template must generate the assignment/traversal statements for a designer-defined mapping between two models. DO NOT USE FOR hardcoded property-to-property assignments, or handler bodies for a modelled interaction that isn't a Mapping (see intent-domain-interactions-expert). REQUIRES the mapping already modelled in the designer and the template inheriting CSharpMappingBase."
                    argument-hint: "[mapping type] [source model] [target model]"
                    template-id: {{TemplateId}}
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
