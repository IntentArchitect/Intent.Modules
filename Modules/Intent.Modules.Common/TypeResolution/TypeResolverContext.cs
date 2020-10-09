using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    public class TypeResolverContext : ITypeResolverContext
    {
        private readonly List<ITypeSource> _classTypeSources;
        private readonly Func<ITypeReference, string, IResolvedTypeInfo> _resolveTypeFunc;

        public TypeResolverContext(List<ITypeSource> classTypeSources, Func<ITypeReference, string, IResolvedTypeInfo> resolveTypeFunc)
        {
            _classTypeSources = classTypeSources;
            _resolveTypeFunc = resolveTypeFunc;
        }

        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            if (typeInfo == null)
            {
                return null;
            }

            foreach (var classLookup in _classTypeSources)
            {
                var foundClass = classLookup.GetType(typeInfo);
                if (foundClass != null)
                {
                    return foundClass;
                }
            }
            return _resolveTypeFunc(typeInfo, collectionFormat);
        }
    }
}