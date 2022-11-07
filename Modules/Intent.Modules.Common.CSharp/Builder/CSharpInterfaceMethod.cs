using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceMethod : CSharpMember<CSharpInterfaceMethod>
{
    public string ReturnType { get; private set; }
    public string Name { get; private set; }
    public IList<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();
    public CSharpInterfaceMethod(string returnType, string name)
    {
        ReturnType = returnType;
        Name = name;
        Separator = CSharpCodeSeparatorType.None;
    }

    public CSharpInterfaceMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
    {
        var param = new CSharpParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{ReturnType} {Name}({string.Join(", ", Parameters.Select(x => x.ToString()))});";
    }
}