### Version 1.0.0

- New Feature: Adds `intent-modelers-integration`, documenting how a module wires against Intent's built-in Domain, Services, Eventing and User Interface designers — the four-identities rule and wrong-by-default traps, a full reference resource per designer, and a worked wiring recipe taken from `Intent.Modules.Metadata.RDBMS`.
- Improvement: Each designer resource now includes a complete model API reference (every public type and member), and the recipe covers the shared SDK types those models return, so agents no longer need to reflect over designer assemblies.
- Improvement: Extension modules (e.g. Domain Events, Services CQRS, UI Core) moved out of the base designer resources into their own `domain-extensions.md`, `services-extensions.md` and `user-interface-extensions.md`, each with install identities and model API reference, so an agent loads them only when it needs them.
