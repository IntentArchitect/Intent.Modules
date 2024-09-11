### Version 5.4.0

- Service Proxy elements now have the "Injectable Service" trait, allowing them to be used as targets on particular advanced mapping dialogues.
- Operation elements now have the "Processing Handler" trait, allowing them to be used as targets on particular advanced mapping dialogues.
- Improvement: Certain element types will now show validation errors if their name is not unique.

### Version 5.3.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 5.2.3

- Improvement: Enabled support for an additional endpoint type: `Message Triggered`.

### Version 5.2.2

- Improvement: Moved auto-management of proxy parameters to the "Service Proxies" modeler.

### Version 5.2.1

- Fixed: Types.ServicesProxies on-loaded on every Service Proxy causing designer to hang on open.

### Version 5.2.0

- Improvement: Support for visually representing proxies in the Service designer.

### Version 5.1.0

- Fixed: Mapping system not able to traverse into nested folders when mapping to Commands and Queries (required Intent Architect 4.1.x)

### Version 5.0.0

> ⚠️ NOTE
>
> Major version update, ensure all Service Proxy related modules are also updated.

- Removed `Service Proxy DTO` and `Service Proxy Enum` element types (these were used for "wrappers" for `DTO` and `Enum` element types).
- Removed now obsolete extension methods.
- Designer Setting to create a `Service Proxy` within a `Folder` has now been moved to this module.
- When creating a `Service Proxy`, the mapping dialogue is now automatically launched.
- After mapping a `Service Proxy` if it was still default name, then it's name is automatically changed to `<Mapped Service Name>Proxy`.

### Version 4.1.0

- Service references will allow operation selection for generating proxy clients.
- Fixed: Issue where only ever a single enum would be generated per service proxy.

### Version 4.0.2

- Fixed: DTO Model discovery ignoring generic argument types.

### Version 4.0.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.0

- Updated to work with `Intent.Metadata.WebApi` version 4.x.

### Version 3.3.4

- Update: Introduced new API queries. Potentially optimized DTO discovery code and prevent circular references from crashing code.

### Version 3.3.3

- Update: Querying Service Proxy metadata will provide a Service Proxy model that references the Operations and Parameter metadata from the referenced Service.
