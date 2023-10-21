using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField : CSharpMember<CSharpField>
{
    private bool _canBeNull;
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }
    public CSharpStatement Assignment { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsRequired { get; private set; }

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
    
    public CSharpField Static()
    {
        IsStatic = true;
        return this;
    }
    public CSharpField Required()
    {
        IsRequired = true;
        return this;
    }

    public CSharpField CanBeNull()
    {
        _canBeNull = true;
        return this;
    }

    [Obsolete("Make use of WithAssignment(CSharpStatement value)")]
    public CSharpField WithAssignment(string value)
    {
        Assignment = value;
        return this;
    }
    
    public CSharpField WithAssignment(CSharpStatement value)
    {
        Assignment = value;
        return this;
    }

    public override string GetText(string indentation)
    {
        string assignment = string.Empty;
        if (Assignment is not null)
        {
            var result = Assignment.GetText(indentation).TrimStart();
            if (!string.IsNullOrWhiteSpace(result))
            {
                assignment = $" = {result}";
            }
        }

        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsStatic ? "static " : "")}{(IsRequired ? "required " : "")}{Type}{(_canBeNull ? "?" : "")} {Name}{assignment};";
    }
}