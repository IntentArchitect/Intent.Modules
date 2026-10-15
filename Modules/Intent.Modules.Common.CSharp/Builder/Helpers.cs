using System.Collections.Generic;
using System;
using System.Linq;
using Intent.Modules.Common.CSharp.VisualStudio;

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

    /// <summary>
    /// Whether the project <paramref name="file"/> is generated into supports the <c>required</c> member
    /// modifier, which needs C# 11. Assumed supported when there is no template to determine the project from.
    /// </summary>
    public static bool SupportsRequiredMembers(this ICSharpFile file)
    {
        var outputTarget = file?.Template?.OutputTarget;
        return outputTarget == null || outputTarget.GetProject().GetLanguageVersion().Major >= 11;
    }
}