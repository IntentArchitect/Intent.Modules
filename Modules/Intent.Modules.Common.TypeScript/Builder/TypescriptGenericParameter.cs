using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptGenericParameter
{
    public TypescriptGenericParameter(string typeName)
    {
        TypeName = typeName;
    }

    public string TypeName { get; }

    public override string ToString()
    {
        return TypeName;
    }

    public static implicit operator string(TypescriptGenericParameter param)
    {
        return param.ToString();
    }
}
