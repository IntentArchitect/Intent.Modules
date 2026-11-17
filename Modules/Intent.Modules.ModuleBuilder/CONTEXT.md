# Intent.ModuleBuilder — Context

## Template review importance (formerly "Severity")

- **3.20.0 renamed `Template Settings` → `Severity` to `Review Importance`.** This applies to both `Template
  Settings` stereotypes: the one on File/C# Templates and the one on Static Content Templates. Property ids are
  unchanged, so existing values carry over.
- **Which `.imodspec` tag is written depends on the module's minimum client version** (the user's decision).
  If the `supportedClientVersions` lower bound is at or above `5.4.0-beta.6`, the first client that reads
  `<reviewImportance>`, it writes `<reviewImportance>`. Otherwise it writes `<severity>`, because older clients
  only read that. An absent or unparseable range counts as allowing older clients. Regeneration removes
  whichever of the two tags isn't chosen. The client reads `<reviewImportance>` and falls back to
  `<severity>`.
- **`GetSeverity()` stays as an obsolete shim on purpose.** `Intent.ModuleBuilder.CSharp` and possibly modules
  in other repos compile against the published `Intent.Modules.ModuleBuilder` NuGet package and implement
  `IModuleBuilderTemplate.GetSeverity()`. The new `GetReviewImportance()` is a default interface method that
  falls back to `GetSeverity()`, so those builds keep feeding the value through without recompiling. Remove the
  shims only once every implementer has moved to `GetReviewImportance()`.
- **`Api/Extensions/TemplateRegistrationExtensions.cs` is hand-maintained.** It is not SF-managed: its
  `ModuleBuilder.Templates.Api.ApiModelExtensions` template id no longer generates. Renaming a `Template
  Settings` property therefore needs a manual edit there too.

## Element visual anchors (Anchor Segments / Anchor Spread)

- **A point is a segment `[p, p]`, so 3.21.0 adds `Anchor Segments` and deprecates `Anchor Points`** (the user's
  decision, so that older clients keep their anchors; `supportedClientVersions` was deliberately not raised).
  `Anchor Points` is shown only while it has a value (its Is Active Function is
  `return !!properties["Anchor Points"].value;`) and is still written as `anchorPointsFunction`. It must not be
  made always-inactive: the designer doesn't persist an inactive property's value and code generation ignores
  it, and a property definition has no separate hidden flag, only Is Active and Is Required functions. The client
  uses segments over points when both are set, so writing both is safe.
- **The deprecated `Anchor Points` has a new property id.** The original was removed before it was restored,
  and the designer can't recreate a property with a given id. The version migration `3.21.0-pre.0` re-keys
  consumers' values from the original id onto the new one, and copies them into `Anchor Segments` as
  `points.map(p => [p, p])`. The client reads a body that doesn't start with `return`, `const`, `let`, `var`,
  `if` or `function` as a template literal, so such a body never worked: the migration keeps it as is and
  logs it, without copying it into segments.
- **`Anchor Spread` persists only as `<anchorSpread>even</anchorSpread>`; `Off` emits nothing.** The client accepts
  only `even`.
- **The migration uses `Intent.Architect.Persistence` 3.6.0 types, which the module gets transitively.** There a
  stereotype property's definition id is `StereotypePropertyPersistable.Id`; 3.7.x and later use `DefinitionId`.
- **The designer scripting API's `setControlType` takes `"function"`, not `"javascript-function"`**, although
  the typings and the getter use the latter.
