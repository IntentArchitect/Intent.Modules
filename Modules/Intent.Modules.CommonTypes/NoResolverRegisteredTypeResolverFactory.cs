using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.CommonTypes
{
    internal class NoResolverRegisteredTypeResolverFactory : Contracts.ITypeResolverFactory
    {
        public string Name
        {
            get
            {
                return "No Type Resolver Registered";
            }
        }

        public int Priotiry
        {
            get
            {
                return 0;
            }
        }

        public IEnumerable<string> SupportedFileTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITypeResolver Create()
        {
            return new NoResolverRegisteredTypeResolver();
        }

        internal class NoResolverRegisteredTypeResolver : ITypeResolver
        {
            public void AddClassTypeSource(Func<ITypeReference, string> typeLookup)
            {
                throw new NotImplementedException();
            }

            public void AddClassTypeSource(SoftwareFactory.Templates.IClassTypeSource classTypeSource)
            {
                throw new NotImplementedException();
            }

            public string Get(ITypeReference typeInfo)
            {
                if (typeInfo == null)
                    return string.Empty;
                return typeInfo.Name;
            }
        }
    }
}
