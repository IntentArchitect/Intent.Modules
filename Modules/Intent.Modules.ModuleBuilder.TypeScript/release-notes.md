### Version 3.5.3

- Improvement: Updated default output template to have correct constructor for TypeScriptFile.

### Version 3.5.2

- Improvement: The templates for the templates no longer use T4.

### Version 3.5.1

- Fixed: Module and nuget package installation will target stable version.

### Version 3.5.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.4.0

- Improvement: Will now add module dependencies on the following as needed:
    - Intent.Modules.Common.TypeScript 4.0.0.
    - Intent.Code.Weaving.TypeScript 1.0.0.
- Improvement: Ensure that this module is running on the latest patterns and dependencies.

### Version 3.3.10

- Fixed: Support for `ITypescriptFileBuilderTemplate` added and File Builder is now default option.

### Version 3.3.9

- Update: Partial Class Access modifier will now be `public` when `Custom` is used as Templating method.

### Version 3.3.8

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.7

- Updated supported client version to [3.3.0-pre.0, 5.0.0).

### Version 3.3.3

- New: `String Interpolation` is now available for `Templating Method` on `Typescript File Template`s.
