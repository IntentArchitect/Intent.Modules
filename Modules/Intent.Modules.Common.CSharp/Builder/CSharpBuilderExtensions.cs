using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpBuilderExtensions
{
    public static IEnumerable<CSharpStatement> ConvertToStatements(this string s)
    {
        return s.Trim().Replace("\r\n", "\n").Split("\n").Select(x=> new CSharpStatement(x));
    }

    public static void SeparateAll(this IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            statement.SeparatedFromPrevious();
        }
    }
}