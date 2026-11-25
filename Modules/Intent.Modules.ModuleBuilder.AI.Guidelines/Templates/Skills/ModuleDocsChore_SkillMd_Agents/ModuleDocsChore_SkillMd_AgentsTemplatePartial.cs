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

namespace Intent.Modules.ModuleBuilder.AI.Guidelines.Templates.Skills.ModuleDocsChore_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleDocsChore_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Guidelines.Skills.ModuleDocsChore_SkillMd_Agents";

        internal const string SkillName = "module-docs-chore";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleDocsChore_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;

            // Fully qualified on purpose: everything outside this constructor body is template-managed,
            // so a `using` for the Settings namespace would not survive regeneration.
            var aiGuidelinesSettings = Intent.Modules.ModuleBuilder.AI.Guidelines.Settings.ModuleSettingsExtensions
              .GetAIGuidelinesSettings(ExecutionContext.Settings);
            var maintainReadme = aiGuidelinesSettings.MaintainModuleREADME();
            var maintainIcon = aiGuidelinesSettings.MaintainModuleIcon();
            var maintainReleaseNotes = aiGuidelinesSettings.MaintainReleaseNotes();

            // Formats (release-notes entries, README headings, summary length, tags, cross-module links) are owned
            // by Intent Architect's built-in `module-docs` skill. This skill keeps only what that one does not
            // say: when docs are due in the workflow, which artifact carries what, and this environment's choices.
            // On, release notes are a requirement for every module changed: the agent turns on the module's own
            // Include Release Notes and lets the Software Factory create the file. Off, it writes only where the
            // module already opted in. Entry format stays with module-docs either way. The Software Factory
            // creates release-notes.md once (OnceOff) and does not recreate a deleted one. A ticked box with no
            // file only happens when someone deleted it, so the agent asks the developer (developer ruling).
            var releaseNotesRow = maintainReleaseNotes
              ? "| `release-notes.md` | **Required for every module you change: this environment maintains release notes.** If the module's `Include Release Notes` is unticked, tick it (in the Module Builder designer, set `Include Release Notes` on the package's `Module Settings` stereotype with `run_designer_script`) and run the Software Factory, which creates the file. Then add an entry for the change, following the built-in `module-docs` skill. If `Include Release Notes` is already ticked but the file is missing, it was deleted outside the normal flow: ask the developer what to do, and do not create or restore it yourself. |"
              : "| `release-notes.md` | An entry for the change, following the built-in `module-docs` skill, **only if the module's `Include Release Notes` is already ticked**. Otherwise leave release notes alone, and do not tick it. |";

            var releaseNotesChecklistItem = maintainReleaseNotes
              ? "- [ ] Release notes maintained (required): `Include Release Notes` ticked, `release-notes.md` present, and an entry for the change following `module-docs`"
              : "- [ ] `release-notes.md` entry added only if `Include Release Notes` was already ticked, following `module-docs`";

            var readmeRow = maintainReadme
              ? "| `docs/README.md` | The section the change affects. **Create one if the module has none**: this environment maintains module READMEs. |"
              : "| `docs/README.md` | Not maintained in this environment. Leave an existing one alone unless the developer asks, and do not create one. |";

            var iconRow = maintainIcon
              ? "| Module icon | Only when the module has none; never overwrite an existing icon. Describe it from the module's `.imodspec` summary and tags, or its `CONTEXT.md` Purpose, and craft it with `module-svg-icon`. If that skill is unavailable, leave the module without an icon. |"
              : "| Module icon | Not maintained in this environment. Leave an existing icon alone, and do not create one. |";

            var readmeChecklistItem = maintainReadme
              ? "\n- [ ] `docs/README.md` reflects the change, created if the module had none"
              : "";

            var iconChecklistItem = maintainIcon
              ? "\n- [ ] Module icon created only if missing, existing icons left untouched"
              : "";

            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
              .FromMarkdown($$""""""
          ---
          name: {{SkillName}}
          description: "The module-building workflow's documentation chore: decide which docs an observable change needs (release notes, README, .imodspec metadata, icon) and write them in the same turn. For format, load the built-in module-docs skill. USE ONLY WHEN a module change alters anything observable — a new/removed template, setting, stereotype, config default, or behavioural fix. DO NOT USE FOR internal refactors with no observable effect, or for bumping the module's version number itself (see module-version-increment). REQUIRES the observable change already implemented or decided."
          keywords: [documentation, release-notes, readme, imodspec, icon, chore, upkeep]
          template-id: {{TemplateId}}
          ---

          # Skill: module-docs-chore

          Documentation is part of the change, not a follow-up task. This skill says **when** a module's
          documentation is due and **which artifact** carries what.

          For the format and rules of release-notes.md, docs/README.md and module metadata, load the built-in `module-docs` skill (in Intent Architect it is built in; from an external harness, load it with the Intent Architect MCP `use_skill` tool). Where this skill and `module-docs` overlap, `module-docs` wins.

          ## When Docs Are Due

          Anything a consumer of the module can observe:

          - A new or removed template, or a change in the shape of generated output
          - A new module setting, stereotype, or designer element
          - A new configuration option, or a changed default
          - A behavioural fix that changes what the module produces

          Write it **in the same turn as the change**. Internal refactoring with no observable effect on
          generated output needs nothing.

          `module-docs` calls itself optional, run only when a maintainer asks. In the module-building workflow
          it is not optional: an observable change is that request.

          ## Where It Sits In The Workflow

          - **Phase 3, Implement** — alongside each observable change, as it is made.
          - **Phase 4, Close Out** — after the version is confirmed (`module-version-increment`) and the
          dependencies are audited (`module-dependency-audit`), so entries go under the version that ships.
          Cover the whole version line: earlier changes under the same unreleased version that were never
          written up go into the entry they belong with, not one bullet per change.

          ## Which Artifact Carries What

          | Artifact | What goes in |
          |---|---|
          | Module metadata: summary, description, tags | Kept accurate as behaviour changes; replace placeholder filler when you notice it. The author is copied from sibling modules' manifests, or asked for, never invented. |
          {{releaseNotesRow}}
          {{readmeRow}}
          {{iconRow}}

          ## Checklist

          - [ ] Written in the same turn as the observable change, and the whole version line covered
          - [ ] Every artifact follows `module-docs`
          {{releaseNotesChecklistItem}}{{readmeChecklistItem}}{{iconChecklistItem}}
          - [ ] Author copied from sibling modules or asked for, never invented
          """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
