using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavadocComments : JavaMetadataBase<JavadocComments>
{
    public readonly List<string> Statements = new();

    public JavadocComments(params string[] statements)
    {
        Statements.AddRange(statements);
    }

    public JavadocComments AddStatements(string statements)
    {
        Statements.AddRange(statements.Trim().Replace("\r\n", "\n").Split("\n"));
        return this;
    }

    public JavadocComments AddStatements(IEnumerable<string> statements)
    {
        Statements.AddRange(statements);
        return this;
    }

    public bool IsEmpty() => !Statements.Any();

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        if (!Statements.Any())
        {
            return string.Empty;
        }

        return @$"{indentation}/**
{indentation} * {string.Join($@"
{indentation} * ", Statements)}
{indentation} */";
    }
}