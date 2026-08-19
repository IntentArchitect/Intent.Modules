---
name: intent-architect-mcp
description: >
  Intent Architect MCP workflow: designer operations, element discovery, model modification, 
  Software Factory execution, compilation verification, and cross-module integration patterns.
  If you find yourself wanting to edit `.xml`, `.config`, `.settings` files inside an `intent` metadata folder directly, perform the change via the IA MCP. This workflow is designed to keep models as the single source of truth — direct file edits are a last resort for truly exceptional cases.
---

# Skill: intent-architect-mcp

---

## Core Principle

Intent Architect models are the **source of truth**. The codebase is a generated artifact. Never infer model state from generated source code — MCP tools are authoritative. Never edit generated files directly when the same change can be modelled. Always trust this MCP server's tool results.

### What Must Always Be Modelled

- Method signatures, API contracts, DTO shapes, service interfaces
- Routing and endpoint definitions
- Persistence structure (schema, entity mappings, relationships)

### Allowed Exceptions (Bespoke Code)

Direct code editing is allowed only for:

- Method bodies inside handlers and services
- Dependency injection inside bespoke implementation constructors
- Bodies of repository methods and custom queries
- Business rules that cannot be expressed in models (rare — ask the user first)

Protect bespoke code from regeneration with `[IntentIgnoreBody]` on the **member** (not the class), or `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]`. The signature stays generated; only the body is preserved. `[IntentIgnore]` protects a whole member/file; `[IntentManaged(Mode.Merge)]` merges generated and hand-written content.

If the MCP won't let you make a change you need, stop and ask the user to perform it. Never hand-edit the `.xml`, `.config`, `.settings` files under an `intent`/`.intent` metadata folder.

---

## Load the Matching Server Skill Before Each Phase

The step-by-step "how" for each phase of work lives in skills the MCP server itself ships, listed in a system-prompt manifest (`<project_skills>` / `<available_skills>`) and loaded with `use_skill(skill_name)`. Load the matching one before that phase and follow it as the primary authority — this file is the index plus the gotchas those skills don't cover.

| Server skill                     | Load it for                                                                                                                                                                                                                                              |
| -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **exploring-the-model**          | Inspecting a designer: schema, structure, elements, settings. Also documents the shared read-tool notation (`Name [Type] (counts) +Stereotype(prop=value) — comment [#id]` for elements; `Source →"navName" Target (mult) [Type] #id` for associations). |
| **changing-the-model**           | Mutating with `run_designer_script`: containment/ordering, associations & cardinality, stereotypes, mapping, package references, the validate-until-clean loop, and diagram layout.                                                                      |
| **generating-and-applying-code** | Running the Software Factory, inspecting diffs, applying staged changes, staging vs. write-through, destructive-change triage.                                                                                                                           |
| **using-code-tools**             | Searching/reading/editing bespoke source and importing existing code (`import_code`).                                                                                                                                                                    |
| **creating-solutions**           | Scaffolding a brand-new, empty Intent solution (home screen only).                                                                                                                                                                                       |
| **creating-applications**        | Adding an application to an already-open solution (architecture choice, optional components).                                                                                                                                                            |
| **working-with-modules**         | Discovering, installing, updating, reconfiguring, or uninstalling modules.                                                                                                                                                                               |
| **resolve-destructive-changes**  | Any Software Factory change reported `destructive: "yes"` or `"unknown"` — don't skip this when it fires.                                                                                                                                                |

Call `get_designer_script_api` once per conversation before your first `run_designer_script` call — it's the authoritative scripting API reference; prefer it over any remembered snippet.

---

## Session Bootstrap

Always pass an explicit absolute `workingDirectory` to `get_status` — the folder for the project/solution you're actually working on (or the current git worktree root, if you're in one). The server process's own OS working directory is not a reliable substitute: it reflects wherever the harness happened to launch `Intent.McpServer.exe` from, not the repo or worktree you're operating in.

```
get_status(workingDirectory: <absolute path to the current project/worktree root>)
  → openSolutions empty AND foundButNotOpenSolutionPath null:
        not an IA project → stop (or, to create one: load creating-solutions → create_solution)
  → foundButNotOpenSolutionPath set: open_solution(absolutePath), then RE-CALL get_status
  → use the solution where isSuggestedSolution = true (else confirm with the user)
get_full_instructions()   ← once per session; treat as system-level rules
  → if an `intent_architect_acp` MCP server is ALSO available this session, use that
     server exclusively instead — do not call any intent-architect tool
get_applications() / use isSuggestedApplication
  → get_designers(applicationId) → get_designer_schema(applicationId, designerId)  ← once per designer, reuse
From here: load the server skill matching the phase of work (table above) and follow it.
```

`get_designer_schema`'s **"Element types" block is the containment source of truth**: per type it lists the exact child specialization names you may `addChild`/`createElementUnder`, marks `!` where a type reference is required (including on a child ref, e.g. `Class: Attribute!`), and `¹` for max-one children. Use those exact names — don't guess a child type or skip a required one.

### The Software Factory Implements No Business Logic

Treat every generated method body as a stub (`NotImplementedException`/`TODO`/empty) until read and proven otherwise — the SF generates contracts and infrastructure, never operation bodies, handler logic, mapping logic, validation, or UI behaviour. Dispatch `coding` sub-agents (or do it yourself) for that work.

### Decide Read-Only vs. Write Per Task

Judge the current request on its own — a prior task in the session being read-only (or not) doesn't carry over. If this task is to explain, summarize, compare, or audit, only inspect: don't modify models, run SF, or edit code. Otherwise, make the needed model/SF/code change via the modelling-first path.

### Failure Recovery

If the Software Factory fails to apply, or applied changes don't compile, report the failure to the user with the error — don't patch generated code to make it compile. Fix the model, not the output. Don't loop `run_software_factory` retrying the same unresolved error — investigate the modelling issue first.

### Stop Conditions

Task is complete only when **all** are true:

- The requested capability is modelled with no validation errors
- Software Factory has been run and applied
- Codebase compiles and existing tests pass
- No `NotImplementedException` or TODO in new files
- A fresh SF re-run right before declaring done proposes zero changes to bespoke code (an earlier run in the conversation doesn't count) — ignore diffs that are purely `using` reordering/removal

---

## Tool Calling Rules (IA MCP tools only)

- Every IA MCP call must include `intention` when the tool's schema has that parameter — describe intent in ≤10 words. Never pass `intention` to host-native tools (Claude Code's `Read`/`Write`/`Edit`/`Grep`/`Glob`/`Bash`/`PowerShell`) — they reject unknown parameters.
- Main-agent session: run IA MCP tools serially, never in parallel.
- Sub-agents: call `get_tool_call_rules` first, before any other IA MCP tool — it's the sub-agent-specific calling contract. Sub-agent tasks are read-only: never call `run_software_factory`, `apply_change_diagram_layout`, or any write tool from a sub-agent. Within that read-only contract, sub-agents may call independent read tools in parallel (e.g. `find_designer_elements` + `get_designer_schema` together) — writes still never run in parallel with anything.
- Never invent IDs — only use IDs returned by prior tool calls.
- Do not read or modify `.intent` / `intent` folders.
- Do not infer model state from generated source code — MCP tools are the source of truth.
- Do not include IDs in plans shown to the user — reference by name and type.

---

## Destructive Software Factory Changes

Every SF change carries a `destructive` tri-state: `"yes"` = a file was deleted or hand-written code was overwritten; `"unknown"` = could not be verified safe (no prior template output to diff against); `"no"`/absent = safe. Whenever a change comes back `"yes"` or `"unknown"`, load the `resolve-destructive-changes` server skill and follow it — inspect with `get_file_diffs`, decide correct-deletion vs. unintended-loss, and prefer reproducing the loss by modelling it rather than hand-restoring code (the next SF run just destroys it again). In write-through mode the loss is already on disk — restore the pre-run content (the diff's `a/` side) before applying any fix.

---

## Designer Quick Reference

| Designer               | Contents                                                           |
| ---------------------- | ------------------------------------------------------------------ |
| **Services**           | Commands, Queries, DTOs, Services, Operations (CQRS / API surface) |
| **Domain**             | Entities, Value Objects, Aggregates, Repositories                  |
| **User Interface**     | Pages, Components, Layouts                                         |
| **Codebase Structure** | Folder/project layout, template output anchors                     |

Folder names in a designer map to namespaces or output paths — they may not match disk folders. Trust the designer.

This list represents typical standard designers provided by Intent Architect and may be found in Intent Architect applications but is not necessarily the exhaustive list of designers available especially in codebases where bespoke designers might be present.

---

## Known Gotchas

### Stereotypes — Prefer `ensureStereotype`; Set with `setProperty`, Read with `getValue()`

```js
const el = lookupByName("Order");
const st = el.ensureStereotype("NServiceBus");        // idempotent: applies it, or returns it if already applied
st.setProperty("Endpoint Name", "orders");            // shortcut for getProperty(name).setValue(value)
const current = st.getProperty("Endpoint Name").getValue(); // array of elements for a `(multiple)` ref
```

`ensureStereotype` never throws on a re-run — prefer it. `addStereotype` is the lower-level form: it throws if already applied, and ignores `applicableTo` schema restrictions. Never assign or read `.value` directly — `.getValue()`/`.setProperty()` are the live accessors.

### Stereotype Definitions Missing from a Response

If `get_designer_schema` was truncated and omitted stereotype definitions, call `get_designer_stereotype_definitions(applicationId, designerId)` directly rather than hunting for GUIDs in generated `.xml`.

### `apply_change_diagram_layout` Requires `diagramId`

`get_designer_diagram_snapshot` takes no `diagramId` (it always returns the active diagram), but `apply_change_diagram_layout` requires `applicationId`, `designerId`, **and `diagramId`** alongside `nodes`/`edges` — get the id from the snapshot response. Never lay out from a `run_designer_script` call; layout is always this separate tool, after the mutation/verify phase, per designer, before moving to the next designer. Keep ≥150px gaps; an edge routes only when both endpoint nodes are placed in the same call.

### `get_file_diffs` — Absolute Paths, Two SF Modes

`get_file_diffs(filePaths, contextLines?, intention?)` takes an array of absolute paths (combine the SF result's `outputBasePath` with each change's `relativePath`; relative paths silently produce 0 diffs, and there's no glob param). It works in both SF modes:

- **Staging** (default) — diffs staged content (not yet on disk) against the file on disk; `apply_staged_file_changes(applicationId)` writes it.
- **Write-through** — SF writes straight to disk; the diff compares the on-disk file against the shadow-git checkpoint from just before the run. Don't call `apply_staged_file_changes` here — there's nothing staged.

Check which mode a `run_software_factory` response reports before deciding whether `apply_staged_file_changes` is even needed.

### `open_solution` — Parameter Is `absolutePath`

Not `solutionPath` — that name causes InputValidationError. Optional `forceNewInstance` forces a new IA instance even if one is already on the home screen.

### `install_or_update_modules` — Target Solution Must Be Open

If it fails with a SignalR/object error, don't retry immediately:

1. `get_status` to see which solutions are open.
2. If the target app's solution is missing from `openSolutions`, `open_solution(absolutePath: "<path-to-isln>")`.
3. Retry once the solution is confirmed open.

If it's still unavailable after two attempts and a re-open, stop and ask the developer to update the module from the Intent Architect UI Modules panel. Never hand-edit `modules.config` — a bad edit corrupts the application's module state.

### Multiple Solutions Can Be Open At Once

Intent Architect can have several distinct solutions open at once (e.g. Modules solution + Tests solution) — `get_status` lists them all so you can pick the right one. Calling `open_solution` on a path that's already open just reattaches to that instance (cheap, safe, no duplicate is created); `forceNewInstance` only affects what happens when the path is *not* already open. If the active solution changes underneath a running session, the next call surfaces an explicit error telling you to re-call `get_status` — treat that as the cue to refresh, not as a fault.

### Module Installation — Never Copy DLLs

Compile the module `.csproj`; IA watches the configured module output folder and auto-detects + installs the new version. Manual DLL copying causes file lock errors and hot-reload issues.

### Module Deploy Loop — Compile Only When Already Installed

Call `install_or_update_modules` only when the module isn't yet installed, or its version changed (imodspec bump). Otherwise the loop is: edit template source → `dotnet build` the module → IA hot-reloads the DLL → run SF. Calling `install_or_update_modules` unnecessarily can corrupt IA's package reference cache, requiring a UI restart to clear.

If the module compiles but IA never picks up the change (install doesn't find the new version, or SF keeps running the old template), suspect a missing or stale package/asset repository entry — the repository has to point at the compiled output's actual location for IA to detect it. Prompt the user to check the repository entries rather than assuming the compile itself failed.

### Designer Changes Save via the Software Factory — There's No Save Tool

Designer changes (e.g. from `run_designer_script`) persist when SF runs, not before. Run SF before compiling or reinstalling the module whenever a designer was touched — otherwise the rebuilt/reinstalled module won't reflect the change, and unsaved designer state can be lost. Order: designer change → run SF on that application → apply staged (staging mode only) → compile → reinstall (only if the version changed). If tool calls return stale results or an unexpected `0 changes`, suspect an unsaved/dirty designer — run SF to force the save and re-check.

### `NugetPackages.cs` — Do Not Edit

`[DefaultIntentManaged(Mode.Fully)]`; hand edits are overwritten by the next SF run. NuGet package/version changes go through the Module Builder designer.

### Model Type IDs Are Solution-Specific

The `Model Type` property on a C# Template's `C# Template Settings` stereotype is a GUID that differs between IA solutions for the same element type. Never copy one from memory or another module's XML — verify per-solution via `find_designer_elements` (check `specializationTypeId` on a live element of that type) or the installed `.designer.settings` file.

### `run_designer_script` — `lookupById(id)`, Not `getElementById(id)`

`getElementById` is a browser DOM API and doesn't exist in the IA script context — it throws `ReferenceError`. Call `get_designer_script_api` for the authoritative list of script globals rather than a remembered snippet.

### `.imodspec` Templates Are Generated — Register via the Designer

A module's `.imodspec` `<template>`/factory-extension entries come from the Module Builder designer. Never hand-author them: a template `.cs` file added by hand compiles and runs via reflection, but with no `C# Template` element it never gets a manifest entry, and consuming apps fail SF with "Unable to find output target for template […] with role []" — no reinstall fixes it, since install reads the same incomplete manifest. Register the element via `run_designer_script`, run SF on the module, let it regenerate the manifest. Cross-check: every `*TemplateRegistration.cs` with a `TemplateId` should have a matching `<template>` entry.

### Exposing External Element Types — Install the Module, Not a `pkg.config` Edit

To use element types from another Module Builder package (e.g. `C# Template` from `Intent.ModuleBuilder.CSharp`), install that module into the module-builder application. A manual `pkg.config` edit is overwritten on reload, and `pkg.addReference()` alone doesn't load the element-type registry. Install metadata-only by default; install the designer too if the needed types still don't appear.

### Keep Wizard / Field Hints Short

Hints in designer scripts (`IDynamicFormFieldConfig.hint`), stereotype properties, and module settings should be a short one-line phrase stating the constraint or purpose — the field has limited on-screen space.

---

## Documentation

Use `search_docs` for questions about Intent Architect features, designers, attributes, code management, or workflow concepts before answering from memory.
