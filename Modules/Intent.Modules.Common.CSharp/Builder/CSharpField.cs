using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField : CSharpMember<CSharpField>
{
    private bool _canBeNull;
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }
    public string Assignment { get; private set; }

    public CSharpField(string type, string name) : this (type, name, null)
    {
    }

    public CSharpField(string type, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Assignment = value;
        AccessModifier = "private ";
        Type = type;
        Name = name;
    }

    public CSharpField PrivateReadOnly()
    {
        AccessModifier = "private readonly ";
        return this;
    }

    public CSharpField Private() => Private(null);

    public CSharpField Private(string value)
    {
        AccessModifier = "private ";
        Assignment = value;
        return this;
    }

    public CSharpField Constant(string value)
    {
        AccessModifier = "public const ";
        Assignment = value;
        return this;
    }

    public CSharpField PrivateConstant(string value)
    {
        AccessModifier = "public const ";
        Assignment = value;
        return this;
    }

    public CSharpField CanBeNull()
    {
        _canBeNull = true;
        return this;
    }

    public CSharpField WithAssignment(string value)
    {
        Assignment = value;
        return this;
    }

    public override string GetText(string indentation)
    {
        var assignment = !string.IsNullOrWhiteSpace(Assignment)
            ? $" = {Assignment}"
            : string.Empty;

        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{Type}{(_canBeNull ? "?" : "")} {Name}{assignment};";
    }
}