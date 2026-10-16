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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.ModuleVersioning_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleVersioning_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.ModuleVersioning_SkillMd_Agents";

        internal const string SkillName = "module-versioning";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleVersioning_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Set an Intent Architect module's version via the Module Builder model property, then propagate it to dependents (architecture templates, other modules). USE ONLY WHEN asked to set, release, publish, or bump a module to a specific, already-decided version string. DO NOT USE FOR deciding whether or when a task should bump a version, or which component to bump (see module-version-increment) — this skill only executes a version already supplied. REQUIRES the target version string supplied by the caller; it does not decide or validate what it should be."
                    argument-hint: "[new version, e.g. 1.3.0 or 1.3.0-pre.1]"
                    keywords: [version, versioning, release, imodspec, module settings]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-versioning

                    ## What The Software Factory Writes

                    `.imodspec` is **not** a fully generated file. The template reads the existing file, writes a defined
                    set of elements, and leaves everything else exactly as it found it. So "never hand-edit
                    `.imodspec`" is wrong — part of that file is hand-authored by design. Know which part before you
                    touch it.

                    These elements are **written on every run.** A hand edit to one of these is overwritten on the next Software Factory run,
                    without an error:

                    | Element | What the Software Factory does | Where to change it instead |
                    |---|---|---|
                    | `<id>` | Set to the package name | The Module Builder package name |
                    | `<version>` | Raised to the designer's value **only when that value is higher**; a lower value is skipped, never written | Module Builder designer → `Module Settings` → `Version` |
                    | `<summary>` and `<description>` | Both set to the application's description, whenever one is set | The **Application Settings page**, or `update_application_settings` with its `description` parameter. Both receive the *same single string* — there is no long-form field to expand into. |
                    | `<iconUrl>` | Set from the application's icon | `.application.config`'s root `<icon>`/`<iconType>`, by script — see `module-svg-icon` |
                    | `<supportedClientVersions>` | If present, a fixed list of older bounds is advanced (`[4.1.0,` becomes `[5.1.0-a,`); any other range is left as it is | A rewritten range is the mechanism working. Hand-edit the range only when its floor needs to move — see `module-version-increment` |
                    | `<templates>`, `<factoryExtensions>`, `<decorators>` | Entries synced with the modelled elements: added, updated, and removed when the element is deleted | The modelled templates, factory extensions and decorators |
                    | `<metadata>` install entries | Synced with the packages included in the module | The package's `Module Settings` (`Include in Module`, `Reference in`) |
                    | `<moduleSettings>`, `<moduleSettingsExtensions>` | Each group's children wiped and rebuilt | Modelled settings elements |
                    | `<migrations>` | Deleted and rebuilt wholesale | Modelled Migration elements |

                    These are **added, never removed.** The Software Factory adds them when they are missing, then leaves them:

                    | Element | What the Software Factory does |
                    |---|---|
                    | `<dependency>` entries | Added when missing. An existing entry's `version` is raised according to the `Dependency version overwrite behavior` application setting (default *If newer*). No entry is ever removed — see `module-dependency-audit`. |
                    | `<releaseNotes>` | Added as `release-notes.md` when absent and `Module Settings` → `Include Release Notes` is ticked |
                    | `<projectUrl>` | Added from `Module Settings` → `Project URL` when absent and that property has a value |
                    | `<tags>` | An empty element added when absent — the value is never written |

                    These are **hand-maintained.** The template never writes a value for these, so hand-editing is the *only* way
                    to set them, and the edit survives: `<tags>`, `<authors>` and `<files>` (scaffolded once when the file
                    is first created), `<interoperability>`, `<designers>`, and `<projectUrl>` / `<releaseNotes>` once
                    present.

                    ## How to Set It
                    1. On the module's package, in the Module Builder designer:

                       `pkg.ensureStereotype("Module Settings").setProperty("Version", "<supplied version>")`
                       (or the designer UI). Use the version exactly as supplied. Persist it with `saveOnSuccess`
                       (or the designer's own save) so the change survives immediately — but this writes only the
                       designer model, not `.imodspec`.

                    2. Run the Software Factory to regenerate `.imodspec`'s `<version>`. `saveOnSuccess` never
                       substitutes for this step: `<version>` is generated output, and only a Software Factory run
                       produces it — the model and the manifest are two different files, and only one of the two
                       write paths reaches the manifest.
                    3. Confirm via `get_file_diffs` that the version line changed, and that anything else in the
                       `.imodspec` diff is expected — other synced elements (see the table above) can move in the same run.

                    > **`<version>` is written only when the designer's value sorts strictly higher than the one on disk.**
                    > A lower value is skipped in silence — no error, no warning. Only `<version>` is skipped: the rest of
                    > the file is still written, so if nothing else changed, nothing is staged. That asymmetry is the
                    > downgrade guard `module-version-increment` describes, and it is the one case where editing the
                    > `<version>` line by hand is correct: drop it to a safe value, then set the version you actually want
                    > in the designer and regenerate forward.

                    ## Propagating the Change
                    - **Architecture Templates** referencing this module — set the new version on the `Component Module`
                      element in the Application Template Builder designer (its `Module Settings` stereotype → `Version`),
                      then regenerate. `metadata.iatspec` is generated from that element, so a hand edit there is
                      overwritten. Do this only once the new version is actually published — confirm with
                      `run_solution_query` → `searchAvailableModules("<module id>", { includePrerelease: true })`, never guess.
                    - **Other modules depending on it** — update their `<dependency id="..." version="...">` entry. If that
                      entry comes from a metadata-only install, update the install instead and regenerate; the Software
                      Factory raises the entry. That is propagation of a version you already know about; confirming the
                      dependency *list itself* is right is a separate close-out check — see `module-dependency-audit`.
                    - **NuGet package alignment** — keep `.csproj` package versions in step with the module version
                    to avoid `NU1605` (see known-build-gotchas).

                    ## After Setting the Version
                    Bring `release-notes.md` and other version-dependent documentation into step with the new version —
                    this skill sets the version, it doesn't own the documentation that mirrors it. If the `module-docs-chore`
                    skill is available in your environment, it covers this; `module-docs` is the opt-in pass for bringing
                    documentation up to a canonical format, not the routine follow-up.

                    ## Verification Checklist
                    - [ ] Set via `Module Settings → Version` — not by hand-editing `.imodspec`'s `<version>` (except to clear a downgrade)
                    - [ ] Software Factory run; `.imodspec` confirmed to match, no stray diffs
                    - [ ] Dependents checked/updated if they pin this module's version
                    - [ ] Release notes and other version-dependent documentation brought into step
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
