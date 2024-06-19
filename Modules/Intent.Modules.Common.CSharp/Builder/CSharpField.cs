using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpField : CSharpMember<CSharpField>, ICSharpField
{
    private bool _canBeNull;
    private readonly ICSharpField _wrapping;
    public string Type { get; }
    public string Name { get; }
    public string AccessModifier { get; private set; }
    public CSharpStatement Assignment { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsOmittedFromRender { get; private set; } 

    public CSharpField(string type, string name, ICSharpCodeContext @class) : this (type, name, @class, null)
    {
    }

    public CSharpField(string type, string name, ICSharpCodeContext @class, string? value)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapping = new CSharpFieldWrapper(this);
        Assignment = value;
        AccessModifier = "private ";
        Type = type;
        Name = name;
        Parent = @class;    
    }

    internal static CSharpField CreateFieldOmittedFromRender(string type, string name, ICSharpCodeContext @class, string? value)
    {
        var field = new CSharpField(type, name, @class, value);
        field.IsOmittedFromRender = true;
        return field;
    }

    public CSharpField ProtectedReadOnly()
    {
        AccessModifier = "protected readonly ";
        return this;
    }

    public CSharpField Protected() => Protected(null);

    public CSharpField Protected(string value)
    {
        AccessModifier = "protected ";
        Assignment = value;
        return this;
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
        var assignment = string.Empty;
        if (Assignment is not null)
        {
            var result = Assignment.GetText(indentation).TrimStart();
            if (!string.IsNullOrWhiteSpace(result))
            {
                assignment = $" = {result}";
            }
        }

        var accessModifier = AccessModifier;
        if (IsStatic)
        {
            if (accessModifier.StartsWith("public "))
            {
                accessModifier = accessModifier.Replace("public ", "public static ");
            }
            else if (accessModifier.StartsWith("private "))
            {
                accessModifier = accessModifier.Replace("private ", "private static ");
            }
            else if (accessModifier.StartsWith("internal "))
            {
                accessModifier = accessModifier.Replace("internal ", "internal static ");
            }
            else
            {
                accessModifier = $"static {accessModifier}";
            }
        }

        return $"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{accessModifier}{(IsRequired ? "required " : "")}{Type}{(_canBeNull ? "?" : "")} {Name}{assignment};";
    }

    #region ICSharpField implementation

    ICSharpField ICSharpDeclaration<ICSharpField>.AddAttribute(string name, Action<ICSharpAttribute> configure = null)
    {
        return _wrapping.AddAttribute(name, configure);
    }

    ICSharpField ICSharpDeclaration<ICSharpField>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure = null)
    {
        return _wrapping.AddAttribute(attribute, configure);
    }

    ICSharpField ICSharpDeclaration<ICSharpField>.WithComments(string xmlComments)
    {
        return _wrapping.WithComments(xmlComments);
    }

    ICSharpField ICSharpDeclaration<ICSharpField>.WithComments(IEnumerable<string> xmlComments)
    {
        return _wrapping.WithComments(xmlComments);
    }

    ICSharpField ICSharpField.Protected()
    {
        return _wrapping.Protected();
    }

    ICSharpField ICSharpField.Protected(string value)
    {
        return _wrapping.Protected(value);
    }

    ICSharpField ICSharpField.PrivateReadOnly()
    {
        return _wrapping.PrivateReadOnly();
    }

    ICSharpField ICSharpField.Private()
    {
        return _wrapping.Private();
    }

    ICSharpField ICSharpField.Private(string value)
    {
        return _wrapping.Private(value);
    }

    ICSharpField ICSharpField.Constant(string value)
    {
        return _wrapping.Constant(value);
    }

    ICSharpField ICSharpField.PrivateConstant(string value)
    {
        return _wrapping.PrivateConstant(value);
    }

    ICSharpField ICSharpField.Static()
    {
        return _wrapping.Static();
    }

    ICSharpField ICSharpField.Required()
    {
        return _wrapping.Required();
    }

    ICSharpField ICSharpField.CanBeNull()
    {
        return _wrapping.CanBeNull();
    }

    ICSharpField ICSharpField.WithAssignment(ICSharpStatement value)
    {
        return _wrapping.WithAssignment(value);
    }

    ICSharpField ICSharpField.ProtectedReadOnly()
    {
        return _wrapping.ProtectedReadOnly();
    }

    #endregion
}