using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public abstract class TypescriptVariableValue : TypescriptMember<TypescriptVariableValue>
{
    public string Name { get; internal set; }

    public string Indentation { get; set; }

    public TypescriptVariableValue Value { get; internal set; }

    public TypescriptVariableValue WithName(string name)
    {
        Name = name;
        return this;
    }

    public TypescriptVariableValue WithValue(TypescriptVariableValue value)
    {
        Value = value;
        return this;
    }

    public override string GetText(string indentation)
    {
        return string.Empty;
    }
}
