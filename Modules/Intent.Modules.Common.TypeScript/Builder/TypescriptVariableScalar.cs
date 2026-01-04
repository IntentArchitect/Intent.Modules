using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptVariableScalar : TypescriptVariableValue
{
    public TypescriptVariableScalar(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return GetText(string.Empty);
    }

    public override string GetText(string indentation)
    {
        return Value;
    }

}
