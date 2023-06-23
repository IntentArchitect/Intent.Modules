### Version 3.4.2

- Fixed: The execution order of template `AfterTemplateRegistration` and `BeforeTemplateExecution` methods could vary between different operating systems resulting in event based generation having a different order on different operating systems. 

### Version 3.4.1

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.0

- Supports searching for Stereotypes by Definition Id

### Version 3.3.21

- Fixed: An exception would occur when a TypeScript template would try resolve a collection type.

### Version 3.3.20

- Fixed: When `ClassTypeSource.Create()` was used without `WithNullableFormatter()` and/or `WithCollectionFormatter()` also being called, no default implementations would be applied.

### Version 3.3.19

- It is now possible to control OverwriteBehaviour for Static Content Template Files.

### Version 3.3.18

The following extension methods now use the Humanizer library's implementation and will also now remove any `-` characters present:

- `.ToDotCase()`
- `.ToKebabCase()` (does not remove `-` characters)
- `.ToSnakeCase()`

### Version 3.3.17

- Fixed: The GetTypeName system would discused generic type arguments for some languages.

### Version 3.3.16

- Fixed: `ToSnakeCase()`, `ToKebabCase()`, and `ToDotCase()` extension methods would put a separator before numeric characters.

### Version 3.3.15

- Extended the `IIntentTemplate` interface with `AddTypeSource`, `TryGetTypeName`, `GetTemplate` and `TryGetTemplate` methods.

### Version 3.3.14

- The `ToPluralName()` extension method's implementation now simply calls the `Pluralize(...)` method which has more robust logic and `ToPluralName()` has been marked as obsolete.

### Version 3.3.13

- Fixed: `ToSnakeCase()`, `ToKebabCase()`, and `ToDotCase()` extension methods did not respect special characters.

### Version 3.3.12

- Added `OfType<TTemplate>` `TemplateDependency` which can be used to find all templates of the provided `TTemplate` type.
