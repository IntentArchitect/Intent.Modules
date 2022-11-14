using System;
using System.Collections.Generic;
using System.Linq;

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
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements<T>(matchFunc)))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(this IHasCSharpStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasCSharpStatements>().SelectMany(x => x.FindStatements<T>(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(this TParent parent, string statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        return parent.AddStatement(new CSharpStatement(statement), configure);
    }

    public static TParent AddStatement<TParent>(this TParent parent, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatement<TParent>(this TParent parent, int index, CSharpStatement statement, Action<CSharpStatement> configure = null)
        where TParent : IHasCSharpStatements
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(this TParent parent, int index, IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
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
}