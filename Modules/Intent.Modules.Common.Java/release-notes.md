### Version 3.4.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.4.1

- Update: JavaComments now properly supports the /* */ format.
- Update: Free Code Blocks now supported on Interfaces and Classes for cases where only freetext needs to be added a Class / Interface.
- Fixed: Interface generated wrong code for extending other interfaces.

### Version 3.4.0

- New: Introducing the Java File Builder Pattern.

### Version 3.3.18

- Fixed: Code management would not apply changes to to generic type parameters when merging.

### Version 3.3.17

- Added `ApplicationPropertyRequiredEvent` and convenience `.ApplyApplicationProperty(...)` extension method for setting default application properties.

### Version 3.3.16

- Improved the logic of `.ToJavaIdentifier()` to exhaustively conform to the Java identifier specification. For more information review the XML documentation comments for `Intent.Modules.Common.Java.Templates.JavaIdentifierExtensionMethods.ToJavaIdentifier(string,CapitalizationBehaviour)`.
- Improved the robustness of `.GetPackage()` to more accurately detect "root" `java` folders.

### Version 3.3.14

- Improved the logic of the `JavaResolvedTypeInfo.Create` method.
