# Context: Intent.Metadata.WebApi

## Purpose

Designer metadata for describing services as HTTP endpoints: the `Http Settings`, `Parameter Settings`, `Api Version Settings` and related stereotypes on the Services and Service Proxies designers, plus the designer scripts that keep an endpoint's route in step with its parameters and validate it. It generates no code. Other modules (controllers, proxies, OpenAPI) read its stereotypes through the extension methods in `Api/`.

## Architectural Decisions

- **Route parameters are matched ignoring case, in every ASP.NET Core form.** ASP.NET Core binds `{CacheKey}` to a parameter named `cacheKey`, so the designer has to as well. Both `updateRouteParameters` and the route validation recognise `{name}`, `{name:constraint}`, `{name?}`, `{name=default}`, `{*name}` and `{**name}`, and compare names without regard to case. The update script also accepts the `toCamelCase` form of a name, because that is the form it appends, and `toCamelCase` drops spaces and underscores (`Cache Key` becomes `cacheKey`). Before 4.7.22 the script matched with exact-case `indexOf` on `{name}` and `{name:` only, so a `{CacheKey}` route got `/{cacheKey}` appended on every on-changed event, and the user had no way to undo it.
  - Rejected: stop appending missing route parameters and rely on validation instead. Appending is long-standing behaviour that users rely on when they switch a parameter to `From Route`, and validation already reports a route that still lacks the parameter.
- **A case-only rename leaves the route alone.** `getPreviousName()` keeps returning the old name until the element is renamed again, so the rename block re-runs on every later change. Without this check, a case-only rename would rewrite the user's `{CacheKey}` to `{cacheKey}`.

## Invariants & Constraints

- **The update script and the validation must agree on what a route parameter is.** `getRouteParameterNames` / `renameRouteParameter` in the update script, and the parameter extraction in `RestRoute`'s constructor in the validation, must recognise the same forms. If they disagree, the script leaves a route alone that validation then rejects, or appends a segment that validation would have accepted.
- **The two scripts are maintained in different places.**
  - The route update functions (`updateVersionParameter`, `updateRouteParameters` and their helpers) are an inline script on the `HTTP Route Update Scripts` Script element in this module's Module Builder designer. There is no TypeScript source for them.
  - The route validation is TypeScript in `DesignerMacros/src/services-rest-route-validation/` at the repository root. Compile it with `tsc -p <command|query|operation>/tsconfig.json`, then set the `_dist.js` output as the `Validate Function` of all three Element Extensions: `Command Extension` and `Query Extension` under `WebApi CQRS Extensions`, and `Operation Extension` under `WebApi Services Extensions`. Each model copy is expected to equal its compiled `_dist.js` exactly, line endings aside, so diff before replacing one.
- **Script element functions share a scope with the designer's other scripts.** The on-changed event handlers call `updateRouteParameters` from a different element, so a helper name added to the Script element can collide with a function defined by another script. Check new names against the other `.designer.settings` scripts in this repository and Intent.Modules.NET.
- **The Software Factory does not write `.imodspec`, the `.csproj` or `release-notes.md` here**, because all three are file-level ignored in this application. To move the version, set `Module Settings` → `Version` in the Module Builder designer, then edit `.imodspec` `<version>` and `.csproj` `<Version>` by hand. Release notes are hand-maintained.
