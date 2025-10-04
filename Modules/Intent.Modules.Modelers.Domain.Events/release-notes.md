### Version 4.4.5

- Improvement: `Domain Events` support modeling inheritance.
- Improvement: Made `Source End` of domain events not navigable, this makes it possible to model Domain Events in different packages without dependencies.

### Version 4.4.4

- Improvement: `Publish Domain Event` association lines in the Services designer are now aligned with the Domain and dotted.
- Fixed: Critical issue with missing configuration of Mapping Options causing loss of metadata on save.
- Fixed: Unique name warning when modeling multiple domain events.
- Fixed: Inheritance properties not accessible in `Entity to Domain Event Mapping`.

### Version 4.4.3

- Improvement: Updated Properties icon and added syntax highlighting to visuals.
- Improvement: Added Suggestion to auto-create a `Domain Event Handler` element from the `Domain Event` directly from the Domain designer.
- Fixed: Can't reorder Processing Actions under a `Domain Event Handler`.

### Version 4.4.2

- Improvement: `Publish Domain Event` suggestion now only shows for aggregate roots

### Version 4.4.1

- Improvement: Updated module references

### Version 4.4.0

- Improvement: Added `[Processing Handler]` trait to `Domain Event Handler` elements.

### Version 4.3.0

- Improvment: Added `Suggestion` (4.4.x feature) to auto-create a `Domain Event Handler` element from the `Domain Event`.

### Version 4.2.2

- Improvement: Updated module icon

### Version 4.2.1

- Fixed: Subscribing to `DomainEvent`s was renaming Domain Event Handler if it had been given a name, it now only "Names" it if it is the default name.

### Version 4.2.0

- Improvement: Advanced mapping configurations updated to use new capabilities made available in Intent Architect 4.3 for a better experience.
- Improvement: Certain element types will now show validation errors if their name is not unique.

### Version 4.1.1

- Fixed: Made a change to Entity -> Domain Event mapping config to be forward compatible with 4.3.

### Version 4.1.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 4.0.1

- Improvement: Updated icon(s).

### Version 4.0.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 3.5.0

- Supports indicating Domain Events being triggered from Class Constructors and Operations in the diagrams.

### Version 3.4.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.5

- Update: Changed the shortcut from `CTRL + SHIFT + E` to `CTRL + SHIFT + V`. This is to avoid collisions with adding `Enums`.