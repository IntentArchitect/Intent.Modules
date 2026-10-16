# Context: Intent.Modelers.Types.ServiceProxies

## Purpose

Defines the `Service Proxy` element type and its `Operation` / `Parameter` children, and adds them to the Services designer. A proxy maps to another application's HTTP-exposed endpoints and keeps its parameters in sync with them.

## Architectural Decisions

- **Two designer settings, with different jobs.** `Service Proxy Types` holds the element _definitions_. Its `.designer.settings` is installed with no `target` and `Extend Designers` empty, so it reaches the Services designer only through the Designer Reference from `Service Proxy Services Extension`. `Service Proxy Services Extension` is the one installed with `target="Services"`, and it extends the Services designer directly.
- **Context-menu entry points live in `Service Proxy Services Extension`** (5.5.7). The Services package option and the Diagram option are both defined there. The 5.5.4 package option was defined as a Package Extension in `Service Proxy Types` and never appeared in the Services designer. It was moved, not duplicated: keeping both would show the option twice if the referenced one ever did surface. Other modules that add Services-package menus (CQRS, Domain Events) follow the same pattern.
  - Exception: `Folder Extensions` still lives in `Service Proxy Types` and does work through the reference. It was left there to keep the change minimal. Put new menu options in `Service Proxy Services Extension` anyway.
- **Hidden "type registration" Element Creation Option.** Each package or folder extension that creates a Service Proxy with a script also carries a creation option whose visibility function returns `false`. Without it, Service Proxy is not an accepted child type of that container, and `createElement` fails.
- **Diagram creation targets the diagram's owner container** (`getCurrentDiagram().getOwner().getParent()`), not the folder of the visuals under the cursor. This matches the DomainInteractions CRUD diagram options.

## Invariants & Constraints

- The New Service Proxy creation script is duplicated inline on the Folder option, the Services package option, the Diagram option (with an extra layout step) and `Service Proxies Package`. A fix to one must be applied to all four. Moving it to a shared designer `Script` was deferred as out of scope for 5.5.7.
- `launchMappingDialog` is followed by deleting the proxy if nothing was mapped, so cancelling leaves nothing behind. The diagram variant then skips placing the visual.
