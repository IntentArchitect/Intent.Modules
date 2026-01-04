using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public static class TypescriptBuilderExtensions
{
    public static IEnumerable<TypescriptStatement> ConvertToStatements(this string s)
    {
        return s.Trim().Replace("\r\n", "\n").Split("\n").Select(x => new TypescriptStatement(x));
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.BeforeSeparator = TypescriptCodeSeparatorType.EmptyLines;
            statement.AfterSeparator = TypescriptCodeSeparatorType.EmptyLines;
        }
    }

    internal static string ConcatCode(this IReadOnlyCollection<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        return string.Concat(codeBlocks.Select(s => $"{codeBlocks.DetermineSeparator(s, indentation, separator)}"));
    }

    internal static string ConcatCode(this IReadOnlyCollection<ICodeBlock> codeBlocks, string indentation)
    {
        return string.Concat(codeBlocks.Select(s => $"{codeBlocks.DetermineSeparator(s, indentation, string.Empty)}"));
    }

    internal static string JoinCode(this IReadOnlyCollection<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        return string.Concat(codeBlocks.Select(s => $"{codeBlocks.DetermineSeparator(s, indentation, separator)}"));
    }

    private static string DetermineSeparator(this IEnumerable<ICodeBlock> codeBlocks, ICodeBlock s1, string indentation, string separator = "")
    {
        var codeBlocksList = codeBlocks.ToList();
        var index = codeBlocksList.IndexOf(s1);
        var s0 = index > 0 ? codeBlocksList[index - 1] : null;

        if (s0 == null && s1.BeforeSeparator is TypescriptCodeSeparatorType.None)
        {
            return $"{s1.GetText(string.Empty)}{(index < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (s0 == null)
        {
            return $"{Environment.NewLine}{s1.GetText(indentation)}{(index < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (s0.AfterSeparator is TypescriptCodeSeparatorType.EmptyLines ||
            s1.BeforeSeparator is TypescriptCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{s1.GetText(indentation)}{(index < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (s0.AfterSeparator is TypescriptCodeSeparatorType.NewLine ||
            s1.BeforeSeparator is TypescriptCodeSeparatorType.NewLine)
        {
            return $"{Environment.NewLine}{s1.GetText(indentation)}{(separator)}";
        }

        return $"{s1.GetText(string.Empty)}{(index < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
    }

    public static string TrimEnd(this string value, string ending)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(ending))
            return value;

        if (value.EndsWith(ending, StringComparison.Ordinal))
        {
            value = value[..^ending.Length];
        }

        return value;
    }
}