using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.TypeScript
{
    public class TypescriptTypeSource : ITypeSource
    {
        private readonly Func<TypescriptTypeSource, ITypeReference, IResolvedTypeInfo> _execute;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();

        internal TypescriptTypeSource(Func<TypescriptTypeSource, ITypeReference, IResolvedTypeInfo> execute)
        {
            _execute = execute;
        }

        public static ITypeSource InProject(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat = "{0}[]")
        {
            return new TypescriptTypeSource((_this, typeInfo) =>
            {
                var typeName = _this.GetTypeName(context, templateId, typeInfo, collectionFormat);

                return typeName;
            });
        }

        //public static ITypeSource InApplication(IApplication application, string templateId, string collectionFormat = "{0}[]")
        //{
        //    return new TypescriptTypeSource((_this, typeInfo) =>
        //    {
        //        var typeName = _this.GetTypeName(application, templateId, typeInfo);
        //        if (typeInfo.IsCollection)
        //        {
        //            return string.Format(collectionFormat, typeName);
        //        }
        //        return typeName;
        //    });
        //}

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return _execute(this, typeInfo);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IHasClassDetails GetTemplateInstance(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = context.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel(templateId, typeInfo.Element));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(templateId, typeInfo.Element));
            }

            return templateInstance;
        }

        //private IHasClassDetails GetTemplateInstance(IApplication application, string templateId, ITypeReference typeInfo)
        //{
        //    var templateInstance = application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel(templateId, typeInfo.Element));
        //    if (templateInstance != null)
        //    {
        //        _templateDependencies.Add(TemplateDependency.OnModel(templateId, typeInfo.Element));
        //    }

        //    return templateInstance;
        //}

        private IResolvedTypeInfo GetTypeName(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo, string collectionFormat)
        {
            var templateInstance = GetTemplateInstance(context, templateId, typeInfo);
            if (templateInstance == null)
            {
                return null;
            }
            var name = (string.IsNullOrWhiteSpace(templateInstance.Namespace) ? "" : templateInstance.Namespace + ".") +
                templateInstance.ClassName + (typeInfo.GenericTypeParameters.Any() 
                    ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(context, templateId, x, collectionFormat).Name))}>" 
                    : "");

            if (!string.IsNullOrWhiteSpace(name) && typeInfo.IsCollection)
            {
                name = string.Format(collectionFormat, name);
            }

            return new ResolvedTypeInfo(name, false, templateInstance);
        }

        //private string GetTypeName(IApplication application, string templateId, ITypeReference typeInfo)
        //{
        //    var templateInstance = GetTemplateInstance(application, templateId, typeInfo);

        //    return templateInstance != null ? (string.IsNullOrWhiteSpace(templateInstance.Namespace) ? "" : templateInstance.Namespace + ".") +
        //                                      templateInstance.ClassName + (typeInfo.GenericTypeParameters.Any()
        //                                          ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(application, templateId, x)))}>"
        //                                          : "")
        //        : null;
        //}
    }
}
