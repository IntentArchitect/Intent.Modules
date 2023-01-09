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

    public TypescriptInterface AddField(string name, string type, Action<TypescriptInterfaceField> configure = null)
    {
        var field = new TypescriptInterfaceField(name, type)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };
        Fields.Add(field);
        configure?.Invoke(field);
        return this;
    }

    public TypescriptInterface AddMethod(string name, string returnType, Action<TypescriptInterfaceMethod> configure = null)
    {
        return InsertMethod(Methods.Count, name, returnType, configure);
    }

    public TypescriptInterface InsertMethod(int index, string name, string returnType, Action<TypescriptInterfaceMethod> configure = null)
    {
        var method = new TypescriptInterfaceMethod(name, returnType)
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
        return $@"{indentation}{(IsExported ? "export " : string.Empty)}interface {Name}{GetBaseTypes()} {{{GetMembers($"{indentation}    ")}
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