using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.Contracts;

namespace Intent.Modules.CommonTypes.Contracts
{
    public interface ITypeResolverFactoryResolution
    {
        ITypeResolverFactory DetermineTypeResolver(ITypeResolverFactoryRepository factory);
    }
}
