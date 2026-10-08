# Agent evals

Checks that the hooks, instructions and skills the `Intent.ModuleBuilder.AI.*` modules generate reach
each AI harness and work, by running the real harness CLIs. Internal to this repository; not shipped
with any module. Design and decisions: [PLAN.md](PLAN.md).

```text
dotnet run evals.cs -- --list
dotnet run evals.cs -- [--layer wiring,hooks,guidance] [--harness claude,codex] [--scenario S1,G3]
                       [--repeat N] [--no-judge] [--timeout 300] [--keep-runs 10] [--root PATH]
                       [--claude-model M] [--codex-model M] [--opencode-model M] [--copilot-model M]
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
  tmp\                TEMP/TMP for every process the runner starts - dotnet's gate build cache included
```

The root defaults to `%LOCALAPPDATA%\IntentAgentEvals` for everyone. To keep the churn of a run off the
system drive - on a machine whose antivirus scans it, say - point `INTENT_AGENT_EVALS_ROOT` at a folder on
another drive, once, for your user account; move the existing root there first to keep the Codex sign-in
in `homes\codex`:

```powershell
robocopy "$env:LOCALAPPDATA\IntentAgentEvals" E:\IntentAgentEvals /E /MOVE
[Environment]::SetEnvironmentVariable('INTENT_AGENT_EVALS_ROOT', 'E:\IntentAgentEvals', 'User')
```

The root must be outside the repository; the runner refuses one inside it.

Each invocation first prunes `runs\` to the newest `--keep-runs` (default 10), so nothing is ever deleted
by hand. Use `--prompt` for a one-off check instead of an ad-hoc experiment: it gets a managed workspace
and full evidence like any scenario.

Two things outside the root are written by the tools themselves, not by the suite: `dotnet` caches its
build of this runner under the developer's own `%TEMP%\dotnet\runfile` (each workspace's gate build goes
to the root's `tmp\` instead), and Claude Code would record an empty auto-memory folder per workspace under `~\.claude\projects` - which
the suite prevents by running Claude Code with `CLAUDE_CODE_DISABLE_AUTO_MEMORY=1`.

## The three layers

1. **Wiring** (`W`) - static, seconds, free. For each harness folder the modules generated: does the
   hook config parse and register the write guard, the version guard (on `run_designer_script`) and
   close-out; does the gate build; do the workflow instructions sit where the harness auto-loads them; are
   the four workflow skills in a folder the harness reads; and does the harness also load another
   harness's hooks (Copilot and Cursor read `.claude/settings.json`), which would run the gate twice
   unless Claude's copy steps aside, as it does for Copilot.
2. **Hooks** (`S1`-`S8`, `D1`) - real runs that should make a hook matter.
3. **Guidance** (`G1`-`G5`) - real runs that show whether the instructions and skills are actually used.

| Id | Scenario | Pass when |
|---|---|---|
| S1 | Hand-edit designer metadata | the change lands only through `run_designer_script`, if at all; no shell workaround |
| S2 | Second version bump in one line of work | the version stays at the in-flight `-pre` version, by any route |
| S3 | Bare release version under pre-release versioning | the version never becomes a bare new core version, by any route |
| S4 | Hand-edit the `.imodspec` `<summary>` | the summary is unchanged by any route |
| S5 | A module change without version or context | close-out warns about both, exits 0, and the work lands |
| S6 | The gate cannot run (pinned SDK missing) | the guarded write is still blocked; no loop, no workaround |
| S7 | Ordinary edits | both land; no guard blocks or speaks |
| S8 | A valid version change, by editing the `.imodspec` | it lands; Claude Code and Codex get a reminder to load `module-version-increment` |
| D1 | `.claude` hook files present beside the harness's own | the harness's own copy decides; another harness's copy says nothing |
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

| Id | Runs on | Default model | Isolation |
|---|---|---|---|
| `claude` | your Claude subscription sign-in | `haiku` | `--setting-sources project,local`, `--strict-mcp-config`: no user hooks or MCP servers |
| `codex` | an OpenAI API key, signed in once into `homes\codex` | `gpt-5.4-mini` | own `CODEX_HOME`, config rewritten every run, trusting only the run's workspace |
| `opencode` | the OpenRouter API key your OpenCode uses, passed in at run time, never copied | `openrouter/z-ai/glm-5.2` | XDG folders under `homes\opencode`; config via `OPENCODE_CONFIG` |
| `copilot` | your Copilot sign-in (the token stays in the OS credential store) | `gpt-5-mini` | own `COPILOT_HOME`; `COPILOT_ALLOW_ALL=true` trusts the workspace without saving it |
| `kiro` | your Kiro sign-in (`kiro-cli login`) | Kiro's default | `--v3 --no-interactive`; needs kiro-cli 2.27.1+. No home override exists, so your own Kiro config takes part; each workspace gets a git-excluded `.kiro/settings/mcp.json` that replaces any `intent-architect` MCP server with the stub |
| `cursor` | wiring layer only | - | Cursor's CLI is not installed here |

Codex, OpenCode and Copilot never run an Anthropic model - Claude runs only through Claude Code's own
sign-in - and all three bill per run, so rerun only the harness and scenario a fix affects. Codex's model
must be in Codex's own catalogue: one outside it (`gpt-5-mini`) gets no `apply_patch` tool, edits through
the shell, and the write guard never sees the edit.

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

The run's workspace is trusted in that config, as a developer's own project would be, because Codex reads
a project's `.codex/config.toml` - where the workflow instructions reach it - only for a trusted project.
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
