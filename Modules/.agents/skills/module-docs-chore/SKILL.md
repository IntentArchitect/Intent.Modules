---
name: module-docs-chore
description: "Update a module's documentation in the same turn as the change that affects it. Use whenever a module change alters anything a consumer can observe."
keywords: [documentation, release-notes, readme, imodspec, chore, upkeep]
contentHash: 08E8BE5172E8B01C5D36E890C11F909DBCFA51A441A52444C4CC56AB1E59CAA5
---
# Skill: module-docs-chore

Documentation is part of the change, not a follow-up task. This skill covers **when** to write and

- *which artifact** carries what. Where a canonical format exists for an artifact in your environment,

follow it for structure and wording.

## What Triggers An Update

Anything a consumer of the module can observe:

- A new or removed template, or a change in the shape of generated output
- A new module setting, stereotype, or designer element
- A new configuration option, or a changed default
- A behavioural fix that changes what the module produces

Write it **in the same turn as the change**. Internal refactoring with no observable effect on
generated output needs nothing.

## Which Artifact Carries What

| Artifact | What goes in |
|---|---|
| Module metadata — summary, description, tags | Kept accurate as a matter of course. These are what a consumer sees before installing anything. |
| `release-notes.md` | One bullet under the current version — **only if the file already exists** |
| `docs/README.md` | The section the change affects — a settings table, a generated-output example, a feature description. **Create it if the module does not have one.** |

## Release Notes Are Maintained, Never Introduced

If a module has a `release-notes.md`, add an entry for the change: a single bullet under the current
version, prefixed to say whether it is a new feature, an improvement, or a fix. A fix entry is more
useful when it names the scenario that triggered the bug rather than just the symptom.

### Keep Entries Short, And Group Them

Release notes are read by someone deciding whether a release affects them — not auditing what was
built. Write for that reader.

- **One line per entry.** If a bullet needs a second sentence to explain itself, that detail belongs in

  the module's documentation, not here.

- **Group related changes into one entry.** Work that added four templates, two settings and an

  instruction file is *one* entry describing the capability — not seven describing its parts.

- **Split only when a consumer would act on each differently.** Two independent changes affecting

  different people are two entries. Two halves of one change are one entry.

Fewer, higher-altitude entries scan better and age better. A reader months later wants to know what
changed about the module, not which files were touched to do it.

- *If a module has no `release-notes.md`, do not create one.** Its absence is a deliberate choice

about how that module is maintained, not an oversight to correct. Introducing the file commits the
module to a history nobody agreed to keep, and a half-kept changelog is worse than none.

## Module Metadata Is Always Kept Current

The module's own summary, description, and tags are maintained regardless of which other artifacts a
module keeps. They are the module's shopfront — the only thing a consumer reads while deciding
whether to install it — so they should describe what the module actually does now.

Modules scaffolded from a template often keep placeholder metadata long after they have stopped being
placeholders. When you notice generic filler in a summary or description, or an empty tag list,
replace it. This is upkeep to do in passing, not an inspection to complete before moving on.

### Tag Format

Tags are **lowercase** and **separated by spaces**. Where a single concept spans more than one word,
join it with a hyphen so it stays one tag rather than splitting into two meaningless ones:

```
intent module-builder ai workflow
entity-framework persistence csharp dotnet
azure-service-bus messaging eventing infrastructure
```

`module-builder` is one tag; `module builder` is two, and neither half means anything on its own.

Aim for a handful — enough to place the module, not an exhaustive list. Between them the tags should
cover the technology it integrates, the concern or pattern it addresses, and the stack it targets.
Skip words that would describe every module equally: `module`, `common`, `helper`, `base`.

### Authorship Is Copied, Never Invented

The module's author or publisher field identifies **who publishes the module**, and it is not yours to
choose. Take it from the other modules in the same repository: read their manifests and use the value
they already carry, or the most common one if they disagree.

If no sibling module has it set, or there are no siblings to look at, **ask the developer** who
publishes these modules and use their answer.

Never infer a publisher from the module's name, its namespace, the tooling it is built with, or the
platform it targets — and never carry a value across from a different repository. A module published
under the wrong name is worse than one published under none, because it misattributes ownership to
someone who never agreed to it.

## The README

Every module should have a `docs/README.md` explaining what it generates and how it is configured.
Unlike the release notes, this one **is** created when missing — a module with no README leaves a
consumer with nothing to read at all.

Keep it describing the module's current behaviour rather than its history. When a change makes a
section wrong, correct that section; do not append a note saying it changed.

## Cover The Whole Version Line

Before writing today's entry, check what else has landed under the current unreleased version —
earlier changes in the same version line often went undocumented. Cover them, but **fold them into the
entry they belong with** rather than appending a bullet per change. The aim is that the version's
entries account for everything in it, not that there is an entry per change.

## Checklist

- [ ] Module summary, description, and tags describe current behaviour
- [ ] Tags are lowercase, space-separated, hyphenated within a compound term
- [ ] Author matches what sibling modules use — copied or asked for, never invented
- [ ] `release-notes.md` updated **if present** — and not created if absent
- [ ] Entries are one line each, grouped by capability rather than listed per change
- [ ] `docs/README.md` reflects the change — created if the module had none
- [ ] Earlier undocumented changes in the same version line covered
