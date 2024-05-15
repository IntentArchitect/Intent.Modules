### Version 1.1.2

- Improvement: Aligned some of the CRUD Generation functionality, to make traditional service modeling has the same features as the CQRS paradigm .

### Version 1.1.1

- Improvement: Updated Crud scripts to not add implicit primary keys.

### Version 1.1.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 1.0.5

- Improvement: CRUD scripts improved filtering on which Entities can be generated i.e. filter out abstract classes and compositional inheritance.

### Version 1.0.4

- Improvement: Added support for Service Operations to also work against Stored Procedures.
- Improvement: Added heuristic check on operation names to identify Query operations, and not generate Commands for them.
- Improvement: Model input on Classes for Service Operation parameters of type Class.
- Fixed: Removed `Create CRUD CQRS Operations` script from menu if commands and queries are already present in a folder.

### Version 1.0.3

- Improvement: Removed support for implicit keys, allowing keyless entities to now be modelled.

### Version 1.0.2

- Improvement: CRUD Dialog no longer shows non-aggregate entities without primary keys.
- Fixed: Fixed an issue around generating CQRS CRUD `Command`s and `Query`s, failing under certain circumstances related to custom named `Primary Keys`.

### Version 1.0.0

- New Feature: Services modeler extensions for describing interactions between an application's services and its domain.
