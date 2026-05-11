using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownSection
{
    string Title { get; set; }
    int HeadingLevel { get; set; }
    IReadOnlyList<IMarkdownBlock> Blocks { get; }
    IReadOnlyList<IMarkdownListItem> ListItems { get; }

    IMarkdownSection WithText(string text);
    IMarkdownSection WithCodeBlock(string code, string language = null, string title = null);
    IMarkdownSection WithListItem(string content, Action<IMarkdownListItem> configure = null);
    IMarkdownSection WithOrderedListItem(string content, Action<IMarkdownListItem> configure = null);

    /// <summary>
    /// Parses a raw markdown list string and appends each item. Supports both
    /// unordered (<c>- </c>, <c>* </c>, <c>+ </c>) and ordered (<c>1. </c>) items.
    /// </summary>
    IMarkdownSection WithListItems(string markdownList);

    /// <summary>
    /// Inserts a new list item immediately after the last item matching <paramref name="predicate"/>.
    /// </summary>
    IMarkdownSection AfterListItem(Func<IMarkdownListItem, bool> predicate, string content, Action<IMarkdownListItem> configure = null);

    /// <summary>
    /// Removes all list items matching <paramref name="predicate"/>.
    /// </summary>
    IMarkdownSection RemoveListItem(Func<IMarkdownListItem, bool> predicate);
}
