using System;
using System.Collections.Generic;
using Intent.MetaModel.Common;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.TypeResolvers
{
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

        public string Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, DEFAULT_CONTEXT);
        }

        public string Get(ITypeReference typeInfo, string contextName)
        {
            if (typeInfo == null)
            {
                return null;
            }

            if (contextName == null)
                contextName = DEFAULT_CONTEXT;

            if (typeInfo.Type == ReferenceType.ClassType)
            {
                if (!_classTypeSources.ContainsKey(contextName))
                {
                    throw new InvalidOperationException($"contextName '{contextName}' does not exist.");
                }

                foreach (var classLookup in _classTypeSources[contextName])
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
