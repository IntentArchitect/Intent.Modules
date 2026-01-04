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
        Value = new TypescriptVariableScalar(value);    
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
        Value = new TypescriptVariableScalar(value);
    }

    public TypescriptVariableField(string name, TypescriptVariableValue value)
    {
        if (value is null)
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

    public override string GetText(string indentation)
    {
        if(Name is null)
        {
            return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{Value.GetText(indentation)}";
        }

        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{Name}: {Value.GetText(indentation)}";
    }
}
