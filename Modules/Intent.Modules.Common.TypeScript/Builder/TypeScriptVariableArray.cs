using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptVariableArray : TypescriptVariableValue
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptVariableArray AddObject(Action<TypescriptVariableObject> configure = null)
    {
        var item = new TypescriptVariableObject
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = this.Indentation
        };

        Items.Add(item);
        configure?.Invoke(item);
        return this;
    }

    public TypescriptVariableArray AddValue(string value, Action<TypescriptVariableField> configure = null)
    {
        var item = new TypescriptVariableField(value)
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator
        };

        Items.Add(item);
        configure?.Invoke(item);
        return this;
    }

    public List<TypescriptVariableValue> Items { get; } = new();

    /// InheritedDoc
    public override string GetText(string indentation)
    {
        var codeBlocks = new List<ICodeBlock>();
        codeBlocks.AddRange(Items);

        var incomingIndentation = indentation;
        indentation += this.Indentation;

        var codeBlockText = codeBlocks.ConcatCode($",", indentation);

        return $@"[{codeBlockText}
{incomingIndentation}]";
    }

    private string GetSeperator() => _fieldsSeparator switch
    {
        TypescriptCodeSeparatorType.None => "",
        TypescriptCodeSeparatorType.NewLine => Environment.NewLine,
        TypescriptCodeSeparatorType.EmptyLines => $"{Environment.NewLine}{Environment.NewLine}",
        _ => Environment.NewLine
    };
}
