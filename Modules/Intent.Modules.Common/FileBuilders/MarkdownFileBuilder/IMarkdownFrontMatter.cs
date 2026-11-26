using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownFrontMatter
{
    /// <summary>
    /// Each key with the value on its own line. For a value YAML spreads over the following indented
    /// lines, such as a block list, <c>Value</c> is whatever follows the colon on the key's line
    /// (usually empty); read a list with <see cref="GetList"/>.
    /// </summary>
    IReadOnlyList<(string Key, string Value)> Entries { get; }

    /// <summary>
    /// Sets <paramref name="key"/> to a one-line value, written verbatim after <c>key: </c>.
    /// Replaces any existing value, including a list.
    /// </summary>
    IMarkdownFrontMatter Set(string key, string value);

    /// <summary>
    /// Sets <paramref name="key"/> to a YAML block list, one <c>  - item</c> line per value. An item is
    /// double-quoted only when YAML would otherwise read it as something other than a string, such as
    /// <c>"**/*.razor"</c>. An empty list is written as <c>key: []</c>.
    /// Replaces any existing value.
    /// </summary>
    IMarkdownFrontMatter SetList(string key, IEnumerable<string> values);

    IMarkdownFrontMatter Remove(string key);
    bool HasKey(string key);

    /// <summary>
    /// The text after <c>key:</c> on the key's own line, or <c>null</c> when the key is absent.
    /// </summary>
    string Get(string key);

    /// <summary>
    /// The items of a block list (<c>  - a</c> lines) or an inline list (<c>[a, b]</c>), unquoted.
    /// <c>null</c> when the key is absent or its value is not a list.
    /// </summary>
    IReadOnlyList<string> GetList(string key);
}
