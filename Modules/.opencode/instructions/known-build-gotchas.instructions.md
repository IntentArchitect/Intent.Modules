---
applyTo: '**/*.cs'
description: >
Template authoring pitfalls and solutions: NuGet dependency registration,
keywords: [intent architect, template authoring, nuget, build, gotchas, constants]
contentHash: F19E6242A956CC55B58BD89DC322D76288392A26EEC8DD86D8FEFE78C313402D
---
## Known Build Gotchas

### NuGet Dependencies — Not Inside `OnBuild`

Declare NuGet dependencies in the **template constructor**, not inside `OnBuild` or `AfterBuild` lambdas. Ideally declare them at the top of the constructor, but conditional registration based on model state is valid mid-constructor. The rule is strictly: never inside a build callback.

```csharp
// Correct — constructor body
public MyTemplate(...)
{
AddNugetDependency(NuGetPackages.SomePackage);
if (model.NeedsExtra()) AddNugetDependency(NuGetPackages.ExtraPackage);

CSharpFile = new CSharpFile(...);
}

// Wrong — inside OnBuild
OnBuild(file =>
{
AddNugetDependency(...); // ❌ will not work reliably
});
```

===

### `SingleFileListModel` — Filename Instability

When a template uses `SingleFileListModel` and generates multiple classes via `foreach`, `CSharpFile` derives its filename from the **first class added**. If that order is non-deterministic, the filename changes between SF runs.

The anchor-class approach (adding a dummy first class) is awkward and not recommended. For exceptional cases, use:

```csharp
// IntentIgnore
CSharpFile = new CSharpFile("DesiredFileName", folderPath)
```

`// IntentIgnore` prevents SF from overwriting that line, letting you hardcode the filename directly. Reserve this for genuinely exceptional multi-class single-file scenarios — the normal pattern is one class per template output.

===

### `FilterMessagesForThisMessageBroker` — Pass `ExecutionContext`, Not `this`

The three-argument overload requires an `ISoftwareFactoryExecutionContext`. Passing `this` (the template instance) compiles but fails silently at runtime — the filter returns incorrect results.

```csharp
// Correct
FilterMessagesForThisMessageBroker(messages, selector, ExecutionContext);

// Wrong
FilterMessagesForThisMessageBroker(messages, selector, this); // ❌
```

===

### `Constants` Class Name Conflict

If your module defines a `Constants` class, it conflicts with `Intent.Modules.Constants` from the SDK. Use an alias:

```csharp
using NServiceBusConstants = Intent.Modules.Eventing.NServiceBus.Templates.Constants;
```

===

### NuGet Package Downgrade Errors (NU1605)

You may encounter SDK package versions after an SF run, triggering `NU1605` downgrade errors. When this happens, manually correct the affected package versions in the `.csproj`. The root cause is NuGet versions drifting out of sync with the corresponding Intent module version — keep them aligned to avoid recurrence.

Packages most commonly affected: `Intent.Modules.Common`, `Intent.Modules.Common.CSharp`, `Intent.SoftwareFactory.SDK`.

===

### Template Changes Not Taking Effect

Building a module compiles the `.csproj` that represents it, and the step that packages the `.imod` runs off that compilation. If your changes were to non-C# files, the compilation may not trigger, the package step is skipped, and no new `.imod` is produced — the templates then keep generating from the previously packaged content, with nothing reported.

To force it:

```
dotnet build --no-incremental
```
