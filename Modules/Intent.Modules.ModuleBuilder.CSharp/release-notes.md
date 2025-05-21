### Version 3.7.4

- Fixed: NuGet version switch statement check improved to cater for certain versions which would not be caught by the switch.

### Version 3.7.3

- Improvement: NuGet packages better support for Minor versions.

### Version 3.7.2

- Improvement: Added `.NETStandard,Version=v2.1` as a minimum target framework for NuGet modeling.

### Version 3.7.1

- Improvement: Updated NuGet Version element to support duplicate names.
- Improvement: Updated Modules / NuGets to official versions.

### Version 3.7.0

- Improvement: Added support for generating Razor templates.

### Version 3.6.2

- Improvement: Added support for NuGet Package dependencies.

### Version 3.6.1

- Improvement: Fixed bug around using generation of NugetPackages template.

### Version 3.6.0

- Improvement: Added implementations for NuGet modeling

### Version 3.5.2

- Improvement: NuGet modeling support.

### Version 3.5.1

- Improvement: Updated interpolated C# templates to be simpler and use raw string literals.

### Version 3.5.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.4.8

- Improvement: Uses newer version of the `Intent.Common.CSharp` module.

### Version 3.4.7

- Improvement: Updated dependencies.

### Version 3.4.6

- Improvement: Improved default implementation of interfaces.

### Version 3.4.5

- Improvement: Implements a default interface implementation for CSharp Templates that's name is suffixed with `Interface`.

### Version 3.4.4

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.3
- Fix: Custom Templates not compiling out the box, added missing `using System` clause. 

### Version 3.4.1

- Update: Partial Class Access modifier will now be `public` when `C# File Builder` or `Custom` is used as Templating method.

### Version 3.4.0

- New: File Builder (Builder Pattern paradigm) selectable as Templating Method for Templates.