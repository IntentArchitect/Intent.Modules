namespace Intent.Modules.Common.Java.Builder;

public class JavaArgument : JavaStatement
{
    public JavaStatement Statement { get; }

    public JavaArgument(JavaStatement statement) : base(null)
    {
        Statement = statement;
    }

    public string ArgumentName { get; internal set; }
    public JavaArgument WithName(string name)
    {
        ArgumentName = name;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{(ArgumentName != null ? $"{ArgumentName}: " : string.Empty)}{Statement.GetText(indentation).TrimStart()}";
    }
}