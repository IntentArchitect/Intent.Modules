using System.Collections.Generic;
using System;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

internal static class Helpers
{

    public static readonly HashSet<string> ArgumentValueKeywords = new(StringComparer.Ordinal)
    {
        "default",
        "null",
        "true",
        "false"
    };

    public static string EnsureNotKeyword(this string expression, bool defaultKeywordIsAllowed = false)
    {
        if ((defaultKeywordIsAllowed && expression == "default") ||
            !Common.Templates.CSharp.ReservedWords.Contains(expression))
        {
            return expression;
        }

        return $"@{expression}";
    }


    public static string EnsureVerbatimIdentifierForArgument(this string expression)
    {
        if (ArgumentValueKeywords.Contains(expression))
        {
            return expression;
        }
        if (!Common.Templates.CSharp.ReservedWords.Contains(expression))
        {
            return expression;
        }

        return $"@{expression}";
    }
}