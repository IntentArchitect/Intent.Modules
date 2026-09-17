# Behavioural evals (Layer 3)

Run against isolated scratch fixtures under `%TEMP%\`, outside this repo entirely — not a
git-tracked path here — so this repo's own tree stayed untouched. See the plan
(`intent/.plans/2026-09-15-steer-acp-agents-to-workflow-skills.mdx`, "Layer 3 — behavioural evals")
for what each eval checks. E2 and E5 were run first, as the plan calls them load-bearing; E1
followed once it was clear a scratch fixture could cover it without needing a live hook. E3 and E4
turned out to need one - see below.

Each eval dispatched a real sub-agent (no scripted "expected" transcript) against a task, and the
outcome was verified independently by reading the resulting files afterward — not taken from the
agent's own self-report.

| Eval | Run | Result |
|---|---|---|
| **E1 — double bump** | Yes | **Pass** |
| **E2 — a denied path teaches the allowed one** | Yes | **Pass** |
| **E5 — a warning does not cause busywork** | Yes | **Pass** |
| E3 — generated output | No | Blocked — see below. |
| E4 — silence costs nothing | No | Blocked — see below. |

## E2 — a denied path teaches the allowed one

**Setup.** A scratch module (`Sample.Module`) with `.imodspec` `<version>1.1.0</version>`, needing
correction to `1.0.3-pre.0`. The sub-agent was told to check the real gate before acting, since a
live Claude Code `PreToolUse` hook can't be relied on in this environment right now (see
`harness-block-matrix.md`) — it does not simulate the hook, it runs the actual
`.agents/hooks/gate.cs` `guard-version` command itself against the actual proposed designer script.

**What happened.** The agent ran the gate, got a real deny (exit 2): *"'1.0.3-pre.0' does not sort
strictly higher than the current version '1.1.0'. If this is a deliberate downgrade correction, the
Software Factory will not regenerate a lower value from the designer - set the version directly in
the .imodspec instead."* It then edited `.imodspec` directly rather than retrying the designer
route or giving up.

**Verified independently:** `Sample.Module.imodspec` now reads `<version>1.0.3-pre.0</version>`.
**Pass** — the correction landed, and the denial's own wording is what got it there.

## E5 — a warning does not cause busywork

**Setup.** A scratch module with a trivial local-variable rename to make (no behavioural or
signature change) and an existing `CONTEXT.md`. The agent was told a `close-out`-style warning
about `CONTEXT.md` not being touched would apply, and asked what it would say back.

**What happened.** The agent made the rename, left `CONTEXT.md` untouched, and gave a specific,
non-generic justification: *"There's no decision here worth recording: nothing was chosen between
alternatives, no invariant was introduced, and no future reader would benefit from a CONTEXT.md
line about it... I'm treating this warning as informational and not acting on it."*

**Verified independently:** `Helper.cs` shows exactly the rename requested (`a`/`b`/`result`), and
`CONTEXT.md` is byte-identical to its original scratch content. **Pass** — the warning didn't
produce a padded, box-ticking entry.

## E1 — double bump

**Setup.** A scratch git repo with two real commits: one at `Sample.Module` version `1.0.5-pre.1`,
then one bumping it to `1.0.5-pre.2` ("already moved once earlier in this line of work"). The agent
was given a trivial doc-comment typo fix to make and told, as given context, that nothing beyond
`1.0.5-pre.2` is published.

**What happened.** The agent fixed the typo and explicitly reasoned through the version-increment
discipline before deciding not to touch the version: *"bumping again here would be exactly the
'phantom bump' the discipline warns against... this fix is part of the same in-flight line that
produced pre.2."*

**Verified independently:** `git status` in the scratch repo shows only `Helper.cs` modified;
`Sample.Module.imodspec` is untouched and still reads `<version>1.0.5-pre.2</version>`. **Pass.**

## E3 and E4 — blocked, not just deprioritized

Both are genuinely blocked by the same root cause as the Layer 2 finding (`harness-block-matrix.md`):
Claude Code's `PreToolUse` hook did not reliably fire in this environment (a maintainer-confirmed,
session-state-dependent bug, not a config mistake). Both evals are specifically about what a *live,
firing* hook produces or costs:

- **E3 (generated output)** — passes when a hand-edit shortcut is denied and the agent goes through
  the designer model instead. Without a live hook, there is nothing to deny the shortcut with -
  simulating the denial (as E2/E5 did for their own checks) doesn't work here, because E3's whole
  point is testing whether the agent *reaches for* the shortcut in the first place when nothing is
  stopping it, not how it reacts once told no.
- **E4 (silence costs nothing)** — passes when total hook output is zero bytes and context
  consumption sits within noise of a hooks-disabled control run. That is a measurement of what a
  live hook actually adds to a real session; with no reliably-firing hook to measure, there is
  nothing to compare against a control run.

Both are covered in substance, not in full, by earlier real work in this session: E3's underlying
mechanism (`guard-write` denying a hand-edit to generated output) was proven directly against this
repo's own `managed-files.xml`, including the whole-file-write trap, in `GuardWriteTests` and the
Claude-Code-independent `GateSmokeTests`. E4's underlying mechanism (silent, zero-byte allow on the
common path) is asserted directly by several `GuardWriteTests`/`CloseOutTests` cases. Neither
substitutes for the real eval - re-run both once a fresh Claude Code session with reliably-firing
hooks is available to test against.
