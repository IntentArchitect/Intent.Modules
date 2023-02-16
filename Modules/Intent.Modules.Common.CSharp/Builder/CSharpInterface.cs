using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterface : CSharpDeclaration<CSharpInterface>
{
    private CSharpCodeSeparatorType _fieldsSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _propertiesSeparator = CSharpCodeSeparatorType.NewLine;
    private CSharpCodeSeparatorType _methodsSeparator = CSharpCodeSeparatorType.NewLine;

    public CSharpInterface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.ToCSharpIdentifier();
    }

    public string Name { get; }
    protected string AccessModifier { get; private set; } = "public ";
    public IList<string> Interfaces { get; set; } = new List<string>();
    public IList<CSharpInterfaceField> Fields { get; } = new List<CSharpInterfaceField>();
    public IList<CSharpInterfaceProperty> Properties { get; } = new List<CSharpInterfaceProperty>();
    public IList<CSharpInterfaceMethod> Methods { get; } = new List<CSharpInterfaceMethod>();
    public IList<CSharpInterfaceGenericParameter> GenericParameters { get; } = new List<CSharpInterfaceGenericParameter>();
    public IList<CSharpGenericTypeConstraint> GenericTypeConstraints { get; } = new List<CSharpGenericTypeConstraint>();
    public IList<CSharpCodeBlock> CodeBlocks { get; } = new List<CSharpCodeBlock>();
    
    public CSharpInterface ExtendsInterface(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        Interfaces.Add(type);
        return this;
    }

    public CSharpInterface ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public CSharpInterface AddField(string type, string name, Action<CSharpInterfaceField> configure = null)
    {
        var field = new CSharpInterfaceField(type, name)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public CSharpInterface AddProperty(string type, string name, Action<CSharpInterfaceProperty> configure = null)
    {
        var property = new CSharpInterfaceProperty(type, name)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public CSharpInterface InsertProperty(int index, string type, string name, Action<CSharpInterfaceProperty> configure = null)
    {
        var property = new CSharpInterfaceProperty(type, name)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Insert(index, property);
        configure?.Invoke(property);
        return this;
    }

    public CSharpInterface AddMethod(string returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }
    
    public CSharpInterface AddCodeBlock(string codeLine)
    {
        CodeBlocks.Add(new CSharpCodeBlock(codeLine));
        return this;
    }

    public CSharpInterface AddGenericParameter(string typeName, Action<CSharpInterfaceGenericParameter> configure = null)
    {
        var param = new CSharpInterfaceGenericParameter(typeName);
        configure?.Invoke(param);
        GenericParameters.Add(param);
        return this;
    }
    
    public CSharpInterface AddGenericParameter(string typeName, out CSharpInterfaceGenericParameter param, Action<CSharpInterfaceGenericParameter> configure = null)
    {
        param = new CSharpInterfaceGenericParameter(typeName);
        configure?.Invoke(param);
        GenericParameters.Add(param);
        return this;
    }
    
    public CSharpInterface AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
    {
        var param = new CSharpGenericTypeConstraint(genericParameterName);
        configure(param);
        GenericTypeConstraints.Add(param);
        return this;
    }

    public CSharpInterface InsertMethod(int index, string returnType, string name, Action<CSharpInterfaceMethod> configure = null)
    {
        var method = new CSharpInterfaceMethod(returnType, name)
        {
            BeforeSeparator = _methodsSeparator,
            AfterSeparator = _methodsSeparator
        };
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public CSharpInterface WithFieldsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public CSharpInterface WithPropertiesSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        return this;
    }

    public CSharpInterface WithMethodsSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _methodsSeparator = separator;
        return this;
    }

    public CSharpInterface WithMembersSeparated(CSharpCodeSeparatorType separator = CSharpCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        _methodsSeparator = separator;
        return this;
    }

    public CSharpInterface Internal()
    {
        AccessModifier = "internal ";
        return this;
    }
    public CSharpInterface InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public CSharpInterface Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpInterface Private()
    {
        AccessModifier = "private ";
        return this;
    }
    public CSharpInterface Partial()
    {
        IsPartial = true;
        return this;
    }

    public bool IsPartial { get; set; }

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        return $@"{GetAttributes(indentation)}{indentation}{AccessModifier}{(IsPartial ? "partial " : "")}interface {Name}{GetGenericParameters()}{GetBaseTypes()}{GetGenericTypeConstraints(indentation)}
{indentation}{{{GetMembers($"{indentation}    ")}
{indentation}}}";
    }
    
    private string GetGenericTypeConstraints(string indentation)
    {
        if (!GenericTypeConstraints.Any())
        {
            return string.Empty;
        }

        string newLine = $@"
{indentation}    ";
        return newLine + string.Join(newLine, GenericTypeConstraints);
    }

    private string GetGenericParameters()
    {
        if (!GenericParameters.Any())
        {
            return string.Empty;
        }

        return $"<{string.Join(", ", GenericParameters.Select(s => s.GetText()))}>";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        foreach (var @interface in Interfaces)
        {
            types.Add(@interface);
        }

        return types.Any() ? $" : {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);
        codeBlocks.AddRange(Properties);
        codeBlocks.AddRange(Methods);
        codeBlocks.AddRange(CodeBlocks);

        return !codeBlocks.Any() ? "" : $@"
{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }
}