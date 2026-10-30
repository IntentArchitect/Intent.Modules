# Context: Intent.Modelers.Services.ProxyInteractions

## Purpose

Adds context-menu options to Service Proxies and their Operations in the Services designer: **Create CQRS Operations**, **Create Service**, **Create CQRS Operation** and **Create Service Operation**. Each creates (or updates) the Commands, Queries or Service Operations that invoke the proxy's Operations through `Perform Invocation`, with their fields mapped to the Operation's parameters.

## Architectural Decisions

- **The element for a proxy Operation is found by its `Perform Invocation`, not by name** (1.1.8). Since Intent.Modelers.Types.ServiceProxies 5.5.8, proxy Operations may share a name. The earlier name lookup then reused the first `GetByIdQuery` for a second, unrelated `GetById` Operation, so its mapping was never created.
  - Fallback, kept for compatibility: when nothing invokes the Operation, an element with the would-be name is reused only if it invokes no proxy Operation, meaning one made by hand or before the association existed. That was the old behaviour for that case.
  - On a clash with an element invoking a different proxy Operation, the first free number is inserted before the suffix: `GetById2`, `GetById2Query`. The developer chose this over reusing or failing.
  - The lookup is still limited to the target folder (CQRS) or service (app service), as before. An element moved elsewhere is not found.
  - `_createService` still matches the Service by proxy name, because proxy names stay unique.

## Invariants & Constraints

- **The four Run Script Options embed a compiled copy of `DesignerMacros/src/common/proxyServiceHelper.ts`** (with `elementManager.ts`, `servicesHelper.ts` and the rest), compiled from the four entry points under `DesignerMacros/src/services-cqrs-crud/create-*-proxy-*` and `DesignerMacros/src/services-crud/create-appservice-proxy-*`. A fix to the helper has to be embedded in all four.
- **The embedded copies have drifted from source in `ElementManager.addChild`.** Source has the generic-type-parameter fix (commit 7eeb46f012, `setType(type.toModel())`); the embedded copies still have the older `typeId` / `isNullable` / `isCollection` form. In 1.1.8 only the helper's compiled diff was applied to each embedded copy, not a full recompile-and-paste, so that unrelated behaviour change was not shipped untested. A full recompile would bring it in: diff the compiled `_dist.js` against the embedded script before pasting.
