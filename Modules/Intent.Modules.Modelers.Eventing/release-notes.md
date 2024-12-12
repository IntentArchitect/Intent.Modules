### Version 6.0.0

> ⚠️ WARNING
>
> This version removes the Eventing Designer and any integration message modelling will now need to be performed in the Services Designer.
>
> Any existing messages in the Eventing Designer can simply be used as is in the Services Designer, but existing subscriptions will need to be re-modelled by creating subscription associations from the messages to Integration Event Handlers.
>
> For more information refer to the [migration documentation](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html#migrating-from-the-eventing-designer) or otherwise reach out to [Intent Architect Support](https://github.com/IntentArchitect/Support).

- Removes the `Eventing` designer from the application. All eventing related design work must now happen in the `Services` designer.
- Improvement: Added help documentation.

### Version 5.1.1

- Improvement: Updated module icon

### Version 5.1.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

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