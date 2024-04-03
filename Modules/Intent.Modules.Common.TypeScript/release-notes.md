### Version 4.0.1

- Fixed: In some cases imports would not be generated.

### Version 4.0.0

- Improvement: Refactored code to use separately installable `Intent.Code.Weaving.TypeScript` module removing need for a "private" NuGet package.
- Improvement: Ensure that this module is running on the latest patterns and dependencies.
 
### Version 3.4.3

- Fixed: An exception would be thrown when the same decorator was applied more than once to a syntax node.
- Show additional information when a TypeScript weaving error occur.

### Version 3.4.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.1

- Updated supported client version to [3.3.0-pre.0, 5.0.0).

### Version 3.3.9

- Added basic support for named imports.

### Version 3.3.8

- TypeScript weaving better supports class base types when in `IntentMerge()` mode.
- TypeScript weaving will change the location on imports which have a single type imported, that is the same in the existing file.
- Fixed: Weaving not applied when a file is renamed.