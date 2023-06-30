using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaField : JavaMember<JavaField>
{
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }
    public string DefaultValue { get; private set; }

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

    public JavaField Public()
    {
        AccessModifier = "public ";
        return this;
    }

    public JavaField Final()
    {
        AccessModifier = "final ";
        return this;
    }
    
    public JavaField PrivateFinal()
    {
        AccessModifier = "private final ";
        return this;
    }
    
    public JavaField PublicFinal()
    {
        AccessModifier = "public final ";
        return this;
    }

    public JavaField WithoutAccessModifier()
    {
        AccessModifier = "";
        return this;
    }
    
    public JavaField WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAnnotations(indentation)}{indentation}{AccessModifier}{Type} {Name}{(DefaultValue != null ? $" = {DefaultValue}" : string.Empty)};";
    }
}