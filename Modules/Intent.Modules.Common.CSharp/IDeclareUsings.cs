using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp
{
    public interface IDeclareUsings
    {
        IEnumerable<string> DeclareUsings();
    }
}