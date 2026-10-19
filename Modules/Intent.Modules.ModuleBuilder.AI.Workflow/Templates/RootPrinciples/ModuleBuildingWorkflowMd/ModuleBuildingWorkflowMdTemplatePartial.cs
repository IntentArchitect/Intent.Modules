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

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.RootPrinciples.ModuleBuildingWorkflowMd
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ModuleBuildingWorkflowMdTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.RootPrinciples.ModuleBuildingWorkflowMd";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleBuildingWorkflowMdTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;

            // Fully qualified on purpose: everything outside this constructor body is template-managed,
            // so a `using` for the Settings namespace would not survive regeneration.
            var installAgentGateHooks = Intent.Modules.ModuleBuilder.AI.Workflow.Settings.ModuleSettingsExtensions
                .GetAIWorkflowSettings(ExecutionContext.Settings)
                .InstallAgentGateHooks();

            // When the gate's hook cannot run at all, the agent only ever sees dotnet's own error text,
            // which says nothing about the gate. Agents met it and kept retrying, or reported it as a
            // flaw in their own work; this maps each error to the fix the user has to make.
            var gateFailureSection = !installAgentGateHooks ? "" : """
                ===

                ## If An Agent Gate Hook Itself Fails

                This repository runs an agent gate from your harness's hooks: `dotnet run <harness folder>/hooks/gate/gate.cs`,
                such as `.claude/hooks/gate/gate.cs`. When that hook cannot run at all, the error comes from `dotnet`, not
                from the gate. It is a machine setup problem, not a problem with your work. Do not retry the hook, work
                around it, or treat it as a finding to fix. Tell the user which of these applies:

                | Error from the hook | Cause | Fix for the user |
                | --- | --- | --- |
                | `dotnet: command not found`, or `dotnet` is not recognized | No .NET SDK on `PATH` | Install the .NET 10 SDK |
                | `Couldn't find a project to run` | The active SDK is older than .NET 10, often because a `global.json` pins one | Install the .NET 10 SDK, or update `global.json` |
                | `A compatible .NET SDK was not found` | A `global.json` pins an SDK that is not installed | Install that SDK, or update `global.json` |
                | `An error occurred trying to start process`, naming the gate's own build output | The gate has not been built yet | Run `dotnet run <harness folder>/hooks/gate/gate.cs -- warm` from the project root |
                | `gate.cs` itself cannot be found | Your working directory is not the project root | Change back to the project root |

                A failing close-out hook never needs action from you. It only reports, and the turn may end.


                """;

            // Each harness has its own always-on switch, and the one file carries all of them: applyTo
            // for Copilot, alwaysApply for Cursor, inclusion for Kiro. Claude Code needs none - a rule
            // without "paths" always loads. Cursor alone also needs a different extension: its rules
            // system ignores a plain .md file in .cursor/rules.
            var underCursor = false;
            for (var target = OutputTarget; target != null; target = target.Parent)
            {
                underCursor |= target.Name == ".cursor";
            }

            MarkdownFile = new MarkdownFile("module-building-workflow.instructions", relativeLocation: "", extension: underCursor ? "mdc" : "md")
                .FromMarkdown($$""""""
                    ---
                    applyTo: '**'
                    alwaysApply: true
                    inclusion: always
                    description: "Phase-by-phase workflow for any task that builds or changes an Intent Architect module, and which workflow skill each phase calls for."
                    keywords: [workflow, module building, phases, version, documentation, context]
                    template-id: {{TemplateId}}
                    ---

                    # Module Building Workflow

                    Standing instructions for any task that builds or changes a module for **Intent Architect**.

                    Read this before starting. It describes the phases a module task moves through and what each phase
                    owes you. It does not describe how to author templates or model designer elements — load the
                    module-authoring skills in your environment for that.

                    ## What A Module Is, In This Context

                    An Intent Architect module is a package that **generates code into other applications**. You are not
                    writing the code a consumer runs; you are writing the templates that produce it. Two consequences
                    shape everything below:

                    - **Most files in a module's own folder are generated output**, produced from a designer model.
                    Editing them by hand either gets overwritten on the next regeneration or leaves the model and the
                    files disagreeing. Change the model, then regenerate.
                    - **A module is consumed by applications you cannot see.** Its version number is how those
                    applications decide whether to take your change, and its documentation is the only explanation
                    they get. Neither is optional paperwork.

                    ## The Four Workflow Skills

                    Load the skill named for the phase you are in.

                    | Skill                      | Phase                                              |
                    | -------------------------- | -------------------------------------------------- |
                    | `module-context-capture`   | Phase 1 (read it) and Phase 4 (write it)           |
                    | `module-version-increment` | Phase 2 (run the gate up front) and Phase 4 (confirm) |
                    | `module-docs-chore`        | Phase 3 and Phase 4                                |
                    | `module-dependency-audit`  | Phase 4 (verify)                                   |

                    ===

                    ## Phase 1 — Understand The Task And What It Touches

                    Establish which modules the task affects, then read each one's `CONTEXT.md`.

                    `CONTEXT.md` is a markdown file kept in a module's own folder. It records the design decisions,
                    invariants, and cross-module relationships behind that module — the reasoning that is not visible in
                    the code. It tells you which constraints are deliberate rather than accidental.

                    > **If what the task asks for conflicts with what `CONTEXT.md` records, surface the conflict before
                    > going further.** Either the record is out of date and this change should update it, or the task is
                    > based on a wrong assumption. Both need a decision from the developer; neither should be quietly
                    > resolved by you.

                    → `module-context-capture`

                    > **Exit condition:** you can name the modules in scope and any recorded constraint that bears on the
                    > task.

                    ===

                    ## Phase 2 — Classify And Plan

                    ### Classify the change

                    | Change                                          | What it obliges                                                                                           |
                    | ----------------------------------------------- | --------------------------------------------------------------------------------------------------------- |
                    | **New module**                                  | The full sequence, starting from no existing context                                                      |
                    | **Change that affects generated output**        | Capture the current generated output *before* changing anything, so the difference is provable afterwards |
                    | **Designer or metadata only, no output impact** | No output capture — but still a version increment, and documentation if the modelling experience changed  |

                    If you cannot tell whether generated output is affected, treat it as affected. Capturing output that
                    turned out not to change costs minutes; shipping an unverified change does not.

                    ### Anticipate the modules in scope, and ensure each is at an unpublished version

                    Name every module you expect this task to change, then run the gate in `module-version-increment` — *Is
                    This Version Already In Flight?* — for each: if a published version already exists at or beyond its
                    current one, increment it now; if nothing at or beyond it is published (in flight from earlier work on
                    this same line), leave it alone. Do not leave this until the end.

                    Doing it first buys three things:

                    - **The change becomes installable as soon as it exists.** A module still sitting at a published
                    version is ignored in favour of the published copy, so leaving it there can leave you testing the old
                    behaviour without realising it.
                    - **It cannot be forgotten, and it cannot be done twice.** Running the gate up front is what prevents a
                    module being bumped a second time for a change an earlier bump already accounted for.
                    - **It forces the scope question early.** Naming the modules you are about to touch surfaces the ones
                    you had not thought of — usually the dependents.

                    If the task began from a written specification, plan, or design document, that document is where the
                    anticipated modules and their versions belong. Otherwise record them wherever the task's working notes
                    live.

                    Two things will not always go to plan, and neither is a failure:

                    - **A module you did not anticipate needs changing.** Run the gate for it at the point you first change
                    it, and treat the miss as a signal the scope was wider than it looked.
                    - **The impact turns out larger than you assumed** — something planned as a minor change breaks
                    existing output. Raise the component at close-out — this is the one mid-flight adjustment the gate
                    permits, not a second increment.

                    → `module-version-increment`

                    > **Exit condition:** the change is classified, output captured if applicable, and every module you
                    > expect to change has been through the gate.

                    ===

                    ## Phase 3 — Implement

                    Work through the change, and keep two things current as you go rather than at the end.

                    > **Record decisions when you make them.** A decision written up later is written from memory, and the
                    > alternatives you rejected — the part a future reader most needs — are gone by then.

                    > **Update documentation in the same turn as the behaviour it describes.** A version line whose
                    > documentation was deferred becomes a set of changes nobody can account for, and reconstructing it
                    > later costs far more than writing it at the time.

                    → `module-context-capture`, `module-docs-chore`

                    #### Standing rules while implementing

                    - **Compiling is not working.** A successful build proves the syntax is valid. It says nothing about
                    whether the generated output is correct. Do not report a change as done on a green build.
                    - **Inspect what regeneration actually produced.** Read the difference before accepting it. A
                    regeneration reporting "no changes" only means something if the output could genuinely have been
                    rewritten — output that is protected, excluded, or already sitting on disk in the expected shape
                    will report clean whether or not your change works.
                    - **Never edit generated output to make a regeneration look correct.** That inverts the test: you are
                    no longer checking that the template produces the right thing, only that the disk matches itself.
                    - **A version number is not a debugging tool.** If a change is not being picked up, diagnose that.
                    Renumbering to force it hides the real fault.

                    > **Exit condition:** the change is implemented, the build exits 0, and regenerated output has been
                    > inspected rather than assumed.

                    ===

                    ## Phase 4 — Close Out

                    In this order:

                    1. **Version** — confirm every module you actually changed went through the gate in Phase 2, including
                    any you did not anticipate, and that a still-in-flight version was correctly left alone rather than
                    bumped again. Raise the component if the impact turned out larger than planned, and move any
                    dependent modules that need to move. → `module-version-increment`
                    2. **Dependencies** — verify the module's `.imodspec` dependencies match what it actually references,
                    and supply any the Software Factory did not detect. A missing one compiles cleanly and fails at
                    install, so nothing earlier catches it. → `module-dependency-audit`
                    3. **Documentation** — confirm what shipped is described, including anything from earlier in the same
                    version line that was never written up. → `module-docs-chore`
                    4. **Context** — consolidate the durable knowledge from this change: decisions taken, invariants
                    established, anything a future session would otherwise rediscover the hard way. A problem with Intent
                    Architect itself is not module knowledge: report it to the user for the Intent support team instead,
                    and record only a genuine API or SDK limitation, together with the decision it forced.
                    → `module-context-capture`

                    Version comes first because the documentation refers to it. Dependencies come before documentation
                    because a fix made there is itself an observable change the documentation step then has to describe.

                    > **Exit condition:** every box below is ticked.

                    - [ ] Change was classified, and output-affecting changes were captured before being modified
                    - [ ] `CONTEXT.md` was read for every module touched, and any conflict was surfaced
                    - [ ] Decisions were recorded as they were made
                    - [ ] Every changed module went through the version gate up front, impact re-checked, and dependents moved
                    - [ ] `.imodspec` dependencies verified against what the module actually references
                    - [ ] Documentation reflects what shipped
                    - [ ] `CONTEXT.md` updated with this change's durable knowledge
                    - [ ] Build exits 0, and regenerated output was inspected

                    {{gateFailureSection}}===

                    ## If A Phase Cannot Be Completed

                    Say so explicitly and stop, rather than proceeding on an assumption. A module task that skips a phase
                    silently produces work that looks finished and is not — and the cost lands on a consumer who cannot
                    see what was skipped.
                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
