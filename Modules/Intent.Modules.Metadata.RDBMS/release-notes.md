### Version 3.7.1

- Improvement: Improved documentation around the `Default Constraint` stereotype, as well as improved in-application hints.
- Fixed: Domain packages without any stereotypes at all would be regarded as having a "Relational Database" Stereotype applied. This was due to compatibility reasons for version 3.5.0 which added support for multiple persistence paradigms being used within the same Intent Architect Application. Module migrations now exist and will automatically add a "Relational Database" Stereotype for Intent Architect Applications which have never had their Domain Designer opened and saved with version 3.5.0 or higher of this module.

### Version 3.7.0

- Improvement: Improved documentation and added a number of help topics
- Improvement: Removed legacy `Key Creation Mode` option `Remove all managed PKs and FKs`

### Version 3.6.2

- Improvement: Added `Order` to the `Column` stereotype.

### Version 3.6.1

- Improvement: Added `TryGetDecimalPrecisionAndScale()` module setting method to parse the precision and scale from `Decimal Precision And Scale` setting.

### Version 3.6.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.5.12

- Improvement: "Auto Manage Keys" is now only available on Relational DB Domain Models.

### Version 3.5.11

- Fixed: Self-referencing Classes will handle Foreign Keys correctly.

### Version 3.5.10

- Improvement: Index naming respects table naming conventions.

### Version 3.5.9

- Improvement: Moving the on-load script from Attributes to a package level can have drastically faster load times.

### Version 3.5.8

- Improvement: `Decimal precision and scale` setting added to configure the default decimal precision and scale for your database technology. 

### Version 3.5.7

- Fixed: When modeling `0..1 --> 1` or `0..1 --> 0..1` (aggregrational) associations, foreign keys would not be added on the source `Class`, resulting in an incorrect requirement of the primary keys of each `Class` needing to match at the time of committing to the database.

### Version 3.5.6

- New Feature: Added `Schema` stereotype to modeling SQL Schema's simpler and more intuitive.
- New Feature: Added the ability to specify sort direction on Index columns .

### Version 3.5.5

- Fixed: Scripting errors to the effect of `on-loaded [RDBMS Key Automation Extensions] on element "<element name>" [Attribute]` would sometimes show in the Task Output Console on loading of the domain designer.

### Version 3.5.4

- Fixed: Designer management of foreign keys would be "opted out" when deleting a foreign/primary key attribute.

### Version 3.5.3

- Fixed: Primary and foreign keys would sometimes not update themselves correctly for inherited class scenarios.
- New Feature: An `Auto Manage Keys` context menu option has been added to `Class` elements. Using this option will ensure that keys are being managed automatically by the designer, this is useful for scenarios where a key was manually deleted from a `Class` in the past which put it into an "unmanaged" state preventing keys from being automatically managed going forward.

### Version 3.5.2

- Fixed: Script errors when creating new associations from the diagram.

### Version 3.5.1

- Fixed: Non Classes will not be affected by automated key management scripts.

### Version 3.4.2

- New Feature: Support for modeling Sql Views through the `View` stereotype which can be applied to `Class`s.

### Version 3.4.1

- Fixed: When properties were unset for `Column` and `Table` stereotypes, the annotation display would show blank or `null` values.

### Version 3.4.0

- New: Foreign Keys now have an explicit link with an Association Target End so that Attributes that act as Foreign Keys can be renamed independently from the Association.
- Improvement: Compute stereotype icon visible on diagram.
- Improvement: Default Constraint Stereotype has added hint for `Treat as SQL Expression` checkbox.
- Improvement: Fill Factor property on Indexes in order for some database providers to configure it accordingly.

### Version 3.3.12

- Fixed: Issues around adding Text Constraints automatically via script.

### Version 3.3.11

- Fixed: Script would break when assigning associations to Entities.

### Version 3.3.10

- Fixed: Changing an Attribute on a Class would have toggled the Text Constraint Stereotype wrongfully.

### Version 3.3.9

- Improvement: Changed default `Key Creation Mode` from `Manually add PKs and FKs` to `Explicitly add PKs and FKs automatically`.

### Version 3.3.7

- Improvement: Attribute naming conventions (i.e. pascal-case vs camel-case) moved to Domain Settings (see settings panel).

### Version 3.3.6

- Fixed: When referenced in the `Module Builder` the Software Factory was updating `.csproj` files to reference version `3.3.6` of the NuGet Package which did not exist.
