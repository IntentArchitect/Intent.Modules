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
