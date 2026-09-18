# Harness enforcement matrix (Layer 2)

One deliberately illegal action per harness — editing a path listed in `managed-files.xml` — with
the observed result recorded here rather than left in a transcript. See the plan
(`intent/.plans/2026-09-15-steer-acp-agents-to-workflow-skills.mdx`, "Layer 2 — harness
enforcement") for what this is checking and why it can't be settled by reading vendor docs alone.

| Harness | Tested | Result | Notes |
|---|---|---|---|
| **Claude Code** | Yes | **Pass (confirmed in a fresh session)** | See below. First attempt was inconclusive; retest after an Intent Architect restart (fresh session, no stale hook state) blocked cleanly. |
| **Codex** | No | — | No interactive Codex CLI access from this environment. API-key auth is available though — see below. |
| **Kiro** | Yes | **Fail — hooks never execute on this build** | See below. Not a module schema error; Kiro's own CLI does not run them yet. |
| **Cursor** | No | — | No interactive Cursor access from this environment. |
| **OpenCode** | Yes | **Pass** | See below. |

## Claude Code — confirmed pass, second attempt (fresh session)

The first attempt (below) was inconclusive because the hook never fired. Intent Architect — which
hosts this Claude Code session over ACP — was restarted, giving a genuinely fresh session (no
completed background tasks, no extended idle period, matching the exact condition the first
attempt's write-up called for). Re-ran the identical probe:

1. `.claude/settings.local.json` did not survive the restart (gitignored, so this is expected) —
   recreated it verbatim from `.agents/hooks/CLAUDE_SETUP.md`.
2. Warmed the gate (`dotnet run gate.cs -- warm`, then confirmed `--no-build` exits 0).
3. Attempted the same edit as before — a one-word change to
   `.agents/instructions/module-building-workflow.instructions.md`, a real path listed in
   `Modules/ModuleBuilderAiSkills/ModuleBuilderAiSkills.application.managed-files.xml` — via the
   `Edit` tool.
4. **The edit was denied.** The `PreToolUse` hook fired and returned the gate's exact message,
   naming the owning template (`Intent.ModuleBuilder.AI.Workflow.RootPrinciples.ModuleBuildingWorkflowMd`)
   and the managed-files.xml source, and pointing at the designer as the correct route instead.
5. Verified independently: `git status --porcelain` clean, `grep` for the attempted change text
   in the file returns nothing — the edit never reached disk.

This is a genuine end-to-end pass: real Claude Code session, real `PreToolUse` hook, real gate
binary, real deny surfaced to the model before any write occurred.

## Claude Code — first attempt, inconclusive (kept for the record)

1. Installed the hook block from the generated `.agents/hooks/CLAUDE_SETUP.md` into
   `.claude/settings.local.json` (gitignored — a local self-test, not the shared
   `.claude/settings.json` a real consumer would edit), matching the documented shape exactly.
2. Confirmed the gate itself was already built and fast (`dotnet run gate.cs -- warm`, then
   `--no-build`, both ~0.55s) so the test would not be confounded by a cold compile.
3. Edited `.agents/instructions/module-building-workflow.instructions.md` via the `Edit` tool,
   which the `PreToolUse` hook's `"Write|Edit"` matcher should catch.
4. **The edit succeeded. No denial reached the model, and the file was confirmed changed on
   disk.** The change was then reverted by hand.

Independent research turned up `anthropics/claude-code` issue **#47810**: PreToolUse hooks (and
the `--dangerously-skip-permissions` flag itself) **stop being invoked entirely after certain
session-state events** — background task completion, or an extended/idle session — with a
maintainer-confirmed hook log showing calls simply stop appearing, no error, no degraded mode
reported to the user. It is closed as a duplicate of #40241 and #1498, i.e. a known, recurring
class of bug, not a one-off report. This session had **both** triggers: a background Bash task
completed earlier, and it was a long-running session with a very large context — consistent with
the clean pass once a genuinely fresh session removed both triggers.

## What this means for the plan

- **Claude Code enforcement is now confirmed**, via the second attempt above — not the first.
- The first attempt's failure is best explained by #47810 (circumstantial but consistent: both
  named triggers were present, and removing them via a fresh session fixed it), not a config or
  gate defect — the gate's own Layer 1 logic and real `dotnet run gate.cs` executions were already
  independently proven correct earlier in this work.
- #47810 remains a live risk for any long-running or background-task-heavy Claude Code session,
  independent of anything this module controls — a consumer hitting it would see the same silent
  "edit just succeeds" failure mode. Worth naming in the module's own docs as a known Claude Code
  limitation, not something the gate can work around.
- The upstream issues the plan already named for this exact risk class
  (`openai/codex` #27833, `anthropics/claude-code` #43407) remain unresolved too — #47810 is an
  additional, distinct failure mode on Claude Code, not a resolution of either.

## OpenCode — genuinely confirmed, real CLI, real model

The plan's own model-changes table called for an `OpenCodePlugin` (`.opencode/plugins/intent-agent-gate.ts`) that was never actually built during the main implementation pass — found and fixed as part of running this probe, not before it.

**What was built.** A `tool.execute.before` plugin using Node's `spawnSync` (not the `$` shell API, to avoid any shell-quoting risk) to invoke the real gate, throwing on any non-zero exit — which naturally covers the fail-closed case too, since a build failure and a deny both produce a non-zero exit and both throw. Confirmed empirically (not assumed from docs) via a diagnostic plugin logging real `tool.execute.before` payloads: `input.tool` is `"write"`/`"edit"`, `output.args.filePath`, `output.args.content` (write), `output.args.oldString`/`newString` (edit) — **camelCase `newString`, not Claude Code's `new_string`**. The gate's own `StdinEditExtractor` only checked the snake_case form; fixed to check both, with a unit test locking it in.

**What was actually run.** Real `opencode run` CLI, real model (`openrouter/z-ai/glm-5.2`, confirmed against the user's own working screenshot), `--agent build --auto`, against an isolated scratch project (copied real `.agents/hooks/*` and the real generated plugin, not a reduced re-implementation):

- **Illegal action** — told to directly hand-edit a file listed in a real `managed-files.xml`. The `Write` call failed with exactly the gate's deny message; the agent reported it couldn't do it and named the correct path (edit the model, regenerate). Verified independently: file byte-identical afterward, `git status` clean.
- **Legal action** — told to edit a plain, unmanaged `README.md`. Succeeded normally, verified independently by reading the file's final content.

This is the one row in this matrix backed by an actual harness run end to end, not documentation or a partial/broken hook.

## Kiro — real run, and it fails, but not for the reason you would guess

Run against `kiro-cli.exe` **2.22.0** (reporting `KAS (Kiro Agent Server) 0.66.0`), logged in, invoked
as `kiro chat --v3 --no-interactive --trust-all-tools` against this repo with the module's own
generated `.kiro/hooks/intent-agent-gate.json` in place.

**Attempt 1 — the generated config.** Told to hand-edit
`.agents/instructions/module-building-workflow.instructions.md`, a real `managed-files.xml` path.
**The edit succeeded**; verified by hash (`e04f9e5f…` → `ce228063…`) and reverted.

**Attempt 2 — the decisive diagnostic.** Replaced the config with a single `PreToolUse` hook using a
catch-all `".*"` matcher whose command was `echo FIRED >> .kiro/hook-fired.txt; exit 2` — it cannot
fail to block if it runs at all. **The sentinel file was never created, and the edit succeeded
again.** So the hook is not being invoked; this is not a matcher mismatch, a schema error, or an
exit-code-contract problem. Kiro's CLI simply never runs the file.

Ruled out: no hook-related setting exists (`kiro settings list` is empty), and hooks are not in the
agent config (`kiro agent create` scaffolds twelve keys, none of them `hooks`).

**The generated config is, as far as the documentation goes, correct.** Kiro's v3 docs put hooks at
`.kiro/hooks/` in the project root, with `"version": "v1"`, a `hooks[]` array, PascalCase triggers
(`SessionStart` / `PreToolUse` / `Stop`) and `"action": { "type": "command", … }` — which is exactly
what this module emits. Blocking is documented as *any* non-zero exit for `PreToolUse`, with stderr
returned to the agent. So the shape is right and the feature is documented; this build just does not
implement it. Treat as **Kiro-side, pending a build that executes hooks** — retest on a later CLI.

**One real module bug found anyway, and it survives the above.** The generated `matcher` is
`"Write|Edit|ApplyPatch"` — Claude Code's tool names. Kiro's are its own internal ones (its `--trust-tools`
help documents `fs_read,fs_write`, and the 2.x hook reference matched on `fs_write` / `execute_bash`).
Even once Kiro runs hooks, that matcher would not match anything. **Deliberately not "fixed" by
guessing**: the v3 engine reports tools by display name (`Read File`, `Replace in File`) and its
internal v3 names could not be observed, because no hook ever executed to reveal them. Swapping one
unverified matcher for another proves nothing. Fix it when a build runs hooks and the real names can
be read off a firing hook.

Minor, non-blocking: the `; test $? -eq 0 && exit 0 || exit 2` wrapper is redundant on Kiro, whose
documented contract already blocks on any non-zero exit. Harmless, and it keeps one command shape
across harnesses.

## Also learned, not yet acted on

- **Codex can run on a plain API key** — `printenv OPENAI_API_KEY | codex login --with-api-key`
  (the flag reads from stdin). OpenAI's docs state API-key auth "supports local Codex workflows"
  with only cloud/workspace features limited, so a local hook probe is unaffected. This removes the
  subscription blocker on the Codex row.
- **GitHub Copilot has a real, enforcing `preToolUse` hook**, GA on the Copilot CLI, configured at
  `.github/hooks/*.json`. Its documented contract is **fail-closed** — "a crash or non-zero exit
  (including exit `2`) denies the tool call" — which matches the universal contract this module
  already generates. Two caveats worth carrying into any future support: command-hook **timeouts
  are fail-open**, as are HTTP hooks; and there are open bugs on `preToolUse` not firing in
  subagents and under parallel tool calls. Copilot is a plausible sixth target, not currently
  generated for.

## Still to do

- Run the Codex and Cursor probes — Codex is now unblocked via API-key auth; Cursor still needs a
  human with it installed.
- Re-run Kiro on a CLI build that actually executes `.kiro/hooks/*.json`, and fix the tool-name
  matcher once the real names are observable.
