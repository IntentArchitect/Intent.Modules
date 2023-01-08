using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptImport
{
    public string Source { get; }
    public string DefaultExport { get; private set; }
    public Dictionary<string, string> ExportsByName { get; } = new();

    public TypescriptImport(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(source));
        }

        Source = source;
    }

    public TypescriptImport HasDefaultExport(string defaultExport)
    {
        DefaultExport = defaultExport;

        return this;
    }

    public TypescriptImport HasExport(string name, string alias = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        ExportsByName[name] = alias;

        return this;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(DefaultExport))
        {
            sb.Append(DefaultExport);
        }

        if (ExportsByName.Count > 0 && !string.IsNullOrWhiteSpace(DefaultExport))
        {
            sb.Append(", ");
        }

        if (ExportsByName.Count > 0)
        {
            var exports = ExportsByName.Select(x => !string.IsNullOrWhiteSpace(x.Value)
                ? $"{x.Key} as {x.Value}"
                : x.Key);

            sb.Append($"{{ {string.Join(", ", exports)} }}");
        }

        sb.Append($" from '{Source}';");

        return sb.ToString();
    }
}