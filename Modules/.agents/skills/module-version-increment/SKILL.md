---
name: module-version-increment
description: "Increment a module's version (choosing the right component) before implementing a change that touches it, then confirm and propagate to dependents at close-out. USE ONLY WHEN the modules a task will change are known — as soon as that's decided, and again at close-out. DO NOT USE FOR writing the change's documentation (see module-docs-chore) or recording design rationale (see module-context-capture). REQUIRES the set of modules the task will touch already identified."
keywords: [version, increment, release, bump, dependents, publish]
template-id: Intent.ModuleBuilder.AI.Workflow.Skills.ModuleVersionIncrement_SkillMd_Agents
contentHash: 85F20CDC2B73D7741344B5CD168880B21FCBBB22ED773A9206A24C55A43CD01C
---
# Skill: module-version-increment

A module's version is how consuming applications decide whether to take your change. Getting it wrong
in either direction has a cost: too low and consumers never receive the change, too high and they
expect a compatibility break that did not happen.

## Increment Before You Implement, Not After

The version moves **up front**, as soon as you know which modules a task will touch — not once the
work is finished.

| When | What this skill does |
|---|---|
| **Before implementing** | Increment every module the task is expected to change |
| **At close-out** | Confirm each changed module was incremented, correct the component if the impact grew, and move dependents |

Incrementing first is what makes the change testable at all: a module rebuilt at an already-published
version is ignored in favour of the published copy, so work done before the version moves can be
verified against the old behaviour without any sign that it was. It also removes the question of
whether a module has already been moved for this task — you moved it, deliberately, at the start.

- *At close-out the normal outcome is that there is nothing to do.** Confirm the version moved and stop.

Act only in these cases:

- **A module was changed that you did not anticipate** — increment it now.
- **The impact turned out larger than assumed** — a change planned as minor breaks existing output.

Raise the component. Correcting a version that has not been published is not a second increment.

- **A dependent needs to move** — see *Move The Dependents Too* below.

## Deciding The Increment

Judge the impact on **consumers of the module's generated output and modelling experience**, not on
the module's own source.

| Impact | What it looks like | Move |
|---|---|---|
| **Patch** | Small improvement or fix with little to no impact — a bug fix, one more setting affecting a small portion of the module, or an addition alongside an already-established set that changes nothing already generated | third component |
| **Minor** | A new capability that adds a meaningfully new dimension to the module's behavior or experience, without breaking anything already there | second component |
| **Major** | A structural addition that changes how users already interact with the module — even without a hard technical break | first component |

When the impact is genuinely ambiguous, take the **higher** of the two. Under-stating is the expensive
mistake — consumers upgrade expecting a compatibility they do not get.

### Judging Impact — Minor vs Patch

This is a magnitude judgment, not a checklist. A single setting that affects a small portion of the
module is Patch — even if it's a wholly new setting, its impact is narrow. Minor is reserved for
something that materially expands the experience: for example, a module gaining the ability to
generate templates tailored to a specific AI harness (Claude Code vs. another agent) is Minor —
that's a new dimension of capability, not an incremental item alongside an existing set.

### Judging Impact — Minor vs Major

Major isn't only "this breaks a build." It's **"this changes how users already work with the
module."** For example, adding a whole new designer to the module: even if nothing that existed
before stops working, users now have a new way to interact with the module that can change
established habits or expectations, and that alone is enough to warrant the first component moving.
Reserve Minor for additions that expand the module without touching how it's already used.

### Apply the rule

| Situation | Rule |
|---|---|
| Brand-new module | Start at `1.0.0-pre.0` |
| Already on a prerelease | Move the **prerelease component only** |
| Released (non-prerelease) version being changed | Move per impact, then add `-pre.0` |
State the impact and the reasoning before applying it — *"patch, because this setting only affects a
small portion of the module's behavior"* / *"minor, because this is a genuinely new capability
dimension, not breaking anything"* / *"major, because this changes how users already interact with
the module"* — so the choice is reviewable rather than asserted.

## Ad-hoc Changes — Check Before You Move

Sometimes a change reaches you without the up-front step: an edit was already made, and the version has
to move for it to be installable. Here the risk returns — **double-incrementing**, moving a version
that was already moved for this same work and has not been published yet. That leaves a version nobody
asked for and a gap in whatever history the module keeps.

Work down this list and stop at the first step that answers:

1. **The task's own notes or plan.** If they record that this module was already incremented for this

work, it is done. Leave it.

2. **Version control.** Compare the module's current version against the same file where the branch

diverged. If the version has already changed in this branch or working tree, it has already been
moved for this work.

3. **The module's release notes.** If the module keeps them and there is already an entry for the

current version, that version is in flight and already accounts for your change.

4. **Ask.** If nothing above answers it, ask the developer whether the current version has been

published. Take the answer as given, note it with the task's working notes, and do not ask again for
that module during this task.

Once you have moved it, record that you did — that record is step 1 for whoever comes next.

- *Never change a version number to resolve a build or regeneration failure.** A version expresses

intent about compatibility; it is not a troubleshooting lever. If a change is not being picked up,
diagnose that instead.

## Keep The Version Consistent Everywhere

A module's version appears in more than one place — its package manifest, its project file, and its
designer settings. All of them must agree. Where they differ, the **designer value wins when it is the
higher one**: bring the others up to it rather than pulling it down.

## Move The Dependents Too

An increment is incomplete if the modules that depend on the change stay behind.

- **Every module whose template or extension code changed needs its own increment** — even when the

change is "only" narrowing an existing query. To a consumer, a same-version code change is
indistinguishable from no change at all.

- **Shared contracts move together.** When several modules cooperate through a shared string contract

— a template role name, a well-known key — changing that contract requires a synchronised increment
on **every** module that reads *or* writes it, not just the one that motivated the change. A
consumer left behind silently stops matching, which is worse than the behaviour before the change.

- **Modules that pin this one** need their dependency entry updated.
- **Consumers that depend on the change's *effect*** are best handled declaratively, by declaring a

minimum-version floor in the module's own metadata, rather than by reinstalling into each
application you happen to know about — that only ever fixes the ones you thought of.

## Confirm It Is Actually Ahead

Before releasing, confirm the new version is ahead of what is already published. A release that is not
will not carry your changes.

> **The local-compile trap.** Compiling a module registers it in the same location a module search
> reads from. From then on the search reports that version as existing, whether or not it was ever
> published. Treat an "exists" result as inconclusive rather than as proof — and keep in mind another
> branch may have published the same number first.

> **The downgrade guard.** The Software Factory silently refuses to regenerate `.imodspec` if the
> version you just set compares as *lower*, by semver precedence, than the `<version>` already on
> disk — regeneration reports nothing staged, with no error or warning. A `-pre.#` suffix sorts
> *below* the same `X.Y.Z` with no suffix, so correcting `1.1.0` down to `1.0.3-pre.0` trips this
> guard even though the intent is a fix. If a version change reports zero diff when you expected
> one, suspect this before anything else — check the `.imodspec`'s current `<version>` and compare
> precedence. Workaround: temporarily set only the `<version>` line down to a safe value (confirm
> via `git diff`/`git status` that the file is uncommitted first), then reapply your intended
> version and regenerate forward.

## Iterating A Version That Is Already Published

Rebuilding at a version that has already been published is shadowed — the published copy is served
and the rebuild is silently ignored. Move the prerelease component forward so the new build is picked
up, and note that you have done so. This is the one case where a module's version legitimately moves
more than once in a task, because each iteration needs its own build to be installable. Consolidate
to the final release version when the work is done.

## Supported Client Version Range

While you are here, confirm the module's supported client version range still holds. Its lower bound
is the **higher** of two floors:

1. **The SDK floor** — the minimum client version required by the SDK packages the module references.

A regeneration failure states this one outright.

2. **The dependency floor** — the highest lower bound across the modules this one depends on.

Take the maximum of the two. Re-check after anything that moves an SDK or dependency version, and
after a package rename — a renamed package whose manifest falls out of step can silently revert the
range to its scaffold default.

## Checklist

- [ ] Every module the task was expected to change was incremented before implementation started
- [ ] Impact assessed for each, with the reasoning stated
- [ ] Any module changed but not anticipated has since been incremented
- [ ] Impact re-checked at close-out, and the component raised if the change grew
- [ ] For an ad-hoc change: checked the module had not already been moved before moving it
- [ ] Version consistent across manifest, project file, and designer settings
- [ ] Every module whose code changed for this work moved, shared-contract participants included
- [ ] Dependents' pinned versions and minimum-version floors updated
- [ ] New version confirmed ahead of what is published, allowing for the local-compile trap
- [ ] Supported client version range still correct
