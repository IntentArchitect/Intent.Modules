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
- **Diagrams keep the name they are created with, like every other element.** `New Diagram` is a Run
  Script Option that names the Diagram after its parent and opens it for renaming. The original Element
  Creation Option is kept but hidden, so Diagram stays a creatable child type for scripts. This matches
  Intent.Modelers.Domain: see its `CONTEXT.md` for the reasoning and invariants, and change all three
  modules together.
- **The creation-time mapping dialog is driven from `Invocation Mapping`'s Is Required Function.** The 5.x client
  (checked 5.0 to 5.4) evaluates a mapping setting's Is Required Function in one place only: about 100 ms after a user
  creates the association from a context menu. When it returns true, the client opens that mapping's dialog without
  waiting on it. Scripts, AI agents and a later `Map Call Operation` never reach it. The function:
  - creates the mapping and its invocation end, so the dialog opens pre-filled;
  - opens the dialog itself;
  - takes the mapping straight back out of the model, since the dialog has already taken its snapshot (Done saves the
    dialog's result as the mapping, and Cancel leaves the invocation unmapped rather than half-mapped with a warning);
  - returns false, so the client opens no dialog of its own.

  This function is the only place the mapping is created. `Perform Invocation` has no On Created handler, so an
  invocation created by a script or an AI agent is unmapped (a grey line, no warning), and no dialog opens for it.
  Helpers that need a mapped invocation (the CQRS proxy and repository helpers in `DesignerMacros`) call
  `createAdvancedMapping` and add the invocation end themselves.
  Rejected: keeping On Created's mapping pre-creation. It ran for every creation, so every AI- or script-created
  invocation got a mapping the agent never asked for, and the hook needed a "put it back if no dialog was shown"
  fallback.
  Rejected: On Created owning the dialog. It cannot tell a user from a script. The dialog is blocked for AI agents, so
  the handler deleted the mapping and restored only the bare pre-created one, losing an AI script's own mappings. Bulk
  scripts also needed an opt-out tag across ProxyInteractions and DomainInteractions; that was tried and reverted.
  Rejected: deleting the mapping on Cancel while it stayed in the model. When the function returns false, the client
  calls `goToError()` on a new association that has warnings. On an association end, that runs the first error's
  click action, which opens the dialog for the half-mapped mapping. Angular Material's show cancels an open dialog, so
  ours flashed and closed. Rejected: waiting for a client fix, which the user decided against for now.

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
- `Invocation Mapping`'s Is Required Function must take the mapping out of the model before it returns, and must
  return false. A half-mapped mapping left in the model lets the client's `goToError()` open a second dialog that
  replaces the first. Returning true opens a second dialog too.
- Is Required Function does not mean "required" here. A future client that evaluates it anywhere else (validation,
  the mapping dialog) would open dialogs at those points. Re-check the client's call sites whenever the supported
  client range moves, and move this logic to a proper creation-time hook if the client gains one.
- The `association` an Is Required Function receives is not the one an On Created macro receives. The function gets
  the association's raw API context, which has no `getOtherEnd()` (checked on 5.4). Calling it threw, and the client
  re-throws any error from this function, so no dialog opened at all. The function therefore maps the source root to
  the target root with empty paths (`addMappedEnd(id, [], [])`) and uses only `createAdvancedMapping`,
  `launchAdvancedMappingDialog` and `getAdvancedMapping`. If pre-filling fails, it still opens the dialog empty.
  A failure here is visible only in the client log, `%TEMP%\IntentArchitect\logs\electron\electron-*.log`
  ("Failed to evaluate:").
- The function must copy `association` (and `console`) into locals before calling anything. Every designer function
  runs in one shared sandbox whose globals are deleted and replaced on each evaluation. Creating the mapping refreshes
  errors synchronously, which evaluates other designer functions, so afterwards the global `association` is gone
  ("ReferenceError: association is not defined"). The dialog never opened and the mapping stayed in the model.
- Do not reintroduce an On Created mapping pre-creation on `Perform Invocation`: AI and script creations must stay
  unmapped.
- The function uses `delete?.()`. A client without mapping `delete()` keeps the mapping in the model while the dialog
  is open, so Cancel leaves it half-mapped instead of failing.

## Module Interactions
- **Intent.Modelers.Services.CQRS** and **Intent.Metadata.WebApi**: their Command/Query
  on-name-changed handlers read `Entity Naming Convention` by hardcoded field id
  (`625c6211-0dc7-4190-af49-6eadb82c7015`). They do not use `Naming Convention Scripts`.
- **Intent.Common** owns the `[Service Argument]` trait (`8b78644d-8b44-48c7-8c13-68255e2252c0`,
  `IServiceArgumentModel`). **Intent.Modelers.Domain** (`Class`, `Data Contract`) and
  **Intent.Modelers.Domain.ValueObjects** (`Value Object`) implement it. The trait id is a cross-module
  contract: never delete and recreate the trait.
- **Intent.Application.Dtos** owns the DTO "default value order" warning (a field without a default value after one
  with a default value). Until 4.0.16 this module's DTO Validate Function held it, reading Dtos' DTO Settings by
  GUID. It moved because the DTO template is what drops out-of-order defaults, and the check fired in applications
  without that module. Dtos adds it as a DTO Element Extension. This module's `<interoperability>` entry updates
  Dtos to 4.6.2 (written against `4.6.2-pre.0`), and Dtos' dependency floor on this module is 4.0.17. Together they
  stop an application running both copies (a duplicate warning) or neither. Don't re-add a Validate Function for
  this to the DTO element settings.
