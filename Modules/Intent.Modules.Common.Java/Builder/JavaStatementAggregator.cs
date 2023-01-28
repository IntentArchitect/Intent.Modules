using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Builder;

public class JavaStatementAggregator
{
    private readonly List<JavaStatement> _statements = new();

    public JavaStatementAggregator Add(JavaStatement statement, Action<JavaStatement> configure = null)
    {
        _statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public JavaStatementAggregator AddRange(IEnumerable<JavaStatement> statements, Action<JavaStatement> configure = null)
    {
        foreach (var statement in statements)
        {
            Add(statement, configure);
        }
        return this;
    }

    public IList<JavaStatement> ToList()
    {
        return _statements;
    }
}