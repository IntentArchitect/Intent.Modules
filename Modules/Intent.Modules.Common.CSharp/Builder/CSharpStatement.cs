namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpStatement : CSharpMetadataBase<CSharpStatement>
{
    public CSharpStatement(string text)
    {
        Text = text;
    }

    internal bool MustSeparateFromPrevious { get; private set; } = false;

    public string Text { get; set; }
    public CSharpStatement SeparatedFromPrevious()
    {
        MustSeparateFromPrevious = true;
        return this;
    }

    public override string ToString()
    {
        return Text;
    }
}