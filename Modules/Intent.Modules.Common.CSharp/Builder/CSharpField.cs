#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.VisualStudio;

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
    public bool IsReadOnly { get; private set; }
    public bool IsConstant { get; private set; }

    public CSharpField(string type, string name, ICSharpCodeContext @class) : this(type, name, @class, null)
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
        File = @class?.File; // can be null because of CSharpInterfaceProperty :(
    }

    internal static CSharpField CreateFieldOmittedFromRender(string type, string name, ICSharpCodeContext @class, string? value)
    {
        var field = new CSharpField(type, name, @class, value);
        field.IsOmittedFromRender = true;
        return field;
    }

    public CSharpField Internal(string? value)
    {
        AccessModifier = "internal ";
        if (value != null)
        {
            Assignment = value;
        }
        return this;
    }

    public CSharpField PrivateReadOnly() => Private().ReadOnly();

    public CSharpField Private() => Private(null);

    public CSharpField Private(string? value)
    {
        AccessModifier = "private ";
        Assignment = value;
        return this;
    }

    public CSharpField PrivateConstant(string value) => Private().Constant(value);

    public CSharpField ProtectedReadOnly() => Protected().ReadOnly();

    public CSharpField Protected() => Protected(null);

    public CSharpField Protected(string? value)
    {
        AccessModifier = "protected ";
        Assignment = value;
        return this;
    }

    public CSharpField ProtectedInternal(string? value = null)
    {
        AccessModifier = "protected internal ";
        if (value != null)
        {
            Assignment = value;
        }
        return this;
    }

    public CSharpField Public(string? value)
    {
        AccessModifier = "public ";
        if (value != null)
        {
            Assignment = value;
        }
        return this;
    }

    public CSharpField ReadOnly(bool readOnly = true)
    {
        IsReadOnly = readOnly;
        return this;
    }

    public CSharpField ReadOnly(string value, bool readOnly = true)
    {
        Assignment = value;
        IsReadOnly = readOnly;
        return this;
    }

    public CSharpField Constant(bool isConstant = true)
    {
        IsConstant = isConstant;
        return this;
    }

    public CSharpField Constant(string value)
    {
        AccessModifier = "public ";
        IsConstant = true;
        Assignment = value;
        return this;
    }

    public CSharpField Constant(string value, bool isConstant)
    {
        IsConstant = isConstant;
        if (value != null)
        {
            Assignment = value;
        }
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

    public CSharpField WithInstantiation()
    {
        var propertyType = CSharpTypeParser.Parse(Type);

        if (File?.Template.OutputTarget.GetProject().GetLanguageVersion().Major >= 12 && propertyType is not null && propertyType.IsCollectionType())
        {
            Assignment = "[]";
            return this;
        }

        if (File?.Template.OutputTarget.GetProject().GetLanguageVersion().Major < 12 && propertyType is not null && propertyType.IsCollectionType())
        {
            var concreteImplementation = propertyType.GetCollectionImplementationType();
            Assignment = GetInstantiationValue(propertyType, concreteImplementation);

            return this;
        }

        return this;
    }

    public override string GetText(string indentation)
    {
        var sb = new StringBuilder();

        sb.Append(GetComments(indentation));
        sb.Append(GetAttributes(indentation));
        sb.Append(indentation);

        if (AccessModifier != null)
        {
            sb.Append(AccessModifier);
        }

        if (IsStatic)
        {
            sb.Append("static ");
        }

        if (IsReadOnly)
        {
            sb.Append("readonly ");
        }

        if (IsConstant)
        {
            sb.Append("const ");
        }

        if (IsRequired)
        {
            sb.Append("required ");
        }

        sb.Append(Type);

        if (_canBeNull)
        {
            sb.Append('?');
        }

        sb.Append(' ');

        sb.Append(Name);

        if (Assignment != null)
        {
            var assignment = Assignment.GetText(indentation).TrimStart();
            if (!string.IsNullOrWhiteSpace(assignment))
            {
                sb.Append(" = ");
                sb.Append(assignment);
            }
        }

        sb.Append(';');

        return sb.ToString();
    }

    private string GetInstantiationValue(CSharpType? propertyType, ICSharpType concreteImplementation)
    {
        if (propertyType is CSharpTypeGeneric generic)
        {
            var argTypes = string.Join(", ", generic.TypeArgumentList.Select(s => File.Template.UseType(s.ToString())));
            return $"new {File.Template.UseType($"{concreteImplementation}<{argTypes}>").Replace("?", "")}()";
        }

        return $"new {File.Template.UseType($"{concreteImplementation}").Replace("?", "")}()";
    }

    #region ICSharpField implementation

    IEnumerable<ICSharpAttribute> ICSharpDeclaration<ICSharpField>.Attributes => Attributes;

    ICSharpField ICSharpDeclaration<ICSharpField>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapping.AddAttribute(name, configure);

    ICSharpField ICSharpDeclaration<ICSharpField>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure = null) => _wrapping.AddAttribute(attribute, configure);

    ICSharpField ICSharpDeclaration<ICSharpField>.WithComments(string xmlComments) => _wrapping.WithComments(xmlComments);

    ICSharpField ICSharpDeclaration<ICSharpField>.WithComments(IEnumerable<string> xmlComments) => _wrapping.WithComments(xmlComments);

    ICSharpField ICSharpField.CanBeNull() => _wrapping.CanBeNull();

    ICSharpField ICSharpField.Constant(bool isConstant) => _wrapping.Constant(isConstant);

    ICSharpField ICSharpField.Constant(string value) => _wrapping.Constant(value);
    
    ICSharpField ICSharpField.Constant(string value, bool isConstant) => _wrapping.Constant(value, isConstant);

    ICSharpField ICSharpField.Internal(string? value) => _wrapping.Internal(value);

    ICSharpField ICSharpField.Private() => _wrapping.Private();

    ICSharpField ICSharpField.Private(string value) => _wrapping.Private(value);

    ICSharpField ICSharpField.PrivateConstant(string value) => _wrapping.PrivateConstant(value);

    ICSharpField ICSharpField.PrivateReadOnly() => _wrapping.PrivateReadOnly();

    ICSharpField ICSharpField.Protected() => _wrapping.Protected();

    ICSharpField ICSharpField.Protected(string value) => _wrapping.Protected(value);

    ICSharpField ICSharpField.ProtectedInternal(string? value) => _wrapping.ProtectedInternal(value);

    ICSharpField ICSharpField.ProtectedReadOnly() => _wrapping.ProtectedReadOnly();

    ICSharpField ICSharpField.Public(string? value) => _wrapping.Public(value);

    ICSharpField ICSharpField.ReadOnly(bool readOnly) => _wrapping.ReadOnly(readOnly);

    ICSharpField ICSharpField.ReadOnly(string value, bool readOnly) => _wrapping.ReadOnly(value, readOnly);

    ICSharpField ICSharpField.Required() => _wrapping.Required();

    ICSharpField ICSharpField.Static() => _wrapping.Static();

    ICSharpField ICSharpField.WithAssignment(ICSharpStatement value) => _wrapping.WithAssignment(value);

    ICSharpField ICSharpField.WithInstantiation() => _wrapping.WithInstantiation();

    #endregion
}