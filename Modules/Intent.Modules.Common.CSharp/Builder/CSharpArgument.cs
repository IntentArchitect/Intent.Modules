namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpArgument : CSharpStatement
{
    public CSharpStatement Statement { get; }

    public CSharpArgument(CSharpStatement statement) : base(null)
    {
        Statement = statement;
    }

    public string ArgumentName { get; internal set; }
    public CSharpArgument WithName(string name)
    {
        ArgumentName = name;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{(ArgumentName != null ? $"{ArgumentName}: " : string.Empty)}{Statement.GetText(indentation).TrimStart()}";
    }
}