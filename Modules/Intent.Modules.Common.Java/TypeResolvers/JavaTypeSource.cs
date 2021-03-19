using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Java.TypeResolvers
{
    public class JavaTypeSource : ITypeSource
    {
        private readonly Func<JavaTypeSource, ITypeReference, IResolvedTypeInfo> _execute;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();

        internal JavaTypeSource(Func<JavaTypeSource, ITypeReference, IResolvedTypeInfo> execute)
        {
            _execute = execute;
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat = "{0}[]")
        {
            return JavaTypeSource.Create(context, templateId, (type) => string.Format(collectionFormat, type));
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, Func<string, string> formatCollection)
        {
            return new JavaTypeSource((_this, typeInfo) =>
            {
                var typeName = _this.GetTypeName(context, templateId, typeInfo, formatCollection);

                return typeName;
            });
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return _execute(this, typeInfo);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IResolvedTypeInfo GetTypeName(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo, Func<string, string> formatCollection)
        {
            var templateInstance = GetTemplateInstance(context, templateId, typeInfo);
            if (templateInstance == null)
            {
                return null;
            }
            var name = templateInstance.ClassName + (typeInfo.GenericTypeParameters.Any() 
                           ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(context, templateId, x, formatCollection).Name))}>" 
                           : "");

            if (!string.IsNullOrWhiteSpace(name) && typeInfo.IsCollection)
            {
                name = formatCollection(name);
            }

            return new ResolvedTypeInfo(name, false, templateInstance);
        }

        private IClassProvider GetTemplateInstance(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo)
        {
            if (typeInfo.Element == null)
            {
                return null;
            }
            var templateInstance = context.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(templateId, typeInfo.Element));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(templateId, typeInfo.Element));
            }

            return templateInstance;
        }
    }
}
