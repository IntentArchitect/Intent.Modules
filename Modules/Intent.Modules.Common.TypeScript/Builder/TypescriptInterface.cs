using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptInterface : TypescriptDeclaration<TypescriptInterface>
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;
    private TypescriptCodeSeparatorType _methodsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptInterface(string name, TypescriptFile file)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        File = file;
        Name = name;
    }

    public TypescriptFile File { get; }
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
        return GetText("");
    }

    public string GetText(string indentation)
    {
        var sb = new StringBuilder();

        sb.Append(indentation);
        sb.Append(GetComments(indentation));
        sb.Append(GetDecorators(indentation));
        if (IsExported)
        {
            sb.Append("export ");
        }

        sb.Append("interface ");
        sb.Append(Name);
        sb.Append(' ');

        if (Interfaces.Count > 0)
        {
            sb.Append("extends ");

            foreach (var @interface in Interfaces)
            {
                sb.Append(@interface);
                sb.Append(", ");
            }

            // Remove trailing comma:
            sb.Length -= ", ".Length;

            sb.Append(' ');
        }

        sb.Append('{');

        var concatenatedCodeBlocks = Enumerable.Empty<ICodeBlock>()
            .Concat(Fields)
            .Concat(Methods)
            .ToArray()
            .ConcatCode($"{indentation}{File.Indentation}");

        sb.Append(concatenatedCodeBlocks);
        sb.AppendLine();

        sb.AppendLine("}");

        return sb.ToString();
    }
}