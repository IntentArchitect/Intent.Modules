using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modules.Common
{
    public interface IDeclareUsings
    {
        IEnumerable<string> DeclareUsings();
    }
}