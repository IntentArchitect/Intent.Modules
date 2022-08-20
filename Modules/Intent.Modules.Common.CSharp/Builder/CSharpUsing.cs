namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpUsing
{
    public string Namespace { get; }

    public CSharpUsing(string @namespace)
    {
        Namespace = @namespace;
    }

    public override string ToString()
    {
        return $"using {Namespace};";
    }
}