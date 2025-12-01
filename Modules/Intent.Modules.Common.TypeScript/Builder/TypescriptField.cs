using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptField : TypescriptMember<TypescriptField>
{
    public string Type { get; }
    public string Name { get; }
    public string Value { get; private set; }
    public string AccessModifier { get; private set; } = string.Empty;
    public bool IsDefinitelyAssigned { get; private set; } = false;


    public TypescriptField(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Type = type;
        Name = name;
    }

    public TypescriptField(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public TypescriptField PrivateReadOnly()
    {
        AccessModifier = "private readonly ";
        return this;
    }

    public TypescriptField Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public TypescriptField DefinitelyAssigned()
    {
        IsDefinitelyAssigned = true;
        return this;
    }

    public TypescriptField WithValue(string value)
    {
        Value = value;
        return this;
    }

    public TypescriptField WithDefaultValue(IntentTemplateBase template, ITypeReference property)
    {
        // TODO - this must be better
        Value = GetPropertyDefaultValue(template, property, "  ", "  ");
        return this;
    }

    public TypescriptField WithDefaultValue(string defaultValue)
    {
        Value = defaultValue;
        return this;
    }

    public override string GetText(string indentation)
    {
        // TODO. The ! operation screws up the weaving
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{Name}{(!string.IsNullOrWhiteSpace(Type) ? $": {Type}" : string.Empty)}{(!string.IsNullOrWhiteSpace(Value) ? $" = {Value}" : string.Empty)};";
        //return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{Name}{(IsDefinitelyAssigned ? "!" : string.Empty)}{(!string.IsNullOrWhiteSpace(Type) ? $": {Type}" : string.Empty)}{(!string.IsNullOrWhiteSpace(Value) ? $" = {Value}" : string.Empty )};";
    }

    private string GetPropertyDefaultValue(IntentTemplateBase template, ITypeReference property, string indentation, string currentIndentation)
    {
        return template.GetTypeName(property) switch
        {
            "boolean" => "false",
            "Date" => "null",
            "number" => "0",
            "any" => "null",
            "string" => "''",
            _ => BuildComplexDefaultValue(template, property.Element as IElement, indentation, indentation + indentation)
        };
    }

    private string BuildComplexDefaultValue(IntentTemplateBase template, IElement property, string indentation, string currentIndentation)
    {
        var builder = new StringBuilder();

        builder.AppendLine("{");

        foreach (var child in property.ChildElements)
        {
            builder.AppendLine($"{currentIndentation}{child.Name.ToCamelCase()}: {GetPropertyDefaultValue(template, child.TypeReference, indentation, currentIndentation)},");
        }

        builder.Remove(builder.Length - 3, 1);
        builder.Append($"{currentIndentation.Remove(currentIndentation.IndexOf(indentation), indentation.Length)}}}");

        return builder.ToString();
    }
}