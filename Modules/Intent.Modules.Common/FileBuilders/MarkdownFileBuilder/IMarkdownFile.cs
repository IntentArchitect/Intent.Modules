using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownFile : IFileBuilderBase<IMarkdownFile>
{
    IMarkdownFrontMatter FrontMatter { get; }
    IReadOnlyList<IMarkdownSection> Sections { get; }

    /// <summary>
    /// Parses <paramref name="markdown"/> and populates front matter and sections immediately.
    /// Call this before any augmentation methods.
    /// </summary>
    IMarkdownFile FromMarkdown(string markdown);

    IMarkdownFile WithFrontMatter(Action<IMarkdownFrontMatter> configure);

    /// <summary>
    /// Gets the existing section with <paramref name="title"/>, or creates it at the end.
    /// </summary>
    IMarkdownFile WithSection(string title, Action<IMarkdownSection> configure = null, int headingLevel = 2);

    /// <summary>
    /// Inserts a new section immediately after the section titled <paramref name="existingTitle"/>.
    /// When <paramref name="headingLevel"/> is 0 (default) the new section inherits the anchor's heading level.
    /// Falls back to appending at the end if the anchor section is not found.
    /// </summary>
    IMarkdownFile AfterSection(string existingTitle, string newTitle, Action<IMarkdownSection> configure = null, int headingLevel = 0);

    /// <summary>
    /// Inserts a new section immediately before the section titled <paramref name="existingTitle"/>.
    /// When <paramref name="headingLevel"/> is 0 (default) the new section inherits the anchor's heading level.
    /// Falls back to appending at the end if the anchor section is not found.
    /// </summary>
    IMarkdownFile BeforeSection(string existingTitle, string newTitle, Action<IMarkdownSection> configure = null, int headingLevel = 0);

    /// <summary>
    /// Finds the existing section titled <paramref name="title"/> and passes it to <paramref name="configure"/>.
    /// Throws if the section does not exist.
    /// </summary>
    IMarkdownFile ConfigureSection(string title, Action<IMarkdownSection> configure);

    IMarkdownFile RemoveSection(string title);

    bool HasSection(string title);
    IMarkdownSection FindSection(string title);
}
