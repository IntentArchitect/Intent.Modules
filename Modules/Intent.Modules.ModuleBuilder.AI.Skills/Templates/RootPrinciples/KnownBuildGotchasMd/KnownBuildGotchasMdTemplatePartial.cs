using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.RootPrinciples.KnownBuildGotchasMd
{
  [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
  public class KnownBuildGotchasMdTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
  {
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.RootPrinciples.KnownBuildGotchasMd";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public KnownBuildGotchasMdTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
      WithContentHashing = true;
      MarkdownFile = new MarkdownFile("known-build-gotchas.instructions", relativeLocation: "")
        .FromMarkdown("""""""
          ---
          applyTo: '**/*.cs'
          description: "Intent Architect module build and template-authoring gotchas, and diagnosing a module change that never reaches generated output."
          keywords: [intent architect, template authoring, nuget, build, gotchas, intent ignore, csproj, nullable, asset repositories]
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

          ### Constructor vs Build Callbacks — NuGet In The Constructor, Lookups In A Callback

          A template's constructor and its `OnBuild` / `AfterBuild` callbacks run at different points of the Software Factory run, and two kinds of work depend on that timing.

          - **NuGet dependencies must be added before they are collected.** Packages are collected from every template when NuGet dependencies are resolved, at `BeforeTemplateExecution` (Order 1000). The constructor and `OnBuild` both run before that, so either works: the constructor is the simplest place, and registering conditionally there (on a setting or the model) is fine; `OnBuild` suits a package that depends on what a lookup finds. `AfterBuild` callbacks run in that same phase but after collection, so a package added there is silently never installed.
          - **Template lookups go in `OnBuild` / `AfterBuild`.** `GetTypeName`, `TryGetTypeName` and `FindTemplateInstance` resolve against other templates' instances. A constructor runs while templates are still being registered, so a lookup there succeeds or fails depending on registration order. The rule itself is `file-builder-expert` Musts #7.
          - **"Will that other template generate its file?" is an opt-in check.** `ExecutionContext.TemplateExists(idOrRole)` is true for any registered instance, including stubs: some modules register a template only so its types still resolve, then set `CanRun = false` (in the registration, or in an `AfterBuild` at order 100) so it writes nothing. When you need the file to really exist, add the predicate `t => t.CanRunTemplate()`, and evaluate it in an `AfterBuild` ordered above 100 so those stubs have already switched off. Do not add it to type lookups — resolving a stub's type is the reason the stub exists.

          ```csharp
          public MyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
          {
              AddNugetDependency("Some.Package", "1.2.3"); // constructor or OnBuild; AfterBuild is too late

              CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                  .AddClass("MyService")
                  .OnBuild(file =>
                  {
                      // lookups resolve here, once every template is registered
                      if (TryGetTypeName("MyModule.AppSettings", out var settingsType))
                      {
                          file.Classes.First().AddProperty(settingsType, "Settings");
                      }
                  })
                  .AfterBuild(file =>
                  {
                      // opt-in: a template with this role exists AND will generate its file
                      if (ExecutionContext.TemplateExists("MyModule.Registrations", t => t.CanRunTemplate()))
                      {
                          file.Classes.First().AddMethod("void", "Register");
                      }
                  }, 200);
          }
          ```

          ===

          ### `SingleFileListModel` — Name The File When It Holds Several Types

          `CSharpFile` names its output after the first type declared in it (interfaces first, then classes, records and enums) unless you name it yourself.

          - **One type per template** — the normal case — needs nothing. The type's name is the file name.
          - **Several types in one file**, for example one class per model in a `SingleFileListModel` template: set the name with `.WithFileName(...)`. Otherwise the first type comes from the first model, in designer order, so adding, removing or renaming a model can rename the file. Sorting the models does not prevent that.

          ```csharp
          // the constructor's first argument is the namespace, the second the relative location
          CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
              .WithFileName("MessageContracts");

          foreach (var item in Model)
          {
              CSharpFile.AddClass(item.Name);
          }
          ```

          ===

          ### A Template's `Role` Picks Where Its Output Lands

          A template's `Template Settings` in the Module Builder designer carry a `Role` and a `Default Location`, and they do different jobs.

          - **`Role`** names the consuming application's `Output Anchor` to place the template under. On install, Intent Architect looks for an anchor with that name in the consumer's Codebase Structure and puts the template's `Template Output` there. With no matching anchor, the output lands in the designer's root folder, usually not where you want it.
          - **`Default Location`** is only a sub-folder path under that anchor. Blank means directly under it.

          So set `Role` to an anchor the consumer actually has, for example the `Domain` or `Application` anchor an architecture template creates, and use `Default Location` for any sub-folder. An output, once placed, stays put on a reinstall: moving it to a corrected default takes uninstalling and reinstalling the module. See [Configuring a Template's default output location](https://docs.intentarchitect.com/articles/module-building/templates-general/configuring-a-templates-default-output-location/configuring-a-templates-default-output-location.html).

          ===

          ### The Module's `.csproj` — Set Project Properties In The Model

          A module's own `.csproj` is generated from its `C# Project (.NET)` element in the Visual Studio designer. Properties that element models (`TargetFramework`, `Nullable`, `LangVersion`, `ImplicitUsings` and others) are re-applied from the model on every Software Factory run, so a hand edit to one of them is reclaimed. To change nullable reference types, set `C# Project Options` → `Nullable` on that element and regenerate.

          Package versions behave differently. Under the default overwrite behaviour the Software Factory only ever raises a package version, never lowers it, so a version you raised by hand stays. A lower version you pinned is not reset on every run either; it is raised only when a template asks for a higher one.

          ===

          ### NuGet Package Downgrade Errors (NU1605)

          An SF run can leave SDK package versions that trigger `NU1605`. Correct them in the `.csproj`. The cause is NuGet versions drifting out of sync with the Intent module version — keep them aligned. Most affected: `Intent.Modules.Common`, `Intent.Modules.Common.CSharp`, `Intent.SoftwareFactory.SDK`.

          The usual trigger is raising one package by hand. Module Builder writes `Intent.Modules.Common` 3.7.2, and a raised `Intent.Modules.Common.CSharp` (or a designer package such as `Intent.Modules.Modelers.Services`) depends on a higher `Intent.Modules.Common`, so restore fails with `error NU1605: Detected package downgrade: Intent.Modules.Common from 3.7.3 to 3.7.2`. Raise the `Intent.Modules.Common` package reference to at least the version the error names, and the `.imodspec` `Intent.Common` `<dependency>` to match. For `Intent.Modules.Common.CSharp`: 3.8.5 to 3.9.2 need `Intent.Modules.Common` 3.7.3, 3.9.6 needs 3.9.1, 3.10.12 needs 3.11.0. Each raise can pull another. `Intent.Modules.Common`, some `Intent.Modules.Common.CSharp` versions and some designer packages depend on a higher `Intent.SoftwareFactory.SDK`, which Module Builder writes at 3.7.0: `Intent.Modules.Common` 3.9.0 and 3.9.1 need SDK 3.10.0, 3.9.2 needs 3.12.0, 3.10.x needs 3.13.0 and 3.11.x needs 3.14.0. `Intent.Modules.Common` 3.11.x also needs `Intent.Persistence.SDK` 1.0.3, which matters once a Version Migration has added its 1.0.1-alpha.2 reference. Restore reports one downgrade at a time, so raise the reference it names, restore again, and repeat until it is clean. A higher SDK needs a newer Intent Architect: SDK 3.10.0 needs 4.5.15, 3.11.0 needs 4.5.18, 3.12.0 needs 4.5.21, 3.13.0 needs 4.6.0 and 3.14.0 needs 5.0.0. Never suppress `NU1605` with `<NoWarn>`: restore then keeps the lower `Intent.Modules.Common` underneath a package built against a higher one.

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
          """"""");
    }

    [IntentManaged(Mode.Fully)]
    public override IMarkdownFile MarkdownFile { get; }

    [IntentManaged(Mode.Fully)]
    public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

  }
}
