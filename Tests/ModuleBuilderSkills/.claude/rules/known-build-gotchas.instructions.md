---
applyTo: '**/*.cs'
description: "Intent Architect module build and template-authoring gotchas, and diagnosing a module change that never reaches generated output."
keywords: [intent architect, template authoring, nuget, build, gotchas, intent ignore, asset repositories]
contentHash: 20A276FA88BBCE04477C44B6A259FC8BC74382A2AF073BBDB671C0C4424AA384
---
## Known Build Gotchas

### A Scaffolded Template File Is Part Designer-Owned, Part Yours

`*TemplatePartial.cs` and `*TemplateRegistration.cs` are scaffolded from the Module Builder designer and then **co-owned**. The `[IntentManaged(...)]` attributes say which half is which, member by member:

- `[IntentManaged(Mode.Fully)]` — **designer-owned**. A hand edit is overwritten on the next Software Factory run.
- `Body = Mode.Ignore`, or `Mode.Merge` — **yours**. Template logic is authored here, and regeneration preserves it.

The member that catches people is `TemplateId`:

```csharp
[IntentManaged(Mode.Fully)]
public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Skills.ModuleContextCapture_SkillMd_Agents";
```

It is **derived, not authored** — `{PackageName}.{FolderPath}.{ElementName}`, from the File Template element's own name and where it sits in the designer's folder tree. To change it, rename or move that element in the **Module Builder designer**, then regenerate. Editing the constant achieves nothing durable: the next run restores it, and until then every `Role` or template-id lookup resolving against the real id silently stops matching — which shows up as a template that mysteriously no longer participates, not as an error.

===

### NuGet Dependencies — Not Inside `OnBuild`

Declare NuGet dependencies in the **template constructor**, **never** inside an `OnBuild`/`AfterBuild` callback — registration there does not work reliably. Conditional registration mid-constructor is fine.

```csharp
AddNugetDependency(NuGetPackages.SomePackage); // constructor body, never a build callback
```

===

### `SingleFileListModel` — Filename Instability

When a template generates multiple classes via `foreach`, `CSharpFile` takes its filename from the **first class added** — so a non-deterministic order changes the filename between SF runs. The normal pattern is one class per template output (a dummy anchor class is not the answer). For genuinely exceptional multi-class files, hardcode it behind `// IntentIgnore`, which stops the Software Factory overwriting that line:

```csharp
// IntentIgnore
CSharpFile = new CSharpFile("DesiredFileName", folderPath)
```

===

### `FilterMessagesForThisMessageBroker` — Pass `ExecutionContext`, Not `this`

The three-argument overload needs an `ISoftwareFactoryExecutionContext`. Passing the template instance compiles but silently returns wrong results.

```csharp
FilterMessagesForThisMessageBroker(messages, selector, ExecutionContext); // never `this`
```

===

### `Constants` Class Name Conflict

A module-defined `Constants` class conflicts with `Intent.Modules.Constants` from the SDK. Alias it:

```csharp
using NServiceBusConstants = Intent.Modules.Eventing.NServiceBus.Templates.Constants;
```

===

### NuGet Package Downgrade Errors (NU1605)

An SF run can leave SDK package versions that trigger `NU1605`. Correct them in the `.csproj`. The cause is NuGet versions drifting out of sync with the Intent module version — keep them aligned. Most affected: `Intent.Modules.Common`, `Intent.Modules.Common.CSharp`, `Intent.SoftwareFactory.SDK`.

===

### Template Changes Not Taking Effect

Packaging the `.imod` runs off the module's `.csproj` compilation. Changes to non-C# files may not trigger that compilation, so no new `.imod` is produced and templates keep generating from the previously packaged content — silently. Force it:

```
dotnet build --no-incremental
```

Expect that to be enough on its own: an **already-installed** module is normally re-detected and re-installed once repackaged, and a version bump is never part of the routine fix.

Anticipate that re-installation, but **confirm it rather than assuming it** — check your change is actually present in the generated output. If it is not, re-install that module yourself at its current version with `install_or_update_modules`, then regenerate. Whether the automatic pickup happens is environment-dependent, and there is no reliable way to interrogate it from inside a session, so treat the check as routine rather than exceptional.

Re-install **one module at a time** — one module per call, one call at a time, never several in parallel — so a failed pickup stays attributable to the module you just touched.

From there the ladder below applies as normal.

===

### When A Module Change Still Does Not Appear

A clean regeneration proves nothing on its own — output that could not be rewritten reports no change either way. Work in order; skipping a step makes a later result meaningless.

1. **Regenerate** and read the result.
2. **Rule out protected output** — file-level ignore (`list_ignored_files` / `unignore_file`), `[IntentManaged(Body = Mode.Ignore)]` or `Mode.Merge` on a member, or `// IntentIgnore` on the line. If protected, the templates are fine; decide whether the protection is still wanted.
3. **Check the build reached the cache** — `<solutionFolder>\.cache\modules\<ModuleId>.<Version>\lib\`. The folder is named from the `.imodspec`, the assembly from the `.csproj`, so they differ: `Intent.Common.3.11.2\lib\` holds `Intent.Modules.Common.dll`. Match on the folder.
4. **Force a re-install** of the same version.
5. **Force a rewrite** — perturb the output and regenerate. Back with your change: the pipeline is live. Back unchanged: the change is not reaching the generator. **Never delete ignored or protected output** — Intent will not recreate it. (Unrelated to a Software Factory *destructive change*, which is a hazard to resolve, not a probe.)
6. **Stop and report what you ruled out.** **Do not renumber to force it** — see *"A version number is not a debugging tool"* in the module-building workflow instructions. A bump only takes effect once the application's install moves to it; if an authorised bump changes nothing, the fault is the template's logic.

===

### Module Not Discoverable For Install (Asset Repositories)

Narrow trigger: use this **only** when you cannot install or update the module *at all* — typically a first install. A change not appearing is the ladder above.

A locally-built `.imod` is only found if a repository entry points at where the packager drops it: the solution's `intent.repositories.config`, or the user's global **Asset Repositories** settings. Often neither needs touching. But if neither matches the path the build reports — `Successfully created module '<path>\<Module>.<Version>.imod'` — the build succeeds with 0 errors while the `.imod` lands where Intent never looks.

Your job is to **tell the user, not configure it**. Report the path and note an entry may be missing — adding one is their call. If they add one and it still does not surface, ask them to restart Intent Architect, then re-check.
