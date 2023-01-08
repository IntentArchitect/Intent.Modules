using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptSuperConstructorCall : ICodeBlock
{
    public List<string> Arguments { get; } = new();

    public TypescriptSuperConstructorCall AddArgument(string expression)
    {
        Arguments.Add(expression);
        return this;
    }

    public TypescriptCodeSeparatorType BeforeSeparator { get; set; }
    public TypescriptCodeSeparatorType AfterSeparator { get; set; }

    public virtual string GetText(string indentation)
    {
        return $"{Environment.NewLine}{indentation}    super({string.Join(", ", Arguments)});";
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }
}