# Intent.Modules.Common.CSharp

## C# Style Settings

There are a number of configuration settings available on the application _Settings_ screen which allow one to change the style of the output C# code.

### Constructor Initializer

This setting relates to [Style Cop SA1128](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1128.md) and allows the user to determine how a _constructor initializer_ is declared.

- **Same Line** :

    The constructor initializer is declared on the same line as the constructor declaration:

    ``` csharp
        public class TypeName
        {
            public TypeName() : base
            {
            }
        }
    ```

- **New Line** :

    The constructor initializer is declared on a seperate line as the constructor declaration:

    ``` csharp
        public class TypeName
        {
            public TypeName() 
                : base
            {
            }
        }
    ```

- **Mixed** :
    The constructor initializer is declared on a seperate line as the constructor declaration, only if the lengeth of the _constructor declaration + constructor initializer_ is longer than a predefined length (currently set to 110 characters). If the lengeth is less than this, then the _constructor initializer_ is declared on the same line.
