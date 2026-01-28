### Version 2.4.1

- Fixed: Mapping system not correctly handling primary keys when working with inheritance.

### Version 2.4.0

- Feature: Added suggestion to synchronize Commands/Queries/Service Operations with Domain Entity Attributes.

> ?? NOTE
> Requires Intent Architect version 4.6 or higher.

### Version 2.3.14

- Fixed: Modified createQueryMappingEnds to use EntityProjector's pre-established field mappings instead of name-based lookups, eliminating case-sensitivity issues.

### Version 2.3.13

- Improvement: Documented relevant rules for AI-implemented Service model changes.
- Fixed: Updated `Intent.Persistence.SDK` to latest version to resolve compatibility issues.

### Version 2.3.12

- Improvement: Introduced new `Aggregate Association Ends` to Create and Update Entity Action mappings in order to map to their IDs directly which helps with entity creation and updates that involve aggregate relationships.

### Version 2.3.11

- Fixed: Circular reference bug in CRUD scripts, around bi-directional 1-1 relationships.

### Version 2.3.10

- Improvement: Can map the aggregate when publishing Integration Events for domain Operations.

### Version 2.3.9

- Improvement: Updated module documentation to use centralized documentation site.
- Improvement: Mapping a `Create Action` on a static `Operation` now also works when return a type other than the containing entity, as long as that return type has a generic type parameter matching the containing entity. 
- Improvement: Added suggestions for creating new result DTO contracts.

### Version 2.3.8

- Improvement: Add CQRS Operations and Traditional Service Operations from Domain Entity Operations.
- Fixed: Creating Update Operations for Composite 1 -> 1 associations will no longer attempt to map `Id` attributes.

### Version 2.3.7

- Improvement: It is now possible to use the result of an invocation as a data source on `Create` and `Update` mapping screens.
- Fixed: Create CRUD Operations for Services and CQRS didn't generate elements for Domain Entity Operations.
- Fixed: Update Entity associations not querying by surrogate key by default when connected to Domain Operations.

### Version 2.3.6

- Fixed: CRUD for domain operations now visually points to the operations.

### Version 2.3.5

- Improvement: Added suggestions to the Domain Designer to repository operations for creating CQRS requests and Traditional Services in the Services Designer.

### Version 2.3.4

- Improvement: Processing Actions that are executed after the current Processing Action will not show in the Mapping Dialog.
- Improvement: Added syntax highlighting to Processing Actions.
- Improvement: Added Constructor and Static Operation icons in the diagrams for Domain entities.
- Fixed: Syntax highlighting not showing for Operations and Constructors on Domain entities.

### Version 2.3.3

- Fixed: Adjusted the creation of association from "Call Service Operation" to the new "Perform Invocation".

### Version 2.3.2

- Improvement: Processing Actions now visually indicate that they aren't mapped by being slightly greyed out.
- Improvement: Query Entity Action will now automatically indicate as a collection if the return type of the Processing Handler is a collection.
- Fixed: Processing Actions incorrectly always naming variables `entity` and not adjusting automatically.
- Fixed: Incorrect bidirectionality on Create, Update and Delete Entity Actions.

### Version 2.3.1

- Improvement: Domain CRUD service suggestions will now prompt the user if they have unsaved changes before proceeding.
- Improvement: CRUD accelerators now offer the user a way to change the diagram into which they want to add their services.

### Version 2.3.0

- Improvement: Added CRUD accelerators to diagrams in the Services designer.
- Improvement: Added CRUD accelerators as suggestions on Classes in the Domain designer.

### Version 2.2.1

- Fixed: Re-running CQRS Crud Script caused an issue on `GetAll` queries.

### Version 2.2.0

- Improvement: Updated mapping configurations to support generalized `Invoke Service` interaction system.
- Improvement: Grouped context menu options for clearer separation of features and usability.
- Improvement: CRUD accelerator improvements, including better validation and error messaging, improved support for compositional crud.

### Version 2.1.5

- Improvement: Accelerators added to easily add a CQRS or Traditional service to invoke a repository operation.

### Version 2.1.4

- Improvement: New setting to control how null child entities should be handled when performing an *update*.
- Improvement: A validation message will show when a "Call Service Operation" or "Query Entity Action" has a name which conflicts with a sibling element (such as a field on a Command) as such conflicts can cause ambiguities in mappings.

### Version 2.1.3

- Fixed: `Call Service Operation` mapping didn't allow mapping a 1 -> * association on an Entity that is supplied to the operation.
- Fixed: Creating DTOs off of Entities on the Advanced mapping screen will also happen consistently when done from Service Operations.

### Version 2.1.2

- Fixed: Added reference to Proxy `Operation`, so now mapping can correctly be done.

### Version 2.1.1

- Fixed: Accelorator for creating CRUD services will now correctly map operations named `Delete` or `Create`.

### Version 2.1.0

- Improvement: Help topics added.
- Improvement: New CRUD Creation screen added.
- Fixed: Creating CRUD operations for Nested Entities can now be targeted in a specific folder and no longer contains the parent element name in the operation/element names.

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
