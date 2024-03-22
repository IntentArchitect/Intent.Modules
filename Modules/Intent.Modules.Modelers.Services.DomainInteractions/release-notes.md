### Version 1.0.4

- Fixed: Removed `Create CRUD CQRS Operations` script from menu if commands and queries are already present in a folder.
- Improvement: Added support for Service Operations to also work against Stored Procedures.
- Improvement: Added hueristic check on operation names to identify Query operations, and not generate Commands for them.

### Version 1.0.3

- Improvement: Removed support for implicit keys, allowing keyless entities to now be modelled.

### Version 1.0.2

- Improvement: CRUD Dialog no longer shows non-aggregate entities without primary keys.
- Fixed: Fixed an issue around generating CQRS CRUD `Command`s and `Query`s, failing under certain circumstances related to custom named `Primary Keys`.

### Version 1.0.0

- New Feature: Services modeler extensions for describing interactions between an application's services and its domain.
