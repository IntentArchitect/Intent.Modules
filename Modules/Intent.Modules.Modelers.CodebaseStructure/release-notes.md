### Version 1.0.2

- Fixed: The Root Folder's own output target ignored the `Relative Location` shift defined by `Intent.Modules.VisualStudio.Projects`' `Root Folder Options` stereotype, always resolving to the unshifted location. Once this module took over Root Folder registration (from `1.0.1-pre.0`), anything anchored directly to the Root Folder - such as static files dropped outside the VS Solution - stopped shifting with everything else, producing duplicate/orphaned files. `RelativeLocation` is now read generically from the `Root Folder Options` stereotype by name and applied like the rest of the tree.

### Version 1.0.1

- Improvement: Moved common folder logic from `Intent.VisualStudio.Projects`

### Version 1.0.0

- Initial version.
