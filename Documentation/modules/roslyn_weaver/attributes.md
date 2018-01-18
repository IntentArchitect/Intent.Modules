---
uid: RoslynWeaver_Attributes
---
# Roslyn Weaver 

## Roslyn Weaver Attributes

Full list of Attributes
- [IntentTemplate](#intenttemplate-attribute)
- [IntentManaged](#intentmanaged-attribute)
- [DefaultIntentManaged](#defaultintentmanaged-attribute)
- [IntentInitialGen](#intentinitialgen-attribute)

## IntentTemplate Attribute

### Overview
This attribute is added by the Roslyn Weaver automatically to all generated code, this attribute is used for.
- Determining which template, and version of the template, produced the solution code
- Code Migrations

### Usage
This attribute is managed by the Roslyn Weaver which introduces it to the generated output based on the executing template. This attribute should not be used or introduced to a file directly by the developer, as it is managed by the Roslyn Weaver. This attribute is an assembly level attribute, as it applies to the file rather than any specific artifact within the file. 

### Properties

|Name|Description|
|----|-----------|
|Id|Corresponds to the Template Id of the template which produced this code file.|
|Version|Corresponds to the version of template  which produced this code file.|

### Example

A generated file, using a template with Id : *MyFirstTemplate* would contain the following code.

```csharp
...
using Intent.CodeGen;

[assembly: IntentTemplate("MyFirstTemplate", Version = "1.0")]

...
```

## IntentManaged Attribute

### Overview
This attribute can be added to a code element to control how the Roslyn Weaver merges the code. The merge behaviour is determined by the [mode](Modes.md) which is specified.

### Usage
This attriibute can be introduced into generated code to alter the behaviour of the Roslyn Weaver. The typical scenario would be to allow you to introduce custom code into a generated code file. 

This attribute is typically used by developers to modify the template output, it typically should not be used in templates themselves, templates should typically using [DefaultIntentManaged](#defaultintentmanaged-attribute) if they want to control the Roslyn Weavers behaviour.

This attribute is will override [DefaultIntentManaged](#defaultintentmanaged-attribute) if both are targeting the same code element.

This attribute can be applied to any of the following.
- Class 
- Constructor
- Enum
- Field
- Interface
- Method
- Property

### Properties
|Name|Description|
|----|-----------|
|ElementMode|Specified the mode under which the code element should be merged|
|Body|Specified the mode under which the body of the code element should be merged. This is only required if the mode for the Body differs from the element mode.|
|Signature|Specified the mode under which the signature of the code element should be merged. This is only required if the mode for the Signature differs from the element mode|

### Examples

#### Using Ignore mode to change or add functionality

In the code below a *ToString* could be newly introduced or changed from the version which was being generated. Either way the method is now in the developers control.

Generated Code File

```csharp
        ...

        [IntentManaged(Mode.Ignore)]
        public override string ToString()
        {
            return this.Id;
        }
        ...
```

#### Fine tuning the mode

The code example below is similar to the one above, how ever in this example the method signature is running fully under code gen while the body of the method is under the developers control.

Generated Code File

```csharp
        ...

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string ToString()
        {
            return this.Id;
        }
        ...
```


## DefaultIntentManaged Attribute

### Overview
This attribute can be used to specify the default mode for 1 or more code elements. 

### Usage

The attribute is scoped based on were it is declared, i.e. if it is declared at an assembly level it's scope is the entire file, if it is declared on a class or interface it's scope is that class or interface.

The code elements the attribute applies to work as follows.
- If *Targets* is specified the mode is applied to all code elements of the listed targets. e.g. Targets.Properties | Targets.Methods would apply the mode to all properties and methods within it's scope.
- If *AccessModifiers* is specified the mode is applied to all code elements with any of the listed access modifiers. e.g. AccessModifier.Public would apply the mode to all public code elements within it's scope.
- If both *Targets* and *AccessModifiers* are specified the mode will apply to any code elements which match any of the target AND any of the access modifiers. e.g. AccessModifier.Public and Targets.Properties | Targets.Methods would be all public methods and all public properties.
- If neither *Targets* or *AccessModifiers* is specified the mode will be applied to all code elements within the attributes scope.

This attribute can be applied to any of the following.
- Assembly
- Class 
- Interface

### Properties
|Name|Description|
|----|-----------|
|Element Mode|Specified the mode under which the targeted code element should be merged|
|Body|Specified the mode under which the body of the targeted code element should be merged. This is only required if the mode for the body differs from the element mode.|
|Signature|Specified the mode under which the signature of the targeted code element should be merged. This is only required if the mode for the signature differs from the element mode.|
|Targets|Allows for targeting specific code elements which the mode must apply to. Multiple targets can be specified using the \| syntax. The valid targets are Classes, Constructors, Enums, Fields, Interfaces, Methods, Namespaces, Properties and Usings |
|AccessModifiers|This property can be used to target code elements based on access modifiers. Multiple access modifiers can be specified using the \| syntax. The valid access modifiers are internal, private, protected and public. |

### Examples

#### Letting the code gen explicitly control all public members of a class

TODO

Template Code File
```csharp
        ...

        [DefaultIntentManaged(Mode.Fully, Body = Mode.Ignore, Targets = Targets.Methods | Targets.Properties, AccessModifiers = AccessModifiers.Public)]
        public MyClass()
        {
            ...
        }
        ...
```

## IntentInitialGen Attribute

### Overview
This attribute can be used by developers creating templates to indicate code elements which are generated *once off* when the template is initially run. These code elements are typically stubs or default implementations for developers to complete or enrich. From the Roslyn Weavers perspective this code is seen as manual code, it just happens to get added in by the template as opposed to a developer. 

### Usage
This attribute should only be used in templates and does not make sense to exist in generated code. The Roslyn Weaver will remove these attributes from the generated code.

Any templates using this attribute should be designed in such a way that the code targeted by these attributes is in a managed mode of *Ignore* or *Merge*. If the code element is under *fully* mode it will be removed on a subsequent execution of the template.

This attribute can be applied to any of the following.
- Constructor
- Field
- Property
- Method

### Properties
N/A

### Examples

#### Adding an initial constructor
The code below would add a default constructor to the class, with a compile time warning indicating that the constructor needs to be implemented.

Template Code File
```csharp
        ...

        [IntentInitialGen]
        public MyClass()
        {
            #warning please implement 1 or more meaningful non-anemic constructors 
        }
        ...
```

Generated Code File
```csharp
        ...

        public MyClass()
        {
            #warning please implement 1 or more meaningful non-anemic constructors 
        }
        ...
```

Note the generated code is not seem as part of the template, hance if it is running under *Fully* managed mode, the code will be removed on a subsequent run.