using System;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaStatement : JavaMetadataBase<JavaStatement>, ICodeBlock, IJavaExpression
{
    public JavaStatement(string invocation)
    {
        Text = invocation?.Trim();
    }

    public IHasJavaStatements Parent { get; set; }

    public JavaCodeSeparatorType BeforeSeparator { get; set; } = JavaCodeSeparatorType.NewLine;
    public JavaCodeSeparatorType AfterSeparator { get; set; } = JavaCodeSeparatorType.NewLine;

    protected string Text { get; set; }
    protected string RelativeIndentation { get; private set; } = "";
    public JavaStatement SeparatedFromPrevious()
    {
        BeforeSeparator = JavaCodeSeparatorType.EmptyLines;
        return this;
    }

    public JavaStatement Indent()
    {
        RelativeIndentation += "    ";
        return this;
    }

    public JavaStatement Outdent()
    {
        RelativeIndentation = RelativeIndentation["    ".Length..];
        return this;
    }

    public JavaStatement SetIndent(string relativeIndentation)
    {
        RelativeIndentation = relativeIndentation;
        return this;
    }

    public JavaStatement InsertAbove(JavaStatement statement, Action<JavaStatement> configure = null)
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

    public JavaStatement InsertAbove(params JavaStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertAbove(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public JavaStatement InsertBelow(JavaStatement statement, Action<JavaStatement> configure = null)
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

    public JavaStatement InsertBelow(params JavaStatement[] statements)
    {
        if (statements.Length == 0)
        {
            return this;
        }
        InsertBelow(statements[0], s => s.InsertBelow(statements.Skip(1).ToArray()));
        return this;
    }

    public virtual JavaStatement FindAndReplace(string find, string replaceWith)
    {
        Text = Text.Replace(find, replaceWith);
        return this;
    }

    public virtual void Replace(JavaStatement replaceWith)
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
    
    public static implicit operator JavaStatement(string input)
    {
        return input != null ? new JavaStatement(input) : null;
    }
}