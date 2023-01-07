using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public interface IHasTypescriptStatements
{
    List<TypescriptStatement> Statements { get; }
}

public static class HasTypescriptStatementsExtensions
{
    public static TypescriptStatement FindStatement(this IHasTypescriptStatements parent, Func<TypescriptStatement, bool> matchFunc)
    {
        return parent.FindStatement<TypescriptStatement>(matchFunc);
    }

    public static IEnumerable<TypescriptStatement> FindStatements(this IHasTypescriptStatements parent, Func<TypescriptStatement, bool> matchFunc)
    {
        return parent.FindStatements<TypescriptStatement>(matchFunc);
    }

    public static T FindStatement<T>(this IHasTypescriptStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasTypescriptStatements>().SelectMany(x => x.FindStatements(matchFunc)))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(this IHasTypescriptStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasTypescriptStatements>().SelectMany(x => x.FindStatements(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(this TParent parent, string statement, Action<TypescriptStatement> configure = null)
        where TParent : IHasTypescriptStatements
    {
        return parent.AddStatement(new TypescriptStatement(statement), configure);
    }

    public static TParent AddStatement<TParent>(this TParent parent, TypescriptStatement statement, Action<TypescriptStatement> configure = null)
        where TParent : IHasTypescriptStatements
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatement<TParent>(this TParent parent, int index, TypescriptStatement statement, Action<TypescriptStatement> configure = null)
        where TParent : IHasTypescriptStatements
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(this TParent parent, int index, IReadOnlyCollection<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
        where TParent : IHasTypescriptStatements
    {
        foreach (var s in statements.Reverse())
        {
            parent.Statements.Insert(index, s);
            s.Parent = parent;
        }
        configure?.Invoke(statements);
        return parent;
    }

    public static TParent AddStatements<TParent>(this TParent parent, string statements, Action<IEnumerable<TypescriptStatement>> configure = null)
        where TParent : IHasTypescriptStatements
    {
        return parent.AddStatements(statements.ConvertToStatements(), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<string> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
        where TParent : IHasTypescriptStatements
    {
        return parent.AddStatements(statements.Select(x => new TypescriptStatement(x)), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
        where TParent : IHasTypescriptStatements
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