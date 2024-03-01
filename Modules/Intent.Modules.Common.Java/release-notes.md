### Version 3.4.9

- Fixed: Better test coverage for Java parsing to iron out remaining issues.

### Version 3.4.8

- Fixed: Bug in Java parsing of certain annotations.

### Version 3.4.7

- Improvement: Java Weaver updated to support Java 17 syntax (includes support now for `"""` text block quotes).

### Version 3.4.6

- Fixed: `static` keyword (along with other modifiers) will be placed before generic `<T>` parameters.

### Version 3.4.5

- Improvement: `Exclusions`, `Type` and `Scope` properties have been added to `JavaDependency` so that it now covers all [available dependency options](https://maven.apache.org/pom.html#dependencies).
- Improvement: Class and Interface methods and now indicate that they `throws` one or more checked Exceptions.
- Improvement: Constructor parameters can have parameters be automatically assigned (and create) private (final) fields.
- Improvement: Java Fields can now be marked as Static.

### Version 3.4.4

- Improvement: Updated Java File Builder in order for `final` to be applied in fields and parameters.
- Improvement: Made minor corrections in the naming of certain Java concepts in the File Builder.
- Improvement: Added Generic Type parameter support for Classes and Methods (no constraint support yet).
- Improvement: Modified the Java Fields and Interface Fields with appropriate defaults and with additional access modifier methods. Fields can also have default values.
- Improvement: Java Interfaces can now have method bodies as per the `default` keyword.

### Version 3.4.3

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.1

- Improvement: JavaComments now properly supports the /* */ format.
- Improvement: Free Code Blocks now supported on Interfaces and Classes for cases where only freetext needs to be added a Class / Interface.
- Fixed: Interface generated wrong code for extending other interfaces.

### Version 3.4.0

- New Feature: Introducing the Java File Builder Pattern.

### Version 3.3.18

- Fixed: Code management would not apply changes to to generic type parameters when merging.

### Version 3.3.17

- Improvement: Added `ApplicationPropertyRequiredEvent` and convenience `.ApplyApplicationProperty(...)` extension method for setting default application properties.

### Version 3.3.16

- Improvement: Improved the logic of `.ToJavaIdentifier()` to exhaustively conform to the Java identifier specification. For more information review the XML documentation comments for `Intent.Modules.Common.Java.Templates.JavaIdentifierExtensionMethods.ToJavaIdentifier(string,CapitalizationBehaviour)`.
- Improvement: Improved the robustness of `.GetPackage()` to more accurately detect "root" `java` folders.

### Version 3.3.14

- Improvement: Improved the logic of the `JavaResolvedTypeInfo.Create` method.
