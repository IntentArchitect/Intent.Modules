# Context: Intent.ModuleBuilder.AI.Workflow

## Purpose
Bundles conditional, process-shaped AI agent workflow skills for the module-building lifecycle —
when to increment a version, when and how to keep documentation current, when to capture durable
context — into the consuming repo's `.agents/` folder. Unlike `Intent.ModuleBuilder.AI.Skills`
(unconditional reference skills), every bundled skill here reads its own settings group and adapts
its generated content accordingly.

## Architectural Decisions

- **No package dependency on `Intent.ModuleBuilder.AI.Skills`, by deliberate choice.** The two
  modules cover adjacent but decoupled ground (see that module's own `CONTEXT.md` for the mirror of
  this decision). Installing AI.Workflow must not pull in AI.Skills' whole bundle as a side effect.

- **`module-docs-chore` references `module-svg-icon` by name, but only as a soft, conditional
  pointer** ("if the `module-svg-icon` skill is available in your environment, use it") — never as a
  hard requirement. This is the first case where AI.Workflow content names an AI.Skills skill at
  all; it was a deliberate exception granted for this feature specifically, not a reversal of the
  no-dependency stance above. If AI.Skills isn't installed, the generated chore skill explicitly
  says to leave the module without an icon rather than improvising a substitute.

- **Icon creation is create-when-missing only — never a refresh.** An existing icon (however dated)
  is treated as a deliberate choice by whoever set it, mirroring how `docs/README.md` is created
  when absent but never forcibly rewritten wholesale. This is a narrower stance than release notes
  ("maintained, never introduced") and deliberately so: an icon is a visual asset a maintainer might
  hand-pick, unlike prose that this chore is expected to keep in sync with behaviour.

- **`MaintainModuleIcon` is a plain boolean, mirroring `MaintainModuleREADME` exactly** (`Switch`
  control, same accessor shape in `ModuleSettingsExtensions.cs`). No richer tri-state was added —
  off means "never touch icons," on means "create when missing."

- **`module-version-increment`'s major/minor/patch rubric judges impact on the user's experience,
  not a generic semver textbook rule.** The original table treated "new capability, setting, or
  generated file" as automatically Minor; Dandré rejected that after two changes that fit that
  description (`module-svg-icon`, `MaintainModuleIcon`) were both actually Patch. The replacement
  axis:
  - **Patch** — narrow impact (a setting affecting a small portion of the module, an addition
    alongside an already-established similar set), regardless of whether it's technically "new."
  - **Minor** — a genuinely new capability *dimension* (Dandré's example: a module gaining the
    ability to generate templates tailored to a specific AI harness) — high impact, not breaking.
  - **Major** — changes how users already interact with the module (Dandré's example: adding a
    whole new designer to the module) — even with zero hard technical break, established habits or
    expectations can shift enough to need developer attention.

  Minor vs Patch is explicitly a magnitude judgment, not a checklist — being opt-in/off-by-default
  does not by itself make something Patch (`module-svg-icon` had no setting at all and was still
  Patch). Reasoning must be stated at the point of choosing, same as before.

- **Two gotchas were added to `module-version-increment`/`module-docs-chore` after a real consuming
  session tripped on both.** With `UsePreReleaseVersions` on, a developer corrected a module from
  `1.1.0` down to `1.0.3-pre.0`; the Software Factory silently reported nothing staged because
  `-pre.#` sorts *below* the same `X.Y.Z` with no suffix — a downgrade by semver precedence, distinct
  from the already-documented local-compile trap. The same session then carried the `-pre.#` suffix
  into a `release-notes.md` heading, because the rule to strip it lived only in the opt-in
  `module-docs` skill, never in the routine `module-docs-chore` path that actually fires during
  ordinary work. Both gotchas are now documented where the routine path will actually see them.

## Invariants & Constraints

- `AIWorkflowSettings` accessors (`UsePreReleaseVersions`, `MaintainModuleREADME`,
  `MaintainModuleIcon`) are generated from `Module Settings Field Configuration` children of the
  `AI Workflow Settings` element — never hand-edit `Settings/ModuleSettingsExtensions.cs`; add the
  field in the designer and regenerate.
- `module-docs-chore`'s conditional sections (`readme*`, `icon*` variable pairs) all follow the same
  three-part shape: a table row, a full section, a checklist item, each toggled by the same boolean.
  A new conditional artifact should follow this exact shape rather than inventing a different one.

## Module Interactions

- **Intent.ModuleBuilder.AI.Skills** — see the "No package dependency" and soft-reference decisions
  above. `module-svg-icon` there owns the actual SVG-crafting mechanics; this module only decides
  *when* to invoke it and *where to source its input description* (the module's own `.imodspec`
  summary/tags or `CONTEXT.md` Purpose section) — it does not duplicate the crafting guidance.

## Superseded

(none yet)
