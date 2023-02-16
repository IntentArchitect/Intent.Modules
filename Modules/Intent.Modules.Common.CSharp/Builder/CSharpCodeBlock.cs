using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

/// <summary>
/// Similar to <see cref="CSharpStatement"/> but meant to be used for custom freetext content.
/// </summary>
public class CSharpCodeBlock : ICodeBlock
{
    public CSharpCodeBlock(string codeLine)
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

    public CSharpCodeSeparatorType BeforeSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    public CSharpCodeSeparatorType AfterSeparator { get; set; } = CSharpCodeSeparatorType.NewLine;
    
    public string GetText(string indentation)
    {
        return indentation + string.Join($"{Environment.NewLine}{indentation}", Lines);
    }

    public override string ToString()
    {
        return GetText(string.Empty);
    }

    public static implicit operator CSharpCodeBlock(string input)
    {
        return new CSharpCodeBlock(input);
    }
}