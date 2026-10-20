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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_ResourcesTroubleshootingMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileBuilderExpert_ResourcesTroubleshootingMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.FileBuilderExpert_ResourcesTroubleshootingMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FileBuilderExpert_ResourcesTroubleshootingMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("troubleshooting", relativeLocation: "file-builder-expert/resources")
                .FromMarkdown(""""""
                    # File Builder Troubleshooting

                    Indexed failure modes for CSharpFile-based templates.

                    ===

                    ## 1. `ToString` Before Build Completion

                    **Symptom:** `Build() needs to be called before ToString()`  
                    **Cause:** `TransformText()` was called before the builder lifecycle ran. Usually means `ICSharpFileBuilderTemplate` is not implemented, so the framework never triggers `Build()`.  
                    **Fix:** Implement `ICSharpFileBuilderTemplate` on the template class. Keep `TransformText` as `return CSharpFile.ToString();` only.

                    ===

                    ## 2. Empty Structural Output

                    **Symptom:** Exception: `Either a file must use top level statements or at least one type must be specified for C# file`  
                    **Cause:** `CSharpFile.GetConfig()` found no class, interface, record, enum, or top-level statements. The Software Factory reads the file config (`DefineFileConfig()` → `CSharpFile.GetConfig()`) straight after constructing the template, before any `OnBuild` callback has run — so a type added only in a callback is too late. The config takes its file name from the first type.  
                    **Fix:** Add the primary type in the constructor — `.AddClass(Model.Name)` with no body is enough — and fill it in from callbacks.

                    ===

                    ## 3. Invalid `OnBuild` Timing

                    **Symptom:** `This file has already been built. Consider registering your configuration in the AfterBuild(...) method.`  
                    **Cause:** `OnBuild(...)` was called after the `OnBuild` phase had ended — for example, inside an `AfterBuild` callback, or from a factory extension's `OnBeforeTemplateExecution`.  
                    **Fix:** Register `OnBuild` callbacks in the constructor, in a factory extension's `OnAfterTemplateRegistrations`, or from inside another `OnBuild` callback (that one is queued into the running phase). From an `AfterBuild` handler, register another `AfterBuild` instead.

                    ===

                    ## 4. Invalid `AfterBuild` Timing

                    **Symptom:** `The AfterBuild step has already been run for this file`  
                    **Cause:** `AfterBuild(...)` was registered after the lifecycle already completed the `AfterBuild` phase.  
                    **Fix:** Register `AfterBuild` callbacks in the constructor, in a factory extension's `OnAfterTemplateRegistrations`, or from inside an `OnBuild` or another `AfterBuild` callback (queued into the running phase) — never from code that runs once files are being written.

                    ===

                    ## 5. Pending Configuration Delegates

                    **Symptom:** `Pending configurations have not been executed. Please contact support@intentarchitect.com for assistance.`  
                    **Cause:** The file reached the end of the `AfterBuild` phase with `OnBuild` callbacks that never ran. Each phase only runs callbacks for templates whose `CanRunTemplate()` is true when the phase starts, so a template that returned `false` during the `OnBuild` phase and `true` by the `AfterBuild` phase keeps its `OnBuild` queue — typically because `CanRun` was set back to `true`, or `CanRunTemplate()` depends on state that changes between the two phases.  
                    **Fix:** Decide whether the template runs once, before the build phases (in the constructor or the registration), and do not switch `CanRun` from `false` back to `true` afterwards.

                    ===

                    ## 6. Metadata-Resolution Failures

                    **Symptom:** `KeyNotFoundException`, or a `NullReferenceException` later on, during post-processing — or compile error `CS0452` on `GetMetadata<bool>`  
                    **Cause:** `GetMetadata<T>(key)` throws `KeyNotFoundException` when the key was never set. When the value is there but is not a `T`, it returns `null` without throwing, and the `NullReferenceException` surfaces wherever the value is used. `T` must be a reference type, so `GetMetadata<bool>` does not compile.  
                    **Fix:** Use `TryGetMetadata<T>` (any `T`; false when the key is absent or the value is another type), or guard with `HasMetadata`. Do not assume metadata set by one template is always present when consumed by another (different execution orders, optional features).

                    ```csharp
                    // Unsafe: throws if the key is absent; null (no exception) if the value is not a MyState
                    var state = method.GetMetadata<MyState>("my-state");

                    // Safe, and the only option for value types:
                    if (method.TryGetMetadata<bool>("my-key", out var flag) && flag)
                    {
                        // use flag
                    }
                    ```

                    ===

                    ## 7. Mismatched `TemplateId`

                    **Symptom:** Template not discovered, or registration resolves to wrong implementation  
                    **Cause:** `TemplateId` constant in the template class and the value passed to the registration class differ (case-sensitive).  
                    **Fix:** Define `TemplateId` as a `public const string` in the template and reference it by name in the registration:

                    ```csharp
                    // Template:
                    public const string TemplateId = "My.Module.MyTemplate";

                    // Registration (SingleFileTemplateRegistration is not generic):
                    public class MyTemplateRegistration : SingleFileTemplateRegistration
                    {
                        public override string TemplateId => MyTemplate.TemplateId;  // reference the constant

                        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget) => new MyTemplate(outputTarget);
                    }
                    ```

                    ===

                    ## 8. Wrong Registration Type

                    **Symptom:** Template runs once but model-specific files are not generated (or vice versa)  
                    **Cause:** Using `SingleFileTemplateRegistration` when the template should produce one file per model element.  
                    **Fix:**

                    | Scenario | Registration base |
                    |----------|-------------------|
                    | One output file | `SingleFileTemplateRegistration` |
                    | One file per model element | `FilePerModelTemplateRegistration<TModel>` — must also override `GetModels` |
                    | Event/pipeline-driven | `ITemplateRegistration` directly |

                    ===

                    ## 9. Usings don't follow members relocated to another file

                    **Principle:** usings belong to the **file that emits the member**, and the builder only adds them for type references it can *track* — i.e. ones resolved through the type system (`UseType` / `GetTypeName`) against **that file's** template. So whenever generated members are emitted into a file other than the current template's own (code-behind, an aggregating/partial file, a sibling template, a file authored via `OnBuild`/`FindClass` on another template), the relevant usings only follow if you resolve types against the *destination* template.
                    **Symptom:** A relocated/extracted member fails to compile — types or attributes (`Task`, `[SupplyParameterFromForm]`, `[Required]`, `IEnumerable<>`, …) not found — even though the same code compiled in its original file.
                    **Cause:** (a) members were added with **raw type strings** the builder can't track; and/or (b) the destination file doesn't inherit the source's implicit imports (e.g. a plain `.cs` gets none of Razor's `_Imports`; global usings differ per project); and/or (c) the type was resolved against the **wrong** template, so the using landed on the source file, not the destination.
                    **Fix:** Resolve every type/return/attribute through the **destination block's** template — `targetBlock.File.Template.UseType("Namespace.Type")` (e.g. `code.File.Template.UseType(...)` where `code` is the destination class; `code.Template` does not compile) — so the using lands on the file that holds the members. Add `.RemoveSuffix("Attribute")` for attribute names. Ensure the destination template exposes the right context, e.g. `public override ICSharpCodeContext RootCodeContext => CSharpFile.Classes.Single();`. Types referenced only inside **raw statement/expression strings** are never tracked — interpolate a `UseType(...)` into the string, or add the namespace explicitly with `CSharpFile.AddUsing(...)`. See Must Not #7 in `SKILL.md`.

                    ===

                    ## 10. `FindMethod` Returns Only the First Overload

                    **Symptom:** When a class has multiple overloaded methods with the same name (e.g. two `HandleAsync` methods for different event types), only the first overload is modified or enriched — subsequent overloads are silently ignored.  
                    **Cause:** `cls.FindMethod("HandleAsync")` returns the first match from that class's own `Methods` — it is not recursive (no nested types, no statement tree).  
                    **Fix:** Use LINQ to enumerate all matching methods:

                    ```csharp
                    // Wrong — only finds the first overload
                    var firstOnly = cls.FindMethod("HandleAsync");

                    // Correct — handles all overloads
                    var methods = cls.Methods.Where(m => m.Name == "HandleAsync");
                    foreach (var method in methods)
                    {
                        // apply changes to each overload
                    }
                    ```

                    ===

                    ## 11. Double semicolon from `AddReturn` wrapping a statement-type expression

                    **Symptom:** Generated returns emit a stray second semicolon, e.g. `return await Task.FromResult(value); ;`.
                    **Cause:** `method.AddReturn(stmt)` already appends the `;`. `CSharpObjectInitializerBlock` renders as an expression (no own semicolon), so `AddReturn` works cleanly with it — but `CSharpInvocationStatement` (and other statement-type nodes) default to rendering **with** their own trailing `;`, which then doubles up.
                    **Fix:** Call `.WithoutSemicolon()` on the inner statement before handing it to `AddReturn` (or to `.AddArgument(...)` / any expression position):

                    ```csharp
                    // Wrong — invocation keeps its own ';', AddReturn adds another → "...); ;"
                    method.AddReturn(new CSharpInvocationStatement("await Task.FromResult").AddArgument(value));

                    // Right — invocation renders as an expression; AddReturn supplies the single ';'
                    method.AddReturn(new CSharpInvocationStatement("await Task.FromResult").AddArgument(value).WithoutSemicolon());
                    ```

                    Rule of thumb: any `CSharp*Statement` placed in an expression slot (return value, argument, init value, a lambda's `WithExpressionBody(...)`) needs `.WithoutSemicolon()`; object-initializer blocks already behave as expressions. Without it a lambda body renders `services => services.AddOptions();` inside the call.

                    ===

                    ## 12. Missing or doubled `var`, or a missing `;`, on an assignment

                    **Symptom:** `var var dto = ...`, or an assignment line with no trailing `;`.  
                    **Cause:** `new CSharpVariableDeclaration(name)` writes `var ` itself, so passing `"var dto"` doubles it. A `CSharpAssignmentStatement` writes no `;` of its own — the right-hand side must end with one.  
                    **Fix:** Pass only the variable name, and put `.WithSemicolon()` on the right-hand side:

                    ```csharp
                    method.AddStatement(new CSharpAssignmentStatement(
                        new CSharpVariableDeclaration("result"),
                        new CSharpStatement("await service.GetAsync()").WithSemicolon()));

                    method.AddAssignmentStatement("var total", new CSharpStatement("items.Count").WithSemicolon());
                    ```

                    Inside a block (`if`, `foreach`, `try`, a lambda), do not call `.WithSemicolon()` on the **assignment** itself: it returns the base `CSharpStatement`, so `block.AddStatement(...)` binds to the obsolete overload (`CS0618`).

                    ===

                    ## 13. `Cannot remove statement from unknown parent` on a collection item or switch case

                    **Symptom:** `InvalidOperationException: Cannot remove statement from unknown parent` (or `Cannot insert statement for unknown parent`) from `Remove()`, `Replace(...)`, `InsertAbove(...)` or `InsertBelow(...)` — or `AddInvocation(...)` on a collection item that changes nothing.  
                    **Cause:** Before `Intent.Common.CSharp` 3.10.12, items added with `CSharpCollectionExpression.AddItem(...)` and statements added with `CSharpSwitchCodeBlock.AddStatement(string)` have no `Parent`.  
                    **Fix:** Raise the module's `Intent.Common.CSharp` dependency to 3.10.12 (`.imodspec` `<dependency>` and `.csproj` package reference) with `Intent.Modules.Common` 3.11.0 and `Intent.SoftwareFactory.SDK` 3.14.0 (Intent Architect 5.0.0 or later), or restore fails with `NU1605`. That needs 3.10.12 or later on your feed: if `run_solution_query` → `searchAvailableModules("Intent.Common.CSharp", { includePrerelease: true })` does not list it, use the alternative. The alternative works on any version: keep a reference to the collection / case block and edit its `Statements` list directly. If `FindStatement` returns `null` for an item of `options.X = [ ... ]`, that is a separate limit: search does not enter an assignment's right-hand side — find the assignment first, then search its `Rhs`. See `builder-patterns.md` § "Editing Collection Items and Switch-Case Statements".

                    ===

                    ## 14. Callbacks on a template never run

                    **Symptom:** An `OnBuild` / `AfterBuild` callback — your own, or one a factory extension registered on another template — has no effect, and no error is raised.  
                    **Cause:** Build callbacks only run for templates whose `CanRunTemplate()` is true when each phase starts. A template that is switched off (its `CanRunTemplate()` returns `false`, or something set `CanRun = false`) runs none of them.  
                    **Fix:** Check the target template's `CanRunTemplate()` before relying on its callbacks. To act only when the other template's file will really be written, check `ExecutionContext.TemplateExists("TemplateIdOrRole", t => t.CanRunTemplate())` inside a callback of your own template — see `builder-patterns.md` § "Checking Whether Another Template Exists" for the stub-template caveat.

                    ===

                    ## 15. Seeing a runtime value

                    When a template, factory extension or registration misbehaves and you need the actual value of something
                    at generation time, log it. There is no debugger to attach: module code runs inside Intent Architect's
                    own Software Factory process. (A designer script is different: `run_designer_script` returns its
                    `console.log` output directly.)

                    ```csharp
                    using Intent.Utils;

                    Logging.Log.Debug($"[AI-DEBUG] Customer.Name={model.Name}, Attributes={model.Attributes.Count}");
                    ```

                    `Logging.Log` is an `Intent.Engine.ITracing` with four levels only — `Debug`, `Info`, `Warning` and
                    `Failure` (which also takes an exception). There is no `Error`, `Trace` or `Fatal`, and no templated
                    `{Placeholder}` overload, so build the whole string yourself.

                    The output is not exposed through any tool, environment variable or config file. It lands in
                    `<OS temp directory>/IntentArchitect/logs/software-factory/sf-{contextId}-{pid}-{date}.log`:

                    - Resolve the temp directory on the machine running Intent Architect (`$env:TEMP` on Windows, `$TMPDIR`
                      falling back to `/tmp` on macOS and Linux); never hardcode it.
                    - The file is day-rolling and per process, so there is no fixed name. After the run, pick the most
                      recently modified `.log` file in that folder.
                    - Code that runs as a module task rather than during a Software Factory run logs to the sibling
                      `logs/module-tasks/` folder instead.
                    - The file also carries Intent Architect's own log lines, so prefix yours with something distinctive
                      (`[AI-DEBUG]`) and search for it.

                    Rebuild the module and reinstall it (or run the Software Factory on the target application), read the
                    value, then remove the temporary log lines — they are for diagnosis, not shipped output.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
