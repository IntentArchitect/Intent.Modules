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
    private ImportType _importType = ImportType.Unspecified;

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

        if (name == "*")
        {
            if (_importType is not (ImportType.Unspecified or ImportType.Namespace))
            {
                throw new InvalidOperationException($"Cannot add a namespace import when an imported is already {_importType}");
            }

            _importType = ImportType.Namespace;
        }
        else
        {
            if (_importType is not (ImportType.Unspecified or ImportType.Named))
            {
                throw new InvalidOperationException($"Cannot add a named import when an imported is already {_importType}");
            }

            _importType = ImportType.Named;
        }

        ExportsByName[name] = alias;

        return this;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("import ");

        if (!string.IsNullOrWhiteSpace(DefaultExport))
        {
            sb.Append(DefaultExport);

            if (_importType != ImportType.Unspecified)
            {
                sb.Append(", ");
            }
        }

        if (ExportsByName.Count > 0)
        {
            var imports = ExportsByName
                .Select(x => !string.IsNullOrWhiteSpace(x.Value)
                    ? $"{x.Key} as {x.Value}"
                    : x.Key)
                .ToArray();

            var toAppend = _importType switch
            {
                ImportType.Named => $"{{ {string.Join(", ", imports)} }}",
                ImportType.Namespace => string.Join(", ", imports),
                _ => throw new ArgumentOutOfRangeException()
            };

            sb.Append(toAppend);
        }

        sb.Append($" from '{Source}';");

        return sb.ToString();
    }

    private enum ImportType
    {
        Unspecified,
        Named,
        Namespace
    }
}