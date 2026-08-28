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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.ModuleElementIcons_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleElementIcons_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.ModuleElementIcons_SkillMd_Agents";

        internal const string SkillName = "module-element-icons";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleElementIcons_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Set the icon on a stereotype definition or on a custom element type in a designer you own. USE ONLY WHEN a stereotype or element type you define should show a distinct icon in the Intent Architect UI. DO NOT USE FOR a module's own package icon (see module-svg-icon) — different file, different mechanism, and the two are routinely confused. REQUIRES the stereotype definition or element type already modelled in the Module Builder designer."
                    keywords: [icon, stereotype, element type, designer, module builder, ui]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-element-icons

                    Two different things in a designer can carry an icon, and they are configured in two different places.
                    Neither is the module's own package icon — see the boundary below before going further.

                    | What you are icon-ing | Where the icon lives |
                    |---|---|
                    | A **stereotype** | Fields on the stereotype definition itself: an `icon` (a type + source pair), plus `displayIcon` and `displayIconFunction` |
                    | A **custom element type** (`Element Settings` / `Core Type`) | The `Settings` stereotype applied to it: `Icon`, `Expanded Icon`, `Icon Function` |

                    ===

                    ## Stereotype Icons

                    A stereotype's icon is **structural** — part of the stereotype definition, not a property you apply to it.
                    It is a type/source pair: a `FontAwesome` type takes an icon name as its source (`cogs`, `database`); a
                    `UrlImagePath` type takes a data URI.

                    Two companions control *when* it shows:

                    - **`displayIcon`** — whether the badge renders on elements carrying the stereotype at all.
                    - **`displayIconFunction`** — a script for conditional display, when the badge should only appear for
                    certain property values. Prefer plain `displayIcon` unless the condition is real; a function that always
                    returns the same answer is just a slower checkbox.

                    A stereotype badge competes for space with the element's own icon and its name. Reach for one when the
                    stereotype changes how the element should be *read* at a glance — not merely to show it was applied.

                    ## Element Type Icons

                    For an element type you define — an `Element Settings` or `Core Type` node — the icon is a property on the
                    `Settings` stereotype that node carries:

                    | Property | Purpose |
                    |---|---|
                    | `Icon` | The element's icon in the model tree and on diagrams |
                    | `Expanded Icon` | Optional alternate shown when the node is expanded. Leave unset unless expanding genuinely changes what the node represents. |
                    | `Icon Function` | A script returning an icon per element, for types whose icon depends on their own state. **Returning null falls back to the default icon** — that is the designed escape, not a failure. |

                    `Icon` and `Icon Function` are alternatives. Set the static `Icon` unless the icon genuinely varies per
                    element; a function is re-evaluated as the model changes and is harder to reason about.

                    ## Setting The Value

                    Use the designer's icon picker. The stored value is a serialized blob — for an image icon it embeds the
                    whole base64 payload — and **you should not hand-author or copy it between elements.** Transcribing it
                    risks a silent corruption that renders as a blank icon with no error, and it wastes context for no gain.
                    This mirrors the rule `module-svg-icon` applies to the package icon for the same reason.

                    If you need the same icon on several element types, set it on each through the picker rather than copying
                    the value across.

                    ===

                    ## The Boundary With `module-svg-icon`

                    A module's **own package icon** — the one shown in the module registry — is a different thing entirely. It
                    lives in the module's `.application.config` as a root-level `icon`/`iconType` pair, and the `.imodspec`
                    `iconUrl` is regenerated from it. That is `module-svg-icon`'s job — and it goes through a script not because
                    nothing else can set it, but because the base64 payload would otherwise pass through your context.

                    The confusion is easy to hit because a designer's `.application.config` also contains many per-element icon
                    entries. **Those are not the package icon**, and the package icon is not one of them.

                    | If you want… | Use |
                    |---|---|
                    | the icon for a stereotype or element type you defined | this skill |
                    | the icon for the module itself, in the registry | `module-svg-icon` |

                    ## Checklist

                    - [ ] Icon set on the right thing — stereotype definition vs element type's `Settings` stereotype
                    - [ ] `Icon` used rather than `Icon Function` unless the icon genuinely varies per element
                    - [ ] `Expanded Icon` left unset unless expanding changes what the node represents
                    - [ ] Value set through the designer's picker — never hand-authored or copied between elements
                    - [ ] The module's own package icon left alone (that is `module-svg-icon`)
                    - [ ] Software Factory run and the designer inspected to confirm the icon actually renders
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}