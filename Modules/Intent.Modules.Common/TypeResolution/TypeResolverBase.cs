﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public class TypeResolverContext : ITypeResolverContext
    {
        private readonly List<ITypeSource> _classTypeSources;
        private readonly Func<ITypeReference, string, string> _resolveTypeFunc;

        public TypeResolverContext(List<ITypeSource> classTypeSources, Func<ITypeReference, string, string> resolveTypeFunc)
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
                var foundClass = classLookup.GetType(typeInfo);
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
        private readonly IDictionary<string, List<ITypeSource>> _classTypeSources;

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

        protected abstract string ResolveType(ITypeReference typeInfo, string collectionFormat = null);
    }
}
