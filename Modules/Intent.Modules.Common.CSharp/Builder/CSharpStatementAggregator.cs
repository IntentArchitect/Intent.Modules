using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatementAggregator
{
    private readonly List<CSharpStatement> _statements = new();

    public CSharpStatementAggregator Add(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        _statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public CSharpStatementAggregator AddRange(IEnumerable<CSharpStatement> statements, Action<CSharpStatement> configure = null)
    {
        foreach (var statement in statements)
        {
            Add(statement, configure);
        }
        return this;
    }

    public IList<CSharpStatement> ToList()
    {
        return _statements;
    }
}