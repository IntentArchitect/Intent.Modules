---
contentHash: C2F1AAABAA9B80100B872B8BE178DADEFBD24B5E97028816D9DC13902B567DB7
---
# C# File Builder Cheat Sheet & Patterns

## Minimal Template Shape

```csharp
[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class SampleTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "My.Module.SampleTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public SampleTemplate(IOutputTarget outputTarget, object model = null)
        : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
        .AddUsing("System")
        .AddClass("Sample", @class =>
        {
            @class.AddConstructor(ctor =>
                ctor.AddParameter("string", "value", p => p.IntroduceReadonlyField()));
            @class.AddMethod("void", "DoWork", method =>
                method.AddStatement("// TODO"));
        });
    }

    [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }
    [IntentManaged(Mode.Fully)] protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();
    [IntentManaged(Mode.Fully)] public override string TransformText() => CSharpFile.ToString();
}
```

## Resolving Other Templates' Namespaces

Resolve another template's **type** rather than its namespace: `GetTypeName(...)` returns the type name and adds the using in one call, so the using tracks the reference. Do it inside a build callback (`file-builder-expert` Must #7).

```csharp
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass(Model.Name)
    .OnBuild(file =>
    {
        // One instance per model: pass the model too.
        var dtoType = GetTypeName("My.Module.DtoTemplate", Model);
        file.Classes.First().AddMethod(dtoType, "ToDto");
    });
```

Only when you need the namespace itself (no type name to emit), find the instance and take it:

```csharp
var configTemplate = ExecutionContext.FindTemplateInstance<IClassProvider>("My.Module.ConfigurationTemplate");
if (configTemplate != null)
{
    file.AddUsing(configTemplate.Namespace);
}
```

`FindTemplateInstance<T>` is for a template with one instance: it returns `null` when none matches and throws when more than one does. For a per-model template, call `ExecutionContext.FindTemplateInstances<T>("TemplateIdOrRole")`, which returns every matching instance (available at the `Intent.Common` 3.7.2 floor), rather than collecting them by hand.

## Resolving Single-File Template Namespaces

`AddTypeSource(templateIdOrRole)` makes **designer type references** resolve — `GetTypeName(attribute)`, `GetTypeName(typeReference)` — by finding the template instance generated *for that referenced element*. A single-file template (one instance, no model) is never found that way. Call `GetTypeName("TemplateIdOrRole")` with **no model** instead: it finds the single instance, returns its type name, and adds its using. It throws when no instance exists (or more than one does), so use `TryGetTypeName` when the template is optional.

```csharp
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass(Model.Name)
    .OnBuild(file =>
    {
        // Single-instance template: the id or role alone. Returns e.g. "IEventHandler" and adds its using.
        var handlerInterface = GetTypeName("My.Module.EventHandlerInterface");
        file.Classes.First().ImplementsInterface(handlerInterface);

        // Optional single-instance template:
        if (TryGetTypeName("My.Module.UnitOfWorkInterface", out var unitOfWork))
        {
            file.Classes.First().AddConstructor(ctor => ctor.AddParameter(unitOfWork, "unitOfWork", p => p.IntroduceReadonlyField()));
        }
    });
```

`IClassProvider` is from `Intent.Modules.Common.Templates` — already available in most template files.

## Checking Whether Another Template Exists

Ask the execution context, inside a build callback (the constructor runs while templates are still being registered):

```csharp
CSharpFile.OnBuild(file =>
{
    // Default: is there a template with this id or role at all?
    if (ExecutionContext.TemplateExists("My.Module.CacheInterface"))
    {
        file.Classes.First().AddMethod("void", "InvalidateCache");
    }

    // Opt-in: will its file actually be written?
    if (ExecutionContext.TemplateExists("My.Module.CacheInterface", t => t.CanRunTemplate()))
    {
        // ...
    }
});
```

Add the `t => t.CanRunTemplate()` predicate **only** when "will the file be written" is the real question. Some modules register **stub templates** on purpose: the stub is constructed so other templates can inspect it and resolve its types, then switches itself off with `CanRun = false` in an `AfterBuild(…, 100)`. With the predicate, the same stub reads as present until that `AfterBuild` has run and as absent after it, while `GetTypeName` resolves it the whole time. If what you need is the type, `TryGetTypeName` is the right check.

Build callbacks themselves only run for templates whose `CanRunTemplate()` is true when each phase starts — a template that returns `false` runs none of its `OnBuild` / `AfterBuild` callbacks, including any registered on it by a factory extension.

## Probing a Legacy Role

When an older version of another module publishes its template under a role that has since been renamed, probe the **old role string first**, then the current one. Do not reference an `[Obsolete]` role constant and do not silence it with `#pragma` — the string needs neither. In Intent's eventing modules, for example, the legacy event bus interface carries both roles, so the legacy probe must come first:

```csharp
CSharpFile.OnBuild(file =>
{
    string busType;
    if (TryGetTypeName("Application.Eventing.EventBusInterface", out var legacyBusType))
    {
        busType = legacyBusType;   // older module version
    }
    else if (TryGetTypeName("Application.Eventing.MessageBusInterface", out var messageBusType))
    {
        busType = messageBusType;  // current module version
    }
    else
    {
        return;                    // no bus installed
    }

    file.Classes.First().AddConstructor(ctor => ctor.AddParameter(busType, "bus", p => p.IntroduceReadonlyField()));
});
```

## Editing Collection Items and Switch-Case Statements

`Remove()`, `Replace(...)`, `InsertAbove(...)`, `InsertBelow(...)` and `AddInvocation(...)` all work through the statement's `Parent`. Two kinds of statement only get a `Parent` from **`Intent.Common.CSharp` 3.10.12**: items added with `CSharpCollectionExpression.AddItem(...)`, and statements added with `CSharpSwitchCodeBlock.AddStatement(string)` (a `case` body). Before 3.10.12, `Remove` and the inserts throw `InvalidOperationException` ("Cannot remove statement from unknown parent"), and `AddInvocation` returns a new statement but leaves the item unchanged — silently. Requires 3.10.12 or later; check your feed: if `run_solution_query` → `searchAvailableModules("Intent.Common.CSharp", { includePrerelease: true })` does not list 3.10.12 or later, these edits do not work for you yet — keep a reference to the collection or case block and edit its `Statements` list directly instead. Otherwise, to use the edits, raise the `Intent.Common.CSharp` `<dependency>` (`.imodspec`) and the `Intent.Modules.Common.CSharp` package reference (`.csproj`) to 3.10.12, with `Intent.Modules.Common` / the `Intent.Common` `<dependency>` at 3.11.0 and the `Intent.SoftwareFactory.SDK` package reference at 3.14.0 (Intent Architect 5.0.0 or later), or restore fails with `NU1605` (see the `known-build-gotchas` instructions).

Two search limits apply on every version:

- `FindStatement` / `FindStatements` do **not** look inside an assignment's right-hand side. To reach the items of `options.X = [ ... ]`, find the assignment, then search its `Rhs`.
- They do not reach into `case` bodies either. Keep a reference to the `CSharpSwitchCodeBlock` you are given in `AddCase(...)`.

```csharp
// Intent.Common.CSharp 3.10.12 or later (with Intent.Modules.Common 3.11.0 and Intent.SoftwareFactory.SDK 3.14.0 or later)
CSharpSwitchCodeBlock adminCase = null;

@class.AddMethod("void", "Configure", method =>
{
    method.AddStatement(new CSharpAssignmentStatement(
        new CSharpStatement("options.Roles"),
        new CSharpCollectionExpression()
            .AddItem("\"Reader\"")
            .AddItem("\"Writer\"")
            .AddItem("builder.Build()")));

    method.AddSwitchStatement("role", @switch => @switch
        .AddCase("\"Admin\"", block =>
        {
            adminCase = block;  // case bodies are not searchable: keep the reference
            block.AddStatement("Grant();");
            block.WithBreak();
        }));
});

// Later, e.g. in an AfterBuild callback:
var configure = file.Classes.First().FindMethod("Configure");
var assignment = configure.FindStatement<CSharpAssignmentStatement>(a => a.Lhs.GetText("") == "options.Roles");
var roles = (CSharpCollectionExpression)assignment.Rhs;   // step two: search the Rhs

roles.FindStatement<CSharpStatement>(s => s.GetText("") == "\"Writer\"").Remove();
roles.FindStatement<CSharpStatement>(s => s.GetText("") == "\"Reader\"").InsertBelow(new CSharpStatement("\"Auditor\""));
roles.FindStatement<CSharpStatement>(s => s.GetText("") == "builder.Build()").AddInvocation("Single");  // no ";" inside the list
adminCase.Statements.First(s => s.GetText("") == "Grant();").InsertAbove(new CSharpStatement("Audit();"));

// Renders:
//   options.Roles = ["Reader", "Auditor", builder.Build().Single()];
//   ...
//   case "Admin":
//       Audit();
//       Grant();
//       break;
```

On an older floor, tag the item when you add it, keep a reference to the collection (or case block), and edit its `Statements` list directly:

```csharp
// Any Intent.Common.CSharp version
var roles = new CSharpCollectionExpression()
    .AddItem("\"Reader\"")
    .AddItem("\"Writer\"", item => item.AddMetadata("my-module:writer-role", true));
method.AddStatement(new CSharpAssignmentStatement(new CSharpStatement("options.Roles"), roles));

// Later:
var writer = roles.Statements.First(s => s.HasMetadata("my-module:writer-role"));
var index = roles.Statements.IndexOf(writer);
roles.Statements.RemoveAt(index);
roles.Statements.Insert(index, new CSharpStatement("\"Auditor\""));
```

## Finding Another Template's Statements

Match on a **metadata tag** the producing template set, never on rendered text. A catch block shows why: `ExceptionType` holds whatever string the producer passed — one producer passes `"Exception e"` as the type, so `c.ExceptionType == "Exception"` silently finds nothing.

```csharp
// Producer — tag the catch block when adding it:
method.AddTryBlock(@try => @try.AddStatement("await next();"));
method.AddCatchBlock("Exception", "ex", @catch =>
{
    @catch.AddMetadata("my-module:unhandled-catch", true);
    @catch.AddStatement("throw;");
});

// Consumer — a later callback (higher priority, or AfterBuild):
var unhandled = method.FindStatement<CSharpCatchBlock>(c => c.HasMetadata("my-module:unhandled-catch"));
unhandled?.Statements.Last().InsertAbove(new CSharpStatement("_logger.LogError(ex, \"Unhandled\");"));
```

Prefix keys with your module's name so two modules never collide — `AddMetadata` throws on a duplicate key.

===

## Initial Scaffold with `Body = Mode.Ignore`

Use this pattern for a method body a developer or AI implements: the template writes a **starting point**, usually a stub, and later SF runs leave the body alone. It is the default for a new template's "implement here" body (`module-building-strategies` § "Managed modes").

The key behaviour of `Body = Mode.Ignore` on a **generated output method**:

- **When the method first appears** (a new file, or a new method in an existing one): the template's generated body IS written — the developer receives it as their starting point.
- **Subsequent SF runs**: RoslynWeaver sees `Mode.Ignore` on that body and keeps whatever is on disk, silently discarding the template's new output for it.

Emit the attribute through the typed `CSharpIntentManagedAttribute`, never as an attribute name plus argument strings:

- `CSharpIntentManagedAttribute.IgnoreBody()` renders `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` — the template keeps owning the signature, the body is the developer's after the first run. Usually the one you want.
- `CSharpIntentManagedAttribute.Ignore()` renders `[IntentManaged(Mode.Ignore)]` — the whole member, signature included, is left alone after the first run. Not for an "implement here" body: the signature stops following the model, so a renamed operation leaves the old member behind.

```csharp
// In the template constructor:
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass($"{Model.Name}Handler")
    .OnBuild(file =>
    {
        var @class = file.Classes.First();
        @class.AddMethod("Task", "Handle", method =>
        {
            method.Async();
            method.AddParameter(GetTypeName("My.Module.Request", Model), "request"); // a lookup, so inside a callback (Must #7)
            method.AddOptionalCancellationTokenParameter();

            // Signature stays generated; the body below is written only when the method first appears.
            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
            method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
            method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
        });
    });
```

Keep code the template can generate from the model **out** of an ignored body: after the first run the template's body is never applied again, so a model change (a new property, a renamed operation) would not reach it. Put that code in its own fully managed member, such as a private method, and call it from the ignored body; the body's call to it is then the only part that can go stale, and a changed signature breaks the build rather than hiding. Compare with `Mode.Merge`, where Intent keeps changing and removing the lines it wrote on every run, and `Mode.Fully`, where the template owns the body completely.

## Conditional AddUsing Patterns

```csharp
// Branch-based:
if (useTopLevelStatements)
{
    CSharpFile.AddUsing(this.GetNamespace());
}

// Dependency-driven (GetTemplate throws when nothing matches — DoNotThrow makes it optional):
foreach (var templateDependency in @event.TemplateDependencies)
{
    var template = GetTemplate<IClassProvider>(templateDependency, TemplateDiscoveryOptions.DoNotThrow);
    if (template != null)
    {
    AddUsing(template.Namespace);
    }
}

// Namespace collection:
foreach (var ns in @event.RequiredNamespaces)
{
    AddUsing(ns);
}

// Only introduce the namespace when this exact type is needed:
method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
```

## Registration Quick-Ref

| Template type | Registration base |
|---|---|
| Single output file | `SingleFileTemplateRegistration` |
| One file per model | `FilePerModelTemplateRegistration<TModel>` — override `GetModels` |
| One file for all models | `SingleFileListModelTemplateRegistration<TModel>` — override `GetModels` |
| Event/pipeline driven | `ITemplateRegistration` |
