using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpBuilderExtensions
{
    public static IEnumerable<CSharpStatement> ConvertToStatements(this string s)
    {
        return s.Trim().Replace("\r\n", "\n").Split("\n").Select(x=> new CSharpStatement(x));
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.Separator = CSharpCodeSeparatorType.EmptyLines;
        }
    }

    internal static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation)
    {
        return string.Join(@"
", codeBlocks.Select((s, index) => $"{s.GetText(indentation).TrimEnd()}{(DetermineSeparator(s, codeBlocks.Skip(index + 1).FirstOrDefault()))}"));
    }

    private static string DetermineSeparator(ICodeBlock s1, ICodeBlock s2)
    {
        if (s1 == null || s2 == null)
        {
            return string.Empty;
        }
        if (s1.Separator is CSharpCodeSeparatorType.EmtpyLineBelowOnly or CSharpCodeSeparatorType.EmptyLines ||
            s2.Separator is CSharpCodeSeparatorType.EmtpyLineAboveOnly or CSharpCodeSeparatorType.EmptyLines)
        {
            return Environment.NewLine;
        }
        return string.Empty;
    }
}