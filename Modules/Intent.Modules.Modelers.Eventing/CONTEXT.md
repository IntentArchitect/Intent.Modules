# Context: Intent.Modelers.Eventing

## Purpose

Defines the `Core Eventing Types` designer settings that extend the Services designer with message-based integration modeling: the `Eventing Package`, `Message` (Integration Event), `Integration Command` and `Eventing DTO` element types, plus their publish/subscribe associations. As of `6.0.5` it also carries the AI Context (`AI Settings.Rules`) that steers an AI _modeling_ agent's decisions about those elements.

## Architectural Decisions

- **AI Context lives on the designer/element settings, not only in a bundled skill.** `Intent.Eventing.Contracts` already ships a coding-agent skill (`MessageBasedIntegrationSkillTemplate` → `.agents/skills/message-based-integration-modeling/SKILL.md`) that states the same ownership and reuse rules. That channel reaches the wrong audience: a coding agent reads it from a consumer's generated output, _after_ the modeling decisions are already made. The agent that actually decides _where_ to create a `Message` or `Integration Command` is a modeling agent reading the model through `get_designer_schema` / `get_designer_element_details`, which surfaces `AI Settings.Rules` and never sees a consumer's `.agents` folder. So the guidance was added to the model itself, following the convention already used by `Intent.Modelers.Domain`'s `Domain Types` and `Intent.Modelers.Services.CQRS`' `Query`.
- **Both channels kept, deliberately.** The `Intent.Eventing.Contracts` skill was explicitly _not_ removed or migrated. The two serve different agents at different points in the lifecycle; `Intent.Eventing.Contracts` was left untouched (no version bump, no doc change) as part of this work.
- **Split by scope, not kept as one document.** Cross-cutting workflow (search the whole solution first, Package Reference over duplication, publish/subscribe wiring order, anti-patterns) sits on the `Core Eventing Types` **Designer Settings**. Narrow ownership rules sit on the **element types** they govern, so they appear exactly when an agent is about to create that kind of element: `Message` → owned by the publisher; `Integration Command` → owned by the _receiver_, which is the opposite of what the "Publish Integration Command" quick-create option does and therefore marked CRITICAL.
- **No `On Loaded` script on `Eventing Package` (removed in `6.0.6`).** The script (source was `DesignerMacros/src/eventing-designer-on-load`) existed to place the old Eventing designer's `Application` element on its diagram. That element type went with the Eventing designer in `6.0.0`, so the lookup never matched and every run just stamped `initialized-version=1` metadata on each package and expanded it. Because it looped over `getPackages()` rather than the package that loaded, and `Eventing Package` now lives in the Services designer, it dirtied every Services-designer package that lacked the key — including any created later. Narrowing it to Eventing packages was rejected: with no `Application` element left there is nothing for it to do. Nothing reads `initialized-version`; the copies already saved on consumer packages, and the one seeded by the `Intent.Modules.Modelers.Eventing.Metadata` sample harness, are inert and were left in place.

## Invariants & Constraints

- **`Package Settings` has no `Rules` in the generated designer settings.** `PackageSettingsPersistable` (in `Intent.Modules.ModuleBuilder/Model/Settings/`) declares no `Rules` property — unlike `DesignerSettingsPersistable`, `ElementSettingPersistable`, `AssociationSettingsPersistable` and the extension persistables. An `AI Settings` stereotype _can_ be applied to a `Package Settings` element (the stereotype definition allows it) but it is never serialized into `*.designer.settings`, so it never reaches a consuming agent. The `Eventing Package` naming/sharing guidance is therefore ALSO written into the `Core Eventing Types` designer-level rules, where it is actually emitted. The stereotype on `Eventing Package` itself is retained as documentation of intent and will start working for free if the persistable ever gains `Rules`.
- Element-level `<rules>` serializes as the **last** child of its `<elementSetting>`; the designer-level `<rules>` serializes immediately after `<name>`. This follows `XmlSerializer` property declaration order in the persistables — relevant if the generated file is ever inspected or diffed by hand.

## Module Interactions

- **Intent.Eventing.Contracts** — ships the parallel coding-agent skill; intentionally unchanged (see above).
- **Intent.Modelers.Services / .EventInteractions** — host designer and interaction wiring the rules refer to (Publish/Subscribe Integration Event/Command, Integration Event Handler).

## Superseded

- **"`modelers/Core Eventing Types.designer.settings` is intentionally left stale; a Software Factory run is still owed."** The `6.0.5` AI Settings work was applied straight to the Module Builder model without a Software Factory run. That run happened during the `6.0.6` work: the generated file differed only by the removed `On Loaded` macro, so the AI rules were already in it and nothing is owed.
