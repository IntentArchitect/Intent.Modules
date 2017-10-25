using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Bower.Contracts
{
    public interface IHasBowerDependencies
    {
        IEnumerable<IBowerPackageInfo> GetBowerDependencies();
    }
}
