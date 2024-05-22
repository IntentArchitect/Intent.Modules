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
