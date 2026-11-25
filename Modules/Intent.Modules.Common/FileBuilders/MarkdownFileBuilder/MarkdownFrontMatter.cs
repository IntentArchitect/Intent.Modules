using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public partial class MarkdownFrontMatter : IMarkdownFrontMatter
{
    // Each entry is a "key: value" line plus, for a value YAML spreads over several lines (a block list,
    // a nested map, a "|" or ">" block scalar), those following lines verbatim. A one-line entry has
    // no Lines and renders exactly as it always has.
    private sealed class Entry
    {
        public Entry(string key, string value, List<string> lines = null)
        {
            Key = key;
            Value = value;
            Lines = lines;
        }

        public string Key { get; }
        public string Value { get; }
        public List<string> Lines { get; }
    }

    private readonly List<Entry> _entries = new();

    [GeneratedRegex(@"^\s*-(?:\s+(.*))?$")]
    private static partial Regex BlockListItemRegex();

    public IReadOnlyList<(string Key, string Value)> Entries => _entries.Select(e => (e.Key, e.Value)).ToList();

    public IMarkdownFrontMatter Set(string key, string value)
    {
        Upsert(new Entry(key, value));
        return this;
    }

    public IMarkdownFrontMatter SetList(string key, IEnumerable<string> values)
    {
        var items = values?.ToList() ?? new List<string>();
        Upsert(items.Count == 0
            ? new Entry(key, "[]")
            : new Entry(key, string.Empty, items.Select(v => "  - " + QuoteIfNeeded(v)).ToList()));
        return this;
    }

    public IMarkdownFrontMatter Remove(string key)
    {
        _entries.RemoveAll(e => e.Key == key);
        return this;
    }

    public bool HasKey(string key) => _entries.Any(e => e.Key == key);

    public string Get(string key) => _entries.FirstOrDefault(e => e.Key == key)?.Value;

    public IReadOnlyList<string> GetList(string key)
    {
        var entry = _entries.FirstOrDefault(e => e.Key == key);
        if (entry == null)
            return null;

        if (entry.Lines is { Count: > 0 })
        {
            if (!string.IsNullOrWhiteSpace(entry.Value))
                return null;

            var items = new List<string>();
            foreach (var line in entry.Lines.Where(l => l.Trim().Length > 0))
            {
                var match = BlockListItemRegex().Match(line);
                if (!match.Success)
                    return null; // a nested map or block scalar, not a list
                items.Add(Unquote(match.Groups[1].Value));
            }
            return items;
        }

        var value = entry.Value?.Trim();
        if (value == null || !value.StartsWith('[') || !value.EndsWith(']'))
            return null;

        return SplitInlineList(value[1..^1]).Select(Unquote).ToList();
    }

    public override string ToString()
    {
        if (_entries.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("---");
        foreach (var entry in _entries)
        {
            if (entry.Lines is not { Count: > 0 })
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
                continue;
            }

            sb.AppendLine(string.IsNullOrEmpty(entry.Value) ? $"{entry.Key}:" : $"{entry.Key}: {entry.Value}");
            foreach (var line in entry.Lines)
                sb.AppendLine(line);
        }
        sb.Append("---");
        return sb.ToString();
    }

    // Used by the parser: a key whose value continues onto the indented lines that follow it.
    internal void SetParsed(string key, string value, List<string> lines) =>
        Upsert(new Entry(key, value, lines is { Count: > 0 } ? lines : null));

    private void Upsert(Entry entry)
    {
        var index = _entries.FindIndex(e => e.Key == entry.Key);
        if (index >= 0)
            _entries[index] = entry;
        else
            _entries.Add(entry);
    }

    // Quote a list item only when YAML would otherwise read it as something else: an alias or tag
    // ("**/*.razor" starts with "*"), a flow collection, a comment, a mapping, or a bool/null/number.
    private static string QuoteIfNeeded(string value)
    {
        value ??= string.Empty;
        var needsQuotes =
            value.Length == 0 ||
            value != value.Trim() ||
            "-?:,[]{}#&*!|>'\"%@`".Contains(value[0]) ||
            value.Contains(": ") ||
            value.Contains(" #") ||
            value.EndsWith(':') ||
            value.Any(char.IsControl) ||
            Regex.IsMatch(value, "^(true|false|yes|no|on|off|null|~)$", RegexOptions.IgnoreCase) ||
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

        if (!needsQuotes)
            return value;

        var escaped = value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
        return $"\"{escaped}\"";
    }

    private static string Unquote(string value)
    {
        value = value?.Trim() ?? string.Empty;
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
        {
            var sb = new StringBuilder();
            for (var i = 1; i < value.Length - 1; i++)
            {
                if (value[i] == '\\' && i + 1 < value.Length - 1)
                {
                    i++;
                    sb.Append(value[i] switch { 'n' => '\n', 'r' => '\r', 't' => '\t', var c => c });
                    continue;
                }
                sb.Append(value[i]);
            }
            return sb.ToString();
        }

        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
            return value[1..^1].Replace("''", "'");

        var comment = value.IndexOf(" #", System.StringComparison.Ordinal);
        return comment >= 0 ? value[..comment].TrimEnd() : value;
    }

    private static IEnumerable<string> SplitInlineList(string body)
    {
        var current = new StringBuilder();
        char? quote = null;
        foreach (var c in body)
        {
            if (quote != null)
            {
                if (c == quote) quote = null;
                current.Append(c);
            }
            else if (c is '"' or '\'')
            {
                quote = c;
                current.Append(c);
            }
            else if (c == ',')
            {
                yield return current.ToString();
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.ToString().Trim().Length > 0)
            yield return current.ToString();
    }
}
