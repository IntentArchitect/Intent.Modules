### Version 3.4.5

- Update : Ability to map from base classes for `Constructor` and `Operation` mappings.

### Version 3.4.4

- Added creation option for `Enum`s to `Diagram`s as it only existed in the tree view before.

### Version 3.4.3

- Parameters can now have default values.

### Version 3.4.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.0

- Added concept of `Data Contract`: a generic contract type that has attributes. This type is particularly useful in DDD environments where we want to pass data into constructors and operations.

### Version 3.3.10

- Updated display text of classes to show base class if inheriting.

### Version 3.3.8

- Update: `IsAggregateRoot()` will no longer consider `IsAbstract` models as Aggregate Roots. 

### Version 3.3.6

 - Domain settings: Allows specifiying default naming conventions for Attributes (e.g. `camel-case` or `pascal-case`).
 - Validations to warn of duplicate Attributes and Classes names.