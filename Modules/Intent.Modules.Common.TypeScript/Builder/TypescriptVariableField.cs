using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptVariableField : TypescriptVariableValue
{
    public TypescriptVariableField(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(value));
        }

        Name = null;
        Value = value;
    }

    public TypescriptVariableField(string name, string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(value));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Name = name;
        Value = value;
    }

    public string Value { get; }
    public string Name { get; }

    public override string GetText(string indentation)
    {
        if(Name is null)
        {
            return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{Value}";
        }

        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{Name}: {Value}";
    }
}
