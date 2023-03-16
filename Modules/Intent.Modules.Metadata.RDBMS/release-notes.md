### Version 3.4.2

- New: Support for modelling Sql Views throught the `View` stereotype which can be applied to `Class`s.

### Version 3.4.1

- Fixed: When properties were unset for `Column` and `Table` stereotypes, the annotation display would show blank or `null` values.

### Version 3.4.0

- New: Foreign Keys now have an explicit link with an Association Target End so that Attributes that act as Foreign Keys can be renamed independently from the Association.
- Update: Compute stereotype icon visible on diagram.
- Update: Default Constraint Stereotype has added hint for `Treat as SQL Expression` checbox.
- Update: Fill Factor property on Indexes in order for some database providers to configure it accordingly.

### Version 3.3.12

- Fixed: Issues around adding Text Constraints automatically via script.

### Version 3.3.11

- Fixed: Script would break when assigning associations to Entities.

### Version 3.3.10

- Fixed: Changing an Attribute on a Class would have toggled the Text Constraint Stereotype wrongfully.

### Version 3.3.9

- Update: Changed default `Key Creation Mode` from `Manually add PKs and FKs` to `Explicitly add PKs and FKs automatically`.

### Version 3.3.7

- Update: Attribute naming conventions (i.e. pascal-case vs camel-case) moved to Domain Settings (see settings panel).

### Version 3.3.6

- Fixed: When referenced in the `Module Builder` the Software Factory was updating `.csproj` files to reference version `3.3.6` of the NuGet Package which did not exist.
