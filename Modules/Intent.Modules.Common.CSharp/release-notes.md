### Version 3.8.0

- Improvement: Added support for Razor file code management.

### Version 3.7.2

- Improvement: Added Attribute support for Class Constructor parameters.
- Improvement: `CSharpInvocationStatement` can now be used to chain method calls similarly to what `CSharpMethodChainStatement` did.

> ⚠️ NOTE
> 
> `CSharpMethodChainStatement` and `AddMethodChainStatement()` is made obsolete. Please make use of `CSharpInvocationStatement` and `AddInvocation()` instead. 

### Version 3.7.1

- Improvement: Support for `ValueTask` as a `CSharpType` on Methods.

### Version 3.7.0

- New Feature: `CSharpType` to represent a C# Type that can be more easily mutated and reliably inspected for the type it is. It has a parser to convert `string` into a `CSharpType`.
- New Feature: `CSharpVariableDeclaration` and `CSharpDeclarationExpression` to represent variable declarations in C# like `var myVar` and `var (prop1, prop2, prop3)`.
- Improvement: `CSharpClass`, `CSharpClassMethod`, `CSharpInterface` and `CSharpInterfaceMethod` are able to accomodate return types in the form of `CSharpType`.

### Version 3.6.4

- Fixed: Generated namespaces would `Pascal Case` application names, it now uses the casing as is.
- Fixed: `CSharpMethod` and `CSharpInterfaceMethod`  return types starting with the letters `Task` generated incorrect code under certain circumstances.

### Version 3.6.3

- New Feature: Classes and Records can now define `Primary Constructors`.
- Improvement: Convenient `IncludesXmlDocComments` method to insert `/// <inheritdoc cref="" />` comments.
- Improvement: Exposed `Member` as public in `CSharpAccessMemberStatement` class.
- Fixed: CRUD Mapping issue around constructors with ValueObjects as parameters.

### Version 3.6.2

- Fixed: XML Comment on parameter had too many `>` characters, i.e. `// <param name="param">>Test Parameter</param>`.

### Version 3.6.1

- Improvement: Under certain scenarios, the software factory would crash with certain model names, a set of these has been addressed. 

### Version 3.6.0

> ⚠️ NOTE
> 
> Prior to this version of the module, any data in Intent Architect designers referencing the date 
> type would generate `System.DateTime` in C# code, but going forward they will instead generate 
> `System.DateOnly`. To avoid causing code compilation issues and possible bugs, on upgrading 
> this module it will change all designer references using `date` to instead use `datetime`. 
> Should you wish to maintain date uses in your designers, you can simply roll-back the changes that 
> get made to the `.xml` files in your source code management software (e.g. Git). Please don't 
> hesitate to reach out to our [Support](https://github.com/IntentArchitect/Support) should you 
> have any comments or questions.

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.
- Improvement: Added Events for requesting Adding Properties to CSProj files and secrets to UserSecrets.
- Improvement: `date` types will now translate into `System.DateOnly` .NET types and no longer to `System.DateTime`.

### Version 3.5.6

- Fixed: Advanced mappings not generating compiling in certain scenarios.

### Version 3.5.5

- Improvement: Aligned VS Solution Project Type Ids to the new project system ids (https://github.com/dotnet/project-system/blob/main/docs/opening-with-new-project-system.md).
- Improvement: New `AddKnownType(string fullyQualifiedTypeName)` method added to `CSharpTemplateBase` which can be used to disambiguate type references in cases where multiple using directive namespaces have types with the same.
- Improvement: For the advanced mapping system, `ConstructorMapping` now has better handling of un-mapped constructor parameters, similarly for `MethodInvocationMapping`.
- Improvement: `AddNestedInterface` added to `CSharpClass`.
- Improvement: `ObjectInitializationMapping` is now able to also detect `ctors` from File Builder Templates when no `ctor` mappings are present.
- Improvement: `ObjectInitializationMapping` updated to help facilitate inheritance mappings found through Element Type Reference.
- Improvement: Overload added to `AddOptionalCancellationTokenParameter` to allow specifying name of the parameter.
- Fixed: When using `.Async()` in the C# File Builder on a nested class, a null reference exception would be thrown.

### Version 3.5.4

- Improvement: Added `WithParamsParameterModifier()` method to `CSharpParameter`.

### Version 3.5.3

- Improvement: Added support for protected fields.

### Version 3.5.2

- Fixed: Indentation would always get lost when using `.AddStatements(...)` with a single string to add multiline statements.
- Fixed: Ordering of access modifiers would be incorrect for fields which were `static readonly`.
- Fixed: Ordering of CSharpFile build actions could vary by operating system, additionally, processing of build actions were only ordered within a particular template rather than holistically across all templates.

### Version 3.5.1

- Improvement: Added support for `Value Object` collection mapping.

### Version 3.5.0

- Improvement: `IHasCSharpStatements`'s `FindStatement` method now returns the child statement match (the more specific match) rather than parent match if there are multiple matches.
- Improvement: CreateOrUpdate methods now support domain interfaces.

### Version 3.4.1

- Fixed: Before and After Template Execution phases of the Software Factory would be very slow for large applications.

### Version 3.4.0

- Added: Advanced Mapping support with the `CSharpClassMappingManager`
- Added: `CSharpAssignmentStatement` which takes in a left-hand and right-hand set of statements for standard variable and property assignements.
- Added: `IHasCSharpName GetReferenceForModel(...)` on `CSharpFile` to support resolving reference names for methods, properties and parameters.
- Added: `AddTopLevelStatements` on `CSharpFile` for support of [top-level statements](https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements).
- Added: Abstractions for working with `Program.cs` and `Startup.cs` files in a way where you don't have to be aware of whether or not _top-level statements_ and/or _use minimal hosting model_ have been selected.
- Improvement: `CSharpFile` and `CSharpFileConfig` now have `IntentTagMode(Ex/Im)plicit()` methods which can be used by templates to individually override the application's default Tag Mode for themselves. Requires at least  version 4.4.0 of the `Intent.OutputManager.RoslynWeaver` module for this option to be respected.
- Fixed: Necessary type disambiguation would not occur when one of the current class's namespace parts contained a type with the same name as the type being resolved.
- Fixed: When a .NET project was set to use .NET 8 the software factory would show the following warning: `Assuming language version "11.0" for project "<Name>" targeting "net8.0"`. .NET 8 projects will now use language version `12.0`.

### Version 3.3.44

- Improvement: Added support changing `ReturnType` on class and interface methods using `WithReturnType`.
- Improvement: Added `InsertParameter` method to method builders.

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