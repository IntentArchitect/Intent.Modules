using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;
using IClassTypeSource = Intent.Modules.Common.TypeResolution.IClassTypeSource;

namespace Intent.Modules.Common.Templates
{
    public class CSharpTypeSource : IClassTypeSource
    {
        private readonly Func<ITypeReference, CSharpTypeSource, string> _execute;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();

        internal CSharpTypeSource(Func<ITypeReference, CSharpTypeSource, string> execute)
        {
            _execute = execute;
        }

        public static IClassTypeSource InProject(IProject project, string templateId, string collectionFormat = "IEnumerable<{0}>")
        {
            return new CSharpTypeSource((typeInfo, _this) =>
            {
                var typeName = _this.GetTypeName(project, templateId, typeInfo);
                if (!string.IsNullOrWhiteSpace(typeName) && typeInfo.IsCollection)
                {
                    return string.Format(collectionFormat, typeName);;
                }
                return typeName;
            });
        }

        public static IClassTypeSource InApplication(IApplication application, string templateId, string collectionFormat = "IEnumerable<{0}>")
        {
            return new CSharpTypeSource((typeInfo, _this) =>
            {
                var typeName = _this.GetTypeName(application, templateId, typeInfo);
                if (!string.IsNullOrWhiteSpace(typeName) && typeInfo.IsCollection)
                {
                    return string.Format(collectionFormat, typeName); ;
                }
                return typeName;
            });
        }

        private string GetTypeName(IProject project, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = GetTemplateInstance(project, templateId, typeInfo);

            return templateInstance != null ? 
                templateInstance.FullTypeName() + (typeInfo.GenericTypeParameters.Any() 
                    ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(project, templateId, x)))}>" 
                    : "") 
                : null;
        }

        private string GetTypeName(IApplication application, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = GetTemplateInstance(application, templateId, typeInfo);

            return templateInstance != null ?
                templateInstance.FullTypeName() + (typeInfo.GenericTypeParameters.Any()
                    ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(application, templateId, x)))}>"
                    : "")
                : null;
        }

        public string GetClassType(ITypeReference typeInfo)
        {
            return _execute(typeInfo, this);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IHasClassDetails GetTemplateInstance(IProject project, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = project.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id));
            }

            return templateInstance;
        }

        private IHasClassDetails GetTemplateInstance(IApplication application, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id));
            }

            return templateInstance;
        }
    }
}
