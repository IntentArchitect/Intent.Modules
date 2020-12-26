using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Intent.Engine;

namespace Intent.Modules.Common
{
    public interface IDeclareUsings
    {
        IEnumerable<string> DeclareUsings();
    }
}