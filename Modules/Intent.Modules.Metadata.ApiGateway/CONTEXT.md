# Context: Intent.Metadata.ApiGateway

## Purpose
Designer metadata for API gateways: the `Api Gateway Route` element, its `Route Association` to a downstream
HTTP endpoint (Command, Query or Operation with `Http Settings`), and the menus that create routes in a
gateway's Services designer. Code generation lives in other modules (e.g. Intent.ApiGateway.Ocelot in
Intent.Modules.NET), which read these routes.

## Architectural Decisions
- **Manage Gateway Routes is a file-resource macro, not inline script** (1.0.0-beta.7) - the logic is
  TypeScript in `DesignerMacros/src/api-gateway/manage-gateway-routes/`, compiled into
  `resources/manage-gateway-routes.js`, registered as a Module Builder `Script` (File Resource) and called from a
  `Run Script Option` with `await manageGatewayRoutes(element);`. This follows the service-synchronizer and the
  WebApi "Expose as HTTP Endpoint" options. Rebuild the `.js` with `tsc -p` on that folder after changing the
  `.ts`; the `.js` is committed because the packager copies `resources/` into the imod as-is.
- **New routes are created under the element the menu was opened on.** The user picks where routes go by where
  they right-click. Rejected: placing them next to an existing route for the same package (the spike's
  behaviour), which is invisible to the user and surprising when a package has routes in several folders.
- **Changes are confirmed on Apply, not shown live.** A `tree-view` form field raises no change event, so the
  form cannot show a pending add/remove summary while the user ticks. The confirm dialog lists what will be added
  and removed, and warns that hand edits to a removed route in `ocelot.json` are lost.
- **Route naming, verb, route and body come from the `Route Association` event scripts**, not from the macro.
  The macro only creates the element and the association, so the menu and "New Api Gateway Route" produce the
  same route.

## Invariants
- The `.imodspec` is `ignored` in this application's output config, so the Software Factory never rewrites it.
  Its `<version>` (and the `.csproj` `<Version>`) are raised by hand to match `Module Settings` -> `Version`.
