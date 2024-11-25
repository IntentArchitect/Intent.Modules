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

    The constructor initializer is declared on a separate line as the constructor declaration:

    ``` csharp
        public class TypeName
        {
            public TypeName() 
                : base
            {
            }
        }
    ```

- **Depends on length** :
    The constructor initializer is declared on a separate line as the constructor declaration, only if the length of the _constructor declaration + constructor initializer_ is longer than a predefined length (currently set to 110 characters). If the length is less than this, then the _constructor initializer_ is declared on the same line.

### Parameter Multi-line Spanning

This setting relates to [Style Cop SA1116](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1116.md) and allows the user to determine parameters are declared across multiple lines (or not)

- **Same line** :
    The generated code will be output on the same line. This could result in a long list of parameters on a single line:

    ``` csharp
        // this is valid
        public class TypeName
        {
            public TypeName(string name, string name1, string name2, string name3, string name4, string name5)
            {
            }
        }
    ```

- **Default** :

    This will revert the styling back to the default template behavior.  
    **For constructors**: this means if there are multiple parameters and if the line length exceeds a certain value, each parameter will be output on a new line, except for the first parameter:

    ``` csharp
 
    public class TypeName
    {
        // single parameter output
        public TypeName(string name)
        {
        }

        // multi parameter output
        public TypeName(string name,
            string name2,
            string name3)
        {
        }
    }
    ```
  
    **For methods**: this means if there are multiple parameters and if the line length exceeds a certain value, each parameter will be output on a new line:

    ``` csharp
    public class TypeName
    {
        // single parameter output
        public SingleParamMethod(string name)
        {
        }

        // multi parameter output
        public MultiParamMethod(
            string name,
            string name2,
            string name3)
        {
        }
    }
    ```

    **For interfaces**:, this means all parameters on a single line:

    ``` csharp
    public interface IEFRepository<TDomain, TPersistence> : IRepository<TDomain>
    {
        Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    }
    ```

- **Depends on length** :
    In this mode, only if the line length exceeds a certain value, will each parameter be output on a new line:

    ``` csharp
 
    public class TypeName
    {
        // short parameter length output
        public TypeName(string name, string name2)
        {
        }

        // long parameter length output
        public TypeName(
            string name,
            string name2,
            string name3,
            string name4,
            string name5)
        {
        }
    }
    ```
