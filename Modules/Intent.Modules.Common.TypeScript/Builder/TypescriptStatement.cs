using System;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptStatement : TypescriptMetadataBase<TypescriptStatement>, ICodeBlock
{
    public TypescriptStatement(string invocation)
    {
        Text = invocation?.Trim();
    }

    public IHasTypescriptStatements Parent { get; set; }

    public TypescriptCodeSeparatorType BeforeSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;
    public TypescriptCodeSeparatorType AfterSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;

    protected string Text { get; set; }
    protected string RelativeIndentation { get; private set; } = "";

    public TypescriptStatement SeparatedFromPrevious()
    {
        BeforeSeparator = TypescriptCodeSeparatorType.EmptyLines;
        return this;
    }

    public TypescriptStatement Indent()
    {
        RelativeIndentation += "    ";
        return this;
    }

    public TypescriptStatement Outdent()
    {
        RelativeIndentation = RelativeIndentation.Substring("    ".Length);
        return this;
    }

    public TypescriptStatement SetIndent(string relativeIndentation)
    {
        RelativeIndentation = relativeIndentation;
        return this;
    }

    public TypescriptStatement InsertAbove(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
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

    public TypescriptStatement InsertAbove(params TypescriptStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertAbove(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public TypescriptStatement InsertBelow(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
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

    public TypescriptStatement InsertBelow(params TypescriptStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertBelow(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public virtual TypescriptStatement FindAndReplace(string find, string replaceWith)
    {
        Text = Text.Replace(find, replaceWith);
        return this;
    }

    public virtual void Replace(TypescriptStatement replaceWith)
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

    public static implicit operator TypescriptStatement(string input)
    {
        return new TypescriptStatement(input);
    }
}