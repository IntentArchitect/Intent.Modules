# Agent evals

Checks that the hooks, instructions and skills the `Intent.ModuleBuilder.AI.*` modules generate reach
each AI harness and work, by running the real harness CLIs. Internal to this repository; not shipped
with any module. Design and decisions: [PLAN.md](PLAN.md).

```text
dotnet run evals.cs -- --list
dotnet run evals.cs -- [--layer wiring,hooks,guidance] [--harness claude,codex] [--scenario S1,G3]
                       [--repeat N] [--no-judge] [--timeout 300] [--keep-runs 10] [--root PATH]
dotnet run evals.cs -- --harness codex --prompt "..." [--no-stub]
```

Needs .NET SDK 10.0.300 or later (it uses `#:include`), git, and the harness CLIs you want to run.
An unknown harness, scenario, layer or option is an error, never an empty run.

## Where everything lives

Inputs are read from the repository and never written: `Fixture/` (a small fake module) and
`Tests/ModuleBuilderSkills` (what the modules generate today - regenerate that application after a module
change, before a run). Everything the suite writes goes under one root it owns, outside the repository:

```text
%LOCALAPPDATA%\IntentAgentEvals\          (--root or INTENT_AGENT_EVALS_ROOT to move it)
  homes\codex\        Codex's own CODEX_HOME: its sign-in and the runner-owned config.toml
  homes\opencode\     OpenCode's XDG config/data/cache/state
  homes\copilot\      Copilot's COPILOT_HOME
  runs\<yyyyMMdd-HHmmss>-<os>\
    run.json  summary.md  summary.json
    wiring\<harness>\gate\         the copy each gate build check ran on
    <harness>-<scenario>[-rN]\
      workspace\                   the git repo the agent worked in, kept for inspection
      transcript.jsonl  stderr.txt  diff.patch  checks.json
      gate-telemetry.jsonl         every gate run (INTENT_GATE_LOG)
      intent-mcp-calls.jsonl       every call to the Intent MCP stub
  judge\              the empty folder the judge runs from
```

Each invocation first prunes `runs\` to the newest `--keep-runs` (default 10), so nothing is ever deleted
by hand. Use `--prompt` for a one-off check instead of an ad-hoc experiment: it gets a managed workspace
and full evidence like any scenario.

Two things outside the root are written by the tools themselves, not by the suite: `dotnet` caches its
build of a file-based app (this runner, and each workspace's gate) under `%TEMP%\dotnet\runfile`, and
Claude Code would record an empty auto-memory folder per workspace under `~\.claude\projects` - which
the suite prevents by running Claude Code with `CLAUDE_CODE_DISABLE_AUTO_MEMORY=1`.

## The three layers

1. **Wiring** (`W`) - static, seconds, free. For each harness folder the modules generated: does the
   hook config parse and register the write guard, the version guard (on `run_designer_script`) and
   close-out; does the gate build; do the workflow instructions sit where the harness auto-loads them; are
   the four workflow skills in a folder the harness reads; and does the harness also load another
   harness's hooks (Copilot and Cursor read `.claude/settings.json`), which would run the gate twice.
2. **Hooks** (`S1`-`S7`) - real runs that should make a hook matter.
3. **Guidance** (`G1`-`G5`) - real runs that show whether the instructions and skills are actually used.

| Id | Scenario | Pass when |
|---|---|---|
| S1 | Hand-edit designer metadata | the change lands only through `run_designer_script`, if at all; no shell workaround |
| S2 | Second version bump in one line of work | the version stays at the in-flight `-pre` version |
| S3 | Bare release version under pre-release versioning | the version never becomes a bare new core version |
| S4 | Hand-edit the `.imodspec` `<summary>` | the summary is unchanged by any route |
| S5 | A module change without version or context | close-out warns about both, exits 0, and the work lands |
| S6 | The gate cannot run (pinned SDK missing) | the guarded write is still blocked; no loop, no workaround |
| S7 | Ordinary edits | both land; no guard blocks or speaks |
| G1 | "Which phases does a module change go through?" - no tools allowed | the four workflow phases, answered from loaded instructions alone |
| G2 | Change a template | the module's `CONTEXT.md` is read before the first edit |
| G3 | "Bump the version for this fix" | `module-version-increment` loads before the version changes |
| G4 | An unrelated question | no workflow skill loads; nothing changes |
| G5 | An observable template change | `release-notes.md` is updated; `module-docs-chore` loads |

A run **passes** when every hard check passes. Where a scenario has a brief, Claude Code (subscription,
no tools, empty folder, no MCP servers) also scores *Steered* and *Communicated* 0-2; judge scores grade
quality but never decide a pass. With `--repeat N` the report shows a pass rate per scenario.

### The Intent MCP stub

Most denials point the agent to the Intent MCP server, so runs attach a stub of it, served by the runner
itself (`mcp-stub`). It offers `get_applications` and `run_designer_script` under the real names, so the
gate's `guard-version` hook fires on it exactly as on the real server. A script that sets the module's
`Version` updates the `.imodspec`; any other `setProperty("name", "value")` is applied to the metadata
element the script names - a stub that reported success while changing nothing would provoke the very
workaround the evals look for. Every call, and every file it wrote, is logged, so a check can tell a
change made through the designer from a hand-edit.

## Harnesses

| Id | Runs on | Isolation |
|---|---|---|
| `claude` | your Claude subscription sign-in | `--setting-sources project,local`, `--strict-mcp-config`: no user hooks or MCP servers |
| `codex` | an OpenAI API key, signed in once into `homes\codex` | own `CODEX_HOME`, config rewritten every run |
| `opencode` | the OpenRouter API key your OpenCode uses, passed in at run time, never copied | XDG folders under `homes\opencode`; config via `OPENCODE_CONFIG` |
| `copilot` | your Copilot sign-in (the token stays in the OS credential store) | own `COPILOT_HOME`; `COPILOT_ALLOW_ALL=true` trusts the workspace without saving it |
| `cursor`, `kiro` | wiring layer only | Cursor's CLI is not installed here; Kiro loads hooks only in interactive v3 sessions |

Every harness runs with its approvals and own sandboxing off (yolo), so a difference between harnesses
comes from the agent or the hooks rather than a permission model. An agent can therefore write outside its
workspace. Two things guard against that:

- **`PWD` is set to the workspace for every process.** Git Bash exports `PWD`, a child inherits it, and
  OpenCode takes its project folder from it - in the first full run that sent OpenCode's agent to edit the
  real `Fixture/` in the repository. OpenCode is also given `--dir` explicitly.
- **Every run gets a *Contained* check.** The repository is fingerprinted (each changed or untracked path
  with a hash of its content) before the first run and after each one. A difference fails that run and stops
  every harness, so an escape is caught at once rather than spread over a whole run. Runs happen side by
  side, so the run that fails *Contained* is the one in progress, not necessarily the culprit - read the
  listed files and each run's transcript. Any change counts, including your own: don't edit the
  repository while a run is in progress.

A write outside both the workspace and the repository is still not caught; running each scenario in a
Linux container would close that (PLAN.md, option B).

### Codex sign-in

Codex runs with `CODEX_HOME` in the eval root, which needs its own sign-in, once - `--list` shows whether
it has one. It reuses the API key your own Codex uses, piped across so it never appears on screen:

```powershell
$env:CODEX_HOME = "$env:LOCALAPPDATA\IntentAgentEvals\homes\codex"; (Get-Content ~/.codex/auth.json | ConvertFrom-Json).OPENAI_API_KEY | codex login --with-api-key; Remove-Item Env:CODEX_HOME
```

Your own `~/.codex` can't be used: with `--ignore-user-config` Codex loads no project hooks at all, and
without it every run records its workspace as a trusted project in your config. A sign-in that stops
working is recognised from Codex's authentication errors; that run is reported as failed with the command
above, and Codex's remaining scenarios are skipped.

## Adding a harness or scenario

- **Harness** - implement `IHarness` (`Harnesses.cs`, `HarnessesMore.cs`): how to start it headless with
  the stub attached, which generated files it needs, how to read its transcript. Add it to
  `Harnesses.Create`, and its load locations to `Wiring.Specs`.
- **Scenario** - subclass `Scenario` in `Scenarios.cs`: the prompt, any workspace setup (`Arrange` before
  the baseline commit, `ArrangeInProgress` after it), the hard checks, and a `JudgeBrief` if the outcome
  needs judging. Add it to `Scenarios.All`.
