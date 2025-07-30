### Version 3.16.3

- Improvement: Documented the `Mappable Element Settings` stereotype.

### Version 3.16.2

- Improvement: Extended Suggestions configuration to include an `Icon` and `Order Priority` settings.
- Fixed: Script `Dependencies` on Event Handlers not being added to the `.designer.settings` file.

### Version 3.16.1

- Improvement: Made ElementModel construction more reliable by using the `SpecializationTypeId` instead of the `SpecializationType`.
- Improvement: Renamed `Accepted Stereotypes` to `Accepted Traits`, which is more correct to its function.
- Improvmeent: Throws an `ElementException` if a Stereotype Definition that isn't a trait is selected by the `Accepted Traits` type.
 
### Version 3.16.0

- Feature: Model support for Stereotype Definition `Is Trait` option. When checked, the Stereotype Definition will generate an interface. If it is applied to `Element Settings` or `Association End Settings`, the interface will be implemented by the wrapping models and/or appropriate extension methods will be created.

### Version 3.15.0

- Improvement: Documentation topics added.

### Version 3.14.0

- Improvement: Advanced mapping `Mapping Settings` now have a `Override Map To Parent Function` which allows the user to override which parent is used to add automatically mapped fields. This improvement addresses the friction in mapping `Service Operation` elements to the domain.
- Improvement: Menu Items can now be grouped with a group number, respected in Intent Architect 4.5.x and later.
- Improvement: Comments made against Element Settings and Stereotypes will now get propagated and made visible in Intent Architect 4.5.x and later.
- Improvement: Support for extension methods for `Item Lists` control types on Stereotype Properties (supported in Intent Architect 4.5.x and later)

### Version 3.13.1

- Fixed: `AttributeModel` extension methods now using Domain Model not the Common Types Model.

### Version 3.13.0

- Feature: Support for "suggestions" scripts that will be made available to users as part of Intent Architect 4.4.x and later.
- Improvement: Mappings and Script context menu options can now be configured to trigger on double-click of the element.

### Version 3.12.1

- Fixed: When using a multi-select setting type using directives were missing for `System.Linq` and `System.Text.Json`.

### Version 3.12.0

- Feature: Support for adding documentation to the module which will generate a `documentation.json` file. This file can be consumed by the new Documentation Dialog feature in Intent Architect 4.4.x and later.

### Version 3.11.3

- Fixed: `Intent.Common` update to fix "DefaultOverrideBehaviour" lacking `IOutputTarget` context.

### Version 3.11.2

- Fixed: Templates for generation of association APIs would output uncompilable code.

### Version 3.11.1

- Fixed: Intent Application name/descriptions containing characters that are invalid for XML will now be encoded correctly in the imodspec file.

### Version 3.11.0

- Improvement: Certain element types will now show validation errors if their name is not unique.
- Improvement: Updated the Module Builder to expose new designer configuration capabilities made available in Intent Architect 4.3.

### Version 3.10.3

- Improvement: Added/updated hints on some stereotype properties to better document their purpose and/or effect.

### Version 3.10.2

- Fixed: Bumped core nuget and module versions to fix compilation errors.

### Version 3.10.1

- Improvement: Removed "Icon" from the "Module Settings" stereotype as the icon can be controlled by the default Application "Settings" screen for the particular Module Builder application.

### Version 3.10.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.9.0

- Improvement: Supports additional options for the Advanced Mapping settings.

### Version 3.8.4

- Improvement: Uses a more recent version of the `Intent.OutputManager.RoslynWeaver` module.

### Version 3.8.3

- Improvement: Software Factory Extension classes now have `Intent.Modules.Common` added as a using directive by default.

### Version 3.8.2

- Improvement: Updated icon.

### Version 3.6.13

- Improvement: `TemplateExtensions` will now simply fully qualified type names and add using directives as appropriate.

### Version 3.6.11

- Fixed: Internal module dependency updates to be on the latest versions.

### Version 3.6.10

- Fixed: Nuget package version that gets automatically installed is no longer `pre`-release.

### Version 3.6.9

- Improvement: Added a default `OnAfterTemplateRegistrations` overload method to the factory extensions.
- Improvement: Added `Project URL` property to `Module Settings` stereotype.
- Removed template which generates `README.txt` file.

### Version 3.6.8

- Templates and Decorators will now be under full management in certain areas.

### Version 3.6.7

- Added `<tags></tags>` and `<releaseNotes></releaseNotes>` to `imodspec` files by default.
- Manage Dependency Versions for Intent Architect Modules and nuget packages in C# projects used to build Modules.

### Version 3.6.6
- Fixed: Fixed an issue around `File Template`s crashing the Software Factory.

### Version 3.6.4

- Added support for Templates to target Type-Definition model types. Will respect `C#` stereotype's `Namespace` setting.
- Updated version of `Intent.SoftwareFactory.SDK` NuGet package to install from `3.4.1` to `3.4.2`.

### Version 3.6.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.6.2

- Updated module dependencies to prevent Software Factory warnings of possibly incompatible modules.

### Version 3.6.1

- Nuget Version bumps to align with `Domain Designer`
- Fixed : `ModuleSettingsExtensionsTemplate` was missing a Nuget Dependancy on `Intent.Common`.

### Version 3.6.0

- TemplateExtensions now use `IIntentTemplate` as extension base instead of `IntentTemplateBase`.
- Support for association ends to have Script Options in their context menus.

### Version 3.5.0

- Fixed: Fixed: Static content template `readme.txt` files would be deleted on subsequent software factory runs.
- Fixed: Removed possible legacy hook-in point for something more appropriate.
- New: Added Extension method for checking if a `IPackage` is a certain Model type.
- `ApiPackageExtensionModelTemplate` will not run unless it has at least one method to generate.
- `ApiPackageExtensionModelTemplate` will add NuGet dependencies of extended packages if available.

### Version 3.4.1

- Feature: Support for explicit Element / Association Vision Settings for Diagrams.
- Fixed: Association Extensions applying source-end settings to the target-end.

### Version 3.4.0

- Support for Package DefaultNameFunction. Allows the name of a new package to be determined by a script.

### Version 3.3.14

- Default `Templating Method` changed to `String Interpolation` on `File Template`s.

### Version 3.3.13

- Role fields are mandatory of Template Type is not `Custom`.

### Version 3.3.10

- New: `String Interpolation` is now available for `Templating Method` on `File Template`s.
