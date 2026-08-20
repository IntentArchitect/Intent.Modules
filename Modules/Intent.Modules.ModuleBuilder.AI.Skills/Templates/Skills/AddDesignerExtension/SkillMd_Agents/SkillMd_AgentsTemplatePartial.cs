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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.AddDesignerExtension.SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.AddDesignerExtension.SkillMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "")
                .FromMarkdown(""""""
---
name: add-designer-extension
description: "Add a context-menu-driven element, association-creation, or mapping option to a foreign package, element, or association end via a packageExtension/elementExtension. USE ONLY WHEN extending an element/package/association end owned by another module, not one you own. DO NOT USE FOR defining a brand-new association type from scratch (see add-association-type) or building an architecture template's component picker (see architecture-templates). REQUIRES the foreign target's typeId and designer GUID already identified."
argument-hint: "[target element/package type name and what to add]"
---

# Add Designer Extension

> [!TIP]
> **Read more if you want to know about** designer extensions, package/element extensions, mapping options, or context menu items:
> *   [Conceptual Reference](./resources/designer-extensions-reference.md)
> *   [Workflow & Common Mistakes](./resources/designer-extension-workflow.md)
> *(To conserve tokens, avoid reading these files for simple or minor updates.)*

## Purpose

Extend an existing element, package, or association end from a foreign module by adding new context menu options (e.g. child element/association creation or mapping options).

## Musts
1. Identify foreign target `typeId` via MCP `find_designer_elements` or generated C# `SpecializationTypeId` constants.
2. Link `Designer Settings` to foreign designer GUID (Domain: `6ab29b31-27af-4f56-a67c-986d82097d63`, Services: `81104ae6-2bc5-4bae-b05a-f987b0372d81`).
3. For Element Extensions, apply `Type Reference Extension Settings` (`Mode = Inherit`) and `Extension Settings` stereotypes.
4. Add all context menu options under a `[context menu]` child element.
5. Set type reference of `Association Creation Option` to target end of association settings.
6. Verify via `get_designer_validation_errors` and generated `.designer.settings` file.

## Must Nots
1. Never guess or hardcode a type GUID from memory.
2. Never skip `Mode = Inherit` on `Type Reference Extension Settings` for element extensions.
3. Never edit `.designer.settings` directly (always change the Module Builder model).

"""""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
