---
name: intent-module-orchestrator
description: "Wire a module's cross-module integration logic — DI/config registration requests, startup DSL calls, priority-banded callbacks, and template Role/TemplateId lookups across module boundaries. USE ONLY WHEN one module's template needs to enrich, depend on, or register infrastructure (DI, appsettings, startup) owned by another module's generated output. DO NOT USE FOR authoring a single template's own C# builder statements (see file-builder-expert) or reading designer metadata off a model (see intent-metadata-consumer). REQUIRES the target module/template's Role or TemplateId already identified."
argument-hint: "[event type | factory extension scenario] [target template role or id]"
template-id: Intent.ModuleBuilder.AI.Skills.Skills.IntentModuleOrchestrator_SkillMd_Agents
contentHash: 729E9246E9FF6E722B5E2E1D149E7BCBDE43DD0C42977CB9DE378186D43C17C9
---
# Intent Module Orchestrator

> [!TIP]
> **Read more if you want to know about** priority bands, broker filtering, Startup DSL, DI/Config registration events, or cross-template lookups:
> *   [Orchestration Cheatsheet](./resources/orchestration-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Safe Resolution:** Prefer Role-based lookup via `TemplateRoles.*`. Guard templates (use `?.` or check null) before accessing `CSharpFile`. For an **optional** integration — enrich only if another module happens to be installed — resolve its type with `TryGetTypeName(templateId, out var typeName)` and skip silently when it returns `false`. `GetTypeName(...)` is the non-optional form and presumes the target is present.
2. **Callbacks:** Use `TryGetModel<T>` to verify model shape; use `TryGetTemplate(...)` for multi-fallback chains.
3. **Shape vs Wire:** Use **FileBuilder mutation** when shaping a generated type; use **request-publishing** when wiring into host infrastructure (DI, startup, app settings, connection strings). Request-publishing is not legacy — the host template owns lifetime mapping, `using` injection and de-duplication, and only it knows which startup shape it is generating.
4. **DI/Config Events:** Publish `ContainerRegistrationRequest` (register a type), `ServiceConfigurationRequest` (a `Services.AddX()` call), `ApplicationBuilderRegistrationRequest` (an `app.UseX()` middleware call), `AppSettingRegistrationRequest` and `ConnectionStringRegistrationRequest` from `OnBeforeTemplateExecution` (never from `OnAfterTemplateRegistrations`).
5. **The Model Bridge:** To answer *"which designer element is this generated node?"*, read the model the host template stamped onto it — `node.TryGetMetadata<TModel>("model", out var model)`. Methods, parameters and properties each carry their own. Always `TryGetMetadata`, never `GetMetadata` — the key is a host-template convention, not a framework guarantee.
6. **Dependencies:** Declare with `.HasDependency(...)`. Set `ForConcern` for specific startup target files.
7. **Two-Tier Module Dependency:** Role-string lookup through a generic interface (`ICSharpFileBuilderTemplate`) needs **no** dependency on the target module in your `.imodspec`. Reading that module's **typed model interfaces** does — declare it and accept the version coupling. Decide which tier you need before you start.
8. **Priority Bands:** Pass explicit priorities to `AfterBuild` (e.g. 0=Core, 100=Enrichment, 500=Extension, 1000=Final).
9. **Startup DSL:** Use `IAppStartupFile` DSL (e.g., `AddServiceConfiguration`) over manual `FindMethod` edits.
10. **Broker Filter:** Filter event subscriptions using `.FilterMessagesForThisMessageBroker(ExecutionContext, ...)` (pass `ExecutionContext`).
11. **NuGet Packaging:** Dispatch modules do not need to install target NuGets if the core module already does.

## Must Nots

1. Never use Regex to modify `Program.cs` or `appsettings.json`.
2. Never publish registration requests from `OnAfterTemplateRegistrations`.
3. Never call `AddAppConfigurationLambda("UseEndpoints", ...)`; use `AddUseEndpointsStatement` instead.
4. Never correlate a generated member with a designer element by **matching names**. It is fragile — names are transformed, de-duplicated and overloaded — and unnecessary; read the `"model"` metadata instead.
5. Never take a hard module dependency purely to enrich another module's output. If role-string lookup gets you there, stay at Tier 1.
