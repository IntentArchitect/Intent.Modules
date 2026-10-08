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
- The duplicate-operation check is compiled TypeScript. Its source is
  `DesignerMacros/src/services-operation-validation`, with shared logic in `common/`. Build it with
  `tsc -p tsconfig.json` in the `operation/` and `operation-parameter/` folders. Each `dist/dist.js` is
  pasted into the **Validate Function** of the Operation and Parameter element settings respectively.
  In the pasted copy, comment out the `validate…(element);` line and uncomment `return validate…(lookup(id));`.
  Fix the source first: a change made only in the designer is lost on the next recompile.
- The duplicate-operation signature follows C# overload rules. It counts each parameter's type name,
  generic type arguments (recursively) and collection flag, but never the return type. Nullability counts
  only for a non-collection value type (an Enum, or a Type-Definition named in `isValueType`). That list
  mirrors `CSharpType.NonNullableValueTypes` in Intent.Modules.Common.CSharp and must be kept in step with it.
  Rejected: counting nullability on every type. `string`/`string?` and DTO/DTO? are not distinct C# overloads,
  so the check would pass overloads that fail to compile. A nullable collection stays nullable-insensitive
  because the type resolver applies nullability to the collection itself (`List<int>?`), not to its elements.
- Three hand-pasted copies of the compiled `calculateSignature` / `calculateTypeSignature` / `isValueType`
  block also exist. They are in the Operation Validate Function of Intent.Modelers.Domain and of
  Intent.Common.Types, and in the Component Operation Validate Function of Intent.Modelers.UI. Their
  `findPeerOperations` differs: UI looks up `Component Operation`. When the signature logic changes,
  re-paste the block into all three and bump each module. Nothing enforces this, so the copies drift silently.

## Module Interactions
- **Intent.Modelers.Services.CQRS** and **Intent.Metadata.WebApi**: their Command/Query
  on-name-changed handlers read `Entity Naming Convention` by hardcoded field id
  (`625c6211-0dc7-4190-af49-6eadb82c7015`). They do not use `Naming Convention Scripts`.
- **Intent.Common** owns the `[Service Argument]` trait (`8b78644d-8b44-48c7-8c13-68255e2252c0`,
  `IServiceArgumentModel`). **Intent.Modelers.Domain** (`Class`, `Data Contract`) and
  **Intent.Modelers.Domain.ValueObjects** (`Value Object`) implement it. The trait id is a cross-module
  contract: never delete and recreate the trait.
