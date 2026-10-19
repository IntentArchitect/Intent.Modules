# Context: Intent.Modelers.UI.ServiceProxies

## Purpose

Extends the User Interface designer with the `Service Proxy` element defined in Intent.Modelers.Types.ServiceProxies. It adds a creation option on the User Interface package, makes proxy Operations invokable and mappable from **Call Service Operation**, and keeps proxy parameters in sync with their endpoints.

## Invariants & Constraints

- **The `Service Proxy Extension` `On Changed` script is one of three live copies of the parameter-sync script.** The other two are in Intent.Modelers.Types.ServiceProxies, which records the full picture in its own `CONTEXT.md`. A fix to one must be applied to all three. The source is `DesignerMacros/src/service-proxies/manage-parameters/services-proxies-manage-parameters.ts`, but the copies have drifted from it, so patch each copy rather than recompiling and pasting.
  - This copy creates Parameters by type id (`const parameterTypeId = "f11273c1-…"`, the Proxy `Parameter` type) rather than by the name `"Parameter"`. It has been that way since the script was first added in 2024 (c77cd8f8a), and the reason was never recorded. Preserve it when patching.
  - It must tolerate an Operation with no mapping (1.0.4). See the Types module's `CONTEXT.md` for how such Operations arise.
