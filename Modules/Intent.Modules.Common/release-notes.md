### Version 3.9.2

- Improvement: Now leverages `IFileSystem` to benefit from file access caching in the Software Factory.

### Version 3.9.1

- Improvement: Created `ExecutionLifeCycle` static for statically registering actions to be executed during Software Factory runs. Currently used to clear static cache between "hot SF" runs.
- Fixed: Execution order of file builder `OnBuild(...)` and `OnAfterBuild(...)` methods were not deterministic between different operating systems resulting in different generated output under certain circumstances.

### Version 3.9.0

- Feature: Added support for filtering template instances for registrations.
- Improvement: `UpdateTemplateFileConfig` virtual method added to `StaticContentTemplateRegistration` to allow updates or replacement of the `ITemplateFileConfig` to be used by the templates.
- Improvement: `RegisterTemplate` virtual method added to `StaticContentTemplateRegistration` to allow customization of template registration.

### Version 3.8.0

- Improvement: Added `[Invokable]` stereotype (trait) to this module.

### Version 3.7.8

- Improvement: Moved `[Allow Comment]` stereotype to this module.

### Version 3.7.7

- Improvement: Added support for JSON merging.
- Improvement: Added support for YAML merging.

### Version 3.7.6

- Improvement: Added `ToSanitized` method to remove special characters from a string

### Version 3.7.5

- Improvement: Added Software Factory Execution help topics.

### Version 3.7.4

- Improvement: Added `Element Extensions` to the targetable types for the core trait stereotypes.

### Version 3.7.3

- Improvement: Disabling Templates via the CanRun property during Software Factory Execution.

### Version 3.7.2

- Fixed: `DefaultOverrideBehaviour` lacked `IOutputTarget` context which provides access to application settings that can help better resolve what the override behaviour should be.
- Fixed: The YAML file builder would not correctly indent multiline string literal values when compiled on an operating system using `\n` for line endings, such as Linux.

### Version 3.7.1

- Improvement: Made `TryGetExistingFileContent` and `TryGetExistingFilePath` available on `IIntentTemplateBase`.

### Version 3.7.0

- Improvement: Update file builders used by template authors to be more consistent with other file builders and ensure that execution order is deterministic.

### Version 3.6.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.5.3

- Improvement: Added a `strict` option to `ToCamelCase`, when true the algorithm is in-line with `System.Text.Json` camel case algorithm.

### Version 3.5.2

- Improvement: Added overrideable `RelativeOutputPathPrefix` to `StaticContentTemplateRegistration`. This can be used to add a prefix to the relative output path of all the files for the template.

### Version 3.5.1

- Fixed: The Software Factory would crash when trying to access a stereotype property whose package hadn't been added as a reference.

### Version 3.5.0

- New Feature: Support for binary based templates, including `File Template` and `Static Content Template` being enhanced to support binary files.

### Version 3.4.4

- Fixed: Changed the default behaviour or Singularize and Pularize methods to not assume the current state of the word.

### Version 3.4.3

- Offers a useful extension method overload for finding templates: `IEnumerable<TTemplate> FindTemplateInstances<TTemplate>(this ISoftwareFactoryExecutionContext executionContext, string templateIdOrRole)`

### Version 3.4.2

- Fixed: The execution order of template `AfterTemplateRegistration` and `BeforeTemplateExecution` methods could vary between different operating systems resulting in event based generation having a different order on different operating systems.
- Fixed: `AfterTemplateRegistration` and `BeforeTemplateExecution` were being called for templates even if their `CanRunTemplate()` was returning `false`.

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
