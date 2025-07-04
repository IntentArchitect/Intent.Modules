using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructorCall : ICSharpConstructorCall
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

    ICSharpConstructorCall ICSharpConstructorCall.AddArgument(string name) => AddArgument(name);
    public CSharpConstructorCall AddArgument(string name)
    {
        Arguments.Add(name);
        return this;
    }

    public override string ToString()
    {
        if (Arguments.Any())
        {
            var arguments = Arguments
                .Select(x => x.EnsureVerbatimIdentifierForArgument());

            return $" : {_type}({string.Join(", ", arguments)})";
        }

        if (_type == "this")
        {
            return " : this()";
        }

        return string.Empty;
    }
}