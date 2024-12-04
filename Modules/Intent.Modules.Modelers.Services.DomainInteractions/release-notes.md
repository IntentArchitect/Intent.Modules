### Version 2.1.0

- Improvement: Help topics added.

### Version 2.0.3

- Improvement: The full path of the class is now displayed on the `Create CRUD Service` and `Create CRUD CQRS Operations` class selection dropdown.

### Version 2.0.2

- Improvement: Changed `Default Mapping Mode` to be `Advanced` by default.
- Improvement: Added setting `Default Query Implementation`, allowing for configuration of how `Query Entity Actions` are implemented.
- Improvement: Create CRUD Scripts will inform the user when it is hindered by the Private Setters setting and the absence of Entity constructors to produce all the needed service operations.
- Fixed: Create CRUD Scripts were breaking on `Ensure Private Property Setters`.
- Fixed: On Create and Update Mappings the fields marked as `set-by-infrastructure` will not be mappable since they will be set by Infrastructure.

### Version 2.0.1

- Fixed: `Data Contracts` showing all `Attributes` as required on mapping.
- Fixed: CQRS CRUD script incorrectly mapping managed Foreign Key `Attributes` (i.e. where `Attribute` is part of compositional one-to-many relationship).

### Version 2.0.0

- Improvement: Advanced mapping configurations updated to use new trait system introduced in Intent Architect 4.3 to allowing for support of additional mapping scenarios.

### Version 1.1.6

- Fixed: Advanced mapping created on Commands/Queries' Fields to Class Attributes that are nullable.
- Fixed: Advanced mapping created on Standard Services DTO Fields to Class Attributes that are nullable.
- Fixed: Standard Service DTO mapping on Domain Associations that are Value Objects.

### Version 1.1.5

- Improvement: CQRS CRUD script now supports surrogate Primary keys marked as `User supplied`.

### Version 1.1.4

- Improvement: Added ability to drag Repositories onto the application service designer you can perform Call Operations on them.

### Version 1.1.3

- Improvement: Improved mapping support around `Data Contracts` and `Value Objects`.
- Improvement: Double-clicking on fields on the right hand side will add properties on Commands and Queries on the left hand side inside the Advanced Mapping Screen for Service Operation Mappings.

### Version 1.1.2

- Improvement: Aligned some of the CRUD Generation functionality, to make traditional service modeling has the same features as the CQRS paradigm.

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
