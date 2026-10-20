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

namespace Intent.Modules.ModuleBuilder.AI.Skills.Templates.Skills.IntentModuleOrchestrator_SkillMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentModuleOrchestrator_SkillMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Skills.Skills.IntentModuleOrchestrator_SkillMd_Agents";

        internal const string SkillName = "intent-module-orchestrator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentModuleOrchestrator_SkillMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: SkillName)
                .FromMarkdown($$""""""
                    ---
                    name: {{SkillName}}
                    description: "Wire a module's cross-module integration logic — DI/config registration requests, startup DSL calls, priority-banded callbacks, and template Role/TemplateId lookups across module boundaries. USE ONLY WHEN one module's template needs to enrich, depend on, or register infrastructure (DI, appsettings, startup) owned by another module's generated output. DO NOT USE FOR authoring a single template's own C# builder statements (see file-builder-expert) or reading designer metadata off a model (see intent-metadata-consumer). REQUIRES the target module/template's Role or TemplateId already identified."
                    argument-hint: "[event type | factory extension scenario] [target template role or id]"
                    template-id: {{TemplateId}}
                    ---

                    # Intent Module Orchestrator

                    > [!TIP]
                    > **Read more if you want to know about** when to publish requests, `EmitOrPublish`, priority bands, Startup DSL, DI/Config/launch-settings requests, or cross-template lookups:
                    > *   [Orchestration Cheatsheet](./resources/orchestration-cheatsheet.md)
                    > *(To conserve tokens, avoid reading this file for simple or minor updates.)*

                    ## Musts
                    1. **Safe Resolution:** Look templates up by a **literal role string** from the *Common Role Values* table below (or by the target's template id string when it has no role). Guard templates (use `?.` or check null) before accessing `CSharpFile`. For an **optional** integration — enrich only if another module happens to be installed — resolve its type with `TryGetTypeName(templateIdOrRole, out var typeName)` and skip silently when it returns `false`. `GetTypeName(...)` is the non-optional form and presumes the target is present.
                    2. **Callbacks:** Use `TryGetModel<T>` to verify model shape; use `TryGetTemplate(...)` for multi-fallback chains.
                    3. **Shape vs Wire:** Use **FileBuilder mutation** when shaping a generated type; use **request-publishing** when wiring into host infrastructure (DI, startup, app settings, connection strings, launch settings). Request-publishing is not legacy — the host template owns lifetime mapping, `using` injection and de-duplication, and only it knows which startup shape it is generating.
                    4. **DI/Config Requests:** Publish `ContainerRegistrationRequest` (register a type), `ServiceConfigurationRequest` (a `Services.AddX()` call), `ApplicationBuilderRegistrationRequest` (an `app.UseX()` middleware call), `AppSettingRegistrationRequest`, `ConnectionStringRegistrationRequest` and the launch-settings, host.json and `InfrastructureRegisteredEvent` types in the cheatsheet once every template has been registered: from a factory extension's `OnAfterTemplateRegistrations` or `OnBeforeTemplateExecution`, or from a template's `AfterTemplateRegistration()` or `BeforeTemplateExecution()` override. `InfrastructureRegisteredEvent` must go out no later than `OnAfterTemplateRegistrations` / `AfterTemplateRegistration()`.
                    5. **Template-Side Publishing:** Inside a template, prefer `EmitOrPublish(request)` to `ExecutionContext.EventDispatcher.Publish(request)`; a host template you own subscribes with `OnEmitOrPublished<T>(...)`. Both need `Intent.Common` 3.9.0: raise the `.imodspec` `<dependency>` and the `Intent.Modules.Common` package reference to 3.9.0 with `Intent.SoftwareFactory.SDK` 3.10.0 (Intent Architect 4.5.15 or later), or restore fails with `NU1605` (see `known-build-gotchas`). The default floor of 3.7.2 lacks them, and a module built against 3.9.0 but installed with an older `Intent.Common` throws `MissingMethodException`.
                    6. **The Model Bridge:** To answer *"which designer element is this generated node?"*, read the model the host template stamped onto it — `node.TryGetMetadata<TModel>("model", out var model)`. Methods, parameters and properties each carry their own. Always `TryGetMetadata`, never `GetMetadata` — the key is a host-template convention, not a framework guarantee.
                    7. **Dependencies:** `.HasDependency(...)` adds the template dependency and the `using` for the registered type. `ForConcern("Application")` / `ForConcern("Infrastructure")` picks which DependencyInjection template takes the registration.
                    8. **Two-Tier Module Dependency:** Role-string lookup through a generic interface (`ICSharpFileBuilderTemplate`) needs **no** dependency on the target module in your `.imodspec`. Reading that module's **typed model interfaces** does — declare it and accept the version coupling. Decide which tier you need before you start.
                    9. **Priority Bands:** Pass explicit priorities to every `OnBuild` / `AfterBuild` (e.g. 0=Core, 100=Enrichment, 500=Extension, 1000=Final).
                    10. **Startup DSL:** Use the `IAppStartupFile` DSL (e.g., `AddServiceConfiguration`), resolved through `IAppStartupTemplate.RoleName`, over manual `FindMethod` edits.
                    11. **NuGet Packaging:** Dispatch modules do not need to install target NuGets if the core module already does.

                    ## Must Nots
                    1. Never use Regex to modify `Program.cs` or `appsettings.json`.
                    2. Never publish a registration request from a template **constructor** (or a factory extension's `OnBeforeTemplateRegistrations`). Handlers subscribe in their own template constructors, and the event dispatcher delivers synchronously and keeps nothing for late subscribers: a request published before its handler exists is dropped with no error.
                    3. Never call `AddAppConfigurationLambda("UseEndpoints", ...)`; use `AddUseEndpointsStatement` instead.
                    4. Never correlate a generated member with a designer element by **matching names**. It is fragile — names are transformed, de-duplicated and overloaded — and unnecessary; read the `"model"` metadata instead.
                    5. Never take a hard module dependency purely to enrich another module's output. If role-string lookup gets you there, stay at Tier 1.
                    6. Never reference `TemplateRoles.*` or anything else from `Intent.Modules.Constants`. That package is not published on nuget.org, so a module that uses it does not compile outside Intent's own repository. Use the literal role string instead.

                    ## Common Role Values

                    Roles are plain strings, matched exactly. A template answers to its configured role and to any extra role it registers at runtime. The **Constant** column names the `TemplateRoles` constant you may see in Intent's own modules, only so you can translate it; never reference the constant itself.

                    | Role string | Resolves to | Registered by | Constant (do not use) |
                    |---|---|---|---|
                    | `"Domain.Entity"` | The entity class to use as a type: the entity, or its state class when "Separate State from Behaviour" is on | `Intent.Entities` | `TemplateRoles.Domain.Entity.Primary` |
                    | `"Domain.Entity.Primary"` | The `Intent.Entities.DomainEntity` template itself: the behaviour class when state is separated | `Intent.Entities` | `TemplateRoles.Domain.Entity.EntityImplementation` |
                    | `"Domain.Entity.Interface"` | The entity interface, or the entity class when entity interfaces are off | `Intent.Entities` | `TemplateRoles.Domain.Entity.Interface` |
                    | `"Domain.Enum"` | Domain enums | `Intent.Entities` | `TemplateRoles.Domain.Enum` |
                    | `"Domain.DataContract"` | Domain data contracts | `Intent.Entities` | `TemplateRoles.Domain.DataContract` |
                    | `"Domain.ValueObject"` | Value objects | `Intent.ValueObjects` | `TemplateRoles.Domain.ValueObject` |
                    | `"Application.Contract.Dto"` | Service-layer DTOs | `Intent.Application.Dtos` | `TemplateRoles.Application.Contracts.Dto` |
                    | `"Application.Command"` / `"Application.Query"` | CQRS command and query classes | `Intent.Application.MediatR` and other CQRS dispatch modules | `TemplateRoles.Application.Command` / `.Query` |
                    | `"Application.Command.Handler"` / `"Application.Query.Handler"` | Their handlers | `Intent.Application.MediatR` and other CQRS dispatch modules | `TemplateRoles.Application.Handler.Command` / `.Query` |
                    | `"Application.DependencyInjection"` | Application-layer `AddApplication` DI class | `Intent.Application.DependencyInjection` | `TemplateRoles.Application.DependencyInjection` |
                    | `"Infrastructure.DependencyInjection"` | Infrastructure-layer `AddInfrastructure` DI class | `Intent.Infrastructure.DependencyInjection` | `TemplateRoles.Infrastructure.DependencyInjection` |
                    | `"Infrastructure.Data.DbContext"` | The EF Core `DbContext` | `Intent.EntityFrameworkCore` | `TemplateRoles.Infrastructure.Data.DbContext` |
                    | `"Repository.Interface.Entity"` / `"Repository.Implementation.Entity"` | Entity repository interface and implementation | The repository modules (for example `Intent.EntityFrameworkCore.Repositories`) | `TemplateRoles.Repository.Interface.Entity` / `.Implementation.Entity` |
                    | `"Distribution.WebApi.Controller"` | ASP.NET Core controllers | `Intent.AspNetCore.Controllers` | `TemplateRoles.Distribution.WebApi.Controller` |
                    | `IAppStartupTemplate.RoleName` | The startup or `Program` file, through `IAppStartupTemplate` | Startup hosts such as `Intent.AspNetCore` | — (an SDK property in `Intent.Modules.Common.CSharp`; use it directly) |

                    > [!WARNING]
                    > **The `Domain.Entity.Primary` trap.** The constant `TemplateRoles.Domain.Entity.Primary` has the value `"Domain.Entity"`. The string `"Domain.Entity.Primary"` is a **different role**. Translating the constant's *name* into a string resolves the wrong template: with "Separate State from Behaviour" on, you get the behaviour partial instead of the entity type. When porting code that used the constant, write `"Domain.Entity"`.

                    """""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
