using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpBuilderExtensions
{
    public static IEnumerable<CSharpStatement> ConvertToStatements(this string s)
    {
        var lines = s.TrimEnd().Replace("\r\n", "\n")
            .Split("\n")
            .SkipWhile(string.IsNullOrWhiteSpace)
            .Select(x => x.TrimEnd())
            .ToArray();

        // We want to remove leading whitespace, but maintain indentation. This below algorithm
        // will assume that first non-empty line dictates the "base" indentation.

        var firstNonBlankLine = lines.FirstOrDefault();
        if (firstNonBlankLine == null)
        {
            return lines.Select(x => new CSharpStatement(x));
        }

        var leadingWhitespaceLength = firstNonBlankLine.Length - firstNonBlankLine.TrimStart().Length;
        var baseIndentation = firstNonBlankLine[..leadingWhitespaceLength];

        return lines.Select(line =>
        {
            var withoutBaseIndentation = line.StartsWith(baseIndentation)
                ? line[leadingWhitespaceLength..]
                : line;

            // The constructor automatically trims the parameter, hence us setting it by the property
            var statement = new CSharpStatement(null)
            {
                Text = withoutBaseIndentation
            };
            return statement;
        });
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
            statement.AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        }
    }

    public delegate string CodeTextTransformer(int codeBlockIndex, ICodeBlock codeBlock, string indentation);
    public static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation, CodeTextTransformer codeTextTransformer = null)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            //.OrderBy(x => x is CSharpLocalMethod) // we should not prescribe this, and it is hidden from the user and likely to lead to confusion
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select((s, i) => $"{orderedCodeBlocks.DetermineSeparator(i, s, indentation, string.Empty, codeTextTransformer)}"));
    }

    public static string JoinCode(this IEnumerable<ICodeBlock> codeBlocks, string separator, string indentation)
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

        if (prevCodeBlock is null && currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.None)
        {
            return $"{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (prevCodeBlock is null)
        {
            return $"{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.EmptyLines ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.NewLine ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.NewLine)
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