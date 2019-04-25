using System.Collections.Generic;
using Intent.Modules.CommonTypes.Contracts;
using Intent.Templates;

namespace Intent.Modules.Common.Types.Contracts
{
    public interface ITypeResolverFactoryRepository
    {
        IEnumerable<ITypeResolverFactory> TypeResolvers { get; }

        ITypeResolverFactory GetTypeResolver(IFileMetadata meta);
    }
}