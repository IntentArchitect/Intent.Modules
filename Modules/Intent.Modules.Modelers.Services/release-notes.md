### Version 4.0.6

- Fixed: Visual bug on Azure Function traditional service routes.
 
### Version 4.0.5

- Improvement: DTOs will now warn if default values have been set on properties which are proceeded by properties without default values

### Version 4.0.4

- Improvement: `Perform Invocation` interactions will automatically add the Invocation mapping on creation.
- Improvement: `Perform Invocation` that execute after any Processing Actions will not appear in the Mapping dialog. 

### Version 4.0.3

- Improvement: `Perform Invocation` interactions will now color in yellow when invoking remote services from other applications.
- Improvement: Referenced `Service` visuals will have a slight teal colored to signify that they are referenced.
- Improvement: `Perform Invocation` will always launch the Invocation Mapping dialog.
- Fixed: Errors in `Display Text Function` scripts for Operations.
- Fixed: DTO Mappings that create new DTOs (e.g. for new collection mappings) would not respect the `Is Nullable` and `Is Collection` settings for the automatically added child fields.

### Version 4.0.2

- Improvement: Made `Service` visuals on diagrams slightly more compact.
- Improvement: Added HTTP information to the diagram visuals where a Service's `Operation` is exposed via HTTP.
- Improvement: Added `CursorPagedResult` to support cursor based pagination.

### Version 4.0.1

- Fixed: `Operation Parameter` mapping now working correctly (double clicking and dragging)

### Version 4.0.0

- New Feature: Generalized `Perform Invocation` interactions for indicating invocations of any accessible services in the application. This replaces legacy associations like the `Call Service Operation` and `Send Command` that existed in the `Intent.Modelers.Services.EventInteractions` module.

### Version 3.10.0

- Improvement: Services can now be associated with comments in designer diagrams.

### Version 3.9.4

- Improvement: Mapping from the Domain to `Operations` on `Services` will now add automatically add new fields to the first parameter of type `DTO`.

### Version 3.9.3

- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.
- Fixed: Creating DTOs off of Entities on the Advanced mapping screen will also happen consistently when done from Service Operations.

### Version 3.9.2

- Improvement: Mapping from Domain to DTO will now try to match the fields by name as opposed to just adding new fields to the DTO with the mapping information alongside the original fields.

### Version 3.9.1

- Improvement: Added `Service Proxy`s element type to `Add to Diagram`.


### Version 3.9.0

- Improvement: Added `Add to Diagram` option to the `Services` diagrams.

### Version 3.8.3

- Improvement: It is now possible to map fields on DTOs to attributes on generalized types of a _Domain Data Contract_.

### Version 3.8.2

- Improvement: Updated icons.
- Improvement: Inherited DTO fields are now displayed on the mapping screen

### Version 3.8.1

- Fixed: Operation uniqueness validation using name without consideration for parameters.

### Version 3.8.0

- Improvement: Certain element types will now show validation errors if their name is not unique.
- Improvement: `PagedResult` type altered so that it no longer gets picked up by DTO generation templates by accident.

### Version 3.7.5

- Improvement: It is now possible to create a _Comment_ element type in the _Services_ designer. Inline with comments which already existed in the _Domain_ designer, these comments can be placed on a diagram and associated with other elements.

### Version 3.7.4

- Fixed: Ensure that when the `Web Client` module is installed that it will install the Service designer package reference.

### Version 3.7.3

- Improvement: Better Rich Domain Support around `Value Object`s and `Data Contract`s.

### Version 3.7.2

- Improvement: Bringing Traditional `ServiceModel`s to have better feature parity with the CQRS paradigm.

### Version 3.7.1

- Improvement: Updated Crud scripts to not add implicit primary keys.

### Version 3.7.0

- Improvement: Updated Dto mapping script to not add implicit primary keys.

### Version 3.7.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.6.1

- Improvement: Removed support for implicit keys, allowing keyless entities to now be modelled.

### Version 3.6.0

- Improvement: Updated additional icons. 

### Version 3.5.1

- Improvement: Updated icons. 

### Version 3.5.0

- New Feature: Upgraded mapping system to use advanced mapping capabilities.

### Version 3.4.9

- Improvement: `PagedResult<TData>` Type-Definition moved into this module from the `Intent.Application.Dtos.Pagination` module.

### Version 3.4.8

- Improvement : Upgraded CRUD Scripts to be forwards compatible with Intent Architect v4.1.

### Version 3.4.7

- New Feature : Duplicate `Operation` validation check added.

### Version 3.4.5

- Improvement : Moved `Project to Domain` script into `Services Designer` and out of the individual CRUD modules.
- Improvement : DTOs will ignore mapping certain Class Attributes that are internally marked as `set-by-infrastructure` so as to not exposing them to the public unintentionally.

### Version 3.4.3

- Improvement : Support default values parameters for Services, Domain and Http Metadata.

### Version 3.4.2

- Improvement : Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.9

- Fixed: The shortcut keys from `DTO`s and `Type`s weren't working.

### Version 3.3.8

- Improvement : Validation added to DTO Fields to ensure that the names are unique.

### Version 3.3.7

- It's now possible to map DTOs from Value Objects.
- The default name for new DTOs has changed from `NewDTO` to `NewDto`.

### Version 3.3.6

- DTOs can now have a generalization association with other DTOs.
