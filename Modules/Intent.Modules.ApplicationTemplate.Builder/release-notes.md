### Version 3.7.0

- Improvement: Added support for `Is New`, `Incompatibilities`, `Documentation Url` and `Tags` to components, which will be respected in version `4.5.23` of Intent Architect and later.

### Version 3.6.1

- Feature: A context menu option has been added to packages to update versions from the content of a modules.config file.

### Version 3.6.0

- Improvement: Supports adding `Images` to Application Templates. These are supported and displayed in Intent Architect 4.5.0 and later.
- Improvement: Priority of application template ordering now shows on the display text of the package.

### Version 3.5.1

- Improvement: Added validation to prevent Modules being configured as `required` but not `included by default`, which caused them not to be installed when creating a new application, even if selected.

### Version 3.5.0

- Improvement: Added support for granular module installation settings introduced in Intent Architect 4.4.0.

  > ⚠️ **NOTE**
  >
  > Generated application templates will now have a minimum client version of 4.4.0.

### Version 3.4.9

- Improvement: Added ability to specify default `Create folder for solution` value.

### Version 3.4.8

- Improvement: The AutoCompile factory extension has been removed from this module and the more robust `Intent.ModuleBuilder.AutoCompile` module should be installed and used instead.
- Improvement: It is now possible to specify minimum versions of module dependencies.

### Version 3.4.7

- Improvement: Updated icon.

### Version 3.4.6

- Fixed: Packages will now assign the supported client version to include Intent Architect v5.

### Version 3.4.5

- Fixed: Dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.4

- Fixed: Module dependencies supported client versions update to prevent Software Factory warnings of possibly incompatible modules.

### Version 3.4.3

- It is now possible to specify the following Application Template defaults:
  - `Place solution and application in the same directory`
  - `Store Intent Architect files separate to codebase`
  - `Set .gitignore entries`
  - `Relative Output Location`

  Version 3.4.3-pre.2 or newer of Intent Architect is required for these defaults to be respected.
