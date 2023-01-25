using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaField : JavaMember<JavaField>
{
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }

    public JavaField(string type, string name)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        AccessModifier = "private ";
        Type = type;
        Name = name;
    }

    public JavaField Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAnnotations(indentation)}{indentation}{AccessModifier}{Type} {Name};";
    }
}