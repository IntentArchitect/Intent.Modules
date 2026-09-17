# Behavioural evals (Layer 3)

Run against an isolated scratch fixture (`%TEMP%\gate-evals\`, outside this repo entirely — not a
git-tracked path here) so a real repository's tree, and this repo's own just-committed work, stayed
untouched. See the plan (`intent/.plans/2026-09-15-steer-acp-agents-to-workflow-skills.mdx`, "Layer
3 — behavioural evals") for what each eval checks and why E2/E5 were run first.

Each eval dispatched a real sub-agent (no scripted "expected" transcript) against a task, and the
outcome was verified independently by reading the resulting files afterward — not taken from the
agent's own self-report.

| Eval | Run | Result |
|---|---|---|
| **E2 — a denied path teaches the allowed one** | Yes | **Pass** |
| **E5 — a warning does not cause busywork** | Yes | **Pass** |
| E1 — double bump | No | Not yet run. |
| E3 — generated output | No | Not yet run. |
| E4 — silence costs nothing | No | Not yet run. |

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

## Remaining (E1, E3, E4)

Not run — this pass prioritized the two evals the plan calls load-bearing ("these two check what
the check *costs*"). The other three check that a mechanism fires at all, which Layer 1 (unit
tests against the gate's own logic) and the manual runs earlier in this work already cover more
directly:

- **E1 (double bump)** — covered in substance by `guard-version`'s own double-bump guard, exercised
  directly against a real module version during this same implementation (see the plan's Phase 2
  version-gate work on `Intent.ModuleBuilder.AI.Skills`, and the `cases-guard-version` table).
- **E3 (generated output)** — covered in substance by the `guard-write` deny tests already run for
  real against this repo's own managed-files.xml (see the implementation session's Layer 1/2 work).
- **E4 (silence costs nothing)** — covered in substance by the repeated "zero-byte allow" checks
  already run for real against `guard-write` and `close-out` in this repo.

None of those substitute for a dedicated eval with a fresh scratch fixture and an independent
grading pass — they're the reason the risk here is judged low, not a reason to skip running E1/E3/E4
properly at some point.
