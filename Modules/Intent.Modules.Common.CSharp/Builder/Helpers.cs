namespace Intent.Modules.Common.CSharp.Builder;

internal static class Helpers
{
    public static string EnsureNotKeyword(this string expression, bool defaultKeywordIsAllowed = false)
    {
        if ((defaultKeywordIsAllowed && expression == "default") ||
            !Common.Templates.CSharp.ReservedWords.Contains(expression))
        {
            return expression;
        }

        return $"@{expression}";
    }
}