using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptInterface : TypescriptDeclaration<TypescriptInterface>
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;
    private TypescriptCodeSeparatorType _methodsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptInterface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
    public bool IsExported { get; private set; }
    public List<string> Interfaces { get; set; } = new();
    public List<TypescriptInterfaceField> Fields { get; } = new();
    public List<TypescriptInterfaceMethod> Methods { get; } = new();

    public TypescriptInterface Export()
    {
        IsExported = true;
        return this;
    }

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
        Interfaces.AddRange(types);

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

    public TypescriptInterface WithMethodsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _methodsSeparator = separator;
        return this;
    }

    public TypescriptInterface WithMembersSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        _methodsSeparator = separator;
        return this;
    }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{(IsExported ? "export " : string.Empty)}interface {Name}{GetBaseTypes()} {{
{indentation}{GetMembers($"{indentation}    ")}
{indentation}}}";
    }

    private string GetBaseTypes()
    {
        var types = new List<string>();
        foreach (var @interface in Interfaces)
        {
            types.Add(@interface);
        }

        return types.Any() ? $" implements {string.Join(", ", types)}" : "";
    }

    private string GetMembers(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);
        codeBlocks.AddRange(Methods);

        return !codeBlocks.Any() ? "" : $@"
{string.Join(@"
", codeBlocks.ConcatCode(indentation))}";
    }
}