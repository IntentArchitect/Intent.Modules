---
name: intent-module-orchestrator
description: Wire cross-module logic, DI/appsettings events, priority bands, and template lookups.
argument-hint: "[event type | factory extension scenario] [target template role or id]"
contentHash: 737F5CB160613DA4B1276723597487FDEB73D4CFDDF886E1B72224FD8F542C4E
---
# Intent Module Orchestrator

> [!TIP]
> **Read more if you want to know about** priority bands, broker filtering, Startup DSL, DI/Config registration events, or cross-template lookups:
> *   [Orchestration Cheatsheet](./resources/orchestration-cheatsheet.md)
> *(To conserve tokens, avoid reading this file for simple or minor updates.)*

## Musts

1. **Safe Resolution:** Prefer Role-based lookup via `TemplateRoles.*`. Guard templates (use `?.` or check null) before accessing `CSharpFile`.
2. **Callbacks:** Use `TryGetModel<T>` to verify model shape; use `TryGetTemplate(...)` for multi-fallback chains.
3. **DI/Config Events:** Publish `ContainerRegistrationRequest` / `AppSettingRegistrationRequest` from `OnBeforeTemplateExecution` (never from `OnAfterTemplateRegistrations`).
4. **Dependencies:** Declare with `.HasDependency(...)`. Set `ForConcern` for specific startup target files.
5. **Priority Bands:** Pass explicit priorities to `AfterBuild` (e.g. 0=Core, 100=Enrichment, 500=Extension, 1000=Final).
6. **Startup DSL:** Use `IAppStartupFile` DSL (e.g., `AddServiceConfiguration`) over manual `FindMethod` edits.
7. **Broker Filter:** Filter event subscriptions using `.FilterMessagesForThisMessageBroker(ExecutionContext, ...)` (pass `ExecutionContext`).
8. **NuGet Packaging:** Dispatch modules do not need to install target NuGets if the core module already does.

## Must Nots

1. Never use Regex to modify `Program.cs` or `appsettings.json`.
2. Never publish registration requests from `OnAfterTemplateRegistrations`.
3. Never call `AddAppConfigurationLambda("UseEndpoints", ...)`; use `AddUseEndpointsStatement` instead.
