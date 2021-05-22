using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public class CollectionFormatter : ICollectionFormatter
    {
        private readonly Func<IResolvedTypeInfo, string> _formatCollection;

        public CollectionFormatter(string collectionFormat)
        {
            _formatCollection = (type) => string.Format(collectionFormat, type.Name);
        }

        public CollectionFormatter(Func<string, string> formatCollection)
        {
            _formatCollection = (type) => formatCollection(type.Name);
        }

        public CollectionFormatter(Func<IResolvedTypeInfo, string> formatCollection)
        {
            _formatCollection = formatCollection;
        }

        public string AsCollection(IResolvedTypeInfo typeInfo)
        {
            return _formatCollection(typeInfo);
        }
    }

    public abstract class TypeResolverBase : ITypeResolver
    {
        private const string DEFAULT_CONTEXT = "_default_";
        //private readonly IDictionary<string, List<ITypeSource>> _classTypeSources;
        private readonly IDictionary<string, ITypeResolverContext> _contexts;

        protected TypeResolverBase(ITypeResolverContext defaultContext)
        {
            _contexts = new Dictionary<string, ITypeResolverContext>()
            {
                { DEFAULT_CONTEXT, defaultContext }
            };
        }

        protected abstract ITypeResolverContext CreateContext();

        public virtual string DefaultCollectionFormat
        {
            set => SetDefaultCollectionFormatter(new CollectionFormatter(value));
        }

        public void SetDefaultCollectionFormatter(ICollectionFormatter formatter)
        {
            _contexts[DEFAULT_CONTEXT].SetCollectionFormatter(formatter);
        }

        public void AddTypeSource(ITypeSource typeSource)
        {
            AddTypeSource(typeSource, DEFAULT_CONTEXT);
        }

        [Obsolete]
        public void AddClassTypeSource(ITypeSource typeSource)
        {
            AddTypeSource(typeSource);
        }

        public void AddTypeSource(ITypeSource typeSource, string contextName)
        {
            if (contextName == null)
                contextName = DEFAULT_CONTEXT;

            if (!_contexts.ContainsKey(contextName))
            {
                _contexts.Add(contextName, CreateContext());
            }
            _contexts[contextName].AddTypeSource(typeSource);
        }

        [Obsolete]
        public void AddClassTypeSource(ITypeSource typeSource, string contextName)
        {
            AddTypeSource(typeSource, contextName);
        }

        public ITypeResolverContext InContext(string contextName)
        {
            if (!_contexts.ContainsKey(contextName))
            {
                throw new InvalidOperationException($"contextName '{contextName}' does not exist.");
            }

            return _contexts[contextName];
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _contexts.Values
                .SelectMany(x => x.TypeSources)
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
            return InContext(DEFAULT_CONTEXT).Get(element.AsTypeReference(), collectionFormat);
        }
    }

    public static class CanBeReferencedTypeExtensions
    {
        /// <summary>
        /// Converts <see cref="ICanBeReferencedType"/> to type of <see cref="ITypeReference"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ITypeReference AsTypeReference(this ICanBeReferencedType type)
        {
            return new ElementTypeReference(type);
        }

        private class ElementTypeReference : ITypeReference, IHasStereotypes
        {
            public ElementTypeReference(ICanBeReferencedType element, bool isNullable = false, bool isCollection = false)
            {
                Element = element;
                IsNullable = isNullable;
                IsCollection = isCollection;
            }

            public bool IsNullable { get; }
            public bool IsCollection { get; }
            public ICanBeReferencedType Element { get; }
            public IEnumerable<ITypeReference> GenericTypeParameters { get; } = new ITypeReference[0];
            public string Comment { get; } = null;
            public IEnumerable<IStereotype> Stereotypes { get; } = new List<IStereotype>();
        }
    }

    
}
