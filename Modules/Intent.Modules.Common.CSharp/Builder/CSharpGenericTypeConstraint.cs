using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpGenericTypeConstraint : ICSharpGenericTypeConstraint
{
    public CSharpGenericTypeConstraint(string genericTypeParameter, params string[] types)
    {
        GenericTypeParameter = genericTypeParameter;
        if (types?.Any() == true)
        {
            Types = types.ToList();
        }
    }

    public string GenericTypeParameter { get; }
    public IList<string> Types { get; } = new List<string>();

    ICSharpGenericTypeConstraint ICSharpGenericTypeConstraint.AddType(string typeName) => AddType(typeName);
    public CSharpGenericTypeConstraint AddType(string typeName)
    {
        Types.Add(typeName);
        return this;
    }

    public override string ToString()
    {
        return $"where {GenericTypeParameter} : {string.Join(", ", Types)}";
    }
}