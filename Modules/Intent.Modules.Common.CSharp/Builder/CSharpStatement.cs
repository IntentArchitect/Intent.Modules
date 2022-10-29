using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatement : CSharpMetadataBase<CSharpStatement>
{
    public CSharpStatement(string text)
    {
        Text = text?.Trim();
    }

    internal IHasCSharpStatements Parent { get; set; }

    internal bool MustSeparateFromPrevious { get; private set; } = false;

    public string Text { get; set; }
    public string RelativeIndentation { get; private set; } = "";
    public CSharpStatement SeparatedFromPrevious()
    {
        MustSeparateFromPrevious = true;
        return this;
    }

    public CSharpStatement Indent()
    {
        RelativeIndentation += "    ";
        return this;
    }

    public CSharpStatement InsertAbove(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Cannot insert statement for unknown parent");
        }
        Parent.Statements.Insert(Parent.Statements.IndexOf(this), statement);
        configure?.Invoke(statement);
        return this;
    }

    public CSharpStatement InsertAbove(params CSharpStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertAbove(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public CSharpStatement InsertBelow(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Cannot insert statement for unknown parent");
        }
        Parent.Statements.Insert(Parent.Statements.IndexOf(this) + 1, statement);
        configure?.Invoke(statement);
        return this;
    }

    public CSharpStatement InsertBelow(params CSharpStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertBelow(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public void Remove()
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Cannot remove statement from unknown parent");
        }
        Parent.Statements.Remove(this);
    }

    public virtual string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{Text}";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }

    public static implicit operator CSharpStatement(string input)
    {
        return new CSharpStatement(input);
    }
}

public interface IHasCSharpStatements
{
    IList<CSharpStatement> Statements { get; }

//int GetStatementIndex(CSharpStatement statement);
    //void InsertStatement(int index, CSharpStatement statement, Action<CSharpStatement> configure = null);
    //void RemoveStatement(CSharpStatement statement);
}