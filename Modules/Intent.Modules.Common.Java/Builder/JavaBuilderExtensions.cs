﻿using System;
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

    internal static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation)
    {
        return string.Concat(codeBlocks.Select(s => $"{codeBlocks.DetermineSeparator(s, indentation, string.Empty)}"));
    }

    internal static string JoinCode(this IEnumerable<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        return string.Concat(codeBlocks.Select(s => $"{codeBlocks.DetermineSeparator(s, indentation, separator)}"));
    }

    private static string DetermineSeparator(this IEnumerable<ICodeBlock> codeBlocks, ICodeBlock s1, string indentation, string separator = "")
    {
        var codeBlocksList = codeBlocks.ToList();
        var index = codeBlocksList.IndexOf(s1);
        var s0 = index > 0 ? codeBlocksList[index - 1] : null;

        if (s0 == null && s1.BeforeSeparator is JavaCodeSeparatorType.None)
        {
            return $"{s1.GetText(string.Empty)}{(index < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (s0 == null)
        {
            return $"{Environment.NewLine}{s1.GetText(indentation)}{(index < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (s0.AfterSeparator is JavaCodeSeparatorType.EmptyLines ||
            s1.BeforeSeparator is JavaCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{s1.GetText(indentation)}{(index < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (s0.AfterSeparator is JavaCodeSeparatorType.NewLine ||
            s1.BeforeSeparator is JavaCodeSeparatorType.NewLine)
        {
            return $"{Environment.NewLine}{s1.GetText(indentation)}{(index < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        return $"{s1.GetText(string.Empty)}{(index < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
    }
}