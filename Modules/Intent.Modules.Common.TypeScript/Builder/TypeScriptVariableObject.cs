using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptVariableObject : TypescriptVariableValue
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptVariableObject AddField(string name, string value, Action<TypescriptVariableField> configure = null)
    {
        var property = new TypescriptVariableField(name, value)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };

        Fields.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public TypescriptVariableObject AddField<T>(string name, T value, Action<TypescriptVariableField> configure = null)
        where T : TypescriptVariableValue, new()
    {
        var property = new TypescriptVariableField(name, value)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };

        Fields.Add(property);
        configure?.Invoke(property);
        return this;
    }

    public TypescriptVariableObject WithFieldsSeparated(TypescriptCodeSeparatorType separator = TypescriptCodeSeparatorType.EmptyLines)
    {
        _fieldsSeparator = separator;
        return this;
    }

    public List<TypescriptVariableValue> Fields { get; } = new();

    public override string GetText(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Fields);

        return $@"{{ {string.Join($@"{GetSeperator()}", codeBlocks.ConcatCode(",", string.Concat(indentation)))}{GetSeperator()}{(_fieldsSeparator == TypescriptCodeSeparatorType.None ? " " : "")}}}";
    }

    private string GetSeperator() => _fieldsSeparator switch
    {
        TypescriptCodeSeparatorType.None => "",
        TypescriptCodeSeparatorType.NewLine => Environment.NewLine,
        TypescriptCodeSeparatorType.EmptyLines => $"{Environment.NewLine}{Environment.NewLine}",
        _ => Environment.NewLine
    };
}
