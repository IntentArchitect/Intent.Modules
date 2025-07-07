### Version 2.0.1

- Fixed: Resolved issue where operation results could not be mapped to Imtegration Commands or Events.
- Fixed: The context menu options `Publish Integration Event` and `Send Integration Command` were missing from Service Operations.

### Version 2.0.0

- Improvement: Added `[Processing Handler]` trait to Event Handlers.

### Version 1.2.1

- Improvement: `ApiMetadataManagerCustomExtensions` been updated to have method overloads accepting `Application Id` parameters.
- Fixed: `GetExplicitlySentIntegrationCommandModels` and `GetExplicitlyPublishedMessageModels` implementations have been fixed.

### Version 1.2.0

- Improvement: Added `Suggestion` (4.4.x feature) for `Message` and `Integration Command` to auto-create an `Integration Event Handler`.

### Version 1.1.3

- Improvement: Updated references

### Version 1.1.2

- Improvement: Improved Event and Integration Command mappings for supporting Collections.
 
### Version 1.1.1

- Improvement: Added Stereotype visuals for `Message` and `Integration Command`.
- Fixed: Scripting issue which would under certain circumstances rename Integration Handlers.

### Version 1.1.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.
- Improvement: Added extension methods for getting messages, message DTOs and message Enums based on being used in an association of any type the services designer.

### Version 1.0.4

- Fixed: The `GetExplicitlyPublishedMessageModels` and `GetExplicitlySubscribedToMessageModels` extension methods had their implementations swapped between each other.

### Version 1.0.3

- Improvement: Added mapping scenario for a call operation to map in the services designer

### Version 1.0.2

- New Feature: Ability to model Integration Commands in Application Services.

### Version 1.0.0

- New Feature: Services modeler extensions for describing interactions between an application's services and integration events.
