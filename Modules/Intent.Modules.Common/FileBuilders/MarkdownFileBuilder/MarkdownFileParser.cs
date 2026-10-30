using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

internal static partial class MarkdownFileParser
{
    [GeneratedRegex(@"^(#{1,6})\s+(.+)$")]
    private static partial Regex HeadingRegex();

    // The whitespace after the bullet is REQUIRED (\s+, not \s*). With \s* a paragraph opening
    // with emphasis - "**Note** that ..." or "*pointer* is ..." - matches, the leading asterisk is
    // consumed as the bullet marker, and the line is re-emitted as "- *Note** that ...".
    [GeneratedRegex(@"^(\s*)([-*+])\s+(.+)$")]
    private static partial Regex UnorderedListRegex();

    [GeneratedRegex(@"^(\s*)\d+\.\s+(.+)$")]
    private static partial Regex OrderedListRegex();

    [GeneratedRegex(@"^(`{3,})(.*)?$")]
    private static partial Regex CodeFenceRegex();

    // A line that starts its own block even straight after a list item, so it never continues one:
    // a table row, a blockquote, a "===" / "---" line (which, under a paragraph line, Markdown would
    // read as a setext heading underline), any other thematic break ("***", "_ _ _"), the start of an
    // HTML block, or an item-like marker this parser does not model as a list item ("1a.", "2)") -
    // authors use those as their own sub-steps.
    [GeneratedRegex(@"^\s*(\||>|={3,}\s*$|-{3,}\s*$|\d+[A-Za-z]?[.)]\s)|^\s{0,3}([*_])(\s*\2){2,}\s*$|^\s{0,3}<(/?[A-Za-z][A-Za-z0-9-]*([\s/>]|$)|!--|\?|!\[CDATA\[|![A-Z])")]
    private static partial Regex BlockStartRegex();

    public static void Parse(string markdown, MarkdownFrontMatter frontMatter, List<IMarkdownSection> sections)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        var i = 0;

        i = ParseFrontMatter(lines, i, frontMatter);

        MarkdownSection currentSection = null;
        var pendingTextLines = new List<string>();
        // The indentation of the first item in the current run of list items. Levels are measured
        // from it, so a list indented as a whole (e.g. four spaces under a paragraph) is still a top
        // level list rather than being re-emitted four spaces in, where Markdown reads it as code.
        var listBaseIndent = 0;
        // Whether the block before a list item was itself indented - a paragraph or fence inside the
        // previous item. A list after one of those belongs to that item, so it keeps the run's base.
        var previousBlockIndented = false;
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
            {
                currentSection.AddBlock(new MarkdownTextBlock(string.Join("\n", pendingTextLines)));
                previousBlockIndented = IndentOf(pendingTextLines[0]) > 0;
            }
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
                previousBlockIndented = IndentOf(line) > 0;
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
                previousBlockIndented = false;
                listBaseIndent = 0;
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
                currentSection.AddBlock(new MarkdownListItem(unordered.Groups[3].Value, isOrdered: false,
                    indentLevel: ListIndentLevel(currentSection, unordered.Groups[1].Value.Length, previousBlockIndented, ref listBaseIndent)));
                i++;
                continue;
            }

            var ordered = OrderedListRegex().Match(line);
            if (ordered.Success)
            {
                FlushText();
                currentSection.AddBlock(new MarkdownListItem(ordered.Groups[2].Value, isOrdered: true,
                    indentLevel: ListIndentLevel(currentSection, ordered.Groups[1].Value.Length, previousBlockIndented, ref listBaseIndent)));
                i++;
                continue;
            }

            // A non-blank line straight after a list item continues that item, as in Markdown itself
            // (a "lazy" continuation line). Treated as separate text it was re-emitted after a blank
            // line, splitting the bullet mid-sentence. It is stored already indented to the item's
            // content column, so MarkdownFile writes Content as it always has and items built through
            // the API render exactly as before. Trailing spaces stay: two of them are a hard break.
            if (pendingTextLines.Count == 0 &&
                line.Trim().Length > 0 &&
                !BlockStartRegex().IsMatch(line) &&
                currentSection.Blocks.Count > 0 &&
                currentSection.Blocks[^1] is MarkdownListItem continued)
            {
                continued.Content += "\n" + new string(' ', continued.IndentLevel * 2 + (continued.IsOrdered ? 3 : 2)) + line.TrimStart();
                i++;
                continue;
            }

            pendingTextLines.Add(line);
            i++;
        }

        FlushText();
    }

    private static int ListIndentLevel(MarkdownSection section, int indent, bool previousBlockIndented, ref int listBaseIndent)
    {
        var continuesRun = section.Blocks.Count > 0 && section.Blocks[^1] is MarkdownListItem;
        if (!continuesRun && !previousBlockIndented)
        {
            listBaseIndent = indent;
        }

        return Math.Max(0, indent - listBaseIndent) / 2;
    }

    private static int IndentOf(string line) => line.Length - line.TrimStart().Length;

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
