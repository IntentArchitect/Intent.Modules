using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public class ClassTypeSource : ITypeSource
    {
        private readonly ISoftwareFactoryExecutionContext _context;
        private readonly string _templateId;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
        public ICollectionFormatter CollectionFormatter { get; }

        internal ClassTypeSource(ISoftwareFactoryExecutionContext context, string templateId, ICollectionFormatter collectionFormatter)
        {
            _context = context;
            _templateId = templateId;
            CollectionFormatter = collectionFormatter;
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, ICollectionFormatter collectionFormatter)
        {
            return new ClassTypeSource(context, templateId, collectionFormatter);
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return GetTypeName(typeInfo);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IResolvedTypeInfo GetTypeName(ITypeReference typeInfo)
        {
            var templateInstance = TryGetTemplateInstance(typeInfo);
            if (templateInstance == null)
            {
                return null;
            }

            return new ResolvedTypeInfo(templateInstance.ClassName, false, templateInstance);
        }

        private IClassProvider TryGetTemplateInstance(ITypeReference typeInfo)
        {
            if (typeInfo.Element == null)
            {
                return null;
            }
            var templateInstance = _context.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(_templateId, typeInfo.Element));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(_templateId, typeInfo.Element));
            }

            return templateInstance;
        }
    }
}