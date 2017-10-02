---
uid: RoslynWeaver_Appendix
---
# Roslyn Weaver 

## Appendix

### List of Roslyn Weave Code Elements
- Classes
- Constructors
- Enums
- Fields
- Interfaces
- Methods
- Properties

### Roslyn Weave Modes
|Mode|Description|
|-|-|
|Fully|The generated code element will be used.|
|Ignore|The non-generated element code will be used.|
|Merge|The code elements from both sources will be merged together.|

### Code Element mode specifics

|Code Element|Signature|Body
|-|-|-|
|Class|The class header i.e. every thing before the first {.| The class body i.e. every thing after the first {.|
|Constructor|The signature of the constructor, i.e. every thing before the first {.|The body of the constructor, i.e. every thing after the first {|
|Enum|The enum header i.e. every thing before the first {.| The enum body i.e. every thing after the first {.|
|Field|The field declaration.|N/A|
|Interface|The interface header i.e. every thing before the first {.| The interface body i.e. every thing after the first {.|
|Method|The signature of the method, i.e. every thing before the first {.|The body of the method, i.e. every thing after the first {|
|Property|The signature of the property, i.e. every thing before the first {.|The body of the property, i.e. every thing after the first {|