using System.Collections.Generic;
using Intent.Modules.Common.Types.Contracts;
using Intent.Templates;

namespace Intent.Modules.Common.Types.Contracts
{
    public interface ITypeResolverFactoryRepository
    {
        IEnumerable<ITypeResolverFactory> TypeResolvers { get; }

        ITypeResolverFactory GetTypeResolver(IFileMetadata metadata);
    }
}