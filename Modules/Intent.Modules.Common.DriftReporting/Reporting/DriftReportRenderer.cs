using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Modules.Common.DriftReporting.Api;

namespace Intent.Modules.Common.DriftReporting.Reporting
{
    /// <summary>
    /// The drift report, an MDX document: a summary of the Areas, then one section per Area with a table per category. It
    /// has no timestamps, so a run that finds the same drift writes the same report.
    /// </summary>
    internal static class DriftReportRenderer
    {
        public static string Render(string application, string outputRoot, DriftCollector collector)
        {
            var text = new StringBuilder();
            var errors = collector.Areas.Sum(x => x.Errors);
            var warnings = collector.Areas.Sum(x => x.Warnings);
            text.Append("# Drift report: ").Append(Text(application)).Append("\n\n");
            text.Append(errors > 0 ? "✖ **Failed**" : warnings > 0 ? "⚠ **Passed with warnings**" : "✓ **Clean**")
                .Append(": ").Append(Count(errors, "error")).Append(", ").Append(Count(warnings, "warning")).Append("\n\n");

            text.Append("| Area | Errors | Warnings |\n| --- | --- | --- |\n");
            foreach (var area in collector.Areas)
            {
                text.Append("| [").Append(Text(area.Name)).Append("](#").Append(Slug(area.Name)).Append(") | ")
                    .Append(area.Errors).Append(" | ").Append(area.Warnings).Append(" |\n");
            }

            text.Append("| **Total** | **").Append(errors).Append("** | **").Append(warnings).Append("** |\n");

            foreach (var area in collector.Areas)
            {
                while (text[^1] == '\n')
                {
                    text.Length--;
                }

                text.Append("\n\n---\n\n## ").Append(Text(area.Name)).Append("\n\n");
                foreach (var category in area.Categories)
                {
                    var findings = area.Findings.Where(x => x.Category == category).ToList();
                    text.Append("### ").Append(Label(category)).Append(" drift");
                    if (findings.Count == 0)
                    {
                        text.Append("\n\n✓ Clean\n\n");
                        continue;
                    }

                    text.Append(" (").Append(findings.Count).Append(")\n\n");
                    var withAgainst = category != DriftCategory.Code || findings.Any(x => x.Against.Count > 0);
                    text.Append(withAgainst
                        ? "| Severity | Subject | Against | Finding |\n| --- | --- | --- | --- |\n"
                        : "| Severity | Subject | Finding |\n| --- | --- | --- |\n");
                    foreach (var finding in findings)
                    {
                        text.Append("| ").Append(finding.Severity == DriftSeverity.Error ? "✖ Error" : "⚠ Warning")
                            .Append(" | ").Append(Anchor(finding.Subject, application, outputRoot));
                        if (withAgainst)
                        {
                            text.Append(" | ").Append(finding.Against.Count == 0 ? "—" : string.Join(", ", finding.Against.Select(x => Anchor(x, application, outputRoot))));
                        }

                        text.Append(" | ").Append(Text(finding.Message)).Append(" |\n");
                    }

                    text.Append('\n');
                }
            }

            return text.ToString().TrimEnd() + "\n";
        }

        public static string Label(DriftCategory category) => category switch
        {
            DriftCategory.ModelCode => "Model / Code",
            DriftCategory.CodeCode => "Code / Code",
            _ => "Code"
        };

        public static string Count(int count, string noun) => $"{count} {noun}{(count == 1 ? string.Empty : "s")}";

        /// <summary>
        /// A file anchor's absolute path, with forward slashes: the path as given when it is absolute (a drive-letter path
        /// counts on every OS), else under the application's output root.
        /// </summary>
        public static string FullPath(string path, string outputRoot)
        {
            path = path.Replace('\\', '/');
            var absolute = Path.IsPathRooted(path) || path.Length > 2 && char.IsLetter(path[0]) && path[1] == ':' && path[2] == '/';
            return Normalize(absolute || string.IsNullOrEmpty(outputRoot) ? path : $"{Root(outputRoot)}{path.TrimStart('/')}");
        }

        /// <summary>The output root, normalised, with forward slashes and one trailing slash.</summary>
        public static string Root(string outputRoot) => Normalize(outputRoot).TrimEnd('/') + "/";

        /// <summary>
        /// A path with forward slashes, and without <c>..</c> segments when it is rooted on this OS: an application's output
        /// root can be relative to its <c>.application.config</c>, e.g. <c>intent/eShop/../../eShop</c>.
        /// </summary>
        private static string Normalize(string path) => (Path.IsPathRooted(path) ? Path.GetFullPath(path) : path).Replace('\\', '/');

        private static string Anchor(DriftAnchor anchor, string application, string outputRoot)
        {
            switch (anchor.Kind)
            {
                case DriftAnchorKind.Element:
                    return $"<ModelRef application=\"{Attribute(application)}\" elementId=\"{Attribute(anchor.ModelElement.Id)}\" />";
                case DriftAnchorKind.File:
                    var path = FullPath(anchor.FilePath, outputRoot);
                    var name = path[(path.LastIndexOf('/') + 1)..];
                    return anchor.Line is { } line
                        ? $"<FileRef path=\"{Attribute(path)}\" line={{{line}}} label=\"{Attribute($"{name}:{line}")}\" />"
                        : $"<FileRef path=\"{Attribute(path)}\" label=\"{Attribute(name)}\" />";
                default:
                    return string.IsNullOrWhiteSpace(anchor.Value) ? "—" : Text(anchor.Value);
            }
        }

        /// <summary>
        /// Plain text as MDX on one line, safe in a table cell. A backtick span renders as inline code, where only a pipe is
        /// escaped. Everywhere else, everything Markdown or JSX would read is escaped: a brace or an angle bracket would start
        /// JSX, a pipe would end the cell, and emphasis, link and escape characters would change the text. Code spans are
        /// found as CommonMark finds them: a run of backticks opens one only if a run of the same length closes it; any other
        /// run is literal.
        /// </summary>
        public static string Text(string plain)
        {
            plain = (plain ?? string.Empty).Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ');
            var text = new StringBuilder();
            var i = 0;
            while (i < plain.Length)
            {
                if (plain[i] != '`')
                {
                    text.Append(Escape(plain[i++]));
                    continue;
                }

                var run = Run(plain, i);
                var close = ClosingRun(plain, i + run, run);
                var content = close < 0 ? null : plain[(i + run)..close];
                if (content == null || content.Trim().Length == 0 || content.Contains("\\|"))
                {
                    // Not a code span (or one GFM can't keep in a table cell): every character is literal.
                    var end = close < 0 ? i + run : close + run;
                    foreach (var c in plain[i..end])
                    {
                        text.Append(Escape(c));
                    }

                    i = end;
                    continue;
                }

                var fence = new string('`', run);
                text.Append(fence).Append(content.Replace("|", "\\|")).Append(fence);
                i = close + run;
            }

            return text.ToString();
        }

        private static string Escape(char c) => c switch
        {
            '{' => "&#123;",
            '}' => "&#125;",
            '<' => "&lt;",
            '>' => "&gt;",
            '&' => "&amp;",
            '|' => "\\|",
            '`' or '\\' or '*' or '_' or '[' or ']' or '~' or '#' => "\\" + c,
            _ => c.ToString()
        };

        /// <summary>How many backticks start at <paramref name="start"/>.</summary>
        private static int Run(string text, int start)
        {
            var end = start;
            while (end < text.Length && text[end] == '`')
            {
                end++;
            }

            return end - start;
        }

        /// <summary>Where the next run of exactly <paramref name="length"/> backticks from <paramref name="from"/> starts; -1 for none.</summary>
        private static int ClosingRun(string text, int from, int length)
        {
            var i = from;
            while (i < text.Length)
            {
                if (text[i] != '`')
                {
                    i++;
                    continue;
                }

                var run = Run(text, i);
                if (run == length)
                {
                    return i;
                }

                i += run;
            }

            return -1;
        }

        /// <summary>The heading's id in the document viewer, as GitHub makes it: lower case, punctuation dropped, spaces as hyphens.</summary>
        public static string Slug(string heading)
        {
            var slug = new StringBuilder();
            foreach (var c in heading.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(c) || c is '-' or '_')
                {
                    slug.Append(c);
                }
                else if (c == ' ')
                {
                    slug.Append('-');
                }
            }

            return slug.ToString();
        }

        private static string Attribute(string value) => (value ?? string.Empty).Replace("&", "&amp;").Replace("\"", "&quot;");
    }
}
