using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

internal static partial class MarkdownFileParser
{
    [GeneratedRegex(@"^(#{1,6})\s+(.+)$")]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"^(\s*)([-*+])\s*(.+)$")]
    private static partial Regex UnorderedListRegex();

    [GeneratedRegex(@"^(\s*)\d+\.\s+(.+)$")]
    private static partial Regex OrderedListRegex();

    [GeneratedRegex(@"^(`{3,})(.*)?$")]
    private static partial Regex CodeFenceRegex();

    public static void Parse(string markdown, MarkdownFrontMatter frontMatter, List<IMarkdownSection> sections)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        var i = 0;

        i = ParseFrontMatter(lines, i, frontMatter);

        MarkdownSection currentSection = null;
        var pendingTextLines = new List<string>();
        var inCodeBlock = false;
        string codeFence = null;
        string codeLanguage = null;
        var codeLines = new List<string>();

        void FlushText()
        {
            if (pendingTextLines.Count == 0) return;
            while (pendingTextLines.Count > 0 && string.IsNullOrEmpty(pendingTextLines[0]))
                pendingTextLines.RemoveAt(0);
            while (pendingTextLines.Count > 0 && string.IsNullOrEmpty(pendingTextLines[^1]))
                pendingTextLines.RemoveAt(pendingTextLines.Count - 1);
            if (pendingTextLines.Count > 0 && currentSection != null)
                currentSection.AddBlock(new MarkdownTextBlock(string.Join("\n", pendingTextLines)));
            pendingTextLines.Clear();
        }

        while (i < lines.Length)
        {
            var line = lines[i];

            if (inCodeBlock)
            {
                var fenceClose = CodeFenceRegex().Match(line.TrimStart());
                if (fenceClose.Success && fenceClose.Groups[1].Value.Length >= codeFence.Length)
                {
                    inCodeBlock = false;
                    currentSection?.AddBlock(new MarkdownCodeBlock(string.Join("\n", codeLines), codeLanguage));
                    codeLines.Clear();
                    codeFence = null;
                    codeLanguage = null;
                }
                else
                {
                    codeLines.Add(line);
                }
                i++;
                continue;
            }

            var codeFenceOpen = CodeFenceRegex().Match(line.TrimStart());
            if (codeFenceOpen.Success)
            {
                FlushText();
                inCodeBlock = true;
                codeFence = codeFenceOpen.Groups[1].Value;
                codeLanguage = codeFenceOpen.Groups[2].Value.Trim();
                codeLines.Clear();
                i++;
                continue;
            }

            var headingMatch = HeadingRegex().Match(line);
            if (headingMatch.Success)
            {
                FlushText();
                var level = headingMatch.Groups[1].Value.Length;
                var title = headingMatch.Groups[2].Value.Trim();
                currentSection = new MarkdownSection(title, level);
                sections.Add(currentSection);
                i++;
                continue;
            }

            if (currentSection == null)
            {
                i++;
                continue;
            }

            var unordered = UnorderedListRegex().Match(line);
            if (unordered.Success)
            {
                FlushText();
                var indentLevel = unordered.Groups[1].Value.Length / 2;
                currentSection.AddBlock(new MarkdownListItem(unordered.Groups[3].Value, isOrdered: false, indentLevel: indentLevel));
                i++;
                continue;
            }

            var ordered = OrderedListRegex().Match(line);
            if (ordered.Success)
            {
                FlushText();
                var indentLevel = ordered.Groups[1].Value.Length / 2;
                currentSection.AddBlock(new MarkdownListItem(ordered.Groups[2].Value, isOrdered: true, indentLevel: indentLevel));
                i++;
                continue;
            }

            pendingTextLines.Add(line);
            i++;
        }

        FlushText();
    }

    private static int ParseFrontMatter(string[] lines, int i, MarkdownFrontMatter frontMatter)
    {
        if (i >= lines.Length || lines[i].TrimEnd() != "---")
            return i;

        i++;
        while (i < lines.Length && lines[i].TrimEnd() != "---")
        {
            var colonIndex = lines[i].IndexOf(':');
            if (colonIndex > 0)
            {
                var key = lines[i][..colonIndex].Trim();
                var value = lines[i][(colonIndex + 1)..].Trim();
                frontMatter.Set(key, value);
            }
            i++;
        }

        return i < lines.Length ? i + 1 : i;
    }
}
