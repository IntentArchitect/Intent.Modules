using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpConstructorCall
{
    ICSharpConstructorCall AddArgument(string name);
    IList<string> Arguments { get; }
}