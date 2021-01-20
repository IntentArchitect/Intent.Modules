using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Types.Contracts;
using Intent.Templates;

namespace Intent.Modules.Common.Types
{
    public class TypeResolverFactoryRepository : ITypeResolverFactoryRepository
    {
        private IEnumerable<Contracts.ITypeResolverFactory> _typeResolvers;
        private NoResolverRegisteredTypeResolverFactory _noTypeResolverResolver = new NoResolverRegisteredTypeResolverFactory();

        public TypeResolverFactoryRepository(Contracts.ITypeResolverFactory[] typeResolvers)
        {
            _typeResolvers = typeResolvers;
        }

        public IEnumerable<Contracts.ITypeResolverFactory> TypeResolvers
        {
            get
            {
                return _typeResolvers;
            }
        }

        public Contracts.ITypeResolverFactory GetTypeResolver(IFileMetadata metadata)
        {
            var resolver = _typeResolvers.OrderByDescending(tr => tr.Priority).FirstOrDefault(tr => tr.SupportedFileTypes.Any( ft => ft.ToLower().EndsWith( metadata.FileExtension.ToLower())) );

            return resolver;
        }
    }
}
