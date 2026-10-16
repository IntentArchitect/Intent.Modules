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

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Skills.ModuleDependencyAudit_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleDependencyAudit_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Skills.ModuleDependencyAudit_SkillMd_Agents";

        internal const string SkillName = "module-dependency-audit";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleDependencyAudit_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Verify a module's .imodspec dependencies match what its code and templates actually reference, and supply the ones the Software Factory did not detect. USE ONLY WHEN closing out a module change — this is Phase 4 step 2 of the module-building workflow, run after the version is confirmed and before documentation. DO NOT USE FOR bumping the module's own version (see module-version-increment), or for editing modules.config, which is never hand-edited. REQUIRES the module's implementation complete and its build green."
                    keywords: [dependencies, imodspec, install, close-out, verification, module]
                    template-id: {{TemplateId}}
                    ---

                    # Skill: module-dependency-audit

                    ## Why This Is A Gate, Not An Errand

                    **A missing dependency compiles perfectly and fails at install** — in a consumer's application, not yours.
                    The module builds, the Software Factory runs, the templates emit what you expected, and nothing goes wrong
                    until someone else installs it and the type it needs is not there.

                    That is the exact failure class the workflow's *"compiling is not working"* rule exists for. A green build
                    is not evidence here, so no earlier phase catches it. This check is why the manifest is trustworthy.

                    ## What The Software Factory Already Does

                    The Software Factory adds `<dependency>` entries to `.imodspec` from exactly these sources:

                    | Source | Version it writes |
                    |---|---|
                    | Every module installed **metadata-only** in the module's own application — designers, template outputs and factory extensions all off | The installed version |
                    | Dependencies raised by Module Builder's own templates while generating — a C# template adds `Intent.Common.CSharp`; a template whose model type or designer comes from another module adds that module | The version the template carries |
                    | `Intent.Common` and `Intent.Common.Types`, always; `Intent.OutputManager.RoslynWeaver` when the module has a C# template | Hard-coded floors |

                    The hard-coded floors are low: Module Builder writes `Intent.Common` 3.7.2 and `Intent.Common.CSharp`
                    3.8.0. A module that uses a newer API needs a higher version than the floor, and nothing raises it for you.

                    An entry is added when missing, and an existing entry's version is raised according to the
                    `Dependency version overwrite behavior` application setting: *If newer* (the default) raises it only when
                    the source carries a higher version, *Always* overwrites it with whatever the source carries (which can
                    lower a version you raised by hand), *Never* fills only an empty version. **No entry is ever removed.**

                    **It is a heuristic, and the source acknowledges as much.** It infers from what is *installed* rather than
                    from what the code actually *references*, so the gap is systematic rather than random:

                    - A `PackageReference` in the `.csproj` adds nothing. A module whose code uses the `Intent.Modules.Common.CSharp`
                    package still needs an `Intent.Common.CSharp` dependency at the version that API arrived in.
                    - A type reached through a transitively-available assembly — present at compile time because something else
                    pulled it in, never declared as this module's own dependency.
                    - A dependency that was real when a template was written and is now only referenced from a code path the
                    generator no longer visits — its entry stays, because nothing is removed.

                    ## The Audit

                    1. **Collect what the module actually references.** Its `.csproj` `PackageReference` entries, the
                    `Intent.*` namespaces its templates and factory extensions import, and any designer metadata it reads.
                    2. **Read the generated `<dependencies>`** in the module's `.imodspec`.
                    3. **Compare, and care about one direction.** Something referenced but *not* declared is the defect — that
                    is the install-time failure. An entry that looks surplus is usually a legitimate transitive or
                    event-raised dependency; leave it unless you can show nothing needs it.
                    4. **Check the versions, not just the ids.** For each `Intent.*` package the module's code uses, the
                    dependency's version must be at least the version that introduced the API it calls. A hard-coded floor
                    below that compiles and fails at install time, with a missing method or type.
                    5. **Report what you checked**, not just that it passed. Name the references you compared against.

                    Then **re-run the audit after every Software Factory run on the module**, not only once at close-out. Each run
                    can add or raise entries, and under *Always* it can lower one.

                    ## Fixing What You Find

                    There are two supported fixes. Pick one per dependency, and say which one you picked and why.

                    | Fix | How | Trade-off |
                    |---|---|---|
                    | **Metadata-only install** | Install the module into the module's own application with designers, template outputs and factory extensions off (`install_or_update_modules` with `installDesigners`, `installTemplateOutputs` and `enableFactoryExtensions` set to `false`), then regenerate | The entry is computed for you and stays in step with the installed version. It is pinned to whatever version is installed, and keeps being raised every time the install is updated — even when the module does not need the newer version. |
                    | **Hand-added `<dependency>`** | Add `<dependency id="..." version="..." />` to `.imodspec` by hand, or raise an existing entry's `version` | You choose the minimum version the module actually requires. Entries are *added if absent and never pruned*, so it survives regeneration (under *If newer*, a version you raised stays unless a source carries a higher one), but nothing keeps it current: you raise it yourself when the module starts needing a newer version. |

                    Both are supported outcomes, not hacks. Prefer the metadata-only install when the module reads that
                    module's designer metadata anyway; prefer a hand-added entry when it only needs a minimum version of a
                    package such as `Intent.Common.CSharp`.

                    Match the id form exactly — the Intent module id, which is not the NuGet package id (the
                    `Intent.Modules.Common.CSharp` package is the `Intent.Common.CSharp` module) — and the version to what the
                    module actually requires. When you raise a dependency's version, raise the matching `.csproj`
                    `PackageReference` with it, so the code compiles against the version the manifest promises.

                    ## When The Install Or Uninstall Cannot Be Done

                    If a fix needs a module installed or uninstalled and that is not possible here — the install fails, the
                    module is not on any reachable feed, or no tool is available to do it — stop and report it to the
                    developer: which dependency, which fix it needs, and what failed. Hand-add the `<dependency>` instead only
                    when you can justify the id and version from what the code references. Never edit `modules.config` to
                    make an install look done.

                    ## Two Things This Skill Does Not Touch

                    - **`modules.config`** — never hand-edited, for any reason. It records what is installed; a bad edit
                    corrupts the application's module state. Change it by installing or updating through Intent Architect.
                    - **`supportedClientVersions`** — scaffolded once and then advanced automatically as the client SDK moves
                    forward. A range being rewritten to a newer one is the mechanism working, not drift. Look at it only if a
                    Software Factory run actually complains about client-version compatibility.

                    ## Checklist

                    - [ ] Referenced set collected from `.csproj`, template imports, and designer metadata reads
                    - [ ] Generated `<dependencies>` compared against it; every referenced-but-undeclared entry resolved
                    - [ ] Versions checked against the APIs used, not left at the hard-coded floors
                    - [ ] Each fix is a metadata-only install or a hand-added entry, with the choice stated
                    - [ ] Audit re-run after the last Software Factory run
                    - [ ] Surplus-looking entries left alone unless shown to be unused
                    - [ ] `modules.config` untouched; `supportedClientVersions` untouched
                    - [ ] What was compared is stated in the close-out, not just that the check ran
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}