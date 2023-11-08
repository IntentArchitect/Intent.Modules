using System.Globalization;

namespace Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;

public class DataFileOclWriter : DataFileWriter
{
    public DataFileOclWriter() : base("  ", "#")
    {
    }

    public override void Visit(IDataFileObjectValue @object)
    {
        if (@object.Parent != null)
        {
            PushIndentation();
            StringBuilder.AppendLine("{");
        }

        var index = 0;
        foreach (var (name, value) in @object)
        {
            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            if (value is not IDataFileScalarValue && (index != 0 || @object.Parent != null))
            {
                StringBuilder.AppendLine();
            }

            StringBuilder.Append(Indentation);
            StringBuilder.Append(name);
            StringBuilder.Append(' ');

            Visit(value);

            StringBuilder.AppendLine();

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }

            index++;
        }

        if (@object.Parent != null)
        {
            PopIndentation();
            StringBuilder.Append(Indentation);
            StringBuilder.Append("}");
        }
    }

    public override void Visit(IDataFileArrayValue array)
    {
        if (array.Parent is not IDataFileArrayValue)
        {
            StringBuilder.Append("= ");
        }

        StringBuilder.Append("[");

        for (var index = 0; index < array.Count; index++)
        {
            var value = array[index];
            if (value.IsCommentedOut)
            {
                PushIndentation(CommentSyntax);
            }

            Visit(value);
            if (index != array.Count - 1)
            {
                StringBuilder.Append(", ");
            }

            if (value.IsCommentedOut)
            {
                PopIndentation();
            }
        }

        StringBuilder.Append("]");
    }

    public override void Visit(IDataFileScalarValue scalar)
    {
        if (scalar.Parent is not IDataFileArrayValue)
        {
            StringBuilder.Append("= ");
        }

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
}