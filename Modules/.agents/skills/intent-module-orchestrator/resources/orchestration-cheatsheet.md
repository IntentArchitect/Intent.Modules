---
contentHash: CA78B49531E49CFBEAE2CF73F2344C186BB4AF36056E42C3CDE7F33D3B72C9D0
---
# Orchestration Cheatsheet

Quick-reference snippets for the `intent-module-orchestrator` skill.  
Source of truth: <https://github.com/IntentArchitect/Intent.Modules>

> **Strategy — broadcast over direct coupling.** Recurring cross-module integration is done by *broadcasting* a request that a responsible module *handles* — never by wiring modules together directly. DI registration (`ContainerRegistrationRequest`) and app settings (`AppSettingRegistrationRequest`) below are the documented examples. **Infrastructure-resource registration** (resources other modules consume — e.g. for health checks) follows the same broadcast pattern; confirm the exact request type via `search_docs` before use rather than assuming. See `module-building-strategies` §1.

===

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
        .ToRegister(this)                           // IClassProvider: uses FullTypeName()
        .ForInterface(interfaceTemplate)            // resolves via IClassProvider or string
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

You declare the *lifetime*; the host template owns the mapping onto whatever registration method its
container uses. That mapping is the host's business, not yours — which is exactly why you publish a
request instead of writing the call yourself.

| Constant | Meaning | Typical host mapping |
|===|===|===|
| `LifeTime.Transient` | Created on every resolution | `AddTransient` |
| `LifeTime.PerServiceCall` | Scoped to one request/unit-of-work | `AddScoped` |
| `LifeTime.Singleton` | Single instance for application lifetime | `AddSingleton` |

===

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

===

## §Host Wiring — ServiceConfigurationRequest & Siblings

`ContainerRegistrationRequest` registers a *type*. When you instead need the host to **call an
extension method** during startup, publish one of these. Same rules as its siblings: publish from
`OnBeforeTemplateExecution(...)`, and the host template merges, orders and de-duplicates.

```csharp
// Produces: services.AddMyFeature();   (or builder.Services.AddMyFeature(); — the host decides)
ExecutionContext.EventDispatcher.Publish(
    ServiceConfigurationRequest
        .ToRegister("AddMyFeature")
        .ForConcern("Infrastructure")            // which startup file to target
        .HasDependency(this)                     // import the namespace holding the method
        .WithPriority(100));

// Pass the configuration object in — produces: services.AddMyFeature(configuration);
ExecutionContext.EventDispatcher.Publish(
    ServiceConfigurationRequest
        .ToRegister("AddMyFeature", ServiceConfigurationRequest.ParameterType.Configuration)
        .RequiresUsingNamespaces("MyEcosystem.Feature"));

// Middleware — produces: app.UseMyFeature();
ExecutionContext.EventDispatcher.Publish(
    ApplicationBuilderRegistrationRequest
        .ToRegister("UseMyFeature")
        .HasDependency(this));

// Named connection string — left untouched if an entry with that name already exists
ExecutionContext.EventDispatcher.Publish(
    new ConnectionStringRegistrationRequest(
        name:             "DefaultConnection",
        connectionString: "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;",
        providerName:     "System.Data.SqlClient"));
```

`ToRegister` takes the **extension method name only** — never a whole statement. Parameters are a
`params` list of `ParameterType` constants, and the host substitutes the correct variable name for
whichever startup shape it is generating.

### Which request for which job

| You want | Publish |
|---|---|
| A type resolved from the container | `ContainerRegistrationRequest` |
| A `Services.AddX()` call at startup | `ServiceConfigurationRequest` |
| A middleware `app.UseX()` call | `ApplicationBuilderRegistrationRequest` |
| A configuration key or section | `AppSettingRegistrationRequest` |
| A named connection string | `ConnectionStringRegistrationRequest` |

### ⚠ Shape vs Wire — the most common wrong turn

- *Shaping a generated type → FileBuilder mutation. Wiring into host infrastructure → publish a request.**

Reaching for `CSharpFile` on the startup template to append a registration line looks more direct, but
it moves the whole burden onto you: knowing which host shape you landed in, injecting the right
`using`, placing the statement in the correct block, and not appending it a second time on the next
run. Publishing a request hands all of that to the host template that owns the file. Several different
host templates listen for these requests and each maps them onto its own startup shape — which is
precisely the knowledge you avoid having to encode.

===

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

### Step 5 — Optional presence: TryGetTypeName

Use this when you want to integrate with another module **only if it happens to be installed**, with
no hard dependency and no failure when it is absent. `TryGetTypeName` resolves the type name and
registers the `using` when the target is present, and simply returns `false` when it is not — so the
whole enrichment degrades gracefully to a no-op.

```csharp
// Enrich only if the validation module is installed; otherwise skip silently.
if (template.TryGetTypeName("MyEcosystem.Application.ValidatorProviderInterface", out var validatorProvider))
{
    var ctor = cls.Constructors.First();

    // Idempotency guard — this callback re-runs on every Software Factory execution.
    if (ctor.Parameters.All(p => p.Type != validatorProvider))
    {
        ctor.AddParameter(validatorProvider, "validatorProvider",
            p => p.IntroduceReadonlyField((_, stmt) => stmt.ThrowArgumentNullException()));
    }
}
```

| Method | When the target is missing | Use for |
|---|---|---|
| `GetTypeName(templateId, model)` | Throws / assumes presence | A target your module genuinely requires |
| `TryGetTypeName(templateId, out var name)` | Returns `false` | An optional, best-effort integration |

Overloads mirror the `GetTypeName` family — by template id alone, or by template id plus a model /
model id, and by a *list* of candidate template ids for a fallback chain.

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

===

## §The Model Bridge — reading the designer model off a generated node

Once you have a template, you can reach its own model with `TryGetModel<T>`. But enrichment usually
needs to go one level finer: *which designer operation is **this** generated method?* Do not answer
that by matching names.

Host templates that generate a member per modelled element **stamp the originating element onto the
generated node** as metadata under the well-known key `"model"`. Reading it back is the bridge from
generated C# to the designer model.

```csharp
// Role-string lookup — no module dependency needed for this part (see Two-Tier rule below).
var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
    "MyEcosystem.Application.CommandHandler");

foreach (var template in templates)
{
    // Narrow to templates whose own model is the shape you expect.
    if (!template.TryGetModel<IHandlerModel>(out var handlerModel))
    continue;

    template.CSharpFile.OnBuild(file =>
    {
        var cls = file.Classes.First();

        foreach (var method in cls.Methods)
        {
            // ── THE BRIDGE ──
            // The host template stamped the designer operation onto this method.
            if (!method.TryGetMetadata<IOperationModel>("model", out var operation))
                continue;                                  // not model-derived — leave it alone

            if (!operation.HasStereotype("Auditable"))
                continue;

            // Parameters carry their own designer model the same way.
            var commandParam = method.Parameters.FirstOrDefault(p =>
                p.TryGetMetadata<IParameterModel>("model", out var pm) &&
                pm.TypeReference.Element?.SpecializationType == "Command");

            // MUST be idempotent — OnBuild re-runs on every Software Factory execution,
            // so an unguarded AddAttribute appends a duplicate every time.
            if (method.Attributes.All(a => !a.Name.Contains("Audit")))
            {
                method.AddAttribute("Audit", attr => attr.AddArgument($"\"{operation.Name}\""));
            }
        }
    }, 500);   // second arg = build priority band (Extension)
}
```

- *Rules for the bridge**
1. **Always `TryGetMetadata`, never `GetMetadata`.** `"model"` is a convention host templates opt into, not a framework guarantee — a node with no model, or one built by a different template version, simply won't have it. `GetMetadata` on an absent key throws.
2. **Type the read.** `TryGetMetadata<T>` returns `false` on a type mismatch as well as on an absent key, so a wrong `T` degrades to "skip" rather than to a cast exception.
3. **Every node level carries its own.** Classes, methods, properties and parameters are stamped independently — read the one on the node you are actually enriching.
4. **Guard every mutation for idempotency.** These callbacks re-run on every execution.

> **Setting vs reading.** Metadata has two distinct uses, and the second is easy to miss:
> your own cross-step state (`node.AddMetadata("my-key", value)` in one callback, read back in a
> later one), **and** reading the designer model the host template already attached. `"model"` is
> the second kind. See `file-builder-expert` for the metadata API itself.

===

## §Two-Tier Module Dependency

How much you can do to another module's output depends on whether you declare a dependency on it in
your `.imodspec`. There are exactly two tiers, and the choice is worth making deliberately.

| Tier | Needs an `.imodspec` dependency on the target module | What you can do |
|---|---|---|
| **1 — cold** | No | Role-string lookup + the generic `ICSharpFileBuilderTemplate`. Add attributes, usings, properties, statements; publish registration requests. |
| **2 — typed** | **Yes** | Everything above, plus reading the target's typed model interfaces via `TryGetModel<T>` / `TryGetMetadata<T>("model")`. |

- *Why the split.** A role string is just a string — resolving one costs nothing and couples you to

nothing. But the *model interfaces* a host module exposes are types defined **inside that module's
assembly**. Referencing them means compiling against it, which means declaring it as a dependency:

```xml
<!-- In your .imodspec — required only for Tier 2 -->
<dependencies>
<dependency id="MyEcosystem.Application.Handlers" version="1.2.0" />
</dependencies>
```

- *The tradeoff.** Tier 1 is free but blind — you can shape generated code without knowing what any of

it means. Tier 2 is what most genuinely useful integration needs, and it costs a version-coupled
dependency: when the target module's model interfaces change, your module has to move with them.

Start at Tier 1. Move to Tier 2 when you actually need to read the model, not before — and see the
Cross-Module Boundary Rule above, which still applies at both tiers.

===

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

===

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

===

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

===

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

===

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
