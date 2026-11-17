# CONTEXT.md — Intent.Modelers.Services.DomainInteractions

## Purpose

Intention, plus tools that maintain existing intentions. This module owns the Services-designer vocabulary for service-to-domain interactions: the Create, Update, Delete and Query Entity Action associations, their mapping settings, and Processing Action, along with the tools that operate on an Entity Action mapping that already exists (Sync with Entity, Create new result DTO, Convert to advanced mapping system). Anything that starts from a domain element and generates services from it belongs in `Intent.Application.DomainInteractions` (Intent.Modules.NET); use that test when deciding where a new accelerator goes.

## Architectural Decisions

- **CRUD accelerators moved to `Intent.Application.DomainInteractions` (v2.5.0)**: the `Create CRUD CQRS Operations` and `Create CRUD Traditional Service` options (basic and advanced), the Domain-designer CRUD suggestions, the repository-operation suggestions, the `Crud Api` and `Repository Operation Api` scripts and their documentation topics. They generate intentions rather than describe them, and every .NET CRUD module already depends on Application.DomainInteractions. Kept here: Sync with Entity, Create new result DTO, Convert to advanced mapping system, and all three module settings.
- **Accepted cost**: `Intent.Java.Services.CRUD` and `Intent.NodeJS.Services.CRUD` depend only on this module, so from 2.5.0 they have no CRUD accelerators.

## Invariants & Constraints

- **`Default Mapping Mode` stays here, and other modules read it by id**: setting group `0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad`, field `14bc1e98-930b-494c-97a9-5da4e1f3a5fa`. The moved accelerators' visibility functions read it, as do the designer settings of Modelers.Services and Modelers.Services.CQRS. Renaming, moving or re-identifying it breaks them silently.
- **The `Crud Api` / `Repository Operation Api` TypeScript sources stay in `DesignerMacros/src`**, because other bundles share them. Each bundle's `tsconfig.json` writes a local `_dist.js`, and only its `tsconfig-save-to-module.json` writes into `../Intent.Modules.NET/Modules/Intent.Modules.Application.DomainInteractions/resources/` (resolves only when the two repos are siblings). CI's `compile-all.ps1` compiles every `tsconfig.json`, so a `tsconfig.json` must never point outside this repo.
- **Neither moved `.js` is recompiled by CI any more.** Intent.Modules.NET has no DesignerMacros build, so a TS fix to either bundle ships only once someone compiles `tsconfig-save-to-module.json` and commits the output there. At the move, `crud-api.js` was copied as committed (CI never recompiled it), and `repository-operation-api.js` was compiled from current sources, because CI used to recompile it on every build and that compiled output (with the recursion guard) is what 2.4.x shipped. `crud-api.js` therefore still lags its TS sources (it lacks the commit `0304849817` recursion guard); the next compile of it is a behaviour change that needs its own release note.

## Module Interactions

- **Intent.Application.DomainInteractions** — owns the CRUD accelerators from its 1.3.0, and depends on this module at 2.5.0. The two must be released together. This module's interoperability `detect` installs Application.DomainInteractions 1.3.0 when it is present, so an old copy can't leave a consumer with no accelerators. In the other direction, Application.DomainInteractions' raised dependency stops every menu item appearing twice.

## AI Context on `Create Entity Action` / `Update Entity Action` (v2.4.8)

Observed AI-modeling mistakes on Create/Update Entity advanced mappings:

- Fields get Data-Mapped but the `Invocation Mapping` (constructor call) is left unwired, so the target stays required-but-unmapped.
- `mapExpression` used for string interpolation, producing invalid C#.

`AI Settings.Rules` on both associations now calls these out directly. Keep additions here terse — bullet the symptom and the fix, no elaboration.

Default policy: wire only the minimum needed to satisfy designer validation (the `Invocation Mapping` mapped end), then `// IntentIgnore` the generated method body and hand-write the field logic in code — not "leave it fully unmapped" (that fails validation). Full field mapping via the designer only when the user explicitly asks for it.
