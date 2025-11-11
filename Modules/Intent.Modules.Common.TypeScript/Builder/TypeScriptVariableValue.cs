using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public abstract class TypescriptVariableValue : TypescriptMember<TypescriptVariableValue>
{
    public string Name { get; }

    public TypescriptVariableValue Value { get; }

    public override string GetText(string indentation)
    {
        return string.Empty;
    }
}
