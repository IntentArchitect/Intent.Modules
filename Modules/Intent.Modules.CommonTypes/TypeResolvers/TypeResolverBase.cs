using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.CommonTypes.TypeResolvers
{
    public abstract class TypeResolverBase : ITypeResolver
    {
        private List<SoftwareFactory.Templates.IClassTypeSource> _classTypeSources;

        public TypeResolverBase()
        {
            _classTypeSources = new List<SoftwareFactory.Templates.IClassTypeSource>();
        }

        public void AddClassTypeSource(SoftwareFactory.Templates.IClassTypeSource classTypeSource)
        {
            _classTypeSources.Add(classTypeSource);
        }

        public string Get(ITypeReference typeInfo)
        {
            if (typeInfo.Type == ReferenceType.ClassType)
            {
                foreach (var classLookup in _classTypeSources)
                {
                    var foundClass = classLookup.GetClassType(typeInfo);
                    if (!string.IsNullOrWhiteSpace(foundClass))
                    {
                        return foundClass;
                    }
                }
            }
            return ResolveType(typeInfo);
        }

        protected abstract string ResolveType(ITypeReference typeInfo);
    }
}
