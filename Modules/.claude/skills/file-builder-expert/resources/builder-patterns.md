---
contentHash: 6DEED1BDAE5C95C7C188E0500DD7458050F64ABD9560BE9556598EE719CDBB21
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

```csharp
var configTemplate = application.FindTemplateInstance<IClassProvider>(NServiceBusConfigurationTemplate.TemplateId);
if (configTemplate != null)
{
    file.AddUsing(configTemplate.Namespace);
}
```

## Resolving Single-File Template Namespaces

`AddTypeSource(templateId)` only works for **file-per-model** templates — it resolves a type by finding the template instance that was generated for a specific model. For **single-file** templates (one output, no model per instance), there is no model to look up. To inject the using for a single-file template's type, find the template instance directly inside an `OnBuild` callback:

```csharp
// In the template constructor — BEFORE CSharpFile:
// AddTypeSource("Intent.Eventing.Contracts.IntegrationEventMessage");  // ← correct, file-per-model
// AddTypeSource("Intent.Eventing.Contracts.IntegrationEventHandlerInterface");  // ← won't inject using

CSharpFile = new CSharpFile(...)
    .OnBuild(file =>
    {
        // Single-file template: find the instance, take its namespace
        var handlerInterface = ExecutionContext.FindTemplateInstance<IClassProvider>(
        "Intent.Eventing.Contracts.IntegrationEventHandlerInterface");
        if (handlerInterface != null)
        file.AddUsing(handlerInterface.Namespace);

        // Now safe to use "IIntegrationEventHandler" as a raw type name in statements
    }, 0);
```

`IClassProvider` is from `Intent.Modules.Common.Templates` — already available in most template files.

- --

## Initial Scaffold with `[IntentManaged(Mode.Ignore)]`

Use this pattern when a generated method body needs to be a **useful starting point on first install**, but must survive developer customisation on subsequent SF runs without being overwritten.

The key behaviour of `Mode.Ignore` on a **generated output method**:

- **First SF run** (no file on disk yet): the template's generated body IS written — the developer receives it as their starting point.
- **Subsequent SF runs**: RoslynWeaver sees `Mode.Ignore` on that method and keeps whatever is on disk, silently discarding the template's new output for that method.

```csharp
// In the template constructor:
@class.AddMethod("void", "Configure", method =>
{
    method.Override();
    method.AddParameter("MessageProcessingBuilder", "builder");

    // Mark Mode.Ignore so developer customisations survive regeneration.
    // The body below is only written on the FIRST SF run for this file.
    method.AddAttribute("IntentManaged", attr => attr.AddArgument("Mode.Ignore"));
});

// Populate the initial body in OnBuild — it becomes the developer's starting point:
.OnBuild(file =>
{
    var configure = file.Classes.First().FindMethod("Configure");
    foreach (var handler in Model)
    {
    foreach (var sub in handler.IntegrationEventSubscriptions())
    {
    var eventType = GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage",
    sub.TypeReference.Element.AsMessageModel()!);
    configure.AddStatement(
    $"builder.Event<{eventType}>(\"{handler.Name}\")" +
    $".HandledBy<OrchestratorEventConsumer<IIntegrationEventHandler<{eventType}>, {eventType}>>();");
    }
    }
}, 0);
```

- *When to use:** The method body has a meaningful generated starting point AND the developer will need to add bespoke logic (e.g. upcast chains, custom publish config) that must survive future SF runs. Compare with `Mode.Merge` (template continuously regenerates on every run) and `Mode.Fully` (template owns the body completely).
- *Caution:** Once the developer has customised the body, the template's model-driven generation no longer fires for that file. If the model changes (e.g. a new event subscription is added in the designer), the developer must update Configure manually.

## Conditional AddUsing Patterns

```csharp
// Branch-based:
if (useTopLevelStatements)
{
    CSharpFile.AddUsing(this.GetNamespace());
}

// Dependency-driven:
foreach (var templateDependency in @event.TemplateDependencies)
{
    var template = GetTemplate<IClassProvider>(templateDependency);
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
