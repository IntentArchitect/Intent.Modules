using System.Globalization;
using System.Linq;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

public class DataFileJsonWriter : DataFileWriter
{
    public DataFileJsonWriter(string tab = "  ") : base(tab, "//")
    {
    }

    public override void Visit(IDataFileObjectValue @object)
    {
        if (@object.Parent != null)
        {
            PushIndentation();
        }

        StringBuilder.AppendLine("{");

        var index = 0;
        foreach (var (name, value) in @object)
        {
            PushIndentation();

            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            StringBuilder.Append(Indentation);

            StringBuilder.Append("\"");
            StringBuilder.Append(name);
            StringBuilder.Append("\": ");

            Visit(value);

            if (index != @object.Count - 1)
            {
                StringBuilder.Append(",");
            }

            StringBuilder.AppendLine();

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }

            PopIndentation();

            index++;
        }

        StringBuilder.Append(Indentation);
        StringBuilder.Append("}");

        if (@object.Parent != null)
        {
            PopIndentation();
        }
    }

    public override void Visit(IDataFileArrayValue array)
    {
        if (array.Parent != null)
        {
            PushIndentation();
        }

        StringBuilder.AppendLine("[");

        for (var index = 0; index < array.Count; index++)
        {
            PushIndentation();

            var value = array[index];
            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            StringBuilder.Append(Indentation);
            Visit(value);
            if (index != array.Count - 1)
            {
                StringBuilder.Append(',');
            }

            StringBuilder.AppendLine();

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }

            PopIndentation();
        }

        StringBuilder.Append(Indentation);
        StringBuilder.Append("]");

        if (array.Parent != null)
        {
            PopIndentation();
        }
    }

    public override void Visit(IDataFileScalarValue scalar)
    {
        StringBuilder.Append(scalar.Value switch
        {
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
            string castValue => $"\"{castValue}\"",
            _ => scalar.Value.ToString()
        });
    }

    public override string ToString()
    {
        return StringBuilder.ToString();
    }
}