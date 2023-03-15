namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpPropertyAccessor
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

    public CSharpPropertyAccessor Public()
    {
        AccessModifier = "public ";
        return this;
    }

    public CSharpPropertyAccessor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }

    public CSharpPropertyAccessor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpPropertyAccessor Init()
    {
        AccessModifier = "init";
        return this;
    }

    public CSharpPropertyAccessor WithImplementation(params string[] statements)
    {
        Implementation = new CSharpXmlComments(statements);
        IsExpression = statements.Length == 1 && !statements[0].Contains("return");
        return this;
    }

    public CSharpPropertyAccessor WithBodyImplementation(params string[] statements)
    {
        Implementation = new CSharpXmlComments(statements);
        IsExpression = false;
        return this;
    }

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
            return $"{indentation}{Accessor} => {Implementation};";
        }
        else if (Implementation.Statements.Count == 1)
        {
            return $"{indentation}{Accessor} {{ {Implementation} }}";
        }
        else
        {
            return @$"{indentation}{Accessor}
{indentation}{{ 
{Implementation.ToString($"{indentation}    ")} 
{indentation}}}";
        }
    }
}