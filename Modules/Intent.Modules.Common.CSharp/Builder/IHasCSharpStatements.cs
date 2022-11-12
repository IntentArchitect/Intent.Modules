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
}