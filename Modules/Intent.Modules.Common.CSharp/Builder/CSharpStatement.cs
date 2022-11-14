using System;
using System.Collections;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatement : CSharpMetadataBase<CSharpStatement>, ICodeBlock
{

    public CSharpStatement(string invocation)
    {
        Text = invocation?.Trim();
    }

    public IHasCSharpStatements Parent { get; set; }

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;

    protected string Text { get; set; }
    protected string RelativeIndentation { get; private set; } = "";
    public CSharpStatement SeparatedFromPrevious()
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        return this;
    }

    public CSharpStatement Indent()
    {
        RelativeIndentation += "    ";
        return this;
    }

    public CSharpStatement Outdent()
    {
        RelativeIndentation = RelativeIndentation.Substring("    ".Length);
        return this;
    }

    public CSharpStatement SetIndent(string relativeIndentation)
    {
        RelativeIndentation = relativeIndentation;
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

public interface ICodeBlock
{
    CSharpCodeSeparatorType BeforeSeparator { get; set; }
    CSharpCodeSeparatorType AfterSeparator { get; set; }
    string GetText(string indentation);
}

public enum CSharpCodeSeparatorType
{
    None = 0,
    NewLine = 1,
    EmptyLines = 2
}