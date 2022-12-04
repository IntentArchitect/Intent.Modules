### Version 3.3.17

- Added `ApplicationPropertyRequiredEvent` and convenience `.ApplyApplicationProperty(...)` extension method for setting default application properties.

### Version 3.3.16

- Improved the logic of `.ToJavaIdentifier()` to exhaustively conform to the Java identifier specification. For more information review the XML documentation comments for `Intent.Modules.Common.Java.Templates.JavaIdentifierExtensionMethods.ToJavaIdentifier(string,CapitalizationBehaviour)`.
- Improved the robustness of `.GetPackage()` to more accurately detect "root" `java` folders.

### Version 3.3.14

- Improved the logic of the `JavaResolvedTypeInfo.Create` method.
