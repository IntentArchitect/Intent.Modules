using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaConstructorCall
{
    private readonly string _type;
    public IList<string> Arguments { get; } = new List<string>();

    private JavaConstructorCall(string type)
    {
        _type = type;
    }

    public static JavaConstructorCall Super()
    {
        return new JavaConstructorCall("super");
    }

    public static JavaConstructorCall This()
    {
        return new JavaConstructorCall("this");
    }

    public JavaConstructorCall AddArgument(string name)
    {
        Arguments.Add(name);
        return this;
    }

    public override string ToString()
    {
        if (Arguments.Any())
        {
            return $"{_type}({string.Join(", ", Arguments)});";
        }

        return string.Empty;
    }
}