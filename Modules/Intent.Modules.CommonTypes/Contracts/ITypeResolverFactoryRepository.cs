using System;
using System.Collections.Generic;

namespace Intent.Modules.CommonTypes.Contracts
{
    public interface ITypeResolverFactoryRepository
    {
        IEnumerable<ITypeResolverFactory> TypeResolvers { get; }

        ITypeResolverFactory GetTypeResolver(SoftwareFactory.Templates.IFileMetaData meta);
    }
}