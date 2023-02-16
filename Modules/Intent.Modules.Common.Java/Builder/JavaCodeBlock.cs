using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Builder;

/// <summary>
/// Similar to <see cref="JavaStatement"/> but meant to be used for custom freetext content.
/// </summary>
public class JavaCodeBlock : ICodeBlock
{
    public JavaCodeBlock(string codeLine)
    {
        if (codeLine == null)
        {
            Lines = Array.Empty<string>();
        }
        else
        {
            Lines = codeLine.Replace("\r\n", "\n").Split("\n");
        }
    }

    public IReadOnlyCollection<string> Lines { get; }

    public JavaCodeSeparatorType BeforeSeparator { get; set; } = JavaCodeSeparatorType.NewLine;
    public JavaCodeSeparatorType AfterSeparator { get; set; } = JavaCodeSeparatorType.NewLine;
    
    public string GetText(string indentation)
    {
        return indentation + string.Join($"{Environment.NewLine}{indentation}", Lines);
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }

    public static implicit operator JavaCodeBlock(string input)
    {
        return new JavaCodeBlock(input);
    }
}