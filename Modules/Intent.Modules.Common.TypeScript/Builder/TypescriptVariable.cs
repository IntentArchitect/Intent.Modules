using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptVariable: TypescriptMember<TypescriptVariable>
{
    private TypescriptCodeSeparatorType _fieldsSeparator = TypescriptCodeSeparatorType.NewLine;

    public TypescriptVariable(string name, TypescriptFile file)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }
        
        Name = name;
        File = file;

        Value = new TypescriptVariableObject
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = File.Indentation
        };
    }

    public TypescriptVariable(string name, string type, TypescriptFile file)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
        File = file;
        Type = type;

        Value = new TypescriptVariableObject
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = File.Indentation
        };
    }

    public TypescriptFile File { get; }

    public string Name { get; }

    public string Type { get; }

    public bool IsExported { get; private set; }

    public bool IsConst { get; private set; }
    
    public TypescriptVariableValue Value { get; private set; }

    public TypescriptVariable Export()
    {
        IsExported = true;
        return this;
    }

    public TypescriptVariable Const()
    {
        IsConst = true;
        return this;
    }

    public TypescriptVariable WithObjectValue(Action<TypescriptVariableObject> configure = null)
    {
        Value = new TypescriptVariableObject
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = File.Indentation
        };
        configure?.Invoke((TypescriptVariableObject)Value);
        return this;
    }

    public TypescriptVariable WithArrayValue(Action<TypescriptVariableArray> configure = null)
    {
        Value = new TypescriptVariableArray
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = File.Indentation
        };
        configure?.Invoke((TypescriptVariableArray)Value);
        return this;
    }

    public TypescriptVariable WithExpressionFunctionValue(Action<TypeScriptVariableExpressionFunction> configure = null)
    {
        Value = new TypeScriptVariableExpressionFunction
        {
            BeforeSeparator = _fieldsSeparator,
            AfterSeparator = _fieldsSeparator,
            Indentation = File.Indentation
        };
        configure?.Invoke((TypeScriptVariableExpressionFunction)Value);
        return this;
    }

    public override string ToString()
    {
        return GetText("");
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{(IsExported ? "export " : string.Empty)}{(IsConst ? "const " : "")}{Name}{(!string.IsNullOrEmpty(Type) ? $": {Type}" : "")} = {GetValue("")};";
    }

    private string GetValue(string indentation)
    {
        if (Value is not null)
        {
            return Value.GetText(indentation);
        }

        return "";
    }
}
