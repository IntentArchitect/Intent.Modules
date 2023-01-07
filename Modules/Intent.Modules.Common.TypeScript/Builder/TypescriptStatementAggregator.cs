using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptStatementAggregator
{
    private readonly List<TypescriptStatement> _statements = new();

    public TypescriptStatementAggregator Add(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        _statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptStatementAggregator AddRange(IEnumerable<TypescriptStatement> statements, Action<TypescriptStatement> configure = null)
    {
        foreach (var statement in statements)
        {
            Add(statement, configure);
        }
        return this;
    }

    public IList<TypescriptStatement> ToList()
    {
        return _statements;
    }
}