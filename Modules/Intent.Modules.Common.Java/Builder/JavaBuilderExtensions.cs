using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public static class JavaBuilderExtensions
{
    public static IEnumerable<JavaStatement> ConvertToStatements(this string s)
    {
        return s.Trim().Replace("\r\n", "\n").Split("\n").Select(x => new JavaStatement(x));
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.BeforeSeparator = JavaCodeSeparatorType.EmptyLines;
            statement.AfterSeparator = JavaCodeSeparatorType.EmptyLines;
        }
    }

    internal delegate string CodeTextTransformer(int codeBlockIndex, ICodeBlock codeBlock, string indentation);
    internal static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation, CodeTextTransformer codeTextTransformer = null)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            //.OrderBy(x => x is CSharpLocalMethod) // we should not prescribe this, and it is hidden from the user and likely to lead to confusion
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select((s, i) => $"{orderedCodeBlocks.DetermineSeparator(i, s, indentation, string.Empty, codeTextTransformer)}"));
    }

    internal static string JoinCode(this IEnumerable<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            //.OrderBy(x => x is CSharpLocalMethod) // we should not prescribe this, and it is hidden from the user and likely to lead to confusion
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select((s, i) => $"{orderedCodeBlocks.DetermineSeparator(i, s, indentation, separator)}"));
    }

    private static string DetermineSeparator(this IEnumerable<ICodeBlock> codeBlocks, int codeBlockIndex, ICodeBlock currentCodeBlock, string indentation, string separator = "", CodeTextTransformer codeTextTransformer = null)
    {
        var codeBlocksList = codeBlocks.ToList();
        var currentBlockIndex = codeBlocksList.IndexOf(currentCodeBlock);
        var prevCodeBlock = currentBlockIndex > 0 ? codeBlocksList[currentBlockIndex - 1] : null;

        if (prevCodeBlock is null && currentCodeBlock.BeforeSeparator is JavaCodeSeparatorType.None)
        {
            return $"{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (prevCodeBlock is null)
        {
            return $"{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is JavaCodeSeparatorType.EmptyLines ||
            currentCodeBlock.BeforeSeparator is JavaCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is JavaCodeSeparatorType.NewLine ||
            currentCodeBlock.BeforeSeparator is JavaCodeSeparatorType.NewLine)
        {
            return $"{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        return $"{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
    }

    private static string GetCodeText(int codeBlockIndex, ICodeBlock codeBlock, string indentation, CodeTextTransformer codeTextTransformer)
    {
        return codeTextTransformer == null
            ? codeBlock.GetText(indentation)
            : codeTextTransformer(codeBlockIndex, codeBlock, indentation);
    }
}