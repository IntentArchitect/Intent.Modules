#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpPropertyAccessor : ICSharpPropertyAccessor
{
    public string Accessor { get; }
    public string AccessModifier { get; private set; } = "";
    public bool IsExpression { get; private set; }

    public CSharpXmlComments Implementation { get; private set; } = new(); // TODO JL: This doesn't look correct

    private CSharpPropertyAccessor(string accessor)
    {
        Accessor = accessor;
    }

    public static CSharpPropertyAccessor Getter()
    {
        return new CSharpPropertyAccessor("get");
    }

    public static CSharpPropertyAccessor Setter()
    {
        return new CSharpPropertyAccessor("set");
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.Internal() => Internal();
    public CSharpPropertyAccessor Internal()
    {
        AccessModifier = "internal ";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.Private() => Private();
    public CSharpPropertyAccessor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.Protected() => Protected();
    public CSharpPropertyAccessor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.ProtectedInternal() => ProtectedInternal();
    public CSharpPropertyAccessor ProtectedInternal()
    {
        AccessModifier = "protected internal ";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.Public() => Public();
    public CSharpPropertyAccessor Public()
    {
        AccessModifier = "public ";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.Init() => Init();
    public CSharpPropertyAccessor Init()
    {
        AccessModifier = "init";
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.WithImplementation(params string[] statements) => WithImplementation(statements);
    public CSharpPropertyAccessor WithImplementation(params string[] statements)
    {
        Implementation = new CSharpXmlComments(statements);
        IsExpression = statements.Length == 1 && !statements[0].Contains("return");
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.WithBodyImplementation(params string[] statements) => WithBodyImplementation(statements);
    public CSharpPropertyAccessor WithBodyImplementation(params string[] statements)
    {
        Implementation = new CSharpXmlComments(statements);
        IsExpression = false;
        return this;
    }

    ICSharpPropertyAccessor ICSharpPropertyAccessor.WithExpressionImplementation(string implementation) => WithExpressionImplementation(implementation);
    public CSharpPropertyAccessor WithExpressionImplementation(string implementation)
    {
        Implementation = new CSharpXmlComments(implementation);
        IsExpression = true;
        return this;
    }

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        if (AccessModifier == "init")
        {
            return "init;";
        }

        if (Implementation.IsEmpty())
        {
            return $@"{AccessModifier}{Accessor};";
        }

        if (IsExpression)
        {
            return $"{indentation}{AccessModifier}{Accessor} => {Implementation};";
        }
        
        if (Implementation.Statements.Count == 1)
        {
            return $"{indentation}{AccessModifier}{Accessor} {{ {Implementation} }}";
        }

        return @$"{indentation}{AccessModifier}{Accessor}
{indentation}{{ 
{Implementation.ToString($"{indentation}    ")} 
{indentation}}}";
    }
}