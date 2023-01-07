using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructorCall
{
    private readonly string _type;
    public IList<string> Arguments { get; } = new List<string>();

    private CSharpConstructorCall(string type)
    {
        _type = type;
    }

    public static CSharpConstructorCall Base()
    {
        return new CSharpConstructorCall("base");
    }

    public static CSharpConstructorCall This()
    {
        return new CSharpConstructorCall("this");
    }

    public CSharpConstructorCall AddArgument(string name)
    {
        Arguments.Add(name);
        return this;
    }

    public override string ToString()
    {
        if (Arguments.Any())
        {
            return $" : {_type}({string.Join(", ", Arguments)})";
        }

        return string.Empty;
    }
}