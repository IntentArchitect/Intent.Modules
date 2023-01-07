using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptComments : TypescriptMetadataBase<TypescriptComments>
{
    public readonly List<string> Statements = new();

    public TypescriptComments(params string[] statements)
    {
        Statements.AddRange(statements);
    }

    public TypescriptComments AddStatements(string statements)
    {
        Statements.AddRange(statements.Trim().Replace("\r\n", "\n").Split("\n"));
        return this;
    }

    public TypescriptComments AddStatements(IEnumerable<string> statements)
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
        return $@"{(Statements.Any() ? $@"{indentation}{string.Join($@"
{indentation}", Statements)}" : string.Empty)}";
    }
}