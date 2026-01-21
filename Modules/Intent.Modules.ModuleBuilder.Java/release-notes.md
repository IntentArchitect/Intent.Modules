### Version 3.6.2

- Improvement: Updated to use latest module builder and ensured that minimum client version matches that required for SDK.

### Version 3.6.1

- Fixed: Module and nuget package installation will target stable version.

### Version 3.6.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.5.0

- Improvement: Will now add module dependencies on the following as needed:
  - Intent.Modules.Common.Java 4.0.0.
  - Intent.Code.Weaving.Java 1.0.0.
- Improvement: Ensure that this module is running on the latest patterns and dependencies.

### Version 3.4.2

- Fixed: Custom templates had a missing System using for NotImplementedException code.

### Version 3.4.1

- Fixed: Adding a new template that is Single File in nature with a name containing a suffix `Template` will no longer add `Template` as part of the Class name.

### Version 3.4.0

- Update: Partial Class Access modifier will now be `public` when `Java File Builder` or `Custom` is used as Templating method.
- New: File Builder (Builder Pattern paradigm) selectable as Templating Method for Templates.

### Version 3.3.8

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.7

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
