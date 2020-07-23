﻿using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;
using IClassTypeSource = Intent.Modules.Common.TypeResolution.IClassTypeSource;

namespace Intent.Modules.Common.TypeScript
{
    public class TypescriptTypeSource : IClassTypeSource
    {
        private readonly Func<TypescriptTypeSource, ITypeReference, string> _execute;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();

        internal TypescriptTypeSource(Func<TypescriptTypeSource, ITypeReference, string> execute)
        {
            _execute = execute;
        }

        public static IClassTypeSource InProject(ITemplateExecutionContext outputContext, string templateId, string collectionFormat = "{0}[]")
        {
            return new TypescriptTypeSource((_this, typeInfo) =>
            {
                var typeName = _this.GetTypeName(outputContext, templateId, typeInfo);
                if (!string.IsNullOrWhiteSpace(typeName) && typeInfo.IsCollection)
                {
                    return string.Format(collectionFormat, typeName);
                }
                return typeName;
            });
        }

        public static IClassTypeSource InApplication(IApplication application, string templateId, string collectionFormat = "{0}[]")
        {
            return new TypescriptTypeSource((_this, typeInfo) =>
            {
                var typeName = _this.GetTypeName(application, templateId, typeInfo);
                if (typeInfo.IsCollection)
                {
                    return string.Format(collectionFormat, typeName);
                }
                return typeName;
            });
        }

        public string GetClassType(ITypeReference typeInfo)
        {
            return _execute(this, typeInfo);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IHasClassDetails GetTemplateInstance(ITemplateExecutionContext outputContext, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = outputContext.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel(templateId, typeInfo.Element));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(templateId, typeInfo.Element));
            }

            return templateInstance;
        }

        private IHasClassDetails GetTemplateInstance(IApplication application, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel(templateId, typeInfo.Element));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel(templateId, typeInfo.Element));
            }

            return templateInstance;
        }

        private string GetTypeName(ITemplateExecutionContext outputContext, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = GetTemplateInstance(outputContext, templateId, typeInfo);

            return templateInstance != null ? (string.IsNullOrWhiteSpace(templateInstance.Namespace) ? "" : templateInstance.Namespace + ".") +
                templateInstance.ClassName + (typeInfo.GenericTypeParameters.Any() 
                    ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(outputContext, templateId, x)))}>" 
                    : "") 
                : null;
        }

        private string GetTypeName(IApplication application, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = GetTemplateInstance(application, templateId, typeInfo);

            return templateInstance != null ? (string.IsNullOrWhiteSpace(templateInstance.Namespace) ? "" : templateInstance.Namespace + ".") +
                                              templateInstance.ClassName + (typeInfo.GenericTypeParameters.Any()
                                                  ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(application, templateId, x)))}>"
                                                  : "")
                : null;
        }
    }
}
