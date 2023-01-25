using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public interface IHasJavaStatements
{
    IList<JavaStatement> Statements { get; }
}

public static class HasJavaStatementsExtensions
{
    public static JavaStatement FindStatement(this IHasJavaStatements parent, Func<JavaStatement, bool> matchFunc)
    {
        return parent.FindStatement<JavaStatement>(matchFunc);
    }

    public static IEnumerable<JavaStatement> FindStatements(this IHasJavaStatements parent, Func<JavaStatement, bool> matchFunc)
    {
        return parent.FindStatements<JavaStatement>(matchFunc);
    }

    public static T FindStatement<T>(this IHasJavaStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasJavaStatements>().SelectMany(x => x.FindStatements(matchFunc)))
            .FirstOrDefault();
    }

    public static IEnumerable<T> FindStatements<T>(this IHasJavaStatements parent, Func<T, bool> matchFunc)
    {
        return parent.Statements.OfType<T>().Where(matchFunc)
            .Concat(parent.Statements.OfType<IHasJavaStatements>().SelectMany(x => x.FindStatements(matchFunc))).ToList();
    }

    public static TParent AddStatement<TParent>(this TParent parent, string statement, Action<JavaStatement> configure = null)
        where TParent : IHasJavaStatements
    {
        return parent.AddStatement(new JavaStatement(statement), configure);
    }

    public static TParent AddStatement<TParent>(this TParent parent, JavaStatement statement, Action<JavaStatement> configure = null)
        where TParent : IHasJavaStatements
    {
        parent.Statements.Add(statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatement<TParent>(this TParent parent, int index, JavaStatement statement, Action<JavaStatement> configure = null)
        where TParent : IHasJavaStatements
    {
        parent.Statements.Insert(index, statement);
        statement.Parent = parent;
        configure?.Invoke(statement);
        return parent;
    }

    public static TParent InsertStatements<TParent>(this TParent parent, int index, IReadOnlyCollection<JavaStatement> statements, Action<IEnumerable<JavaStatement>> configure = null)
        where TParent : IHasJavaStatements
    {
        foreach (var s in statements.Reverse())
        {
            parent.Statements.Insert(index, s);
            s.Parent = parent;
        }
        configure?.Invoke(statements);
        return parent;
    }

    public static TParent AddStatements<TParent>(this TParent parent, string statements, Action<IEnumerable<JavaStatement>> configure = null)
        where TParent : IHasJavaStatements
    {
        return parent.AddStatements(statements.ConvertToStatements(), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<string> statements, Action<IEnumerable<JavaStatement>> configure = null)
        where TParent : IHasJavaStatements
    {
        return parent.AddStatements(statements.Select(x => new JavaStatement(x)), configure);
    }

    public static TParent AddStatements<TParent>(this TParent parent, IEnumerable<JavaStatement> statements, Action<IEnumerable<JavaStatement>> configure = null)
        where TParent : IHasJavaStatements
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