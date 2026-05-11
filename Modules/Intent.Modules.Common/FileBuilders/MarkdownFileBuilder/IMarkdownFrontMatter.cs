using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownFrontMatter
{
    IReadOnlyList<(string Key, string Value)> Entries { get; }

    IMarkdownFrontMatter Set(string key, string value);
    IMarkdownFrontMatter Remove(string key);
    bool HasKey(string key);
    string Get(string key);
}
