### Version 3.3.8

- TypeScript weaving better supports class base types when in `IntentMerge()` mode.
- TypeScript weaving will change the location on imports which have a single type imported, that is the same in the existing file.
- Fixed: Weaving not applied when a file is renamed.