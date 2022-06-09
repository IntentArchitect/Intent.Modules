using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public class ClassTypeSource : ITypeSource
    {
        /// <summary>
        /// Delegate which returns a <see cref="ICollectionFormatter"/> from the provided
        /// <paramref name="collectionFormat"/>.
        /// </summary>
        public delegate ICollectionFormatter StringToCollectionFormatterFactory(string collectionFormat);

        protected readonly ISoftwareFactoryExecutionContext Context;
        protected readonly string TemplateId;
        private readonly StringToCollectionFormatterFactory _stringToCollectionFormatterFactory;
        protected readonly ClassTypeSourceOptions Options;
        protected readonly List<ITemplateDependency> TemplateDependencies = new List<ITemplateDependency>();
        public ICollectionFormatter CollectionFormatter => Options.CollectionFormatter;
        public INullableFormatter NullableFormatter => Options.NullableFormatter;

        protected ClassTypeSource(
            ISoftwareFactoryExecutionContext context,
            string templateId,
            ClassTypeSourceOptions options = null,
            StringToCollectionFormatterFactory stringToCollectionFormatterFactory = null)
        {
            Context = context;
            TemplateId = templateId;
            _stringToCollectionFormatterFactory = stringToCollectionFormatterFactory ??
                                                  TypeResolution.CollectionFormatter.GetOrCreate;
            Options = options ?? new ClassTypeSourceOptions();
        }

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
            return this;
        }

        public ClassTypeSource WithCollectionFormatter(ICollectionFormatter formatter)
        {
            Options.CollectionFormatter = formatter;
            return this;
        }

        public ClassTypeSource WithNullFormatter(INullableFormatter formatter)
        {
            Options.NullableFormatter = formatter;
            return this;
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return TryGetType(typeInfo);
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

            var templateInstance = Context.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(TemplateId, typeInfo.Element)) ??
                TemplateRoleRegistry.FindTemplateInstanceForRole(TemplateId, typeInfo.Element) as IClassProvider;

            return templateInstance;
        }

        protected virtual IEnumerable<ITemplateDependency> GetTemplateDependencies(ITypeReference typeReference,
            IClassProvider templateInstance)
        {
            return new[] { TemplateDependency.OnTemplate(templateInstance) };
        }

        private IResolvedTypeInfo TryGetType(ITypeReference typeReference)
        {
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
    }

    public class ClassTypeSourceOptions
    {
        public ICollectionFormatter CollectionFormatter { get; set; }
        public INullableFormatter NullableFormatter { get; set; }
        public bool TrackDependencies { get; set; } = true;
    }
}