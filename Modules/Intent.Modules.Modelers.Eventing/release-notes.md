### Version 5.0.3

- Improvement: `${application.name}.Eventing.Messages` package will now be created in the Services designer.

### Version 5.0.2

- Improvement: Added support for `Integration Command`s.

### Version 4.0.4

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.3

- Fixed: Updated package names from `Eventing.${application.name}` and `Eventing.${application.name}.Messages` to `${application.name}.Eventing` and `${application.name}.Eventing.Messages`.

### Version 4.0.2

- It is now possible to define and use DTOs.
- Utility methods for module authors to retrieve all used `EnumModel`s, `DTO`s or `Message`s.

### Version 4.0.1

- Fixed: Creating a new package will no longer create a new Application element. Only the first package will have an Application element.

### Version 4.0.0

- New: Designer rebuilt so that it now works off of a Diagram. Application is also now part of diagram and publish/subscribe metaphors are now associations.

### Version 3.3.3

- Eventing designer supports Messages, Publishers and Subscribers with this release.