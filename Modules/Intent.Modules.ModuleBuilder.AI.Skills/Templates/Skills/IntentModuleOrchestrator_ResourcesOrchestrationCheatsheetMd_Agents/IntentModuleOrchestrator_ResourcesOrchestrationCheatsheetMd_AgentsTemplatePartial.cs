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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentModuleOrchestrator_ResourcesOrchestrationCheatsheetMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentModuleOrchestrator_ResourcesOrchestrationCheatsheetMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentModuleOrchestrator_ResourcesOrchestrationCheatsheetMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentModuleOrchestrator_ResourcesOrchestrationCheatsheetMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("orchestration-cheatsheet", relativeLocation: "intent-module-orchestrator/resources")
                .FromMarkdown(""""""
                    # Orchestration Cheatsheet

                    Quick-reference snippets for the `intent-module-orchestrator` skill.
                    Source of truth: the XML documentation shipped inside the `Intent.Modules.Common` and `Intent.Modules.Common.CSharp` NuGet packages your module already references.

                    > **Strategy — broadcast over direct coupling.** Recurring cross-module integration is done by *broadcasting* a request that a responsible module *handles* — never by wiring modules together directly. DI registration (`ContainerRegistrationRequest`) and app settings (`AppSettingRegistrationRequest`) below are the documented examples. **Infrastructure-resource registration** (resources other modules consume, for example for health checks) follows the same broadcast pattern through `InfrastructureRegisteredEvent` — see §Launch Settings, host.json & Infrastructure. See `module-building-strategies` §1.

                    Role strings in these snippets come from the *Common Role Values* table in `SKILL.md`. Names starting `MyEcosystem.` or `My` are placeholders for your own template ids, types and stereotypes.

                    ===

                    ## §When to Publish — the handler must already exist

                    Request handlers subscribe in their own template constructors (a few in a factory extension's `OnBeforeTemplateRegistrations`). The event dispatcher delivers synchronously and keeps nothing for late subscribers, so a request published before its handler exists is **dropped with no error**. Publish only once every template has been registered.

                    | Where you publish from | Safe? |
                    |---|---|
                    | A template constructor | **No** — the handling template may not have been constructed yet |
                    | Factory extension `OnBeforeTemplateRegistrations` | **No** — no template exists yet |
                    | Factory extension `OnAfterTemplateRegistrations`, or a template's `AfterTemplateRegistration()` override | **Yes**, for every request type |
                    | Factory extension `OnBeforeTemplateExecution`, or a template's `BeforeTemplateExecution()` override | **Yes**, except `InfrastructureRegisteredEvent`: its health-check handler responds with an `OnBuild` callback, and every file has already been built by then, so the handler throws |

                    The two sides use different hooks. A **template** overrides `AfterTemplateRegistration()` / `BeforeTemplateExecution()` — no `On` prefix, no parameter — and publishes through `ExecutionContext.EventDispatcher` (or `EmitOrPublish`, below). A **factory extension** overrides `protected void OnAfterTemplateRegistrations(IApplication application)` / `OnBeforeTemplateExecution(IApplication application)` and publishes through `application.EventDispatcher`.

                    ```csharp
                    // Template side — override the lifecycle hook (no "On" prefix, no parameter).
                    public override void AfterTemplateRegistration()
                    {
                        base.AfterTemplateRegistration();

                        ExecutionContext.EventDispatcher.Publish(
                            ContainerRegistrationRequest
                                .ToRegister(this)
                                .ForConcern("Infrastructure"));
                    }

                    // Factory-extension side — protected override with the IApplication parameter.
                    protected override void OnAfterTemplateRegistrations(IApplication application)
                    {
                        application.EventDispatcher.Publish(
                            new AppSettingRegistrationRequest("MySection:Enabled", true));
                    }
                    ```

                    ### Template-side form — `EmitOrPublish` / `OnEmitOrPublished` (Intent.Common 3.9.0+)

                    Inside a template, prefer `EmitOrPublish(request)`. It first *emits* the request through the output-target graph, so a handler in the publisher's own project, or in a project that references it, gets it first — which keeps requests in the right host when a solution has several. Only when no such handler takes it does it fall back to a solution-wide `EventDispatcher.Publish`. A host template you own subscribes with `OnEmitOrPublished<T>(...)` in its constructor; that also subscribes to the plain dispatcher, so requests published the old way still reach it.

                    ```csharp
                    // Publisher (inside a template) — needs Intent.Common 3.9.0+
                    public override void BeforeTemplateExecution()
                    {
                        EmitOrPublish(
                            ContainerRegistrationRequest
                                .ToRegister(this)
                                .ForConcern("Infrastructure"));
                    }

                    // Handler (a host template you own) — subscribe in the constructor
                    public MyHostTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
                    {
                        OnEmitOrPublished<ContainerRegistrationRequest>(request => _requests.Add(request));
                    }
                    ```

                    > [!IMPORTANT]
                    > `EmitOrPublish` and `OnEmitOrPublished` were added in `Intent.Common` **3.9.0**; Module Builder's default floor is 3.7.2. Before using them, raise the `Intent.Common` `<dependency>` and the `Intent.Modules.Common` package reference to 3.9.0 with `Intent.SoftwareFactory.SDK` 3.10.0 (Intent Architect 4.5.15 or later), or restore fails with `NU1605`. Against the 3.7.2 package the code does not compile; built against a newer package but installed with an older `Intent.Common`, it throws `MissingMethodException` at run time. To let other subscribers see an event after yours handles it, pass `stopPropagation: false` (3.9.0). The `OnEmitOrPublished<T>(Func<T, bool> handler)` overload lets your handler decline: return `false` and the event is not marked handled, so it keeps propagating and falls back to `Publish`. Such a handler is also subscribed for the plain `Publish` fallback, so after declining it runs a second time. It needs **3.11.4**, which needs the `Intent.SoftwareFactory.SDK` package reference raised to 3.14.0, and so Intent Architect 5.0.0 or later.

                    Factory extensions have no `EmitOrPublish`; they keep using `application.EventDispatcher.Publish(...)`.

                    ===

                    ## §DI Registration — ContainerRegistrationRequest

                    These run inside a template (`this` is the template), from its `BeforeTemplateExecution()` override or any other safe point in §When to Publish. The host template collects all requests and merges them into its DI registration method.

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
                            .ForConcern("Application")                  // routes to the Application DependencyInjection template
                            .WithPerServiceCallLifeTime()               // Transient | PerServiceCall | Singleton
                            .HasDependency(this));                      // adds the template dependency and its using

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

                    #### `ContainerRegistrationRequest.LifeTime` constants

                    You declare the *lifetime*; the host template owns the mapping onto whatever registration method its
                    container uses. That mapping is the host's business, not yours — which is exactly why you publish a
                    request instead of writing the call yourself.

                    | Constant | Meaning | Typical host mapping |
                    |---|---|---|
                    | `LifeTime.Transient` | Created on every resolution | `AddTransient` |
                    | `LifeTime.PerServiceCall` | Scoped to one request/unit-of-work | `AddScoped` |
                    | `LifeTime.Singleton` | Single instance for application lifetime | `AddSingleton` |

                    ===

                    ## §AppSettings — AppSettingRegistrationRequest

                    Publish from a safe point in §When to Publish (these snippets sit in a template's `BeforeTemplateExecution()`). The host `appsettings.json` template merges the keys; values already in the file are left untouched.

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
                    extension method** during startup, publish one of these. Same rules as its siblings: publish from a
                    safe point in §When to Publish, and the host template merges, orders and de-duplicates.

                    ```csharp
                    // Produces: services.AddMyFeature();   (or builder.Services.AddMyFeature(); — the host decides)
                    ExecutionContext.EventDispatcher.Publish(
                        ServiceConfigurationRequest
                            .ToRegister("AddMyFeature")
                            .ForConcern("Infrastructure")            // routes to the Infrastructure DependencyInjection template
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
                    whichever startup shape it is generating. A second `ConnectionStringRegistrationRequest` with the same
                    name but a different connection string is treated as a misconfiguration and throws.

                    ### Which request for which job

                    Every type in this table lives in `Intent.Modules.Common.CSharp` (namespaces `...DependencyInjection` and
                    `...Configuration`) and is available at Module Builder's default `Intent.Common.CSharp` floor of 3.8.0,
                    except where a later version is named.

                    | You want | Publish |
                    |---|---|
                    | A type resolved from the container | `ContainerRegistrationRequest` |
                    | A `Services.AddX()` call at startup | `ServiceConfigurationRequest` |
                    | A middleware `app.UseX()` call | `ApplicationBuilderRegistrationRequest` |
                    | A configuration key or section | `AppSettingRegistrationRequest` |
                    | A named connection string | `ConnectionStringRegistrationRequest` |
                    | An environment variable in `launchSettings.json` | `EnvironmentVariableRegistrationRequest` |
                    | A whole profile in `launchSettings.json` | `LaunchProfileRegistrationRequest` (its `CommandLineArgs`, `ExecutablePath` and `WorkingDirectory` need `Intent.Common.CSharp` 3.9.5, with `Intent.Common` 3.9.0 and SDK 3.10.0 raised alongside it) |
                    | The `launchUrl` path of the default profiles | `DefaultLaunchUrlPathRequest`, via `this.PublishDefaultLaunchUrlPathRequest(...)` (or `OutputTarget.EmitDefaultLaunchUrlPathRequest(...)`, which needs the same 3.9.5 raise) |
                    | A key in an Azure Functions `host.json` | `HostSettingRegistrationRequest` |
                    | Tell other modules (health checks) about a database, cache or broker you set up | `InfrastructureRegisteredEvent` |

                    ### ⚠ Shape vs Wire — the most common wrong turn

                    The rule: **shaping a generated type → FileBuilder mutation; wiring into host infrastructure → publish a request.**

                    Reaching for `CSharpFile` on the startup template to append a registration line looks more direct, but
                    it moves the whole burden onto you: knowing which host shape you landed in, injecting the right
                    `using`, placing the statement in the correct block, and not appending it a second time on the next
                    run. Publishing a request hands all of that to the host template that owns the file. Several different
                    host templates listen for these requests and each maps them onto its own startup shape — which is
                    precisely the knowledge you avoid having to encode.

                    ===

                    ## §Launch Settings, host.json & Infrastructure

                    The `launchSettings.json` and `host.json` requests are handled by the project templates of the
                    `Intent.VisualStudio.Projects` module; `InfrastructureRegisteredEvent` is consumed by
                    `Intent.AspNetCore.HealthChecks`. When none of those is installed the request is simply not handled.

                    ```csharp
                    // launchSettings.json — an environment variable on every profile (or only the named ones)
                    ExecutionContext.EventDispatcher.Publish(
                        new EnvironmentVariableRegistrationRequest(
                            key:                "MY_FEATURE_MODE",
                            value:              "Local",
                            targetProfiles:     null,
                            forProjectWithRole: null));

                    // launchSettings.json — a whole profile
                    ExecutionContext.EventDispatcher.Publish(
                        new LaunchProfileRegistrationRequest
                        {
                            Name           = "My Tool",
                            CommandName    = "Project",
                            LaunchBrowser  = false,
                            ApplicationUrl = "http://localhost:5080",
                            EnvironmentVariables = new Dictionary<string, string> { ["ASPNETCORE_ENVIRONMENT"] = "Development" }
                        });

                    // Azure Functions host.json — colon-separated key targets nested objects
                    ExecutionContext.EventDispatcher.Publish(
                        new HostSettingRegistrationRequest("extensions:serviceBus:prefetchCount", 100));

                    // launchSettings.json launchUrl path for the default profiles — at most once per run
                    this.PublishDefaultLaunchUrlPathRequest("swagger");
                    ```

                    `DefaultLaunchUrlPathRequest` has an internal constructor: publish it only through the extension
                    method, from `BeforeTemplateExecution()`, and only once — a second publish throws.

                    An **`InfrastructureRegisteredEvent`** names an infrastructure component plus string properties. Publish it
                    from `OnAfterTemplateRegistrations` (or a template's `AfterTemplateRegistration()`), never later — see
                    §When to Publish.

                    ```csharp
                    protected override void OnAfterTemplateRegistrations(IApplication application)
                    {
                        // Infrastructure for health checks — publish no later than this hook.
                        application.EventDispatcher.Publish(
                            new InfrastructureRegisteredEvent("SqlServer")
                                .WithProperty("ConnectionStringName", "DefaultConnection"));
                    }
                    ```

                    The health-check module recognises these component names: `"SqlServer"`, `"PostgreSql"`, `"MySql"`,
                    `"CosmosDb"`, `"MongoDb"`, `"Oracle"` and `"Redis"` (properties `"ConnectionStringName"` and
                    `"ConnectionStringSettingPath"`), and `"Kafka"` (properties `"KafkaSettingsPath"`, `"KafkaSettingsName"`
                    and `"KafkaSchemaRegistryUrl"`). Any other name is ignored.

                    ===

                    ## §Resolution — FindTemplateInstance & Safe Guards

                    ### Step 1 — Role-based single lookup

                    ```csharp
                    // Returns null when the template is not registered — ALWAYS guard before use
                    var diTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                        "Application.DependencyInjection");

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
                    // "Domain.Entity" — the entity type; NOT "Domain.Entity.Primary" (see the trap in SKILL.md)
                    var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
                        "Domain.Entity");

                    foreach (var template in templates)
                    {
                        // MUST: narrow to templates whose model matches the expected type
                        // (ClassModel comes from Intent.Modules.Modelers.Domain — a Tier 2 dependency)
                        if (!template.TryGetModel<ClassModel>(out var classModel))
                            continue;

                        template.CSharpFile.OnBuild(file =>
                        {
                            var cls = file.Classes.First();
                            // ... enrich based on classModel
                        }, 500);                             // Extension band: another module's template
                    }
                    ```

                    ### Step 3 — Role + model-id lookup (fallback chain)

                    ```csharp
                    // Preferred when the model element is known at resolution time.
                    // Each role is tried in order; TryGetTemplate never throws.
                    if (!template.TryGetTemplate(
                            "Domain.Entity",
                            mappedElementId,
                            out ICSharpFileBuilderTemplate entityTemplate)
                        && !template.TryGetTemplate(
                            "Domain.ValueObject",
                            mappedElementId,
                            out entityTemplate)
                        && !template.TryGetTemplate(
                            "Domain.DataContract",
                            mappedElementId,
                            out entityTemplate))
                    {
                        throw new InvalidOperationException(
                            $"Could not resolve the mapped type for element '{mappedElementId}'.");
                    }

                    var typeName = template.GetTypeName(entityTemplate);
                    ```

                    `template` is your own template; `mappedElementId` is the id of the designer element you need the type for
                    (for example a mapping's target element).

                    ### Step 4 — Template-id fallback chain (DoNotThrow)

                    ```csharp
                    // TryGetTemplate already probes without throwing — chain the candidates.
                    bool resolved =
                        template.TryGetTemplate<ICSharpTemplate>("MyEcosystem.CosmosDb.UnitOfWorkInterface", out var uow)
                        || template.TryGetTemplate<ICSharpTemplate>("MyEcosystem.DynamoDb.UnitOfWorkInterface", out uow)
                        || template.TryGetTemplate<ICSharpTemplate>("MyEcosystem.MongoDb.UnitOfWorkInterface", out uow);

                    if (!resolved) return;

                    // Methods that throw by default take TemplateDiscoveryOptions.DoNotThrow and return null instead.
                    var uowTypeName = template.GetTypeName("MyEcosystem.CosmosDb.UnitOfWorkInterface", TemplateDiscoveryOptions.DoNotThrow);
                    if (uowTypeName == null) return;
                    ```

                    Use the shared `TemplateDiscoveryOptions.DoNotThrow` instance; there is no need to declare your own.

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
                    | `GetTypeName(templateIdOrRole, model)` | Throws / assumes presence | A target your module genuinely requires |
                    | `TryGetTypeName(templateIdOrRole, out var name)` | Returns `false` | An optional, best-effort integration |

                    Overloads mirror the `GetTypeName` family — by template id or role alone, or plus a model / model id.
                    The overloads that take a *list* of candidate ids (for a fallback chain) need `Intent.Common` 3.7.3 or
                    later; at the 3.7.2 floor, chain single calls as in Step 4.

                    ### ⚠ Cross-Module Boundary Rule — Use Interface Types, Not Concrete Types

                    When calling `FindTemplateInstance<T>` or `FindTemplateInstances<T>` **from a different module** (i.e. a factory extension in module B looking up a template registered by module A), always use an interface type for `T` — never a concrete template class.

                    IA may load each module's assembly in an isolated `AssemblyLoadContext`. When it does, the concrete `MyTemplate` type in module B's context is a different `Type` object from the one registered by module A in its context — the lookup silently returns `null` with no error.

                    The string you pass — **a role, or a template id when the target has no role** — is the lookup key; prefer the role. The interface type is the secondary filter that crosses ALC boundaries safely.

                    ```csharp
                    // ❌ Wrong — returns null when called from a different module's factory extension
                    var t = application.FindTemplateInstance<MessagingConfigurationTemplate>(
                        "MyEcosystem.Messaging.Configuration");

                    // ✅ Correct — interface types from shared packages cross ALC boundaries safely
                    var t = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                        "MyEcosystem.Messaging.Configuration");
                    ```

                    | Scenario | Interface to use |
                    |---|---|
                    | Template exposes a `CSharpFile` property | `ICSharpFileBuilderTemplate` (from `Intent.Modules.Common.CSharp`) |
                    | Template has a typed model | `IIntentTemplate<TModel>` (from `Intent.Modules.Common`, namespace `Intent.Modules.Common.Templates`) |
                    | Role lookup only, no CSharpFile needed | `ICSharpTemplate` (from `Intent.Modules.Common.CSharp`) |

                    ### Null-conditional shorthand (optional single-template enrichment)

                    ```csharp
                    // Safe when the template is genuinely optional
                    application
                        .FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.DependencyInjection")
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

                    `IHandlerModel`, `IOperationModel` and `IParameterModel` below stand for the host module's typed model
                    interfaces (Tier 2), and `HasAuditable()` for the accessor Module Builder generates from your own
                    "Auditable" stereotype.

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

                                // Typed accessor generated from your own "Auditable" stereotype — never HasStereotype("Auditable").
                                if (!operation.HasAuditable())
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

                    #### Rules for the bridge

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

                    Why the split: a role string is just a string — resolving one costs nothing and couples you to
                    nothing. But the *model interfaces* a host module exposes are types defined **inside that module's
                    assembly**. Referencing them means compiling against it, which means declaring it as a dependency:

                    ```xml
                    <!-- In your .imodspec — required only for Tier 2, with a matching PackageReference in the .csproj -->
                    <dependencies>
                      <dependency id="MyEcosystem.Application.Handlers" version="1.2.0" />
                    </dependencies>
                    ```

                    The tradeoff: Tier 1 is free but blind — you can shape generated code without knowing what any of
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

                        // ── Find templates, schedule build callbacks — every template now exists ──
                        protected override void OnAfterTemplateRegistrations(IApplication application)
                        {
                            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
                                TemplateDependency.OnTemplate("Application.DependencyInjection"));

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

                        // ── Publish registration requests (OnAfterTemplateRegistrations works too) ──
                        protected override void OnBeforeTemplateExecution(IApplication application)
                        {
                            application.EventDispatcher.Publish(
                                new AppSettingRegistrationRequest("MySection", new { Enabled = true }));
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

                    The **Find Rule**: Template B must use a **strictly higher priority** than Template A when B calls `FindMethod`/`FindClass` on elements A created. If B's priority ≤ A's, A may not have run yet.

                    ===

                    ## §Resolution & Consumption — Stereotype-Driven AfterBuild

                    Bridges the two skills: resolve the template (orchestrator), then consume the stereotype (`intent-metadata-consumer`). `GetMySettings()` is the accessor Module Builder generates in your module for a "My Settings" stereotype with a checkbox, a text, a number and a select property.

                    ```csharp
                    // Factory extension: enriches entity templates based on stereotype metadata
                    protected override void OnAfterTemplateRegistrations(IApplication application)
                    {
                        var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
                            TemplateDependency.OnTemplate("Domain.Entity"));

                        foreach (var template in templates)
                        {
                            // MUST: verify model type before consuming stereotype
                            if (!template.TryGetModel<ClassModel>(out var classModel))
                                continue;

                            // MUST: use the generated typed accessor — NOT HasStereotype("My Settings")
                            var settings = classModel.GetMySettings();
                            if (settings == null)
                                continue;

                            template.CSharpFile.AfterBuild(file =>
                            {
                                var cls = file.Classes.First();

                                // Bool property → conditional attribute
                                if (settings.IsTimestamped())
                                    cls.AddAttribute(template.UseType("MyModule.TimestampedAttribute"));

                                // String property → attribute argument
                                var tableName = settings.TableName();
                                if (!string.IsNullOrEmpty(tableName))
                                    cls.AddAttribute("Table", a => a.AddArgument($"\"{tableName}\""));

                                // Number property → int? — null-guard before use
                                if (settings.MaxLength() is { } max && max > 0)
                                {
                                    cls.Properties.FirstOrDefault(p => p.Name == "Name")
                                        ?.AddAttribute("MaxLength", a => a.AddArgument($"{max}"));
                                }

                                // Select property → deterministic switch on the nested enum
                                switch (settings.Mode().AsEnum())
                                {
                                    case ClassModelStereotypeExtensions.MySettings.ModeOptionsEnum.Extended:
                                        cls.AddAttribute("ExtendedMode");
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

                    // Register mutations inside an OnBuild callback, with an explicit priority
                    startup.CSharpFile.OnBuild(_ =>
                    {
                        var sf = startup.StartupFile;
                        // ... DSL calls below ...
                    }, 100);
                    ```

                    ### Context variable reference

                    | Context property | Type in minimal hosting | Type in generic hosting |
                    |---|---|---|
                    | `context.Services` | `"builder.Services"` | `"services"` |
                    | `context.Configuration` | `"builder.Configuration"` | `"Configuration"` (the `Startup` class property) |
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
                    // Produces: services.AddControllers(opt => { opt.Filters.Add<MyExceptionFilter>(); })
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
                                $"{context.Parameters[0]}.Filters.Add<{template.GetTypeName("MyModule.MyExceptionFilter")}>();");

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

                    `AddAppConfigurationLambda("UseEndpoints", ...)` throws in the ASP.NET Core startup host (`Intent.AspNetCore`). Use `AddUseEndpointsStatement(...)` for endpoint mapping.

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
                            statements.Statements
                                .Single(x => x.ToString()!.Contains(".MapControllers("))
                                .InsertBelow(new CSharpInvocationStatement($"{ctx.Endpoints}.MapRazorPages"));
                        }
                    });
                    ```

                    ### 7. Low-level ConfigureServices / ConfigureApp for positional edits

                    Use when you need to read the existing statement list and perform conditional insertion.

                    ```csharp
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
                    | `ExposeProgramClass()` | ✓ | — | Make `partial class Program` public (top-level stmts) — needs `Intent.Common.CSharp` 3.8.10 |

                    > ⚠️ `AddAppConfigurationLambda("UseEndpoints", ...)` throws — use `AddUseEndpointsStatement` instead.

                    The `IAppStartupFile` interface is in `Intent.Modules.Common.CSharp` (namespace `Intent.Modules.Common.CSharp.AppStartup`); the startup host modules (for example `Intent.AspNetCore`) implement it. Everything in the table except `ExposeProgramClass()` is available at the default `Intent.Common.CSharp` floor of 3.8.0. To use `ExposeProgramClass()`, raise the `Intent.Common.CSharp` `<dependency>` and the `Intent.Modules.Common.CSharp` package reference to 3.8.10, and `Intent.Modules.Common` / the `Intent.Common` `<dependency>` to 3.7.3 with them (3.8.10 depends on it; leaving Module Builder's 3.7.2 fails restore with `NU1605`).

                    ===

                    ## TemplateDependency Quick Ref

                    ```csharp
                    TemplateDependency.OnTemplate("Domain.Entity")
                    // ──► matches every template whose TemplateId or one of whose roles equals the string exactly

                    TemplateDependency.OnTemplate(MyTemplate.TemplateId)
                    // ──► matches by exact TemplateId constant

                    TemplateDependency.OnModel(MyTemplate.TemplateId, modelElement)
                    // ──► matches by TemplateId AND model — useful for model-scoped lookups

                    TemplateDependency.OfType<ICSharpFileBuilderTemplate>()
                    // ──► matches ALL registered CSharpFile builder templates
                    ```

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
