using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpCodeBlock : ICodeBlock
{
    IReadOnlyCollection<string> Lines { get; }
}