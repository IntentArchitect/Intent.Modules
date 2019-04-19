using System;
using System.Collections.Generic;
using Intent.MetaModel.Common;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
    public class TypeResolverContext : ITypeResolverContext
    {
        private readonly List<IClassTypeSource> _classTypeSources;
        private readonly Func<ITypeReference, string, string> _resolveTypeFunc;

        public TypeResolverContext(List<IClassTypeSource> classTypeSources, Func<ITypeReference, string, string> resolveTypeFunc)
        {
            _classTypeSources = classTypeSources;
            _resolveTypeFunc = resolveTypeFunc;
        }

        public string Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public string Get(ITypeReference typeInfo, string collectionType)
        {
            if (typeInfo == null)
            {
                return null;
            }

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
            return _resolveTypeFunc(typeInfo, collectionType);
        }
    }


    public abstract class TypeResolverBase : ITypeResolver
    {
        private const string DEFAULT_CONTEXT = "_default_";
        private readonly IDictionary<string, List<IClassTypeSource>> _classTypeSources;

        protected TypeResolverBase()
        {
            _classTypeSources = new Dictionary<string, List<IClassTypeSource>>()
            {
                { DEFAULT_CONTEXT, new List<IClassTypeSource>() }
            };
        }

        public void AddClassTypeSource(IClassTypeSource classTypeSource)
        {
            AddClassTypeSource(classTypeSource, DEFAULT_CONTEXT);
        }

        public void AddClassTypeSource(IClassTypeSource classTypeSource, string contextName)
        {
            if (contextName == null)
                contextName = DEFAULT_CONTEXT;

            if (!_classTypeSources.ContainsKey(contextName))
            {
                _classTypeSources.Add(contextName, new List<IClassTypeSource>());
            }
            _classTypeSources[contextName].Add(classTypeSource);
        }

        public ITypeResolverContext InContext(string contextName)
        {
            if (!_classTypeSources.ContainsKey(contextName))
            {
                throw new InvalidOperationException($"contextName '{contextName}' does not exist.");
            }

            return new TypeResolverContext(_classTypeSources[contextName], ResolveType);
        }

        public string Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public string Get(ITypeReference typeInfo, string collectionType)
        {
            return InContext(DEFAULT_CONTEXT).Get(typeInfo, collectionType);
        }
        
        protected abstract string ResolveType(ITypeReference typeInfo, string collectionType = null);
    }
}
