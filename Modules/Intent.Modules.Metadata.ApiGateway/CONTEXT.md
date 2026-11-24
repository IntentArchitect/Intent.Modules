# Context: Intent.Metadata.ApiGateway

## Purpose
Designer metadata for API gateways: the `Api Gateway Route` element, its `Route Association` to a downstream
HTTP endpoint (Command, Query or Operation with `Http Settings`), and the menus that create routes in a
gateway's Services designer. Code generation lives in other modules (e.g. Intent.ApiGateway.Ocelot in
Intent.Modules.NET), which read these routes.

## Architectural Decisions
- **Manage Gateway Routes is a file-resource macro, not inline script** (1.0.0-pre.0) - the logic is
  TypeScript in `DesignerMacros/src/api-gateway/manage-gateway-routes/`, compiled into
  `resources/manage-gateway-routes.js`, registered as a Module Builder `Script` (File Resource) and called from a
  `Run Script Option` with `await manageGatewayRoutes(element);`. This follows the service-synchronizer and the
  WebApi "Expose as HTTP Endpoint" options. Rebuild the `.js` with `tsc -p` on that folder after changing the
  `.ts`; the `.js` is committed because the packager copies `resources/` into the imod as-is.
- **New routes are created under the element the menu was opened on.** The user picks where routes go by where
  they right-click. Rejected: placing them next to an existing route for the same package (the spike's
  behaviour), which is invisible to the user and surprising when a package has routes in several folders.
- **Submitting the form is the confirmation; no dialog before or after.** The changes are made at once, the designer
  shows them, and Ctrl+Z undoes them. The form's help text warns that removing a route drops its hand edits in
  `ocelot.json`. Rejected (1.0.0-pre.0): a confirm dialog after submitting that listed the adds and removes, and an
  info box afterwards counting them. The user found both redundant clicks, since the ticks already say what changes,
  the tree shows the result and undo is one keystroke. A `tree-view` field raises no change event, so the form itself
  cannot show a live summary.
- **The sync checkbox appears only when a route has drifted.** It updates routes whose verb or route no longer matches
  their endpoint, and starts ticked. With nothing drifted it is left off the form, because a disabled "(0)" checkbox
  only raised the question of what it was.
- **The submit button reads "Done", not "Apply", for now.** The form sets `submitButtonText: "Apply"`, but the
  nightly Intent client ignores it (`config.submitButtonText ?? index == last ? "Done" : "Next"` parses as
  `(config.submitButtonText ?? index == last) ? ...`, in `DynamicFormDialogViewModel`). The setting is kept, so the
  button reads Apply once Intent fixes that; help text and docs say "submit the form" rather than naming the button.
- **Endpoints directly in a package sit at the package's top level in the tree, after its folders.** Only an
  endpoint whose parent is a folder or service is grouped under that parent; for one at the package root,
  `getParent()` is the package, which has no `getIcon()`.
- **"Cannot read properties of null" from `createElement` on Apply means the designer reloaded under the open
  form.** Intent disposes a designer's element registry when it reloads it, for example when a module install or
  restore finishes. The form's macro still holds the old designer, so its first `createElement` fails. The macro
  cannot recover from that; close the form and run the option again.
- **The form shows each route's security and rate limit as read-only tags, not as columns** (1.0.0-pre.0). The tags
  ([SECURED] / [UNSECURED], [RATE LIMITED]) are coloured text after the row, the same idiom as the Synchronize with
  Domain Entity tree in Intent.Modelers.Services.DomainInteractions. Rejected: a Secured and a Rate limit checkbox column
  per row, which the client asked about. The Dynamic Form API has no grid or table field, and a tree-view row has one
  checkbox. Editable columns need a new field type from the Intent team. Until then you change both on the route in the
  designer.
- **The tags read stereotypes this module doesn't own, by definition id, without a dependency.** Secured
  (`a9eade71-...`) and Unsecured (`8b65c29e-...`) come from Intent.Metadata.Security, and Rate Limit (`fee00939-...`)
  from Intent.ApiGateway.Ocelot. Without those modules no route carries them, so every route shows [SECURED]. That
  matches what Ocelot generates, since routes are secured by default. The security rule must stay the same as Ocelot's
  `IsUnsecured`: the closest Secured or Unsecured on the route, its folders and then its package decides, and Secured
  wins where one element has both. A change to either side means changing the other.
- **Route naming, verb, route and body come from the `Route Association` event scripts**, not from the macro.
  The macro only creates the element and the association, so the menu and "New Api Gateway Route" produce the
  same route.

- **A non-HTTP route target is flagged, not prevented** (1.0.0-pre.0). The Route Association's Downstream End has a
  Validate Function that errors when the target has no `Http Settings`. Rejected: narrowing the end's Target Types,
  because types cannot filter on a stereotype (Target Traits match element types, not applied stereotypes), and an
  endpoint can lose `Http Settings` after the route exists, which only validation catches. Generators must still
  guard against it (Intent.ApiGateway.Ocelot raises an `ElementException`), since a model can be saved with the error.
- **`getMethodAndRoute` returns `result: "NotHttp"` instead of throwing** when the target has no `Http Settings`. It is
  compiled into the Display Route, Display Verb and Route Association On Changed scripts, so a change to
  `DesignerMacros/src/api-gateway/_common/` means recompiling and re-pasting all three (`tsc -p` per folder; the
  display scripts need their last `return` line uncommented). On Changed then clears the route's `Http Settings` and
  Body, as On Deleted does.

## Invariants
- The `.imodspec` is `ignored` in this application's output config, so the Software Factory never rewrites it.
  Its `<version>` (and the `.csproj` `<Version>`) are raised by hand to match `Module Settings` -> `Version`.
