using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptConstructorSuperCall : ICodeBlock
{
    public List<string> Arguments { get; } = new();

    public TypescriptConstructorSuperCall AddArgument(string expression)
    {
        Arguments.Add(expression);
        return this;
    }

    public TypescriptCodeSeparatorType BeforeSeparator { get; set; } = TypescriptCodeSeparatorType.NewLine;
    public TypescriptCodeSeparatorType AfterSeparator { get; set; } = TypescriptCodeSeparatorType.EmptyLines;

    public virtual string GetText(string indentation)
    {
        return $"{indentation}super({string.Join(", ", Arguments)});";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}