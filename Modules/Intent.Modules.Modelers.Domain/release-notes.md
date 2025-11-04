### Version 3.12.10

- Improvement: The `[Static Constructor]` trait can now be applied to static methods that return a type other than the containing entity, as long as that return type has a generic type parameter matching the containing entity.

### Version 3.12.9

- Fixed: A validation error that class names must be unique would show even when classes had a different number of generic type parameters.

### Version 3.12.8

- Fixed: `[Static Constructor]` script removing stereotype on edit incorrectly.

### Version 3.12.7

- Improvement: Added `[Static Constructor]` trait, which automatically applies itself to static `Operations` that return the `Class` type.
- Improvement: Show static operation icon in the diagrams.
- Fixed: Syntax highlighting now appearing when domain `Operations` and `Constructors` are referenced from other packages (e.g. from Services designer).

### Version 3.12.6

- Improvement: Subscriptions and Publishes are now colored with a yellow line to indicate that they are pushing the messages externally.

### Version 3.12.5

- Fixed: Default Values set on `Attributes` and `Parameters` not showing in the tree-view Model and diagram.

### Version 3.12.4

- Improvement: Associations can now be changed via a `Change Association` submenu with options to invert the relationship, update cardinality, and update navigability.

### Version 3.12.3

- Improvement: Improved `Aggregate` and `Package referenced` visualizations.

### Version 3.12.2

- Improvement: Newly created entities now have a default name of *NewEntity* and not *NewClass*.
- Improvement: Added support for `Domain` and `Services` naming conventions for `Entities`, `Attributes` and `Operations`.
- Improvement: Updated help topics.

### Version 3.12.1

- Improvement: Added additional help topics.

### Version 3.12.0

- Improvement: Added `ctrl + space` and `double-click` shortcuts to `Operation` and `Constructor` mappings.

### Version 3.11.1

- Fixed: `Attributes` under `Classes` able to reference `DataContracts`. This shouldn't be allowed since `DataContracts` should not be related to what actually is persisted.
- Improvement: Updated icons

### Version 3.11.0

- Improvement: Added icons to the `Attributes`, `Operations` and `Constructors` on domain entities in the diagrams.
- Improvement: Changed colour of `Operations` in syntax highlighting to help with cognitive load.
- Improvement: Changed default colour of entities in diagrams to make text more readable with syntax highlighting now available in 4.3.2 of Intent Architect.
- Fixed: `Operations` uniqueness validation not taking parameters into account.

### Version 3.10.0

- Improvement: Certain element types will now show validation errors if their name is not unique.

### Version 3.9.1

- Improvement: It is now possible to use Data Contracts as Attribute types.
- Fixed: Data Contracts with generalizations would show as `TypeName` as opposed to `TypeName : DerivedType`.

### Version 3.9.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.
- Fixed: Renamed `New Domain Object` to `New Domain Contract`.

### Version 3.8.0

- Improvement: `Data Contract` elements no longer use the `Base Type` (since its a  Type Reference) for doing inheritance but now it has its own `Generalization` association.

### Version 3.7.0

- Improvement: Updated icons.

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