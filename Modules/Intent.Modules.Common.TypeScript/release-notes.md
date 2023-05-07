### Version 3.4.1

- Updated supported client version to [3.3.0-pre.0, 5.0.0).

### Version 3.3.9

- Added basic support for named imports.

### Version 3.3.8

- TypeScript weaving better supports class base types when in `IntentMerge()` mode.
- TypeScript weaving will change the location on imports which have a single type imported, that is the same in the existing file.
- Fixed: Weaving not applied when a file is renamed.