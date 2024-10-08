using System;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatement : CSharpMetadataBase<CSharpStatement>, ICSharpStatement
{
    public CSharpStatement()
    {
    }

    public CSharpStatement(string statement)
    {
        Text = statement?.Trim();
    }

    public CSharpStatement(string statement, ICSharpReferenceable reference)
    {
        Reference = reference;
        Text = statement?.Trim();
    }

    public new IHasCSharpStatements Parent
    {
        get => (IHasCSharpStatements)base.Parent;
        set => base.Parent = (ICSharpCodeContext)value;
    }

    IHasCSharpStatementsActual ICSharpStatement.Parent
    {
        get => (IHasCSharpStatements)base.Parent;
        set => base.Parent = (ICSharpCodeContext)value;
    }

    public ICSharpReferenceable Reference { get; }
    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;

    public string Text { get; set; }
    protected string RelativeIndentation { get; private set; } = "";
    protected char? TrailingCharacter = null;
    public CSharpStatement SeparatedFromPrevious()
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        return this;
    }

    public CSharpStatement SeparatedFromNext()
    {
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        return this;
    }

    public CSharpStatement Indent()
    {
        RelativeIndentation += "    ";
        return this;
    }

    public CSharpStatement Outdent()
    {
        RelativeIndentation = RelativeIndentation["    ".Length..];
        return this;
    }

    public CSharpStatement SetIndent(string relativeIndentation)
    {
        RelativeIndentation = relativeIndentation;
        return this;
    }

    public virtual CSharpStatement WithSemicolon()
    {
        TrailingCharacter = ';';
        return this;
    }

    public CSharpStatement InsertAbove(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Cannot insert statement for unknown parent");
        }
        Parent.Statements.Insert(Parent.Statements.IndexOf(this), statement);
        statement.SetIndent(RelativeIndentation);
        statement.Parent = Parent;
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
        statement.SetIndent(RelativeIndentation);
        statement.Parent = Parent;
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

    public virtual CSharpStatement FindAndReplace(string find, string replaceWith)
    {
        Text = Text.Replace(find, replaceWith);
        return this;
    }

    public virtual void Replace(CSharpStatement replaceWith)
    {
        replaceWith.BeforeSeparator = BeforeSeparator;
        replaceWith.AfterSeparator = BeforeSeparator;
        InsertAbove(replaceWith);
        Remove();
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
        return $"{indentation}{RelativeIndentation}{Text}{(TrailingCharacter != null && !Text.EndsWith(TrailingCharacter.Value) ? TrailingCharacter.Value : "")}";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }

    public static implicit operator CSharpStatement(string input)
    {
        return input != null ? new CSharpStatement(input) : null;
    }

    #region ICSharpStatement

    ICSharpStatement ICSharpStatement.SeparatedFromNext() =>
        SeparatedFromNext();

    ICSharpStatement ICSharpStatement.Indent() =>
        Indent();

    ICSharpStatement ICSharpStatement.Outdent() =>
        Outdent();

    ICSharpStatement ICSharpStatement.SetIndent(string relativeIndentation) =>
        SetIndent(relativeIndentation);

    ICSharpStatement ICSharpStatement.WithSemicolon() =>
        WithSemicolon();

    ICSharpStatement ICSharpStatement.InsertAbove(ICSharpStatement statement, Action<ICSharpStatement> configure) =>
        InsertAbove((CSharpStatement)statement, configure);

    ICSharpStatement ICSharpStatement.InsertAbove(params ICSharpStatement[] statements) =>
        InsertAbove(statements.Cast<CSharpStatement>().ToArray());

    ICSharpStatement ICSharpStatement.InsertBelow(ICSharpStatement statement, Action<ICSharpStatement> configure) =>
        InsertBelow((CSharpStatement)statement, configure);

    ICSharpStatement ICSharpStatement.InsertBelow(params ICSharpStatement[] statements) =>
        InsertBelow(statements.Cast<CSharpStatement>().ToArray());

    ICSharpStatement ICSharpStatement.FindAndReplace(string find, string replaceWith) =>
        FindAndReplace(find, replaceWith);

    void ICSharpStatement.Replace(ICSharpStatement replaceWith) =>
        Replace((CSharpStatement)replaceWith);

    ICSharpStatement ICSharpStatement.SeparatedFromPrevious() =>
        SeparatedFromPrevious();

    #endregion
}