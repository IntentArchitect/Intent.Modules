### Version 3.6.0

- Improvement: Added support for [Syntax Highlighting introduced in Intent Architect 4.2](https://docs.intentarchitect.com/articles/release-notes/intent-architect-v4.2.html#syntax-highlighting-and-ctrl--click-navigation).

### Version 3.5.1

- Fixed: Minor fixes in visual and mapping settings.

### Version 3.5.0

- Fixed: Making a self-referencing association will no longer default to the Class name but suffix with `Reference`. Example: `Node` -> `Node` will have an association of `NodeReference`.

### Version 3.4.6

- Update : Ability to map from association source ends for `Constructor` and `Operation` mappings.

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