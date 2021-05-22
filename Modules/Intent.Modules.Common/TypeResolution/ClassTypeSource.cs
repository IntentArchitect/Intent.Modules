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
        private readonly ClassTypeSourceOptions _options;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
        public ICollectionFormatter CollectionFormatter => _options.CollectionFormatter;

        internal ClassTypeSource(ISoftwareFactoryExecutionContext context, string templateId, ClassTypeSourceOptions options = null)
        {
            _context = context;
            _templateId = templateId;
            _options = options ?? new ClassTypeSourceOptions();
        }

        public static ClassTypeSource Create(ISoftwareFactoryExecutionContext context, string templateId)
        {
            return new ClassTypeSource(context, templateId);
        }

        public ClassTypeSource TrackDependencies(bool track)
        {
            _options.TrackDependencies = track;
            return this;
        }

        public ClassTypeSource WithCollectionFormat(string format)
        {
            _options.CollectionFormatter = new CollectionFormatter(format);
            return this;
        }

        public ClassTypeSource WithCollectionFormatter(Func<IResolvedTypeInfo, string> formatter)
        {
            _options.CollectionFormatter = new CollectionFormatter(formatter);
            return this;
        }

        public ClassTypeSource WithCollectionFormatter(ICollectionFormatter formatter)
        {
            _options.CollectionFormatter = formatter;
            return this;
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return TryGetType(typeInfo);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IResolvedTypeInfo TryGetType(ITypeReference typeInfo)
        {
            var templateInstance = TryGetTemplateInstance(typeInfo);
            if (templateInstance == null)
            {
                return null;
            }

            if (_options.TrackDependencies)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(_templateId, typeInfo.Element));
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
            

            return templateInstance;
        }
    }

    public class ClassTypeSourceOptions
    {
        public ICollectionFormatter CollectionFormatter { get; set; }
        public bool TrackDependencies { get; set; } = true;
    }
}