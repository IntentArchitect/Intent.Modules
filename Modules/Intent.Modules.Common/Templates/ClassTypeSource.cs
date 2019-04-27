using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using IApplication = Intent.Engine.IApplication;
using IClassTypeSource = Intent.Modules.Common.TypeResolution.IClassTypeSource;

namespace Intent.Modules.Common.Templates
{
    public class ClassTypeSource : IClassTypeSource
    {
        private readonly Func<ITypeReference, string> _execute;

        internal ClassTypeSource(Func<ITypeReference, string> execute)
        {
            _execute = execute;
        }

        public static IClassTypeSource InProject(IProject project, string templateId, string collectionType = nameof(IEnumerable))
        {
            return new ClassTypeSource((typeInfo) =>
            {
                var typeName = FullTypeNameInProject(project, templateId, typeInfo);
                if (!string.IsNullOrWhiteSpace(typeName) && typeInfo.IsCollection)
                {
                    return $"{collectionType}<{typeName}>";
                }
                return typeName;
            });
        }

        private static string FullTypeNameInProject(IProject project, string templateId, ITypeReference typeInfo)
        {
            // Hack for bug in 1.4:
            var associationEnd = typeInfo as IAssociationEnd;
            if (associationEnd != null && associationEnd.Id == associationEnd.Association.Id)
            {
                return project.FindTemplateInstance<IHasClassDetails>(
                        TemplateDependency.OnModel<IMetaModel>(templateId, (x) => x.Id == associationEnd.Class.Id))
                    ?.FullTypeName();
            }

            var templateInstance = project.FindTemplateInstance<IHasClassDetails>(
                TemplateDependency.OnModel<IMetaModel>(templateId, (x) => x.Id == typeInfo.Id));
            return templateInstance != null ? templateInstance.FullTypeName() 
                + (typeInfo.GenericTypeParameters.Any() ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => FullTypeNameInProject(project, templateId, x)))}>" : "") 
                : null;
        }

        public static IClassTypeSource InApplication(IApplication application, string templateId, string collectionType = nameof(IEnumerable))
        {
            return new ClassTypeSource((typeInfo) =>
            {
                if (typeInfo.IsCollection)
                {
                    return $"{collectionType}<{FullTypeNameInApplication(application, templateId, typeInfo)}>";
                }
                return FullTypeNameInApplication(application, templateId, typeInfo);
            });
        }

        private static string FullTypeNameInApplication(IApplication application, string templateId, ITypeReference typeInfo)
        {
            return application.FindTemplateInstance<IHasClassDetails>(
                    TemplateDependency.OnModel<IMetaModel>(templateId, (x) => x.Id == typeInfo.Id))
                ?.FullTypeName();
        }

        public string GetClassType(ITypeReference typeInfo)
        {
            return _execute(typeInfo);
        }
    }
}
