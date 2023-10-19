### Version 3.3.43

- Improvement: Added support for `await` on `foreach` statements.
- Improvement: Added support for `required` on `Field`s and `Property`s statements.

### Version 3.3.42

- Improvement: Added CSharpWhileStatement for adding `while(true) { }` statements.
- Improvement: It is now possible specify `when` expressions on `CSharpCatchBlock`s.
- Improvement: Introduced `DefaultIntentManaged` and `IntentTagMode` attributes for File Builder.
- Improvement: CSharpParameter now has ability to store metadata.
- Improvement: CSharpConstructorParameter will propagate metadata to created fields and properties.

### Version 3.3.41

- Improvement: Added Blazor WebAssembly configuration options for `launchsettings.json`.
- Improvement: CSharpConstructorParameter now has ability to store metadata.
- Fixed: Adding statements on a constructor will assign the parent on the element receiving those statements.

### Version 3.3.40

- Added: Description Stereotype for usage with Enum literals
- Added: Support for Parameter Modifiers (out, ref, [in])
- Added: Enum support for File Builder added.

### Version 3.3.39

- Added: `RemoveNugetPackageEvent` to support removal of deprecated and or unused Nuget packages.
- Added: Added support for XMLDocComments on `Method` and `Constructor` `Parameter`s.
- Added: `ProtectedSetter` added to `CSharpProperty`.
- Added: `IsExplicitImplementationFor(string @interface)` added to `CSharpMethod` for being able to make a method explicitly implement an interface's method.
- Added: It is now possible to specify that `CSharpStatementBlock`s and derived types are *not* `SeparatedFromPrevious` by using the the `.SeparatedFromPrevious(false)` method.
- Added: `CSharpClass`'s `WithBaseType` and `ExtendsClass` methods now have overloads which can take an `IEnumerable<string> genericTypeParameters`.
- Added: `InfrastructureRegisteredEvent` which is used to register Infrastructure components for Health checks (for example).

### Version 3.3.38

- Fixed: `IntroduceProperty` wasn't handling C# reserved words correctly.

### Version 3.3.37

- Update: It is now possible to specify `static` modifiers for interface methods for the `CSharpFile` builder.
- Update: It is now possible to declare default implementations for interface methods for the `CSharpFile` builder.
- Update: It is now possible to specify `static` modifiers for Class Fields.
- Fix: Namespace resolution wasn't taking the current template namespace into account, occasionally resulting in ambiguous type references in code.
- Fixed: `CSharpFile`'s `StartBuild()`, `CompleteBuild()` and `AfterBuild()` were being called for templates even if their `CanRunTemplate()` was returning `false`.

### Version 3.3.36

- Update: Added the following methods to `CSharpTemplateBase<T>` for determining nullable reference type statuses for an `ITypeReference`:
  - `public bool IsNonNullableReferenceType(ITypeReference typeReference, bool forceNullableEnabled = false)`
  - `public bool IsNullableReferenceType(ITypeReference typeReference, bool forceNullableEnabled = false)`
  - `IsReferenceTypeWithNullability(ITypeReference typeReference, bool isNullable, bool forceNullableEnabled = false)`

  For all these methods they will normally only return `true` if their template's _Template Output_ has been placed in a C# project with _Nullable_ enabled, but this can be skipped by setting `forceNullableEnabled` to `true`.

### Version 3.3.35

- Update: `CSharpConstructor` now supports `IHasCSharpStatements`.
- Update: CSharpClassMethod can now have an expression body to output the form: `public DateTime GetTime() => DateTime.Now;`.
- Update: CSharpLambdaBlock can now have a single line expression as well for cases like: `services.AddScoped<IUnitOfWork>(provider => provider.GetService<ApplicationDbContext>());`.
- Update: Statement block types will now format multi-line expressions so that for example `if()` expressions look better when the expression inside it spans multiple lines.
- New: Added `CSharpFinallyBlock` to complement the `CSharpCatchBlock`.

### Version 3.3.34

- Update: `.Async()` on Class Methods now checks if return type is of type `Task` and adds it if not specified.
- Fixed: Comments for interfaces themselves would not be generated.
- New: Support for .Net known type to improve `Type` name normalization, currently supports `System.Attribute`, `System.Action`.
- Fixed: `Type` name normalization system was unaware of `CSharpFileBuilder` using clauses.
- Fixed: `Type` name normalization wasn't disambiguating `Type` names and `Namespaces` under certain conditions.
- Fixed: Under certain circumstances `CSharpFileBuilder`'s setters and getters would generate the requested access modifiers.
- Added: `TryAddXmlDocComments` extension method to `CSharpDeclaration`s.

### Version 3.3.33

- Update: `CSharpFileBuilder` now has a `TypeDeclarations` which is a collection of Record and Classes in the file. This change allows templates to treat Records or classes in a unified manner.
- Update: `CSharpField` now has a `WithAssignment` method.

### Version 3.3.32

- Update: `CSharpFileBuilder` will now automatically ensure that parameter names for methods and constructors become prefixed with '@' when they would otherwise be a C# language reserved word. This was most commonly an issue when `event` was used for parameter name in an Intent designer.

### Version 3.3.31

- Changed: on `LaunchProfileRegistrationRequest` class, changed `UseSsl` to default to `true`, inline with `launchSettings.json` defaults.
- Added `CSharpObjectInitKeyValueStatement` and object initialization support to builder for easier instantiation of Dictionaries.

### Version 3.3.29

- Update: Now possible to construct `record`s with the `CSharpFileBuilder`. 

### Version 3.3.28

- Update: Class Constructor can be static.
- Update: Class Properties can be static.
- Update: Constructor parameter can have default value.
- Update: Method chain statement builder can now omit semi-colons.
- Update: Free Code Blocks now supported on Interfaces and Classes for cases where only freetext needs to be added a Class / Interface.
- Update: Added `TimeSpan` and `Dictionary<TKey, TValue>` types and made the `Intent.Common.CSharp` package be automatically referenced in all designers.
- Fixed: Methods with abstract keyword and without statements will not output method body.
- Fixed: ` : this()` constructor call didn't show when telling the Constructor to `CallsThis()` without arguments.

### Version 3.3.27

- New: Added Generic Type Constraint support.

### Version 3.3.26

- New statement classes added:
  - CSharpLambdaBlock
  - CSharpObjectInitializerBlock
  - CSharpMethodChainStatement
- Updated Builders for Classes, Interfaces and Methods to accept Generic parameters.

### Version 3.3.23

- Added `.AsEmbeddedResource(...) extension method which will make the file item in the .csproj an Embedded Resource. See https://docs.intentarchitect.com/articles/module-building/templates/how-to-control-file-properties-in-cs-projects/how-to-control-file-properties-in-cs-projects.html for further information.
- The `GetNamespace(this IOutputTarget target)` extension method is now cognisant of the `Root Namespace` value set for a project in the Visual Studio designer.

### Version 3.3.20

- Removed `GetKnownTypesByNamespace is being called before Template Registration has been completed. Ensure that methods like GetTypeName and UseType are not being used in template constructors.` warning.

### Version 3.3.19

- `UseType` and `GetTypeName` can now disambiguate types by additional qualification when it identifies that a type with a matching name exists for more than one namespace which is present in a file's using directives. This can only be done for types which are generated by templates.
- Improved the logic of the `CSharpResolvedTypeInfo.Create` method.

### Version 3.3.18

- Fixed: Non-first generic type parameters were not being normalized. For example `Dictionary<System.String, System.String>` should have been normalized to `Dictionary<String, String>` but was incorrectly normalized to `Dictionary<String, System.String>`.

### Version 3.3.17

 - Builder pattern for templating C# files (see type `Intent.Modules.Common.CSharp.Builder.CSharpFile`). 
	Note that this is an experimental templating pattern and, if used, one should expect changes to the API in future releases.
	An example of this templating approach can be found in the `Intent.Modules.ValueObjects` module in the `Intent.Modules.NET` open-source repository.