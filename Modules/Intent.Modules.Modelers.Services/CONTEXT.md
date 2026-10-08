# Context: Intent.Modelers.Services

## Purpose
Defines the Services designer: Services, Operations, Parameters, DTOs and DTO-Fields, plus the
Service Settings group whose naming conventions rename those elements as users create and edit them.

## Architectural Decisions
- **Naming conventions go through one shared script.** Every naming handler calls
  `applyNamingConvention(element, "<field title>")` from the `Naming Convention Scripts` Script element
  under the Services Types designer settings, declared as the handler's dependency. Rejected: the previous
  per-handler copies that looked up the settings group and field by raw GUID. They were unreadable, and
  they drifted: Service had picked up the Operation field by copy-paste.
- **Fields are looked up by title; the settings group by id.** The group id is held as a named constant
  inside `applyNamingConvention`. Rejected: looking the group up by its title ("Service Settings"),
  because group titles are not guaranteed unique across installed modules.
- **Parameters name themselves.** The Parameter element has its own On Created and On Name Changed
  handlers using `Parameter Naming Convention`, the same pattern as the Domain designer's Attribute.
  Rejected: the former Operation On Changed handler. It camel-cased every child Parameter on any
  Operation change and ignored settings. It was added incidentally in 2022 with no recorded reason.
- **`Parameter Naming Convention` defaults to Camel Case.** This was a user decision, to preserve the
  behaviour apps had before the setting existed. The other naming fields default to Manual.
- **Services follow `Entity Naming Convention`.** This was a user decision; previously they followed
  `Operation Naming Convention`. "Entity" is a legacy title carried over from the Domain designer. It
  is kept, with only the hint reworded, because renaming the title would rename the generated
  `EntityNamingConvention()` C# accessor that downstream code may call.
- **Operation parameter and return types target the `[Service Argument]` trait.** Other modules opt their
  elements in by implementing the trait, so Services lists no foreign element types. Rejected: adding
  each element type to the target types directly, because that would need Services to depend on every
  module that owns one.
- **Services keeps its `Intent.Common` dependency and client floor where they were.** It refers to the
  trait by id only in designer settings, with no code reference. With an older `Intent.Common`, nothing
  implements the trait and no extra types are offered, but nothing fails. Rejected: raising the dependency
  to 3.11.8, because that would raise Services' minimum client to 5.0 for no functional gain.

## Invariants & Constraints
- Renaming a Service Settings naming field's **title** requires updating every
  `applyNamingConvention(..., "<title>")` call in this module. Otherwise the lookup returns nothing and
  naming silently stops being applied.
- Service Settings **field ids** are a cross-module contract (see Module Interactions). Never delete and
  recreate a field.
- `OnInstallMigration` and `Migration_03_09_03_Pre_00` seed only the Property, Entity and Operation
  fields. `Parameter Naming Convention` is not seeded and relies on its Default Value.

## Module Interactions
- **Intent.Modelers.Services.CQRS** and **Intent.Metadata.WebApi**: their Command/Query
  on-name-changed handlers read `Entity Naming Convention` by hardcoded field id
  (`625c6211-0dc7-4190-af49-6eadb82c7015`). They do not use `Naming Convention Scripts`.
- **Intent.Common** owns the `[Service Argument]` trait (`8b78644d-8b44-48c7-8c13-68255e2252c0`,
  `IServiceArgumentModel`). **Intent.Modelers.Domain** (`Class`, `Data Contract`) and
  **Intent.Modelers.Domain.ValueObjects** (`Value Object`) implement it. The trait id is a cross-module
  contract: never delete and recreate the trait.
