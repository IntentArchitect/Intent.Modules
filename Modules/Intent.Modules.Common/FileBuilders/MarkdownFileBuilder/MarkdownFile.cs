using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownFile : FileBuilderBase<IMarkdownFile>, IMarkdownFile
{
    private readonly List<IMarkdownSection> _sections = new();
    private readonly MarkdownFrontMatter _frontMatter = new();

    public MarkdownFile(
        string fileName,
        string extension = "md",
        string relativeLocation = null,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always)
        : base(fileName, relativeLocation, extension, overwriteBehaviour)
    {
    }

    public IMarkdownFrontMatter FrontMatter => _frontMatter;
    public IReadOnlyList<IMarkdownSection> Sections => _sections;

    public IMarkdownFile FromMarkdown(string markdown)
    {
        MarkdownFileParser.Parse(markdown, _frontMatter, _sections);
        return this;
    }

    public IMarkdownFile WithFrontMatter(Action<IMarkdownFrontMatter> configure)
    {
        OnBuild(_ => configure(_frontMatter));
        return this;
    }

    public IMarkdownFile WithSection(string title, Action<IMarkdownSection> configure = null, int headingLevel = 2)
    {
        OnBuild(_ =>
        {
            var section = GetOrCreateSection(title, headingLevel);
            configure?.Invoke(section);
        });
        return this;
    }

    public IMarkdownFile AfterSection(string existingTitle, string newTitle, Action<IMarkdownSection> configure = null, int headingLevel = 0)
    {
        OnBuild(_ =>
        {
            var index = IndexOf(existingTitle);
            var level = headingLevel > 0 ? headingLevel : (index >= 0 ? _sections[index].HeadingLevel : 2);
            var newSection = new MarkdownSection(NormalizeTitle(newTitle), level);
            if (index < 0)
                _sections.Add(newSection);
            else
                _sections.Insert(index + 1, newSection);
            configure?.Invoke(newSection);
        });
        return this;
    }

    public IMarkdownFile BeforeSection(string existingTitle, string newTitle, Action<IMarkdownSection> configure = null, int headingLevel = 0)
    {
        OnBuild(_ =>
        {
            var index = IndexOf(existingTitle);
            var level = headingLevel > 0 ? headingLevel : (index >= 0 ? _sections[index].HeadingLevel : 2);
            var newSection = new MarkdownSection(NormalizeTitle(newTitle), level);
            if (index < 0)
                _sections.Add(newSection);
            else
                _sections.Insert(index, newSection);
            configure?.Invoke(newSection);
        });
        return this;
    }

    public IMarkdownFile ConfigureSection(string title, Action<IMarkdownSection> configure)
    {
        OnBuild(_ =>
        {
            var normalized = NormalizeTitle(title);
            var section = _sections.FirstOrDefault(s => s.Title.Equals(normalized, StringComparison.OrdinalIgnoreCase));
            if (section == null)
                throw new InvalidOperationException($"Section '{title}' not found in file '{_fileName}'.");
            configure(section);
        });
        return this;
    }

    public IMarkdownFile RemoveSection(string title)
    {
        OnBuild(_ =>
        {
            var normalized = NormalizeTitle(title);
            _sections.RemoveAll(s => s.Title.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        });
        return this;
    }

    public bool HasSection(string title) =>
        _sections.Any(s => s.Title.Equals(NormalizeTitle(title), StringComparison.OrdinalIgnoreCase));

    public IMarkdownSection FindSection(string title) =>
        _sections.FirstOrDefault(s => s.Title.Equals(NormalizeTitle(title), StringComparison.OrdinalIgnoreCase));

    public override string ToString()
    {
        var sb = new StringBuilder();

        var frontMatterStr = _frontMatter.ToString();
        if (!string.IsNullOrEmpty(frontMatterStr))
        {
            sb.AppendLine(frontMatterStr);
        }

        foreach (var section in _sections)
        {
            sb.Append(new string('#', section.HeadingLevel));
            sb.Append(' ');
            sb.AppendLine(section.Title);
            sb.AppendLine();

            var blocks = section.Blocks;
            var orderedCounter = 1;
            var lastListRunWasOrdered = false;

            for (var i = 0; i < blocks.Count;)
            {
                var block = blocks[i];

                if (block is IMarkdownListItem firstItem)
                {
                    // Reset ordered counter only when starting a fresh ordered list (not continuing
                    // one that was interrupted by a text block such as indented sub-bullets).
                    if (!firstItem.IsOrdered || !lastListRunWasOrdered)
                        orderedCounter = 1;

                    while (i < blocks.Count && blocks[i] is IMarkdownListItem li)
                    {
                        var indent = new string(' ', li.IndentLevel * 2);
                        if (li.IsOrdered && li.IndentLevel == 0)
                        {
                            sb.AppendLine($"{indent}{orderedCounter++}. {li.Content}");
                            lastListRunWasOrdered = true;
                        }
                        else if (li.IsOrdered)
                        {
                            sb.AppendLine($"{indent}{li.Content}");
                        }
                        else
                        {
                            if (li.IndentLevel == 0)
                            {
                                orderedCounter = 1;
                                lastListRunWasOrdered = false;
                            }
                            sb.AppendLine($"{indent}- {li.Content}");
                        }
                        i++;
                    }
                    sb.AppendLine();
                }
                else if (block is IMarkdownTextBlock text)
                {
                    sb.AppendLine(text.Text);
                    sb.AppendLine();
                    i++;
                    // lastListRunWasOrdered intentionally preserved — text between ordered
                    // list items (e.g. indented sub-bullets) should not reset the counter.
                }
                else if (block is IMarkdownCodeBlock code)
                {
                    if (!string.IsNullOrEmpty(code.Title))
                    {
                        sb.AppendLine($"**{code.Title}**");
                    }
                    sb.Append("```");
                    if (!string.IsNullOrEmpty(code.Language)) sb.Append(code.Language);
                    sb.AppendLine();
                    sb.AppendLine(code.Code);
                    sb.AppendLine("```");
                    sb.AppendLine();
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }

        return sb.ToString().TrimEnd() + Environment.NewLine;
    }

    // Allows callers to pass either "## My Section" or "My Section" interchangeably.
    private static string NormalizeTitle(string title) =>
        title.TrimStart('#').TrimStart();

    private MarkdownSection GetOrCreateSection(string title, int headingLevel)
    {
        var normalized = NormalizeTitle(title);
        if (_sections.FirstOrDefault(s => s.Title.Equals(normalized, StringComparison.OrdinalIgnoreCase)) is MarkdownSection existing)
            return existing;

        var section = new MarkdownSection(normalized, headingLevel);
        _sections.Add(section);
        return section;
    }

    private int IndexOf(string title)
    {
        var normalized = NormalizeTitle(title);
        return _sections.FindIndex(s => s.Title.Equals(normalized, StringComparison.OrdinalIgnoreCase));
    }
}
