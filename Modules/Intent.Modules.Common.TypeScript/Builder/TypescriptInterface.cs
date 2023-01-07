using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptInterface : TypescriptDeclaration<TypescriptInterface>
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;
    private TypescriptCodeSeparatorType _propertiesSeparator = TypescriptCodeSeparatorType.NewLine;
    private TypescriptCodeSeparatorType _methodsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptInterface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name.ToTypescriptIdentifier();
    }

    public string Name { get; }
    protected string AccessModifier { get; private set; } = "public ";
    public IList<string> Interfaces { get; set; } = new List<string>();
    public IList<TypescriptInterfaceField> Fields { get; } = new List<TypescriptInterfaceField>();
    public IList<TypescriptInterfaceProperty> Properties { get; } = new List<TypescriptInterfaceProperty>();
    public IList<TypescriptInterfaceMethod> Methods { get; } = new List<TypescriptInterfaceMethod>();

    public TypescriptInterface ExtendsInterface(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        Interfaces.Add(type);
        return this;
    }

    public TypescriptInterface ImplementsInterfaces(IEnumerable<string> types)
    {
        foreach (var type in types)
            Interfaces.Add(type);

        return this;
    }

    public TypescriptInterface AddField(string type, string name, Action<TypescriptInterfaceField> configure = null)
    {
        var field = new TypescriptInterfaceField(type, name)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public TypescriptInterface AddProperty(string type, string name, Action<TypescriptInterfaceProperty> configure = null)
    {
        var property = new TypescriptInterfaceProperty(type, name)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public TypescriptInterface InsertProperty(int index, string type, string name, Action<TypescriptInterfaceProperty> configure = null)
    {
        var property = new TypescriptInterfaceProperty(type, name)
        {
            BeforeSeparator = _propertiesSeparator,
            AfterSeparator = _propertiesSeparator
        };
        Properties.Insert(index, property);
        configure?.Invoke(property);
        return this;
    }

    public TypescriptInterface AddMethod(string returnType, string name, Action<TypescriptInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, returnType, name, configure);
    }

    public TypescriptInterface InsertMethod(int index, string returnType, string name, Action<TypescriptInterfaceMethod> configure = null)
    {
        var method = new TypescriptInterfaceMethod(returnType, name)
        {
            BeforeSeparator = _methodsSeparator,
            AfterSeparator = _methodsSeparator
        };
        Methods.Insert(index, method);
        configure?.Invoke(method);
        return this;
    }

    public TypescriptInterface WithFieldsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public TypescriptInterface WithPropertiesSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        return this;
    }

    public TypescriptInterface WithMethodsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _methodsSeparator = separator;
        return this;
    }

    public TypescriptInterface WithMembersSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _propertiesSeparator = separator;
        _methodsSeparator = separator;
        return this;
    }

    public TypescriptInterface Internal()
    {
        AccessModifier = "internal ";
        return this;
    }
    public TypescriptInterface InternalProtected()
    {
        AccessModifier = "internal protected ";
        return this;
    }

    public TypescriptInterface Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public TypescriptInterface Private()
    {
        AccessModifier = "private ";
        return this;
    }
    public TypescriptInterface Partial()
    {
        IsPartial = true;
        return this;
    }

    public bool IsPartial { get; set; }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        return $@"{GetDecorators(indentation)}{indentation}{AccessModifier}{(IsPartial ? "partial " : "")}interface {Name}{GetBaseTypes()}
{indentation}{{{GetMembers($"{indentation}    ")}
{indentation}}}";
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

        return !codeBlocks.Any() ? "" : $@"
{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }
}