---
name: module-context-capture
description: "Record a module's durable design knowledge in a CONTEXT.md in the module folder — decisions, invariants, and cross-module relationships. Use when a design decision is made or a module change concludes; read it before modifying any module."
keywords: [context, decisions, architecture, invariants, cross-module, durable]
contentHash: F4E94BD452DEEF64F1AE235A42FFAC1E408702D7C6A44AEAFE3A2E99AECD713E
---
# Skill: module-context-capture

`CONTEXT.md` is a module's **durable knowledge layer** — the *why* behind its design. The code
records what a module does; `CONTEXT.md` records the reasoning that would otherwise leave with the
session that decided it.

## Where It Lives

Inside the **module project folder** — e.g. `Modules/Intent.Modules.X/CONTEXT.md`. One per module.

- Never at the repository root — there is no global `CONTEXT.md`.
- Never in a transient or build-state folder — those get cleared; this must survive.
- Never inside an `intent` / `.intent` metadata folder.

## Read It Before You Modify

Before changing a module, read the `CONTEXT.md` of **every** module you are about to touch. It
tells you which constraints are deliberate, and which code is load-bearing for reasons the code
itself does not explain.

- *If your intended change conflicts with `CONTEXT.md`, stop and flag the conflict.** Do not

silently "improve" a design that was chosen on purpose. Either the context is stale — in which case
update it deliberately, as part of this change — or the change is wrong. Both need a decision, not
an assumption.

## What Goes In

| Capture | Example |
|---|---|
| Architectural decisions **and their reasoning** | Why a concern became a factory extension rather than a template |
| Invariants and constraints | This template must stay transport-agnostic; it may not reference X |
| Technology constraints | The library cannot do Y in environment Z, so the module generates W instead |
| Accepted patterns | How this module's templates resolve types across layers |
| Cross-module relationships | Which modules this one affects, what it broadcasts, what it expects others to handle |
| Decisions taken during implementation | Options considered and **rejected**, and why |

## What Stays Out

- Transient task state, progress trackers, TODO lists — this file outlives the task.
- Anything already recorded elsewhere: release notes, module documentation, generated code.
- Anything trivially rediscoverable by reading the module source.

## Write At Decision Time

Record the entry **when the decision is made**, not reconstructed at the end. A decision written up
late is written from memory, and by then the alternatives that were rejected — the most valuable
part — are usually gone.

The same applies to correction. When a decision is superseded, **update or remove the entry then**.
A confidently wrong `CONTEXT.md` is worse than none, because the next session trusts it.

## Suggested Shape

```markdown
# Context: [Module Name]

## Purpose
[What this module is responsible for, in two or three sentences.]

## Architectural Decisions
- **[Decision]** — [why; what was rejected and why]

## Invariants & Constraints
- [Rule that must hold, and what breaks if it does not]

## Module Interactions
- **[Other module]** — [how they relate; what is broadcast, consumed, or assumed]

## Superseded
- [Decision that no longer holds, and what replaced it]
```

## Checklist

- [ ] `CONTEXT.md` exists in the module project folder — not the repo root, not a transient folder
- [ ] Every module modified by this change had its `CONTEXT.md` read first
- [ ] New decisions recorded with reasoning, including rejected alternatives
- [ ] Superseded entries updated or removed — no stale claims left standing
- [ ] Any conflict between the change and existing context was surfaced, not silently resolved
