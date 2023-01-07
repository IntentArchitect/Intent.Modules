using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptSuperConstructorCall
{
    public List<string> Arguments { get; } = new();

    public TypescriptSuperConstructorCall AddArgument(string expression)
    {
        Arguments.Add(expression);
        return this;
    }

    public virtual string GetText(string indentation)
    {
        return $"{indentation}    super({string.Join(", ", Arguments)});";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}