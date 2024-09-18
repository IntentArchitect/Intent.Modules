using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpTopLevelStatements : CSharpMetadataBase<CSharpTopLevelStatements>, IHasCSharpStatements
{
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public CSharpTopLevelStatements AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        return AddStatement(new CSharpStatement(statement), configure);
    }

    public CSharpTopLevelStatements AddStatement<T>(T statement, Action<T> configure = null)
        where T : CSharpStatement
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpTopLevelStatements InsertStatement<T>(int index, T statement, Action<T> configure = null)
        where T : CSharpStatement
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public CSharpTopLevelStatements InsertStatements<T>(int index, IReadOnlyCollection<T> statements, Action<IEnumerable<T>> configure = null)
        where T : CSharpStatement
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public CSharpTopLevelStatements AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpTopLevelStatements AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpTopLevelStatements AddStatements<T>(IEnumerable<T> statements, Action<IEnumerable<T>> configure = null)
        where T : CSharpStatement
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
            statement.Parent = this;
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public CSharpTopLevelStatements FindAndReplaceStatement<T>(Func<CSharpStatement, bool> matchFunc, T replaceWith)
        where T : CSharpStatement
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public CSharpLocalMethod FindMethod(string name)
    {
        return Statements.OfType<CSharpLocalMethod>().FirstOrDefault(x => x.Name == name);
    }

    public CSharpLocalMethod FindMethod(Func<CSharpLocalMethod, bool> matchFunc)
    {
        return Statements.OfType<CSharpLocalMethod>().FirstOrDefault(matchFunc);
    }

    public void RemoveStatement(CSharpStatement statement)
    {
        Statements.Remove(statement);
    }

    public override string ToString()
    {
        return Statements.ConcatCode(string.Empty).TrimStart();
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => true;
}