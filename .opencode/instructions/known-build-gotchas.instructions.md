---
applyTo: '**/*.cs'
description: "Intent Architect module build and template-authoring gotchas, and diagnosing a module change that never reaches generated output."
keywords: [intent architect, template authoring, nuget, build, gotchas, intent ignore, asset repositories]
contentHash: 2CD16FD529846C9548EEEE16D70045BE8957FE698B3C0A815075807113140677
---
## Known Build Gotchas

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

That is normally enough: an **already-installed** module is re-detected and re-installed once repackaged, so neither a version bump nor an explicit `install_or_update_modules` belongs in the routine fix.

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
