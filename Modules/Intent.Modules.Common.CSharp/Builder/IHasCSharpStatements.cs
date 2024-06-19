#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Builder;

public interface IHasCSharpStatements : IHasCSharpStatementsActual
{
    new IList<CSharpStatement> Statements { get; }
    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => new WrappedList<CSharpStatement, ICSharpStatement>(Statements);
}

public static class HasCSharpStatementsExtensions
{
    public static CSharpStatement FindStatement(IHasCSharpStatements parent, Func<CSharpStatement, bool> matchFunc)
    {
        return parent.FindStatement(matchFunc);
    }

    public static IEnumerable<CSharpStatement> FindStatements(IHasCSharpStatements parent, Func<CSharpStatement, bool> matchFunc)
    {
        return parent.FindStatements(matchFunc);
    }

    public static T FindStatement<T>(IHasCSharpStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements(matchFunc)) 
            .Concat(parent.Statements.OfType<T>().Where(matchFunc))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(IHasCSharpStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(TParent parent, string statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpStatement(statement), configure);
    }

    [Obsolete("Use TParent AddStatement<TParent, TStatement> method")]
    [FixFor_Version4("Remove overload as 'public static TParent AddStatement<TParent, TStatement>(TParent parent, TStatement statement, Action<TStatement> configure = null)' can be used instead")]
    public static TParent AddStatement<TParent>(TParent parent, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddStatement<TParent, TStatement>(TParent parent, TStatement statement, Action<TStatement> configure = null)
        where TParent : IHasCSharpStatements
        where TStatement : CSharpStatement
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent AddFieldAssignmentStatement<TParent>(TParent parent, string lhs, string rhs, Action<CSharpFieldAssignmentStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpFieldAssignmentStatement(lhs, rhs), x => configure?.Invoke(x));
    }

    public static TParent AddInvocationStatement<TParent>(TParent parent, string invocation, Action<CSharpInvocationStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpInvocationStatement(invocation), x => configure?.Invoke(x));
    }

    public static TParent AddLambdaBlock<TParent>(TParent parent, string invocation, Action<CSharpLambdaBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpLambdaBlock(invocation), x => configure?.Invoke(x));
    }

    public static TParent AddMethodChainStatement<TParent>(TParent parent, string initialInvocation, Action<CSharpMethodChainStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpMethodChainStatement(initialInvocation), x => configure?.Invoke(x));
    }

    public static TParent AddObjectInitializerBlock<TParent>(TParent parent, string invocation, Action<CSharpObjectInitializerBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpObjectInitializerBlock(invocation), x => configure?.Invoke(x));
    }

    public static TParent AddObjectInitStatement<TParent>(TParent parent, string lhs, CSharpStatement rhs, Action<CSharpObjectInitStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpObjectInitStatement(lhs, rhs), x => configure?.Invoke(x));
    }

    public static TParent AddUsingBlock<TParent>(TParent parent, string expression, Action<CSharpUsingBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpUsingBlock(expression), x => configure?.Invoke(x));
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
    public static TParent AddIfStatement<TParent>(TParent parent, string expression, Action<CSharpIfStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpIfStatement(expression), x => configure?.Invoke(x));
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
    public static TParent AddElseIfStatement<TParent>(TParent parent, string expression, Action<CSharpElseIfStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpElseIfStatement(expression), x => configure?.Invoke(x));
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
    public static TParent AddElseStatement<TParent>(TParent parent, Action<CSharpElseStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpElseStatement(), x => configure?.Invoke(x));
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
    public static TParent AddForEachStatement<TParent>(TParent parent,
        string iterationVariable, string sourceCollection,
        Action<CSharpForEachStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpForEachStatement(iterationVariable, sourceCollection), x => configure?.Invoke(x));
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
    public static TParent AddWhileStatement<TParent>(TParent parent, string expression, Action<CSharpWhileStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpWhileStatement(expression), x => configure?.Invoke(x));
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
    public static TParent AddTryBlock<TParent>(TParent parent, Action<CSharpTryBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpTryBlock(), x => configure?.Invoke(x));
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
    public static TParent AddCatchBlock<TParent>(TParent parent, Action<CSharpCatchBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpCatchBlock(), x => configure?.Invoke(x));
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
    public static TParent AddCatchBlock<TParent>(TParent parent, string exceptionType, string parameterType, Action<CSharpCatchBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpCatchBlock(exceptionType, parameterType), x => configure?.Invoke(x));
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
    public static TParent AddFinallyBlock<TParent>(TParent parent, Action<CSharpFinallyBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpFinallyBlock(), x => configure?.Invoke(x));
    }

    public static TParent AddSwitchStatement<TParent>(TParent parent, string expression, Action<CSharpSwitchStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpSwitchStatement(expression), x => configure?.Invoke(x));
    }

    public static TParent AddStatementBlock<TParent>(TParent parent, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpStatementBlock(), x => configure?.Invoke(x));
    }

    public static TParent AddStatementBlock<TParent>(TParent parent, string expression, Action<CSharpStatementBlock> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpStatementBlock(expression), x => configure?.Invoke(x));
    }

    public static TParent InsertStatement<TParent>(TParent parent, int index, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(TParent parent, int index, IReadOnlyCollection<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
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

    public static TParent AddStatements<TParent>(TParent parent, string statements, Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatements(statements.ConvertToStatements(), x => configure?.Invoke(x.Cast<CSharpStatement>()));
    }

    public static TParent AddStatements<TParent>(TParent parent, IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatements(statements.Select(x => new CSharpStatement(x)), x => configure?.Invoke(x.Cast<CSharpStatement>()));
    }

    public static TParent AddStatements<TParent>(TParent parent, IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
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

    public static TParent AddLocalMethod<TParent>(TParent parent, string returnType, string name, Action<CSharpLocalMethod> configure = null)
        where TParent : IHasCSharpStatements
    {
        if (parent is not CSharpMetadataBase metadataBase)
        {
            throw new Exception($"{parent?.GetType()} is not {nameof(CSharpMetadataBase)}");
        }

        var localMethodStatement = new CSharpLocalMethod(returnType, name, metadataBase.File);
        parent.AddStatement((CSharpStatement)localMethodStatement);
        configure?.Invoke(localMethodStatement);

        return parent;
    }
}