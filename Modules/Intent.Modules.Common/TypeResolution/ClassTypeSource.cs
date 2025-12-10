#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public class ClassTypeSource : ITypeSource, ICanUseDefaultFormatters
    {
        /// <summary>
        /// Delegate which returns a <see cref="ICollectionFormatter"/> from the provided
        /// <paramref name="collectionFormat"/>.
        /// </summary>
        public delegate ICollectionFormatter StringToCollectionFormatterFactory(string collectionFormat);

        private readonly StringToCollectionFormatterFactory _stringToCollectionFormatterFactory;
        protected readonly ISoftwareFactoryExecutionContext Context;
        protected readonly string TemplateId;
        protected readonly ClassTypeSourceOptions Options;
        protected readonly List<ITemplateDependency> TemplateDependencies = new();
        private bool _hasCollectionFormatterSet;
        private bool _hasNullableFormatterSet;

        protected ClassTypeSource(
            ISoftwareFactoryExecutionContext context,
            string templateId,
            ClassTypeSourceOptions options = null,
            StringToCollectionFormatterFactory stringToCollectionFormatterFactory = null)
        {
            Context = context;
            TemplateId = templateId;
            _stringToCollectionFormatterFactory = stringToCollectionFormatterFactory ??
                                                  TypeResolution.CollectionFormatter.Create;
            Options = options ?? new ClassTypeSourceOptions();
        }

        public ICollectionFormatter CollectionFormatter => Options.CollectionFormatter;

        public INullableFormatter NullableFormatter => Options.NullableFormatter;

        string? ITypeSource.TemplateId => TemplateId;

        public static ClassTypeSource Create(
            ISoftwareFactoryExecutionContext context,
            string templateId,
            StringToCollectionFormatterFactory stringToCollectionFormatterFactory = null)
        {
            return new ClassTypeSource(
                context: context,
                templateId: templateId,
                stringToCollectionFormatterFactory: stringToCollectionFormatterFactory);
        }

        public ClassTypeSource TrackDependencies(bool track)
        {
            Options.TrackDependencies = track;
            return this;
        }

        public ClassTypeSource WithCollectionFormat(string format)
        {
            Options.CollectionFormatter = _stringToCollectionFormatterFactory(format);
            _hasCollectionFormatterSet = true;
            return this;
        }

        public ClassTypeSource WithCollectionFormatter(ICollectionFormatter formatter)
        {
            Options.CollectionFormatter = formatter;
            _hasCollectionFormatterSet = true;
            return this;
        }

        public ClassTypeSource WithNullableFormatter(INullableFormatter formatter)
        {
            Options.NullableFormatter = formatter;
            _hasNullableFormatterSet = true;
            return this;
        }

        /// <summary>
        /// Obsolete. Use <see cref="WithNullableFormatter"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public ClassTypeSource WithNullFormatter(INullableFormatter formatter) => WithNullableFormatter(formatter);

        public IResolvedTypeInfo GetType(ITypeReference typeReference)
        {
            if (!_hasCollectionFormatterSet)
            {
                throw new Exception("CollectionFormatter has not been set.");
            }

            if (!_hasNullableFormatterSet)
            {
                throw new Exception("NullableFormatter has not been set.");
            }

            return TryGetType(typeReference);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return TemplateDependencies;
        }

        protected virtual ResolvedTypeInfo CreateResolvedTypeInfo(ITypeReference typeReference, IClassProvider templateInstance)
        {
            return ResolvedTypeInfo.Create(
                name: templateInstance.ClassName,
                isPrimitive: false,
                isNullable: typeReference.IsNullable,
                isCollection: typeReference.IsCollection,
                typeReference: typeReference,
                template: templateInstance,
                nullableFormatter: NullableFormatter,
                collectionFormatter: CollectionFormatter);
        }

        protected virtual IClassProvider TryGetTemplateInstance(ITypeReference typeInfo)
        {
            if (typeInfo.Element == null)
            {
                return null;
            }

            var templateInstance = Context.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(TemplateId, typeInfo.Element));

            return templateInstance;
        }

        protected virtual IEnumerable<ITemplateDependency> GetTemplateDependencies(ITypeReference typeReference,
            IClassProvider templateInstance)
        {
            return new[] { TemplateDependency.OnTemplate(templateInstance) };
        }

        /// <inheritdoc/>
        public bool TryGetTypeReference(string typeName, [NotNullWhen(true)] out ITypeNameTypeReference? typeReference)
        {
            var templateInstances = Context.FindTemplateInstances<IClassProvider>(TemplateId);
            var match = templateInstances.FirstOrDefault(x => x.ClassName == typeName);
            if (match is not ITemplateWithModel { Model: IElementWrapper elementWrapper } ||
                elementWrapper.InternalElement is null)
            {
                typeReference = null;
                return false;
            }

            typeReference = new TypeNameTypeReference
            {
                IsNullable = false,
                IsCollection = false,
                GenericTypeParameters = [],
                Element = elementWrapper.InternalElement
            };
            return true;
        }

        private IResolvedTypeInfo TryGetType(ITypeReference typeReference)
        {
            if (typeReference.Element == null)
            {
                return null;
            }

            var registryInstance = TemplateInstanceRegistry.GetTypeInfo(TemplateId, typeReference.Element.Id, null, false);
            if (registryInstance != null)
            {
                return registryInstance;
            }

            var templateInstance = TryGetTemplateInstance(typeReference);
            if (templateInstance == null)
            {
                return null;
            }

            if (Options.TrackDependencies)
            {
                TemplateDependencies.AddRange(GetTemplateDependencies(typeReference, templateInstance));
            }

            return CreateResolvedTypeInfo(typeReference, templateInstance);
        }

        void ICanUseDefaultFormatters.SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter)
        {
            if (_hasCollectionFormatterSet)
            {
                return;
            }

            WithCollectionFormatter(collectionFormatter);
        }

        void ICanUseDefaultFormatters.SetDefaultNullableFormatter(INullableFormatter nullableFormatter)
        {
            if (_hasNullableFormatterSet)
            {
                return;
            }

            WithNullableFormatter(nullableFormatter);
        }
    }

    public class ClassTypeSourceOptions
    {
        public ICollectionFormatter CollectionFormatter { get; set; }
        public INullableFormatter NullableFormatter { get; set; }
        public bool TrackDependencies { get; set; } = true;
    }
}