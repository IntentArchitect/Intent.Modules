using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
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

        public string Get(ITypeReference typeInfo, string collectionFormat)
        {
            if (typeInfo == null)
            {
                return null;
            }

            foreach (var classLookup in _classTypeSources)
            {
                var foundClass = classLookup.GetClassType(typeInfo);
                if (!string.IsNullOrWhiteSpace(foundClass))
                {
                    return foundClass;
                }
            }
            return _resolveTypeFunc(typeInfo, collectionFormat);
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

        public abstract string DefaultCollectionFormat { get; set; }

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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _classTypeSources.Values.SelectMany(x => x).SelectMany(x => x.GetTemplateDependencies()).ToList();
        }

        public string Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public string Get(ITypeReference typeInfo, string collectionFormat)
        {
            return InContext(DEFAULT_CONTEXT).Get(typeInfo, collectionFormat);
        }

        public string Get(IElement element)
        {
            return Get(element, null);
        }

        public string Get(IElement element, string collectionFormat)
        {
            return InContext(DEFAULT_CONTEXT).Get(new ElementTypeReference(element), collectionFormat);
        }

        protected abstract string ResolveType(ITypeReference typeInfo, string collectionFormat = null);

        private class ElementTypeReference: ITypeReference, IHasStereotypes
        {
            public ElementTypeReference(IElement element)
            {
                Element = element;
            }

            public bool IsNullable { get; } = false;
            public bool IsCollection { get; } = false;
            public IElement Element { get; }
            public IEnumerable<ITypeReference> GenericTypeParameters { get; } = new ITypeReference[0];
            public string Comment { get; } = null;
            public IEnumerable<IStereotype> Stereotypes { get; } = new List<IStereotype>();
        }
    }
}
