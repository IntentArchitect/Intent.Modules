using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.Types
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

        public int Priority
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
            public string DefaultCollectionFormat { get; set; } = "{0}";

            public void AddClassTypeSource(Func<ITypeReference, string> typeLookup)
            {
                throw new NotImplementedException();
            }

            public void AddClassTypeSource(IClassTypeSource classTypeSource)
            {
                throw new NotImplementedException();
            }

            public void AddClassTypeSource(IClassTypeSource classTypeSource, string contextName)
            {
                throw new NotImplementedException();
            }

            public string Get(ITypeReference typeInfo)
            {
                if (typeInfo == null)
                    return string.Empty;
                return typeInfo.Element.Name;
            }

            public string Get(ITypeReference typeInfo, string collectionFormat)
            {
                if (typeInfo == null)
                    return string.Empty;
                return string.Format(collectionFormat, typeInfo.Element.Name);
            }

            public string Get(IElement element)
            {
                if (element == null)
                    return string.Empty;
                return element.Name;
            }

            public string Get(IElement element, string collectionFormat)
            {
                if (element == null)
                    return string.Empty;
                return string.Format(collectionFormat, element.Name);
            }

            public ITypeResolverContext InContext(string contextName)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ITemplateDependency> GetTemplateDependencies()
            {
                return new List<ITemplateDependency>();
            }
        }
    }
}
