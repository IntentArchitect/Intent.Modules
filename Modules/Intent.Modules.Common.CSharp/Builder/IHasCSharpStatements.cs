using System;
using System.Collections.Generic;
using System.Linq;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Builder;

public interface IHasCSharpStatements
{
    IList<CSharpStatement> Statements { get; }
}

public static class HasCSharpStatementsExtensions
{
    public static CSharpStatement FindStatement(this IHasCSharpStatements parent, Func<CSharpStatement, bool> matchFunc)
    {
        return parent.FindStatement<CSharpStatement>(matchFunc);
    }

    public static IEnumerable<CSharpStatement> FindStatements(this IHasCSharpStatements parent, Func<CSharpStatement, bool> matchFunc)
    {
        return parent.FindStatements<CSharpStatement>(matchFunc);
    }

    public static T FindStatement<T>(this IHasCSharpStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements(matchFunc))
            .Concat(parent.Statements.OfType<T>().Where(matchFunc))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(this IHasCSharpStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(this TParent parent, string statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpStatement(statement), configure);
    }

    [Obsolete("Use TParent AddStatement<TParent, TStatement> method")]
    [FixFor_Version4(
        "Remove this overload as 'public static TParent AddStatement<TParent, TStatement>(this TParent parent, TStatement statement, Action<TStatement> configure = null)' can be used instead")]
    public static TParent AddStatement<TParent>(this TParent parent, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddStatement<TParent, TStatement>(this TParent parent, TStatement statement, Action<TStatement> configure = null)
        where TParent : IHasCSharpStatements
        where TStatement : CSharpStatement
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddFieldAssignmentStatement<TParent>(this TParent parent, string lhs, string rhs, Action<CSharpFieldAssignmentStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(lhs, rhs), configure);
    }

    public static TParent AddInvocationStatement<TParent>(this TParent parent, string invocation, Action<CSharpInvocationStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(invocation), configure);
    }

    public static TParent AddLambdaBlock<TParent>(this TParent parent, string invocation, Action<CSharpLambdaBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(invocation), configure);
    }

    public static TParent AddMethodChainStatement<TParent>(this TParent parent, string initialInvocation, Action<CSharpMethodChainStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(initialInvocation), configure);
    }

    public static TParent AddObjectInitializerBlock<TParent>(this TParent parent, string invocation, Action<CSharpObjectInitializerBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(invocation), configure);
    }

    public static TParent AddObjectInitStatement<TParent>(this TParent parent, string lhs, CSharpStatement rhs, Action<CSharpObjectInitStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(lhs, rhs), configure);
    }

    public static TParent AddUsingBlock<TParent>(this TParent parent, string expression,
        Action<CSharpUsingBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    /// <summary>
    /// Adds an if block to the <paramref name="parent"/>.
    /// <code>
    /// if (expression)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddIfStatement<TParent>(this TParent parent, string expression, Action<CSharpIfStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    /// <summary>
    /// Adds an else if block to the <paramref name="parent"/>.
    /// <code>
    /// else if (expression)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddElseIfStatement<TParent>(this TParent parent, string expression, Action<CSharpElseIfStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    /// <summary>
    /// Adds an else block to the <paramref name="parent"/>.
    /// <code>
    /// else
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddElseStatement<TParent>(this TParent parent, Action<CSharpElseStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(), configure);
    }

    /// <summary>
    /// Adds a foreach block to the <paramref name="parent"/>.
    /// <code>
    /// foreach (var iterationVariable in sourceCollection)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddForEachStatement<TParent>(this TParent parent,
        string iterationVariable, string sourceCollection,
        Action<CSharpForEachStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(iterationVariable, sourceCollection), configure);
    }

    /// <summary>
    /// Adds a while block to the <paramref name="parent"/>.
    /// <code>
    /// while (true)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddWhileStatement<TParent>(this TParent parent,
        string expression,
        Action<CSharpWhileStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    /// <summary>
    /// Adds a try block to the <paramref name="parent"/>.
    /// <code>
    /// try
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddTryBlock<TParent>(this TParent parent, Action<CSharpTryBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(), configure);
    }

    /// <summary>
    /// Adds a catch block to the <paramref name="parent"/>.
    /// <code>
    /// catch
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddCatchBlock<TParent>(this TParent parent, Action<CSharpCatchBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(), configure);
    }

    /// <summary>
    /// Adds a catch block to the <paramref name="parent"/>.
    /// <code>
    /// catch (&lt;<paramref name="exceptionType"/>&gt; &lt;<paramref name="parameterType"/>&gt;)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddCatchBlock<TParent>(this TParent parent, string exceptionType, string parameterType, Action<CSharpCatchBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(exceptionType, parameterType), configure);
    }

    /// <summary>
    /// Adds a finally block to the <paramref name="parent"/>.
    /// <code>
    /// finally
    /// {
    ///     ...
    /// }
    /// </code>
    /// </summary>
    public static TParent AddFinallyBlock<TParent>(this TParent parent, Action<CSharpFinallyBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(), configure);
    }

    public static TParent AddSwitchStatement<TParent>(this TParent parent, string expression, Action<CSharpSwitchStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    public static TParent AddStatementBlock<TParent>(this TParent parent, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(), configure);
    }

    public static TParent AddStatementBlock<TParent>(this TParent parent, string expression, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new(expression), configure);
    }

    public static TParent InsertStatement<TParent>(this TParent parent, int index, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(this TParent parent, int index, IReadOnlyCollection<CSharpStatement> statements,
        Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        foreach (var s in statements.Reverse())
        {
            parent.Statements.Insert(index, s);
            s.Parent = parent;
        }

        configure?.Invoke(statements);
        return parent;
    }

    public static TParent AddStatements<TParent>(this TParent parent, string statements, Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatements(statements.ConvertToStatements(), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            parent.Statements.Add(statement);
            statement.Parent = parent;
        }

        configure?.Invoke(arrayed);

        return parent;
    }

    public static TParent AddLocalMethod<TParent>(this TParent parent, string returnType, string name, Action<CSharpLocalMethod> configure = null)
        where TParent : IHasCSharpStatements
    {
        if (parent is not CSharpMetadataBase metadataBase)
        {
            throw new Exception($"{parent?.GetType()} is not {nameof(CSharpMetadataBase)}");
        }

        var localMethodStatement = new CSharpLocalMethod(returnType, name, metadataBase.File);
        parent.AddStatement(localMethodStatement);
        configure?.Invoke(localMethodStatement);

        return parent;
    }

    public static TParent AddConditionalExpression<TParent>(this TParent parent, CSharpStatement condition, CSharpStatement whenTrue, CSharpStatement whenFalse, Action<CSharpConditionalExpressionStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        var statement = new CSharpConditionalExpressionStatement(condition, whenTrue, whenFalse);
        parent.AddStatement(statement);
        configure?.Invoke(statement);
        return parent;
    }
    
    public static TParent AddReturn<TParent>(this TParent parent, CSharpStatement returnStatement, Action<CSharpReturnStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        var statement = new CSharpReturnStatement(returnStatement);
        parent.AddStatement(statement);
        configure?.Invoke(statement);
            return parent;
    }
    
    public static TParent AddNotImplementedException<TParent>(this TParent parent, Action<CSharpStatement> configure = null)
        where TParent : CSharpMetadataBase, IHasCSharpStatements
    {
        parent.File.AddUsing("System");
        var exceptionStatement = new CSharpStatement("throw new NotImplementedException();").AddMetadata("exception", "NotImplementedException");
        parent.AddStatement(exceptionStatement);
        configure?.Invoke(exceptionStatement);

        return parent;
    }
}