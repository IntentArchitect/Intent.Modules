# Harness enforcement matrix (Layer 2)

One deliberately illegal action per harness — editing a path listed in `managed-files.xml` — with
the observed result recorded here rather than left in a transcript. See the plan
(`intent/.plans/2026-09-15-steer-acp-agents-to-workflow-skills.mdx`, "Layer 2 — harness
enforcement") for what this is checking and why it can't be settled by reading vendor docs alone.

| Harness | Tested | Result | Notes |
|---|---|---|---|
| **Claude Code** | Yes | **Inconclusive — hook did not fire** | See below. |
| **Codex** | No | — | No interactive Codex CLI access from this environment. |
| **Kiro** | No | — | No interactive Kiro access from this environment. |
| **Cursor** | No | — | No interactive Cursor access from this environment. |
| **OpenCode** | No | — | No interactive OpenCode access from this environment. |

## Claude Code — what was actually done

1. Installed the hook block from the generated `.agents/hooks/CLAUDE_SETUP.md` into
   `.claude/settings.local.json` (gitignored — a local self-test, not the shared
   `.claude/settings.json` a real consumer would edit), matching the documented shape exactly.
2. Confirmed the gate itself was already built and fast (`dotnet run gate.cs -- warm`, then
   `--no-build`, both ~0.55s) so the test would not be confounded by a cold compile.
3. Edited `.agents/instructions/module-building-workflow.instructions.md` — a real path listed in
   `Modules/ModuleBuilderAiSkills/ModuleBuilderAiSkills.application.managed-files.xml` — via the
   `Edit` tool, which the `PreToolUse` hook's `"Write|Edit"` matcher should catch.
4. **The edit succeeded. No denial reached the model, and the file was confirmed changed on
   disk.** The change was then reverted by hand.

## Why this is inconclusive rather than a clean fail

Two things point away from a config mistake on this end:

- **The hook config matches the documented schema exactly** — same `matcher`, same
  `"type": "command"` shape, same `--harness claude` flag the gate's `HarnessProtocol.cs`
  branches on.
- **Claude Code's own docs state hook-file edits are picked up by a file watcher without a
  session restart** ("Direct edits to hooks in settings files are normally picked up
  automatically by the file watcher") — so a stale, unloaded config is not the likely cause
  either.

Independent research turned up `anthropics/claude-code` issue **#47810**: PreToolUse hooks (and
the `--dangerously-skip-permissions` flag itself) **stop being invoked entirely after certain
session-state events** — background task completion, or an extended/idle session — with a
maintainer-confirmed hook log showing calls simply stop appearing, no error, no degraded mode
reported to the user. It is closed as a duplicate of #40241 and #1498, i.e. a known, recurring
class of bug, not a one-off report. This session had **both** triggers: a background Bash task
completed earlier, and this is a long-running session with a very large context.

That match is circumstantial, not a confirmed root cause — this environment offers no way to
inspect Claude Code's own hook-invocation log to prove the hook was silently skipped versus never
registered for some other reason.

## What this means for the plan

- **Do not treat this as "Claude Code enforcement confirmed."** It is not.
- **Do not treat this as "the deny mechanism doesn't work."** The mechanism was never actually
  exercised — it's the same "was the hook even called" failure mode the cited issue describes,
  not a wrong `permissionDecision` shape or a gate bug (the gate's own Layer 1 logic and real
  `dotnet run gate.cs` executions are independently proven correct and denying, earlier in this
  same session, run directly rather than through a Claude Code hook).
- **Re-test in a short, fresh Claude Code session** — one with no completed background tasks and
  no extended idle period — to get a clean signal before this row of the matrix can be marked
  pass or fail. Until then, per the plan's own honesty principle for the deny/warn split: this
  reads as advisory on Claude Code, not confirmed enforcement, and the plan should not describe it
  as more than that.
- The upstream issues the plan already named for this exact risk class
  (`openai/codex` #27833, `anthropics/claude-code` #43407) remain unresolved too — #47810 is an
  additional, distinct failure mode on Claude Code, not a resolution of either.

## Still to do

- Re-run the Claude Code probe in a fresh session.
- Run the Codex, Kiro, Cursor and OpenCode probes — needs a human with those tools installed and
  configured; nothing here can substitute for actually running them.
