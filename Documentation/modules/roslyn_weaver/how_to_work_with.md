# Roslyn Weaver 

## Working with Roslyn Weaver templates

Roslyn Weaver provides several code based attributes ([Roslyn Weaver attributes](attributes.md)) which can used to control or change the code weaving process. As a developer working with the generated output, you will find that while the generated output is typically what you want occasionally you may need to change or extend the output for exceptional circumstances. Note if you are doing this often you should consider upgrading or changing the templates themselves. 

Modifying the solution code file involves introducing, or changing existing, IntentManged attributes to fine tune the Roslyn Weavers behaviour to respect the non-generated code. Below are several examples of doing this.

### Turning off the code generation

Assuming we have a generated solution code file, as per the code example below.

```csharp
...
using System;

[assembly: IntentTemplate("MyFirstTemplate", Version = "1.0")]

public class MyGeneratedClass
{
    ...
}
```

We could introduce the [DefaultIntentManaged](xref:RoslynWeaver_Attributes#defaultintentmanaged-attribute) attribute to turn of the code generation for this specific file.

```csharp
...
using System;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("MyFirstTemplate", Version = "1.0")]

public class MyGeneratedClass
{
    ...
}
```
Note this does not technically turn off the code generation is rather instructing the Roslyn Weaver to ignore all the code elements by default. 

### Changing a classes signature

Assuming we want to change a classes signature, e.g. Introduce a new attribute or an interface, given we have a generated output as below. 

```csharp
...
using System;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("MyFirstTemplate", Version = "1.0")]

public class MyGeneratedClass
{
    ...
}
```

There are several ways we could achieve this, adding any one of the IntentManaged attributes listed below to the class would allow you to modify the signature. 

```csharp
...
using System;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("MyFirstTemplate", Version = "1.0")]

[IntentManaged(Mode.Merge, Signature=Mode.Ignore, Body=Mode.Fully)] // OR
[IntentManaged(Mode.Merge, Signature=Mode.Merge, Body=Mode.Fully)] // OR
[IntentManaged(Mode.Merge, Signature=Mode.Merge, Body=Mode.Merge)] // OR
//This is equivalent to the line above
[IntentManaged(Mode.Merge)] this is equivalent to the line above

[MyManualAttribute]
public class MyGeneratedClass : MyManualBaseClass, IMyManualInterface
{
    ...
}
```

### Changing a method implementation

Assuming we want to change the implementation of a generated method, looking at the example below. 


```csharp
...
public class MyGeneratedClass 
{
    public void MyMethod(string arg1)
    {

    }
}
```

We can modify the method using any of the IntentManaged attributes below.

```csharp
...
public class MyGeneratedClass 
{
    [IntentManaged(Mode.Ignore)] // OR
    [IntentManaged(Mode.Merge, Signature=Mode.Fully, Body=Mode.Ignore)]
    public void MyMethod(string arg1)
    {
        //My Implementation
    }
}
```