using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpBuilderExtensions
{
    public static IEnumerable<CSharpStatement> ConvertToStatements(this string s)
    {
        return s.Trim().Replace("\r\n", "\n").Split("\n").Select(x => new CSharpStatement(x));
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
            statement.AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        }
    }

    internal static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation, Func<string, string> codeTextTransformer = null)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            .OrderBy(x => x is CSharpLocalMethod)
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select(s => $"{orderedCodeBlocks.DetermineSeparator(s, indentation, string.Empty, codeTextTransformer)}"));
    }

    internal static string JoinCode(this IEnumerable<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            .OrderBy(x => x is CSharpLocalMethod)
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select(s => $"{orderedCodeBlocks.DetermineSeparator(s, indentation, separator)}"));
    }

    private static string DetermineSeparator(this IEnumerable<ICodeBlock> codeBlocks, ICodeBlock currentCodeBlock, string indentation, string separator = "", Func<string, string> codeTextTransformer = null)
    {
        var codeBlocksList = codeBlocks.ToList();
        var currentBlockIndex = codeBlocksList.IndexOf(currentCodeBlock);
        var prevCodeBlock = currentBlockIndex > 0 ? codeBlocksList[currentBlockIndex - 1] : null;

        if (prevCodeBlock is null && currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.None)
        {
            return $"{GetCodeText(currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (prevCodeBlock is null)
        {
            return $"{Environment.NewLine}{GetCodeText(currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.EmptyLines ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{GetCodeText(currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.NewLine ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.NewLine)
        {
            return $"{Environment.NewLine}{GetCodeText(currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        return $"{GetCodeText(currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
    }

    private static string GetCodeText(ICodeBlock codeBlock, string indentation, Func<string, string> codeTextTransformer)
    {
        return codeTextTransformer == null
            ? codeBlock.GetText(indentation)
            : indentation + codeTextTransformer(codeBlock.GetText(indentation).TrimStart());
    }
}