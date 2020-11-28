using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public abstract class TypeResolverBase : ITypeResolver
    {
        private const string DEFAULT_CONTEXT = "_default_";
        private readonly IDictionary<string, List<ITypeSource>> _classTypeSources;
        private static IList<ITypeSource> _globalTypeSources = new List<ITypeSource>();

        protected TypeResolverBase()
        {
            _classTypeSources = new Dictionary<string, List<ITypeSource>>()
            {
                { DEFAULT_CONTEXT, new List<ITypeSource>() }
            };
        }

        public abstract string DefaultCollectionFormat { get; set; }

        public void AddClassTypeSource(ITypeSource typeSource)
        {
            AddClassTypeSource(typeSource, DEFAULT_CONTEXT);
        }

        public void AddClassTypeSource(ITypeSource typeSource, string contextName)
        {
            if (contextName == null)
                contextName = DEFAULT_CONTEXT;

            if (!_classTypeSources.ContainsKey(contextName))
            {
                _classTypeSources.Add(contextName, new List<ITypeSource>());
            }
            _classTypeSources[contextName].Add(typeSource);
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
            return _classTypeSources.Values
                .SelectMany(x => x)
                .SelectMany(x => x.GetTemplateDependencies()).ToList();
        }

        public IResolvedTypeInfo Get(ITypeReference typeInfo)
        {
            return Get(typeInfo, null);
        }

        public IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
        {
            return InContext(DEFAULT_CONTEXT).Get(typeInfo, collectionFormat);
        }

        public IResolvedTypeInfo Get(ICanBeReferencedType element)
        {
            return Get(element, null);
        }

        public IResolvedTypeInfo Get(ICanBeReferencedType element, string collectionFormat)
        {
            return InContext(DEFAULT_CONTEXT).Get(new ElementTypeReference(element), collectionFormat);
        }

        protected abstract IResolvedTypeInfo ResolveType(ITypeReference typeInfo, string collectionFormat = null);

        private class ElementTypeReference: ITypeReference, IHasStereotypes
        {
            public ElementTypeReference(ICanBeReferencedType element)
            {
                Element = element;
            }

            public bool IsNullable { get; } = false;
            public bool IsCollection { get; } = false;
            public ICanBeReferencedType Element { get; }
            public IEnumerable<ITypeReference> GenericTypeParameters { get; } = new ITypeReference[0];
            public string Comment { get; } = null;
            public IEnumerable<IStereotype> Stereotypes { get; } = new List<IStereotype>();
        }
    }
}
