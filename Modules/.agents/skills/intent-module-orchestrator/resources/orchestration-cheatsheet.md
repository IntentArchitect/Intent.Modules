---
contentHash: F119D49416CA67C9B68D59F6CF152CEE10568105D3016CFD9ABB9B27BD20421B
---
# Orchestration Cheatsheet

Quick-reference snippets for the `intent-module-orchestrator` skill.  
Source of truth: <https://github.com/IntentArchitect/Intent.Modules>

> **Strategy — broadcast over direct coupling.** Recurring cross-module integration is done by *broadcasting* a request that a responsible module *handles* — never by wiring modules together directly. DI registration (`ContainerRegistrationRequest`) and app settings (`AppSettingRegistrationRequest`) below are the documented examples. **Infrastructure-resource registration** (resources other modules consume — e.g. for health checks) follows the same broadcast pattern; confirm the exact request type via `search_docs` before use rather than assuming. See `module-building-strategies` §1.

- --

## §DI Registration — ContainerRegistrationRequest

Publish from `OnBeforeTemplateExecution(...)`. Framework collects all requests and merges them into the startup DI file.

```csharp
// Minimal — adds: services.AddTransient<MyService>();
ExecutionContext.EventDispatcher.Publish(
    ContainerRegistrationRequest
        .ToRegister(this));

// Concrete + interface + concern + lifetime
ExecutionContext.EventDispatcher.Publish(
    ContainerRegistrationRequest
        .ToRegister(this)                            // IClassProvider: uses FullTypeName()
        .ForInterface(interfaceTemplate)             // resolves via IClassProvider or string
        .ForConcern("Application")                  // targets Application startup file
        .WithPerServiceCallLifeTime()               // Transient | PerServiceCall | Singleton
        .HasDependency(this));                      // declares template ordering dependency

// Open-generic behaviour (e.g., MediatR pipeline behaviours)
ExecutionContext.EventDispatcher.Publish(
    ContainerRegistrationRequest
        .ToRegister($"typeof({ClassName}<,>)")
        .ForConcern("Application")
        .WithPerServiceCallLifeTime()
        .WithPriority(100));                        // ordering among DI registrations

// Interface-only registration resolved from container
ExecutionContext.EventDispatcher.Publish(
    ContainerRegistrationRequest
        .ToRegister(concreteTypeName)
        .ForInterface(interfaceTypeName)
        .WithResolveFromContainer()
        .RequiresUsingNamespaces("My.Namespace"));
```

- *ContainerRegistrationRequest.LifeTime constants**

| Constant | Meaning |
|---|---|
| `LifeTime.Transient` | Created on every resolution |
| `LifeTime.PerServiceCall` | Scoped to one request/unit-of-work |
| `LifeTime.Singleton` | Single instance for application lifetime |

- --

## §AppSettings — AppSettingRegistrationRequest

Publish from `OnBeforeTemplateExecution(...)`. Framework idempotently merges keys; existing values are left untouched.

```csharp
// Flat key — string value
ExecutionContext.EventDispatcher.Publish(
    new AppSettingRegistrationRequest(
        "ConnectionStrings:DefaultConnection",
        "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;"));

// Structured section — anonymous object serialised to JSON
ExecutionContext.EventDispatcher.Publish(
    new AppSettingRegistrationRequest(
        "JwtToken",
        new
        {
            Issuer    = "https://localhost:{sts_port}",
            Audience  = "api",
            SigningKey = "aHHDYCTvyZVbdcGgaDvL+T6837pHCkciU0rLvUbE9a4="
        }));

// Environment-specific override (appsettings.Development.json)
ExecutionContext.EventDispatcher.Publish(
    new AppSettingRegistrationRequest(
        key:                "Logging:LogLevel:Default",
        value:              "Debug",
        runtimeEnvironment: "Development"));

// Project-role targeting (multiple projects in solution)
ExecutionContext.EventDispatcher.Publish(
    new AppSettingRegistrationRequest(
        key:                "FUNCTIONS_WORKER_RUNTIME",
        value:              "dotnet-isolated",
        runtimeEnvironment: null,
        forProjectWithRole: "AzureFunctions"));
```

- --

## §Resolution — FindTemplateInstance & Safe Guards

### Step 1 — Role-based single lookup

```csharp
// Returns null when the template is not registered — ALWAYS guard before use
var diTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
    TemplateRoles.Application.DependencyInjection);

if (diTemplate == null) return;          // ← MUST: guard before accessing .CSharpFile

diTemplate.CSharpFile.AfterBuild(file =>
{
    var method = file.Classes.First().FindMethod("AddApplication");
    method?.AddInvocationStatement("services.AddAutoMapper",
        stmt => stmt.AddArgument("Assembly.GetExecutingAssembly()"));
}, 500);                                 // ← MUST: explicit priority (Extension band)
```

### Step 2 — Role-based multi-template loop with model guard

```csharp
// Source: Intent.Modules.AspNetCore.Controllers.JsonPatch (active)
var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
    TemplateRoles.Distribution.WebApi.Controller);

foreach (var template in templates)
{
    // MUST: narrow to templates whose model matches the expected type
    if (!template.TryGetModel<IControllerModel>(out var controllerModel))
        continue;

    template.CSharpFile.OnBuild(file =>
    {
        var cls = file.Classes.First();
        // ... enrich based on controllerModel
    }, 100);                             // Enrichment band
}
```

### Step 3 — Role + model-id lookup (TemplateDependency)

```csharp
// Preferred when the model element is known at resolution time
if (!template.TryGetTemplate(
        TemplateRoles.Domain.Entity.Primary,
        dtoModel.Mapping.ElementId,
        out ICSharpFileBuilderTemplate entityTemplate)
    && !template.TryGetTemplate(
        TemplateRoles.Domain.ValueObject,
        dtoModel.Mapping.ElementId,
        out entityTemplate)
    && !template.TryGetTemplate(
        TemplateRoles.Domain.DataContract,
        dtoModel.Mapping.ElementId,
        out entityTemplate))
{
    throw new InvalidOperationException(
        $"Could not resolve mapped type for '{dtoModel.Name}'.");
}

var typeName = template.GetTypeName(entityTemplate);
```

### Step 4 — TemplateId fallback chain (DoNotThrow)

```csharp
// Use TemplateDiscoveryOptions.DoNotThrow when probing multiple candidates silently
private static readonly TemplateDiscoveryOptions DoNotThrow =
    new() { TrackDependency = false, ThrowIfNotFound = false };

bool resolved =
    template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out var uow)
    || template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DynamoDBUnitOfWorkInterface, out uow)
    || template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out uow);

if (!resolved) return;
```

### ⚠ Cross-Module Boundary Rule — Use Interface Types, Not Concrete Types

When calling `FindTemplateInstance<T>` or `FindTemplateInstances<T>` **from a different module** (i.e. a factory extension in module B looking up a template registered by module A), always use an interface type for `T` — never a concrete template class.

IA may load each module's assembly in an isolated `AssemblyLoadContext`. When it does, the concrete `MyTemplate` type in module B's context is a different `Type` object from the one registered by module A in its context — the lookup silently returns `null` with no error.

- *Template ID is the primary lookup key** — always use the correct `TemplateId` constant. The interface type is the secondary filter that crosses ALC boundaries safely.

```csharp
// ❌ Wrong — returns null when called from a different module's factory extension
var t = application.FindTemplateInstance<WolverineConfigurationTemplate>(
    WolverineConfigurationTemplate.TemplateId);

// ✅ Correct — interface types from shared packages cross ALC boundaries safely
var t = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
    WolverineConfigurationTemplate.TemplateId);
```

| Scenario | Interface to use |
|---|---|
| Template exposes a `CSharpFile` property | `ICSharpFileBuilderTemplate` (from `Intent.Modules.Common.CSharp`) |
| Template has a typed model | `IIntentTemplate<TModel>` (from `Intent.Templates`) |
| Role lookup only, no CSharpFile needed | `ICSharpTemplate` (from `Intent.Modules.Common.CSharp`) |

### Null-conditional shorthand (optional single-template enrichment)

```csharp
// Safe when the template is genuinely optional
application
    .FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Distribution.WebApi.Startup)
    ?.CSharpFile.AfterBuild(file => { /* enrich if present */ }, 500);
```

- --

## §Factory Extension — Full FactoryExtensionBase Skeleton

> **Required using:** `FindTemplateInstance<T>` and `FindTemplateInstances<T>` are generic extension methods defined in `Intent.Modules.Common`. Add `using Intent.Modules.Common;` to the factory extension file — its absence produces `CS0308: The non-generic method cannot be used with type arguments`, which is a misleading error.

```csharp
[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class MyModuleFactoryExtension : FactoryExtensionBase
{
    public override string Id => "My.Module.MyModuleFactoryExtension";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    // ── Publish registration events here (not in OnAfterTemplateRegistrations) ──
    protected override void OnBeforeTemplateExecution(IApplication application)
    {
        application.EventDispatcher.Publish(
            new AppSettingRegistrationRequest("MySection", new { Enabled = true }));
    }

    // ── Find templates, schedule build callbacks ──
    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnTemplate(TemplateRoles.Application.DependencyInjection));

        foreach (var template in templates)
        {
            template.CSharpFile.AfterBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddApplication");
                if (method == null) return;

                method.AddInvocationStatement("services.AddMyModule");
            }, 500);                     // Extension band — safely after owner's OnBuild
        }
    }
}
```

- --

## §Priority Bands Reference

```csharp
csharpFile
    .OnBuild(file =>
    {
        // Band 0 — Core: owning template builds primary class structure.
        // Never search for elements from other templates here.
        file.Classes.First().AddMethod("void", "Execute");
    }, 0)
    .OnBuild(file =>
    {
        // Band 100 — Enrichment: same-module additions (e.g., add an attribute).
        file.Classes.First().FindMethod("Execute")
            ?.AddAttribute("LogExecutionTime");
    }, 100)
    .AfterBuild(file =>
    {
        // Band 500 — Extension: factory extensions from other modules.
        // All Band 0/100 OnBuild callbacks have finished.
        file.Classes.First().AddAttribute("GeneratedByExtension");
    }, 500)
    .AfterBuild(file =>
    {
        // Band 1000 — Final: cross-template wiring.
        // Safe to FindMethod/FindClass on elements created by other templates.
        var cls = file.Classes.First();
        if (cls.HasMetadata("requires-disposal"))
        {
            cls.ImplementsInterface("IDisposable");
            cls.AddMethod("void", "Dispose", m => m.AddStatement("// cleanup"));
        }
    }, 1000);
```

- *The Find Rule:** Template B must use a **strictly higher priority** than Template A when B calls `FindMethod`/`FindClass` on elements A created. If B's priority ≤ A's, A may not have run yet.
- --

## §Resolution & Consumption — Stereotype-Driven AfterBuild

Bridges the two skills: resolve the template (orchestrator), then consume the stereotype (metadata-explorer).

```csharp
// Factory extension: enriches entity templates based on stereotype metadata
protected override void OnAfterTemplateRegistrations(IApplication application)
{
    var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
        TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary));

    foreach (var template in templates)
    {
        // MUST: verify model type before consuming stereotype
        if (!template.TryGetModel<ClassModel>(out var classModel))
            continue;

        // MUST: use generated typed accessor — NOT GetStereotype("OData")
        if (!classModel.HasStereotype("OData"))
            continue;

        template.CSharpFile.AfterBuild(file =>
        {
            var cls = file.Classes.First();

            // Bool property → conditional attribute
            if (classModel.GetSomeSettings()?.IsTimestamped() == true)
                cls.AddAttribute(UseType("MyModule.TimestampedAttribute"));

            // String property → attribute argument
            var tableName = classModel.GetSomeSettings()?.TableName();
            if (!string.IsNullOrEmpty(tableName))
                cls.AddAttribute("Table", a => a.AddArgument($"\"{tableName}\""));

            // Int? property → constraint argument with null-guard
            var maxLen = classModel.GetTextConstraints()?.MaxLength();
            if (maxLen is { } max && max > 0)
            {
                cls.FindProperty("Name")
                    ?.AddAttribute("MaxLength", a => a.AddArgument($"{max}"));
            }

            // Enum property → deterministic switch
            switch (classModel.GetFileSettings()?.TemplatingMethod().AsEnum())
            {
                case TemplatingMethodOptionsEnum.CSharpFileBuilder:
                    cls.AddAttribute("CSharpFileBuilderManaged");
                    break;
            }
        }, 500);                         // Extension band
    }
}
```

- --

## §Startup & Service Configuration — IAppStartupFile DSL

Prefer the high-level `IAppStartupFile` DSL over raw `FindMethod` calls when modifying Startup or Program files. The DSL abstracts the difference between the minimal hosting model (`Program.cs` top-level statements) and the generic hosting model (`Startup.cs`), so the same code works for both.

### Resolve the startup template

```csharp
// Always resolve via IAppStartupTemplate.RoleName — never hardcode a TemplateId
var startup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
if (startup == null) return;   // ← MUST guard before accessing .StartupFile

// Register mutations inside an OnBuild/AfterBuild callback
startup.CSharpFile.OnBuild(_ =>
{
    var sf = startup.StartupFile;
    // ... DSL calls below ...
});
```

### Context variable reference

| Context property | Type in minimal hosting | Type in generic hosting |
|---|---|---|
| `context.Services` | `"builder.Services"` | `"services"` |
| `context.Configuration` | `"builder.Configuration"` | `"configuration"` |
| `context.App` | `"app"` | `"app"` |
| `context.Env` | `"app.Environment"` | `"env"` |
| `context.Endpoints` | `"app"` (in MapX calls) | `"endpoints"` |
| `context.Parameters[0]` | first lambda param name (e.g. `"opt"`) | same |

### 1. Single-statement service registration

```csharp
// Generates: services.ConfigureGrpc();  (or builder.Services.ConfigureGrpc();)
startup.StartupFile.AddServiceConfiguration(
    ctx => $"{ctx.Services}.ConfigureGrpc();");

// With IConfiguration argument
startup.StartupFile.AddServiceConfiguration(
    ctx => $"{ctx.Services}.ConfigureSwagger({ctx.Configuration});");
```

### 2. Lambda-bearing service call — AddServiceConfigurationLambda

Use when you need to inject statements **inside** an `options =>` lambda of an existing method (e.g., `AddControllers`, `AddAuthentication`). The DSL **merges** multiple callers into the same lambda block for the same `methodName` (no duplicate invocations).

If subsequent calls pass more lambda parameters than the first call, the additional parameter names are appended to the existing lambda signature in order.

```csharp
// Source: ExceptionFilterExtension.cs (active module)
// Produces: services.AddControllers(opt => { opt.Filters.Add<MyFilter>(); })
startup.StartupFile.AddServiceConfigurationLambda(
    methodName: "AddControllers",
    parameters: ["opt"],
    configure: (statement, lambda, context) =>
    {
        // statement  — the CSharpInvocationStatement for services.AddControllers(...)
        // lambda      — the CSharpLambdaBlock inside the invocation
        // context     — carries .Services, .Configuration, and .Parameters
        // context.Parameters[0] — the lambda variable name: "opt"
        lambda.AddStatement(
            $"{context.Parameters[0]}.Filters.Add<{template.GetExceptionFilterName()}>();");

        // Attach metadata so other modules can locate this statement later
        statement.AddMetadata("configure-services-controllers", "generic");
    },
    priority: -10_000_000);  // run early so other modules can augment the same lambda
```

### 3. Container registration lambda — AddContainerRegistrationLambda

Same pattern as above but placed in the container-registration slot (runs before generic service config in the generated output).

```csharp
startup.StartupFile.AddContainerRegistrationLambda(
    methodName: "AddMassTransit",
    parameters: ["x"],
    configure: (statement, lambda, context) =>
    {
        lambda.AddStatement($"{context.Parameters[0]}.AddConsumersFromNamespaceContaining<Anchor>();");
    });
```

### 4. Single-statement middleware registration

```csharp
// Generates: app.UseAuthentication();  (works for both hosting models)
startup.StartupFile.AddAppConfiguration(
    ctx => $"{ctx.App}.UseAuthentication();");

// Two calls; order is determined by natural insertion order
startup.StartupFile.AddAppConfiguration(ctx => $"{ctx.App}.UseAuthentication();");
startup.StartupFile.AddAppConfiguration(ctx => $"{ctx.App}.UseAuthorization();");
```

### 5. Lambda-bearing middleware call — AddAppConfigurationLambda

`AddAppConfigurationLambda("UseEndpoints", ...)` is blocked by the SDK and throws. Use `AddUseEndpointsStatement(...)` for endpoint mapping.

```csharp
// Produces: app.UseRateLimiter(options => { options.GlobalLimiter = ...; })
startup.StartupFile.AddAppConfigurationLambda(
    methodName: "UseRateLimiter",
    parameters: ["options"],
    configure: (statement, lambda, context) =>
    {
        lambda.AddStatement(
            $"{context.Parameters[0]}.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(...);" );
    });
```

### 6. Endpoint mapping

```csharp
// Simple endpoint mapping
startup.StartupFile.AddUseEndpointsStatement(
    ctx => $"{ctx.Endpoints}.MapControllers();");

// With position relative to an existing statement
startup.StartupFile.ConfigureEndpoints((statements, ctx) =>
{
    if (statements.Statements.All(x => !x.ToString()!.Contains(".MapRazorPages")))
    {
        statements
            .Single(x => x.ToString()!.Contains(".MapControllers("))
            .InsertBelow(new CSharpInvocationStatement($"{ctx.Endpoints}.MapRazorPages"));
    }
});
```

### 7. Low-level ConfigureServices / ConfigureApp for positional edits

Use when you need to read the existing statement list and perform conditional insertion.

```csharp
// Source: AspNetCoreIdentityUiFactoryExtension.cs (active module)
startup.StartupFile.ConfigureServices((block, ctx) =>
{
    // Context gives you the correct variable name regardless of hosting model
    if (block.Statements.All(x => !x.ToString()!.Contains(".AddRazorPages()")))
    {
        block.Statements
            .Single(x => x.ToString()!.Contains(".AddInfrastructure("))
            .InsertBelow(new CSharpInvocationStatement($"{ctx.Services}.AddRazorPages"));
    }
});

startup.StartupFile.ConfigureApp((block, ctx) =>
{
    if (block.Statements.All(x => !x.ToString()!.Contains(".UseStaticFiles")))
    {
        block.Statements
            .Single(x => x.ToString()!.Contains(".UseRouting("))
            .InsertAbove(new CSharpInvocationStatement($"{ctx.App}.UseStaticFiles"));
    }
});
```

### IAppStartupFile method quick-ref

| Method | Hosting-model agnostic | Merges duplicates | Use for |
|---|---|---|---|
| `AddServiceConfiguration(ctx => ...)` | ✓ | — | Single-line service call |
| `AddServiceConfigurationLambda(name, params, cfg)` | ✓ | ✓ | Lambda options injection into existing method |
| `AddContainerRegistration(ctx => ...)` | ✓ | — | Single-line DI registration |
| `AddContainerRegistrationLambda(name, params, cfg)` | ✓ | ✓ | Lambda options injection (DI slot) |
| `AddAppConfiguration(ctx => ...)` | ✓ | — | Single-line middleware call |
| `AddAppConfigurationLambda(name, params, cfg)` | ✓ | ✓ | Lambda injection into middleware call |
| `AddUseEndpointsStatement(ctx => ...)` | ✓ | — | Endpoint mapping |
| `ConfigureServices((stmts, ctx) => ...)` | ✓ | — | Positional / conditional service edits |
| `ConfigureApp((stmts, ctx) => ...)` | ✓ | — | Positional / conditional middleware edits |
| `ConfigureEndpoints((stmts, ctx) => ...)` | ✓ | — | Positional / conditional endpoint edits |
| `ExposeProgramClass()` | ✓ | — | Make `partial class Program` public (top-level stmts) |

> ⚠️ `AddAppConfigurationLambda("UseEndpoints", ...)` throws — use `AddUseEndpointsStatement` instead.

- --

## TemplateDependency Quick Ref

```csharp
TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary)
// ──► matches all templates whose role == Primary or TemplateId == Primary

TemplateDependency.OnTemplate(MyTemplate.TemplateId)
// ──► matches by exact TemplateId constant

TemplateDependency.OnModel(MyTemplate.TemplateId, modelElement)
// ──► matches by TemplateId AND model — useful for model-scoped lookups

TemplateDependency.OfType<ICSharpFileBuilderTemplate>()
// ──► matches ALL registered CSharpFile builder templates
```
