using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownSection : IMarkdownSection
{
    private readonly List<IMarkdownBlock> _blocks = new();

    public MarkdownSection(string title, int headingLevel = 2)
    {
        Title = title;
        HeadingLevel = headingLevel;
    }

    public string Title { get; set; }
    public int HeadingLevel { get; set; }
    public IReadOnlyList<IMarkdownBlock> Blocks => _blocks;
    public IReadOnlyList<IMarkdownListItem> ListItems => _blocks.OfType<IMarkdownListItem>().ToList();

    public IMarkdownSection WithText(string text)
    {
        _blocks.Add(new MarkdownTextBlock(text));
        return this;
    }

    public IMarkdownSection WithCodeBlock(string code, string language = null, string title = null)
    {
        _blocks.Add(new MarkdownCodeBlock(code, language, title));
        return this;
    }

    public IMarkdownSection WithListItem(string content, Action<IMarkdownListItem> configure = null)
    {
        var item = new MarkdownListItem(content, isOrdered: false);
        _blocks.Add(item);
        configure?.Invoke(item);
        return this;
    }

    public IMarkdownSection WithOrderedListItem(string content, Action<IMarkdownListItem> configure = null)
    {
        var item = new MarkdownListItem(content, isOrdered: true);
        _blocks.Add(item);
        configure?.Invoke(item);
        return this;
    }

    public IMarkdownSection WithListItems(string markdownList)
    {
        // Parsed by the same code as FromMarkdown, so a wrapped item keeps its continuation lines and
        // an emphasis-led line ("**Note** ...") or a rule ("---") is not mistaken for a bullet.
        var parsed = new MarkdownSection(Title, HeadingLevel);
        MarkdownFileParser.ParseSectionContent(markdownList ?? string.Empty, parsed);
        _blocks.AddRange(parsed.Blocks);
        return this;
    }

    public IMarkdownSection AfterListItem(Func<IMarkdownListItem, bool> predicate, string content, Action<IMarkdownListItem> configure = null)
    {
        var index = _blocks.FindLastIndex(b => b is IMarkdownListItem li && predicate(li));
        if (index < 0)
            throw new InvalidOperationException($"No list item matching the predicate was found in section '{Title}'.");

        var sibling = (IMarkdownListItem)_blocks[index];
        var newItem = new MarkdownListItem(content, isOrdered: sibling.IsOrdered, indentLevel: sibling.IndentLevel);
        _blocks.Insert(index + 1, newItem);
        configure?.Invoke(newItem);
        return this;
    }

    public IMarkdownSection RemoveListItem(Func<IMarkdownListItem, bool> predicate)
    {
        _blocks.RemoveAll(b => b is IMarkdownListItem li && predicate(li));
        return this;
    }

    internal void AddBlock(IMarkdownBlock block) => _blocks.Add(block);
}
