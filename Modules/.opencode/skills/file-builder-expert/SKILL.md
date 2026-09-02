---
name: file-builder-expert
description: "Author or fix a C# code-generation template using the Fluent CSharpFile builder API — constructor structure, callback priorities, type resolution, and DI parameter injection. USE ONLY WHEN writing, reviewing, or converting a *TemplatePartial.cs that emits C# via CSharpFile. DO NOT USE FOR templates that build Markdown/text output (see add-module-skill-template) or for designer-model/script changes. REQUIRES the target template's model shape (single-file vs file-per-model) already decided."
argument-hint: "[source file] [target template name]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.FileBuilderExpert_SkillMd_Agents
contentHash: 64D5C139FA2CA97AF33EDB2A776BAB1402EC8C89839186729D0CA4B6C6019A31
---
# File Builder Expert

> [!TIP]
> **Read more if you want to know about** builder APIs, patterns, or troubleshooting:
> *   [API Cheatsheet](./resources/api-cheatsheet.md) | [Patterns](./resources/builder-patterns.md) | [Troubleshooting](./resources/troubleshooting.md)
> *(To conserve tokens, avoid reading these for minor updates.)*

## Musts

1. Inherit from `CSharpTemplateBase<TModel>`, implement `ICSharpFileBuilderTemplate`, and expose `CSharpFile`.
2. Initialize `CSharpFile` structure in constructor.
3. Implement config/transform methods: `DefineFileConfig() => CSharpFile.GetConfig();` and `TransformText() => CSharpFile.ToString();`.
4. Use flow builders (`AddIfStatement`, `AddForEachStatement`) and `CSharpInvocationStatement`.
5. Register `OnBuild`/`AfterBuild` callbacks in constructor (**Core=0, Enrichment=100, Extension=500, Final=1000**).
6. Lookup callbacks must use higher priority than target template.
7. Resolve types inside callbacks/lambdas, never directly in constructor.
8. Resolve emitted type positions through the Type System APIs: `GetTypeName(...)` for model/type references, `GetTypeName(templateId, model)` for TemplateId-based references, and `UseType("Namespace.Type")` for framework/external types — **including method return/parameter types** (e.g. `method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken")`). Do not use `UseType(...)` for types represented in the Intent model.
9. For generated type declarations, put the final class/interface name on the template model/provider (for example `IHasName.Name`) and use the same model when resolving references via `GetTypeName(templateId, model)`; handle name collisions before `AddClass(...)`.
10. Inject DI parameters using `param.IntroduceReadonlyField()`.
11. For not-implemented handlers/method bodies, use:

```csharp
    method.AddStatement("// IntentInitialGen");
    method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
    method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
```

12. **Metadata has two uses — know both.** (a) **Your own cross-step state:** `node.AddMetadata("key", value)` in one callback, read back in a later one. (b) **Reading the designer model the host template already attached:** a node generated from a modelled element is stamped with that element under the well-known key `"model"`, so `method.TryGetMetadata<TModel>("model", out var m)` tells you which designer element a generated member came from — classes, methods, properties and parameters each carry their own. Guard every read (`GetMetadata` throws on an absent key; `AddMetadata` throws on a duplicate one). See `intent-module-orchestrator` § "The Model Bridge".

## Must Nots

1. Never emit structural C# as raw strings outside the fluent API.
2. **Never ship an obsolete builder API.** Build with warnings visible and treat any `CS0618` "…is obsolete" warning as a failure — switch to the replacement the warning names. Known traps: `CSharpMethodChainStatement` / `AddMethodChainStatement`; `field.WithAssignment(string)` (see #5); and passing a **base-typed `CSharpStatement`** to `AddStatement(...)`, which binds to the obsolete `AddStatement(TParent, CSharpStatement, Action<CSharpStatement>)` overload — pass a `string` (collection-initializer items) or the **concrete** statement subtype (e.g. `CSharpObjectInitializerBlock`) so the generic `AddStatement<TParent, TStatement>` overload is selected.
3. Never add `else`/`catch`/`finally` as children of a block (must be siblings).
4. Never use raw string interpolation for lambda arrows `=>` or object initializer braces `{}`.
5. Never call obsolete `field.WithAssignment(string)` directly (use `WithAssignment(new CSharpStatement(...))`).
6. **`AddTypeSource(templateId)` is not sufficient for single-file templates.** `AddTypeSource` enables `GetTypeName(model)` resolution for *file-per-model* templates — it looks up the template instance by model. For *single-file* templates (one output, no model) like `IIntegrationEventHandler`, there is no model to pass, so `GetTypeName` cannot resolve the type and the using is never injected. Use the pattern in `builder-patterns.md` § "Resolving Single-File Template Namespaces" instead.
