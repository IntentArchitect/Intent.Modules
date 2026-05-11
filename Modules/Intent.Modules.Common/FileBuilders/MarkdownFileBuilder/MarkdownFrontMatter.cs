using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public class MarkdownFrontMatter : IMarkdownFrontMatter
{
    private readonly List<(string Key, string Value)> _entries = new();

    public IReadOnlyList<(string Key, string Value)> Entries => _entries;

    public IMarkdownFrontMatter Set(string key, string value)
    {
        var index = _entries.FindIndex(e => e.Key == key);
        if (index >= 0)
            _entries[index] = (key, value);
        else
            _entries.Add((key, value));
        return this;
    }

    public IMarkdownFrontMatter Remove(string key)
    {
        _entries.RemoveAll(e => e.Key == key);
        return this;
    }

    public bool HasKey(string key) => _entries.Any(e => e.Key == key);

    public string Get(string key)
    {
        var entry = _entries.FirstOrDefault(e => e.Key == key);
        return entry.Key != null ? entry.Value : null;
    }

    public override string ToString()
    {
        if (_entries.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("---");
        foreach (var (key, value) in _entries)
            sb.AppendLine($"{key}: {value}");
        sb.Append("---");
        return sb.ToString();
    }
}
