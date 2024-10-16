using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

public class DataFileYamlWriter : DataFileWriter
{
    private readonly bool _alwaysQuoteStrings;

    public DataFileYamlWriter(bool alwaysQuoteStrings) : base("  ", "# ")
    {
        _alwaysQuoteStrings = alwaysQuoteStrings;
    }

    public override void Visit(IDataFileObjectValue @object)
    {
        if (@object.Parent != null)
        {
            PushIndentation();
        }

        var index = 0;
        foreach (var (name, value) in @object)
        {
            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            if (index == 0)
            {
                switch (@object.Parent)
                {
                    case DataFileArrayValue:
                        StringBuilder.Append(' ');
                        break;
                    case DataFileObjectValue:
                        StringBuilder.AppendLine();
                        StringBuilder.Append(Indentation);
                        break;
                    case null:
                        StringBuilder.Append(Indentation);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                if (@object.BlankLinesBetweenItems)
                {
                    StringBuilder.AppendLine();
                }

                StringBuilder.Append(Indentation);
            }

            StringBuilder.Append(name);
            StringBuilder.Append(':');

            Visit(value);

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }

            index++;
        }

        if (@object.Parent != null)
        {
            PopIndentation();
        }
    }

    public override void Visit(IDataFileArrayValue array)
    {
        if (array.Parent is DataFileArrayValue)
        {
            PushIndentation();
        }

        for (var index = 0; index < array.Count; index++)
        {
            var value = array[index];
            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            if (index == 0)
            {
                switch (array.Parent)
                {
                    case DataFileArrayValue:
                        StringBuilder.Append(CommentSyntax);
                        StringBuilder.Append(' ');
                        break;
                    default:
                        StringBuilder.AppendLine();
                        StringBuilder.Append(Indentation);
                        break;
                }
            }
            else
            {
                if (array.BlankLinesBetweenItems)
                {
                    StringBuilder.AppendLine();
                }

                StringBuilder.Append(Indentation);
            }

            StringBuilder.Append('-');
            Visit(value);

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }
        }

        if (array.Parent is DataFileArrayValue)
        {
            PopIndentation();
        }
    }

    public override void Visit(IDataFileScalarValue scalar)
    {
        StringBuilder.Append(' ');

        var isStringValue = scalar.Value is string;
        var stringValue = isStringValue ? (string)scalar.Value : null;
        if (isStringValue && stringValue.ReplaceLineEndings("\n").Contains('\n'))
        {
            StringBuilder.AppendLine("|");
            PushIndentation();
            foreach (var line in stringValue.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    StringBuilder.Append(Indentation);
                }

                StringBuilder.AppendLine(line);
            }
            PopIndentation();
        }
        else if (isStringValue)
        {
            var quoteStrings = _alwaysQuoteStrings ||
                               long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ||
                               decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ||
                               bool.TryParse(stringValue, out _) ||
                               string.Equals(stringValue, "yes", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(stringValue, "no", StringComparison.OrdinalIgnoreCase) ||
                               stringValue.Any(x => x is '\"' or '\\');
            if (quoteStrings)
            {
                StringBuilder.Append('\'');
            }

            StringBuilder.Append(stringValue);

            if (quoteStrings)
            {
                StringBuilder.Append('\'');
            }

            StringBuilder.AppendLine();
        }
        else
        {
            StringBuilder.Append(scalar.Value switch
            {
                null => "null",
                bool castValue => castValue.ToString(CultureInfo.InvariantCulture).ToLowerInvariant(),
                byte castValue => castValue.ToString(CultureInfo.InvariantCulture),
                sbyte castValue => castValue.ToString(CultureInfo.InvariantCulture),
                char castValue => castValue.ToString(CultureInfo.InvariantCulture),
                decimal castValue => castValue.ToString(CultureInfo.InvariantCulture),
                double castValue => castValue.ToString(CultureInfo.InvariantCulture),
                float castValue => castValue.ToString(CultureInfo.InvariantCulture),
                int castValue => castValue.ToString(CultureInfo.InvariantCulture),
                uint castValue => castValue.ToString(CultureInfo.InvariantCulture),
                long castValue => castValue.ToString(CultureInfo.InvariantCulture),
                ulong castValue => castValue.ToString(CultureInfo.InvariantCulture),
                short castValue => castValue.ToString(CultureInfo.InvariantCulture),
                ushort castValue => castValue.ToString(CultureInfo.InvariantCulture),
                _ => scalar.Value.ToString()
            });

            StringBuilder.AppendLine();
        }
    }
}