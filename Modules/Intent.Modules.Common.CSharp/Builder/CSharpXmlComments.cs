using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpXmlComments : CSharpMetadataBase<CSharpXmlComments>
{
    public readonly IList<string> Statements = new List<string>();

    public CSharpXmlComments(params string[] statements)
    {
        ((List<string>)Statements).AddRange(statements);
    }

    public CSharpXmlComments AddStatements(string statements)
    {
        ((List<string>)Statements).AddRange(statements.Trim().Replace("\r\n", "\n").Split("\n"));
        return this;
    }

    public CSharpXmlComments AddStatements(IEnumerable<string> statements)
    {
        ((List<string>)Statements).AddRange(statements);
        return this;
    }

    public bool IsEmpty() => !Statements.Any();

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{(Statements.Any() ? $@"{indentation}{string.Join($@"
{indentation}", Statements)}" : string.Empty)}";
    }
}

public class CSharpMetadataBase<TCSharp>
    where TCSharp : CSharpMetadataBase<TCSharp>
{
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    public TCSharp AddMetadata<T>(string key, T value)
    {
        Metadata.Add(key, value);
        return (TCSharp)this;
    }

    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (Metadata.TryGetValue(key, out var valueFound) && valueFound is T castValue)
        {
            value = castValue;
            return true;
        }

        value = default(T);
        return false;
    }

    public bool TryGetMetadata(string key, out object value)
    {
        if (Metadata.TryGetValue(key, out var valueFound))
        {
            value = valueFound;
            return true;
        }

        value = null;
        return false;
    }
}