### Version 1.1.2

- Improvement: `Custom File Classification` entries now display as `[Severity - Classification] glob, glob` making them easier to scan when several are applied.
- Improvement: Clarified that where multiple `Custom File Classification` entries match the same file, the last matching entry wins, so broad catch-all entries should be placed first with exceptions below them.
- Improvement: If `Intent.VisualStudio.Projects` is already installed, it will be updated `4.1.10` so that Solution Folders and .NET projects also allow adding of the `Custom File Classification` stereotype them.

### Version 1.1.1

- Improvement: `Custom File Classification` stereotype now has an "Entries" property which allows multiple values instead of the stereotype needing to be applied multiple times.
- Improvement: The `Allows Custom File Classification` is now stored packages to reduce git commit noise.

### Version 1.1.0

- New: Added `Output Classification` (applied to generated `Template Output`s by module install/update) and `Custom File Classification` (applied manually to folders/projects, glob-scoped, multi-apply) stereotypes so tooling can classify output by severity and category.

### Version 1.0.2

- Fixed: Relocated the `Root Folder Options` stereotype to the `Codebase Structure` designer so as to control the root folder options for generated content.

### Version 1.0.1

- Improvement: Moved common folder logic from `Intent.VisualStudio.Projects`

### Version 1.0.0

- Initial version.
