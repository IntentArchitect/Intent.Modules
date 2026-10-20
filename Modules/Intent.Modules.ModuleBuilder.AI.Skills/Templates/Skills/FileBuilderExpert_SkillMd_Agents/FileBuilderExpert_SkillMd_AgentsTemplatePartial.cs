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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.FileBuilderExpert_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileBuilderExpert_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.FileBuilderExpert_SkillMd_Agents";

        internal const string SkillName = "file-builder-expert";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FileBuilderExpert_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Author, fix or debug a C# code-generation template using the Fluent CSharpFile builder API — constructor structure, callback priorities, type resolution, DI parameter injection, statements generated from a designer-modelled mapping (CSharpClassMappingManager, custom mapping resolvers), and logging a runtime value from module code. USE ONLY WHEN writing, reviewing, converting or debugging a *TemplatePartial.cs that emits C# via CSharpFile, or a factory extension that changes one. DO NOT USE FOR reading stereotypes or properties off a model (see intent-metadata-consumer), cross-module DI and startup wiring (see intent-module-orchestrator), or designer-model and script changes. REQUIRES the target template's model shape (single-file vs file-per-model) already decided."
                    argument-hint: "[source file] [target template name]"
                    template-id: {{TemplateId}}
                    ---

                    # File Builder Expert

                    > [!TIP]
                    > **Read more if you want to know about** builder APIs, patterns, designer mappings, or troubleshooting and logging:
                    > *   [API Cheatsheet](./resources/api-cheatsheet.md) | [Patterns](./resources/builder-patterns.md) | [Mapping Cheatsheet](./resources/mapping-cheatsheet.md) | [Troubleshooting](./resources/troubleshooting.md)
                    > *(To conserve tokens, avoid reading these for minor updates.)*

                    ## Musts
                    1. Inherit from `CSharpTemplateBase<TModel>`, implement `ICSharpFileBuilderTemplate`, and expose `CSharpFile`.
                    2. Initialize `CSharpFile` structure in constructor.
                    3. Implement config/transform methods: `DefineFileConfig() => CSharpFile.GetConfig();` and `TransformText() => CSharpFile.ToString();`.
                    4. Use flow builders (`AddIfStatement`, `AddForEachStatement`) and `CSharpInvocationStatement`.
                    5. Register `OnBuild`/`AfterBuild` callbacks in the constructor (or, for another module's template, from a factory extension). They run in **two separate phases**: every `OnBuild` of every template first, then every `AfterBuild` — so an `AfterBuild` always runs after all `OnBuild` work, whatever the numbers. Priority only orders callbacks within one phase (lower runs first; default 0); the conventional bands are **Core=0, Enrichment=100, Extension=500, Final=1000**. A callback registered from inside another callback is queued into the running phase, so that is allowed too — only an `OnBuild` registered after the `OnBuild` phase has ended throws.
                    6. A callback that looks up something another template's callback adds must run after it: a higher priority in the **same phase**, or an `AfterBuild` to see anything added in `OnBuild`.
                    7. Resolve types inside callbacks/lambdas, never directly in constructor.
                    8. Resolve emitted type positions through the Type System APIs: `GetTypeName(...)` for model/type references, `GetTypeName(templateId, model)` for TemplateId-based references, and `UseType("Namespace.Type")` for framework/external types — **including method return/parameter types** (e.g. `method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken")`; for the usual optional `CancellationToken cancellationToken = default`, `method.AddOptionalCancellationTokenParameter()` does it in one call). Do not use `UseType(...)` for types represented in the Intent model.
                    9. For generated type declarations, put the final class/interface name on the template model/provider (for example `IHasName.Name`) and use the same model when resolving references via `GetTypeName(templateId, model)`; handle name collisions before `AddClass(...)`.
                    10. Inject DI parameters using `param.IntroduceReadonlyField()`.
                    11. For a handler or method body a developer or AI implements, emit `IgnoreBody()` and a stub. The stub is written when the method first appears and never put back:
                        ```csharp
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                        method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
                        ```
                    Only when the body is in Merge mode, because the module's `CONTEXT.md` or the developer says generated and hand-written code share it, put `method.AddStatement("// IntentInitialGen");` above the `throw`. That lets the developer replace the stub without the Software Factory restoring it. See `module-building-strategies` § "Managed modes" for choosing the mode.
                    12. **Metadata has two uses — know both.** (a) **Your own cross-step state:** `node.AddMetadata("key", value)` in one callback, read back in a later one. (b) **Finding which designer element a generated member came from.** The builder records it when a template calls `member.RepresentsModel(element)` — read it back from `member.RepresentedModel` (added in `Intent.Common.CSharp` 3.8.5: raise the `Intent.Common.CSharp` `<dependency>` (`.imodspec`) and the `Intent.Modules.Common.CSharp` package reference (`.csproj`) to 3.8.5, with `Intent.Modules.Common` / the `Intent.Common` `<dependency>` at 3.7.3, or restore fails with `NU1605`; see `known-build-gotchas`). Separately, many templates stamp the element under the key `"model"` (`AddMetadata("model", element)`), so `method.TryGetMetadata<TModel>("model", out var m)` finds it; that key is a **convention the host template follows, not something the builder adds**, so check that the template you read actually sets it. Guard every read: `GetMetadata` throws on an absent key, returns `null` (it does not throw) when the value is not a `T`, and only accepts reference types (`GetMetadata<bool>` does not compile) — prefer `TryGetMetadata<T>`. `AddMetadata` throws on a duplicate key. See `intent-module-orchestrator` § "The Model Bridge".
                    13. Know that build callbacks run **only for templates whose `CanRunTemplate()` is true** when each phase starts. A template that returns `false` runs none of its `OnBuild`/`AfterBuild` callbacks — including any a factory extension registered on it — so never rely on a callback to switch a template back on.
                    14. Check that another template exists with `ExecutionContext.TemplateExists("TemplateIdOrRole")`, inside a build callback. Add the predicate — `TemplateExists("TemplateIdOrRole", t => t.CanRunTemplate())` — **only** when you need to know its file will actually be written: some modules register stub templates on purpose and switch them off with `CanRun = false` in an `AfterBuild(…, 100)`, so their types still resolve while no file is written. The predicate reports such a template as absent once it has switched off, and as present before that. See `builder-patterns.md` § "Checking Whether Another Template Exists".
                    15. Edit a collection item or a switch-case statement (`Remove`, `Replace`, `InsertAbove`, `InsertBelow`, `AddInvocation`) only on `Intent.Common.CSharp` 3.10.12 or later. Check your feed first: if `run_solution_query` → `searchAvailableModules("Intent.Common.CSharp", { includePrerelease: true })` does not list 3.10.12 or later, these edits do not work for you yet, so use the older-floor approach below. Otherwise raise the `Intent.Common.CSharp` `<dependency>` (`.imodspec`) and the `Intent.Modules.Common.CSharp` package reference (`.csproj`) to 3.10.12, with `Intent.Modules.Common` / the `Intent.Common` `<dependency>` at 3.11.0 and the `Intent.SoftwareFactory.SDK` package reference at 3.14.0 (Intent Architect 5.0.0 or later), or restore fails with `NU1605` (see `known-build-gotchas`). Before 3.10.12 those statements have no `Parent`: the edits throw, and `AddInvocation` silently changes nothing. On an older floor, keep a reference to the collection or case block and edit its `Statements` list directly. See `builder-patterns.md` § "Editing Collection Items and Switch-Case Statements".
                    16. Mark statements other templates must find with metadata (`AddMetadata("my-module:role", true)`) and match on that key, never on rendered text — for a catch block especially, since `ExceptionType` holds whatever string its producer passed. See `builder-patterns.md` § "Finding Another Template's Statements".
                    17. Emit `[IntentManaged]` attributes through the typed `CSharpIntentManagedAttribute` — `IgnoreBody()`, `Ignore()`, `Merge()`, `Fully()` — not an attribute name plus argument strings.

                    ## Must Nots
                    1. Never emit structural C# as raw strings outside the fluent API.
                    2. **Never ship an obsolete builder API.** Build with warnings visible and treat any `CS0618` "…is obsolete" warning as a failure — switch to the replacement the warning names. Known traps: `CSharpMethodChainStatement` / `AddMethodChainStatement`; `field.WithAssignment(string)` (see #5); and passing a **base-typed `CSharpStatement`** to `AddStatement(...)`, which binds to the obsolete `AddStatement(TParent, CSharpStatement, Action<CSharpStatement>)` overload — pass a `string` (collection-initializer items) or the **concrete** statement subtype (e.g. `CSharpObjectInitializerBlock`) so the generic `AddStatement<TParent, TStatement>` overload is selected.
                    3. Never add `else`/`catch`/`finally` as children of a block (must be siblings).
                    4. Never use raw string interpolation for lambda arrows `=>` or object initializer braces `{}`.
                    5. Never call obsolete `field.WithAssignment(string)` directly (use `WithAssignment(new CSharpStatement(...))`).
                    6. **Never reach a single-instance template through `AddTypeSource` or a hand-added using.** `AddTypeSource(templateIdOrRole)` makes designer type references (`GetTypeName(attribute)`, `GetTypeName(typeReference)`) resolve to the templates generated *per model*; it does nothing for a template that has one instance and no model. For that, call `GetTypeName("TemplateIdOrRole")` with no model, inside a build callback: it finds the single instance, returns its type name **and adds its using**. See `builder-patterns.md` § "Resolving Single-File Template Namespaces".
                    7. **An unconditional `AddUsing("Namespace")` goes dead silently.** It cannot notice when the code that justified it later stops spelling out a type from that namespace — two shipped bugs came from exactly this: a using added for `GetMethod`/`Invoke` reflection calls survived after the code was rewritten to reach them entirely through `var`, and a using added for a collection type survived after the type name was moved into a raw statement string, invisible to the builder. Resolve the type through `UseType("Namespace.Type")` and interpolate the result — even into a raw statement string — so the using tracks the reference and disappears when the reference does. Reserve `AddUsing` for a namespace your code needs independent of any spelled-out type (e.g. an extension-method namespace).
                    8. Never put `?? throw` after `GetTypeName("TemplateIdOrRole"[, model])` or `GetTemplate<T>(...)`. They already throw when no template matches, so the `?? throw` never runs. When the other template is optional, use `TryGetTypeName(...)` / `TryGetTemplate<T>(...)` and branch on the result.
                    9. Never derive your own factory extension from `CSharpFileBuilderFactoryExtension` — that is the SDK's own extension, not a base class. Derive from `FactoryExtensionBase` (`Intent.Modules.Common.Plugins`) and register callbacks on other templates in `OnAfterTemplateRegistrations`.
                    10. Never reference an `[Obsolete]` role constant to support an older module's template, and never silence the warning with `#pragma`. Probe the role by its string value — `TryGetTypeName("Application.Eventing.EventBusInterface", out var legacyType)` — before the current role. See `builder-patterns.md` § "Probing a Legacy Role".

                    ## Designer Mappings

                    When a template generates statements from a mapping modelled in the designer (details and samples in
                    [mapping-cheatsheet.md](./resources/mapping-cheatsheet.md)):

                    1. Configure replacements with `SetFromReplacement(...)` / `SetToReplacement(...)` on a `CSharpClassMappingManager`, then generate with `GenerateCreationStatement(...)` or `GenerateUpdateStatements(...)`. Resolve assignments through mapping element ids and paths, never property names, and never hardcode property-to-property assignments.
                    2. A terminal node has a drawn mapped end (`MappingModel.Mapping != null`); an object node has `Mapping == null` and only `Children`. With no resolver match, the manager falls back to `DefaultCSharpMapping` for terminals and `MapChildrenMapping` for object nodes. Never treat an object or collection mapping as a scalar terminal.
                    3. A custom mapping type implements `ICSharpMapping`, normally by inheriting `CSharpMappingBase`. For nulls use the real members — `GetSourceStatement(true)`, the protected `GetSourcePathText(path, targetIsNullable)` and `GetNullableAwareInstantiation(...)`, and from `Intent.Common.CSharp` 3.9.2 `GetTargetStatement(true)` and `IfNotNullMapping`. There are no "Null-Safe" or "Validate All" mapping options.
                    4. A custom resolver matches on `MappingModel.MappingTypeId`, implements the two-argument `ResolveMappings(mappingModel, next)`, and hands anything it does not handle on with `return next?.Invoke(mappingModel);`. Never return `null` for a node you don't handle (every later resolver then gets a null `next`), and never guard with `if (mappingModel.Mapping == null) return ...;` (object nodes would never reach the resolver). Register it with `AddMappingResolver(resolver, priority)`: lower runs first, and the default is 0.
                    5. The two-argument resolver needs `Intent.Common.CSharp` 3.9.2 and the priority overload 3.9.1: raise the `Intent.Common.CSharp` `<dependency>` (`.imodspec`) and the `Intent.Modules.Common.CSharp` package reference (`.csproj`) to 3.9.2, with `Intent.Modules.Common` / the `Intent.Common` `<dependency>` at 3.7.3, or restore fails with `NU1605` (see `known-build-gotchas`).

                    ## Seeing A Runtime Value

                    There is no debugger for module code: it runs inside the Software Factory. Log the value with
                    `Logging.Log.Debug($"[AI-DEBUG] ...")` (`Intent.Utils`), rebuild the module with `dotnet build --no-incremental`
                    and confirm the reinstall picked it up, then run the Software Factory and search the newest file in
                    `<OS temp>/IntentArchitect/logs/software-factory/` for your prefix. Without the rebuild the old module runs
                    and no line appears. Remove the lines once you have the value. Levels, file naming and the module-tasks log are in
                    [troubleshooting.md](./resources/troubleshooting.md) § 15.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
