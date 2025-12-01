### Version 1.0.4

- Fixed: Suggestions for `Call Backend Service` not processing correctly from an asyc/await microtask perspective when launching dialogs.

### Version 1.0.3

- Improvement: Service call mappings not have `Re-map Service Call` accelerators, for when you change your service contracts.

### Version 1.0.3

- Improvement: Route parameters now support nullable.
- Improvement: Updated error messaging when Route of a `Page` contains incorrect route parameters.

### Version 1.0.2

- Improvement: Navigations between Pages will be bidirectional by default.
- Fixed: Color of light-mode navigations is hard to see (too light).
- Fixed: Navigations can only target pages.

### Version 1.0.1

- Improvement: Created common TypeScript structure for calling backend services.
- Improvement: Added support for adding multiple services.
- Improvement: Made non-nullable Model Definition types nullable for UI concerns.
- Fixed: Traditional service mapping are now working correctly for writes.
- Fixed: `Call Backend Service` will no longer have errors when used without a diagram being open.

### Version 1.0.0

- Initial release.
- Improvement : Configured `Service Call Invocation` parameters to be traversable for mapping child elements.
- Improvement : Dialog Defaults name based on Operation
- Fixed : Applied the `Component` stereotype to custom component.
- Improvement : Remove entity name from route not just suffixes for default route
- Improvement : Menu Sider Suggestion for pages to add to menu