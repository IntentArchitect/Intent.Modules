using System;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

/// <summary>
/// Once we've upgraded to .NET 6 this can be removed.
/// </summary>
internal static class StringExtension
{
    public static string ReplaceLineEndings(this string s)
    {
        var unixStyle = s.Replace("\r\n", "\n");

        return Environment.NewLine == "\n"
            ? unixStyle
            : unixStyle.Replace("\n", "\r\n");
    }
}