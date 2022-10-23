using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatement : CSharpMetadataBase<CSharpStatement>
{
    public CSharpStatement(string text)
    {
        Text = text;
    }

    internal IHasCSharpStatements Parent { get; set; }

    internal bool MustSeparateFromPrevious { get; private set; } = false;

    public string Text { get; set; }
    public CSharpStatement SeparatedFromPrevious()
    {
        MustSeparateFromPrevious = true;
        return this;
    }

    public void Remove()
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Cannot remove statement from unknown parent");
        }
        Parent.RemoveStatement(this);
    }

    public virtual string GetText(string indentation)
    {
        return $"{indentation}{Text}";
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
    void RemoveStatement(CSharpStatement statement);
}