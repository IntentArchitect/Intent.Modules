#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using Intent.SdkEvolutionHelpers;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// <see cref="IHasCSharpStatements"/> can't be updated for backwards compatibility reasons, so
/// this was created for interfaces which needed to be "clean" of concretes.
/// </summary>
public interface IHasCSharpStatementsActual
{
    /// <summary>
    /// Can be used by statement specializations to know whether they should automatically add a semicolon at their end.
    /// </summary>
    bool IsCodeBlock => false;
    IList<ICSharpStatement> Statements { get; }
}

public static class HasCSharpStatementsActualExtensions
{
    public static ICSharpStatement FindStatement(this IHasCSharpStatementsActual parent, Func<ICSharpStatement, bool> matchFunc)
    {
        return parent.FindStatement<ICSharpStatement>(matchFunc);
    }

    public static IEnumerable<ICSharpStatement> FindStatements(this IHasCSharpStatementsActual parent, Func<ICSharpStatement, bool> matchFunc)
    {
        return parent.FindStatements<ICSharpStatement>(matchFunc);
    }

    public static T FindStatement<T>(this IHasCSharpStatementsActual parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<IHasCSharpStatementsActual>().SelectMany(x => x.FindStatements(matchFunc)) 
            .Concat(parent.Statements.OfType<T>().Where(matchFunc))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(this IHasCSharpStatementsActual parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasCSharpStatementsActual>().SelectMany(x => x.FindStatements(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(this TParent parent, string statement, Action<ICSharpStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement<TParent, ICSharpStatement>(new CSharpStatement(statement), configure);
    }

    /// <summary>
    /// Obsolete. Use <see cref="AddStatement{TParent,TStatement}(TParent,TStatement,Action{TStatement})"/>
    /// </summary>
    [Obsolete]
    [FixFor_Version4("Remove this overload as 'public static TParent AddStatement<TParent, TStatement>(this TParent parent, TStatement statement, Action<TStatement> configure = null)' can be used instead")]
    public static TParent AddStatement<TParent>(this TParent parent, ICSharpStatement statement, Action<ICSharpStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddStatement<TParent, TStatement>(this TParent parent, TStatement statement, Action<TStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
        where TStatement : ICSharpStatement
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddFieldAssignmentStatement<TParent>(this TParent parent, string lhs, string rhs, Action<CSharpFieldAssignmentStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(lhs, rhs), configure);
    }

    public static TParent AddInvocationStatement<TParent>(this TParent parent, string invocation, Action<CSharpInvocationStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(invocation), configure);
    }

    public static TParent AddLambdaBlock<TParent>(this TParent parent, string invocation, Action<CSharpLambdaBlock> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(invocation), configure);
    }

    /// <summary>
    /// Use <see cref="HasCSharpStatementsExtensions.AddInvocation"/> instead.
    /// </summary>
    [Obsolete]
    public static TParent AddMethodChainStatement<TParent>(this TParent parent, string initialInvocation, Action<CSharpMethodChainStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(initialInvocation), configure);
    }

    public static TParent AddObjectInitializerBlock<TParent>(this TParent parent, string invocation, Action<CSharpObjectInitializerBlock> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(invocation), configure);
    }

    public static TParent AddObjectInitStatement<TParent>(this TParent parent, string lhs, CSharpStatement rhs, Action<CSharpObjectInitStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(lhs, rhs), configure);
    }

    public static TParent AddUsingBlock<TParent>(this TParent parent, string expression,
        Action<CSharpUsingBlock> configure = null)
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new (iterationVariable, sourceCollection), configure);
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
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new (expression), configure);
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(), configure);
    }

    public static TParent AddSwitchStatement<TParent>(this TParent parent, string expression, Action<CSharpSwitchStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(expression), configure);
    }

    public static TParent AddStatementBlock<TParent>(this TParent parent, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(), configure);
    }

    public static TParent AddStatementBlock<TParent>(this TParent parent, string expression, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatement(new(expression), configure);
    }

    public static TParent InsertStatement<TParent>(this TParent parent, int index, ICSharpStatement statement, Action<ICSharpStatement> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(this TParent parent, int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        foreach (var s in statements.Reverse())
        {
            parent.Statements.Insert(index, s);
            s.Parent = parent;
        }
        configure?.Invoke(statements);
        return parent;
    }

    public static TParent AddStatements<TParent>(this TParent parent, string statements, Action<IEnumerable<ICSharpStatement>> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatements(statements.ConvertToStatements(), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>> configure = null)
        where TParent : IHasCSharpStatementsActual
    {
        return parent.AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>> configure = null)
        where TParent : IHasCSharpStatementsActual
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
        where TParent : IHasCSharpStatementsActual
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
}