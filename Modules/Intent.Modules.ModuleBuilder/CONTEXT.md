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
