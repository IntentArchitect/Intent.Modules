---
applyTo: '**'
description: "Phase-by-phase workflow for any task that builds or changes an Intent Architect module, and which workflow skill each phase calls for."
keywords: [workflow, module building, phases, version, documentation, context]
template-id: Intent.ModuleBuilder.AI.Workflow.RootPrinciples.ModuleBuildingWorkflowMd
contentHash: E13644555A473F285B8DA63F96ADE288A30A361C216043D45BD9D0B555A690BD
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

## The Three Workflow Skills

Load the skill named for the phase you are in.

| Skill                      | Phase                                              |
| -------------------------- | -------------------------------------------------- |
| `module-context-capture`   | Phase 1 (read it) and Phase 4 (write it)           |
| `module-version-increment` | Phase 2 (increment up front) and Phase 4 (confirm) |
| `module-docs-chore`        | Phase 3 and Phase 4                                |

===

## Phase 1 — Understand The Task And What It Touches

Establish which modules the task affects, then read each one's `CONTEXT.md`.

`CONTEXT.md` is a markdown file kept in a module's own folder. It records the design decisions,
invariants, and cross-module relationships behind that module — the reasoning that is not visible in
the code. It tells you which constraints are deliberate rather than accidental.

- *If what the task asks for conflicts with what `CONTEXT.md` records, surface the conflict before

going further.** Either the record is out of date and this change should update it, or the task is
based on a wrong assumption. Both need a decision from the developer; neither should be quietly
resolved by you.

→ `module-context-capture`

- *Exit condition:** you can name the modules in scope and any recorded constraint that bears on the

task.

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

### Anticipate the modules in scope, and increment them now

Name every module you expect this task to change, then **increment each one's version before you start
implementing**. Do not leave it until the end.

Doing it first buys three things:

- **The change becomes installable as soon as it exists.** A module rebuilt at a version that has

  already been published is ignored in favour of the published copy, so a version that has not moved
  can leave you testing the old behaviour without realising it.

- **It cannot be forgotten, and it cannot be done twice.** Incrementing retroactively is where a

  module gets moved a second time for a change the first move already accounted for.

- **It forces the scope question early.** Naming the modules you are about to touch surfaces the ones

  you had not thought of — usually the dependents.

If the task began from a written specification, plan, or design document, that document is where the
anticipated modules and their versions belong. Otherwise record them wherever the task's working notes
live.

Two things will not always go to plan, and neither is a failure:

- **A module you did not anticipate needs changing.** Increment it at the point you first change it,

  and treat the miss as a signal the scope was wider than it looked.

- **The impact turns out larger than you assumed** — something planned as a minor change breaks

  existing output. Raise the component at close-out. Correcting a version that has not been published
  is not a second increment.

→ `module-version-increment`

- *Exit condition:** the change is classified, output captured if applicable, and every module you

expect to change has already been incremented.

===

## Phase 3 — Implement

Work through the change, and keep two things current as you go rather than at the end.

- *Record decisions when you make them.** A decision written up later is written from memory, and the

alternatives you rejected — the part a future reader most needs — are gone by then.

- *Update documentation in the same turn as the behaviour it describes.** A version line whose

documentation was deferred becomes a set of changes nobody can account for, and reconstructing it
later costs far more than writing it at the time.

→ `module-context-capture`, `module-docs-chore`

- *Standing rules while implementing:**
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

- *Exit condition:** the change is implemented, the build exits 0, and regenerated output has been

inspected rather than assumed.

===

## Phase 4 — Close Out

In this order:

1. **Version** — confirm every module you actually changed was incremented in Phase 2, including any

   you did not anticipate. Raise the component if the impact turned out larger than planned, and move
   any dependent modules that need to move. → `module-version-increment`

2. **Documentation** — confirm what shipped is described, including anything from earlier in the same

   version line that was never written up. → `module-docs-chore`

3. **Context** — consolidate the durable knowledge from this change: decisions taken, invariants

   established, anything a future session would otherwise rediscover the hard way.
   → `module-context-capture`

Version comes first because the documentation refers to it.

- *Exit condition:** every box below is ticked.
- [ ] Change was classified, and output-affecting changes were captured before being modified
- [ ] `CONTEXT.md` was read for every module touched, and any conflict was surfaced
- [ ] Decisions were recorded as they were made
- [ ] Every changed module was incremented up front, impact re-checked, and dependents moved
- [ ] Documentation reflects what shipped
- [ ] `CONTEXT.md` updated with this change's durable knowledge
- [ ] Build exits 0, and regenerated output was inspected

===

## If A Phase Cannot Be Completed

Say so explicitly and stop, rather than proceeding on an assumption. A module task that skips a phase
silently produces work that looks finished and is not — and the cost lands on a consumer who cannot
see what was skipped.
