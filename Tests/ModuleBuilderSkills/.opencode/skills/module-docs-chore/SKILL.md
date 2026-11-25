---
name: module-docs-chore
description: "The module-building workflow's documentation chore: decide which docs an observable change needs (release notes, README, .imodspec metadata, icon) and write them in the same turn. For format, load the built-in module-docs skill. USE ONLY WHEN a module change alters anything observable — a new/removed template, setting, stereotype, config default, or behavioural fix. DO NOT USE FOR internal refactors with no observable effect, or for bumping the module's version number itself (see module-version-increment). REQUIRES the observable change already implemented or decided."
keywords: [documentation, release-notes, readme, imodspec, icon, chore, upkeep]
template-id: Intent.ModuleBuilder.AI.Guidelines.Skills.ModuleDocsChore_SkillMd_Agents
contentHash: 70371515346D5922E3F10E695DEFACD37DEE1E888C55419C31A2F411818C83E9
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
| `release-notes.md` | An entry for the change, following the built-in `module-docs` skill, **only if the module's `Include Release Notes` is already ticked**. Otherwise leave release notes alone, and do not tick it. |
| `docs/README.md` | The section the change affects. **Create one if the module has none**: this environment maintains module READMEs. |
| Module icon | Only when the module has none; never overwrite an existing icon. Describe it from the module's `.imodspec` summary and tags, or its `CONTEXT.md` Purpose, and craft it with `module-svg-icon`. If that skill is unavailable, leave the module without an icon. |

## Checklist

- [ ] Written in the same turn as the observable change, and the whole version line covered
- [ ] Every artifact follows `module-docs`
- [ ] `release-notes.md` entry added only if `Include Release Notes` was already ticked, following `module-docs`
- [ ] `docs/README.md` reflects the change, created if the module had none
- [ ] Module icon created only if missing, existing icons left untouched
- [ ] Author copied from sibling modules or asked for, never invented
